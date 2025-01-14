using DataCollection.Common;

namespace DataCollection.Windows;

public class WindowsDataComparer : IDataEqualityComparer<WindowsData>
{
    public bool Equals(WindowsData? x, WindowsData? y)
    {
        if (x is null && y is null)
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.Equals(y);
    }

    public bool CategoryEquals(WindowsData? x, WindowsData? y)
    {
        if (x is null && y is null)
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.CategoryEquals(y);
    }

    public int GetHashCode(WindowsData obj)
    {
        return obj.GetHashCode();
        
    }
}
