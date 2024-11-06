using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia;
using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using LogViewer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LogViewer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    JsonSerializerOptions _jsonOptions;

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
    }

    partial void OnSelectedEntryChanged(LogEntryModel? value)
    {
        var state = value?.State;
        if (state != null)
            SelectedEntryStateText = JsonSerializer.Serialize(state, _jsonOptions);
        else
            SelectedEntryStateText = string.Empty;

    }
}