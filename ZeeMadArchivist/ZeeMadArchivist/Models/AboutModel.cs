using System;
using System.Reflection;

namespace CyberFeedForward.TheMadArchivist.Models;

public sealed class AboutModel
{
    public string ApplicationName { get; init; } = string.Empty;

    public string Version { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public static AboutModel CreateDefault()
    {
        var assembly = typeof(AboutModel).Assembly;

        var name = assembly.GetName().Name ?? "ZeeMadArchivist";
        var version = assembly.GetName().Version?.ToString() ?? string.Empty;

        var description = assembly
            .GetCustomAttribute<AssemblyDescriptionAttribute>()
            ?.Description
            ?? string.Empty;

        return new AboutModel
        {
            ApplicationName = name,
            Version = string.IsNullOrWhiteSpace(version) ? "" : $"Version {version}",
            Description = description,
        };
    }
}
