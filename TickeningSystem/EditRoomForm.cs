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
    public partial class EditRoomForm : Form
    {
        const int MAX_ROOMS_COUNT = 8;
        List<RoomType> roomTypes;
        Room room;
        public void LoadRoomTypes()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(GetRoomTypes().Select(r => r.Name).ToArray());
        }
        public List<RoomType> GetRoomTypes()
        {
            using (Cinema c = new Cinema())
            {
                if (c.RoomType.Any())
                {
                    return c.RoomType.ToList();
                }
                return null;
            }
        }
        public EditRoomForm(string roomName)
        {
                InitializeComponent();
            using (Cinema c = new Cinema())
            {
                LoadRoomTypes();
                room = c.Room.First(r => r.Name == roomName);
            }
        }

        private void EditRoomForm_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            roomTypes = GetRoomTypes();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                using (Cinema c = new Cinema())
                {
                    SeatsPreset seatsPreset;
                    int seatsCount;
                    if (comboBox1.SelectedIndex == 2)
                    {
                        seatsPreset = SeatsPreset.IMAX;
                        seatsCount = 150;
                    }
                    else
                    {
                        if (comboBox1.SelectedIndex == 1)
                        {
                            seatsPreset = SeatsPreset.DX;
                            seatsCount = 120;
                        }
                        else
                        {
                            seatsPreset = SeatsPreset.CINETECH;
                            seatsCount = 430;
                        }
                    }
                    room.RoomId = roomTypes.First(r => r.Name == comboBox1.Text).Id;
                    room.SeatsCount = seatsCount;
                    c.Entry(room).State = System.Data.Entity.EntityState.Modified;
                    c.SaveChanges();
                    if(RoomRepository.ClearSeats(room.Name))
                    {
                        RoomRepository.LoadSeats(room.Name, seatsPreset);
                        MessageBox.Show("OK!");
                        Close();
                    }
                }
            }
        }
    }
}
