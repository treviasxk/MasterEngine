
using MasterEngine.Graphic;

namespace MasterEngine.Runtime;
public class Application {
    public static Platform Platform {get{return CheckPlatform();}}
    public static int FrameRate {get;set;} = 60;

    static Platform CheckPlatform(){
        if(OperatingSystem.IsWindows())
            return Platform.Windows;

        if(OperatingSystem.IsLinux())
            return Platform.Linux;

        if(OperatingSystem.IsMacOS())
            return Platform.Mac;

        if(OperatingSystem.IsAndroid())
            return Platform.Android;

        return Platform.Unknown;
    }
}