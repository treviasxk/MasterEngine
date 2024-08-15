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
        Console.WriteLine("Tab Control:{0} Tab Selected: {1}", ID, tab.Title);
        if(PanelTabs.Children.Contains(tab)){
            if(!PanelContent.Children.Contains(tab.Control)){
                PanelContent.Children.Add(tab.Control);
                tab.Control.IsVisible = true;
            }

            if(CurrentTab != tab){
                Console.WriteLine(CurrentTab);
                if(CurrentTab != null)
                    CurrentTab.Control.IsVisible = false;

                for(int i = 0; i < PanelTabs.Children.Count; i++){
                    var tabTheme = (Tab)PanelTabs.Children[i];
                    tabTheme.TabTitle.Background = new SolidColorBrush(Color.FromRgb(12,12,20));
                    tabTheme.TabTitle.Foreground = new SolidColorBrush(Color.FromRgb(200,200,200));
                }

                tab.TabTitle.Background = tab.Control.Background;
                tab.TabTitle.Foreground = new SolidColorBrush(Color.FromRgb(255,255,255));

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
        public TabControl? tabControl;
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
            dataObject.Set("Tab", new TabData(){id = ID, tab = tab, point = e.GetPosition(tab)});

            var result = await DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Move);
            if(result == DragDropEffects.None){
                if(PanelTabs.Children.Count > 1){
                    Remove(tab);
                    new TabWindow(tab).Show();
                    Console.WriteLine("Created TabControl in window!");
                }
            }
            if(result == DragDropEffects.Move){
                if(dataObject.Get("Tab") is TabData tabData)
                    if(tabData.move){
                        Console.WriteLine("Changed Tab from TabControl from: {0}, to: {1}", tabData.tabControl!.ID, ID);
                        // Remove from current TabControl run dragdrop
                        Remove(tab);
                        // Add tab in TabControl destination.
                        tabData.tabControl!.Add(tab);
                        // Set position drop
                        tabData.tabControl!.MoveTab(tab, tabData.point);
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
            if(tabData.id != ID){    
                // Set TabControl destination.
                tabData.tabControl = this;
                // Change Tab of TabControl.
                tabData.move = true;
                // Set point from destination to MoveTab.
                tabData.point = e.GetPosition(PanelTabs);
            }else
                MoveTab(tabData.tab, point);
        }
    }

    public bool MoveTab(Tab tab, Point point){
        int index = PanelTabs.Children.IndexOf(tab); // -1 = No registed in PanelTabs
        int newIndex = PanelTabs.Children.Count > 0 ? Math.Clamp((int)(point.X / tab.Bounds.Width), 0, PanelTabs.Children.Count - 1) : 0;
        if(index != newIndex){
            Console.WriteLine("Moved Tab of index from: {0} to: {1}", index, newIndex);
            PanelTabs.Children.Move(index, newIndex);
            return true;
        }else
            return false;   // false = Move Tab is out from tabcontrol instance.
    }

    /// <summary>
    /// Add an Tab to TabControl.
    /// </summary>
    /// <param name="tab"></param>
    public void Add(Tab tab){
        tab.OnClose += Remove;
        tab.OnClick += Select;
        tab.PointerMoved += OnPointerMovedAsync;
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

        if(!PanelContent.Children.Contains(tab.Control))
            Console.WriteLine("Warning, not item existent to remove!");

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

        PanelContent.UpdateLayout();
        PanelTabs.UpdateLayout();
    }
}