using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
namespace MasterEngine.Editor.Layout;

public partial class Tab : UserControl{
    public string? Title { get{return TabTitle.Content!.ToString();} set{TabTitle.Content = value;} }
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
        TabTitle.Content = title;
        TabTitle.Click += Click;
        TabClose.Click += Close;
    }

    private void Close(object? sender, RoutedEventArgs e){
        OnClose?.Invoke(this);
    }

    private void Click(object? sender, RoutedEventArgs e){
        OnClick?.Invoke(this);
    }
}