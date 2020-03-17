using System;
namespace HWDLL {
	public class Program {
		public static void Main(string[] args) {
			Console.WriteLine("Payload says ello!!!");
			Console.WriteLine("Executed in: " + System.IO.Directory.GetCurrentDirectory());
			Console.WriteLine("Which Contains: ");
			foreach (var a in System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory()))
				Console.WriteLine($"| \"{a}\"");
			Console.WriteLine("Arguments:");
			foreach (var a in args)
				Console.WriteLine($"| \"{a}\"");
		}
	}
}
