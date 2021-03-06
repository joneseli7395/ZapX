﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZapX.Data;
using ZapX.Models;

namespace ZapX.Controllers
{
    [Authorize]
    public class TicketCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;

        public TicketCommentsController(ApplicationDbContext context, UserManager<BTUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TicketComments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TicketComments.Include(t => t.Ticket).Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TicketComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketComment = await _context.TicketComments
                .Include(t => t.Ticket)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketComment == null)
            {
                return NotFound();
            }

            return View(ticketComment);
        }

        // GET: TicketComments/Create
        public IActionResult Create(int? id)
        {
            var model = new TicketComment();
            if (id == null)
            {
                ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Title");
            }
            else
            {
                model.TicketId = (int)id;
            }
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View(model);
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Comment,TicketId,UserId")] TicketComment ticketComment)
        {
            if (!User.IsInRole("Demo"))
            {
                if (ModelState.IsValid)
                {
                    ticketComment.Created = DateTime.Now;
                    if (ticketComment.UserId == null)
                    {
                        ticketComment.UserId = _userManager.GetUserId(User);
                    }
                    _context.Add(ticketComment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Tickets", new { Id = ticketComment.TicketId });
                }
                //ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description", ticketComment.TicketId);
                //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", ticketComment.UserId);
                return NotFound();
            }
            else
            {
                TempData["DemoLockout"] = "Your changes have not been saved. To make changes to the database you will need to log in as a full user.";
                return RedirectToAction("Details", "Projects", new { id = ticketComment.TicketId });
            }
        }

        // GET: TicketComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketComment = await _context.TicketComments.FindAsync(id);
            if (ticketComment == null)
            {
                return NotFound();
            }
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description", ticketComment.TicketId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", ticketComment.UserId);
            return View(ticketComment);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Comment,Created,TicketId,UserId")] TicketComment ticketComment)
        {
            if (!User.IsInRole("Demo"))
            {
                if (id != ticketComment.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(ticketComment);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TicketCommentExists(ticketComment.Id))
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
                ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description", ticketComment.TicketId);
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", ticketComment.UserId);
                return View(ticketComment);
            }
            else
            {
                TempData["DemoLockout"] = "Your changes have not been saved. To make changes to the database you will need to log in as a full user.";
                return RedirectToAction("Details", "Projects", new { id = ticketComment.TicketId });
            }
        }

        // GET: TicketComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketComment = await _context.TicketComments
                .Include(t => t.Ticket)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketComment == null)
            {
                return NotFound();
            }

            return View(ticketComment);
        }

        // POST: TicketComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int ticketId)
        {
            var ticketComment = await _context.TicketComments.FindAsync(id);
            if (!User.IsInRole("Demo"))
            {
                _context.TicketComments.Remove(ticketComment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Tickets", new { Id = ticketId });
            }
            else
            {
                TempData["DemoLockout"] = "Your changes have not been saved. To make changes to the database you will need to log in as a full user.";
                return RedirectToAction("Details", "Projects", new { id = ticketId });
            }
        }

        private bool TicketCommentExists(int id)
        {
            return _context.TicketComments.Any(e => e.Id == id);
        }
    }
}
