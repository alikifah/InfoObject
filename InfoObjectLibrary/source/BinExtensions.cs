// ============================================================================
//    Author: Al-Khafaji, Ali Kifah
//    Date:   20.4.2022
//    Description: Extension Methods for BinaryReader and BinaryWriter
// ============================================================================

using System;
using System.IO;
using System.Collections.Generic;

namespace InfoObject
{
	internal static class BinExtensions
	{
		//################# string array
		public static void WriteStringArray(this BinaryWriter w, string[] arr)
		{
			w.Write(arr.Length); // save length
			foreach (string s in arr)
				w.Write(s);
		}
		public static string[] ReadStringArray(this BinaryReader r)
		{
			int n = r.ReadInt32(); // load length of the array
			string[] d = new string[n];
			for (int i = 0; i < n; i++)
				d[i] = r.ReadString();
			return d;
		}
		//################# int array	
		public static void WriteIntArray(this BinaryWriter w, int[] arr)
		{
			w.Write(arr.Length); // save length
			foreach (int s in arr)
				w.Write(s);
		}
		public static int[] ReadIntArray(this BinaryReader r)
		{
			int n = r.ReadInt32();// load length of the array
			int[] d = new int[n];
			for (int i = 0; i < n; i++)
				d[i] = r.ReadInt32();
			return d;
		}
		//################## byte array	
		public static void WriteByteArray(this BinaryWriter w, byte[] arr)
		{
			w.Write(arr.Length);
			foreach (byte s in arr)
				w.Write(s);
		}
		public static byte[] ReadByteArray(this BinaryReader r)
		{
			int n = r.ReadInt32();
			byte[] d = new byte[n];
			for (int i = 0; i < n; i++)
				d[i] = r.ReadByte();
			return d;
		}
		//#############################
		public static void WriteArrayOfByteArray(this BinaryWriter w, byte[][] arr)
		{
			w.Write(arr.Length);
			foreach (byte[] s in arr)
				w.WriteByteArray(s);
		}
		public static byte[][] ReadArrayOfByteArray(this BinaryReader r)
		{
			List<byte[]> k = new List<byte[]>();
			int n = r.ReadInt32();// read count of byte arrays
			for (int i = 0; i < n; i++)
				k.Add(r.ReadByteArray());
			byte[][] d = k.ToArray();
			return d;
		}
		//################## float array	
		public static void WriteFloatArray(this BinaryWriter w, float[] arr)
		{
			w.Write(arr.Length);
			foreach (float s in arr)
				w.Write(s);
		}
		public static float[] ReadFloatArray(this BinaryReader r)
		{
			int n = r.ReadInt32();
			float[] d = new float[n];
			for (int i = 0; i < n; i++)
				d[i] = r.ReadByte();
			return d;
		}

		//############################### Datetime
	
		
		public static void WriteDateTime(this BinaryWriter w, DateTime d)
		{
			w.Write(d.Year);
			w.Write(d.Month);
			w.Write(d.Day);
			w.Write(d.Hour);
			w.Write(d.Minute);
			w.Write(d.Second);
			w.Write(d.Millisecond);
		}
		public static DateTime ReadDateTime(this BinaryReader r)
		{
			DateTime d = new DateTime(r.ReadInt32(), r.ReadInt32(), r.ReadInt32(), r.ReadInt32(), r.ReadInt32(), r.ReadInt32(), r.ReadInt32());
			return d;
		}
		



		//############################### Dict
		// code for reading and writing  Dict< string , string >
		public static void WriteDict_str_str(this BinaryWriter w, Dictionary<string, string> dict)
		{
			w.Write(dict.Count);
			foreach (KeyValuePair<string, string> kvp in dict)
			{
				w.Write(kvp.Key);
				w.Write(kvp.Value);
			}
		}
		public static Dictionary<string, string> ReadDict_str_str(this BinaryReader r)
		{
			int n = r.ReadInt32();
			Dictionary<string, string> dict = new Dictionary<string, string>();
			for (int i = 0; i < n; i++)
			{
				dict.Add(r.ReadString(), r.ReadString());
			}
			return dict;
		}

		// code for reading and writing  Dict< int , string >
		public static void WriteDict_int_str(this BinaryWriter w, Dictionary<int, string> dict)
		{
			w.Write(dict.Count);
			foreach (KeyValuePair<int, string> kvp in dict)
			{
				w.Write(kvp.Key);
				w.Write(kvp.Value);
			}
		}
		public static Dictionary<int, string> ReadDict_int_str(this BinaryReader r)
		{
			int n = r.ReadInt32();
			Dictionary<int, string> dict = new Dictionary<int, string>();
			for (int i = 0; i < n; i++)
			{
				dict.Add(r.ReadInt32(), r.ReadString());
			}
			return dict;
		}

		// code for reading and writing  Dict< in , int >
		public static void WriteDict_int_int(this BinaryWriter w, Dictionary<int, int> dict)
		{
			w.Write(dict.Count);
			foreach (KeyValuePair<int, int> kvp in dict)
			{
				w.Write(kvp.Key);
				w.Write(kvp.Value);
			}
		}
		public static Dictionary<int, int> ReadDict_int_int(this BinaryReader r)
		{
			int n = r.ReadInt32();
			Dictionary<int, int> dict = new Dictionary<int, int>();
			for (int i = 0; i < n; i++)
			{
				dict.Add(r.ReadInt32(), r.ReadInt32());
			}
			return dict;
		}

		// code for reading and writing  Dict< string , int >
		public static void WriteDict_str_int(this BinaryWriter w, Dictionary<string, int> dict)
		{
			w.Write(dict.Count);
			foreach (KeyValuePair<string, int> kvp in dict)
			{
				w.Write(kvp.Key);
				w.Write(kvp.Value);
			}
		}
		public static Dictionary<string, int> ReadDict_str_int(this BinaryReader r)
		{
			int n = r.ReadInt32();
			Dictionary<string, int> dict = new Dictionary<string, int>();
			for (int i = 0; i < n; i++)
			{
				dict.Add(r.ReadString(), r.ReadInt32());
			}
			return dict;
		}








	}
}