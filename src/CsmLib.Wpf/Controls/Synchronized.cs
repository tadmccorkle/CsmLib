// Copyright (c) 2025 by Tad McCorkle
// Licensed under the MIT license.

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CsmLib.Synchronization;

namespace CsmLib.Wpf.Controls;

/// <summary>
/// Attached properties used to set up synchronization on the UI thread.
/// </summary>
public static class Synchronized
{
    /// <summary>
    /// Sets an <see cref="ItemsControl"/>'s items source and sets up synchronized collection access
    /// using a simple locking mechanism to allow the collection to be used on multiple threads.
    /// </summary>
    /// <remarks>
    /// The value can either be an <see cref="IEnumerable"/> that implements
    /// <see cref="ISynchronized"/> or a <see cref="ICollectionView"/> whose
    /// <see cref="ICollectionView.SourceCollection"/> meets this constraint.
    /// </remarks>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.RegisterAttached(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(Synchronized),
            new PropertyMetadata(null, OnItemsSourceChanged));

    /// <inheritdoc cref="ItemsSourceProperty"/>
    [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
    public static IEnumerable GetItemsSource(DependencyObject target)
        => (IEnumerable)target.GetValue(ItemsSourceProperty);

    /// <inheritdoc cref="ItemsSourceProperty"/>
    public static void SetItemsSource(DependencyObject target, IEnumerable value)
        => target.SetValue(ItemsSourceProperty, value);

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ic = (ItemsControl)d;

        if (e.NewValue is null)
        {
            ic.ItemsSource = null;
            return;
        }

        var newItemsSource = (IEnumerable)e.NewValue;

        IEnumerable synchronizedItemsSource = newItemsSource is ICollectionView cv
            ? cv.SourceCollection : newItemsSource;

        var synchronized = synchronizedItemsSource as ISynchronized
            ?? throw new ArgumentException(
                $"Cannot synchronize access to items in source collection of type " +
                $"'{synchronizedItemsSource.GetType()}' because it does not implement " +
                $"'{nameof(ISynchronized)}'.");

        BindingOperations.EnableCollectionSynchronization(synchronizedItemsSource, synchronized.SynchronizationLock);

        ic.ItemsSource = newItemsSource;
    }
}
