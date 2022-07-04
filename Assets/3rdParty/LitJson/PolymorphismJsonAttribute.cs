using System;


namespace LitJson
{

    /// <summary>
    /// 父类添加该属性时，其子类会被正确反序列化
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class PolymorphismJsonAttribute : Attribute
    {
    }

}