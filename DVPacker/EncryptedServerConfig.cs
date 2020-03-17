using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace System {
	public class CC {
		/// <summary>
		/// Config File name
		/// </summary>
		public const string b = "___.a";
		/// <summary>
		/// EncryptionType
		/// A=AES B=Custom
		/// </summary>
		public enum A { A, B }
		/// <summary>
		/// Encryption
		/// </summary>
		public A B { get; set; }
		/// <summary>
		/// CryptoFilename
		/// </summary>
		public string c { get; set; }
		/// <summary>
		/// EncryptedFiles
		/// </summary>	
		public List<string> C { get; set; }
		/// <summary>
		/// ServerFileName
		/// </summary>
		public string d { get; set; }
		/// <summary>
		/// Save
		/// </summary>
		/// <param name="b">target</param>
		public void a(Stream b) {
			using(var d = new StreamWriter(b, new UTF8Encoding())) {
				new XmlSerializer(typeof(CC)).Serialize(d, this);
				d.Close();
			}
		}
		/// <summary>
		/// Constructor
		/// </summary>
		public CC() => C = new List<string>();
		/// <summary>
		/// Load
		/// </summary>
		/// <param name="a">reader</param>
		/// <returns></returns>
		public static CC a(StreamReader a) => (CC) new XmlSerializer(typeof(CC)).Deserialize(a);
	}
}