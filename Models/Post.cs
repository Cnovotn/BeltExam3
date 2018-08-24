using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam3.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        
        [Required]
        [MinLength(2)]
        public string Content { get; set; }

        public int UserId { get; set; }
        public int NumLikes { get; set; }

        [NotMapped]
        public User UserLiking {get; set; }
        [NotMapped]
        public List<Like> Likes {get; set;}
        public Post(){
            Likes = new List<Like>();
        }
    }
}