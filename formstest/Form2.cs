using System;
using System.Windows.Forms;

namespace formstest {
    public partial class Form2 : Form {
        public Form2() {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e) {

        }

        private void button1_Click(object sender, EventArgs e) {
            progbar1.Vaule = int.Parse( textBox1.Text );
            progbar1.Style = ProgressBarStyle.Marquee;
        }

        private void button2_Click(object sender, EventArgs e) {
            
        }

        private void toolStripContainer1_ContentPanel_Load(object sender, EventArgs e) {

        }

        private void toolStripContainer1_LeftToolStripPanel_Click(object sender, EventArgs e) {

        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e) {

        }

        private void toolStripContainer1_RightToolStripPanel_Click(object sender, EventArgs e) {

        }
    }
}
