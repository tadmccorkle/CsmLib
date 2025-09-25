// Copyright (c) 2025 by Tad McCorkle
// Licensed under the MIT license.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CsmLib.Wpf.Behaviors;

/// <summary>
/// Attached properties for mouse wheel behaviors.
/// </summary>
public static class MouseWheelBehaviors
{
    /// <summary>
    /// Identifies the <c><see cref="MouseWheelBehaviors"/>.Bubble</c> attached property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attached property specifies if mouse wheel events should be bubbled to the target's
    /// ancestor.
    /// </para>
    /// <para>
    /// If <see langword="true"/>, mouse wheel events will be bubbled to the target's nearest
    /// <see cref="UIElement"/> ancestor. If the target is a <see cref="ScrollViewer"/>, mouse wheel
    /// events will only be bubbled when attempting to scroll beyond its scrollable area.
    /// </para>
    /// </remarks>
    public static readonly DependencyProperty BubbleProperty =
        DependencyProperty.RegisterAttached(
            "Bubble",
            typeof(bool),
            typeof(MouseWheelBehaviors),
            new PropertyMetadata(false, OnBubbleChanged));

    /// <summary>Gets a value that indicates if the target <see cref="UIElement"/> should bubble mouse wheel events.</summary>
    /// <remarks>See the <see cref="BubbleProperty"/> remarks for usage details.</remarks>
    [AttachedPropertyBrowsableForType(typeof(UIElement))]
    public static bool GetBubble(DependencyObject target)
        => (bool)target.GetValue(BubbleProperty);

    /// <summary>Sets a value that indicates if the target <see cref="UIElement"/> should bubble mouse wheel events.</summary>
    /// <remarks>See the <see cref="BubbleProperty"/> remarks for usage details.</remarks>
    public static void SetBubble(DependencyObject target, bool value)
        => target.SetValue(BubbleProperty, value);

    private static void OnBubbleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is true && e.NewValue is not true)
            ((UIElement)d).PreviewMouseWheel -= OnPreviewMouseWheel;
        else if (e.OldValue is not true && e.NewValue is true)
            ((UIElement)d).PreviewMouseWheel += OnPreviewMouseWheel;
    }

    private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (!e.Handled)
        {
            if (sender is ScrollViewer sv)
            {
                if ((e.Delta > 0 && sv.VerticalOffset == 0) || (e.Delta < 0 && sv.VerticalOffset == sv.ScrollableHeight))
                {
                    BubbleMouseWheelEvent(sv, e);
                }
            }
            else
            {
                BubbleMouseWheelEvent((UIElement)sender, e);
            }
        }
    }

    private static void BubbleMouseWheelEvent(UIElement sender, MouseWheelEventArgs e)
    {
        e.Handled = true;

        if (sender is Control { Parent: UIElement immediateParent })
        {
            immediateParent.RaiseEvent(CreateRoutedEventArgs(sender, e));
        }
        else if (sender.FindVisualParent<UIElement>() is UIElement ancestor)
        {
            ancestor.RaiseEvent(CreateRoutedEventArgs(sender, e));
        }

        static MouseWheelEventArgs CreateRoutedEventArgs(UIElement sender, MouseWheelEventArgs e)
        {
            return new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender,
            };
        }
    }
}
