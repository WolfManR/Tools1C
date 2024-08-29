using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Tools1C;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        PlatformsView.SaveState();
        ConfigurationsView.SaveState();
    }

    private async void btnRelaunchAsAdmin_Click(object sender, RoutedEventArgs e)
    {
        ProcessStartInfo processInfo = new()
        {
            UseShellExecute = true,
            Verb = "runas",
            WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
            FileName = "Tools1C.exe"
        };
        try
        {
            Process.Start(processInfo);
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}
