using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class FuncTagAttribute : Attribute
{
    public string Tag { get; private set; }

    public FuncTagAttribute(string _Tag)
    {
        Tag = _Tag;
    }
}
