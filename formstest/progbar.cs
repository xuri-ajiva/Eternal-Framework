using System;
using System.Windows.Forms;

namespace formstest {
    public partial class progbar : UserControl {
        public progbar() {
            InitializeComponent();
        }

        private void progbar_Paint(object sender, PaintEventArgs e) {

        }

        private void progbar_Resize(object sender, EventArgs e) {
            update();
        }

        private void update() {
            label1.Text = ( progressBar1.Value / progressBar1.Maximum ) + " %";
        }

        public int Max {
            get => progressBar1.Maximum;
            set => progressBar1.Maximum = value > 0 ? value : 1;
        }

        public int Min {
            get => progressBar1.Minimum;
            set => progressBar1.Minimum = value;
        }

        public int Vaule {
            get => progressBar1.Value;
            set => progressBar1.Value = value;
        }

        public ProgressBarStyle Style {
            get => progressBar1.Style;
            set => progressBar1.Style = value;
        }

        private void label1_Click(object sender, EventArgs e) {

        }
    }
}
