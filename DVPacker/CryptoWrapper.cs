using System.Text;

namespace System.Security {
	public sealed class CW<TCP> : IA where TCP : IA {

		readonly TCP CP;
		public CW(TCP p) {
			if(p.GetType().IsGenericType && p.GetType().GetGenericTypeDefinition() == typeof(CW<>)) 
				throw new ArgumentException(/*"A Crypto Wrapper must not contain a Crypto Wrapper!"*/);
			CP = p;
		}
		/// <summary>
		/// Decrpt
		/// </summary>
		/// <param name="data"></param>
		/// <param name="mutatedKey"></param>
		/// <returns></returns>
		public byte[] I(byte[] data, SecureString mutatedKey) =>
			CP.I(data, mutatedKey);

		public byte[] DecryptB64(string b64Data, SecureString mutatedKey) =>
			I(Convert.FromBase64String(b64Data), mutatedKey);
		public byte[] II(byte[] data, SecureString mutatedKey) =>
			CP.II(data, mutatedKey);
		/// <summary>
		/// Encrypt
		/// </summary>
		/// <param name="data"></param>
		/// <param name="mutatedKey"></param>
		/// <returns></returns>
		public byte[] II(string data, SecureString mutatedKey) =>
			II(Encoding.Unicode.GetBytes(data), mutatedKey);
		public string EncryptB64(byte[] data, SecureString mutatedKey) =>
			Convert.ToBase64String(II(data, mutatedKey));
		/// <summary>
		/// key Mutation
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public SecureString I(SecureString key) => CP.I(key);
		/// <summary>
		/// key mutation
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public SecureString I(string key) {
			var ss = new SecureString();
			foreach(var c in key) ss.AppendChar(c);
			return CP.I(ss);
		}
	}
}
