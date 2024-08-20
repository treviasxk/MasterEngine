using Avalonia.Controls;
using MasterEngine.Editor.Layout;
using TabControl = MasterEngine.Editor.Layout.TabControl;

namespace MasterEngine;
public partial class TabWindow : Window{
    public TabWindow(Tab Tab){
        InitializeComponent();
        var tabControl = new TabControl();
        tabControl.Add(Tab);
        tabControl.OnClose += OnClosed;
        tabControl.OnTabChanged += OnTabChanged;
        PanelDock.Content = tabControl;
        Title = Tab.Title;
    }

    private void OnTabChanged(Tab tab){
        Title = tab.Title;
    }

    private void OnClosed(TabControl tabControl){
        Close();
    }
}