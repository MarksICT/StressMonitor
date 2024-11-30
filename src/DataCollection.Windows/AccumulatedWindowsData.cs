using System;
using DataCollection.Common;

namespace DataCollection.Windows;

public class AccumulatedWindowsData(string windowTitle, string processFileName, TimeSpan totalTime) : IAccumulatedData
{
    public string WindowTitle { get; } = windowTitle;
    public string ProcessFileName { get; } = processFileName;
    public TimeSpan TotalTime { get; } = totalTime;

    public override bool Equals(object? obj)
    {
        if (obj is not AccumulatedWindowsData other)
        {
            return false;
        }

        return other.WindowTitle.Equals(WindowTitle, StringComparison.InvariantCultureIgnoreCase) &&
               other.ProcessFileName.Equals(ProcessFileName, StringComparison.InvariantCultureIgnoreCase);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(WindowTitle.GetHashCode(), ProcessFileName.GetHashCode());
    }
}