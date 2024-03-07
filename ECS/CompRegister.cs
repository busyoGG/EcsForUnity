

using System;

[System.AttributeUsage(System.AttributeTargets.Class |
                       System.AttributeTargets.Struct)
]
public class CompRegister : Attribute
{
    public CompRegister(Type type)
    {
        Register(type);
    }

    public void Register(Type type)
    {
        int id = ECSManager.Ins().GetCompId();
        ECSManager.Ins().CompRegister(type, id);
    }
}
