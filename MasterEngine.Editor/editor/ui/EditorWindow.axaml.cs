using System;
using Avalonia.Interactivity;
using MasterEngine.Graphic;

namespace MasterEngine;

public partial class EditorWindow : Avalonia.Controls.Window{
    ManagerGraphic? graphic;
    GraphicAPI graphicAPI;
    public EditorWindow(GraphicAPI graphicAPI){
        this.graphicAPI = graphicAPI;
        Console.WriteLine("Initializing UI...");
        InitializeComponent();
        Loaded += OnLoaded;
        Closing += OnClosing;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e){
        graphic = new ManagerGraphic(Viewport, graphicAPI);
        Title = "Master Engine - " + graphic?.API;
    }

    private void OnClosing(object? sender, EventArgs e){
        Console.WriteLine("Closing...");
        graphic?.Dispose();
    }
}