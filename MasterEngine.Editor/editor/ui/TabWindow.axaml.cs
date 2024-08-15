using Avalonia.Controls;
using MasterEngine.Editor.Layout;
using TabControl = MasterEngine.Editor.Layout.TabControl;

namespace MasterEngine;
public partial class TabWindow : Window{
    public TabWindow(Tab tab){
        InitializeComponent();
        var tabControl = new TabControl();
        tabControl.Add(tab);
        tabControl.OnClose += OnClosed;
        tabControl.OnTabChanged += OnTabChanged;
        PanelDock.Content = tabControl;
        Title = tab.Title;
    }

    private void OnTabChanged(Tab tab){
        Title = tab.Title;
    }

    private void OnClosed(TabControl tabControl){
        Close();
    }
}