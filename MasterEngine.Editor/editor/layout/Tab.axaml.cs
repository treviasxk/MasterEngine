using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
namespace MasterEngine.Editor.Layout;

public partial class Tab : UserControl{
    public string? Title { get{return TabTitle.Text!.ToString();} set{TabTitle.Text = value;} }
    public Action<Tab>? OnClick { get; set; }
    public Action<Tab>? OnClose { get; set; }
    public ContentControl Control {get;} = new();

    /// <summary>
    /// Tab to use in TabControl Master Engine.
    /// </summary>
    /// <param name="title"></param>
    public Tab(string title){
        InitializeComponent();
        TabIcon.Data = AppIcons.GetIcon(title);
        Control.Background = TabTitle.Background;
        TabTitle.Text = title;
        TabIcon.PointerPressed += Click;
        TabTitle.PointerPressed += Click;
        TabClose.PointerPressed += Close;
    }

    private void Close(object? sender, RoutedEventArgs e){
        OnClose?.Invoke(this);
    }

    private void Click(object? sender, RoutedEventArgs e){
        OnClick?.Invoke(this);
    }
}