using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGround.Abstract_Class_Example
{
    public class Hotel : HotelBase
    {
        public Hotel()
        {
            Rooms = new List<Room>();
        }

        public List<Room> Rooms { get; set; }
    }
}
