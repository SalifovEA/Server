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
    public class User
    {
        int id;
        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        int position;
        [DataMember]
        public int Position
        {
            get { return position; }
            set { position = value; }
        }
        ICallbackBug callBack = null;
        [DataMember]
        public ICallbackBug CallBack
        {
            get { return callBack; }
            set { callBack = value; }
        }

        string name;
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        [DataMember]
        public int Color
        {
            set { _color = value; }
            get { return _color; }
        }
        private int _color;

        public User(string name, int id, ICallbackBug callBack)
        {
            this.Id = id;
            this.Position = position;
            this.CallBack = callBack;
            this.Name = name;
        }
    }
}
