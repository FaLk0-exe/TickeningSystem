using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TickeningSystem
{
    public enum SeatsPreset
    {
        IMAX=43,
        CINETECH=15,
        DX=12
    }
    public static class RoomRepository
    {
        public static void LoadSeats(string roomName, SeatsPreset seatsPreset)
        {
            using (Cinema c = new Cinema())
            {
                if (c.Room.Any(r => r.Name == roomName))
                {
                    int columns=(int)seatsPreset, rows = 10, seats;
                    
                    var room = c.Room.First(r => r.Name == roomName);
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            c.Seat.Add(new Seat
                            {
                                Row = i,
                                Column = j,
                                RoomId = room.Id
                            });
                            c.SaveChanges();
                        }
                    }
                }
            }
        }
        public static bool ClearSeats(string roomName)
        {
            using (Cinema c = new Cinema())
            {
                var room = c.Room.First(r => r.Name == roomName).Id;
                if (c.Room.Any(r => r.Name == roomName) && c.Screening.All(s => s.RoomId !=
                  room))
                {
                    var deletedSeats = c.Seat.Where(s => s.Room.Name == roomName).ToList();
                    foreach (var s in deletedSeats)
                    {
                        c.Entry(s).State = System.Data.Entity.EntityState.Deleted;
                        c.SaveChanges();
                    }
                    return true;
                }
                return false;
            }
        }
    }
}
