using System;

namespace HWDLL {
	public class Program {
		public static void Main(string[] args) {
			Console.WriteLine("Payload says ello!!!\nArguments:");
			foreach(var a in args) Console.WriteLine($"| \"{a}\"");
		}
	}
}
