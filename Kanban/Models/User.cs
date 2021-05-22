using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4)]
        public string Username { get; set; }
        [MinLength(2)]
        public string Firstname { get; set; }
        [Required, MinLength(4)]
        public string Password { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
    }
}
