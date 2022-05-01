using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TickeningSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (Cinema c = new Cinema())
            {
                var user = c.User.FirstOrDefault(u => u.Login == textBox1.Text && u.Password == textBox2.Text);
                if (user!=null)
                {
                    if (user.Role == "admin")
                    {
                        Form2 f = new Form2("a");
                        f.Show();
                        Hide();
                    }
                    else
                    {
                        Form2 f = new Form2("u");
                        f.Show();
                        Hide();
                    }
                }
                else
                {
                    MessageBox.Show("User with current data doesn't exists!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
        }
    }
}
