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
    public partial class Form2 : Form
    {
        string role;
        public Form2(string role)
        {
            this.role = role;
            InitializeComponent();
            if(role=="u")
            {
                button2.Visible = false;
                button3.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MovieForm f = new MovieForm(role);
            f.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UserForm f = new UserForm();
            f.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RoomForm f = new RoomForm(role);
            f.ShowDialog();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
        }
    }
}
