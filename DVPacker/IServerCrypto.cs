namespace System.Security {
	public interface IA {
		/// <summary>
		/// Mutate key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[SecuritySafeCritical]
		SecureString I(SecureString key);
		/// <summary>
		/// Encrypt
		/// </summary>
		/// <param name="data"></param>
		/// <param name="mutatedKey"></param>
		/// <returns></returns>
		[SecuritySafeCritical]
		byte[] I(byte[] data, SecureString mutatedKey);
		/// <summary>
		/// Decrypt
		/// </summary>
		/// <param name="data"></param>
		/// <param name="mutatedKey"></param>
		/// <returns></returns>
		[SecuritySafeCritical]
		byte[] II(byte[] data, SecureString mutatedKey);
	}
}
