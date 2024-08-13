using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MasterEngine.Editor;
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
        var SceneDock = new Editor.Layout.TabControl("Scene");
        Viewport.Content = SceneDock;
        var GameDock = new Editor.Layout.TabControl("Game");
        SceneDock.Merge(GameDock);
        TextBlock label = new TextBlock(){Text="dddddddd"};
        GameDock.Control.Content = label;

        managerGraphic = new ManagerGraphic(SceneDock.Control, GraphicAPI.OpenGL);
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