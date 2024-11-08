using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using H.NotifyIcon;
using Microsoft.Win32;
using v2rayN.Common;

internal class Program
{
    private static readonly string _regRun = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private static readonly string _runName = "ProxyAutoOff";

    [STAThread]
    private static void Main(string[] args)
    {
        var app = new System.Windows.Application();
        app.SessionEnding += App_SessionEnding;
        app.Startup += App_Startup;

        app.Run();
    }

    private static void App_Startup(object sender, StartupEventArgs e)
    {
        Window window = null!;
        MenuItem startupMenuItem = new MenuItem() { Header = "Startup", IsCheckable = true };

        switch (GetStartup())
        {
            case null:
                SetStartup(true);
                startupMenuItem.IsChecked = true;
                break;
            case true:
                startupMenuItem.IsChecked = true;
                break;
            case false:
                startupMenuItem.IsChecked = false;
                break;
        }
        startupMenuItem.Checked += (_, _) => SetStartup(true);
        startupMenuItem.Unchecked += (_, _) => SetStartup(false);

        window = new Window()
        {
            Content = new TaskbarIcon()
            {
                IconSource = new BitmapImage(new Uri("pack://application:,,,/App.ico")),
                ContextMenu = new ContextMenu()
                {
                    Items =
                    {
                        startupMenuItem,
                        new MenuItem() { Header = "Exit", Command = new RelayCommand(() => window.Close()) },
                    }
                }
            },
            Opacity = 0,
            Width = 0,
            Height = 0,
            WindowStyle = WindowStyle.None,
            ShowInTaskbar = false,
        };
        window.Show();
        window.Hide();
    }

    private static void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
    {
        ProxySetting.UnsetProxy();
    }

    private static bool? GetStartup()
    {
        var run = Registry.CurrentUser.OpenSubKey(_regRun, true);
        var val = run.GetValue(_runName);
        if (val is null)
            return null;
        return !string.IsNullOrWhiteSpace(val as string);
    }

    private static void SetStartup(bool isStartup)
    {
        var run = Registry.CurrentUser.OpenSubKey(_regRun, true);
        if (isStartup is true)
        {
            run.SetValue(_runName, System.Windows.Forms.Application.ExecutablePath);
        }
        else if(isStartup is false)
        {
            run.SetValue(_runName, string.Empty);
        }
    }
}

public class RelayCommand : ICommand
{
    private readonly Action _action;

    public event EventHandler? CanExecuteChanged;

    public RelayCommand(Action action) => _action = action;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter) => _action();
}