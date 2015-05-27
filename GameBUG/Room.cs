using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;

namespace GameBUG
{
    [DataContract]
    public class Room
    {
        byte[] map;
        [DataMember]
        public byte[] Map
        {
            get { return map; }
            set { map = value; }
        }

        List<User> users = new List<User>();
        [DataMember]
        public List<User> Users
        {
            get { return users; }
            set { users = value; }
        }
        int admin = 0;
        [DataMember]
        public int Admin
        {
            get { return admin; }
            set { admin = value; }
        }
        string password = null;
        [DataMember]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        string roomName = null;
        [DataMember]
        public string RoomName
        {
            get { return roomName; }
            set { roomName = value; }
        }

        public Room(byte[] map, int adminId = 0, string roomName = "No_name", string password = null)
        {
            this.Map = map;
            this.Users = new List<User>();
            this.Password = password;
            this.RoomName = roomName;
        }
        public void EnterRoom(User user, string password = null)
        {
            if (Password != null)
            {
                if (Password != password)
                    return;
            }
            Users.Add(user);
        }
    }
}
