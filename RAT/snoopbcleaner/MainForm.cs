using System;
using System.Windows.Forms;

namespace SnoopBCleaner
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            try
            {
                var cleaner = new Cleaner();;
                cleaner.Clean(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Phrases.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
       
        }
    }
}