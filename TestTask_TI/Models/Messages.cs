using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestTask_TI.Models
{
    public class Messages
    {
        public int Id { get; set; }
        public string Message { get; set; }       
        public string CreatorId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedTime { get; set; }
        public int Chats_Id { get; set; }
        //public Chats Chats { get; set; }



    }
}