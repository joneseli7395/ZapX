using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZapX.Models;

namespace ZapX.Services
{
    public interface IBTHistoryService
    {
        Task AddHistory(Ticket oldTicket, Ticket newTicket, string userId);
    }
}
