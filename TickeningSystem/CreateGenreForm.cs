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
    public partial class CreateGenreForm : Form
    {
        public CreateGenreForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (Cinema c = new Cinema())
            {
                if(c.Genre.Any(g=>g.Name==textBox1.Text))
                {
                    MessageBox.Show("Такий жанр вже існує!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } 
                else
                {
                    c.Genre.Add(new Genre
                    {
                        Name = textBox1.Text
                    });
                    c.SaveChanges();
                    MessageBox.Show("OK!");
                    Close();
                }
            }
        }

        private void CreateGenreForm_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
        }
    }
}
