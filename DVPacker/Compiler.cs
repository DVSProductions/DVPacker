using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace DVPacker {
	class Compiler {
		#region hiddenStorage
#pragma warning disable IDE1006 // Benennungsstile
		private Random _rng = null;
		private string _compilerFolder = null;
		private string _payloadName = null;
		private string _code = null;
		private string _proj = null;
		private string _GenPayloadName() => new string(Path.GetFileNameWithoutExtension(Path.GetTempFileName()).Insert(0, Path.GetRandomFileName()).ToCharArray().OrderBy(x => rng.Next()).ToArray());
		#endregion
		private Random rng => _rng ?? (_rng = new Random());
#pragma warning restore IDE1006 // Benennungsstile
		public string CompilerFolder => _compilerFolder ?? (_compilerFolder = Path.GetTempPath() + Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + "\\");
		private string PayloadName => _payloadName ?? (_payloadName = _GenPayloadName());
		private string CodeFolder => CompilerFolder + "Included\\";
		private string MainCS => CodeFolder + "Program.cs";
		private string ProjFile => CodeFolder + "Unpacker.csproj";
		private string Manifest => CodeFolder + "app.manifest";
		private string Code { get => _code ?? File.ReadAllText(MainCS).Replace("\r\n", "\n"); set => _code = value; }
		private string Proj { get => _proj ?? File.ReadAllText(ProjFile).Replace("\r\n", "\n"); set => _proj = value; }
		void ReplaceProj(string tagName, string newContent) => Proj = Regex.Replace(Proj, $"<{tagName}>.*?</{tagName}>", $"<{tagName}>{newContent}</{tagName}>");
		void WriteChanges() {
			File.WriteAllText(MainCS, Code);
			File.WriteAllText(ProjFile, Proj);
		}
		private Compiler() { }
		public void StartShit() {
			Directory.CreateDirectory(CompilerFolder);
			var asm = System.Reflection.Assembly.GetExecutingAssembly();
			foreach (var s in asm.GetManifestResourceNames()) {
				if (s.EndsWith("BuildTools.zip"))
					using (var zip = new ZipArchive(asm.GetManifestResourceStream(s)))
						zip.ExtractToDirectory(CompilerFolder);
			}
			//Debug.WriteLine(CompilerFolder);
		}
		public void Cleanup(Step4 t) {
			try {
				Process.GetProcessesByName("VBCSCompiler").ToList().ForEach((p) => p.Kill());
			}
			catch (Exception ex) {
				t.Output=$"\tError during cleanup... Couldn't kill one or more Compilers: {ex.Message}";
			}
			if (Directory.Exists(CompilerFolder))
				Directory.Delete(CompilerFolder, true);
		}

		void WriteFile(string filename, byte[] data) => File.WriteAllBytes(CompilerFolder + filename, data);
		void WritePayload(byte[] data) => File.WriteAllBytes(CodeFolder + PayloadName, data);

		void RegionPrepend(string targetRegion, string code) {
			var r = Regex.Match(Code, $"#region {targetRegion}\\s*\\n");
			Code = Code.Insert(r.Index + r.Length, code);
		}
		void RegionReplace(string name, string code) => Code = Regex.Replace(Code, $"#region {name}\\s*\\n(.|\\n)*?#endregion", $"#region {name}\n{code}\n#endregion", RegexOptions.Multiline);
		struct XTSR {
			public int tagstart, tagend, endTagStart, endTagEnd;
		}
		XTSR XMLTagSearch(string where, string tag) {
			var result = default(XTSR);
			var r1 = Regex.Match(where, $"<\\s*{tag}(|(\\s+((\".*?[^\\\\]\"|.)*?)))>(\\s|(<((\".*?[^\\\\]\"|.)*?)>))*?\\s*?</\\s*{tag}\\s*>");
			var r2 = Regex.Match(where, $"<\\s*{tag}(|(\\s+((\".*?[^\\\\]\"|.)*?)))>");
			var r3 = Regex.Match(r1.Value, $"</\\s*{tag}\\s*>$");
			result.tagstart = r1.Index;
			result.tagend = r2.Index + r2.Length;
			result.endTagEnd = r1.Index + r1.Length;
			result.endTagStart = result.endTagEnd - r3.Length;
			return result;
		}
		string XMLTagReplaceInnerContent(string where, string tag, string content) {
			var r = XMLTagSearch(where, tag);
			return where.Substring(0, r.tagend) + content + where.Substring(r.endTagStart, r.endTagEnd - r.endTagStart);
		}
		void SetPrivileges(string level, bool uiAccess) => File.WriteAllText(Manifest, XMLTagReplaceInnerContent(File.ReadAllText(Manifest), "requestedPrivileges", $"<requestedExecutionLevel level=\"{ level}\" uiAccess=\"{(uiAccess ? "true" : "false")}\" />"));

		void SetSupportedOS(Parameters p) {
			string genOS(int ID) => ID == 0 ? "{e2011457-1546-43c5-a5fe-008deee3d3f0}" : ID == 1 ? "{35138b9a-5d96-4fbd-8e2d-a2440225f93a}" : ID == 2 ? "{4a2f28e3-53b9-4441-ba9c-d69d4a4a6e38}" : ID == 3 ? "{1f676c76-80e1-4239-95bb-83d0f6d0da78}" : "{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}";
			string supOS(int ID) => $"<supportedOS Id=\"{genOS(ID)}\" />\n";
			File.WriteAllText(Manifest, XMLTagReplaceInnerContent(File.ReadAllText(Manifest), "application", (p.SupportedOS.Item1 ? supOS(0) : "") + (p.SupportedOS.Item2 ? supOS(1) : "") + (p.SupportedOS.Item3 ? supOS(2) : "") + (p.SupportedOS.Item4 ? supOS(3) : "") + (p.SupportedOS.Item5 ? supOS(4) : "")));
		}

		void SetResourceEntry() => Proj = Regex.Replace(Proj, "<ItemGroup>\\n\\s*<EmbeddedResource .*?>\\n\\s*</ItemGroup>", $"<ItemGroup>\n\t<EmbeddedResource Include=\"{PayloadName}\" />\n\t</ItemGroup>");
		public static string ToArrayString(byte[] arr) {
			var s = new StringBuilder();
			foreach (var b in arr) s.Append($"{(int)b}, ");
			return s.ToString();
		}

		public void Compile(Step4 acc) {
			var i = new ProcessStartInfo() {
				FileName = CompilerFolder + "BuildTools\\MSBuild.exe",
				Arguments = CodeFolder + "Unpacker.csproj",
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				RedirectStandardInput = true,
				UseShellExecute = false,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			};
			var p = Process.Start(i);
			//Debug.WriteLine("Out: " + p.StandardOutput.ReadToEnd());
			//Debug.WriteLine("Err: " + p.StandardError.ReadToEnd());
			while (!p.HasExited)
				acc.Output ="\t\t"+ p.StandardOutput.ReadLine();
			p.WaitForExit();
		}
		static string GenSSCode(SecureString ss) {
			var sb = new StringBuilder("public static SecureString b{get{var s=new SecureString();");
			var valuePtr = IntPtr.Zero;
			try {
				valuePtr = Marshal.SecureStringToGlobalAllocUnicode(ss);
				foreach (var c in Marshal.PtrToStringUni(valuePtr)) {
					var ba = BitConverter.GetBytes(c);
					sb.Append($"s.AppendChar(BitConverter.ToChar(new byte[]{{{ba[0]},{ba[1]}}},0));");
				}
			}
			finally {
				Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
			}
			sb.Append("return s;}}");
			return sb.ToString();
		}
		public static int stage = 0;
		public static int stagemax = 7;
		public struct Parameters {
			public byte[] OuterSalt;
			public string password, outFile, ExecutionLevel;
			public SecureString innerPassword;
			public bool uiAccess;
			public (bool, bool, bool, bool, bool) SupportedOS;
			public void FixOS() {
				if (SupportedOS.Item1 && SupportedOS.Item2 && SupportedOS.Item3 && SupportedOS.Item4 && SupportedOS.Item5)
					SupportedOS = (false, false, false, false, false);
			}
		}
		public static void Compile(byte[] internalZip, Parameters p, Step4 t) {
			var assistant = new Compiler();
			try {
				t.Output = "\tFixing OS";
				p.FixOS();
				stage++;
				t.Output = "\tPreparing Compiler";
				assistant.StartShit();
				stage++;
				t.Output = "\tGenerating Payload";
				assistant.WritePayload(internalZip);
				stage++;
				t.Output = "\tCreating Configuration";
				assistant.RegionReplace("Inner Password", GenSSCode(p.innerPassword));
				assistant.RegionReplace("Outer Password", $"public static readonly string B =\"{p.password}\";");
				assistant.RegionReplace("salt", $"public static readonly byte[] C = new byte[]{{{ToArrayString(p.OuterSalt)}}};");
				assistant.RegionReplace("FileName", $"public static string c = \"{assistant.PayloadName}\";");
				assistant.SetResourceEntry();
				stage++;
				t.Output = "\tPreparing Compilation";
				assistant.ReplaceProj("OutputPath", Path.GetDirectoryName(p.outFile) + "\\");
				assistant.ReplaceProj("AssemblyName", Path.GetFileNameWithoutExtension(p.outFile));
				assistant.SetPrivileges(p.ExecutionLevel, p.uiAccess);
				assistant.SetSupportedOS(p);
				t.Output = "\tPushing Buffers";
				assistant.WriteChanges();
				stage++;
				t.Output = "\tCompiling...";
				assistant.Compile(t);
				stage++;
			}
			finally {
				t.Output = "\tPerforming Cleanup";
				assistant.Cleanup(t);
				stage++;
			}
		}

	}
}
