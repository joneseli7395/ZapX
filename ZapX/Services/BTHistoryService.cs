using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZapX.Data;
using ZapX.Models;

namespace ZapX.Services
{
    public class BTHistoryService : IBTHistoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;

        public BTHistoryService(ApplicationDbContext context, UserManager<BTUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task AddHistory(Ticket oldTicket, Ticket newTicket, string userId)
        {
            if (oldTicket.Title != newTicket.Title)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "Title",
                    OldValue = oldTicket.Title,
                    NewValue = newTicket.Title,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }

            if (oldTicket.Description != newTicket.Description)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "Description",
                    OldValue = oldTicket.Description,
                    NewValue = newTicket.Description,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }
            if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
            {
                if (String.IsNullOrWhiteSpace(oldTicket.DeveloperUserId))
                {
                    TicketHistory history = new TicketHistory
                    {
                        TicketId = newTicket.Id,
                        Property = "Developer",
                        OldValue = "No Developer Assigned",
                        NewValue = _context.Users.Find(newTicket.DeveloperUserId).FullName,
                        Created = DateTimeOffset.Now,
                        UserId = userId
                    };
                    await _context.TicketHistories.AddAsync(history);
                }
                else if (String.IsNullOrWhiteSpace(newTicket.DeveloperUserId))
                {
                    TicketHistory history = new TicketHistory
                    {
                        TicketId = newTicket.Id,
                        Property = "Developer",
                        OldValue = _context.Users.Find(oldTicket.DeveloperUserId).FullName,
                        NewValue = "No Developer Assigned",
                        Created = DateTimeOffset.Now,
                        UserId = userId
                    };
                    await _context.TicketHistories.AddAsync(history);
                }
                else
                {
                    TicketHistory history = new TicketHistory
                    {
                        TicketId = newTicket.Id,
                        Property = "Developer",
                        OldValue = _context.Users.Find(oldTicket.DeveloperUserId).FullName,
                        NewValue = _context.Users.Find(newTicket.DeveloperUserId).FullName,
                        Created = DateTimeOffset.Now,
                        UserId = userId
                    };
                    await _context.TicketHistories.AddAsync(history);
                }
            }

            if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "Priority",
                    OldValue = _context.TicketPriorities.Find(oldTicket.TicketPriorityId).Name,
                    NewValue = _context.TicketPriorities.Find(newTicket.TicketPriorityId).Name,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }

            if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "Status",
                    OldValue = _context.TicketStatuses.Find(oldTicket.TicketStatusId).Name,
                    NewValue = _context.TicketStatuses.Find(newTicket.TicketStatusId).Name,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }

            if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "Type",
                    OldValue = _context.TicketTypes.Find(oldTicket.TicketTypeId).Name,
                    NewValue = _context.TicketTypes.Find(newTicket.TicketTypeId).Name,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }

            //if (oldTicket.Comments != newTicket.Comments)
            //{
            //    TicketHistory history = new TicketHistory
            //    {
            //        TicketId = newTicket.Id,
            //        Property = "Comments",
            //        OldValue = _context.TicketComments.Find(oldTicket.Comments).ToString(),
            //        NewValue = _context.TicketComments.Find(oldTicket.Comments).ToString(),
            //        Created = DateTimeOffset.Now,
            //        UserId = userId
            //    };
            //    await _context.TicketHistories.AddAsync(history);
            //}

            //if (oldTicket.Attachments != newTicket.Attachments)
            //{
            //    TicketHistory history = new TicketHistory
            //    {
            //        TicketId = newTicket.Id,
            //        Property = "Attachments",
            //        OldValue = _context.TicketAttachments.Find(oldTicket.Attachments).ToString(),
            //        NewValue = _context.TicketAttachments.Find(newTicket.Attachments).ToString(),
            //        Created = DateTimeOffset.Now,
            //        UserId = userId
            //    };
            //    await _context.TicketHistories.AddAsync(history);
            //}
            await _context.SaveChangesAsync();
        }
    }
}
