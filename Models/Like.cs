using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam3.Models
{
    public class Like
    {
        [Key]
        public int LikeId { get; set; }
        
        public int PostId{ get; set; }

        [NotMapped]
        public Post PostLiked { get; set;}

        public int UserId { get; set; }

        [NotMapped]
        public User UserLiking {get; set;}

    }
}