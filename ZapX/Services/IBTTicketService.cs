using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZapX.Models;

namespace ZapX.Services
{
    public interface IBTTicketService
    {
        public Task AssignDeveloperTicket(string userId, int ticketId);
        public List<Ticket> ListDeveloperTickets(string userId);
        public List<Ticket> ListProjectManagerTickets(string userId);
        public List<Ticket> ListSubmitterTickets(string userId);
        public Task<List<Ticket>> ListProjectTickets(string userId);
    }
}
