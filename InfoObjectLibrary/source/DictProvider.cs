using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace InfoObject
{
    internal class DictProvider
    {

        internal static bool isDict(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                return true;
            return false;
        }

        public static Type getDictKeyType(PropertyInfo prop)
        {
            Type type = prop.PropertyType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                return type.GetGenericArguments()[0];
            }
            return null;
        }
        public static Type getDictValueType(PropertyInfo prop)
        {
            Type type = prop.PropertyType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                return type.GetGenericArguments()[1];
            }
            return null;
        }

        public static void WriteDict(BinaryWriter writer, PropertyInfo prop, object dict)
        {
            Type keyType = getDictKeyType(prop);
            Type valueType = getDictValueType(prop);
            if (keyType == typeof(string) && valueType == typeof(string))
            {
                if (dict != null)
                    writer.WriteDict_str_str((Dictionary<string, string>)dict);
                else
                    writer.WriteDict_str_str(new Dictionary<string, string>());
            }
            else if (keyType == typeof(int) && valueType == typeof(string))
            {
                if (dict != null)
                    writer.WriteDict_int_str((Dictionary<int, string>)dict);
                else
                    writer.WriteDict_int_str(new Dictionary<int, string>());
            }
            else if (keyType == typeof(int) && valueType == typeof(int))
            {
                if (dict != null)
                    writer.WriteDict_int_int((Dictionary<int, int>)dict);
                else
                    writer.WriteDict_int_int(new Dictionary<int, int>() );
            }

            else if (keyType == typeof(string) && valueType == typeof(int))
            {
                if (dict != null)
                    writer.WriteDict_str_int((Dictionary<string, int>)dict);
                else
                    writer.WriteDict_str_int(new Dictionary<string, int>());
            }
        }

        public static void ReadDict(BinaryReader reader, object instance, PropertyInfo prop)
        {
            Type keyType = getDictKeyType(prop);
            Type valueType = getDictValueType(prop);
            if (keyType == typeof(string) && valueType == typeof(string))
                prop.SetValue(instance, reader.ReadDict_str_str());
            else if (keyType == typeof(int) && valueType == typeof(string))
                prop.SetValue(instance, reader.ReadDict_int_str());
            else if (keyType == typeof(int) && valueType == typeof(int))
                prop.SetValue(instance, reader.ReadDict_int_int());
            else if (keyType == typeof(string) && valueType == typeof(int))
                prop.SetValue(instance, reader.ReadDict_str_int());
        }


        public static object getDefaultDict(Type DictType)
        {
            return ReflectionProvider.GetInstanceOftype(DictType);
        }

        public static void ResetDict(object instance, PropertyInfo prop)
        {
            Type keyType = getDictKeyType(prop);
            Type valueType = getDictValueType(prop);
            if (keyType == typeof(string) && valueType == typeof(string))
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                prop.SetValue(instance, d);
            }
            else if (keyType == typeof(int) && valueType == typeof(string))
            {
                Dictionary<int, string> d = new Dictionary<int, string>();
                prop.SetValue(instance, d);
            }
            else if (keyType == typeof(int) && valueType == typeof(int))
            {
                Dictionary<int, int> d = new Dictionary<int, int>();
                prop.SetValue(instance, d);
            }
            else if (keyType == typeof(string) && valueType == typeof(int))
            {
                Dictionary<string, int> d = new Dictionary<string, int>();
                prop.SetValue(instance, d);
            }

        }



    }
}
