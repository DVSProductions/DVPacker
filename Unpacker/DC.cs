using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security;
/// <summary>
/// Decryptor
/// </summary>
sealed class CB {
	[Import]
	IA A;//crypto
	readonly Dictionary<string, byte[]> a;//files
	bool B = false;//succesfullyDecrypted
	readonly Dictionary<string, byte[]> b;//decryptedFiles
	public CB(ZipArchive B, CC A) {
		a = new Dictionary<string, byte[]>();
		b = new Dictionary<string, byte[]>();
		C(B, A);
		foreach(var b in A.C)
			a.Add(b, C(B.GetEntry(b)));
		a.Add(A.d, C(B.GetEntry(A.d)));
	}
	/// <summary>
	/// ReadZipEntry
	/// </summary>
	/// <param name="A"></param>
	/// <returns></returns>
	private byte[] C(ZipArchiveEntry A) {
		using(var b = new MemoryStream()) {
			A.Open().CopyTo(b);
			return b.ToArray();
		}
	}
	/// <summary>
	/// CryptoLoader
	/// </summary>
	/// <param name="a"></param>
	/// <param name="A"></param>
	private void C(ZipArchive a, CC A) {
		switch(A.B) {
			case CC.A.A:
				this.A = new CA();
				break;
			case CC.A.B:
				using(var b = new AggregateCatalog()) {
					b.Catalogs.Add(new AssemblyCatalog(Assembly.Load(C(a.GetEntry(A.c)))));
					new CompositionContainer(b).ComposeParts(this);
				}
				break;
			default:
				break;
		}
	}
	/// <summary>
	/// Decrypt all
	/// </summary>
	/// <param name="c"></param>
	/// <returns></returns>
	public bool C(SecureString c) {
		try {
			B = false;
			b.Clear();
			using(var B = A.I(c))
				foreach(var C in a)
					b.Add(C.Key, A.I(C.Value, B));
			B = true;
			return true;
		}
		catch(Exception/* ex*/) {
			//Debug.WriteLine($"Server Decryption error: {ex.ToString()}");
			return false;
		}
	}
	/// <summary>
	/// DecryptedFiles
	/// </summary>
	public Dictionary<string, byte[]> c => B ? b : null;
}