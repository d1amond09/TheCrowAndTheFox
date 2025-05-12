using System.Windows.Forms;
using System;

namespace TheCrowAndTheFox
{
	public partial class MainForm : Form
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				components?.Dispose();
				_gameInstance?.Dispose();
				_audioManager?.Dispose();
			}
			base.Dispose(disposing);
		}

		private void MainForm_Load(object sender, EventArgs e)
		{

		}
	}
}
