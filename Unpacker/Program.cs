﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text.RegularExpressions;
#region NS
namespace Unpacker {
	#endregion
	#region classname
	class Program {
		#endregion
		#region Inner Password
		public static SecureString b { get; }
		#endregion
		#region Outer Password
		public static string B = "abc";
		#endregion
		#region salt
		public static byte[] C = new byte[] { 1, 2, 3, 5, 6, 7, 8, 9 };
		#endregion
		#region FileName
		public static string c = "exported.zip.enc";
		#endregion
		/// <summary>
		/// ZipDecryptor
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		private static byte[] A(byte[] a) {
			var A = new CB<CA>(new CA());
			var b = CA.IA;
			CA.IA = C;
			var c = A.I(a, A.II(B));
			CA.IA = b;
			return c;
		}
		/// <summary>
		/// ZipUnpacker
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		private static Tuple<Dictionary<String, byte[]>, CC> a(byte[] a) {
			//Debug.WriteLine("Loading Encrypted Zip...");
			try {
				using(var A = new MemoryStream(a)) {
					using(var b = new System.IO.Compression.ZipArchive(A, System.IO.Compression.ZipArchiveMode.Read)) {
						CC B = null;
						foreach(var c in b.Entries) {
							if(c.Name == CC.b)
								using(var r = new StreamReader(c.Open()))
									B = CC.a(r);
						}
						var decr = new CB(b, B?? throw new FileNotFoundException(/*"Config file Missing in package"*/));
						decr.C(Program.b);
						return new Tuple<Dictionary<String, byte[]>, CC>(decr.c, B);
					}
				}
			}
			catch(Exception ex) {
				Console.WriteLine($"Error loading encrypted module : { ex.ToString()}");
				return null;
			}
		}
		/// <summary>
		/// asm
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		private static Assembly A(Tuple<Dictionary<String, byte[]>, CC> a) => Assembly.Load(a.Item1[a.Item2.d]);
		/// <summary>
		/// Run
		/// </summary>
		/// <param name="a"></param>
		/// <param name="A"></param>
		private static void A(Assembly a, string[] A) {
			MethodInfo b = null, B = null;
			foreach(var c in a.GetTypes()) {
				//Debug.WriteLine(c.Name);
				foreach(var C in c.GetMethods()) {
					//Debug.WriteLine(m.Name);
					if(C.IsStatic) {
						if(C.Name == "Main")
							B = C;
						else if(C.Name == "Run")
							b = C;
					}
				}
			}
			if(B != null)
				B.Invoke(null, new object[] { A });
			else if(b != null)
				b.Invoke(null, new object[] { A });
		}
		/// <summary>
		/// Get namespace name
		/// </summary>
		/// <param name="A"></param>
		/// <returns></returns>
		static string A(string A) => A.Substring(0, new Regex(@"^.+?\.").Match(A).Length);
		static void Main(string[] B) {
			byte[] b;
			using(var a = new MemoryStream()) {
				var C = Assembly.GetExecutingAssembly();
				C.GetManifestResourceStream(A(C.GetManifestResourceNames()[0]) + c).CopyTo(a);
				b = a.ToArray();
			}
			A(A(a(A(b))), B);
		}
	}
}