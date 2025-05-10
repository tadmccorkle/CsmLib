// Copyright (c) 2025 by Tad McCorkle
// Licensed under the MIT license.

using System.Windows;
using System.Windows.Media;

namespace CsmLib.Wpf;

public static class VisualTreeUtilities
{
    public static T? FindVisualParent<T>(this DependencyObject obj)
        where T : DependencyObject
    {
        var parent = obj;
        do
        {
            parent = VisualTreeHelper.GetParent(parent);
            if (parent is T found)
                return found;
        } while (parent is not null);

        return null;
    }

    public static T? FindVisualParent<T>(this DependencyObject obj, string name)
        where T : FrameworkElement
    {
        var parent = obj;
        do
        {
            parent = VisualTreeHelper.GetParent(parent);
            if (parent is T found && found.Name == name)
                return found;
        } while (parent is not null);

        return null;
    }

    public static T? FindVisualChild<T>(this DependencyObject obj)
        where T : DependencyObject
    {
        var childCount = VisualTreeHelper.GetChildrenCount(obj);

        for (int i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            if (child is T found)
            {
                return found;
            }
            else
            {
                var descendent = FindVisualChild<T>(child);
                if (descendent is not null)
                    return descendent;
            }
        }

        return null;
    }

    public static T? FindVisualChild<T>(this DependencyObject obj, string name)
        where T : FrameworkElement
    {
        var childCount = VisualTreeHelper.GetChildrenCount(obj);

        for (int i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            if (child is T found && found.Name == name)
            {
                return found;
            }
            else
            {
                var descendent = FindVisualChild<T>(child, name);
                if (descendent is not null)
                    return descendent;
            }
        }

        return null;
    }
}
