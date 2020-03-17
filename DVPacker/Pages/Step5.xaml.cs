using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DVPacker {
	/// <summary>
	/// Interaktionslogik für Step5.xaml
	/// </summary>
	public partial class Step5 : Page, IStep {
		readonly MainWindow p;
		public Step5(MainWindow parent) {
			p = parent;
			InitializeComponent();
		}

		public bool IsReady => false;
		/// <summary>
		/// Opens the output folder and selects the file
		/// </summary>
		private void Button_Click(object sender, RoutedEventArgs e) => Process.Start("explorer", $"/select,\"{p.outputlocation}\"");

		private void Button_Click_1(object sender, RoutedEventArgs e) => p.Reset();
	}
}
