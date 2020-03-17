using System.IO;
using System.Security.Cryptography;

namespace System.Security {
	/// <summary>
	/// EncryptorAES
	/// </summary>
	public class CA : IA {
		public static byte[] IA = new byte[] { 49, 103, 216, 55, 237, 139, 38, 192, 142, 81, 178, 208, 84 };
		public byte[] I(byte[] I, SecureString II) {
			using(var l = Aes.Create()) {
				using(var ll = CB.A(II, IA)) {
					l.Key = ll.GetBytes(32);
					l.IV = ll.GetBytes(16);
				}
				using(var ll = new MemoryStream()) {
					using(var i = new CryptoStream(ll, l.CreateDecryptor(), CryptoStreamMode.Write))
						i.Write(I, 0, I.Length);
					return ll.ToArray();
				}
			}
		}

		public byte[] II(byte[] II, SecureString I) {
			using(var ll = Aes.Create()) {
				using(var l = CB.A(I, IA)) {
					ll.Key = l.GetBytes(32);
					ll.IV = l.GetBytes(16);
				}
				using(var l = new MemoryStream()) {
					using(var g = new CryptoStream(l, ll.CreateEncryptor(), CryptoStreamMode.Write))
						g.Write(II, 0, II.Length);
					return l.ToArray();
				}
			}
		}

		public SecureString I(SecureString I) => I ?? throw new ArgumentNullException(nameof(I));
	}
}
