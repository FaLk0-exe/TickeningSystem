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
    public partial class MovieForm : Form
    {
        string role;
        List<dynamic> movies;
        DataGridViewButtonColumn dgvc = new DataGridViewButtonColumn();
        public MovieForm(string role)
        {
            InitializeComponent();
            movies = GetMovies();
            this.role = role;
            if(role!="a")
            button1.Visible = false;
        }

        List<dynamic> GetMovies()
        {
            using (Cinema c = new Cinema())
            {
                if (c.Movie.Any())
                {
                    return c.Movie.Select(m => new
                    {
                        Name = m.Name,
                        Producer = m.Producer,
                        Description = m.Description,
                        Duration = m.Duration,
                        Genre = m.Genre.Name
                    }).ToList<dynamic>();
                }
                return null;
            }
        }

        void LoadGenres()
        {
            comboBox1.Items.Clear();
            using (Cinema c = new Cinema())
            {
                if (c.Genre.Any())
                {
                    comboBox1.Items.AddRange(c.Genre.Select(G => G.Name).ToArray());
                }
            }
        }

        void LoadMovies()
        {
            dataGridView1.ColumnCount = 4;
            dataGridView1.Rows.Clear();
            if (movies != null)
            {
                foreach (var m in movies)
                {
                    dataGridView1.Rows.Add(m.Name, m.Producer, m.Duration, m.Genre);
                }
                int loadCount = 1;
                if (role != "u")
                    loadCount += 2;
                for (int i = 0; i < loadCount; i++)
                    dataGridView1.Columns.Add(new DataGridViewButtonColumn());

                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    dataGridView1.Rows[i].Cells[4].Value = "...";
                    if (role != "u")
                    {
                        dataGridView1.Rows[i].Cells[5].Value = "Видалити";
                        dataGridView1.Rows[i].Cells[6].Value = "Редагувати";
                    }
                }
                movies = GetMovies();
            }
        }

        private void MovieForm_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            LoadMovies();
            LoadGenres();
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (movies != null)
            {
                switch (e.ColumnIndex)
                {
                    case 0:
                        movies = movies.OrderBy(m => m.Name).ToList();
                        break;
                    case 1:
                        movies = movies.OrderBy(m => m.Producer).ToList();
                        break;
                    case 2:
                        movies = movies.OrderBy(m => m.Duration).ToList();
                        break;
                    case 3:
                        movies = movies.OrderBy(m => m.Genre).ToList();
                        break;
                }
                LoadMovies();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            movies = GetMovies();
            textBox1.Text = "";
            textBox2.Text = "";
            if (movies != null)
            {
                movies = movies.Where(m => m.Genre.Contains(comboBox1.SelectedItem.ToString())).ToList();
                LoadMovies();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddMovieForm f = new AddMovieForm();
            f.ShowDialog();
        }

        private void MovieForm_Activated(object sender, EventArgs e)
        {
            textBox2.Text = "";
            textBox1.Text = "";
            comboBox1.SelectedIndex = -1;
            comboBox1.Text = "";
            LoadMovies();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (movies.Count != 0&&e.RowIndex!=-1&&e.RowIndex!=dataGridView1.RowCount-1)
            {
                if (e.ColumnIndex == 4)
                {
                    MessageBox.Show(movies.First(m => m.Name == dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()).Description);
                }
                if (e.ColumnIndex == 5)
                {
                    DialogResult dialogResult = MessageBox.Show("Ви впевнені, що хочете видалити цей фільм?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        using (Cinema c = new Cinema())
                        {
                            var dgv = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                            var movie = c.Movie.First(m => m.Name ==dgv
                             ).Id;
                            if (!c.Screening.Any(s => s.MovieId == movie)
                            )
                            {
                                var deletedMovie = c.Movie.First(m => m.Name ==
                               dgv);
                                c.Entry(deletedMovie).State = System.Data.Entity.EntityState.Deleted;
                                c.SaveChanges();
                                MessageBox.Show("OK");
                                LoadMovies();
                            }
                            else
                            {
                                MessageBox.Show("Дочекайтеся закінчення поточних сеансів");
                            }
                        }
                    }
                }
                if(e.ColumnIndex==6)
                {
                    EditMovieForm f = new EditMovieForm(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                    f.ShowDialog();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            movies = GetMovies();
            textBox1.Text = "";
            textBox2.Text = "";
            comboBox1.SelectedIndex = -1;
            comboBox1.Text = "";
            if (movies != null)
            {
                LoadMovies();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            movies = GetMovies();
            textBox2.Text = "";
            comboBox1.SelectedIndex = -1;
            comboBox1.Text = "";
            if (movies != null)
            {
                movies = movies.Where(m => m.Name.Contains(textBox1.Text)).ToList();
                LoadMovies();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            movies = GetMovies();
            textBox1.Text = "";
            comboBox1.SelectedIndex = -1;
            comboBox1.Text = "";
            if (movies != null)
            {
                movies = movies.Where(m => m.Producer.Contains(textBox2.Text)).ToList();
                LoadMovies();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ScreeningForm f = new ScreeningForm(role);
            f.ShowDialog();
        }
    }
}
