

using System.Reflection;
using System;
using System.IO;
using System.Collections.Generic;

namespace InfoObject
{

    public static class ReflectionProvider
    {
        public static PropertyInfo[] GetProperties(object infoclass)
        {
            PropertyInfo[] properties = infoclass.GetType().GetProperties();
            return properties;
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

        internal static PropertyInfo[] GetProperties<T>(object info)
        {
            PropertyInfo[] properties = info.GetType().GetProperties();
            return properties;
        }

        //#################  list parsing code ########################################################
        public static bool isList(PropertyInfo prop)
        {
            Type type = prop.PropertyType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return true;
            return false;
        }


        public static Type getListType(PropertyInfo prop)
        {
            Type type = prop.PropertyType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return type.GetGenericArguments()[0]; // use this...
            }
            return null;
        }


        public static void WriteList(BinaryWriter w, PropertyInfo prop, object list)
        {
            Type t = getListType(prop);
            if (t == typeof(string))
            {
                if (((List<string>)list) != null)
                {
                    w.WriteStringArray(((List<string>)list).ToArray());
                }
                else
                {
                    List<string> strings = new List<string>();
                    w.WriteStringArray(strings.ToArray());
                }
            }
            else if (t == typeof(int))
            {
                if (((List<int>)list) != null)
                {
                    w.WriteIntArray(((List<int>)list).ToArray());
                }
                else
                {
                    List<int> ints = new List<int>();
                    w.WriteIntArray(ints.ToArray());
                }
            }
            else if (t == typeof(float))
            {
                if (((List<float>)list) != null)
                {
                    w.WriteFloatArray(((List<float>)list).ToArray());
                }
                else
                {
                    List<float> floats = new List<float>();
                    w.WriteFloatArray(floats.ToArray());
                }
            }
            else if (t == typeof(byte))
            {
                if (((List<byte>)list) != null)
                {
                    w.WriteByteArray(((List<byte>)list).ToArray());
                }
                else
                {
                    List<byte> bytes = new List<byte>();
                    w.WriteByteArray(bytes.ToArray());
                }
            }
            else if (t.IsSubclassOf(typeof(Info)))
            {

            }

            Console.WriteLine("----- list type:" + t);

        }


        public static void ReadList(BinaryReader r, object instance, PropertyInfo prop)//, object list)
        {
            Type t = getListType(prop);
            if (t == typeof(string))
            {
                List<string> lst = new List<string>();
                lst.AddRange(r.ReadStringArray());
                prop.SetValue(instance, lst);
            }
            else if (t == typeof(int))
            {
                List<int> lst = new List<int>();
                lst.AddRange(r.ReadIntArray());
                prop.SetValue(instance, lst);
            }
            else if (t == typeof(float))
            {
                List<float> lst = new List<float>();
                lst.AddRange(r.ReadFloatArray());
                prop.SetValue(instance, lst);
            }
            else if (t == typeof(byte))
            {
                List<byte> lst = new List<byte>();
                lst.AddRange(r.ReadByteArray());
                prop.SetValue(instance, lst);
            }

        }

        //############# dict parsing code ##########################################################
        public static bool isDict(PropertyInfo prop)
        {
            Type type = prop.PropertyType;
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

    }
}



