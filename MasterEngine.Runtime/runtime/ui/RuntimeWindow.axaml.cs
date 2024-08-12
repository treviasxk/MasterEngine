using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MasterEngine.Graphic;

namespace MasterEngine.Runtime;

public partial class RuntimeWindow : Window{
    
    ManagerGraphic? graphic;
    GraphicAPI graphicAPI;

    public RuntimeWindow(GraphicAPI graphicAPI){
        InitializeComponent();
        this.graphicAPI = graphicAPI;
        Console.WriteLine("Initializing UI...");
        InitializeComponent();
        Loaded += OnLoaded;
        Closing += OnClosing;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e){
        graphic = new ManagerGraphic(Viewport, graphicAPI);
        Title = Application.ProductName;
    }

    private void OnClosing(object? sender, EventArgs e){
        Console.WriteLine("Closing...");
        graphic?.Dispose();
    }
}