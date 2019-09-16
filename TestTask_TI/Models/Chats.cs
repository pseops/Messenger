using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestTask_TI.Models
{
    public class Chats
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }

        //public ICollection<Messages> Messages { get; set; }

        //public Chats()
        //{
        //    Messages = new List<Messages>();
        //}
    }
}