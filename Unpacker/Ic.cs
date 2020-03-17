namespace System.Security {
	public interface IA {
		/// <summary>
		/// Key mutation
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		[SecuritySafeCritical]
		SecureString I(SecureString a);
		/// <summary>
		/// Decrypt
		/// </summary>
		/// <param name="a"></param>
		/// <param name="A"></param>
		/// <returns></returns>
		[SecuritySafeCritical]
		byte[] I(byte[] a, SecureString A);
		/// <summary>
		/// Encrypt
		/// </summary>
		/// <param name="a"></param>
		/// <param name="A"></param>
		/// <returns></returns>
		[SecuritySafeCritical]
		byte[] II(byte[] a, SecureString A);
	}
}
