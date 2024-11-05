using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using LogViewer.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace LogViewer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static
    public string Greeting => "Welcome to Avalonia!";
#pragma warning restore CA1822 // Mark members as static

    [ObservableProperty]
    private ObservableCollection<LogEntryModel> _entries;

    public MainWindowViewModel()
    {
        if (Avalonia.Controls.Design.IsDesignMode)
            _entries =
            [
                new LogEntryModel()
                {
                    Timestamp = new DateTime(2020, 10, 5, 17, 30, 01),
                    EventId = 1,
                    LogLevel = "Debug",
                    Category = "Assembly.Namespace.ClassName",
                    Message = "Entry 1",
                },
                new LogEntryModel()
                {
                    Timestamp = new DateTime(2020, 10, 5, 18, 20, 01),
                    EventId = 1,
                    LogLevel = "Debug",
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
            var entry = JsonSerializer.Deserialize<LogEntryModel>(line);
            entries.Add(entry);
        }
        Entries = entries;
    }
}