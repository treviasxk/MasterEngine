using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Platform;
using MasterEngine.Graphic;
using MasterEngine.Runtime;

namespace MasterEngine;
public class ViewNativeControl : NativeControlHost{
    [DllImport("user32.dll")]
    public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    nint Handle {get;}

    /// <summary>
    /// Set handle graphic to embude in content control Avalonia.
    /// </summary>
    /// <param name="handle"></param>
    public ViewNativeControl(nint handle){
        Handle = handle;
    }

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent){
        var view = base.CreateNativeControlCore(parent);
        switch(Application.Platform){
            case Platform.Windows:  // set DXHandle
                SetParent(Handle, view.Handle);
                SetWindowPos(Handle, 0, 0, 0, 0, 0, 0x0001 | 0x0040); 
            break;
            case Platform.Linux:  // set X11
                // code
            break;
        }
        return view;
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control){
        base.DestroyNativeControlCore(control);
    }
}