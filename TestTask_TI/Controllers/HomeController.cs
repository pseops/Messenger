using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestTask_TI.Models;
using System.Data.Entity;
using System.IO;
using System.Data.Entity.Migrations;
using TestTask_TI.Hubs;
using Microsoft.AspNet.SignalR;

namespace TestTask_TI.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();        
        
        [System.Web.Mvc.Authorize]
        public ActionResult Index()
        {
            //var messages = db.Messages.Include(p => p.Chats);
            List<ApplicationUser> users = new List<ApplicationUser>();
            ApplicationUser user = new ApplicationUser();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                users = db.Users.ToList();
                user = db.Users.First();
                ViewBag.Users = users;
                ViewBag.FirstUser = user;
            }

            return View();
        }
        public void SingR(int id)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            Chats ch = new Chats();
            ch = db.Chats.Where(c => c.Id == id).FirstOrDefault();
            //hubContext.Clients.Client(user.Id).addMessage(msg);
            var per1 = ChatHub.Users.Where(u => u.Name == ch.SenderId).LastOrDefault();
            var per2 = ChatHub.Users.Where(u => u.Name == ch.ReceiverId).LastOrDefault();
            if (per1 != null)
                hubContext.Clients.Client(per1.ConnectionId).addMessage(id);
            if (per2 != null)
                hubContext.Clients.Client(per2.ConnectionId).addMessage(id);
        }
        [HttpPost]
        [System.Web.Mvc.Authorize]
        public ActionResult SendMessage(int id, string msg, string userId)
        {            
            try
            {                                
                ApplicationUser user = new ApplicationUser();                
                user = db.Users.Where(u=>u.Id==userId).FirstOrDefault();                
                Messages ms = new Messages {Chats_Id=id, Message = msg, CreatorId = userId, Name=user.UserName, CreatedTime=DateTime.UtcNow };
                           
                ViewBag.Ms = ms;
                db.Messages.Add(ms);
                db.SaveChanges();
                
                SingR(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new HttpStatusCodeResult(200);
            //return  View("Index"); 
        }
        [HttpGet]
        [System.Web.Mvc.Authorize]
        public ActionResult GetChat(string senderId, string receiverId)
        {
            Chats ch = new Chats();
            ch = db.Chats.Where(c => c.ReceiverId == receiverId && c.SenderId == senderId).FirstOrDefault();
            if (ch == null)
            {
                ch = db.Chats.Where(c => c.ReceiverId == senderId && c.SenderId == receiverId).FirstOrDefault();
                if(ch == null)
                {
                    db.Chats.Add(new Chats { ReceiverId = receiverId, SenderId = senderId });
                    db.SaveChanges();
                    var chat= db.Chats.Where(c => c.ReceiverId == receiverId && c.SenderId == senderId).Select(c=> new {
                        ChatId= c.Id,                        
                    }).FirstOrDefault();
                    return Json(chat, JsonRequestBehavior.AllowGet);
                }
                
            }
            
            var msg = db.Messages.Where(m => m.Chats_Id == ch.Id).Select(m => new
            {
                Id = m.Id,
                Message = m.Message,
                CreatorId = m.CreatorId,
                Name = m.Name,
                CreatedTime = m.CreatedTime,
                Chats_Id = m.Chats_Id,
                Permision = DbFunctions.DiffMinutes(m.CreatedTime, DateTime.UtcNow) < 15,
                
            }).ToList();

            Chats cht = new Chats();

            cht = db.Chats.Where(c => c.ReceiverId == receiverId && c.SenderId == senderId).FirstOrDefault();
            if (ch != null)
            {
                cht = ch;
            }
            var chat2 = new
            {
                ChatId = cht.Id,
                Messages = msg
            };
            //var chat2 = db.Chats.Where(c => c.ReceiverId == receiverId && c.SenderId == senderId).Select(c => new {
            //    ChatId = c.Id,
            //    Messages = msg,
            //}).ToList();

            return Json(chat2, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Mvc.Authorize]
        public ActionResult GetUsers()
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            using (ApplicationDbContext db =new ApplicationDbContext())
            {
                users = db.Users.ToList();
                ViewBag.Users = users;
            }
            return View(users);
        }
        [System.Web.Mvc.Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [System.Web.Mvc.Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        [System.Web.Mvc.Authorize]
        public ActionResult Index(HttpPostedFileBase file)
        {
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                // extract only the filename
                var fileName = Path.GetFileName(file.FileName);
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/files"), fileName);
                file.SaveAs(path);
                return Content("/files/"+fileName);
            }
            // redirect back to the index action to show the form once again
            return new HttpStatusCodeResult(400);
        }
        [System.Web.Mvc.Authorize]
        [HttpGet]
        public ActionResult DeleteMessage(int id)
        {
            Messages msg = new Messages();
            msg = db.Messages.Where(m => m.Id == id).FirstOrDefault();            
            var ch = msg.Chats_Id;
            db.Messages.Remove(msg);
            db.SaveChanges();
            SingR(ch);
            return new HttpStatusCodeResult(200);
        }
        [HttpPost]
        [System.Web.Mvc.Authorize]
        public ActionResult EditMessage(int id, string msg)
        {
            try
            {
                Messages ms = new Messages();
                ms = db.Messages.Where(m => m.Id == id).FirstOrDefault();
                ms.Message = msg;
                var ch = ms.Chats_Id;
                db.Messages.AddOrUpdate(ms);
                db.SaveChanges();
                SingR(ch);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new HttpStatusCodeResult(200);
            //return  View("Index"); 
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}