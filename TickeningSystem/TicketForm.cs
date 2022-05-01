using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastReport;

namespace TickeningSystem
{
    public partial class TicketForm : Form
    {
        int sRow=-2, sColumn=-2;
        Screening screening;

        public TicketForm(Screening screening)
        {
            this.screening = screening;
            InitializeComponent();
        }

        private void TicketForm_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ClearReservedSeats();
            FillDataGridView();
        }

        void ClearReservedSeats()
        {
            using (Cinema c = new Cinema())
            {
                TimeSpan timeSpan;
                var editedSeats = c.SeatStatus.Where(s => s.Status.Name == "Заброньовано" && s.ScreeningId == screening.Id);
                foreach (var s in editedSeats.ToList())
                {
                    timeSpan = DateTime.Now - s.Screening.ScreeningStart;
                    if (timeSpan.TotalMinutes <= 60)
                    {
                        s.phoneNumber = "";
                        s.StatusId = c.Status.First(st => st.Name == "Вільне").Id;
                        c.Entry(s).State = System.Data.Entity.EntityState.Modified;
                        c.SaveChanges();
                    }
                }
            }
        }
        void FillDataGridView()
        {
            maskedTextBox1.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            comboBox1.Text = "";
            comboBox1.SelectedIndex = -1;
            using (Cinema c = new Cinema())
            {
                var seats = c.SeatStatus.Where(s =>
                s.Seat.RoomId == screening.RoomId).Select(s => s).ToList();
                int columns, rows = 10;
                var selectedScreening = c.Screening.First(s => s.Id == screening.Id);
                switch (selectedScreening.Room.RoomType.Name)
                {
                    case "IMAX":
                        columns = (int)SeatsPreset.IMAX;
                        break;
                    case "DX":
                        columns = (int)SeatsPreset.DX;
                        break;
                    default:
                        columns = (int)SeatsPreset.CINETECH;
                        break;
                }
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.RowCount = rows;
                dataGridView1.ColumnCount = columns;
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        switch (seats.First(s => s.Seat.Row == i && s.Seat.Column == j).Status.Name)
                        {
                            case "Вільне":
                                dataGridView1[j, i].Style.BackColor = Color.Blue;
                                break;
                            case "Заброньовано":
                                dataGridView1[j, i].Style.BackColor = Color.Yellow;
                                break;
                            case "Куплено":
                                dataGridView1[j, i].Style.BackColor = Color.Red;
                                break;
                        }
                    }
                    dataGridView1.Rows[i].HeaderCell.Value = (i+1).ToString();
                }
                for (int i = 0; i < columns; i++)
                {
                    dataGridView1.Columns[i].HeaderText = (i + 1).ToString();
                }
            }
        }

        bool CheckCondition()
        {
            if (sRow == -2 && sColumn == -2)
            {
                MessageBox.Show("Оберіть комірку!");
                return false;
            }
            if(maskedTextBox1.Text.Length!=10)
            {
                MessageBox.Show("телефон повинен містити 10 символів!");
                return false;
            }
            if(!maskedTextBox1.MaskCompleted)
            {
                MessageBox.Show("Заповніть поле для моб. телефона!");
                return false;
            }
            if (dataGridView1[sColumn, sRow].Style.BackColor == Color.Red)
            {
                MessageBox.Show("Це місце вже куплено!");
                return false;
            }
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Оберіть тип операції!");
                return false;
            }
            return true;

        }

        void ReserveTicket()
        {
            using (Cinema c = new Cinema())
            {
                var seat = c.SeatStatus.First(s => s.ScreeningId == screening.Id && s.Seat.Row == sRow
                  && s.Seat.Column == sColumn);
                var status = c.Status.First(s => s.Name == "Заброньовано");
                seat.StatusId = status.Id;
                seat.phoneNumber = maskedTextBox1.Text;
                c.Entry(seat).State = System.Data.Entity.EntityState.Modified;
                c.SaveChanges();
            }
            FillDataGridView();
        }

        void BuyTicket()
        {
            using (Cinema c = new Cinema())
            {
                var seat = c.SeatStatus.First(s => s.ScreeningId == screening.Id && s.Seat.Row == sRow
                  && s.Seat.Column == sColumn);
                if (seat.Status.Name=="Вільне"||(seat.Status.Name == "Заброньовано" && maskedTextBox1.Text==seat.phoneNumber))
                {
                    var status = c.Status.First(s => s.Name == "Куплено");
                    seat.StatusId = status.Id;
                    seat.phoneNumber = maskedTextBox1.Text;
                    c.Entry(seat).State = System.Data.Entity.EntityState.Modified;
                    c.SaveChanges();
                    Report report = Report.FromFile("Ticket.frx");
                    report.SetParameterValue("row", sRow + 1);
                    report.SetParameterValue("column", sColumn + 1);
                    report.SetParameterValue("id", seat.Id);
                    report.SetParameterValue("room", seat.Screening.Room.Name);
                    report.SetParameterValue("movie", seat.Screening.Movie.Name);
                    report.SetParameterValue("start", seat.Screening.ScreeningStart);
                    report.SetParameterValue("duration", seat.Screening.Movie.Duration);
                    report.SetParameterValue("price", seat.Price);
                    report.Show();
                }
                else
                {
                    MessageBox.Show("Це місце не бронювали по цьому моб. номеру");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (CheckCondition())
            {
                if (dataGridView1[sColumn, sRow].Style.BackColor == Color.Blue)
                {
                    if(comboBox1.SelectedIndex==0)
                    {
                        ReserveTicket();
                        MessageBox.Show("OK!");
                    }
                    if(comboBox1.SelectedIndex==1)
                    {
                        BuyTicket();
                    }
                }
                else
                if(dataGridView1[sColumn, sRow].Style.BackColor == Color.Yellow)
                {
                    if (comboBox1.SelectedIndex == 1)
                    {
                        BuyTicket();
                    }
                    else
                    {
                        MessageBox.Show("Заброньоване місце неможливо забронювати!");
                    }
                }
                dataGridView1.ClearSelection();
                FillDataGridView();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ClearReservedSeats();
            FillDataGridView();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                sRow = e.RowIndex;
                sColumn = e.ColumnIndex;
                textBox1.Text = (sRow+1).ToString();
                textBox2.Text = (1+sColumn).ToString();
            }
        }
    }
}
