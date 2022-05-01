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
    public partial class AddMovieForm : Form
    {
        public AddMovieForm()
        {
            InitializeComponent();
        }

        private void AddMovieForm_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            LoadGenres();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateGenreForm f = new CreateGenreForm();
            f.ShowDialog();
        }

        private void AddMovieForm_Activated(object sender, EventArgs e)
        {
            LoadGenres();
        }
        public void LoadGenres()
        {
            using (Cinema c= new Cinema())
            {
                if (c.Genre.Any())
                {
                    comboBox1.Items.Clear();
                    comboBox1.Items.AddRange(c.Genre.Select(g => g.Name).ToArray());
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox1.Text!=""&&textBox2.Text!=""&&numericUpDown1.Value>0
                &&comboBox1.SelectedIndex!=-1&&richTextBox1.Text!="")
            {
                using (Cinema c = new Cinema())
                {
                    if (!c.Movie.Any(m => m.Name == textBox1.Text))
                    {
                        var selectedGenre = c.Genre.First(g => g.Name == comboBox1.SelectedItem.ToString());
                        c.Movie.Add(new Movie
                        {
                            Name = textBox1.Text,
                            Producer = textBox2.Text,
                            Duration = Convert.ToInt32(numericUpDown1.Value),
                            Description = richTextBox1.Text,
                            Genre = selectedGenre
                        }
                        );
                        c.SaveChanges();
                        MessageBox.Show("OK!");
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Фильм з такою назвою вже існує!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                }
            }
            else
            {
                MessageBox.Show("Заповніть поля!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
