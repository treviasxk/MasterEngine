using System;
using Avalonia.Controls;

namespace MasterEngine.Editor.Layout;
public partial class TabControl : UserControl{
    public ContentControl Control { get; }
    public Tab Tab {get;}
    public TabControl(string title){
        InitializeComponent();
        Control = PanelContent;
        Tab = new Tab(title, this);
        Add(Tab);
    }

    public void Select(Tab tab){
        
    }

    public void Merge(TabControl tabControl){
        Add(new Tab(tabControl.Tab.Title, tabControl));
    }

    public void Add(Tab tab){
        tab.OnClose += OnClose;
        tab.OnClick += OnClick;
        PanelTabs.Children.Add(tab);
    }

    private void OnClick(Tab tab){
        Select(tab);
        Console.WriteLine(tab.Title);
    }

    private void OnClose(Tab tab){
        Remove(tab);
    }

    public void Remove(Tab tab){
        PanelTabs.Children.Remove(tab);
    }
}