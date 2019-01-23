using System;
using System.Windows.Forms;

namespace TerraSketch.View
{
    public partial class FrmWaitView : Form
    {
        public FrmWaitView()
        {
            InitializeComponent();
            textBox1.Text = debugText;
        }

        private string debugText = "Working on a terrain";
        public void UpdateText(string text)
        {
            debugText += Environment.NewLine+ text;
            textBox1.Text = debugText;
            textBox1.ScrollToCaret();
        }

        private void FrmWaitView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
    }
}
