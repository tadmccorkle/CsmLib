// Copyright (c) 2025 by Tad McCorkle
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CsmLib.Synchronization;

namespace CsmLibWpfExamples;

public sealed class MainViewModel : ObservableObject, IDisposable
{
    private readonly CancellationTokenSource cts = new();

    public bool IsCollectionModificationSynchronized { get; set; } = true;
    public SynchronizedObservableCollection<int> Collection { get; } = [];

    public void StartContentGeneration()
    {
        var cancellationToken = this.cts.Token;
        int i = 0;

        _ = Task.WhenAll(Enumerable.Repeat(0, 10).Select(_ => Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (this.IsCollectionModificationSynchronized)
                {
                    if (this.Collection.Count > 1000)
                    {
                        this.Collection.Clear();
                    }
                    else
                    {
                        this.Collection.Add(Interlocked.Increment(ref i));
                    }
                }
                else
                {
                    if (this.Collection.Count > 1000)
                    {
                        this.Collection.UnsyncClear();
                    }
                    else
                    {
                        this.Collection.UnsyncAdd(Interlocked.Increment(ref i));
                    }
                }

                try
                {
                    await Task.Delay(10, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) { }
            }
        })));
    }

    public void Dispose()
    {
        using var cts = this.cts;
        cts.Cancel();
    }
}
