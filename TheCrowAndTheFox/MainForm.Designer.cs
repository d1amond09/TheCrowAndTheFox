using System.Windows.Forms;
using TheCrowAndTheFox.Models;

namespace TheCrowAndTheFox
{
	public partial class MainForm : Form
	{
		private Label label1;


		public MainForm()
		{
			InitializeComponent();
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Application.Exit();
		}
	}
}