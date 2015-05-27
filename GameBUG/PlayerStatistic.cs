using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameBUG
{
    [DataContract]
    class PlayerStatistic
    {
        [DataMember]
        public int Wins { get; set; }
        [DataMember]
        public int Loses { get; set; }
    }
}
