using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MasterEngine.Editor;
using MasterEngine.Editor.Layout;
using MasterEngine.Graphic;
using MasterEngine.Runtime;

namespace MasterEngine;
public partial class EditorWindow : Window{
    public static EditorWindow? Window {get;set;}
    ManagerGraphic? managerGraphic;
    
    public EditorWindow(){
        Debug.Log("Initializing UI...");
        InitializeComponent();
        Loaded += OnLoaded;
        Closing += OnClosing;
    }


    private void OnLoaded(object? sender, RoutedEventArgs e){
        Window = this;

        var GridDock = new GridDock();

        PanelDock.Content = GridDock;

        var tabControl = new Editor.Layout.TabControl();
        Tab Scene = new("Scene");
        Tab Game = new("Game");
        Tab hierarchy = new("Hierarchy");
        TextBlock textBlock = new(){Text = "Hello World!"};
        Game.Control.Content = textBlock;
        tabControl.Add(Scene);
        tabControl.Add(Game);
        tabControl.Add(hierarchy);

        var tabControl2 = new Editor.Layout.TabControl();
        Tab Explorer = new("Explorer");
        Tab Inspector = new("Inspector");

        tabControl2.Add(Explorer);
        tabControl2.Add(Inspector);

        var GridDock2 = new GridDock();

        var tabControl3 = new Editor.Layout.TabControl();
        Tab Console = new("Console");
        Tab Animation = new("Animation");
        tabControl3.Add(Console);
        tabControl3.Add(Animation);

        var tabControl4 = new Editor.Layout.TabControl();
        Tab Shader = new("Shader Graph");
        Tab Audio = new("Audio Mixer");
        tabControl4.Add(Shader);
        tabControl4.Add(Audio);

        GridDock.Add(tabControl, GridDock.DockAlign.Right);
        GridDock.Add(tabControl2, GridDock.DockAlign.Bottom);
        GridDock.Add(GridDock2, GridDock.DockAlign.Bottom);

        GridDock2.Add(tabControl3, GridDock.DockAlign.Right);
        GridDock2.Add(tabControl4, GridDock.DockAlign.Right);

        managerGraphic = new ManagerGraphic(Scene.Control, GraphicAPI.Direct3D11);
        Title = Application.EngineName + " - " + managerGraphic?.API;
        managerGraphic!.GraphicComponent!.OnUpdate += OnUpdate;
        LabelStatusBarVersion.Text = Application.EngineVersion;
    }

    private void OnUpdate(double obj){

    }

    private void OnClosing(object? sender, EventArgs e){
        Debug.Log("Closing...");
        managerGraphic?.Dispose();
    }
}