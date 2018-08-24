using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BeltExam3.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BeltExam3.Controllers
{
    public class HomeController : Controller
    {

        private UsersContext _context;
		public HomeController(UsersContext context)
		{
			_context = context;
		}
        public IActionResult Index()
        {
            return View();
        }
        // Views of pages
        [HttpGet("~/UserDashboard")]
        public IActionResult UserDashboard()
        {
            if(isLoggedIn() == "false"){
                return View("Index");
            }
            int thisUserId = (int)HttpContext.Session.GetInt32("UserId");
            User thisUser = _context.Users.SingleOrDefault(thisId => thisId.UserId == thisUserId);
            List<Post> allPosts = _context.Posts.OrderByDescending(u => u.NumLikes).ToList();
            List<Like> allLikes = _context.Likes.ToList();
            List<User> allUsers = _context.Users.ToList();
            ViewBag.ThisUser = thisUser;
            ViewBag.AllUsers = allUsers;
            ViewBag.AllPosts = allPosts;
            ViewBag.AllLikes = allLikes;
            return View("UserDashboard");
        }
        [HttpGet("~/ViewPost/{postId}")]
        public IActionResult ViewPost(int postId)
        {
            if(isLoggedIn() == "false"){
                return View("Index");
            }
            Post thisPost = _context.Posts.SingleOrDefault(u => u.PostId == postId);
            User Creator = _context.Users.SingleOrDefault(u => u.UserId == thisPost.UserId);
            List<Like> allLikesOnThisPost = _context.Likes.Where(u => u.PostId == thisPost.PostId).ToList();
            List<User> likingUsers = new List<User>();
            foreach(Like thisLike in allLikesOnThisPost){
                User passUser = _context.Users.SingleOrDefault(u => u.UserId == thisLike.UserId);
                likingUsers.Add(passUser);
            }
            ViewBag.LikingUsers = likingUsers;
            ViewBag.ThisPost = thisPost;
            ViewBag.Creator = Creator;
            return View("ViewPost");
        }
        [HttpGet("~/ViewUser/{UserId}")]
        [HttpGet("~/ViewPost/ViewUser/{UserId}")]
        public IActionResult ViewUser(int UserId)
        {
            if(isLoggedIn() == "false"){
                return View("Index");
            }
            User Creator = _context.Users.SingleOrDefault(u => u.UserId == UserId);
            List<Like> allLikesFrom = _context.Likes.Where(u => u.UserId == UserId).ToList();
            List<Post> allPostsFrom = _context.Posts.Where(u => u.UserId == UserId).ToList();
            ViewBag.Likes = allLikesFrom;
            ViewBag.ThisUser = Creator;
            ViewBag.Posts = allPostsFrom;
            ViewBag.LikedMyPosts = allPostsFrom.Sum(u => u.NumLikes);
            return View("ViewUser");
        }


        // Functions of adding posts and likes
        [HttpPost("~/CreatePost")]
        public IActionResult CreatePost(Post checkPost)
        {
            if(ModelState.IsValid){
                Post newPost = new Post {
                    Content = @checkPost.Content,
                    UserId = (int)HttpContext.Session.GetInt32("UserId"),
                    NumLikes = 0,
                };
                _context.Posts.Add(newPost);
                _context.SaveChanges();
                Post LastPost = _context.Posts.Last();
                Console.WriteLine(LastPost.Content + "was added to the database");
                Console.WriteLine(newPost.Content);
                Console.WriteLine(newPost.Content);
                Console.WriteLine(newPost.UserId);
            }
            return RedirectToAction("UserDashboard");
        }
        [HttpGet("~/DeletePost/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            Post toBeDeleted = _context.Posts.SingleOrDefault(po => po.PostId == postId);
            List<Like> likesOnPost = _context.Likes.Where(u => u.PostId == toBeDeleted.PostId).ToList();
            foreach(Like thisLike in likesOnPost){
                _context.Likes.Remove(thisLike);
            }
            _context.Posts.Remove(toBeDeleted);
            _context.SaveChanges();
            return RedirectToAction("UserDashboard");
        }
        [HttpGet("~/LikePost/{postId}")]
        public IActionResult LikePost(int postId)
        {
            Post likedPost = _context.Posts.SingleOrDefault(po => po.PostId == postId);
            Like newLike = new Like{
                PostId = postId,
                UserId = (int)HttpContext.Session.GetInt32("UserId"),
            };
            likedPost.NumLikes += 1;
            likedPost.Likes.Add(newLike);
            _context.Update(likedPost);
            _context.Likes.Add(newLike);
            _context.SaveChanges();
            return RedirectToAction("UserDashboard");
        }
        [HttpGet("~/UnLikePost/{postId}")]
        public IActionResult UnLikePost(int postId)
        {
            Post likedPost = _context.Posts.SingleOrDefault(po => po.PostId == postId);
            Like thisLike = _context.Likes.Where(po => po.PostId == postId).SingleOrDefault(myId => myId.UserId == HttpContext.Session.GetInt32("UserId"));
            _context.Likes.Remove(thisLike);
            likedPost.NumLikes -= 1;
            _context.Update(likedPost);
            _context.SaveChanges();
            return RedirectToAction("UserDashboard");
        }


        // Login, Registration and logout
        [HttpPost("~/Registration")]
        public IActionResult Registration(User checkUser)
        {
            if(ModelState.IsValid){
                User EmailCheck = _context.Users.SingleOrDefault( a=>a.Email == @checkUser.Email );
                if(EmailCheck != null)
                {
                    ViewBag.EmailError = "Email has already been registered.";
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                checkUser.Password = Hasher.HashPassword(checkUser, checkUser.Password);
                DateTime nowTime = DateTime.Now;
                User newUser = new User
                {
                    Name = @checkUser.Name,
                    Alias = @checkUser.Alias,
                    Email = @checkUser.Email,
                    Password = @checkUser.Password,
                    created_at = nowTime,
                    updated_at = nowTime,
                };
                _context.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                HttpContext.Session.SetString("UserName", newUser.Name);
                return RedirectToAction("UserDashboard");
            } else {
                return RedirectToAction("Index");
            }
        }
        [HttpPost("~/Login")]
        public IActionResult Login(User checkUser)
        {
            User thisUser = _context.Users.SingleOrDefault( a=>a.Email == @checkUser.LoginEmail ); 
            if(thisUser != null && @checkUser.LoginPassword != null)
            {
                var Hasher = new PasswordHasher<User>();
                if(0 != Hasher.VerifyHashedPassword(thisUser, thisUser.Password, @checkUser.LoginPassword))
                {
                    HttpContext.Session.SetString("UserName", thisUser.Name);
                    HttpContext.Session.SetInt32("UserId", thisUser.UserId);
                    return RedirectToAction("UserDashboard");
                }
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("~/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public string isLoggedIn(){
            if(HttpContext.Session.GetInt32("UserId") == 0 || HttpContext.Session.GetInt32("UserId") == null) {
                return "false";
            } else {
                return "true";
            }
        }
    }
}
