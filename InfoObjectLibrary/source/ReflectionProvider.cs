
using System.Reflection;
using System;

namespace InfoObject
{
    internal static class ReflectionProvider
    {
        internal static PropertyInfo[] GetProperties(object infoclass)
        {
            PropertyInfo[] properties = infoclass.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return properties;
        }

        internal static FieldInfo[] GetFieldMembers(object structure)
        {
            FieldInfo[] members = structure.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            return members;
        }

        public static string GetPropertyName<T>(PropertyInfo p)
        {
            return p.Name;
        }
        public static object GetPropertyValue(PropertyInfo p, object infoclass)
        {
            return p.GetValue(infoclass, null);
        }
        public static Type GetPropertyType(PropertyInfo p)
        {
            return p.PropertyType;
        }
        public static object GetInstanceOftype(Type type)
        {
            return (object)Activator.CreateInstance(type);
        }
        public static Type GetMemberType(MemberInfo memberInfo)
        {
            var tryMember = memberInfo as PropertyInfo;
            Type type;
            if (tryMember != null)// if property
                type = tryMember.PropertyType;
            else// if field
                type = ((FieldInfo)memberInfo).FieldType;
            return type;
        }
        public static object GetMemberValue(MemberInfo memberInfo, object ob)
        {
            var tryMember = memberInfo as PropertyInfo;
            object value;
            if (tryMember != null)// if property
                value = tryMember.GetValue(ob);
            else// if field
                value = ((FieldInfo)memberInfo).GetValue(ob);
            return value;
        }


        public static object GetDefaultInstance(string TypeName = "")
        {
            Type t = TypeFromString(TypeName);
            return GetInstanceOftype(t);
        }


        public static object GetDefaultValue(Type type)
        {
            if (type == typeof(string))
                return "";
            else if (type == typeof(string[]))
                return Array.Empty<string>();
            else if (type == typeof(int))
                return 0;
            else if (type == typeof(float))
                return default(float);
            else if (type == typeof(short))
                return 0;
            else if (type == typeof(long))
                return 0;
            else if (type == typeof(bool))
                return false;
            else if (type == typeof(byte))
                return 0;
            else if (type == typeof(char))
                return '\0';
            else if (type == typeof(double))
                return default(double);
            else if (type == typeof(byte[]))
                return Array.Empty<byte>();
            else if (type == typeof(int[]))
                return Array.Empty<int>();
            else if (type == typeof(float[]))
                return Array.Empty<float>();
            else if (type == typeof(long[]))
                return Array.Empty<long>();
            else if (type == typeof(short[]))
                return Array.Empty<short>();
            else if (type == typeof(double[]))
                return Array.Empty<double>();
            return null;
        }

        public static bool isStruct(Type objectType)
        {
            return objectType.IsValueType &&
                !objectType.IsEnum &&
                objectType != typeof(Decimal) &&
                objectType != typeof(Double) &&
                objectType != typeof(Single) &&
                objectType != typeof(Int64) &&
                objectType != typeof(Int32) &&
                objectType != typeof(Int16);
        }
        public static object GetDefaultStruct(MemberInfo memberInfo)
        {
            return GetInstanceOftype(GetMemberType(memberInfo));
        }

        public static Type TypeFromString(string typeFullName)
        {
            return Type.GetType(typeFullName);
        }

        public static bool IsTypeValid(string typeFullName)
        {
            if (TypeFromString(typeFullName) == null)
                return false;
            return true;
        }

        //#################################################################

        public static void getMemeberTypeValue(object instance, MemberInfo memberinfo, bool isStruct, out object value, out Type type)
        {
            if (isStruct)
            {
                type = ((FieldInfo)memberinfo).FieldType;
                value = ((FieldInfo)memberinfo).GetValue(instance);
            }
            else
            {
                type = ((PropertyInfo)memberinfo).PropertyType;
                value = ((PropertyInfo)memberinfo).GetValue(instance);
            }
        }
        public static MemberInfo[] getMembers(object instance, out bool isStruct)
        {
            Type instType = instance.GetType();
            if (ReflectionProvider.isStruct(instType))
            {
                isStruct = true;
                return ReflectionProvider.GetFieldMembers(instance);
            }
            else
            {
                isStruct = false;
                return ReflectionProvider.GetProperties(instance);
            }
        }









    }
}



