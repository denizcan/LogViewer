using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogViewer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LogViewer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    JsonSerializerOptions _jsonOptions;

    [ObservableProperty]
    private string _title = "LogViewer";

    [ObservableProperty]
    private ObservableCollection<LogEntryModel> _entries;

    [ObservableProperty]
    LogEntryModel? _selectedEntry;

    [ObservableProperty]
    string? _selectedEntryStateText;

    public MainWindowViewModel()
    {
        _jsonOptions = new()
        {
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        if (Avalonia.Controls.Design.IsDesignMode)
            _entries =
            [
                new LogEntryModel()
                {
                    Timestamp = new DateTime(2020, 10, 5, 17, 30, 01),
                    EventId = 1,
                    LogLevel = LogLevel.Debug,
                    Category = "Assembly.Namespace.ClassName",
                    Message = "Entry 1",
                },
                new LogEntryModel()
                {
                    Timestamp = new DateTime(2020, 10, 5, 18, 20, 01),
                    EventId = 1,
                    LogLevel = LogLevel.Information,
                    Category = "Assembly.Namespace.ClassName1",
                    Message = "Entry 2",
                }
            ];
        else
            _entries = [];
    }
    
    public void LoadFile(string path)
    {
        var reader = new StreamReader(path);
        ObservableCollection<LogEntryModel> entries = [];

        while (reader.EndOfStream == false)
        {
            var line = reader.ReadLine();
            var entry = JsonSerializer.Deserialize<LogEntryModel>(line!, _jsonOptions);
            entries.Add(entry!);

        }
        Entries = entries;
        Title = $"LogViewer - {path}";
    }
    [RelayCommand]
    public async Task Open()   
    {
        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var files = await desktop!.MainWindow!.StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title = "Open Text File",
                    AllowMultiple = false,
                    FileTypeFilter = [
                        new FilePickerFileType("Log files")
                        {
                            Patterns = new[] { "*.log" }
                        }]
                });
            if (files.Count > 0)
                LoadFile(files[0].TryGetLocalPath()!);
        }
    }

    [RelayCommand]
    public void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    //private Regex _exceptionRegex = new Regex(@"^\s*at\s*(?<method>.*)\s*in.*\\(?<file>.*):line\s?(?<lineNumber>\d*)");
    private Regex _exceptionRegex = new Regex(@"^\s*at.*\.(?<method>.*\..*\(.*\))\s*in.*\\(?<file>.*):line\s?(?<lineNumber>\d*)");
    private Regex _filenameRegex = new Regex(@".*\\(?<fileName>.*\.cs)");

    partial void OnSelectedEntryChanged(LogEntryModel? value)
    {
        var sb = new StringBuilder();
        var exception = value?.Exception;

        if (exception != null)
        {
            //sb.AppendLine("Exception");
            var lines = exception.Split("\r\n");
            sb.AppendLine(lines[0]);
            foreach (var line in lines)
            {
                var match = _exceptionRegex.Match(line); 
                if (match.Success)
                    sb.AppendLine($"    {match.Groups["method"]}   {match.Groups["file"]}   {match.Groups["lineNumber"]}: ");
            }

            sb.AppendLine();
            sb.AppendLine("Raw Exception:");
            sb.AppendLine(exception);
            sb.AppendLine();
        }

        var state = value?.State;
        if (state != null)
        {
            sb.AppendLine("State:");
            sb.Append(JsonSerializer.Serialize(state, _jsonOptions));
        }

        SelectedEntryStateText = sb.ToString();
    }
}