﻿// ============================================================================
//    Author: Al-Khafaji, Ali Kifah
//    Date:   20.4.2022
//    Description: Fully serializable abstract class that implements Reflection to convert properties to and from binary data.
//    This class is meant to be used as a containing medium to hold and transfer Information on disk or over network.
//    The classes that inherit from this class can be converted to byte array and transferred over network or saved to disk as a binary file.
//
// ============================================================================


using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
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
        public static Type getDictKeyType(Type dictType)
        {
            if (dictType.IsGenericType && dictType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                return dictType.GetGenericArguments()[0];
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
        public static Type getDictValueType(Type dictType)
        {
            if (dictType.IsGenericType && dictType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                return dictType.GetGenericArguments()[1];
            }
            return null;
        }

        public static void WriteDict(BinaryWriter writer, object dict , Type keyType, Type valueType)
        {
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
                    writer.WriteDict_int_int(new Dictionary<int, int>());
            }

            else if (keyType == typeof(string) && valueType == typeof(int))
            {
                if (dict != null)
                    writer.WriteDict_str_int((Dictionary<string, int>)dict);
                else
                    writer.WriteDict_str_int(new Dictionary<string, int>());
            }
            else if (keyType == typeof(string) && (valueType.IsSubclassOf(typeof(Info)) || ReflectionProvider.isStruct(valueType)))
            {
                int count = 0;
                IDictionary tempDict = dict as IDictionary;
                if (tempDict == null)
                {
                    writer.Write(count);
                    return;
                }
                count = tempDict.Count;
                writer.Write(count);

                if (tempDict == null || count == 0)
                    return;
                foreach (string key in tempDict.Keys)
                {
                    writer.Write(key);
                    IOProvider.Write(writer, tempDict[key]);
                }
            }
            else if (keyType == typeof(int) && (valueType.IsSubclassOf(typeof(Info)) || ReflectionProvider.isStruct(valueType)))
            {
                int count = 0;
                IDictionary tempDict = dict as IDictionary;
                if (tempDict == null)
                {
                    writer.Write(count);
                    return;
                }
                count = tempDict.Count;
                writer.Write(count);

                if (tempDict == null || count == 0)
                    return;
                foreach (int key in tempDict.Keys)
                {
                    writer.Write(key);
                    IOProvider.Write(writer, tempDict[key]);
                }
            }
        }

  
        public static IDictionary ReadDict(BinaryReader reader, PropertyInfo prop)
        {
            Type keyType = getDictKeyType(prop);
            Type valueType = getDictValueType(prop);
            if (keyType == typeof(string) && valueType == typeof(string))
               return  reader.ReadDict_str_str();
            else if (keyType == typeof(int) && valueType == typeof(string))
                return reader.ReadDict_int_str();
            else if (keyType == typeof(int) && valueType == typeof(int))
                return reader.ReadDict_int_int();
            else if (keyType == typeof(string) && valueType == typeof(int))
                return reader.ReadDict_str_int();
            else if (keyType == typeof(string) && (valueType.IsSubclassOf(typeof(Info)) || ReflectionProvider.isStruct(valueType)))
            {
                int count = reader.ReadInt32();
                IDictionary dic = getDictOftype(keyType, valueType);
                for (int i = 0; i < count; i++)
                {
                    var value = Activator.CreateInstance(valueType);
                    string keyObject = reader.ReadString();
                    object valueObject = IOProvider.Read(reader, value);
                    dic.Add(keyObject, valueObject);
                }
                return dic;
            }
            else if (keyType == typeof(int) && (valueType.IsSubclassOf(typeof(Info)) || ReflectionProvider.isStruct(valueType)))
            {
                int count = reader.ReadInt32();
                IDictionary dic = getDictOftype(keyType, valueType);
                for (int i = 0; i < count; i++)
                {
                    var value = Activator.CreateInstance(valueType);
                    int keyObject = reader.ReadInt32();
                    object valueObject = IOProvider.Read(reader, value);
                    dic.Add(keyObject, valueObject);
                }
                return dic;
            }
            return null;
        }
        public static IDictionary ReadDict(BinaryReader reader, Type keyType, Type valueType)
        {
            if (keyType == typeof(string) && valueType == typeof(string))
                return reader.ReadDict_str_str();
            else if (keyType == typeof(int) && valueType == typeof(string))
                return reader.ReadDict_int_str();
            else if (keyType == typeof(int) && valueType == typeof(int))
                return reader.ReadDict_int_int();
            else if (keyType == typeof(string) && valueType == typeof(int))
                return reader.ReadDict_str_int();
            else if (keyType == typeof(string) && (valueType.IsSubclassOf(typeof(Info)) || ReflectionProvider.isStruct(valueType)))
            {
                int count = reader.ReadInt32();
                IDictionary dic = getDictOftype(keyType, valueType);
                for (int i = 0; i < count; i++)
                {
                    var value = Activator.CreateInstance(valueType);
                    string keyObject = reader.ReadString();
                    object valueObject = IOProvider.Read(reader, value);
                    dic.Add(keyObject, valueObject);
                }
                return dic;
            }
            else if (keyType == typeof(int) && (valueType.IsSubclassOf(typeof(Info)) || ReflectionProvider.isStruct(valueType)))
            {
                int count = reader.ReadInt32();
                IDictionary dic = getDictOftype(keyType, valueType);
                for (int i = 0; i < count; i++)
                {
                    var value = Activator.CreateInstance(valueType);
                    int keyObject = reader.ReadInt32();
                    object valueObject = IOProvider.Read(reader, value);
                    dic.Add(keyObject, valueObject);
                }
                return dic;
            }
            return null;
        }



        private static IDictionary getDictOftype(Type keyType, Type valuetype)
        {
            Type dl = typeof(Dictionary<,>);
            Type dictType = dl.MakeGenericType(keyType, valuetype);
            return (IDictionary)Activator.CreateInstance(dictType);
        }

        public static object getDefaultDict(Type DictType)
        {
            return ReflectionProvider.GetInstanceOftype(DictType);
        }

        public static void ResetDict(object instance, PropertyInfo prop)
        {
            Type keyType = getDictKeyType(prop);
            Type valueType = getDictValueType(prop);
            var dict = getDictOftype( keyType, valueType);
            prop.SetValue(instance, dict);
        }



    }
}
