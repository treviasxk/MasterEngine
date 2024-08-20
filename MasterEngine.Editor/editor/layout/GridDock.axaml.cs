
using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;

namespace MasterEngine.Editor.Layout;
public partial class GridDock : UserControl{
    public enum GridAlign {Horizontal, Vertical};

    public int ID {get;}
    static int instance;
    public Controls Controls = [];
    public GridAlign Align { get; set; } = GridAlign.Horizontal;
    public Action<GridDock>? OnClose;
    internal GridDock? GridDockParent;

    public GridDock(){
        InitializeComponent();
        ID = instance++;
        Update();
    }

    public void Update(){
        Dock.Children.Clear();
        Dock.ColumnDefinitions.Clear();
        Dock.RowDefinitions.Clear();

        int index = 0;
        foreach(var item in Controls){
            if(index != 0){
                // Add style to Grid Split
                if(Align == GridAlign.Horizontal)
                    Dock.ColumnDefinitions.Add(new ColumnDefinition{Width = GridLength.Auto});
                else
                    Dock.RowDefinitions.Add(new RowDefinition{Height = GridLength.Auto});
                
                
                // Add Grid Split to Dock
                var gridsplite = new GridSplitter{
                    Background = new SolidColorBrush(Colors.Black),
                    ResizeDirection = Align == GridAlign.Horizontal ? GridResizeDirection.Columns : GridResizeDirection.Rows
                };
                
                gridsplite.SetValue(Align == GridAlign.Horizontal? Grid.ColumnProperty : Grid.RowProperty, index);
                Dock.Children.Add(gridsplite);
                index++;
            }
            
            // Add style to Control
            if(Align == GridAlign.Horizontal)
                Dock.ColumnDefinitions.Add(new ColumnDefinition(){Width = GridLength.Star, MinWidth = 100});
            else
                Dock.RowDefinitions.Add(new RowDefinition(){Height = GridLength.Star, MinHeight = 100});
            
            item.SetValue(Align == GridAlign.Horizontal ? Grid.ColumnProperty : Grid.RowProperty, index);
            Dock.Children.Add(item);
            index++;
        }
    }

    public void AddRange(Controls controls){
        Controls.AddRange(controls);
        foreach(var control in controls){
            if(control is TabControl tabControl){
                tabControl.GridDock = this;
                tabControl.OnClose += Remove;
            }

            if(control is GridDock gridDock){
                gridDock.GridDockParent = this;
                gridDock.OnClose += Remove;
            }
        }
        Update();
    }


    public void Add(Control control, int index = -1){
        Controls.Add(control);
        if(index > -1 && Controls.Count > 0)
            Controls.Move(Controls.Count - 1, index);

        if(control is TabControl tabControl){
            tabControl.GridDock = this;
            tabControl.OnClose += Remove;
        }

        if(control is GridDock gridDock){
            gridDock.GridDockParent = this;
            gridDock.OnClose += Remove;
        }

        Update();
    }

    public void Clear(){
        foreach(var control in Controls.ToList())
            OnlyRemove(control);
        Update();
    }

    public int GetIndex(Control control) => Controls.IndexOf(control);
    

    public void Remove(Control control){
        OnlyRemove(control);
        Update();
    }

    void OnlyRemove(Control control){
        if(control is TabControl tabControl){
            tabControl.GridDock = null;
            tabControl.OnClose -= Remove;
            Controls.Remove(control);
        }
        
        if(control is GridDock gridDock){
            gridDock.OnClose -= Remove;
            if(gridDock.Controls.Count <= 1){
                Controls.Remove(control);
            }
        }

    
        if(Controls.Count == 0){
            OnClose?.Invoke(this);
        }
    }
}