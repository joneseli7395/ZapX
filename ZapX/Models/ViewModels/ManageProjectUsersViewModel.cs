using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZapX.Models.ViewModels
{
    public class ManageProjectUsersViewModel
    {
        public Project Project { get; set; }
        public MultiSelectList Users { get; set; }
        public string[] SelectedUsers { get; set; }
        public List<BTUser> UsersOnProject { get; set; }
        public List<BTUser> UsersNotOnProject { get; set; }
    }
}
