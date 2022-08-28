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
using System.Linq;
namespace InfoObject
{
    public abstract class Info
    {
        private int size = 0; // data size that this class holds(in Byte)
        internal string signature { get; set; }
        public Info()
        {
            signature = IOProvider.GetSignature(this);
        }
        public int GetSize()
        {
            return size;
        }
        // load from byte array
        private bool load(byte[] data, out Exception exception, string password = "")
        {
            size = data.Length;
            using (MemoryStream m = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(m))
                {
                    try
                    {
                        IOProvider.Read(reader, this);
                    }
                    catch (Exception ex2) // on non specific error
                    {
                        exception = ex2;
                        return false;
                    }
                }
            }
            exception = null;
            return true;
        }

        private static byte[] loadFromFile(string filePath, out Exception exception, out int fileSize, string password = "")
        {
            if (!File.Exists(filePath))
            {
                exception = new IOException("Invalid input: File doesn't exist!");
                fileSize = 0;
                return null;
            }
            exception = null;
            return IOProvider.FileToBinary(filePath, out fileSize);
        }

        /// <summary>
        /// Deserialize from byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] data, out Exception exception, string password = "") where T : Info, new()
        {
            if (data == null)
            {
                exception = new IOException("Invalid input: null data!");
                return null;
            }
            var instance = new T();
            // Check compatibility and password
            data = IOProvider.GetDycreptedData(data, instance.signature, out exception, password);
            if (exception != null)
                return null;

            bool isLoadSuccessful = instance.load(data, out exception);
            if (!isLoadSuccessful) return null;
            return instance;
        }

        public static T Deserialize<T>(byte[] data, string password = "") where T : Info, new()
        {
            if (data == null)
                return null;
            var instance = new T();
            // Check compatibility and password
            data = IOProvider.GetDycreptedData(data, instance.signature, out Exception exception, password);
            if (data == null) 
                return null;

            bool isLoadSuccessful = instance.load(data, out exception);
            if (!isLoadSuccessful) return null;
            return instance;
        }
        public static T Deserialize<T>(byte[] data, out Exception exception) where T : Info, new()
        {
            if (data == null)
            {
                exception = new IOException("Invalid input: null data!");
                return null;
            }
            var instance = new T();
            // Check compatibility and password
            data = IOProvider.GetDycreptedData(data, instance.signature, out exception, "");
            if (exception != null)
                return null;

            bool isLoadSuccessful = instance.load(data, out exception);
            if (!isLoadSuccessful) return null;
            return instance;
        }
        public static T Deserialize<T>(byte[] data) where T : Info, new()
        {
            if (data == null)
                return null;
            var instance = new T();
            // Check compatibility and password
            data = IOProvider.GetDycreptedData(data, instance.signature, out Exception exception, "");
            if (data == null)
                return null;

            bool isLoadSuccessful = instance.load(data, out exception);
            if (!isLoadSuccessful) return null;
            return instance;
        }

        /// <summary>
        /// Deserialize from file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string filePath, out Exception exception, string password = "") where T : Info, new()
        {
            var dataFromFile = loadFromFile(filePath, out exception, out int fileSite, password);
            var instance = Deserialize<T>(dataFromFile, out exception, password);
            if (instance == null) return null;
            instance.size = fileSite;
            return instance;
        }
        public static T Deserialize<T>(string filePath, string password = "") where T : Info, new()
        {
            var dataFromFile = loadFromFile(filePath, out Exception exception1, out int fileSite, password);
            var instance = Deserialize<T>(dataFromFile, out Exception exception2, password);
            if (instance == null) return null;
            instance.size = fileSite;
            return instance;
        }
        public static T Deserialize<T>(string filePath, out Exception exception) where T : Info, new()
        {
            var dataFromFile = loadFromFile(filePath, out exception, out int fileSite, "");
            var instance = Deserialize<T>(dataFromFile, out exception, "");
            if (instance == null) return null;
            instance.size = fileSite;
            return instance;
        }
        public static T Deserialize<T>(string filePath) where T : Info, new()
        {
            var dataFromFile = loadFromFile(filePath, out Exception exception1, out int fileSite, "");
            var instance = Deserialize<T>(dataFromFile, out Exception exception2, "");
            if (instance == null) return null;
            instance.size = fileSite;
            return instance;
        }

        /// <summary>
        /// Save the Info object to file
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath, out Exception exception, string password = "")
        {
            var data = Serialize(out exception, password);
            if (data != null)
                IOProvider.BinaryToFile(data, filePath);
        }

        /// <summary>
        /// convert the Info object to byte array
        /// </summary>
        /// <returns></returns>
        public byte[] Serialize(out Exception exception, string password = "")
        {
            var signatureData = IOProvider.GetSignatureData(signature);
            var nonEncryptedData = IOProvider.GetPasswordData(password);
            var encryptedData = IOProvider.GetEncryptedData(this, out exception, password);
            return signatureData.Concat(nonEncryptedData).Concat(encryptedData).ToArray();
        }
        public byte[] Serialize(string password = "")
        {
            var signatureData = IOProvider.GetSignatureData(signature);
            var nonEncryptedData = IOProvider.GetPasswordData(password);
            var encryptedData = IOProvider.GetEncryptedData(this, out Exception exception, password);
            return signatureData.Concat(nonEncryptedData).Concat(encryptedData).ToArray();
        }
        public byte[] Serialize(out Exception exception)
        {
            var signatureData = IOProvider.GetSignatureData(signature);
            var nonEncryptedData = IOProvider.GetPasswordData("");
            var encryptedData = IOProvider.GetEncryptedData(this, out exception, "");
            return signatureData.Concat(nonEncryptedData).Concat(encryptedData).ToArray();
        }
        public byte[] Serialize()
        {
            var signatureData = IOProvider.GetSignatureData(signature);
            var nonEncryptedData = IOProvider.GetPasswordData("");
            var encryptedData = IOProvider.GetEncryptedData(this, out Exception exception, "");
            return signatureData.Concat(nonEncryptedData).Concat(encryptedData).ToArray();
        }

        public void Reset()
        {
            IOProvider.ResetInstance(this, signature);
        }


    }
}
