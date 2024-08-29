using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace Tools1C;

public partial class ConfigurationsView : UserControl
{
    const string configFile = "ibases.v8i";

    public ConfigurationsView()
    {
        InitializeComponent();
        var path = @"AppData\Roaming\1C\1CEStart";
        var basePath = @"C:\Users";
        var baseDirectory = new DirectoryInfo(basePath);
        foreach (var item in baseDirectory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
        {
            var cePath = Path.Combine(basePath, item.Name, path);
            if (Directory.Exists(cePath))
            {
                lbUsers.Items.Add(new UserConfig(item.Name, cePath));
            }
        }

        foreach (UserConfig user in lbUsers.Items)
        {
            var lines = File.ReadAllLines(Path.Combine(user.ConfigFilesPath, configFile));

            ConfigurationNode? currentNode = default;

            foreach (var item in lines)
            {
                if (item.StartsWith('['))
                {
                    currentNode = new();
                    user.Configuration.Add(currentNode);

                    currentNode.Name = item[1..^1];
                    continue;
                }

                if (item is not { Length: > 0 }) continue;

                var separatorIndex = item.IndexOf('=');
                var valueName = item[..separatorIndex];
                var value = item[(separatorIndex + 1)..];
                currentNode.Values.TryAdd(valueName, value);
            }
        }

        if(lbUsers.Items.Count > 0) {
            foreach (var item in ((UserConfig)lbUsers.Items[0]).Configuration.Where(x=>x.Connection is { Length: > 0}))
            {
                lbConfigs.Items.Add(item);
            }
        }
    }

    public void SaveState()
    {

    }

    private void lbUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0 || e.AddedItems[0] is not UserConfig user) return;
        lbConfigs.Items.Clear();
        foreach (var item in user.Configuration.Where(x => x.Connection is { Length: > 0 }))
        {
            lbConfigs.Items.Add(item);
        }
    }
}

public record UserConfig(string Name, string ConfigFilesPath)
{
    public List<ConfigurationNode> Configuration { get; set; } = new();
};

public class ConfigurationNode
{
    public string Name { get; set; } = string.Empty;

    public Dictionary<string, string> Values { get; set; } = new();

    public Guid Id => Values.TryGetValue("ID", out var value) ? Guid.Parse(value) : Guid.Empty;
    public int OrderInList => Values.TryGetValue("OrderInList", out var value) ? int.Parse(value) : default;
    public string Folder => Values.TryGetValue("Folder", out var value) ? value : string.Empty;
    public int OrderInTree => Values.TryGetValue("OrderInTree", out var value) ? int.Parse(value) : default;
    public int External => Values.TryGetValue("External", out var value) ? int.Parse(value) : default;
    public string? Connection => Values.TryGetValue("Connect", out var value) ? value[(value.IndexOf('=') + 1)..] : string.Empty;
    public string? ConnectionMode => Values.TryGetValue("Connect", out var value) ? value[..value.IndexOf('=')] : string.Empty;
    public string? ClientConnectionSpeed => Values.TryGetValue("ClientConnectionSpeed", out var value) ? value : string.Empty;
    public string? App => Values.TryGetValue("App", out var value) ? value : string.Empty;
    public int WA => Values.TryGetValue("WA", out var value) ? int.Parse(value) : default;
    public string? Version => Values.TryGetValue("Version", out var value) ? value : string.Empty;
}