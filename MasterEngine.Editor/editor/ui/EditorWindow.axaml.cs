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
        var tabControl = new Editor.Layout.TabControl();
        Tab Scene = new("Scene");
        Tab Game = new("Game");
        Tab hierarchy = new("hierarchy");
        TextBlock textBlock = new(){Text = "Hello World!"};
        Game.Control.Content = textBlock;
        tabControl.Add(Scene);
        tabControl.Add(Game);
        tabControl.Add(hierarchy);

        PanelDock.Content = tabControl;

        managerGraphic = new ManagerGraphic(Scene.Control, GraphicAPI.OpenGL);
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