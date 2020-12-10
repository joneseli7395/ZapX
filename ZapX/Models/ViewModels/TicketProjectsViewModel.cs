using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ZapX.Data;

namespace ZapX.Models.ViewModels
{
    public class TicketProjectsViewModel
    {
        public Project Project { get; set; }
        public List<Project> Projects { get; set; }
        public Ticket Ticket { get; set; }
        public List<Ticket> Tickets { get; set; }
        public int ProjectId { get; set; }
        public List<TicketPriority> TicketPriorities { get; set; }
        public List<TicketStatus> TicketStatuses { get; set; }
        public List<TicketType> TicketTypes { get; set; }
        public List<ProjectUser> ProjectUsers { get; set; }

        [Display(Name = "Type")]
        public TicketType TicketType { get; set; }
        [Display(Name = "Priority")]
        public TicketPriority TicketPriority { get; set; }
        [Display(Name = "Status")]
        public TicketStatus TicketStatus { get; set; }
        [Display(Name = "Owner")]
        public BTUser OwnerUser { get; set; }
        [Display(Name = "Developer")]
        public BTUser DeveloperUser { get; set; }
    }
}
