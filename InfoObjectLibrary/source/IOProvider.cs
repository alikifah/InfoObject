using System;
using System.IO;
using System.Reflection;

namespace InfoObject
{
    internal static class IOProvider
    {
        public static void WriteTypeSignature(BinaryWriter writer, Info instance)
        {
            Type type = instance.GetType();
            string name = type.FullName;
            string assemblyName = type.Assembly.GetName().Name;
            writer.Write(name);
            writer.Write(assemblyName);
        }
        public static bool CheckTypeCompatibility(BinaryReader reader, Info instance)
        {
            string typeNameFromData = reader.ReadString();
            string assemblyName = reader.ReadString();
            string typeNameFromObject = instance.GetType().FullName;
            if (!ReflectionProvider.IsTypeValid(typeNameFromData + ", " + assemblyName)) // check if the type is of known type
                return false;
            if (typeNameFromObject != typeNameFromData) // check if the types are the same
                return false;
            return true;
        }

        public static byte[] FileToBinary(string filepath, out int size)
        {
            FileStream stream = File.OpenRead(filepath);
            size = (int)stream.Length;
            byte[] fileBytes = new byte[size];
            stream.Read(fileBytes, 0, fileBytes.Length);
            stream.Close();
            return fileBytes;
        }
        public static void BinaryToFile(byte[] fileBytes, string filepath)
        {
            FileStream stream = File.OpenWrite(filepath);
            stream.Write(fileBytes, 0, fileBytes.Length);
            stream.Close();
        }


        public static void ResetInstance(object instance)
        {
            System.Reflection.PropertyInfo[] props = ReflectionProvider.GetProperties(instance);
            foreach (var property in props)
            {
                Type type = property.PropertyType;
                object value = property.GetValue(instance);

                if (type.IsSubclassOf(typeof(Info)))
                {
                    value = ReflectionProvider.GetDefaultInstance(property);
                    property.SetValue(instance, value);
                }
                else if (ListProvider.isList(type))
                {
                    var list = ListProvider.getDefaultList(property, instance);
                    property.SetValue(instance, list);

                }
                else if (ReflectionProvider.isStruct(type))
                {
                    var structure = ReflectionProvider.GetDefaultStruct(property);
                    property.SetValue(instance, structure);
                }
                else if (DictProvider.isDict(type))
                {
                    var dict = DictProvider.getDefaultDict(type);
                    property.SetValue(instance, dict);
                }
                else
                {
                    value = ReflectionProvider.GetDefaultValue(type);
                    property.SetValue(instance, value);
                }
            }
        }

        public static void Write(BinaryWriter writer, object instance)
        {
            MemberInfo[] members = getMembers(instance , out bool isStruct);

            foreach (var member in members)
            {
                getMemeberTypeValue(instance, member, isStruct, out object value, out Type type);

                if (value == null) value = ReflectionProvider.GetDefaultValue(type);

                if (type == typeof(string))
                    writer.Write((string)value);
                else if (type == typeof(string[]))
                    writer.WriteStringArray((string[])value);

                else if (type == typeof(short))
                    writer.Write((short)value);
                else if (type == typeof(int))
                    writer.Write((int)value);
                else if (type == typeof(long))
                    writer.Write((long)value);

                else if (type == typeof(float))
                    writer.Write((float)value);
                else if (type == typeof(double))
                    writer.Write((double)value);
                else if (type == typeof(decimal))
                    writer.Write((decimal)value);

                else if (type == typeof(bool))
                    writer.Write((bool)value);
                else if (type == typeof(byte))
                    writer.Write((byte)value);
                else if (type == typeof(char))
                    writer.Write((char)value);

                else if (type == typeof(byte[]))
                    writer.WriteByteArray((byte[])value);
                else if (type == typeof(int[]))
                    writer.WriteIntArray((int[])value);
                else if (type == typeof(float[]))
                    writer.WriteFloatArray((float[])value);
                else if (type.IsSubclassOf(typeof(Info)) || ReflectionProvider.isStruct(type))
                {
                    if (value == null && !isStruct)
                        value = ReflectionProvider.GetDefaultInstance((PropertyInfo)member);
                    Write(writer, value);
                }
                else if (ListProvider.isList(type))
                    ListProvider.WriteList(writer, (PropertyInfo)member, value);
                else if (DictProvider.isDict(type))
                    DictProvider.WriteDict(writer, (PropertyInfo)member, value);
            }
        }

        public static object Read(BinaryReader reader, object instance)
        {
            MemberInfo[] members = getMembers(instance, out bool isStruct);

            foreach (var member in members)
            {
                getMemeberTypeValue(instance, member, isStruct, out object value, out Type type);

                if (value == null) value = ReflectionProvider.GetDefaultValue(type);

                if (type == typeof(string))
                    member.SetValue(instance, reader.ReadString());
                else if (type == typeof(string[]))
                    member.SetValue(instance, reader.ReadStringArray());

                else if (type == typeof(short))
                    member.SetValue(instance, reader.ReadInt16());
                else if (type == typeof(int))
                    member.SetValue(instance, reader.ReadInt32());
                else if (type == typeof(long))
                    member.SetValue(instance, reader.ReadInt64());

                else if (type == typeof(float))
                    member.SetValue(instance, reader.ReadSingle());
                else if (type == typeof(double))
                    member.SetValue(instance, reader.ReadDouble());
                else if (type == typeof(decimal))
                    member.SetValue(instance, reader.ReadDecimal());

                else if (type == typeof(bool))
                    member.SetValue(instance, reader.ReadBoolean());
                else if (type == typeof(byte))
                    member.SetValue(instance, reader.ReadByte());
                else if (type == typeof(char))
                    member.SetValue(instance, reader.ReadChar());

                else if (type == typeof(byte[]))
                    member.SetValue(instance, reader.ReadByteArray());
                else if (type == typeof(int[]))
                    member.SetValue(instance, reader.ReadIntArray());
                else if (type == typeof(float[]))
                    member.SetValue(instance, reader.ReadFloatArray());
                else if (type.IsSubclassOf(typeof(Info)) || ReflectionProvider.isStruct(type))
                {
                    if (value == null && !isStruct)
                        value = ReflectionProvider.GetDefaultInstance((PropertyInfo)member);
                    object ob = Read(reader, value);
                    member.SetValue(instance, ob);
                }
                else if (ListProvider.isList(type))
                    ListProvider.ReadList(reader, instance, (PropertyInfo)member);
                else if (DictProvider.isDict(type))
                    DictProvider.ReadDict(reader, instance, (PropertyInfo)member);
            }
            return instance;
        }

        private static void getMemeberTypeValue(object instance, MemberInfo memberinfo, bool isStruct, out object value, out Type type)
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
        private static MemberInfo[] getMembers(object instance, out bool isStruct)
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
