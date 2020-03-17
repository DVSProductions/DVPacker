using System.IO;
using System.Security.Cryptography;

namespace System.Security {
	public class CES : IA {
		public static byte[] salt = new byte[] { 49, 103, 216, 55, 237, 139, 38, 192, 142, 81, 178, 208, 84 };
		/// <summary>
		/// Decrpt
		/// </summary>
		/// <param name="data"></param>
		/// <param name="mutatedKey"></param>
		/// <returns></returns>
		public byte[] I(byte[] data, SecureString mutatedKey) {
			using(var encryptor = Aes.Create()) {
				using(var pdb = CryptoTools.DB(mutatedKey, salt)) {
					encryptor.Key = pdb.GetBytes(32);
					encryptor.IV = pdb.GetBytes(16);
				}
				using(var ms = new MemoryStream()) {
					using(var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
						cs.Write(data, 0, data.Length);
					return ms.ToArray();
				}
			}
		}
		/// <summary>
		/// Encrypt
		/// </summary>
		/// <param name="data"></param>
		/// <param name="mutatedKey"></param>
		/// <returns></returns>
		public byte[] II(byte[] data, SecureString mutatedKey) {
			using(var encryptor = Aes.Create()) {
				using(var pdb = CryptoTools.DB(mutatedKey, salt)) {
					encryptor.Key = pdb.GetBytes(32);
					encryptor.IV = pdb.GetBytes(16);
				}
				using(var ms = new MemoryStream()) {
					using(var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
						cs.Write(data, 0, data.Length);
					return ms.ToArray();
				}
			}
		}
		public SecureString I(SecureString key) {
			if(key == null)
				throw new ArgumentNullException(nameof(key));
			return key;
		}
	}
}
