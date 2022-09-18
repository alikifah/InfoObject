// ============================================================================
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
using System.Text;
using System.Reflection;
namespace InfoObject
{
    internal static class ObjectProvider
    {
        // there are no metadata about lists dicts structs and InfoObjects in type object, therefore we must save the type in a string
        public static Type GetObjectType(object ob)
        {
            if (ob == null)
                return null;
            return ob.GetType();
        }

        public static void WriteObject(BinaryWriter writer,  object value)
        {
            string objectMarker = "";
            if (value == null)
            {
                objectMarker = "<object>" + null + "</object>";
                writer.Write(objectMarker);
                return;
            }
            Type objType = GetObjectType(value);

            objectMarker = "<object>" + ReflectionProvider.GetSignature(objType) + "</object>";
            writer.Write(objectMarker);

            if (objType == typeof(string))
                writer.Write((string)value);
            else if (objType == typeof(string[]))
                writer.WriteStringArray((string[])value);

            else if (objType == typeof(short))
                writer.Write((short)value);
            else if (objType == typeof(int))
                writer.Write((int)value);
            else if (objType == typeof(long))
                writer.Write((long)value);

            else if (objType == typeof(float))
                writer.Write((float)value);
            else if (objType == typeof(double))
                writer.Write((double)value);
            else if (objType == typeof(decimal))
                writer.Write((decimal)value);

            else if (objType == typeof(bool))
                writer.Write((bool)value);
            else if (objType == typeof(byte))
                writer.Write((byte)value);
            else if (objType == typeof(char))
                writer.Write((char)value);

            else if (objType == typeof(byte[]))
                writer.WriteByteArray((byte[])value);
            else if (objType == typeof(int[]))
                writer.WriteIntArray((int[])value);
            else if (objType == typeof(float[]))
                writer.WriteFloatArray((float[])value);
            else if (objType.IsEnum)
            {
                var enummarker = "<enum>" + ReflectionProvider.GetSignature(objType)  + "</enum>";
                writer.Write(enummarker );
                writer.Write((int)value);
            }
            else if (InfoProvider.IsInfo(objType))
                InfoProvider.WriteInfoObject(writer, objType, value);

            else if (ReflectionProvider.isStruct(objType))
                IOProvider.Write(writer, value);

            else if (ListProvider.isList(objType))
            {
                var listType = ListProvider.getListType(objType);
                var listMarker = "<list>" + ReflectionProvider.GetSignature(listType) + "</list>";
                writer.Write(listMarker);

                ListProvider.WriteList(writer, listType, value);
            }
            else if (DictProvider.isDict(objType))
            {
                var dictKey = DictProvider.getDictKeyType(objType);
                var dictValue = DictProvider.getDictValueType(objType);

                var dictKeyMarker = "<key>" + ReflectionProvider.GetSignature(dictKey) + "</key>";
                var dictValueMarker = "<value>" + ReflectionProvider.GetSignature(dictValue) + "</value>";
                writer.Write(dictKeyMarker);
                writer.Write(dictValueMarker);
                DictProvider.WriteDict(writer, value, dictKey, dictValue);
            }
        }

        // "<enum>" + objType + "</enum>";
        public static string ReadEnumType(string marker)
        {
            string s = marker.Substring(6, marker.Length - 13);
            if (s.Length == 0) s = null;
            return s;
        }

        // "<object>infoTypename</object>"
        public static string ReadObjectType(string marker)
        {
            string s = marker.Substring(8, marker.Length - 17);
            if (s.Length == 0) s = null;
            return s;
        }
        // "<list>infoTypename</list>"
        public static string ReadListType(string marker)
        {
            string s = marker.Substring(6, marker.Length - 13);
            if (s.Length == 0) s = null;
            return s;
        }

        // "<key>infoTypename</key>"
        public static string ReadDictKeyType(string marker)
        {
            string s = marker.Substring(5, marker.Length - 11);
            if (s.Length == 0) s = null;
            return s;
        }

        // "<value>infoTypename</value>"
        public static string ReadDictValueType(string marker)
        {
            string s = marker.Substring(7, marker.Length - 15);
            if (s.Length == 0) s = null;
            return s;
        }

        public static object ReadObject(BinaryReader reader)
        {
            string objMarker = reader.ReadString();
            string objTypeStr = ReadObjectType(objMarker);

            if (objTypeStr == null)
                return null;

            Type objType = ReflectionProvider.TypeFromString(objTypeStr);

            if (objType == typeof(string))
                return reader.ReadString();
            else if (objType == typeof(string[]))
                return reader.ReadStringArray();

            else if (objType == typeof(short))
                return reader.ReadInt16();
            else if (objType == typeof(int))
                return reader.ReadInt32();
            else if (objType == typeof(long))
                return reader.ReadInt64();

            else if (objType == typeof(float))
                return reader.ReadSingle();
            else if (objType == typeof(double))
                return reader.ReadDouble();
            else if (objType == typeof(decimal))
                return reader.ReadDecimal();

            else if (objType == typeof(bool))
                return reader.ReadBoolean();
            else if (objType == typeof(byte))
                return reader.ReadByte();
            else if (objType == typeof(char))
                return reader.ReadChar();

            else if (objType == typeof(byte[]))
                return reader.ReadByteArray();
            else if (objType == typeof(int[]))
                return reader.ReadIntArray();
            else if (objType == typeof(float[]))
                return reader.ReadFloatArray();
            else if (objType.IsEnum)
            {
                var enumTypeStr = ReadEnumType(reader.ReadString());
                object ob= new object();
                if (Type.GetType(enumTypeStr) != null)
                {
                    var enumType = Type.GetType(enumTypeStr);
                    if (enumType.IsEnum)
                    {
                        ob = Enum.ToObject(enumType, reader.ReadInt32());
                    }
                }
                return ob;
            }
            else if (InfoProvider.IsInfo(objType))
                return InfoProvider.ReadInfoObjectBack(reader);

            else if (ReflectionProvider.isStruct(objType))
                return IOProvider.Read(reader, ReflectionProvider.GetInstanceOftype(objType));
            else if (ListProvider.isList(objType))
            {
                string listmarker = reader.ReadString();
                string listTypeStr = ReadListType(listmarker);
                var listType = ReflectionProvider.TypeFromString(listTypeStr);
                return ListProvider.ReadList(reader, listType);
            }
            else if (DictProvider.isDict(objType))
            {
                string keyMarker = reader.ReadString();
                string valuemarker = reader.ReadString();
                var keyType = ReflectionProvider.TypeFromString(ReadDictKeyType(keyMarker));
                var valueType = ReflectionProvider.TypeFromString(ReadDictValueType(valuemarker));
                return DictProvider.ReadDict(reader, keyType, valueType);
            }

            return null;
        }
        
    }
}
