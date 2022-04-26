// ============================================================================
//    Author: Al-Khafaji, Ali Kifah
//    Date:   20.4.2022
//    Description: Extension Methods for BinaryReader and BinaryWriter
// ============================================================================

using system;
using system.IO;
namespace ExtensionMethods
{
   public static class BinExtensions
   {
//################# string array
   public static void WriteStringArray( this BinaryWriter w, string[] arr ){
	   w.Write(arr.Length); // save length
	   foreach(string s in arr)
				w.Write(s);
	}
   public static string[] ReadStringArray( this BinaryReader r){
	   int n = r.ReadInt32(); // load length of the array
	   string[] d = new string[n];
	   for(int i = 0; i<n;i++ )
			d[i] = r.ReadString();
		return d;
	}
//################# int array	
   public static void WriteIntArray( this BinaryWriter w, int[] arr ){
	   w.Write(arr.Length); // save length
	   foreach(int s in arr)
				w.Write(s);
	}
   public static int[] ReadIntArray( this BinaryReader r){
	   int n = r.ReadInt32();// load length of the array
	   int[] d = new int[n];
	   for(int i = 0; i<n;i++ )
			d[i] = r.ReadInt32();
		return d;
	}
//################## byte array	
   public static void WriteByteArray( this BinaryWriter w, byte[] arr ){
	   w.Write(arr.Length);
	   foreach(byte s in arr)
				w.Write(s);
	}
   public static byte[] ReadByteArray( this BinaryReader r){
	   int n = r.ReadInt32();
	   byte[] d = new byte[n];
	   for(int i = 0; i<n;i++ )
			d[i] = r.ReadByte();
		return d;
	}
//################## float array	
   public static void WriteFloatArray( this BinaryWriter w, float[] arr ){
	   w.Write(arr.Length);
	   foreach(float s in arr)
				w.Write(s);
	}
   public static float[] ReadFloatArray( this BinaryReader r){
	   int n = r.ReadInt32();
	   float[] d = new float[n];
	   for(int i = 0; i<n;i++ )
			d[i] = r.ReadByte();
		return d;
	}
   }
}
