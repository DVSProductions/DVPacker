using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
namespace DVPacker {
	/// <summary>
	/// Interaktionslogik für Step2.xaml
	/// </summary>
	public partial class Step2 : Page, IStep {
		MainWindow p;
		public Step2(MainWindow parent) {
			InitializeComponent();
			p = parent;
			p.localFiles = new HashSet<string>();

		}
		public bool IsReady { get; private set; } = true;

		private void Button_Click_1(object sender, RoutedEventArgs e) {
			var dlg = new Microsoft.Win32.OpenFileDialog {
				DefaultExt = "*.*",
				CheckFileExists = true,
				CheckPathExists = true,
				Multiselect = true,
				RestoreDirectory = true,
				DereferenceLinks = true,
				Title = "Please select Additional Files to encrypt",
				Filter = "Any File *.*|*.*"
			};
			if (dlg.ShowDialog() == true) {
				foreach (var f in dlg.FileNames) {
					if (p.localFiles.Contains(f)) continue;
					p.localFiles.Add(f);
					var b = new Button() {
						Height = 15,
						Width = 15,
						Content = "-",
						HorizontalAlignment = HorizontalAlignment.Left,
						VerticalAlignment = VerticalAlignment.Center,
						FontSize = 21,
						Style = FindResource("BTStyle") as Style,
						Padding = new Thickness(0, -10, 0, 0)
					};
					var g = new StackPanel() {
						HorizontalAlignment = HorizontalAlignment.Stretch,
						Orientation = Orientation.Horizontal,
						Children ={
							b,
							new Label(){
								Content = Path.GetFileName(f),
								VerticalAlignment =VerticalAlignment.Stretch,
								VerticalContentAlignment =VerticalAlignment.Center,
								Foreground= new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White)
							}
						}
					};
					b.Click += (a, c) => {
						lbAdditional.Items.Remove(g);
						p.localFiles.Remove(f);
					};
					lbAdditional.Items.Add(g);

				}
			}
		}

	}
}
