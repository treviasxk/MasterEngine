using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
namespace MasterEngine.Editor.Layout;

public partial class Tab : UserControl{
    /// <summary>
    /// Get Tab title.
    /// </summary>
    public string Title { get{return TabTitle.Text!.ToString();} set{TabTitle.Text = value; TabTitle.SetValue(ToolTip.TipProperty, value);} }
    /// <summary>
    /// Click Tab.
    /// </summary>
    public Action<Tab>? OnClick { get; set; }
    /// <summary>
    /// The event is called when it tab is closed.
    /// </summary>
    public Action<Tab>? OnClose { get; set; }
    /// <summary>
    /// Get Tab content.
    /// </summary>
    public ContentControl Control {get;} = new();

    public TabControl? TabControl { get; set; }

    /// <summary>
    /// Tab to use in TabControl Master Engine.
    /// </summary>
    /// <param name="title"></param>
    public Tab(string title){
        InitializeComponent();
        Title = title;
        TabIcon.Data = AppIcons.GetIcon(title);
        Control.Background = TabTitle.Background;
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