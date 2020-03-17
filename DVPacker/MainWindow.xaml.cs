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
		public MainWindow() {
			InitializeComponent();
			cfg = new CC();
			s1 = new Step1(this);
			s2 = new Step2(this);
			s3 = new Step3(this);
			s4 = new Step4(this);
			Viewer.Content = Gs(step);
			DoCheck();
		}
		async void DoCheck() {
			while (Application.Current.Windows.OfType<MainWindow>().Any()) {
				btN.IsEnabled=(Gs(step) as IStep).IsReady;
				await Task.Delay(50);
			}
		}
		public string cryptoPath, mainFilePath;

		[Import]
		public IA crypto;
		public CC cfg;
		public CC.A encryptionType;
		public HashSet<string> localFiles;

		int step = 0;
		public Step1 s1;
		public Step2 s2;
		public Step3 s3;
		public Step4 s4;
		public Page Gs(int n) {
			switch (n) {
				default: return s1;
				case 1: return s2;
				case 2: return s3;
				case 3: return s4;
			}
		}


		private void Button_Click_4(object sender, RoutedEventArgs e) {
			if (!(Gs(step) as IStep).IsReady) return;
			if (++step > 3) step = 3;
			btN.Visibility = step == 3 ? Visibility.Hidden : Visibility.Visible;
			btP.Visibility = step == 0 ? Visibility.Hidden : Visibility.Visible;
			Viewer.Content = Gs(step);
		}

		private void Button_Click_5(object sender, RoutedEventArgs e) {
			if (--step < 0) step = 0;
			btN.Visibility = step == 3 ? Visibility.Hidden : Visibility.Visible;
			btP.Visibility = step == 0 ? Visibility.Hidden : Visibility.Visible;
			Viewer.Content = Gs(step);
		}
	}
}
