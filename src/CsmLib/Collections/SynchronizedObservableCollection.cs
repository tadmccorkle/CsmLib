// Copyright (c) 2025 by Tad McCorkle
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using CsmLib.Threading;

namespace CsmLib.Collections;

/// <summary>
/// An <see cref="ObservableCollection{T}"/> whose operations support synchronization by default.
/// </summary>
/// <remarks>
/// <para>
/// Accesses to this collection can be synchronized with modifications by acquiring a lock on the
/// corresponding <see cref="ISynchronized.SynchronizationLock"/>.
/// </para>
/// <para>
/// Unsynchronized methods are also provided (<c>Unsync...</c>) that leave synchronization up to the caller.
/// </para>
/// </remarks>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public class SynchronizedObservableCollection<T> : ObservableCollection<T>, ISynchronized
{
    private readonly object syncLock;

    /// <summary>
    /// Initializes a new instance of the <see cref="SynchronizedObservableCollection{T}"/> class
    /// that is empty and uses a new synchronization reference.
    /// </summary>
    public SynchronizedObservableCollection() : this(new object()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SynchronizedObservableCollection{T}"/> class
    /// that contains elements copied from the specified collection and uses a new synchronization
    /// reference.
    /// </summary>
    public SynchronizedObservableCollection(IEnumerable<T> collection) : this(collection, new()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SynchronizedObservableCollection{T}"/> class
    /// that contains elements copied from the specified list and uses a new synchronization
    /// reference.
    /// </summary>
    public SynchronizedObservableCollection(List<T> list) : this(list, new()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SynchronizedObservableCollection{T}"/> class
    /// that is empty and uses the specified synchronization reference.
    /// </summary>
    public SynchronizedObservableCollection(object syncLock)
    {
        this.syncLock = syncLock;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SynchronizedObservableCollection{T}"/> class
    /// that contains elements copied from the specified collection and uses the specified
    /// synchronization reference.
    /// </summary>
    public SynchronizedObservableCollection(IEnumerable<T> collection, object syncLock) : base(collection)
    {
        this.syncLock = syncLock;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SynchronizedObservableCollection{T}"/> class
    /// that contains elements copied from the specified list and uses the specified synchronization
    /// reference.
    /// </summary>
    public SynchronizedObservableCollection(List<T> list, object syncLock) : base(list)
    {
        this.syncLock = syncLock;
    }

    object ISynchronized.SynchronizationLock => this.syncLock;

    protected override void InsertItem(int index, T item)
    {
        lock (this.syncLock)
        {
            base.InsertItem(index, item);
        }
    }

    protected override void SetItem(int index, T item)
    {
        lock (this.syncLock)
        {
            base.SetItem(index, item);
        }
    }

    protected override void MoveItem(int oldIndex, int newIndex)
    {
        lock (this.syncLock)
        {
            base.MoveItem(oldIndex, newIndex);
        }
    }

    protected override void RemoveItem(int index)
    {
        lock (this.syncLock)
        {
            base.RemoveItem(index);
        }
    }

    protected override void ClearItems()
    {
        lock (this.syncLock)
        {
            base.ClearItems();
        }
    }

    /// <inheritdoc cref="Collection{T}.Add(T)"/>
    public void UnsyncAdd(T item)
    {
        base.InsertItem(this.Count, item);
    }

    /// <inheritdoc cref="Collection{T}.Insert(int, T)"/>
    public void UnsyncInsert(int index, T item)
    {
        base.InsertItem(index, item);
    }

    /// <inheritdoc cref="Collection{T}.SetItem(int, T)"/>
    public void UnsyncSetItem(int index, T item)
    {
        base.SetItem(index, item);
    }

    /// <inheritdoc cref="ObservableCollection{T}.Move(int, int)"/>
    public void UnsyncMove(int oldIndex, int newIndex)
    {
        base.MoveItem(oldIndex, newIndex);
    }

    /// <inheritdoc cref="Collection{T}.Remove(T)"/>
    public bool UnsyncRemove(T item)
    {
        int index = this.Items.IndexOf(item);
        if (index != -1) return false;
        base.RemoveItem(index);
        return true;
    }

    /// <inheritdoc cref="Collection{T}.RemoveAt(int)"/>
    public void UnsyncRemoveAt(int index)
    {
        base.RemoveItem(index);
    }

    /// <inheritdoc cref="Collection{T}.Clear"/>
    public void UnsyncClear()
    {
        base.ClearItems();
    }
}
