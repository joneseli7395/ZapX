using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZapX.Data;
using ZapX.Models;

namespace ZapX.Services
{
    public class BTTicketService : IBTTicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTProjectService _projectService;

        public BTTicketService(ApplicationDbContext context, UserManager<BTUser> userManager, IBTProjectService projectService)
        {
            _context = context;
            _userManager = userManager;
            _projectService = projectService;
        }

        public async Task AssignDeveloperTicket(string userId, int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            ticket.DeveloperUserId = userId;
            await _context.SaveChangesAsync();
        }

        public List<Ticket> ListDeveloperTickets(string userId)
        {
            List<Ticket> tickets = (List<Ticket>)_context.Tickets
                .Where(t => t.DeveloperUserId == userId)
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .ToList();

            return tickets;
        }

        public List<Ticket> ListProjectManagerTickets(string userId)
        {
            var projectIds = new List<int>();
            var userProjects = _context.ProjectUsers.Where(pu => pu.UserId == userId).ToList();
            var tickets = new List<Ticket>();
            _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType);
            foreach (var record in userProjects)
            {
                projectIds.Add(record.ProjectId);
            }

            foreach (var id in projectIds)
            {
                tickets.AddRange(_context.Tickets.Where(t => t.ProjectId == id)
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.Project)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .ToList()
                    .ToList());
            }
            return tickets;
        }

        public List<Ticket> ListSubmitterTickets(string userId)
        {
            List<Ticket> tickets = (List<Ticket>)_context.Tickets
                .Where(t => t.OwnerUserId == userId)
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .ToList();
            return tickets;
        }

        public async Task<List<Ticket>> ListProjectTickets(string userId)
        {
            List<Project> projects = (List<Project>)await _projectService.ListUserProjects(userId);
            List<Ticket> tickets = projects.SelectMany(t => t.Tickets).ToList();
            return tickets;
        }

    }
}
