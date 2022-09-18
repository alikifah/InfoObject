// ============================================================================
//    Author: Al-Khafaji, Ali Kifah
//    Date:   20.4.2022
//    Description: Fully serializable abstract class that implements Reflection to convert properties to and from binary data.
//    This class is meant to be used as a containing medium to hold and transfer Information on disk or over network.
//    The classes that inherit from this class can be converted to byte array and transferred over network or saved to disk as a binary file.
//
// ============================================================================

using System.Reflection;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace InfoObject
{
    internal static class ListProvider
    {
        internal static bool isList(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return true;
            return false;
        }
        internal static bool isList(FieldInfo prop)
        {
            return isList(prop.FieldType);
        }
        internal static bool isList(PropertyInfo prop)
        {
            return isList(prop.PropertyType);
        }

        internal static Type getListType(Type type )
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return type.GetGenericArguments()[0];
            }
            return null;
        }

        internal static Type getListType(PropertyInfo prop)
        {
            Type type = prop.PropertyType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return type.GetGenericArguments()[0];
            }
            return null;
        }
        internal static Type getListType(FieldInfo prop)
        {
            Type type = prop.FieldType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return type.GetGenericArguments()[0];
            }
            return null;
        }




        internal static void WriteList(BinaryWriter writer, Type listType, object list)
        {
            if (listType == typeof(string))
            {
                if (((List<string>)list) != null)
                {
                    writer.WriteStringArray(((List<string>)list).ToArray());
                }
                else
                {
                    List<string> strings = new List<string>();
                    writer.WriteStringArray(strings.ToArray());
                }
            }
            else if (listType == typeof(int))
            {
                if (((List<int>)list) != null)
                {
                    writer.WriteIntArray(((List<int>)list).ToArray());
                }
                else
                {
                    List<int> ints = new List<int>();
                    writer.WriteIntArray(ints.ToArray());
                }
            }
            else if (listType == typeof(float))
            {
                if (((List<float>)list) != null)
                {
                    writer.WriteFloatArray(((List<float>)list).ToArray());
                }
                else
                {
                    List<float> floats = new List<float>();
                    writer.WriteFloatArray(floats.ToArray());
                }
            }
            else if (listType == typeof(byte))
            {
                if (((List<byte>)list) != null)
                {
                    writer.WriteByteArray(((List<byte>)list).ToArray());
                }
                else
                {
                    List<byte> bytes = new List<byte>();
                    writer.WriteByteArray(bytes.ToArray());
                }
            }
            else if (listType.IsSubclassOf(typeof(Info)) || ReflectionProvider.isStruct(listType))
            {
                int count = 0;
                IList tempList = list as IList;
                if (tempList == null)
                {
                    writer.Write(count);
                    return;
                }
                count = tempList.Count;
                writer.Write(count);
                if (tempList == null || count == 0)
                    return;
                foreach (object item in tempList)
                {
                    IOProvider.Write(writer, item);
                }
            }
            else if (listType == typeof(object))
            {
                var arr = ((List<object>)list).ToArray();
                writer.WriteObjectArray( arr);
            }

        }


        internal static IList ReadList(BinaryReader reader, PropertyInfo prop)
        {
            Type listType = getListType(prop);
            if (listType == typeof(string))
            {
                List<string> lst = new List<string>();
                lst.AddRange(reader.ReadStringArray());
                return lst;
            }
            else if (listType == typeof(int))
            {
                List<int> lst = new List<int>();
                lst.AddRange(reader.ReadIntArray());
                return lst;
            }
            else if (listType == typeof(float))
            {
                List<float> lst = new List<float>();
                lst.AddRange(reader.ReadFloatArray());
                return lst;
            }
            else if (listType == typeof(byte))
            {
                List<byte> lst = new List<byte>();
                lst.AddRange(reader.ReadByteArray());
                return lst;
            }
            else if (listType.IsSubclassOf(typeof(Info)) || ReflectionProvider.isStruct(listType))
            {
                int count = reader.ReadInt32();
                IList list = getListOftype(listType);
                for (int i = 0; i < count; i++)
                {
                    var item = Activator.CreateInstance(listType);
                    object ob = IOProvider.Read(reader, item);
                    list.Add(ob);
                }
                return list;
            }
            else if (listType == typeof(object))
            {
                var array = reader.ReadObjectArray();
                List<object> list= new List<object>();
                list.AddRange(array);
                return list;
            }




            return null;
        }
        internal static IList ReadList(BinaryReader reader, Type listType)
        {
            if (listType == typeof(string))
            {
                List<string> lst = new List<string>();
                lst.AddRange(reader.ReadStringArray());
                return lst;
            }
            else if (listType == typeof(int))
            {
                List<int> lst = new List<int>();
                lst.AddRange(reader.ReadIntArray());
                return lst;
            }
            else if (listType == typeof(float))
            {
                List<float> lst = new List<float>();
                lst.AddRange(reader.ReadFloatArray());
                return lst;
            }
            else if (listType == typeof(byte))
            {
                List<byte> lst = new List<byte>();
                lst.AddRange(reader.ReadByteArray());
                return lst;
            }
            else if (listType.IsSubclassOf(typeof(Info)) || ReflectionProvider.isStruct(listType))
            {
                int count = reader.ReadInt32();
                IList list = getListOftype(listType);
                for (int i = 0; i < count; i++)
                {
                    var item = Activator.CreateInstance(listType);
                    object ob = IOProvider.Read(reader, item);
                    list.Add(ob);
                }
                return list;
            }
            else if (listType == typeof(object))
            {
                var array = reader.ReadObjectArray();
                List<object> list = new List<object>();
                list.AddRange(array);
                return list;
            }


            return null;
        }



        public static object getDefaultList(MemberInfo memberInfo, object instance)
        {
            var tryMember = memberInfo as PropertyInfo;
            object value;
            Type type;
            Type listType;
            if (tryMember != null)
            { // if property
                type = tryMember.PropertyType;
                value = tryMember.GetValue(instance);
                if (!isList(tryMember)) return null;
                listType = getListType(tryMember);
            }
            else
            {// if member
                type = ((FieldInfo)memberInfo).FieldType;
                value = ((FieldInfo)memberInfo).GetValue(instance);
                if (!isList(((FieldInfo)memberInfo))) return null;
                listType = getListType(((FieldInfo)memberInfo));
            }
            return getListOftype(listType);
        }

        internal static void ResetList(object instance, PropertyInfo prop)
        {
            Type type = getListType(prop);
            IList list = getListOftype(type);
            prop.SetValue(instance, list);
        }

        public static IList getListOftype(Type listType)
        {
            Type dl = typeof(List<>);
            Type infoType = dl.MakeGenericType(listType);
            IList list = (IList)Activator.CreateInstance(infoType);
            return list;
        }



    }
}



