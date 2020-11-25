using System;
using System.Collections.Generic;
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
    }
}
