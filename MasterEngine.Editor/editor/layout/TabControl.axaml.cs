using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace MasterEngine.Editor.Layout;
public partial class TabControl : UserControl{
    /// <summary>
    /// TabControl ID instance;
    /// </summary>
    public int ID {get;}
    /// <summary>
    /// Current Tab Selected.
    /// </summary>
    public Tab? CurrentTab { get; set; }
    /// <summary>
    /// The event is called when all tabs are closed.
    /// </summary>
    public Action? OnClose { get; set; }
    /// <summary>
    /// The event is called when an new Tab are selected.
    /// </summary>
    public Action<Tab>? OnTabChanged { get; set; }

    static int Instance = 0;

    /// <summary>
    /// TabControl Master Engine.
    /// </summary>
    public TabControl(){
        InitializeComponent();
        ID = Instance++;
        AddHandler(DragDrop.DragOverEvent, OnDragOverEvent);
        AddHandler(DragDrop.DropEvent, OnDropEvent);
    }

    /// <summary>
    /// Select and display an tab existient
    /// </summary>
    /// <param name="tab"></param>
    public void Select(Tab tab){
        Console.WriteLine(tab.Title);
        if(PanelTabs.Children.Contains(tab)){
            if(!PanelContent.Children.Contains(tab.Control))
                PanelContent.Children.Add(tab.Control);

            if(CurrentTab != tab){
                if(CurrentTab != null)
                    CurrentTab.Control.IsVisible = false;

                for(int i = 0; i < PanelTabs.Children.Count; i++){
                    var tabTheme = (Tab)PanelTabs.Children[i];
                    tabTheme.PanelTab.Background = new SolidColorBrush(Color.FromRgb(12,12,20));
                    tabTheme.PanelClose.Background = new SolidColorBrush(Color.FromRgb(12,12,20));
                }

                tab.PanelTab.Background = tab.Control.Background;
                tab.PanelClose.Background = tab.Control.Background;

                tab.Control.IsVisible = true;
                CurrentTab = tab;
                OnTabChanged?.Invoke(tab);
            }
        }
    }

    public void Merge(TabControl tabControl){
        //Add(new Tab(tabControl.Tab.Title, tabControl));
    }

    // Data transferer in dragdrop
    class TabData{
        public required int id;
        public required TabControl tabControl;
        public required Tab tab;
        public required Point point;
        public bool move;
    }

    private async void OnPointerMovedAsync(object? sender, PointerEventArgs e){
        if(e.GetCurrentPoint(PanelTabs).Properties.IsLeftButtonPressed && sender is Tab tab){
            Select(tab);
            var startPos = tab.RenderTransform;

            // Set TopMost to Tab when selected
            for(int i = 0; i < PanelTabs.Children.Count; i++)
                ((Tab)PanelTabs.Children[i]).ZIndex = 0;
            tab.ZIndex = 1;

            var dataObject = new DataObject();
            dataObject.Set("Tab", new TabData(){id = ID, tabControl = this, tab = tab, point = e.GetPosition(tab)});

            var result = await DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Move);
            if(result == DragDropEffects.None){
                if(PanelTabs.Children.Count > 1 && ID == 0){
                    Remove(tab);
                    new TabWindow(tab).Show();
                }
            }
            if(result == DragDropEffects.Move){
                if(dataObject.Get("Tab") is TabData tabData)
                    if(tabData.move){
                        Remove(tab);
                        tabData.tabControl.Add(tab);
                    }
            }
            tab.RenderTransform = startPos;
        }
    }


    private void OnDragOverEvent(object? sender, DragEventArgs e){
        // Check drag holding tab
        if(e.Data.Get("Tab") is TabData tabData){
            var mousePos = e.GetPosition(PanelTabs) - tabData.point;

            if(PanelTabs.Children.Contains(tabData.tab))
                tabData.tab.RenderTransform = new TranslateTransform(mousePos.X - tabData.tab.Bounds.X, tabData.tab.Bounds.Y);
        }
    }


    private void OnDropEvent(object? sender, DragEventArgs e){
        // Check drop tab
        if(e.Data.Get("Tab") is TabData tabData){
            var point = e.GetPosition(PanelTabs);
            int index = PanelTabs.Children.IndexOf(tabData.tab);

            if(index != -1 & tabData.id == ID){    // -1 = No registed in PanelTabs
                int newIndex = PanelTabs.Children.Count > 0 ? Math.Clamp((int)(point.X / tabData.tab.Bounds.Width), 0, PanelTabs.Children.Count - 1) : 0;
                if(index != newIndex)
                    PanelTabs.Children.Move(index, newIndex);
            }else{
                tabData.tabControl = this;
                tabData.move = true;
            }
        }
    }

    /// <summary>
    /// Add an Tab to TabControl.
    /// </summary>
    /// <param name="tab"></param>
    public void Add(Tab tab){
        tab.OnClose += Remove;
        tab.OnClick += Select;
        tab.PointerMoved += OnPointerMovedAsync;
        Console.WriteLine(PanelTabs.Children.Count);
        PanelTabs.Children.Add(tab);
        Select(tab);
    }

    /// <summary>
    /// Remove an Tab from TabControl.
    /// </summary>
    /// <param name="tab"></param>
    public void Remove(Tab tab){
        bool select = false;
        if(CurrentTab == tab)
            select = true;

        tab.OnClose -= Remove;
        tab.OnClick -= Select;
        tab.PointerMoved -= OnPointerMovedAsync;
        PanelContent.Children.Remove(tab.Control);
        PanelTabs.Children.Remove(tab);

        if(select){
            if(PanelTabs.Children.Count > 0)
                Select((Tab)PanelTabs.Children[0]);
            else
                OnClose?.Invoke();
        }
    }
}