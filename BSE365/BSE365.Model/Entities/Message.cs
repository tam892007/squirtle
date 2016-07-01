using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSE365.Base.Infrastructures;
using BSE365.Model.Enum;

namespace BSE365.Model.Entities
{
    public class Message : IObjectState
    {
        public Message()
        {
            //Id = Guid.NewGuid().ToString();
            Type = MessageType.Message;
            State = MessageState.UnRead;
            Created = DateTime.Now;
        }

        public string Id { get; set; }

        public string FromAccount { get; set; }

        /*[Required]*/
        public string ToAccount { get; set; }

        public MessageType Type { get; set; }

        public MessageState State { get; set; }

        public string Messsage { get; set; }


        public DateTime Created { get; set; }


        [NotMapped]
        public ObjectState ObjectState { get; set; }
    }
}