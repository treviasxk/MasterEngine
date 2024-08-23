using System;

namespace MasterEngine.Editor.Layout;
[AttributeUsage(AttributeTargets.Method)]
public class MenuItem : Attribute{
    public string Title {get;}
    public int index {get;}
    public Action Action {get;}
    public MenuItem(string Title, int index = 0){
        this.Title = Title;
        this.index = index;
        Action = ()=>{};
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class Range : Attribute{
    public int Value, Min, Max;
    public Range(int Min, int Max){
        
    }
    public Range(float Min, float Max){
        
    }
}