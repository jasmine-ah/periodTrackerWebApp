using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using periodTracker.Data;
using periodTracker.Models;
using periodTracker.Models.ViewModels;

namespace periodTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly PeriodDbContext _context;
        private readonly UserManager<User> _userManager;



        public AdminController(PeriodDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        public IActionResult Dashboard()
        {
            // Retrieve data for dashboard
            var totalUsers = _context.Users.Count();
            var totalMenstruations = _context.MenstrualProfiles.Count();
            var users20to35Count = _context.Users.Count(u => u.Age >= 20 && u.Age <= 35);

            // Calculate percentage of users within age group of 20-35
            double percentageUsers20to35 = (double)users20to35Count / totalUsers * 100;

            // Round percentage to two decimal places
            percentageUsers20to35 = Math.Round(percentageUsers20to35, 2);

            // Create a view model to pass data to the view
            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalMenstruations = totalMenstruations,
                PercentageUsers20to35 = percentageUsers20to35
            };

            return View(viewModel);
        }

        public async Task<IActionResult> DisplayFeedback()
        {

            var feedbacks = await _context.Feedbacks.ToListAsync();
            return View(feedbacks);
        }


        //// GET: User/Details
        //// GET: Admin/Details
        //public async Task<IActionResult> Details(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        return NotFound();
        //    }

        //    var user = await _userManager.FindByIdAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    // Retrieve menstrual profile for the user
        //    var menstrualProfile = await _context.MenstrualProfiles.SingleOrDefaultAsync(mp => mp.UserId == id);
        //    if (menstrualProfile == null)
        //    {
        //        return NotFound();
        //    }

        //    var viewModel = new AdminDetailsViewModel
        //    {
        //        User = user,
        //        MenstrualProfile = menstrualProfile
        //    };

        //    return View(viewModel);
        //}




        //public IActionResult SearchUsers(string searchString)
        //{
        //    var users = _userManager.Users
        //        .Where(u => u.UserName.Contains(searchString) || u.Email.Contains(searchString))
        //        .ToList();
        //    return View(users);
        //}

    }
}

