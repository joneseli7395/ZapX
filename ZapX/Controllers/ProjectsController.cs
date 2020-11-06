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

namespace ZapX.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _projectService;

        public ProjectsController(ApplicationDbContext context, IBTProjectService projectService)
        {
            _context = context;
            _projectService = projectService;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            return View(await _context.Projects.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var vm = new TicketProjectsViewModel();

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
        public async Task<IActionResult> Create([Bind("Id,Name,ImagePath,ImageData")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
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
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }

        //AddUser Get
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

                    //memberIds.ForEach(async (i) => await _projectService.RemoveUsers(i, model.Project.Id));

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
