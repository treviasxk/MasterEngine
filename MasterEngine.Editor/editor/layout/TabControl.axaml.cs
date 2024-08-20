using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
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
    public Action<TabControl>? OnClose { get; set; }
    /// <summary>
    /// The event is called when an new Tab are selected.
    /// </summary>
    public Action<Tab>? OnTabChanged { get; set; }
    internal GridDock? GridDock { get; set; }
    public Tab[] Tabs {get {return (Tab[])PanelTabs.Children.ToArray(); }}
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
        if(PanelTabs.Children.Contains(tab)){
            if(!PanelContent.Children.Contains(tab.Control)){
                PanelContent.Children.Add(tab.Control);
                tab.Control.IsVisible = true;
            }

            if(CurrentTab != tab){
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

    enum AlignMerge {Bottom, Right, Top, Left}
    // Data transferer in dragdrop
    class TabData{
        public required int id;
        public TabControl? tabControl;
        public required Tab tab;
        public required Point point;
        public bool move;
        public AlignMerge align;
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
                }
            }
            if(result == DragDropEffects.Move){               
                if(dataObject.Get("Tab") is TabData tabData && tabData.tabControl != null && tabData.tabControl.GridDock != null)
                    if(tabData.move){
                        // Remove from current TabControl run dragdrop
                        Remove(tab);
                        var gridDock = tabData.tabControl.GridDock;
                        // Add tab in TabControl destination.
                        tabData.tabControl!.Add(tab);

                        if(gridDock.Controls.Count == 1 && gridDock.GridDockParent != null){
                            var index = gridDock.GridDockParent.GetIndex(gridDock);
                            gridDock.Clear();
                            gridDock.GridDockParent?.Remove(gridDock);
                            gridDock.GridDockParent?.Add(tabData.tabControl, index);
                        }

                        // Set position drop
                        tabData.tabControl!.MoveTab(tab, tabData.point);
                    }else
                    if(tabData.tabControl.PanelTabs.Children.Count > 1 && tabData.align != AlignMerge.Top){
                        // Remove from current TabControl run dragdrop
                        Remove(tab);
                        var gridDock = tabData.tabControl.GridDock;
                        var index = tabData.tabControl.GridDock.GetIndex(tabData.tabControl);
                        var align = tabData.align == AlignMerge.Right || tabData.align == AlignMerge.Left ? GridDock.GridAlign.Horizontal : GridDock.GridAlign.Vertical;
                        if(gridDock.Align == align){
                            var tabControl = new TabControl();
                            tabControl.Add(tab);
                            gridDock.Add(tabControl, tabData.align == AlignMerge.Right ? index + 1 : index);
                        }else{
                            tabData.tabControl.GridDock.Remove(tabData.tabControl);
                            var tabControl = new TabControl();
                            tabControl.Add(tab);
                            var grid = new GridDock(){Align = align};

                            grid.AddRange(tabData.align == AlignMerge.Bottom || tabData.align == AlignMerge.Right ? new Controls(){tabData.tabControl, tabControl} : new Controls(){tabControl, tabData.tabControl});
                            gridDock.Add(grid, index);
                        }
                    }
                
            }
            tab.RenderTransform = startPos;
        }
    }


    private void OnDragOverEvent(object? sender, DragEventArgs e){
        // Check drag holding tab
        if(e.Data.Get("Tab") is TabData tabData){
            var mousePos = e.GetPosition(this);

            var areaW = Bounds.Width / 100 * 15;
            var areaH = Bounds.Height / 100 * 15;

            if(mousePos.X < areaW)
                tabData.align = AlignMerge.Left;
            else
            if(mousePos.Y < areaH)
                tabData.align = AlignMerge.Top;
            else
            if(mousePos.X > Bounds.Width - areaW)
                tabData.align = AlignMerge.Right;
            else
            if(mousePos.Y > Bounds.Height - areaH)
                tabData.align = AlignMerge.Bottom;
            else{
                tabData.tabControl = this;
            }

            mousePos = e.GetPosition(PanelTabs) - tabData.point;
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
        tab.TabControl = this;
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
        tab.TabControl = null;
        PanelContent.Children.Remove(tab.Control);
        PanelTabs.Children.Remove(tab);

        if(select){
            if(PanelTabs.Children.Count > 0)
                Select((Tab)PanelTabs.Children[0]);
            else
                OnClose?.Invoke(this);
        }

        PanelContent.UpdateLayout();
        PanelTabs.UpdateLayout();
    }
}