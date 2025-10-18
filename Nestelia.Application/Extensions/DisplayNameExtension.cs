
using System.ComponentModel;
using System.Reflection;

namespace Nestelia.Application.Extensions
{
    public static class DisplayNameExtension
    {
        public static string GetDisplayName(this Type type)
        {
            if (Attribute.GetCustomAttribute(type, typeof(DisplayNameAttribute)) is DisplayNameAttribute displayNameAttribute)
            {
                return displayNameAttribute.DisplayName;
            }
            return type.Name;
        }
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.ToString();
        }
        public static string GetDescription(this Enum GenericEnum)
        {
            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (_Attribs != null && _Attribs.Count() > 0)
                {
                    return ((DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }
            return GenericEnum.ToString();
        }
    }
}
