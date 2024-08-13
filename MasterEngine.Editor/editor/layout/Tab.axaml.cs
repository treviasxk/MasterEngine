using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
namespace MasterEngine.Editor.Layout;

public partial class Tab : UserControl{
    public string Title { get{return PanelTab.Content.ToString();} set{PanelTab.Content = value;} }
    public Action<Tab>? OnClick { get; set; }
    public Action<Tab>? OnClose { get; set; }
    public TabControl TabControl { get; set; }
    public Tab(string title, TabControl? tabControl = null){
        InitializeComponent();
        PanelTab.Content = title;
        TabControl = tabControl == null ? new("New Tab") : tabControl;
        PanelTab.Click += Click;
        PanelClose.Click += Close;
    }

    private void Close(object? sender, RoutedEventArgs e){
        OnClose?.Invoke(this);
    }

    private void Click(object? sender, RoutedEventArgs e){
        OnClick?.Invoke(this);
    }
}