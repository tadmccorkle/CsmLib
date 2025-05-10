// Copyright (c) 2025 by Tad McCorkle
// Licensed under the MIT license.

namespace CsmLib.Synchronization;

/// <summary>
/// Exposes a reference that should be used to synchronize (i.e., <see langword="lock"/>) operations
/// on and access to this instance.
/// </summary>
public interface ISynchronized
{
    /// <summary>
    /// The reference that should be used to synchronize operations on and access to this instance.
    /// </summary>
    object SynchronizationLock { get; }
}
