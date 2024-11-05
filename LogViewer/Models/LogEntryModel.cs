using System;
using System.Text.Json.Nodes;
using Avalonia.Media.TextFormatting;
using Microsoft.Extensions.Logging;

namespace LogViewer.Models;

public class LogEntryModel
{
    public DateTime Timestamp { get; set; }
    public uint EventId { get; set; }
    public string LogLevel { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public JsonObject? State { get; set; }
}