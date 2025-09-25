// Copyright (c) 2025 by Tad McCorkle
// Licensed under the MIT license.

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CsmLib.Threading;

namespace CsmLib.Wpf.Behaviors;

/// <summary>
/// Attached properties for collection behaviors.
/// </summary>
public static class CollectionBehaviors
{
    /// <summary>
    /// Identifies the <c><see cref="CollectionBehaviors"/>.SynchronizedItemsSource</c> attached property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attached property is used to set an <see cref="ItemsControl"/>'s items source and set
    /// up synchronized collection access using a simple locking mechanism to allow the collection
    /// to be used on multiple threads.
    /// </para>
    /// <para>
    /// The value can either be an <see cref="IEnumerable"/> that implements
    /// <see cref="ISynchronized"/> or a <see cref="ICollectionView"/> whose
    /// <see cref="ICollectionView.SourceCollection"/> meets this constraint.
    /// </para>
    /// </remarks>
    public static readonly DependencyProperty SynchronizedItemsSourceProperty =
        DependencyProperty.RegisterAttached(
            "SynchronizedItemsSource",
            typeof(IEnumerable),
            typeof(CollectionBehaviors),
            new PropertyMetadata(null, OnSynchronizedItemsSourceChanged));

    /// <summary>Gets the synchronized items source attached to an <see cref="ItemsControl"/>.</summary>
    /// <remarks>See the <see cref="SynchronizedItemsSourceProperty"/> remarks for usage details.</remarks>
    [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
    public static IEnumerable GetSynchronizedItemsSource(DependencyObject target)
        => (IEnumerable)target.GetValue(SynchronizedItemsSourceProperty);

    /// <summary>Sets the synchronized items source attached to an <see cref="ItemsControl"/>.</summary>
    /// <remarks>See the <see cref="SynchronizedItemsSourceProperty"/> remarks for usage details.</remarks>
    public static void SetSynchronizedItemsSource(DependencyObject target, IEnumerable value)
        => target.SetValue(SynchronizedItemsSourceProperty, value);

    private static void OnSynchronizedItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
