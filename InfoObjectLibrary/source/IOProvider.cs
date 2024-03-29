﻿// ============================================================================
//    Author: Al-Khafaji, Ali Kifah
//    Date:   20.4.2022
//    Description: Fully serializable abstract class that implements Reflection to convert properties to and from binary data.
//    This class is meant to be used as a containing medium to hold and transfer Information on disk or over network.
//    The classes that inherit from this class can be converted to byte array and transferred over network or saved to disk as a binary file.
//
// ============================================================================

using System;
using System.Collections;
using System.IO;
using System.Reflection;

using System.Collections.Generic;

namespace InfoObject
{
    internal static class IOProvider
    {

        public static bool CheckTypeCompatibility(BinaryReader reader, string signature, out Exception exception)
        {
            string signatureFromObject = signature;// instance.GetType().FullName;
            string signatureFromData = "";
            try
            {
                signatureFromData = reader.ReadString();
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
            exception = null;
            if (!ReflectionProvider.IsTypeValid(signatureFromData)) // check if the data type is of known type
                return false;
            if (signatureFromObject != signatureFromData) // check if the types are the same
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

        public static void ResetInstance(object instance, string infoType = "")
        {
            PropertyInfo[] props = ReflectionProvider.GetProperties(instance);
            foreach (var property in props)
            {
                Type type = property.PropertyType;
                object value = property.GetValue(instance);

                if (InfoProvider.IsInfo(type))
                {
                    value = ReflectionProvider.GetDefaultInstance(infoType);
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
             MemberInfo[] members = ReflectionProvider.getMembers(instance, out bool isStruct);
        

            foreach (var member in members)
            {
                ReflectionProvider.getMemeberTypeValue(instance, member, isStruct, out object value, out Type type);


                if (!type.IsValueType && value == null) value = ReflectionProvider.GetDefaultValue(type);

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
                else if (type.IsEnum)
                    writer.Write((int)value);
                else if (type == typeof(DateTime))
                    writer.WriteDateTime((DateTime)value);
                else if (InfoProvider.IsInfo(type))
                    InfoProvider.WriteInfoObject(writer, type, value);
                else if (ReflectionProvider.isStruct(type))
                    Write(writer, value);
                else if (ListProvider.isList(type))
                    ListProvider.WriteList(writer, ListProvider.getListType((PropertyInfo)member), value);
                else if (DictProvider.isDict(type))
                {
                    Type keyType = DictProvider.getDictKeyType((PropertyInfo)member);
                    Type valueType = DictProvider.getDictValueType((PropertyInfo)member);
                    DictProvider.WriteDict(writer, value, keyType, valueType);
                }
                else if (type == typeof(object))
                    ObjectProvider.WriteObject(writer, value);
                else if (type == typeof(object[]))
                    writer.WriteObjectArray((object[])value);
            }
        }


        public static object Read(BinaryReader reader, object instance)
        {
            if (instance == null) return null;
            MemberInfo[] members = ReflectionProvider.getMembers(instance, out bool isStruct);

            foreach (var member in members)
            {

                 ReflectionProvider.getMemeberTypeValue(instance, member, isStruct, out object value, out Type type);

                if (!type.IsValueType && value == null)
                    value = ReflectionProvider.GetDefaultValue(type);

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
                else if (type.IsEnum)
                    member.SetValue(instance, reader.ReadInt32());
                else if (type == typeof(DateTime))
                    member.SetValue(instance,  reader.ReadDateTime());
                else if (InfoProvider.IsInfo(type))
                    InfoProvider.ReadInfoObject(reader, member, instance);
                else if (ReflectionProvider.isStruct(type))
                  member.SetValue(instance, Read(reader, value));
                else if (ListProvider.isList(type))
                    member.SetValue(instance, ListProvider.ReadList(reader, (PropertyInfo)member));
                else if (DictProvider.isDict(type))
                    member.SetValue(instance, DictProvider.ReadDict(reader, (PropertyInfo)member));
                else if (type == typeof(object))
                    member.SetValue(instance, ObjectProvider.ReadObject(reader));
                else if (type == typeof(object[]))
                    member.SetValue(instance, reader.ReadObjectArray());
            }
            return instance;
        }

        //###################################################

        public static byte[] GetSignatureData(string signature)
        {
            using (MemoryStream signatureStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(signatureStream))
                {
                    writer.Write(signature);
                }
                return signatureStream.ToArray();
            }
        }

        public static byte[] GetEncryptedData(object instance, out Exception exception, string password = "")
        {
            byte[] output;
            using (MemoryStream rawdata = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(rawdata))
                {
                    try
                    {
                        Write(writer, instance);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        return null;
                    }
                }
                if (password != null && password.Length > 0)
                    output = SecurityProvider.Encrypt(rawdata.ToArray(), password);
                else
                    output = rawdata.ToArray();
                exception = null;
                return output;
            }
        }

        public static byte[] GetDycreptedData(byte[] data, string signature, out Exception exception, string password = "")
        {
            bool isSavedWithPassword = false;
            string hashedPassword = "";
            string salt = "";
            using (MemoryStream dataStream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(dataStream))
                {
                    // check cmpatibility
                    if (!IOProvider.CheckTypeCompatibility(reader, signature, out Exception ex))
                    {
                        exception = new IOException("Invalid input: input data not compatible!"); ;
                        return null;
                    }
                    // check password
                    isSavedWithPassword = reader.ReadBoolean();

                    if (isSavedWithPassword)
                    {
                        if (password != null && password.Length > 0)
                        {
                            hashedPassword = reader.ReadString();
                            salt = reader.ReadString();
                            bool isPasswordCorrect = SecurityProvider.IsPasswordCorrect(password, hashedPassword, salt);
                            if (!isPasswordCorrect)
                            {
                                exception = new IOException("Wrong password!");
                                return null;
                            }
                        }
                        else
                        {
                            exception = new IOException("Wrong password!");
                            return null;
                        }
                    }
                    else
                    {
                        if (password != null && password.Length > 0)
                        {
                            exception = new IOException("Wrong password!");
                            return null;
                        }
                    }
                    exception = null;
                    byte[] encryptedData = reader.ReadBytes((int)dataStream.Length);
                    if (isSavedWithPassword)
                        return SecurityProvider.Decrypt(encryptedData, password);
                    else
                        return encryptedData;
                }
            }
        }

        public static byte[] GetPasswordData(string password)
        {
            bool isSavedWithPassword = false;
            using (MemoryStream nonEncryptedStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(nonEncryptedStream))
                {
                    if (password != null && password.Length > 0)
                    {
                        isSavedWithPassword = true;
                        string salt = SecurityProvider.GetSalt();
                        string hashedPassword = SecurityProvider.GetHashedPawssword(password, salt);
                        writer.Write(isSavedWithPassword);
                        writer.Write(hashedPassword);
                        writer.Write(salt);
                    }
                    else
                    {
                        isSavedWithPassword = false;
                        writer.Write(isSavedWithPassword);
                    }
                }
                return nonEncryptedStream.ToArray();
            }
        }

        public static ArrayList GetContent(Info info)
        {
            ArrayList d = new ArrayList();
            PropertyInfo[] props = ReflectionProvider.GetProperties(info);
            foreach (var p in props)
            {
                d.Add(p.GetValue(info));
            }
            return d;
        }

        public static int GetContentCount(Info info)
        {
            ArrayList d = new ArrayList();
            PropertyInfo[] props = ReflectionProvider.GetProperties(info);
            return props.Length;
        }

    }
}
