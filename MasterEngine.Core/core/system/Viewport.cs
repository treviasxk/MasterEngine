using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Platform;
using MasterEngine.Graphic;
using MasterEngine.Runtime;

namespace MasterEngine;
public class Viewport : NativeControlHost{
    [DllImport("user32.dll")]
    public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    nint Handle {get;}
    public Viewport(nint handle){
        Handle = handle;
    }

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent){
        var view = base.CreateNativeControlCore(parent);
        switch(Application.Platform){
            case Platform.Windows:  // set DXHandle
                SetParent(Handle, view.Handle);
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