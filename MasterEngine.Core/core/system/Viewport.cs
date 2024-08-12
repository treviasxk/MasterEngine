

using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Platform;
using MasterEngine.Graphic;

namespace MasterEngine;
public class Viewport : NativeControlHost{
    [DllImport("user32.dll")]
    public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    nint Handle {get;}
    Platform Platform {get;}
    public Viewport(nint handle, Platform platform){
        Handle = handle;
        Platform = platform;
    }

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent){
        var view = base.CreateNativeControlCore(parent);
        switch(Platform){
            case Platform.Windows:  // set DXHandle
                SetParent(Handle, view.Handle);
            break;
        }
        return view;
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control){
        base.DestroyNativeControlCore(control);
    }
}