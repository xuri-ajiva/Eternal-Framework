using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Software
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            listView1.MultiSelect = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (var o in openFileDialog1.FileNames)
                {
                    listView1.Items.Add(o);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                listView1.Items.Remove(item);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var args = "u ";
                foreach (var o in openFileDialog1.FileNames)
                {
                    var p = new Process();
                    p.StartInfo = new ProcessStartInfo("container.exe", args + o);
                    p.Start();
                    p.WaitForExit();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("   Sicher das sie fortfahren wollen?") == DialogResult.OK)
            {
                var args = "p ";
                foreach (ListViewItem o in listView1.Items)
                {
                    args += o.Text + " ";
                }

                var p = new Process();
                p.StartInfo = new ProcessStartInfo("container.exe", args);
                p.Start();
                p.WaitForExit();
            }
        }
    }
}
