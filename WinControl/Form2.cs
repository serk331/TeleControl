using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinControl
{
    public partial class Form2 : Form
    {
        Form1 form1;
        public Form2()
        {
            InitializeComponent();
            if (System.IO.File.Exists("auto.txt") == true)
            {
                string auto = System.IO.File.ReadAllText("auto.txt");
                if (auto == "true")
                {
                    var token = System.IO.File.ReadAllText("token.txt");
                    var password = System.IO.File.ReadAllText("password.txt");
                    var sharedPath = System.IO.File.ReadAllText("sharedPath.txt");
                    form1 = new Form1(token, password, this, sharedPath);
                    
                   
                    this.ShowInTaskbar = false;
                    WindowState = FormWindowState.Minimized;
                    form1.Show();
                    this.Hide();
                    this.Opacity = 0;
                }
            }
            if (System.IO.File.Exists("token.txt") == true)
            {
                var token = System.IO.File.ReadAllText("token.txt");
                textBox1.Text = token;
            }
            if (System.IO.File.Exists("password.txt") == true)
            {
                var password = System.IO.File.ReadAllText("password.txt");
                textBox2.Text = password;
            }
            if (System.IO.File.Exists("sharedPath.txt") == true)
            {
                var sharedPath = System.IO.File.ReadAllText("sharedPath.txt");
                textBox3.Text = sharedPath;
            }
            if (System.IO.File.Exists("auto.txt") == true)
            {
                var auto = System.IO.File.ReadAllText("auto.txt");
                if (auto == "true")
                {
                    checkBox1.Checked = true;
                }
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var token = textBox1.Text;
            var password = textBox2.Text;
            var sharedPath = textBox3.Text;
            Form1 form1 = new Form1(token, password, this, sharedPath);
            this.Hide();
            form1.Show();
            if(checkBox1.Checked)
            {
                System.IO.File.WriteAllText("auto.txt", "true");
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
