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
    public partial class EditMovieForm : Form
    {
        Movie movie;
        public EditMovieForm(string movieName)
        {
            InitializeComponent();
            using (Cinema c = new Cinema())
            {
                movie = c.Movie.FirstOrDefault(m => m.Name == movieName);
                textBox1.Text = movie.Name;
                textBox2.Text = movie.Producer;
                numericUpDown1.Value = movie.Duration;
                richTextBox1.Text = movie.Description;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateGenreForm f = new CreateGenreForm();
            f.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "" && numericUpDown1.Value > 0
                && comboBox1.SelectedIndex != -1 && richTextBox1.Text != "")
            {
                using (Cinema c = new Cinema())
                {
                    if (c.Movie.Any(m => m.Name == textBox1.Text))
                    {
                        var selectedGenre = c.Genre.First(g => g.Name == comboBox1.SelectedItem.ToString());
                        movie.Name = textBox1.Text;
                        movie.Producer = textBox2.Text;
                        movie.Description = richTextBox1.Text;
                        movie.Duration = Convert.ToInt32(numericUpDown1.Value);
                        movie.Genre = selectedGenre;
                        c.Entry(movie).State = System.Data.Entity.EntityState.Modified;
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

        private void EditMovieForm_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            LoadGenres();
        }

        public void LoadGenres()
        {
            using (Cinema c = new Cinema())
            {
                if (c.Genre.Any())
                {
                    comboBox1.Items.Clear();
                    comboBox1.Items.AddRange(c.Genre.Select(g => g.Name).ToArray());
                }

            }
        }

        private void EditMovieForm_Activated(object sender, EventArgs e)
        {
            LoadGenres();
        }
    }
}
