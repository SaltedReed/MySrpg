using System;


namespace LitJson
{

    /// <summary>
    /// ������Ӹ�����ʱ��������ᱻ��ȷ�����л�
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class PolymorphismJsonAttribute : Attribute
    {
    }

}