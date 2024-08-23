using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MasterEngine.Editor;
using MasterEngine.Editor.Layout;
using MasterEngine.Graphic;
using Application = MasterEngine.Runtime.Application;
using TabControl = MasterEngine.Editor.Layout.TabControl;

namespace MasterEngine;

public partial class EditorWindow : Window{
    public static EditorWindow? Window {get;set;}
    ManagerGraphic? managerGraphic;
    
    public EditorWindow(){
        Debug.Log("Initializing UI...");
        InitializeComponent();
        LabelStatusBarVersion.Text = Application.EngineVersion;
        Loaded += OnLoaded;
        Closing += OnClosing;
    }


    private void OnLoaded(object? sender, RoutedEventArgs e){
        Window = this;
        TabIcon.Data = AppIcons.Play;
        var gridDock = new GridDock();
        PanelDock.Content = gridDock;


        var tabControl = new TabControl();
        Tab Scene = new("Scene");
        tabControl.Add(Scene);
        gridDock.Add(tabControl);

        
        Tab Game = new("Game");
        Tab hierarchy = new("Hierarchy");
        TextBlock textBlock = new(){Text = "Hello World!"};
        Game.Control.Content = textBlock;

        tabControl.Add(Game);
        tabControl.Add(hierarchy);


        var tabControl2 = new TabControl();
        Tab Explorer = new("Explorer");
        Tab Inspector = new("Inspector");

        tabControl2.Add(Explorer);
        tabControl2.Add(Inspector);

        gridDock.Add(tabControl2);
        var tabControl3 = new TabControl();
        Tab Console = new("Console");
        Tab Animation = new("Animation");
        tabControl3.Add(Console);
        tabControl3.Add(Animation);
        gridDock.Add(tabControl3);

        var tabControl4 = new TabControl();
        Tab Shader = new("Shader Graph");
        Tab Audio = new("Audio Mixer");
        tabControl4.Add(Shader);
        tabControl4.Add(Audio);
        gridDock.Add(tabControl4);


        managerGraphic = new ManagerGraphic(Scene.Control, GraphicAPI.Direct3D11);
        Title = Application.EngineName + " - " + managerGraphic?.API;
        managerGraphic!.GraphicComponent!.OnUpdate += OnUpdate;
    }

    private void OnUpdate(double obj){

    }

    private void OnClosing(object? sender, EventArgs e){
        Debug.Log("Closing...");
        managerGraphic?.Dispose();
    }
}