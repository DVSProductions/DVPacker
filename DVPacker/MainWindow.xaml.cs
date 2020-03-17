using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DVPacker {
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		const int maxstepidx = 4;
		public string cryptoPath, mainFilePath, outputlocation;
		[Import]
		public IA crypto;
		public CC cfg;
		public CC.A encryptionType;
		public HashSet<string> localFiles;
		public int step;
		public Step1 s1;
		public Step2 s2;
		public Step3 s3;
		public Step4 s4;
		public Step5 s5;
		public void Reset() {
			cryptoPath = default;
			mainFilePath = default;
			outputlocation = default;
			cfg = new CC();
			crypto = default;
			encryptionType = default;
			localFiles?.Clear();
			localFiles = default;
			s1 = new Step1(this);
			s2 = new Step2(this);
			s3 = new Step3(this);
			s4 = new Step4(this);
			s5 = new Step5(this);
			Viewer.Content = GetPage(step = 0);
			btP.Visibility = Visibility.Hidden;
		}
		public MainWindow() {
			InitializeComponent();
			Reset();
			DoCheck();
		}
		async void DoCheck() {
			while (Application.Current.Windows.OfType<MainWindow>().Any()) {
				btN.IsEnabled = (GetPage(step) as IStep).IsReady;
				btN.Visibility = step == maxstepidx ? Visibility.Hidden : Visibility.Visible;
				await Task.Delay(50);
			}
		}
		
		public Page GetPage(int n) {
			switch (n) {
				default: return s1;
				case 1: return s2;
				case 2: return s3;
				case 3: return s4;
				case 4: return s5;
			}
		}
		public Page CurrentPage { get => Viewer.Content as Page; set => Viewer.Content = value; }
		private void Button_Click_4(object sender, RoutedEventArgs e) {
			if (!(GetPage(step) as IStep).IsReady) return;
			if (++step > maxstepidx) step = maxstepidx;
			btN.Visibility = step == maxstepidx ? Visibility.Hidden : Visibility.Visible;
			btP.Visibility = step == 0 ? Visibility.Hidden : Visibility.Visible;
			Viewer.Content = GetPage(step);
		}

		private void Button_Click_5(object sender, RoutedEventArgs e) {
			if (--step < 0) step = 0;
			btN.Visibility = step == maxstepidx ? Visibility.Hidden : Visibility.Visible;
			btP.Visibility = step == 0 ? Visibility.Hidden : Visibility.Visible;
			Viewer.Content = GetPage(step);
		}
	}
}
