using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;

namespace GameBUG
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IService1" в коде и файле конфигурации.
    [ServiceContract(CallbackContract=typeof(ICallbackBug))]
    public interface IBug
    {
        [OperationContract]
        void CreateRoom(int adminId = 0, string roomName = "No_name", string password = null);
        [OperationContract]
        string EnterRoom(int playerId, string roomName, string password = null);
        [OperationContract]
        bool Registration(string login, string password);
        [OperationContract]
        int Autorization(string login, string password);
        [OperationContract(IsOneWay = true)]
        void UserMove(int id, string roomName, Keys key);
        [OperationContract]
        void StartGame(string roomName);
    }
    public interface ICallbackBug
    {
        [OperationContract(IsOneWay = true)]
        void UpdateMapOnClient(byte[] map);
        [OperationContract(IsOneWay = true)]
        void UpdateUserList(User[]users);
    }
    
}
 