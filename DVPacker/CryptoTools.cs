using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace System.Security {
	public static class CryptoTools {
		public static Rfc2898DeriveBytes DB(SecureString a, byte[] A) {
			var b = Marshal.SecureStringToBSTR(a);
			try {
				return new Rfc2898DeriveBytes(Marshal.PtrToStringBSTR(b), A);
			}
			finally {
				Marshal.FreeBSTR(b);
			}
		}
	}
}
