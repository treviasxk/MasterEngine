using System;

namespace MasterEngine.Editor;
public class Debug {
    public static void Log(Object message) => Log(message.ToString());
    public static void Log(string? message){
        if(EditorWindow.Window != null)
            EditorWindow.Window.LabelStatusBarCompile.Text = message;
        Console.WriteLine(message);
    }
}