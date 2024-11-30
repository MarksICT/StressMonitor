﻿using DataCollection.Common;
using System;

namespace DataCollection.Windows;

public class WindowsData(string windowTitle, string processFileName, DateTimeOffset startTime, DateTimeOffset stopTime) : IData
{
    public string WindowTitle { get; } = windowTitle;
    public string ProcessFileName { get; } = processFileName;
    public DateTimeOffset StartTime { get; } = startTime;
    public DateTimeOffset StopTime { get; } = stopTime;
    
    public override bool Equals(object? other)
    {
        if (other is not WindowsData otherData)
        {
            return false;
        }

        return otherData.ProcessFileName.Equals(ProcessFileName, StringComparison.InvariantCultureIgnoreCase) &&
               otherData.WindowTitle.Equals(WindowTitle, StringComparison.InvariantCultureIgnoreCase);
    }

    bool IData.CategoryEquals(IData? other)
    {
        return other is WindowsData otherData && CategoryEquals(otherData);
    }

    public bool CategoryEquals(WindowsData? other)
    {
        if (other is null)
        {
            return false;
        }

        return other.ProcessFileName.Equals(ProcessFileName, StringComparison.InvariantCultureIgnoreCase) &&
               other.WindowTitle.Equals(WindowTitle, StringComparison.InvariantCultureIgnoreCase);
    }

    public override int GetHashCode()
    {
        return ProcessFileName.GetHashCode() * WindowTitle.GetHashCode();
    }
}