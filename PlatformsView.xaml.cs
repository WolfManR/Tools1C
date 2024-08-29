using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Tools1C;

public partial class PlatformsView : UserControl
{
    public PlatformsView()
    {
        InitializeComponent();
        if (Properties.Settings.Default.RootPaths is { Count: > 0 } stored)
        {
            foreach (var rootPath in stored)
            {
                if (rootPath is null) continue;
                lbRootPaths.Items.Add(new RootPath(rootPath, Directory.Exists(rootPath)));
            }
        }
    }

    public void SaveState()
    {
        if (Properties.Settings.Default.RootPaths is { } collection)
        {
            collection.Clear();
        }


        foreach (RootPath item in lbRootPaths.Items)
        {
            Properties.Settings.Default.RootPaths.Add(item.Path);
        }

        Properties.Settings.Default.Save();
    }

    

    private void btnAddRootPath_Click(object sender, RoutedEventArgs e)
    {
        var rootPath = tbRootPath.Text;
        lbRootPaths.Items.Add(new RootPath(rootPath, Directory.Exists(rootPath)));
    }

    private void lbRootPaths_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems is not [RootPath { Accessable: true } first, ..]) return;
        lvPlatforms.Items.Clear();
        var directory = new DirectoryInfo(first.Path);
        var directories = directory.GetDirectories("*", SearchOption.TopDirectoryOnly);
        foreach (var d in directories)
        {
            if (d.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).FirstOrDefault(x => x.Name == "bin") is not { } binDir) continue;
            lvPlatforms.Items.Add(new Platform(d.Name, binDir.FullName));
        }
    }

    private async void btnRegCOM_Click(object sender, RoutedEventArgs e)
    {
        if (e.Source is not Button { DataContext: Platform { BinPath: { } binPath } }) return;
        var file = Path.Combine(binPath, "comcntr.dll");
        if (File.Exists(file))
        {
            ProcessStartInfo processInfo = new()
            {
                UseShellExecute = true,
                Verb = "runas",
                Arguments = $"\"{file}\"",
                FileName = "regsvr32"
            };
            try
            {
                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    private async void btnOpen_Click(object sender, RoutedEventArgs e)
    {
        if (e.Source is not Button { DataContext: Platform { BinPath: { } binPath } }) return;
        var file = Path.Combine(binPath, "1cv8.exe");
        if (File.Exists(file))
        { 
            Process.Start(file);
        }
    }
}

public record Platform(string Version, string BinPath);

public record RootPath(string Path, bool Accessable);
