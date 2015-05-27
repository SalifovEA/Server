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
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    class BugServer:IBug
    {
        List<Room> rooms = new List<Room>();
        private BugGameEntities _contextDB = new BugGameEntities();
        private static List<UserData> usersInGame = new List<UserData>();
        private Random _rand = new Random();

        void IBug.CreateRoom(int adminId = 0, string roomName = "No_name", string password = null)
        {
            rooms.Add(new Room(GetMap(), adminId, roomName, password));
        }

        public string EnterRoom(int playerId, string roomName, string password = null)
        {
            Room enterRoom = rooms.SingleOrDefault(a => a.RoomName == roomName);
            if (enterRoom == null)
            {
                MessageBox.Show("Method <EnterRoom> is ERROR!", "BugService");
                return null;
            }
            ICallbackBug enterUser = OperationContext.Current.GetCallbackChannel<ICallbackBug>();
            User player=new User(usersInGame.Single(a=>a.id==playerId).login,playerId,enterUser);
            enterRoom.Users.Add(player);
            foreach (User user in enterRoom.Users)
            {
                user.CallBack.UpdateUserList(enterRoom.Users.ToArray());
            }
            return enterRoom.RoomName;
        }
        /// <summary>
        /// Метод решает возможен ли такой ход
        /// </summary>
        /// <param name="id"> ид игрока сделавшего ввод</param> 
        /// <param name="currentRoom"> комната в каторой игрок играет</param>
        /// <param name="newPosition"> Позиция куда хочет отправиться игрок</param>
        /// <returns></returns>
        private int AcceptUserInput(int id, Room currentRoom, Keys key)
        {
            int width = 25; //ширина поля
            byte[] map = currentRoom.Map;
            try
            {
                int index = map.ToList().IndexOf(Convert.ToByte(id)); //текущее положение игрока 
                switch (key)
                {
                    case Keys.Up:
                        {
                            if ((index - width) >= 0 && map[index - width] != 1)
                            {
                                return index - width;
                            }
                        }
                        break;
                    case Keys.Down:
                        {
                            if ((index + width) < map.Length && map[index + width] != 1)
                            {
                                return index + width;
                            }
                        }
                        break;
                    case Keys.Left:
                        {
                            if (index % 25 != 0 && map[index - 1] != 1)
                            {
                                return index - 1;
                            }
                        }
                        break;
                    case Keys.Right:
                        {
                            if ((index % 25) % 24 != 0 && map[index + 1] != 1)      //Используется (index%25) % 24 != т.к index % 24 - будет справедливо только для первой строки. 
                            {                                                       //Для последующих строк, значения необходимо приводить используя %25
                                return index + 1;
                            }
                        }
                        break;
                    default: return -1;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "AcceptUserInput");
            }

            return -1;
        }

/// <summary>
/// Метод ставит игрока в новую позицию
/// </summary>
        /// <param name="id"> ид игрока сделавшего ввод</param>
        /// <param name="currentRoom"> комната в каторой игрок играет</param>
        /// <param name="newPosition">Позиция куда хочет отправиться игрок</param>
        private void MoveUserPosition(int id, Room currentRoom, int newPosition)
        {
            byte[] map = currentRoom.Map;
            map[newPosition] = (byte)currentRoom.Users.Single(a => a.Id == id).Color;
        }

        public void UserMove(int idUser, string roomName, Keys key)
        {
            Room myRoom = rooms.SingleOrDefault(a => a.RoomName == roomName);
             if (myRoom == null)
            {
                MessageBox.Show("Method <EnterRoom> is ERROR!", "BugService");
                return;
            }
             int newPosition = AcceptUserInput(idUser, myRoom, key);
            if (newPosition!=-1)
            {
                MoveUserPosition(idUser, myRoom, newPosition);
                foreach (User user in myRoom.Users)
                {
                    user.CallBack.UpdateMapOnClient(myRoom.Map);
                }
            }
        }
        public bool Registration(string login, string password)
        {
            if (_contextDB.UserData.FirstOrDefault(a => a.login == login) != null)
                return false;
            UserData user = new UserData { login = login, password = password, wins = 0, loses = 0 };
            _contextDB.UserData.Add(user);
            _contextDB.SaveChanges();
            return true;
        }

        public int Autorization(string login, string password)  
        {
            var request = from a in _contextDB.UserData
                          where a.login == login && a.password == password
                          select a;
            var requestarray = request.ToArray();

            //if (requestarray.Any())
            //{
            //    return 0;
            //}

            if (usersInGame.FirstOrDefault(a => a.login == login) == null)
            {
                usersInGame.Add(requestarray[0]);

                return requestarray[0].id;
            }

            return 0;
        }

        private byte[] GetMap()
        {
            byte[] currentMap = new byte[425];

            
            DirectoryInfo directoryInfo = new DirectoryInfo("../../Maps/");
            int countMaps = directoryInfo.GetFiles().Count();
            int numMap = _rand.Next(1, countMaps + 1);

            string resultString = File.ReadAllText("../../Maps/map_" + numMap + ".txt");

            for (int i = 0, j = 0; i < resultString.Length; i++)
            {
                if (resultString[i] != '\r' && resultString[i] != '\n')
                {
                    currentMap[j] = Convert.ToByte(resultString[i].ToString());
                    j++;
                }
            }

            return currentMap;
        }

        public PlayerStatistic GetPlayerStatistic(int idPlayer)
        {
            UserData userData = _contextDB.UserData.SingleOrDefault(a => a.id == idPlayer);

            if (userData != null)
            {
                return new PlayerStatistic { Wins = (int)userData.wins, Loses = (int)userData.loses };
            }

            return null;
        }

        public void StartGame(string roomName)
        {
            Room myRoom = rooms.SingleOrDefault(a => a.RoomName == roomName);
            User[] userslist = myRoom.Users.ToArray();
            byte[] map = myRoom.Map;
            int position = 0;//Не используется!!!Зачем?
            bool find = true;

            for (int i = 0; i < userslist.Length; i++)
            {
                userslist[i].Color = 51 + i;
                while (find)
                {
                    int pos = map[_rand.Next(0, map.Length)];
                    if (pos == 0)
                    {
                        userslist[i].Position = pos;
                        map[pos] = Convert.ToByte(userslist[i].Color);
                        find = false;
                    }
                }
                find = true;
            }

            foreach (var item in myRoom.Users)
            {
                try
                {
                    item.CallBack.UpdateMapOnClient(map);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "BugService");
                }
                
            }
        }
    }
   


   
}
