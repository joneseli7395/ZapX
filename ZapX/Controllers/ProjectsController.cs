using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZapX.Data;
using ZapX.Models;
using ZapX.Models.ViewModels;
using ZapX.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ZapX.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _projectService;
        private readonly UserManager<BTUser> _userManager;

        public ProjectsController(ApplicationDbContext context, IBTProjectService projectService, UserManager<BTUser> userManager)
        {
            _context = context;
            _projectService = projectService;
            _userManager = userManager;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var vm = new TicketProjectsViewModel();

            vm.Projects = await _context.Projects
                .Include(p => p.ProjectUsers)
                .ThenInclude(p => p.User)
                .ToListAsync();

            return View(vm);
        }

        //GET: Projects/ MyProjects 
        public async Task<IActionResult> MyProjects(int? id)
        {   // Should function similarly to MyTickets, but able to use service to filter projects seen based on user's role or if 
            // the user submitted a ticket for that project.

            var model = new List<Project>();
            var userId = _userManager.GetUserId(User);

            if (User.IsInRole("Admin"))
            {
                //create or use a service (BTProjectService?) that will filter a user's projects on the MyProjects View
                model = _context.Projects.ToList();
                return View("MyProjects", model);
            }
            if (User.IsInRole("ProjectManager") || User.IsInRole("Developer") ||
                User.IsInRole("Submitter") || User.IsInRole("NewUser"))
            {

                model = await _projectService.ListUserProjects(userId);

                return View("MyProjects", model);

            }

            return NotFound();

        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var vm = new TicketProjectsViewModel();


            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name");
            //ViewData["ProjectId"] = id.Value;
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name");


            var project = await _context.Projects
                .Include(p => p.ProjectUsers)//Reference ProjectUsers
                .ThenInclude(p => p.User) //Reference Users
                .FirstOrDefaultAsync(m => m.Id == id); //go to db, into projects table, find first with this Id. Only info for that project

            var tickets = await _context.Tickets
                .Where(t => t.ProjectId == id)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketType)
                .Include(t => t.DeveloperUser)
                .ToListAsync();

            vm.Project = project;
            vm.Tickets = tickets;
            vm.ProjectId = id.Value;


            if (project == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Project project, IFormFile image)
        {
            if (!User.IsInRole("Demo"))
            {
                if (ModelState.IsValid)
                {
                    var memoryStream = new MemoryStream();
                    image.CopyTo(memoryStream);
                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();
                    memoryStream.Dispose();
                    var binary = Convert.ToBase64String(bytes);
                    var ext = Path.GetExtension(image.FileName);

                    project.ImagePath = $"data:image/{ext};base64,{binary}";
                    project.ImageData = bytes;

                    _context.Add(project);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Projects");
                }
                return View(project);
            }
            else
            {
                TempData["DemoLockout"] = "Your changes have not been saved. To make changes to the database you will need to log in as a full user.";
                return RedirectToAction("Index", "Projects");
            }
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImagePath,ImageData")] Project project)
        {
            if (!User.IsInRole("Demo"))
            {
                if (id != project.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(project);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProjectExists(project.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(project);
            }
            else
            {
                TempData["DemoLockout"] = "Your changes have not been saved. To make changes to the database you will need to log in as a full user.";
                return RedirectToAction("Details", "Projects", new { id = project.Id });
            }
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (!User.IsInRole("Demo"))
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["DemoLockout"] = "Your changes have not been saved. To make changes to the database you will need to log in as a full user.";
                return RedirectToAction("Details", "Projects", new { id = project.Id });
            }
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }

        //AddUser Get
        [Authorize(Roles = "Admin, Project Manager")]
        public async Task<IActionResult> AssignUsers(int id)
        {
            var model = new ManageProjectUsersViewModel();
            var project = _context.Projects.Find(id);

            model.Project = project;
            List<BTUser> users = (List<BTUser>)await _context.Users.ToListAsync();
            List<BTUser> members = (List<BTUser>)await _projectService.UsersOnProject(id);
            //var users = await _context.Users.Where(u => _projectService.IsUserOnProject(u.Id, (int)id).Result == false).ToListAsync();
            model.UsersNotOnProject = (List<BTUser>)await _projectService.UsersNotOnProject(id);
            model.UsersOnProject = (List<BTUser>)await _projectService.UsersOnProject(id);
            model.Users = new MultiSelectList(users, "Id", "FullName", members);
            return View(model);
        }

        //AddUser Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignUsers(ManageProjectUsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SelectedUsers != null)
                {
                    var currentMembers = await _context.Projects.Include(p => p.ProjectUsers).FirstOrDefaultAsync(p => p.Id == model.Project.Id);
                    List<string> memberIds = currentMembers?.ProjectUsers.Select(u => u.UserId).ToList();

                    foreach (string id in memberIds)
                    {
                        await _projectService.RemoveUsers(id, model.Project.Id);
                    }

                    foreach (string id in model.SelectedUsers)
                    {
                        await _projectService.AssignUsers(id, model.Project.Id);
                    }
                    return RedirectToAction("Details", "Projects", new { id = model.Project.Id });
                }
                else
                {
                    Debug.WriteLine("***Error assigning user***");
                }
            }
            return View(model);
        }


        //RemoveUserFromProject Get
        [Authorize(Roles = "Admin, Project Manager")]
        public async Task<IActionResult> RemoveUsers(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index, Projects");
            }
            var model = new ManageProjectUsersViewModel();
            model.Project = await _context.Projects.FindAsync((int)id);
            var users = await _context.Users.Where(u => _projectService.IsUserOnProject(u.Id, (int)id).Result).ToListAsync();
            model.Users = new MultiSelectList(users, "Id", "FullName");
            //model.UsersNotOnProject = (List<BTUser>)await _projectService.UsersNotOnProject((int)id);
            //model.UsersOnProject = (List<BTUser>)await _projectService.UsersOnProject((int)id);

            return View(model);
        }

        //RemoveUsersFromProject Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUsers(ManageProjectUsersViewModel model)
        {
            foreach (var userId in model.SelectedUsers)
            {
                if (!await _projectService.IsUserOnProject(userId, model.Project.Id))
                {
                    await _projectService.RemoveUsers(userId, model.Project.Id);
                }
            }
            return RedirectToAction("RemoveUserFromProject");
        }
    }
}
