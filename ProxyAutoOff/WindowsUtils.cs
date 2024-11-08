using System;
using Microsoft.Win32;

namespace v2rayN
{
    internal static class WindowsUtils
    {
        public static void RegWriteValue(string path, string name, object value)
        {
            RegistryKey? regKey = null;
            try
            {
                regKey = Registry.CurrentUser.CreateSubKey(path);
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    regKey?.DeleteValue(name, false);
                }
                else
                {
                    regKey?.SetValue(name, value);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                regKey?.Close();
            }
        }

    }
}