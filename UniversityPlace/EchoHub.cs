using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using UniversityPlace.Models.Data;

namespace UniversityPlace
{
    [HubName("echo")]
    public class EchoHub : Hub
    {
        public void Hello(string message)
        {
            //Clients.All.hello();
            Trace.WriteLine(message);

            // Set clients
            var clients = Clients.All;

            // Call js function
            clients.test("this is a test");
        }

        public void Notify(string friend)
        {
            // Init db
            Db db = new Db();

            // Get friend's id
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendId = userDTO.Id;

            // Get fr count
            var frCount = db.Friends.Count(x => x.User2 == friendId && x.Active == false);

            // Set clients
            var clients = Clients.Others;

            // Call js function
            clients.frnotify(friend, frCount);
        }

        public void GetFrcount()
        {
            // Init db
            Db db = new Db();

            // Get user id
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            // Get fr count
            var friendReqCount = db.Friends.Count(x => x.User2 == userId && x.Active == false);

            // Set clients
            var clients = Clients.Caller;

            // Call js function
            clients.frcount(Context.User.Identity.Name, friendReqCount);
        }

        public void GetFcount(int friendId)
        {
            // Init db
            Db db = new Db();

            // Get user id
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            // Get friend count for user
            var friendCount1 = db.Friends.Count(x => x.User2 == userId && x.Active == true || x.User1 == userId && x.Active == true);

            // Get user2 username
            UserDTO userDTO2 = db.Users.Where(x => x.Id == friendId).FirstOrDefault();
            string username = userDTO2.Username;

            // Get friend count for user2
            var friendCount2 = db.Friends.Count(x => x.User2 == friendId && x.Active == true || x.User1 == friendId && x.Active == true);

            // Update chat
            //UpdateChat();

            // Set clients
            var clients = Clients.All;

            // Call js function
            clients.fcount(Context.User.Identity.Name, username, friendCount1, friendCount2);

        }
    }
}