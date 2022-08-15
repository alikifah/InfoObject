// ============================================================================
//    Author: Al-Khafaji, Ali Kifah
//    Date:   20.4.2022
//    Description: A binary serializable abstract class that implements Reflection to convert properties to and from binary data.
//    This class is meant to be used as a containing medium to hold and transfer Information on disk or over network.
//    The classes that inherit from this class can be converted to byte array and transferred over network or saved to disk as a binary file.
//
// ============================================================================
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace InfoObject
{

    public abstract class Info
    {
        private int size = 0; // data size that this class holds(in Byte)
        public int GetSize()
        {
            return size;
        }
        private byte[] FileToBinary(string filepath)
        {
            FileStream stream = File.OpenRead(filepath);
            size = (int)stream.Length;
            byte[] fileBytes = new byte[size];
            stream.Read(fileBytes, 0, fileBytes.Length);
            stream.Close();
            return fileBytes;
        }
        private void BinaryToFile(byte[] fileBytes, string filepath)
        {
            FileStream stream = File.OpenWrite(filepath);
            stream.Write(fileBytes, 0, fileBytes.Length);
            stream.Close();
        }
        // load from byte array
        public virtual void Load(byte[] data)
        {
            size = data.Length;
            using (MemoryStream m = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(m))
                {
                    read(reader, this);
                }
            }
        }
        // load from file
        public void Load(string filePath)
        {
            Load(FileToBinary(filePath));
        }

        // Deserialize from byte array
        public static T Deserialize<T>(byte[] data) where T : Info, new()
        {
            var o = new T();
            o.Load(data);
            return o;
        }
        // Deserialize from file
        public static T Deserialize<T>(string filePath) where T : Info, new()
        {
            var o = new T();
            o.Load(filePath);
            return o;
        }
        /// <summary>
        /// Save the Info object to file
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            BinaryToFile(Serialize(), filePath);
        }
        /// <summary>
        /// convert the Info object to byte array
        /// </summary>
        /// <returns></returns>
        public virtual byte[] Serialize()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                    write(writer, this);
                }
                return m.ToArray();
            }
        }

        private void write(BinaryWriter writer, Info instance)
        {
            PropertyInfo[] prop = instance.GetType().GetProperties();
            foreach (var t in prop)
            {
                object? v = t.GetValue(instance);
                if (v == null) v = getDefaultProperty(t);
                if (t.PropertyType == typeof(string))
                    writer.Write((string)v);
                else if (t.PropertyType == typeof(string[]))
                    writer.WriteStringArray((string[])v);
                else if (t.PropertyType == typeof(int))
                    writer.Write((int)v);
                else if (t.PropertyType == typeof(float))
                    writer.Write((float)v);
                else if (t.PropertyType == typeof(bool))
                    writer.Write((bool)v);
                else if (t.PropertyType == typeof(byte))
                    writer.Write((byte)v);
                else if (t.PropertyType == typeof(char))
                    writer.Write((char)v);
                else if (t.PropertyType == typeof(double))
                    writer.Write((double)v);
                else if (t.PropertyType == typeof(byte[]))
                    writer.WriteByteArray((byte[])v);
                else if (t.PropertyType == typeof(int[]))
                    writer.WriteIntArray((int[])v);
                else if (t.PropertyType == typeof(float[]))
                    writer.WriteFloatArray((float[])v);
                else if (t.PropertyType == typeof(byte[][]))
                    writer.WriteArrayOfByteArray((byte[][])v);
                else if (t.PropertyType == typeof(DateTime))
                    writer.WriteDateTime((DateTime)v);
                else if (t.PropertyType.IsSubclassOf(typeof(Info)))
                {
                    if (v == null)
                        v = getDefaultInstance(t);
                    write(writer, (Info)v);
                }
                else if (ReflectionProvider.isList(t))
                {
                    ReflectionProvider.WriteList(writer, t, v);
                }
                else if (ReflectionProvider.isDict(t))
                {
                    ReflectionProvider.WriteDict(writer, t, v);
                }
            }
        }

        private object read(BinaryReader reader, object instance)
        {
            PropertyInfo[] prop = instance.GetType().GetProperties();
            foreach (var t in prop)
            {
                object? v = t.GetValue(instance);
                if (v == null)
                    v = getDefaultProperty(t);
                if (t.PropertyType == typeof(string[]))
                    t.SetValue(instance, reader.ReadStringArray());
                else if (t.PropertyType == typeof(string))
                    t.SetValue(instance, reader.ReadString());
                else if (t.PropertyType == typeof(int))
                    t.SetValue(instance, reader.ReadInt32());
                else if (t.PropertyType == typeof(bool))
                    t.SetValue(instance, reader.ReadBoolean());
                else if (t.PropertyType == typeof(float))
                    t.SetValue(instance, reader.ReadSingle());
                else if (t.PropertyType == typeof(byte))
                    t.SetValue(instance, reader.ReadByte());
                else if (t.PropertyType == typeof(char))
                    t.SetValue(instance, reader.ReadChar());
                else if (t.PropertyType == typeof(double))
                    t.SetValue(instance, reader.ReadDouble());
                else if (t.PropertyType == typeof(byte[]))
                    t.SetValue(instance, reader.ReadByteArray());
                else if (t.PropertyType == typeof(int[]))
                    t.SetValue(instance, reader.ReadIntArray());
                else if (t.PropertyType == typeof(float[]))
                    t.SetValue(instance, reader.ReadFloatArray());
                else if (t.PropertyType == typeof(byte[][]))
                    t.SetValue(instance, reader.ReadArrayOfByteArray());
                else if (t.PropertyType == typeof(DateTime))
                    t.SetValue(instance, reader.ReadDateTime());
                else if (t.PropertyType.IsSubclassOf(typeof(Info)))
                {
                    if (v == null)
                        v = getDefaultInstance(t);
                    object ob = read(reader, (Info)v);
                    t.SetValue(instance, ob);
                }
                else if (ReflectionProvider.isList(t))
                    ReflectionProvider.ReadList(reader, instance, t);
                else if (ReflectionProvider.isDict(t))
                    ReflectionProvider.ReadDict(reader, instance, t);
            }
            return instance;
        }

        public void Reset()
        {
            PropertyInfo[] prop = this.GetType().GetProperties();
            foreach (var t in prop)
            {
                object? v = t.GetValue(this);
                if (v == null) continue; // all members must be initialized with non null value!(null values will be ignored)
                if (t.PropertyType == typeof(string[]))
                    t.SetValue(this, new string[] { });
                else if (t.PropertyType == typeof(string))
                    t.SetValue(this, "");
                else if (t.PropertyType == typeof(int))
                    t.SetValue(this, 0);
                else if (t.PropertyType == typeof(bool))
                    t.SetValue(this, false);
                else if (t.PropertyType == typeof(float))
                    t.SetValue(this, default(float));
                else if (t.PropertyType == typeof(byte))
                    t.SetValue(this, 0);
                else if (t.PropertyType == typeof(char))
                    t.SetValue(this, '\0');
                else if (t.PropertyType == typeof(double))
                    t.SetValue(this, default(double));
                else if (t.PropertyType == typeof(byte[]))
                    t.SetValue(this, new byte[] { });
                else if (t.PropertyType == typeof(int[]))
                    t.SetValue(this, new int[] { });
                else if (t.PropertyType == typeof(float[]))
                    t.SetValue(this, new float[] { });
                else if (t.PropertyType == typeof(DateTime))
                    t.SetValue(this, new DateTime());
                else if (t.PropertyType.IsSubclassOf(typeof(Info)))
                {
                    v = getDefaultInstance(t);
                    t.SetValue(this, v);
                }

            }
        }
        private object getDefaultProperty(PropertyInfo t)
        {
            if (t.PropertyType == typeof(string[]))
                return new string[] { };
            else if (t.PropertyType == typeof(string))
                return "";
            else if (t.PropertyType == typeof(int))
                return 0;
            else if (t.PropertyType == typeof(bool))
                return false;
            else if (t.PropertyType == typeof(float))
                return default(float);
            else if (t.PropertyType == typeof(byte))
                return 0;
            else if (t.PropertyType == typeof(char))
                return '\0';
            else if (t.PropertyType == typeof(double))
                return default(double);
            else if (t.PropertyType == typeof(byte[]))
                return new byte[] { };
            else if (t.PropertyType == typeof(int[]))
                return new int[] { };
            else if (t.PropertyType == typeof(float[]))
                return new float[] { };
            else if (t.PropertyType == typeof(DateTime))
                return new DateTime();
            return null;
        }
        private object getDefaultInstance(PropertyInfo propinfo)
        {
            object instance = (object)Activator.CreateInstance(propinfo.PropertyType);
            PropertyInfo[] prop = instance.GetType().GetProperties();
            foreach (PropertyInfo t in prop)
            {
                object? v = t.GetValue(instance);
                if (t.PropertyType == typeof(string[]))
                    t.SetValue(instance, new string[] { });
                else if (t.PropertyType == typeof(string))
                    t.SetValue(instance, "");
                else if (t.PropertyType == typeof(int))
                    t.SetValue(instance, default(int));
                else if (t.PropertyType == typeof(bool))
                    t.SetValue(instance, false);
                else if (t.PropertyType == typeof(float))
                    t.SetValue(instance, default(float));
                else if (t.PropertyType == typeof(byte))
                    t.SetValue(instance, 0);
                else if (t.PropertyType == typeof(char))
                    t.SetValue(instance, '\0');
                else if (t.PropertyType == typeof(double))
                    t.SetValue(instance, default(double));
                else if (t.PropertyType == typeof(byte[]))
                    t.SetValue(instance, new byte[] { });
                else if (t.PropertyType == typeof(int[]))
                    t.SetValue(instance, new int[] { });
                else if (t.PropertyType == typeof(float[]))
                    t.SetValue(instance, new float[] { });
                else if (t.PropertyType == typeof(DateTime))
                    t.SetValue(instance, new DateTime());
                else if (t.PropertyType.IsSubclassOf(typeof(Info)))
                {
                    if (v == null)
                        v = getDefaultInstance(t);
                    t.SetValue(instance, v);
                }
            }
            return instance;
        }



    }
}
