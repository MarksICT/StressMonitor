using DataCollection.Common;
using DataCollection.Persistence.Entities;
using System;

namespace DataCollection.Windows;

public class WindowsData(
    string windowTitle,
    string processFileName,
    string processFriendlyName,
    DateTimeOffset startTime,
    DateTimeOffset stopTime) : IData
{
    public string WindowTitle { get; } = windowTitle;
    public string ProcessFileName { get; } = processFileName;
    public string ProcessFriendlyName { get; } = processFriendlyName;
    public DateTimeOffset StartTime { get; } = startTime;
    public DateTimeOffset StopTime { get; } = stopTime;
    
    public override bool Equals(object? other)
    {
        if (other is WindowsDataEntity windowsDataEntity)
        {
            return (windowsDataEntity.ProcessFileName?.Equals(ProcessFileName, StringComparison.InvariantCultureIgnoreCase) ?? false) &&
                   (windowsDataEntity.ProcessFriendlyName?.Equals(ProcessFriendlyName, StringComparison.InvariantCultureIgnoreCase) ?? false) &&
                   (windowsDataEntity.WindowTitle?.Equals(WindowTitle, StringComparison.InvariantCultureIgnoreCase) ?? false) &&
                   windowsDataEntity.StartTime.Equals(StartTime) && windowsDataEntity.StopTime.Equals(StopTime);
        }

        if (other is not WindowsData otherData)
        {
            return false;
        }

        return otherData.ProcessFileName.Equals(ProcessFileName, StringComparison.InvariantCultureIgnoreCase) &&
               otherData.ProcessFriendlyName.Equals(ProcessFriendlyName, StringComparison.InvariantCultureIgnoreCase) &&
               otherData.WindowTitle.Equals(WindowTitle, StringComparison.InvariantCultureIgnoreCase) &&
               otherData.StartTime.Equals(StartTime) && otherData.StopTime.Equals(StopTime);
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
               other.ProcessFriendlyName.Equals(ProcessFriendlyName, StringComparison.InvariantCultureIgnoreCase);
    }

    public override int GetHashCode()
    {
        return ProcessFileName.GetHashCode() * WindowTitle.GetHashCode();
    }
}