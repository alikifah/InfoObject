// ============================================================================
//    Author: Al-Khafaji, Ali Kifah
//    Date:   20.4.2022
//    Description: fully serializable abstract C# class that implements Reflection to convert field members to and from binary data.
//    This class is meant to be used as a containing medium to hold and transfer Information on disk or over network.
//    The classes that inherit from this class can be converted to byte array and transferred over network or saved to disk as a binary file.
//    Loading of the original data from file or from byte array is also possible.
// ============================================================================
using System;
using System.IO;
using System.Reflection;
using ExtensionMethods;
using System.Collections.Generic;

public abstract class Info
{
    private FieldInfo[] myFieldInfo;
    private int size = 0;
    public Info()
    {
        initialize(); // save all field members in the Array
    }
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
    private void Load(byte[] data)
    {
        size = data.Length;
        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                read(reader);
            }
        }
    }
// load from file
    private void Load(string filePath)
    {
        Load(FileToBinary(filePath));
    }

    // Deserialize from byte array
    // PrizeInfo k =  Info.Deserialize<PrizeInfo>(b);
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

    public void Save(string filePath)
    {
        BinaryToFile(Serialize(), filePath);
    }

    public byte[] Serialize()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                write(writer);
            }
            return m.ToArray();
        }
    }

    private void write(BinaryWriter writer)
    {
        // write class name for sake of compatibility check
        writer.Write(this.GetType().ToString());
        // write content
        foreach (var t in myFieldInfo)
        {
            var v = t.GetValue(this);
            if (v == null) continue;
            if (v.GetType() == typeof(string))
                writer.Write((string)v);
            else if (v.GetType() == typeof(string[]))
                writer.WriteStringArray((string[])v);
            else if (v.GetType() == typeof(int))
                writer.Write((int)v);
            else if (v.GetType() == typeof(float))
                writer.Write((float)v);
            else if (v.GetType() == typeof(bool))
                writer.Write((bool)v);
            else if (v.GetType() == typeof(byte))
                writer.Write((byte)v);
            else if (v.GetType() == typeof(char))
                writer.Write((char)v);
            else if (v.GetType() == typeof(double))
                writer.Write((double)v);
            else if (v.GetType() == typeof(byte[]))
                writer.WriteByteArray((byte[])v);
            else if (v.GetType() == typeof(int[]))
                writer.WriteIntArray((int[])v);
            else if (v.GetType() == typeof(float[]))
                writer.WriteFloatArray((float[])v);
            else if (v.GetType() == typeof(byte[][]))
                writer.WriteArrayOfByteArray((byte[][])v);
        }
    }
    private void read(BinaryReader reader)
    {
        // read class name and compare it with the class of the byte array
        if (reader.ReadString() != this.GetType().ToString())
        {
            //throw new ArgumentException("Type compatibility error!"  );
            return;
        }
        // read content
        foreach (var t in myFieldInfo)
        {
            var v = t.GetValue(this);
            if (v == null) continue;// all members must be initialized with non null value!
            if (v.GetType() == typeof(string[]))
                t.SetValue(this, reader.ReadStringArray());
            else if (v.GetType() == typeof(string))
                t.SetValue(this, reader.ReadString());
            else if (v.GetType() == typeof(int))
                t.SetValue(this, reader.ReadInt32());
            else if (v.GetType() == typeof(bool))
                t.SetValue(this, reader.ReadBoolean());
            else if (v.GetType() == typeof(float))
                t.SetValue(this, reader.ReadSingle());
            else if (v.GetType() == typeof(byte))
                t.SetValue(this, reader.ReadByte());
            else if (v.GetType() == typeof(char))
                t.SetValue(this, reader.ReadChar());
            else if (v.GetType() == typeof(double))
                t.SetValue(this, reader.ReadDouble());
            else if (v.GetType() == typeof(byte[]))
                t.SetValue(this, reader.ReadByteArray());
            else if (v.GetType() == typeof(int[]))
                t.SetValue(this, reader.ReadIntArray());
            else if (v.GetType() == typeof(float[]))
                t.SetValue(this, reader.ReadFloatArray());
            else if (v.GetType() == typeof(byte[][]))
                t.SetValue(this, reader.ReadArrayOfByteArray());
        }
    }
    public void Reset()
    {
        foreach (var t in myFieldInfo)
        {
            var v = t.GetValue(this);
            if (v == null) continue; // all members must be initialized with non null value!
            if (v.GetType() == typeof(string[]))
                t.SetValue(this, new string[] { });
            else if (v.GetType() == typeof(string))
                t.SetValue(this, "");
            else if (v.GetType() == typeof(int))
                t.SetValue(this, 0);
            else if (v.GetType() == typeof(bool))
                t.SetValue(this, false);
            else if (v.GetType() == typeof(float))
                t.SetValue(this, default(float));
            else if (v.GetType() == typeof(byte))
                t.SetValue(this, 0);
            else if (v.GetType() == typeof(char))
                t.SetValue(this, '\0');
            else if (v.GetType() == typeof(double))
                t.SetValue(this, default(double));
            else if (v.GetType() == typeof(byte[]))
                t.SetValue(this, new byte[] { });
            else if (v.GetType() == typeof(int[]))
                t.SetValue(this, new int[] { });
            else if (v.GetType() == typeof(float[]))
                t.SetValue(this, new float[] { });
        }
    }
   

    private void initialize()
    {
        Type myType = this.GetType();
        myFieldInfo = myType.GetFields(BindingFlags.Instance | BindingFlags.Public);
    }


}

