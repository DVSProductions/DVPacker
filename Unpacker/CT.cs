using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace System.Security {
	/// <summary>
	/// CryptoTools
	/// </summary>
	public static class CB {
		public static Rfc2898DeriveBytes A(SecureString a, byte[] A) {
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
