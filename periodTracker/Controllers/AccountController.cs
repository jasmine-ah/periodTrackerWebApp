using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using periodTracker.Data;
using periodTracker.Models;
using periodTracker.Models.ViewModels;
using periodTracker.Services;
namespace periodTracker.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly PeriodDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMailService _mailService;

        public AccountController(PeriodDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IMailService mailService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mailService = mailService;

        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            //return _context.Users != null ? 
            return View(await _context.Users.ToListAsync());
                          //Problem("Entity set 'PeriodDbContext.Users'  is null.");
        }



        //// GET: User/Details
        //public async Task<IActionResult> Details()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(user);
        //}



        // GET: User/Details
        public IActionResult Details()
        {
            var userId = _userManager.GetUserId(User);
            //var user = await _userManager.GetUserAsync(User);
            if (userId == null)
            {
                return NotFound();
            }
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            var menstrualProfile = _context.MenstrualProfiles.FirstOrDefault(m => m.UserId == userId);
            var userProfileViewModel = new UserProfileViewModel
            {
                User = user,
                MenstrualProfile = menstrualProfile
            };

            return View(userProfileViewModel);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    Email = model.Email,
                    Age = model.Age,
                    DateOfBirth = model.DateOfBirth,

                };

                var result = await _userManager.CreateAsync(user,model.Password!);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, false);
                    TempData["SuccessMessage"] = "Successfully registered";
                    return RedirectToAction("Login", "Account");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,Age,UserName,PasswordHash,Email,DateOfBirth")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Find the user by ID and update their properties
                    var existingUser = await _userManager.FindByIdAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Update the properties of the existing user with the values from the posted user
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Age = user.Age;
                    //existingUser.UserName = user.UserName;
                    existingUser.Email = user.Email;
                    existingUser.DateOfBirth = user.DateOfBirth;

                    // Handle password separately as it requires a special method
                    if (!string.IsNullOrWhiteSpace(user.PasswordHash))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                        var result = await _userManager.ResetPasswordAsync(existingUser, token, user.PasswordHash);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(user);
                        }
                    }

                    // Save the changes using the UserManager
                    var updateResult = await _userManager.UpdateAsync(existingUser);
                    if (!updateResult.Succeeded)
                    {
                        foreach (var error in updateResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(user);
                    }

                    return RedirectToAction(nameof(Details));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(user);
        }


        //// GET: /User/Login

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username!);
                    var roles = await _userManager.GetRolesAsync(user);

                    // Check if the user is in the Admin role and redirect accordingly
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    // Assuming "User" is the default role for regular users
                    else
                    {
                        //// In your Login action method, after validating the user
                        //HttpContext.Session.SetString("IsAuthenticated", "true");

                        return RedirectToAction("CreateProfile", "MenstrualCycle");
                    }
                }
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }
            return View(model);
        }


        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var menstrualCycles = _context.MenstrualCycles.Where(m => m.UserId == id);
            _context.MenstrualCycles.RemoveRange(menstrualCycles);
            await _context.SaveChangesAsync();
            var menstrualProfiles = _context.MenstrualProfiles.Where(m => m.UserId == id);
            _context.MenstrualProfiles.RemoveRange(menstrualProfiles);
            await _context.SaveChangesAsync();
            var feebacks = _context.Feedbacks.Where(m => m.UserId == id);
            _context.Feedbacks.RemoveRange(feebacks);
            await _context.SaveChangesAsync();
            IdentityResult result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index2","Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
        }
        // GET: Feedback/Create
        public IActionResult Feedback()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Feedback(Feedback model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get the current user's ID
            var userId = _userManager.GetUserId(User);
            var username = _userManager.Users
                .Where(c => c.Id == userId)
                .Select(c => c.Email)
                .FirstOrDefault();
            if (username == null)
            {
                // Handle the case where the user is not found
                return RedirectToAction("Calendar", "MenstrualCycle"); // Redirect to a suitable page
            }

            //var username = user.UserName;
            // Assign the user ID to the feedback model
            model.userName = username;
            model.UserId = userId;
            model.SubmissionTime = DateTime.Now;
            try
            {

                // Save the feedback to the database
                _context.Feedbacks.Add(model);
            await _context.SaveChangesAsync();

            // Redirect to the feedback success page or back to the home page
            return RedirectToAction("Index2", "Home");
            }
            catch (Exception ex)
            {
                // Handle the exception (log, display an error message, etc.)
                ModelState.AddModelError("", "An error occurred while saving the feedback.");
                return View(model);
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }

        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }

        //    var result = await _userManager.ConfirmEmailAsync(user, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        // GET: forgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // Generate password reset token
                string code = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Create a callback URL for the reset password action
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                // Use the IMailService to send the email with the reset link
                await _mailService.SendAsync(new MailData
                {
                    Email = user.Email,
                    Subject = "Reset Password",
                    Body = $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>"
                });

                return RedirectToAction("ResetPassword", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: resetpassword
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            // Add errors to ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }
        // GET: resetpasswordconfirm
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        private bool UserExists(string id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
