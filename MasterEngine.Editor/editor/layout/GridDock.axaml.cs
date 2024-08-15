
using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;

namespace MasterEngine.Editor.Layout;
public partial class GridDock : UserControl{
    public enum DockAlign {Top, Bottom, Left, Right, Center};
    public int ID {get;}
    static int instance;
    List<ContentControl> DockHorizontal = new();
    List<ContentControl> DockVertical = new();
    public Action? OnClose;

    public GridDock(){
        InitializeComponent();
        ID = instance++;
    }

    private void Close(TabControl tabControl){
        DockHorizontal.Remove(tabControl);
        DockVertical.Remove(tabControl);
        if(DockHorizontal.Count == 0 && DockVertical.Count == 0)
            OnClose?.Invoke();
        UpdateDock();
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
                MinHeight = 200
            };

            Dock.RowDefinitions.Add(ControlRowDefinition);
            item.SetValue(Grid.RowProperty, rows);
            // Add Control to Dock
            Dock.Children.Add(item);
            rows++;
        }
    }

    public void Add(ContentControl control, DockAlign align){
        if(align == DockAlign.Right){
            DockHorizontal.Add(control);
            UpdateDock();
        }

        if(align == DockAlign.Bottom){
            DockVertical.Add(control);
            UpdateDock();
        }

        if(control is TabControl)
            ((TabControl)control).OnClose = Close;
    }

    public void Remove(ContentControl control){
        DockHorizontal.Remove(control);
        DockVertical.Remove(control);
        UpdateDock();
    }
}