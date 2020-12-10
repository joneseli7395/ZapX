using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZapX.Data;
using ZapX.Models;
using ZapX.Models.ViewModels;
using ZapX.Services;

namespace ZapX.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;
        private readonly IBTProjectService _projectService;
        private readonly UserManager<BTUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IBTRolesService rolesService, IBTProjectService projectService, UserManager<BTUser> userManager)
        {
            _logger = logger;
            _context = context;
            _rolesService = rolesService;
            _projectService = projectService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
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

        [AllowAnonymous]
        public IActionResult LandingPage()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
