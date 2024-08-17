
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace MasterEngine.Editor.Layout;
public partial class GridDock : UserControl{
    public enum DockAlign {Top, Bottom, Left, Right, Center};
    public enum GridAlign {Horizontal, Vertical};

    public int ID {get;}
    static int instance;
    List<ContentControl> DockHorizontal = [];
    List<ContentControl> DockVertical = [];
    public Action<GridDock>? OnClose;

    public GridDock(){
        InitializeComponent();
        ID = instance++;
    }

    private void UpdateDock(){
        Dock.Children.Clear();
        Dock.ColumnDefinitions.Clear();
        Dock.RowDefinitions.Clear();

        int column = 0;
        foreach(var item in DockHorizontal){
            if(column != 0){
                // Add style to Grid Split Columns
                Dock.ColumnDefinitions.Add(new ColumnDefinition{Width = GridLength.Auto});
                
                // Add Grid Split Columns to Dock
                var gridsplite = new GridSplitter{
                    Background = new SolidColorBrush(Colors.Black),
                    ResizeDirection = GridResizeDirection.Columns
                };
                
                gridsplite.SetValue(Grid.ColumnProperty, column);
                Dock.Children.Add(gridsplite);
                column++;
            }
            

            // Add style to Control
            var controlColumnDefinition = new ColumnDefinition{
                Width = GridLength.Star,
                MinWidth = 200
            };

            Dock.ColumnDefinitions.Add(controlColumnDefinition);
            item.SetValue(Grid.ColumnProperty, column);
            Dock.Children.Add(item);
            column++;
        }

        int rows = 0;
        foreach(var item in DockVertical){
            if(rows != 0){
                // Add style to Grid Split Rows
                Dock.RowDefinitions.Add(new RowDefinition{Height = GridLength.Auto});
                
                // Add Grid Split Rows to Dock
                var gridsplite = new GridSplitter{
                    Background = new SolidColorBrush(Colors.Black),
                    ResizeDirection = GridResizeDirection.Rows
                };
                gridsplite.SetValue(Grid.RowProperty, rows);
                Dock.Children.Add(gridsplite);
                rows++;
            }

            // Add style to Control
            var ControlRowDefinition = new RowDefinition{
                Height = GridLength.Star,
                MinHeight = 100
            };

            Dock.RowDefinitions.Add(ControlRowDefinition);
            item.SetValue(Grid.RowProperty, rows);
            
            // Add Control to Dock
            Dock.Children.Add(item);
            rows++;
        }

    }

    public void AddRange(List<ContentControl> controls, GridAlign align){
        if(align == GridAlign.Vertical)
            DockVertical.AddRange(controls);

        if(align == GridAlign.Horizontal)
            DockHorizontal.AddRange(controls);

        foreach(var control in controls){
            if(control is TabControl)
                ((TabControl)control).OnClose += Remove;

            if(control is GridDock gridDock)
                gridDock.OnClose += Remove;
        }
        UpdateDock();
    }

    public void Add(ContentControl control, DockAlign align){
        if(align == DockAlign.Right || align == DockAlign.Left)
            if(DockVertical.Count == 0)
                DockHorizontal.Insert(align == DockAlign.Right ? DockHorizontal.Count : 0, control);
            else{
                List<ContentControl> listH = DockHorizontal.ToList(), listV = DockVertical.ToList();
                listH.Insert(align == DockAlign.Right ? listH.Count : 0, control);
                Clear();
                GridDock gridH = new(), gridV = new();
                gridH.AddRange(listH, GridAlign.Horizontal);
                gridV.AddRange(listV, GridAlign.Vertical);
                AddRange(align == DockAlign.Right ? new List<ContentControl>(){gridV, gridH} : new List<ContentControl>(){gridH, gridV}, GridAlign.Horizontal);
                return;
            }

        if(align == DockAlign.Bottom || align == DockAlign.Top)
            if(DockHorizontal.Count == 0)
                DockVertical.Insert(align == DockAlign.Bottom ? DockVertical.Count : 0, control);
            else{
                List<ContentControl> listH = DockHorizontal.ToList(), listV = DockVertical.ToList();
                listV.Insert(align == DockAlign.Bottom ? listV.Count : 0, control);
                Clear();
                GridDock gridH = new(), gridV = new();
                gridH.AddRange(listH, GridAlign.Horizontal);
                gridV.AddRange(listV, GridAlign.Vertical);
                AddRange(align == DockAlign.Bottom ? new List<ContentControl>(){gridH, gridV} : new List<ContentControl>(){gridV, gridH}, GridAlign.Vertical);
                return;
            }

        if(control is TabControl){
            ((TabControl)control).GridDock = this;
            ((TabControl)control).OnClose += Remove;
        }

        if(control is GridDock gridDock)
            gridDock.OnClose += Remove;

        UpdateDock();
    }

    public void Clear(){
        foreach(var control in DockHorizontal.ToList())
            OnlyRemove(control);

        foreach(var control in DockVertical.ToList())
            OnlyRemove(control);

        UpdateDock();
    }

    public void Remove(ContentControl control){
        OnlyRemove(control);
        UpdateDock();
    }

    void OnlyRemove(ContentControl control){
        if(control is TabControl){
            ((TabControl)control).OnClose -= Remove;
        }
        
        if(control is GridDock gridDock)
            gridDock.OnClose -= Remove;

        DockHorizontal.Remove(control);
        DockVertical.Remove(control);
        if(DockHorizontal.Count == 0 && DockVertical.Count == 0)
            OnClose?.Invoke(this);
    }
}