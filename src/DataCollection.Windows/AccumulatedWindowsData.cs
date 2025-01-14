using DataCollection.Common;
using System;

namespace DataCollection.Windows;

public class AccumulatedWindowsData(
    string windowTitle,
    string processFileName,
    string processFriendlyName,
    TimeSpan totalTime) : IAccumulatedData
{
    public string WindowTitle { get; } = windowTitle;
    public string ProcessFileName { get; } = processFileName;
    public string ProcessFriendlyName { get; } = processFriendlyName;
    public TimeSpan TotalTime { get; } = totalTime;

    public override bool Equals(object? obj)
    {
        if (obj is not AccumulatedWindowsData other)
        {
            return false;
        }

        return other.WindowTitle.Equals(WindowTitle, StringComparison.InvariantCultureIgnoreCase) &&
               other.ProcessFileName.Equals(ProcessFileName, StringComparison.InvariantCultureIgnoreCase) &&
               other.ProcessFriendlyName.Equals(ProcessFileName);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(WindowTitle.GetHashCode(), ProcessFileName.GetHashCode(), ProcessFriendlyName.GetHashCode());
    }
}