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

namespace InfoObject
{
    public abstract class Info
    {
        private int size = 0; // data size that this class holds(in Byte)

        public int GetSize()
        {
            return size;
        }
        // load from byte array
        private bool Load(byte[] data)
        {
            size = data.Length;
            using (MemoryStream m = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(m))
                {
                    try
                    {
                        if (!IOProvider.CheckTypeCompatibility(reader, this))
                        {
                            throw new IOException("The input Binary data are not compatible with this Info object!");
                        }
                        IOProvider.Read(reader, this);
                    }
                    catch(EndOfStreamException ex) // on wrong Password
                    {
                        return false;
                    }
                    catch(IOException ex1) // on Compatibility error
                    {
                        return false;
                    }
                    catch (Exception ex2) // on non specific error
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool loadFromFile(string filePath, string password ="")
        {
            var data = IOProvider.FileToBinary(filePath, out size);
            if (password != null && password.Length > 0)
                data = SecurityProvider.Decrypt(data, password);
            return Load(data);
        }

        /// <summary>
        /// Deserialize from byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] data, string password ="") where T : Info, new()
        {
            if (password != null && password.Length > 0 )
                data = SecurityProvider.Decrypt(data, password);

            var o = new T();
           bool isLoadSuccessful = o.Load(data);
            if (!isLoadSuccessful) return null;
            return o;
        }

        /// <summary>
        /// Deserialize from file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string filePath, string password ="") where T : Info, new()
        {
            var o = new T();
            bool isLoadSuccessful = o.loadFromFile(filePath, password);
            if (!isLoadSuccessful) return null;
            return o;
        }
        /// <summary>
        /// Save the Info object to file
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath, string password="")
        {
            var data = Serialize(password);
            if (data!= null)
                IOProvider.BinaryToFile(data , filePath);
        }
        /// <summary>
        /// convert the Info object to byte array
        /// </summary>
        /// <returns></returns>
        public virtual byte[] Serialize(string password ="")
        {
            byte[] output;
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                    IOProvider.WriteTypeSignature(writer, this);
                    try
                    {
                        IOProvider.Write(writer, this);
                    }
                    catch
                    {
                        return null;
                    }
                }

                if (password != null && password.Length > 0)
                    output = SecurityProvider.Encrypt(m.ToArray(), password);
                else
                    output = m.ToArray();

                return output;
            }
        }

        public void Reset()
        {
            IOProvider.ResetInstance(this);
        }

    }
}
