using System;
using Avalonia.Interactivity;
using MasterEngine.Editor;
using MasterEngine.Graphic;
using MasterEngine.Runtime;

namespace MasterEngine;

public partial class EditorWindow : Avalonia.Controls.Window{
    public static EditorWindow? Window {get;set;}
    ManagerGraphic? managerGraphic;
    GraphicAPI graphicAPI;
    
    public EditorWindow(GraphicAPI graphicAPI){
        this.graphicAPI = graphicAPI;
        Debug.Log("Initializing UI...");
        InitializeComponent();
        Loaded += OnLoaded;
        Closing += OnClosing;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e){
        Window = this;
        managerGraphic = new ManagerGraphic(Viewport, graphicAPI);
        Title = Application.EngineName + " - " + managerGraphic?.API;

        if(managerGraphic != null && managerGraphic.GraphicComponent != null)
            managerGraphic.GraphicComponent.OnUpdate += OnUpdate;
        
        LabelStatusBarVersion.Text = Application.EngineVersion;
    }

    private void OnUpdate(double obj){

    }

    private void OnClosing(object? sender, EventArgs e){
        Debug.Log("Closing...");
        managerGraphic?.Dispose();
    }
}