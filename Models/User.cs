using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BeltExam3.Models
{
    public class User : BaseEntity
    {
        [Key]
        public int UserId { get; set; }
        
        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        [Required]
        [MinLength(2)]
        public string Alias { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [NotMapped]
        [Compare("Password")]
        [MinLength(8)]
        public string ConfirmPassword { get; set; }

        [EmailAddress]
        [NotMapped]
        public string LoginEmail { get; set; }

        [DataType(DataType.Password)]
        [NotMapped]
        public string LoginPassword { get; set; }

        public DateTime created_at {get;set;}

        public DateTime updated_at {get;set;}
        public List<Post> Posts {get;set;}
        public List<Like> Likes {get;set;}

        public User(){
            Posts = new List<Post>();
            Likes = new List<Like>();
        }
    }
}