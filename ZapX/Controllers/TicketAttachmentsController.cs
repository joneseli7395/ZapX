using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZapX.Data;
using ZapX.Models;
using ZapX.Utilities;

namespace ZapX.Controllers
{
    [Authorize]
    public class TicketAttachmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;

        public TicketAttachmentsController(ApplicationDbContext context, UserManager<BTUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TicketAttachments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TicketAttachments.Include(t => t.Ticket).Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TicketAttachments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketAttachment = await _context.TicketAttachments
                .Include(t => t.Ticket)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketAttachment == null)
            {
                return NotFound();
            }

            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Create
        public IActionResult Create(int? id)
        {
            var model = new TicketAttachment();
            if (id == null)
            {
                ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description");
            }
            else
            {
                model.TicketId = (int)id;
                model.FileData = new Byte[64];
            }
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");
            return View(model);
        }

        // POST: TicketAttachments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,TicketId")] TicketAttachment ticketAttachment, IFormFile attachment)
        {
            if (!User.IsInRole("Demo"))
            {
                if (ModelState.IsValid)
                {
                    var memoryStream = new MemoryStream();
                    attachment.CopyTo(memoryStream);
                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();
                    memoryStream.Dispose();
                    var binary = Convert.ToBase64String(bytes);
                    var ext = Path.GetExtension(attachment.FileName);

                    ticketAttachment.FilePath = $"data:image/{ext};base64,{binary}";
                    ticketAttachment.FileData = bytes;
                    ticketAttachment.Created = DateTimeOffset.Now;
                    ticketAttachment.UserId = _userManager.GetUserId(User);
                    ticketAttachment.Description = Path.GetFileNameWithoutExtension(attachment.FileName);

                    _context.Add(ticketAttachment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
                }
                return NotFound();
            }
            else
            {
                TempData["DemoLockout"] = "Your changes have not been saved. To make changes to the database you will need to log in as a full user.";
                return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
            }
        }

        // GET: TicketAttachments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketAttachment = await _context.TicketAttachments.FindAsync(id);
            if (ticketAttachment == null)
            {
                return NotFound();
            }
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description", ticketAttachment.TicketId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FilePath,FileData,Description,Created,TicketId,UserId")] TicketAttachment ticketAttachment)
        {
            if (!User.IsInRole("Demo"))
            {


                if (id != ticketAttachment.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(ticketAttachment);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TicketAttachmentExists(ticketAttachment.Id))
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
                ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description", ticketAttachment.TicketId);
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", ticketAttachment.UserId);
                return View(ticketAttachment);
            }
            else
            {
                TempData["DemoLockout"] = "Your changes have not been saved. To make changes to the database you will need to log in as a full user.";
                return RedirectToAction("Details", "Projects", new { id = ticketAttachment.TicketId });
            }
        }

        // GET: TicketAttachments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketAttachment = await _context.TicketAttachments
                .Include(t => t.Ticket)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketAttachment == null)
            {
                return NotFound();
            }

            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int ticketId)
        {
            var ticketAttachment = await _context.TicketAttachments.FindAsync(id);
            if (!User.IsInRole("Demo"))
            {
                _context.TicketAttachments.Remove(ticketAttachment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Tickets", new { id = ticketId });
            }
            else
            {
                TempData["DemoLockout"] = "Your changes have not been saved. To make changes to the database you will need to log in as a full user.";
                return RedirectToAction("Details", "Projects", new { id = ticketId });
            }
        }

        private bool TicketAttachmentExists(int id)
        {
            return _context.TicketAttachments.Any(e => e.Id == id);
        }
    }
}
