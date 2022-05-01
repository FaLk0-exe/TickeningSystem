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
    public partial class RoomForm : Form
    {
        List<dynamic> rooms;
        public RoomForm(string role)
        {
            InitializeComponent();
            if (role == "a")
                button1.Visible = true;
        }


        List<dynamic> GetRooms()
        {
            using (Cinema c = new Cinema())
            {
                if (c.Room.Any())
                {
                    return c.Room.Select(m => new
                    {
                        Name = m.Name,
                        SeatsCount = m.SeatsCount,
                        RoomName = m.RoomType.Name
                    }).ToList<dynamic>();
                }
                return null;
            }
        }

        List<string> GetRoomTypes()
        {
            using (Cinema c = new Cinema())
            {
                if (c.RoomType.Any())
                    return c.RoomType.Select(m => m.Name).ToList();
                return null;
            }
        }

        void LoadRoomTypes()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(GetRoomTypes().ToArray());
        }

        public void LoadRooms()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.ColumnCount = 3;
            if (GetRooms() != null)
            {
                foreach (var r in rooms)
                {
                    dataGridView1.Rows.Add(r.Name, r.SeatsCount, r.RoomName);
                }
                for (int i = 0; i < 2; i++)
                    dataGridView1.Columns.Add(new DataGridViewButtonColumn());
                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    dataGridView1.Rows[i].Cells[3].Value = "Видалити";
                    dataGridView1.Rows[i].Cells[4].Value = "Редагувати";
                }
                rooms = GetRooms();
            }
        }

        private void RoomForm_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            rooms = GetRooms();
            LoadRooms();
            LoadRoomTypes();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rooms != null)
            {
                rooms = rooms.Where(r => r.RoomName == comboBox1.SelectedItem.ToString()).ToList<dynamic>();
                LoadRooms();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (rooms != null) LoadRooms();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (rooms.Count != 0 && e.RowIndex != -1)
            {
              
                if (e.ColumnIndex == 3)
                {
                    DialogResult dialogResult = MessageBox.Show("Ви впевнені, що хочете видалити цей зал?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        using (Cinema c = new Cinema())
                        {
                            var value = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                            var id = c.Room.First(r => r.Name ==
                               value).Id;
                            if (!c.Screening.Any(s => s.RoomId ==id))
                            {
                                var val = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                                var deletedRoom = c.Room.First(r => r.Name ==val
                               );
                                RoomRepository.ClearSeats(deletedRoom.Name);
                                c.Entry(deletedRoom).State = System.Data.Entity.EntityState.Deleted;
                                c.SaveChanges();
                                MessageBox.Show("OK");
                                GetRooms();
                                LoadRooms();
                            }
                            else
                            {
                                MessageBox.Show("Дочекайтеся закінчення поточних сеансів");
                            }
                        }
                    }
                }
                if (e.ColumnIndex == 4)
                {
                    EditRoomForm f = new EditRoomForm(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                    f.ShowDialog();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddRoomForm f = new AddRoomForm();
            f.ShowDialog();
        }

        private void RoomForm_Activated(object sender, EventArgs e)
        {
            rooms = GetRooms();
            LoadRooms();
            LoadRoomTypes();
        }
    }
}
