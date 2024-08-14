
using System;
using Avalonia.Controls;
using MasterEngine.Editor.Layout;

namespace MasterEngine;
public partial class TabWindow : Window{
    public TabWindow(Tab tab){
        InitializeComponent();
        var tabControl = new Editor.Layout.TabControl();
        tabControl.Add(tab);
        tabControl.OnClose += OnClosed;
        tabControl.OnTabChanged += OnTabChanged;
        PanelDock.Content = tabControl;
        Title = tab.Title;
    }

    private void OnTabChanged(Tab tab){
        Title = tab.Title;
        Console.WriteLine("c");
    }

    private void OnClosed(){
        Close();
    }
}