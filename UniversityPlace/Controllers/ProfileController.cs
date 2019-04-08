using UniversityPlace.Models.Data;
using UniversityPlace.Models.ViewModels.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace UniversityPlace.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            return View();
        }

        // POST: Profile/LiveSearch
        [HttpPost]
        public JsonResult LiveSearch(string searchVal)
        {
            // Init db
            Db db = new Db();

            // Create list
            List<LiveSearchUserVM> usernames = db.Users.Where(x => x.Username.Contains(searchVal) && x.Username != User.Identity.Name).ToArray().Select(x => new LiveSearchUserVM(x)).ToList();

            // Return json
            return Json(usernames);
        }

        // POST: Profile/AddFriend
        [HttpPost]
        public void AddFriend(string friend)
        {
            // Init db
            Db db = new Db();

            // Get user's id
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            // Get friend to be id
            UserDTO userDTO2 = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendId = userDTO2.Id;

            // Add DTO

            FriendDTO friendDTO = new FriendDTO();

            friendDTO.User1 = userId;
            friendDTO.User2 = friendId;
            friendDTO.Active = false;

            db.Friends.Add(friendDTO);

            db.SaveChanges();
        }

        // POST: Profile/DisplayFriendRequests
        [HttpPost]
        public JsonResult DisplayFriendRequests()
        {
            // Init db
            Db db = new Db();

            // Get user id
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            // Create list of fr
            List<FriendRequestVM> list = db.Friends.Where(x => x.User2 == userId && x.Active == false).ToArray().Select(x => new FriendRequestVM(x)).ToList();

            // Init list of users

            List<UserDTO> users = new List<UserDTO>();

            foreach (var item in list)
            {
                var user = db.Users.Where(x => x.Id == item.User1).FirstOrDefault();
                users.Add(user);
            }

            // Return json
            return Json(users);
        }

        // POST: Profile/AcceptFriendRequest
        [HttpPost]
        public void AcceptFriendRequest(int friendId)
        {
            // Init db
            Db db = new Db();

            // Get user id
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            // Make friends

            FriendDTO friendDTO = db.Friends.Where(x => x.User1 == friendId && x.User2 == userId).FirstOrDefault();

            friendDTO.Active = true;

            db.SaveChanges();
        }

        // POST: Profile/DeclineFriendRequest
        [HttpPost]
        public void DeclineFriendRequest(int friendId)
        {
            // Init db
            Db db = new Db();

            // Get user id
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            // Delete friend request

            FriendDTO friendDTO = db.Friends.Where(x => x.User1 == friendId && x.User2 == userId).FirstOrDefault();

            db.Friends.Remove(friendDTO);

            db.SaveChanges();
        }


        // POST: Profile/SendMessage
        [HttpPost]
        public void SendMessage(string friend, string message)
        {
            // Init db
            Db db = new Db();

            // Get user id
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            // Get friend id
            UserDTO userDTO2 = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int userId2 = userDTO2.Id;

            // Save message

            MessageDTO dto = new MessageDTO
            {
                From = userId,
                To = userId2,
                Message = message,
                DateSent = DateTime.Now,
                Read = false
            };

            db.Messages.Add(dto);
            db.SaveChanges();
        }

        // POST: Profile/DisplayUnreadMessages
        [HttpPost]
        public JsonResult DisplayUnreadMessages()
        {
            // Init db
            Db db = new Db();

            // Get user id
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            // Create a list of unread messages
            List<MessageVM> list = db.Messages.Where(x => x.To == userId && x.Read == false).ToArray().Select(x => new MessageVM(x)).ToList();

            // Make unread read
            db.Messages.Where(x => x.To == userId && x.Read == false).ToList().ForEach(x => x.Read = true);
            db.SaveChanges();

            // Return json
            return Json(list);
        }


    }
}