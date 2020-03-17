using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DVPacker {
	/// <summary>
	/// Interaktionslogik für Step3.xaml
	/// </summary>
	public partial class Step3 : Page, IStep {
		readonly MainWindow p;
		public bool IsReady { get; private set; } = false;
		public Step3(MainWindow parent) {
			p = parent;
			InitializeComponent();
		}
		public (bool, bool, bool, bool, bool) GetCheckedOS() {
			var it = new List<bool>();
			foreach(var e in spSupportedOS.Children)
				it.Add((e as CheckBox).IsChecked == true);
			return (it[0], it[1], it[2], it[3], it[4]);

		}
		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if(btEnc == null) return;
			switch((sender as ComboBox).SelectedIndex) {
				case (0):
					p.encryptionType = CC.A.A;
					btEnc.Visibility = Visibility.Collapsed;
					IsReady = true;
					break;
				case (1):
					p.encryptionType = CC.A.B;
					btEnc.Visibility = Visibility.Visible;
					if(!hasICrypto)IsReady = false;
					break;
			}
		}
		public string ExecutionLevel => (cbExecutionLevel.SelectedItem as ComboBoxItem).Content as string;
		public bool UIAccess => cbuiAccess.IsChecked == true;
		bool hasICrypto = false;
		private void Button_Click_2(object sender, RoutedEventArgs e) {
			var dlg = new Microsoft.Win32.OpenFileDialog {
				DefaultExt = "*.dll",
				CheckFileExists = true,
				CheckPathExists = true,
				Multiselect = false,
				RestoreDirectory = true,
				DereferenceLinks = true,
				Title = "Please select a DLL that Implements ICrypto",
				Filter = "Crypto Files *.dll|*.dll"
			};
			if(dlg.ShowDialog() == true) {
				p.crypto = null;
				try {
					using(var cat = new AggregateCatalog()) {
						cat.Catalogs.Add(new DirectoryCatalog(Path.GetDirectoryName(dlg.FileName), Path.GetFileName(dlg.FileName)));
						var loader = new CompositionContainer(cat);
						loader.ComposeParts(this);
					}
					p.cryptoPath = dlg.FileName;
					(sender as Button).Content = Path.GetFileName(dlg.FileName);
					hasICrypto = true;
					IsReady = true;
				}
				catch(Exception ex) {
					MessageBox.Show(ex.ToString(), "Error Loading Crypto");
					return;
				}
			}
		}
	}
}
