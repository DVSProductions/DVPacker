using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DVPacker {
	/// <summary>
	/// Interaktionslogik für Step1.xaml
	/// </summary>
	public partial class Step1 : Page,IStep {
		readonly MainWindow myParent;
		public Step1(MainWindow parent) {
			myParent = parent;
			InitializeComponent();
		}

		public bool IsReady { get; private set; } = false;

		private void Button_Click(object sender, RoutedEventArgs e) {
			var dlg = new Microsoft.Win32.OpenFileDialog {
				DefaultExt = "*.dll",
				CheckFileExists = true,
				CheckPathExists = true,
				Multiselect = false,
				RestoreDirectory = true,
				DereferenceLinks = true,
				Title = "Please select a DLL to encrypt",
				Filter = "DLL Files *.dll|*.dll"
			};
			if(dlg.ShowDialog() == true) {
				myParent.mainFilePath = dlg.FileName;
				myParent.cfg.d = Path.GetFileName(dlg.FileName);
				(sender as Button).Content = myParent.cfg.d;
				IsReady = true;
			}
		}
	}
}
