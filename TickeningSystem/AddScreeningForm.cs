using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TickeningSystem
{
    public partial class AddScreeningForm : Form
    {
        DateTime date;
        bool IsClicked = false;
        public AddScreeningForm()
        {
            InitializeComponent();
        }

        List<string> GetMovies()
        {
            using (Cinema c=new Cinema())
            {
                if(c.Movie.Any())
                    return c.Movie.Select(m => m.Name + " " + m.Producer).ToList();
                return null;
            }
        }

        List<string> GetRooms()
        {
            using (Cinema c = new Cinema())
            {
                if (c.Room.Any())
                    return c.Room.Where(r=>r.Screening.All(s=>s.StatusId==3)).Select(r => r.Name).ToList();
                return null;
            }
        }

        void LoadRooms()
        {
            using (Cinema c = new Cinema())
            {
                if (GetRooms() != null)
                {
                    comboBox2.Items.Clear();
                    if (!GetRooms().Any())
                    {
                        MessageBox.Show("Поки що усі зали зайняті. Спробуйте пізніше.");
                        Close();
                    }
                    else
                        comboBox2.Items.AddRange(GetRooms().ToArray());
                }
            }
        }

        void LoadMovies()
        {
            if (GetMovies() != null)
            {
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(GetMovies().ToArray());
            }
        }

        private void AddScreeningForm_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            LoadRooms();
            LoadMovies();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex != -1)
            {
                using (Cinema c = new Cinema())
                {
                    if (c.Screening.Any())
                    {
                        date = c.Screening.ToList().Last(s => s.StatusId != 3).ScreeningStart;
                        date.AddMinutes(1);
                    }
                    else
                    {
                        date = DateTime.Now;
                    }
                    groupBox1.Text = $"Оберіть дату не раніше {date.ToString("dd.MM.yyyy HH:mm")}";
                }
            }
            else
            {
                MessageBox.Show("Спочатку оберіть зал!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (maskedTextBox1.MaskCompleted && comboBox1.SelectedIndex != -1)
            {
                DateTime screeningDate = dateTimePicker1.Value;
                if (Convert.ToInt32(maskedTextBox1.Text.Substring(3, 2)) < 60 &&
                    Convert.ToInt32(maskedTextBox1.Text.Substring(0, 2)) < 24)
                {
                    screeningDate = screeningDate.AddHours(-date.Hour);
                    screeningDate = screeningDate.AddMinutes(-date.Minute);
                    screeningDate = screeningDate.AddSeconds(-date.Second);
                    screeningDate =screeningDate.AddHours(Convert.ToInt32(maskedTextBox1.Text.Substring(0, 2)));
                    screeningDate = screeningDate.AddMinutes(Convert.ToInt32(maskedTextBox1.Text.Substring(3, 2)));
                    if (date<screeningDate)
                    {
                        using (Cinema c = new Cinema())
                        {
                            var roomId = c.Room.First(r => r.Name == comboBox2.SelectedItem.ToString()).Id;
                            var movieId = c.Movie.First(m => comboBox1.SelectedItem.ToString().Contains(m.Name) &&
                              comboBox1.SelectedItem.ToString().Contains(m.Producer)).Id;
                            c.Screening.Add(new Screening
                            {
                                RoomId = roomId,
                                MovieId = movieId,
                                ScreeningStart = screeningDate,
                                StatusId = 1,
                                
                            });
                            c.SaveChanges();
                            var seats = c.Seat.Where(s => s.RoomId == roomId);
                            var screening = c.Screening.ToList().Last();
                            foreach(var seat in seats.ToList())
                            {
                                c.SeatStatus.Add(new SeatStatus
                                {
                                    StatusId=1,
                                    SeatId=seat.Id,
                                    ScreeningId=screening.Id,
                                    Price=numericUpDown1.Value,
                                    phoneNumber=""
                                });
                                c.SaveChanges();
                             
                            }
                            MessageBox.Show("OK");
                            Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Оберіть дату не раніше {date}!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Введіть коректний час!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Оберіть фільм та введите правильний час!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
