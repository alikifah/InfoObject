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

        internal static void WriteList(BinaryWriter w, PropertyInfo prop, object list)
        {
            Type type = getListType(prop);
            if (type == typeof(string))
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
            else if (type == typeof(int))
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
            else if (type == typeof(float))
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
            else if (type == typeof(byte))
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
            else if ( type.IsSubclassOf(typeof(Info)) || ReflectionProvider.isStruct(type)  )
            {
                int count = 0;
                IList tempList = list as IList;
                if (tempList == null)
                {
                    w.Write(count);
                    return;
                }
                count = tempList.Count;
                w.Write(count);
                if (tempList == null || count == 0)
                    return;
                foreach (object item in tempList )
                {
                    IOProvider.Write(w, item);
                }
            }
        }


        internal static void ReadList(BinaryReader r, object instance, PropertyInfo prop)
        {
            Type type = getListType(prop);
            if (type == typeof(string))
            {
                List<string> lst = new List<string>();
                lst.AddRange(r.ReadStringArray());
                prop.SetValue(instance, lst);
            }
            else if (type == typeof(int))
            {
                List<int> lst = new List<int>();
                lst.AddRange(r.ReadIntArray());
                prop.SetValue(instance, lst);
            }
            else if (type == typeof(float))
            {
                List<float> lst = new List<float>();
                lst.AddRange(r.ReadFloatArray());
                prop.SetValue(instance, lst);
            }
            else if (type == typeof(byte))
            {
                List<byte> lst = new List<byte>();
                lst.AddRange(r.ReadByteArray());
                prop.SetValue(instance, lst);
            }
            else if (type.IsSubclassOf(typeof(Info)) || ReflectionProvider.isStruct(type)  )
            {
                int count = r.ReadInt32();
                IList list = getListOftype(type);
                for (int i = 0; i < count; i++)
                {
                    var item = Activator.CreateInstance(type);
                    object ob = IOProvider.Read(r, item);
                    list.Add(ob);
                }
                prop.SetValue(instance, list);
            }

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


        private static IList getListOftype(Type listType)
        {
            Type dl = typeof(List<>);
            Type infoType = dl.MakeGenericType(listType);
            IList list = (IList)Activator.CreateInstance(infoType);
            return list;
        }



    }
}



