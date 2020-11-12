using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZapX.Data;

namespace ZapX.Services
{
    public class BTAccessService : IBTAccessService
    {
        private readonly ApplicationDbContext _context;

        public BTAccessService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationDbContext Context { get; }

        public async Task<bool> CanInteractProject(string userId, int projectId, string roleName)
        {
            switch(roleName)
            {
                case "Admin":
                    return true;
                case "Project Manager":
                    if(await _context.ProjectUsers.Where(pu => pu.UserId == userId && pu.ProjectId == projectId).AnyAsync())
                    {
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }


        public async Task<bool> CanInteractTicket(string userId, int ticketId, string roleName)
        {
            bool result = false;
            switch (roleName)
            {
                case "Admin":
                    result = true;
                    break;
                case "Project Manager":
                    var projectId = (await _context.Tickets.FindAsync(ticketId)).ProjectId;

                    if (await _context.ProjectUsers.Where(pu => pu.UserId == userId && pu.ProjectId == projectId).AnyAsync())
                    {
                        result = true;
                    }
                    break;
                case "Developer":
                    if(await _context.Tickets.Where(t => t.DeveloperUserId == userId && t.Id == ticketId).AnyAsync())
                    {
                        result = true;
                    }
                    break;
                case "Submitter":
                    if (await _context.Tickets.Where(t => t.OwnerUserId == userId && t.Id == ticketId).AnyAsync())
                    {
                        result = true;
                    }
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
