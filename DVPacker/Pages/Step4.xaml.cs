using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace DVPacker {
	/// <summary>
	/// Interaktionslogik für Step4.xaml
	/// </summary>
	public partial class Step4 : Page, IStep {
		private readonly Brush defaultFG;
		public SecureString PasswordS = new SecureString();
		public RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
		private const string options = "#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[]^_`abcdefghijklmnopqrstuvwxyz{|}~ ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþ";
		public bool IsReady { get; private set; } = false;
		private string outputCache = "";
		public string Output { get => outputCache; set => outputCache += '\n' + value; }
		public string OutputNoNL { get => outputCache; set => outputCache += value; }
		readonly MainWindow p;
		public Step4(MainWindow parent) {
			p = parent;
			InitializeComponent();
			defaultFG = pb.Foreground;
			OutputWriter();
		}
		async void OutputWriter() {
			var last = 0;
			while (true) {
				if (Output.Length != last) {
					last = Output.Length;
					tbOutput.Text = Output;
					//tbOutput.CaretIndex = Output.Length;
					tbOutput.ScrollToEnd();
					await Task.Delay(50);
				}
				await Task.Delay(10);
			}
		}
		private async void Button_Click_3(object sender, RoutedEventArgs e) {
			tbOutput.Clear();
			outputCache = "";
			(sender as Button).IsEnabled = false;
			try {
				OutputNoNL = "Generating password";
				var r = new Random();
				PasswordS.Clear();
				for (var n = 0; n < 64; n++) {
					if (r.Next() % 5 != 0) {
						var arr = new byte[2];
						rng.GetBytes(arr);
						PasswordS.AppendChar(BitConverter.ToChar(arr, 0));
					}
				}
				pb.IsIndeterminate = true;
				var dlg = new Microsoft.Win32.SaveFileDialog() {
					RestoreDirectory = true,
					AddExtension = true,
					DefaultExt = "*.exe",
					FileName = Path.GetFileNameWithoutExtension(p.mainFilePath),
					DereferenceLinks = true,
					Title = "Save as",
					ValidateNames = true,
					Filter = "Packed executable *.exe | *.exe"
				};
				Output = "Waiting for Output file...";
				if (dlg.ShowDialog() == false) {
					Output = "Dialog closed without answer.";
					return;
				}
				else
					p.outputlocation = dlg.FileName;
				switch (p.encryptionType) {
					case CC.A.A:
						p.crypto = new CES();
						break;
				}
				p.cfg.B = p.encryptionType;
				var enc = new CW<IA>(p.crypto);
				var dat = new Dictionary<string, byte[]>();
				try {
					Output = "Encrypting Files";
					var t = new Thread(() => LoadFiles(enc, enc.I(PasswordS), dat)) {
						Name = "Encryptor",
					};
					t.Start();
					while (t.IsAlive) await Task.Delay(100);
					t.Join();
					Output = "Encryption Successfull";
				}
				catch (Exception ex) {
					Output = $"ERROR reading and encrypting files\n{ex.Message}";
					//MessageBox.Show(ex.ToString(), "ERROR reading and encrypting files");
					return;
				}
				Output = "Compressing...";

				try {
					var t = new Thread(() => CreateZipMemoryThread(dat)) {
						Name = "Compressor",
					};
					t.Start();
					while (t.IsAlive) await Task.Delay(100);
					t.Join();
					//zipMemory = CreateZipMemory(dat);
				}
				catch (Exception ex) {
					Output = $"ERROR compressing files\n{ex.Message}";
					//MessageBox.Show(ex.ToString(), "ERROR compressing files");
					return;
				}
				pb.IsIndeterminate = false;
				Output = "Running Compiler...";
				await RunCompiler(r, zipMemory, PasswordS, dlg.FileName);
				zipMemory = null;
				p.CurrentPage=p.GetPage(++p.step);
			}
			finally {
				pb.IsIndeterminate = false;
				(sender as Button).IsEnabled = true;
				Output = "Done!";
			}
		}
		byte[] zipMemory;
		public void CreateZipMemoryThread(Dictionary<string, byte[]> dat) => zipMemory = CreateZipMemory(dat);
		public void LoadFiles(CW<IA> enc, SecureString mutated, Dictionary<string, byte[]> dat) {
			foreach (var f in p.localFiles) {
				Output = $"\tLoading \"{f}\"...";
				var sh = Path.GetFileName(f);
				dat.Add(sh, enc.II(File.ReadAllBytes(f), mutated));
				p.cfg.C.Add(sh);
				OutputNoNL = " done.";
			}
			Output = "\tLoading dll";
			dat.Add(p.cfg.d, enc.II(File.ReadAllBytes(p.mainFilePath), mutated));
			if (p.encryptionType == CC.A.B) {
				p.cfg.c = Path.GetFileName(p.cryptoPath);
				dat.Add(p.cfg.c, File.ReadAllBytes(p.cryptoPath));
			}
		}
		private byte[] CreateZipMemory(Dictionary<string, byte[]> dat) {
			byte[] zipMemory;
			using (var ms = new MemoryStream()) {
				using (var a = new ZipArchive(ms, ZipArchiveMode.Create)) {
					foreach (var k in dat.Keys) {
						Output = $"\tCompressing \"{k}\"";
						using (var eStream = a.CreateEntry(k, CompressionLevel.Optimal).Open())
							eStream.Write(dat[k], 0, dat[k].Length);
					}
					Output = $"\tCompressing dll";
					using (var configStream = a.CreateEntry(CC.b).Open())
						p.cfg.a(configStream);
				}
				zipMemory = ms.ToArray();
			}
			return zipMemory;
		}
		private async Task RunCompiler(Random rng, byte[] zipMemory, SecureString InnerPW, string outFile) {
			var pw = "";
			for (var n = 0; n < (rng.Next() % 64) + 64; n++)
				pw += options[rng.Next() % options.Length];
			var AES = new CW<CES>(new CES());
			var salt = new byte[64];
			rng.NextBytes(salt);
			var psalt = CES.salt;
			CES.salt = salt;
			var p = new Compiler.Parameters {
				password = pw,
				OuterSalt = salt,
				innerPassword = InnerPW,
				outFile = outFile,
				ExecutionLevel = this.p.s3.ExecutionLevel,
				uiAccess = this.p.s3.UIAccess
			};
			var compiler = new Thread(() => Compiler.Compile(AES.II(zipMemory, AES.I(pw)), p, this)) {
				Priority = ThreadPriority.Highest
			};
			Compiler.stage = 0;
			pb.Maximum = Compiler.stagemax;
			pb.IsIndeterminate = false;
			compiler.Start();
			while (compiler.IsAlive) {
				pb.Value = Compiler.stage;
				await Task.Delay(50);
			}
			Output = "Compilation complete";
			pb.Value = Compiler.stage;
			compiler.Join();
			CES.salt = psalt;

		}
	}
}
