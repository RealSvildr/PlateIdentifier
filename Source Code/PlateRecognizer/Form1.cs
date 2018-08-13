using System;
using System.Windows.Forms;

namespace PlateRecognizer {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            Form2 f = new Form2();

            f.Show();
        }
    }
}
