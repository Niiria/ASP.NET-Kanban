using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Subject { get; set; }

        [DataType(DataType.DateTime), Required, Display(Name = "Start date")]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd}")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.DateTime), Required, Display(Name ="Due date")]
        [DisplayFormat(DataFormatString = "{yyyy-MM-ddThh:mm}")]
        public DateTime? DueDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{yyyy-MM-ddThh:mm}")]
        public DateTime? CompletedDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{yyyy-MM-ddThh:mm}")]
        public DateTime? CreatedDate { get; set; }


        [Required, ForeignKey(nameof(Status))]
        public int StatusId { get; set; }
        public virtual Status Status { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
