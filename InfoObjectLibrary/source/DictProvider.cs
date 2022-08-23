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

        public static void WriteDict(BinaryWriter w, PropertyInfo prop, object dict)
        {
            Type keyType = getDictKeyType(prop);
            Type valueType = getDictValueType(prop);
            if (keyType == typeof(string) && valueType == typeof(string))
            {
                if (((Dictionary<string, string>)dict) != null)
                {
                    w.WriteDict_str_str((Dictionary<string, string>)dict);
                }
                else
                {
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    w.WriteDict_str_str(d);
                }
            }
            else if (keyType == typeof(int) && valueType == typeof(string))
            {
                if (((Dictionary<int, string>)dict) != null)
                {
                    w.WriteDict_int_str((Dictionary<int, string>)dict);
                }
                else
                {
                    Dictionary<int, string> d = new Dictionary<int, string>();
                    w.WriteDict_int_str(d);
                }
            }
            else if (keyType == typeof(int) && valueType == typeof(int))
            {
                if (((Dictionary<int, int>)dict) != null)
                {
                    w.WriteDict_int_int((Dictionary<int, int>)dict);
                }
                else
                {
                    Dictionary<int, int> d = new Dictionary<int, int>();
                    w.WriteDict_int_int(d);
                }
            }

            else if (keyType == typeof(string) && valueType == typeof(int))
            {
                if (((Dictionary<string, int>)dict) != null)
                {
                    w.WriteDict_str_int((Dictionary<string, int>)dict);
                }
                else
                {
                    Dictionary<string, int> d = new Dictionary<string, int>();
                    w.WriteDict_str_int(d);
                }
            }

        }

        public static void ReadDict(BinaryReader r, object instance, PropertyInfo prop)
        {
            Type keyType = getDictKeyType(prop);
            Type valueType = getDictValueType(prop);
            if (keyType == typeof(string) && valueType == typeof(string))
            {
                Dictionary<string, string> d = r.ReadDict_str_str();
                prop.SetValue(instance, d);
            }
            else if (keyType == typeof(int) && valueType == typeof(string))
            {
                Dictionary<int, string> d = r.ReadDict_int_str();
                prop.SetValue(instance, d);
            }
            else if (keyType == typeof(int) && valueType == typeof(int))
            {
                Dictionary<int, int> d = r.ReadDict_int_int();
                prop.SetValue(instance, d);
            }
            else if (keyType == typeof(string) && valueType == typeof(int))
            {
                Dictionary<string, int> d = r.ReadDict_str_int();
                prop.SetValue(instance, d);
            }
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
