using System;
using System.IO;
using System.Reflection;

namespace InfoObject
{
    internal static class InfoProvider
    {
        public static void WriteInfoObject(BinaryWriter writer, Type type,  object infoObject)
        {
            if (infoObject == null)
            {
                string infoMarker = "<InfoType>" + null + "</InfoOType>";
                writer.Write(infoMarker);
                return;
            }
            else
            {
                string signature = IOProvider.GetSignature((Info)infoObject);
                string infoMarker = "<InfoType>" + signature + "</InfoOType>";
                writer.Write(infoMarker);
            }
            IOProvider.Write(writer, infoObject);
        }

        public static void ReadInfoObject( BinaryReader reader , MemberInfo member, object instance)
        {
            string infoMarker = reader.ReadString();
            string infoType = ReadInfoType(infoMarker);
            
            if (infoType == null)
                return;
            
            object value = ReflectionProvider.GetDefaultInstance(infoType);
            object ob = IOProvider.Read(reader, value);
            member.SetValue(instance, ob);
        }

        public static bool IsInfo(Type type)
        {
            if (type == typeof(Info) || type.IsSubclassOf(typeof(Info)))
                return true;
            return false;
        }

        // "<InfoType>infoTypename</InfoOType>"
        public static string ReadInfoType(string marker)
        {
            string s = marker.Substring(10, marker.Length - 22);
            if (s.Length == 0) s = null;
            return s;
        }





    }
}