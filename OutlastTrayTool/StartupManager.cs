using Microsoft.Win32;
using System;
using System.Windows.Forms;

public static class StartupManager
{
    private const string AppName = "Lathe";

    public static void EnableStartup()
    {
        string exePath = Application.ExecutablePath;

        using RegistryKey key = Registry.CurrentUser.OpenSubKey(
            @"Software\Microsoft\Windows\CurrentVersion\Run",
            true
        );

        key.SetValue(AppName, $"\"{exePath}\" --startup");
    }

    public static void DisableStartup()
    {
        using RegistryKey key = Registry.CurrentUser.OpenSubKey(
            @"Software\Microsoft\Windows\CurrentVersion\Run",
            true
        );

        key.DeleteValue(AppName, false);
    }
}