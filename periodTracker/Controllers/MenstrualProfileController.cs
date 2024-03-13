using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using periodTracker.Models;
using periodTracker.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
// GET: /<controller>/




namespace periodTracker.Controllers
    {
        [Authorize]
        public class MenstrualProfileController : Controller
        {
            private readonly PeriodDbContext _context;
            private readonly UserManager<User> _userManager;

        public MenstrualProfileController(PeriodDbContext context, UserManager<User> userManager)
            {
                _context = context;
                _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: MenstrualProfile/Edit
        public async Task<IActionResult> Edit()
            {
                var userId = _userManager.GetUserId(User);
                var profile = await _context.MenstrualProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

                if (profile == null)
                {
                    return View("Create");
                }

                return View(profile);
            }

            // GET: MenstrualProfile/Create
            public IActionResult Create()
            {
                return View();
            }

            // POST: MenstrualProfile/Edit
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("Id,CycleLength,PeriodLength,Weight,Height,UserId")] MenstrualProfile profile)
            {
                if (id != profile.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(profile);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MenstrualProfileExists(profile.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Edit));
                }
                return View(profile);
            }

            // POST: MenstrualProfile/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("CycleLength,PeriodLength,Weight,Height")] MenstrualProfile profile)
            {
                if (ModelState.IsValid)
                {
                    profile.UserId = _userManager.GetUserId(User);
                    _context.Add(profile);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Edit));
                }
                return View(profile);
            }

            private bool MenstrualProfileExists(int id)
            {
                return _context.MenstrualProfiles.Any(e => e.Id == id);
            }
        }
    }



