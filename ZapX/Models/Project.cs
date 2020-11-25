using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ZapX.Extensions;

namespace ZapX.Models
{
    public class Project
    {
        public Project()
        {
            Tickets = new HashSet<Ticket>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Please give the project an appropriate name")]
        [StringLength(50)]
        [Display(Name = "Project Name")]
        public string Name { get; set; }

        [Display(Name = "Project Image")]
        public string ImagePath { get; set; }

        [Display(Name = "Upload Image")]
        [MaxFileSize(2000)]
        [AllowedExtensions(new string[] {".jpg", ".png"})]
        public byte[] ImageData { get; set; }

        public List<ProjectUser> ProjectUsers { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }

        public static implicit operator MultiSelectList(Project v)
        {
            throw new NotImplementedException();
        }
    }
}
