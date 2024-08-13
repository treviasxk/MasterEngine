using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MasterEngine.Graphic;

namespace MasterEngine.Runtime;

public partial class RuntimeWindow : Window{
    
    ManagerGraphic? graphic;

    public RuntimeWindow(){
        InitializeComponent();
        Console.WriteLine("Initializing UI...");
        Loaded += OnLoaded;
        Closing += OnClosing;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e){
        graphic = new ManagerGraphic(Viewport, GraphicAPI.OpenGL);
        Title = Application.ProductName;
    }

    private void OnClosing(object? sender, EventArgs e){
        Console.WriteLine("Closing...");
        graphic?.Dispose();
    }
}