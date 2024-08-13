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
        managerGraphic = new ManagerGraphic(Viewport, GraphicAPI.Direct3D11);
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