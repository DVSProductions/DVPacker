using System.Text;

namespace System.Security {
	/// <summary>
	/// CryptoWrapper
	/// </summary>
	/// <typeparam name="T">TCrypto</typeparam>
	public sealed class CB<T> : IA where T : IA {
		readonly T A;
		public CB(T a) {
			if(a.GetType().IsGenericType && a.GetType().GetGenericTypeDefinition() == typeof(CB<>))
				throw new ArgumentException(/*"A Crypto Wrapper must not contain a Crypto Wrapper!"*/);
			A = a;
		}
		public byte[] I(byte[] I, SecureString l) =>
			A.I(I, l);
		public byte[] II(byte[] ll, SecureString II) =>
			A.II(ll, II);
		public SecureString I(SecureString I) => A.I(I);
		public SecureString II(string P) {
			var p = new SecureString();
			foreach(var q in P) p.AppendChar(q);
			return A.I(p);

		}
	}
}
