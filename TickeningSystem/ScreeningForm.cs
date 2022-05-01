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
    public partial class ScreeningForm : Form
    {
        List<dynamic> screenings;
        public ScreeningForm(string role)
        {

            InitializeComponent();
            if (role == "a")
                button1.Visible = true;
        }

        void LoadMovies()
        {
            using (Cinema c = new Cinema())
            {
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(
                            c.Movie.Select(m=>m.Name).ToArray());
            }
        }

        List<dynamic> GetScreenings()
        {
            using (Cinema c=new Cinema())
            {
                var screenings= c.Screening
                    .Where(s=>
                     (s.ScreeningStatus.Name!="Завершено"||
                     s.ScreeningStatus.Name != "У процесі"))
                     .Select(s =>new { 
                         Id=s.Id,
                        MovieName=s.Id.ToString() + " " + s.Movie.Name,
                        RoomName=s.Room.Name,
                        StartDateTime=s.ScreeningStart});
                if (screenings.Any())
                {
                    return screenings.ToList<dynamic>();
                }
                return null;
            }
        }
        void UpdateScreenings()
        {
            TimeSpan diff;
            if(screenings!=null)
            {
                using(Cinema c=new Cinema())
                {
                    foreach(var s in c.Screening.ToList())
                    {
                        if (s.ScreeningStart < DateTime.Now)
                        {
                            diff = s.ScreeningStart - DateTime.Now;
                            if(diff.TotalMinutes<=s.Movie.Duration&&diff.TotalMinutes>=1)
                            {
                                s.StatusId = 2;
                                c.Entry(s).State = System.Data.Entity.EntityState.Modified;
                                c.SaveChanges();
                            }
                            else
                            {
                                
                                    s.StatusId = 3;
                                    c.Entry(s).State = System.Data.Entity.EntityState.Modified;
                                    c.SaveChanges();
                                
                            }
                        }
                    }
                }
            }

        }

        void LoadScreenings()
        {
            dataGridView1.ColumnCount = 4;
            dataGridView1.Rows.Clear();
            if (screenings!=null)
            {
                foreach (var s in screenings)
                {
                    dataGridView1.Rows.Add(s.Id,s.MovieName, s.RoomName, s.StartDateTime);
                }
                dataGridView1.Columns.Add(new DataGridViewButtonColumn());
                for(int i=0;i<dataGridView1.RowCount-1;i++)
                {
                    dataGridView1.Rows[i].Cells[4].Value = "До квитків";
                }
            }
        }

        private void ScreeningForm_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            screenings = GetScreenings();
            UpdateScreenings();
            LoadMovies();
            LoadScreenings();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateScreenings();
            screenings = GetScreenings();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            screenings = screenings.Where(s =>comboBox1.SelectedItem.ToString().Contains(s.Name)).ToList<dynamic>();
            LoadScreenings();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (screenings != null)
            {
                comboBox1.SelectedIndex = -1;
                comboBox1.Text = "";
                screenings = screenings.Where(s => s.StartDateTime.ToString("yyyy-MM-dd HH:mm") == dateTimePicker1.Value.ToString("yyyy=MM-dd HH:mm")).ToList<dynamic>();
                LoadScreenings();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = -1;
            comboBox1.Text = "";
            screenings = GetScreenings();
            LoadScreenings();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddScreeningForm f = new AddScreeningForm();
            f.ShowDialog();
        }

        private void ScreeningForm_Activated(object sender, EventArgs e)
        {
            UpdateScreenings();
            screenings = GetScreenings();
            LoadMovies();
            LoadScreenings();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex!=-1&&e.RowIndex!=dataGridView1.RowCount-1)
            {
                if(e.ColumnIndex==4)
                {
                    using (Cinema c = new Cinema())
                    {
                        var id = dataGridView1[0, e.RowIndex].Value.ToString();
                        var screening = c.Screening.First(s => s.Id.ToString() == id);
                        TicketForm f = new TicketForm(screening);
                        f.ShowDialog();
                    }
                }
            }
        }
    }
}
