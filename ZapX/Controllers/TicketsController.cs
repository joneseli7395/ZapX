﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ZapX.Data;
using ZapX.Models;
using ZapX.Models.ViewModels;
using ZapX.Services;
using ZapX.Utilities;

namespace ZapX.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTHistoryService _historyService;
        private readonly IBTAccessService _accessService;
        private readonly IBTTicketService _ticketService;
        private readonly IBTRolesService _rolesService;
        private readonly IBTProjectService _projectService;

        public TicketsController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTHistoryService historyService, IBTAccessService accessService, IBTTicketService ticketService, IBTRolesService rolesService, IBTProjectService projectService)
        {
            _context = context;
            _userManager = userManager;
            _historyService = historyService;
            _accessService = accessService;
            _ticketService = ticketService;
            _rolesService = rolesService;
            _projectService = projectService;
        }

        // GET: Tickets
        [Authorize]
        public async Task<IActionResult> Index(string userId)
        {
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name");
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name");

            var vm = new TicketProjectsViewModel();
            //UserId for arg for action
            var projectIds = new List<int>();
            var model = new List<Ticket>();
            var userProjects = _context.ProjectUsers.Where(pu => pu.UserId == userId).ToList();
            foreach (var record in userProjects)
            {
                projectIds.Add(_context.Projects.Find(record.ProjectId).Id);
            }
            foreach (var id in projectIds)
            {
                var tickets = _context.Tickets.Where(t => t.ProjectId == id).ToList();
                model.AddRange(tickets);
            }

            vm.Tickets = await _context.Tickets.Include(t => t.DeveloperUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).ToListAsync();
            vm.TicketPriorities = await _context.TicketPriorities.ToListAsync();
            vm.TicketStatuses = await _context.TicketStatuses.ToListAsync();
            vm.TicketTypes = await _context.TicketTypes.ToListAsync();
            return View(vm);
        }


        // GET: UserTickets
        public async Task<IActionResult> UserTickets()
        {
            var userId = _userManager.GetUserId(User); // Get the currently logged in user.

            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName");
            var projects = _context.Projects;
            var devProjects = await _projectService.ListUserProjects(userId);
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["ProjectId"] = new SelectList(projects, "Id", "Name");
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name");
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name");

            var vm = new TicketProjectsViewModel();
            //var userId = _userManager.GetUserId(User); // Get the currently logged in user.
            var myRole = await _rolesService.ListUserRoles(_context.Users.Find(userId));
            var test = myRole.FirstOrDefault();
            List<Ticket> model;
            switch (test)
            {
                case "Admin":
                    model = _context.Tickets
                        .Include(t => t.OwnerUser)
                        .Include(t => t.DeveloperUser)
                        .Include(t => t.TicketPriority)
                        .Include(t => t.TicketStatus)
                        .Include(t => t.TicketType)
                        .Include(t => t.Project)
                        .ToList();
                    break;
                // Snippet to get ticket for project manager - special case for roles
                case "ProjectManager":
                    var projectIds = new List<int>();
                    model = new List<Ticket>();
                    var userProjects = _context.ProjectUsers.Where(pu => pu.UserId == userId).ToList();
                    foreach (var record in userProjects)
                    {
                        projectIds.Add(_context.Projects.Find(record.ProjectId).Id);
                    }
                    foreach (var id in projectIds)
                    {
                        var tickets = _context.Tickets.Where(t => t.ProjectId == id)
                            .Include(t => t.OwnerUser)
                            .Include(t => t.DeveloperUser)
                            .Include(t => t.TicketPriority)
                            .Include(t => t.TicketStatus)
                            .Include(t => t.TicketType)
                            .Include(t => t.Project)
                            .ToList();
                        model.AddRange(tickets);
                    }
                    break;
                case "Developer":
                    model = _context.Tickets.Where(t => t.DeveloperUserId == userId)
                        .Include(t => t.OwnerUser)
                        .Include(t => t.TicketPriority)
                        .Include(t => t.TicketStatus)
                        .Include(t => t.TicketType)
                        .Include(t => t.Project)
                        .ToList();
                    ViewData["ProjectId"] = new SelectList(devProjects, "Id", "Name");

                    break;
                case "Submitter":
                    model = _context.Tickets.Where(t => t.OwnerUserId == userId)
                        .Include(t => t.DeveloperUser)
                        .Include(t => t.TicketPriority)
                        .Include(t => t.TicketStatus)
                        .Include(t => t.TicketType)
                        .Include(t => t.Project)
                        .ToList();
                    ViewData["ProjectId"] = new SelectList(devProjects, "Id", "Name");
                    break;
                default:
                    return RedirectToAction("Index", "Home");
            }
            vm.Tickets = await _context.Tickets.Include(t => t.DeveloperUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).ToListAsync();
            return View(vm);
        }

        // GET: Tickets/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            var roleName = (await _userManager.GetRolesAsync(await _userManager.GetUserAsync(User))).FirstOrDefault();
            if (await _accessService.CanInteractTicket(userId, (int)id, roleName))
            {
                var ticket = await _context.Tickets
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.Project)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Comments).ThenInclude(tc => tc.User)
                    .Include(t => t.Attachments)
                    .Include(t => t.Histories)
                    .FirstOrDefaultAsync(m => m.Id == id);

                var availableUsers = new List<BTUser>();
                if (User.IsInRole("ProjectManager"))
                {
                    availableUsers = (List<BTUser>)await _projectService.DevelopersOnProjectAsync(ticket.ProjectId);
                }
                else
                {
                    availableUsers = (List<BTUser>)await _projectService.UsersOnProject(ticket.ProjectId);
                }
                ViewData["DeveloperUserId"] = new SelectList(availableUsers, "Id", "FullName", ticket.DeveloperUserId);
                ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.OwnerUserId);
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
                ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
                ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
                ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);



                if (ticket == null)
                {
                    return NotFound();
                }

                return View(ticket);
            }
            TempData["InvalidAccess"] = "You have attempted to access a ticket outside of your authorization";
            return RedirectToAction("Index");
        }

        // GET: Tickets/Create
        public IActionResult Create(int? id)
        {
            var model = new Ticket();
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "FullName");
            if (id == null)
            {
                if (User.IsInRole("Admin"))
                {
                    ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
                }
                else
                {
                    var userId = _userManager.GetUserId(User);
                    var records = _context.ProjectUsers.Where(pu => pu.UserId == userId).Select(pu => pu.Project).ToList();
                    ViewData["ProjectId"] = new SelectList(records, "Id", "Name");
                }
            }
            else
            {
                model.ProjectId = (int)id;
            }
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name");
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name");
            return View(model);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,ProjectId,Description,TicketTypeId,TicketPriorityId,TicketStatusId,DeveloperUserId")] Ticket ticket, int? id)
        {
            if (!User.IsInRole("Demo"))
            {
                if (ModelState.IsValid)
                {
                    if (id != null)
                    {
                        ticket.ProjectId = id.Value;
                    }
                    ticket.Created = DateTime.Now;
                    if (ticket.TicketPriorityId == 0 || ticket.TicketStatusId == 0)
                    {
                        ticket.TicketPriorityId = _context.TicketPriorities.Where(tp => tp.Name == "Low").FirstOrDefault().Id;
                        ticket.TicketStatusId = _context.TicketStatuses.Where(tp => tp.Name == "Unassigned").FirstOrDefault().Id;
                    }
                    if (ticket.OwnerUserId == null)
                    {
                        ticket.OwnerUserId = _userManager.GetUserId(User);
                    }
                    _context.Add(ticket);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Projects", new { Id = ticket.ProjectId });
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                TempData["DemoLockout"] = "Your changes have not been saved. To make changes to the database you will need to log in as a full user.";
                return RedirectToAction("Details", "Projects", new { id = ticket.ProjectId });
            }
            //ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Name", ticket.DeveloperUserId);
            //ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Name", ticket.OwnerUserId);
            //ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            //ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            //ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            //ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            //return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var roleName = (await _userManager.GetRolesAsync(await _userManager.GetUserAsync(User))).FirstOrDefault(r => r != "Demo");

            if (await _accessService.CanInteractTicket(userId, (int)id, roleName))
            {
                var ticket = await _context.Tickets.FindAsync(id);
                if (ticket == null)
                {
                    return NotFound();
                }
                var availableUsers = new List<BTUser>();
                if (User.IsInRole("ProjectManager"))
                {
                    availableUsers = (List<BTUser>)await _projectService.DevelopersOnProjectAsync(ticket.ProjectId);
                }
                else
                {
                    availableUsers = (List<BTUser>)await _projectService.UsersOnProject(ticket.ProjectId);
                }
                ViewData["DeveloperUserId"] = new SelectList(availableUsers, "Id", "FullName", ticket.DeveloperUserId);
                ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.OwnerUserId);
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
                ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
                ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
                ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);

                return View(ticket);
            }

            TempData["InvalidAccess"] = "You have attempted to access a ticket outside of your authorization";
            return RedirectToAction("Index");
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket)
        {
            if (!User.IsInRole("Demo"))
            {
                if (id != ticket.Id)
                {
                    return NotFound();
                }

                Ticket oldTicket = await _context.Tickets
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                    .Include(t => t.DeveloperUser)
                    .AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticket.Id);

                if (ModelState.IsValid)
                {
                    try
                    {
                        ticket.Updated = DateTime.Now;
                        _context.Update(ticket);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TicketExists(ticket.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    //Add history
                    string userId = _userManager.GetUserId(User);
                    Ticket newTicket = await _context.Tickets
                        .Include(t => t.TicketPriority)
                        .Include(t => t.TicketStatus)
                        .Include(t => t.TicketType)
                        .Include(t => t.Project)
                        .Include(t => t.DeveloperUser)
                        .AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticket.Id);

                    await _historyService.AddHistory(oldTicket, newTicket, userId);

                    return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                TempData["DemoLockout"] = "Your changes have not been saved. To make changes to the database you will need to log in as a full user.";
                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
            }
            //ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.DeveloperUserId);
            //ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.OwnerUserId);
            //ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            //ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            //ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            //ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.Comments).ThenInclude(t => t.User)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            var projectId = ticket.ProjectId;
            if (!User.IsInRole("Demo"))
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Projects", new { id = projectId });
            }
            else
            {
                TempData["DemoLockout"] = "Your changes have not been saved. To make changes to the database you will need to log in as a full user.";
                return RedirectToAction("Details", "Projects", new { id = projectId });
            }
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}

//Create post bind
//public async Task<IActionResult> Create([Bind("Id,Title,Description,Updated,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket)
