using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IEmailSender _emailSender;

        public BTHistoryService(ApplicationDbContext context, UserManager<BTUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
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
                        NewValue = newTicket.DeveloperUser.FullName,
                        Created = DateTimeOffset.Now,
                        UserId = userId
                    };
                    await _context.TicketHistories.AddAsync(history);

                    Notification notification = new Notification
                    {
                        TicketId = newTicket.Id,
                        Description = "You have a new ticket.",
                        Created = DateTimeOffset.Now,
                        SenderId = userId,
                        RecipientId = newTicket.DeveloperUserId
                    };
                    await _context.Notifications.AddAsync(notification);

                    //Send an email
                    string devEmail = newTicket.DeveloperUser.Email;
                    string subject = "New Ticket Assignment";
                    string message = $"You have a new ticket for project: {newTicket.Project.Name}";

                    await _emailSender.SendEmailAsync(devEmail, subject, message);
                }
                else if (String.IsNullOrWhiteSpace(newTicket.DeveloperUserId))
                {
                    TicketHistory history = new TicketHistory
                    {
                        TicketId = newTicket.Id,
                        Property = "Developer",
                        OldValue = oldTicket.DeveloperUser.FullName,
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
                        OldValue = oldTicket.DeveloperUser.FullName,
                        NewValue = newTicket.DeveloperUser.FullName,
                        Created = DateTimeOffset.Now,
                        UserId = userId
                    };
                    await _context.TicketHistories.AddAsync(history);

                    Notification notification = new Notification
                    {
                        TicketId = newTicket.Id,
                        Description = "You have a new ticket.",
                        Created = DateTimeOffset.Now,
                        SenderId = userId,
                        RecipientId = newTicket.DeveloperUserId
                    };
                    await _context.Notifications.AddAsync(notification);

                    //Send an email
                    string devEmail = newTicket.DeveloperUser.Email;
                    string subject = "New Ticket Assignment";
                    string message = $"You have a new ticket for project: {newTicket.Project.Name}";

                    await _emailSender.SendEmailAsync(devEmail, subject, message);
                }
            }

            if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "Priority",
                    OldValue = oldTicket.TicketPriority.Name,
                    NewValue = newTicket.TicketPriority.Name,
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
                    OldValue = oldTicket.TicketStatus.Name,
                    NewValue = newTicket.TicketStatus.Name,
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
                    OldValue = oldTicket.TicketType.Name,
                    NewValue = newTicket.TicketType.Name,
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
