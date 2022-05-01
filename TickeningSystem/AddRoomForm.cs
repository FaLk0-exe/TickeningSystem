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
    public partial class AddRoomForm : Form
    {
        const int MAX_ROOMS_COUNT = 8;
        List<RoomType> roomTypes;
        int[] maxValues = new int[3] { 150, 120, 430};
        public AddRoomForm()
        {
            InitializeComponent();
        }

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

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                using (Cinema c = new Cinema())
                {
                    SeatsPreset seatsPreset;
                    string roomName;
                    var selectedRoomType = roomTypes.First(rt => rt.Name == comboBox1.SelectedItem.ToString());
                    if (c.Room.Any())
                    {
                        roomName = c.Room.ToList().Last().Name;
                        int numb = Convert.ToInt32(roomName.ElementAt(roomName.Length - 1).ToString()) + 1;
                        roomName = roomName.Remove(roomName.Length - 1, 1);
                        roomName += numb.ToString();
                    }
                    else
                    {
                        roomName = "Зал 1";
                    }
                    int seatsCount;
                    if (comboBox1.SelectedIndex == 0)
                    {
                        seatsCount = 150;
                        seatsPreset = SeatsPreset.CINETECH;
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
                            seatsPreset = SeatsPreset.IMAX;
                            seatsCount = 430;
                        }
                    }
                    c.Room.Add(new Room
                    {
                        Name = roomName,
                        SeatsCount =seatsCount,
                        RoomId = selectedRoomType.Id
                    });
                    c.SaveChanges();
                    if (RoomRepository.ClearSeats(roomName))
                    {
                        RoomRepository.LoadSeats(roomName, seatsPreset);
                        MessageBox.Show("OK!");
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Виникла помилка. Зверніться до розробника!");
                    }
                }
            }
        }

        private void AddRoomForm_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            using (Cinema c = new Cinema())
            {
                if (c.Room.Count() == MAX_ROOMS_COUNT)
                {
                    MessageBox.Show("Максимальна кількість залів - 8!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    Close();
                }
                else
                {
                    roomTypes = GetRoomTypes();
                    LoadRoomTypes();
                }

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
