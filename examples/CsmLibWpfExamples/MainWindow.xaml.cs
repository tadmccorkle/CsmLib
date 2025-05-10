// Copyright (c) 2025 by Tad McCorkle
// Licensed under the MIT license.

using System.Windows;

namespace CsmLibWpfExamples;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();

        MainViewModel vm = new();
        this.DataContext = vm;
        vm.StartContentGeneration();
        this.Closing += (s, e) => vm.Dispose();
    }
}
