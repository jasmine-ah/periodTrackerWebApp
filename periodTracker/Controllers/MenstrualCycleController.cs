using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using periodTracker.Data;
using periodTracker.Models;
using periodTracker.Models.ViewModels;
using System.IO;
using System.Drawing;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace periodTracker.Controllers
{
    public class MenstrualCycleController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly PeriodDbContext _context;
        private readonly ILogger<MenstrualCycleController> _logger;

        public MenstrualCycleController(UserManager<User> userManager, PeriodDbContext context, ILogger<MenstrualCycleController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        private string GetCurrentUserId()
        {
            return _userManager.GetUserId(User);
        }
        private string GetCurrentUserName()
        {
            return _userManager.GetUserName(User);
        }



        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            //return _context.Users != null ? 
            return View(await _context.MenstrualCycles.ToListAsync());
            //Problem("Entity set 'PeriodDbContext.Users'  is null.");
        }



        // GET: MenstrualCycle/CollectInitialInfo
        public IActionResult CreateProfile()
        {
            // Check if the user has already entered initial info
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = _userManager.GetUserId(User);
            var menstrualProfile = _context.MenstrualProfiles.FirstOrDefault(p => p.UserId == userId);

            if (menstrualProfile == null)
            {
                // Show the initial info collection form
                return View("CreateProfile");
            }
            else
            {
                // Redirect to the cycle entry page
                return RedirectToAction("Calendar");
            }
        }

        // POST: MenstrualCycle/CollectInitialInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfile(MenstrualProfile profile)
        {
            if (ModelState.IsValid)
            {
                profile.UserId = _userManager.GetUserId(User);
                _context.MenstrualProfiles.Add(profile);
                await _context.SaveChangesAsync();

                // Create and insert menstrual cycle information
                var menstrualCycle = new MenstrualCycle
                {
                    UserId = _userManager.GetUserId(User),
                    CycleLength = profile.CycleLength,
                    PeriodLength = profile.PeriodLength,
                    
                    // You may need to add other properties depending on your model
                };
                _context.MenstrualCycles.Add(menstrualCycle);
                await _context.SaveChangesAsync();


                // Redirect to the cycle entry page
                return RedirectToAction("Calendar");
            }

            return View(profile);
        }

        // GET: MenstrualCycle/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: MenstrualCycle/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("StartDate,EndDate")] MenstrualCycle model)
        {
            if (ModelState.IsValid)
            {
                // Set the user ID based on the current user
                model.UserId = _userManager.GetUserId(User);
                var menstrualProfile = await _context.MenstrualProfiles.FirstOrDefaultAsync(mp => mp.UserId == model.UserId);

                model.CycleLength = menstrualProfile.CycleLength;
                model.PeriodLength = menstrualProfile.PeriodLength;
                // Save the new menstrual cycle to the database
                _context.MenstrualCycles.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Calendar));
            }
            return View(model);
        }


        [Authorize]
        public IActionResult EnterCycle()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var currentCycle = _context.MenstrualCycles
        .Where(m => m.UserId == userId)
        .OrderByDescending(m => m.StartDate)
        .FirstOrDefault();
      
            if (currentCycle?.StartDate == DateTime.MinValue && currentCycle.EndDate == DateTime.MinValue)
            {
                currentCycle = null;
            }
            var viewModel = new MenstrualCycleViewModel
            {
                CurrentCycle = currentCycle, // Initialize with the retrieved current cycle or a new empty cycle,
                UserName = _userManager.GetUserName(User),
                AverageCycleLength = CalculateAverageCycleLength(),
                PreviousCycles = GetPeriodHistory(),
                PredictedNextPeriod = PredictNextPeriod(),
                OvulationDay=CalculateOvulationDay(),
                FertileWindow=CalculateFertileWindow(),
                DaysLeftForNextPeriod = DaysLeftForNextPeriod(),
                FutureCycles =CalculateFutureCycles(),
                UserId = _userManager.GetUserId(User)

            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnterCycle(MenstrualCycleViewModel viewModel)
        {
            //var userId = _userManager.GetUserId(User);
            //if (string.IsNullOrEmpty(userId))
            //       {
            //     return Unauthorized();
            //       }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = viewModel.UserId;
                    var menstrualProfile = await _context.MenstrualProfiles.FirstOrDefaultAsync(mp => mp.UserId == userId);

                    if (menstrualProfile == null)
                    {
                        // Handle the case where no menstrual profile is found for the user

                        return RedirectToAction(nameof(Index));
                    }

                    // Map the ViewModel to a MenstrualCycle object
                    var menstrualCycle = new MenstrualCycle
                    {
                        StartDate = viewModel.CurrentCycle.StartDate,
                        EndDate = viewModel.CurrentCycle.EndDate,
                        CycleLength = menstrualProfile.CycleLength,
                        PeriodLength = menstrualProfile.PeriodLength,

                        UserId = viewModel.UserId // Retrieve the current user's ID
                    };

                    _context.MenstrualCycles.Add(menstrualCycle);
                    await _context.SaveChangesAsync();

                    // Log the successful creation of the MenstrualCycle
                    _logger.LogInformation($"Created MenstrualCycle with ID {menstrualCycle.Id} for User {menstrualCycle.UserId}");

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log any exceptions that occur during the save operation
                    _logger.LogError(ex, "Failed to create MenstrualCycle");
                    throw; // Re-throw the exception to see the full stack trace in the debugger
                }
            }

            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }


        //////Calculate Average Cycle Length
        ///
        public double? CalculateAverageCycleLength()
        {
            // Retrieve the current user's ID
            var userId = GetCurrentUserId();

            // Query the database for the user's menstrual cycles
            var cycles = _context.MenstrualCycles
                .Where(c => c.UserId == userId && c.StartDate != DateTime.MinValue && c.EndDate != DateTime.MinValue);

            // Calculate the average cycle length
            if (!cycles.Any())
            {
                return null; // Return null if no valid cycles exist
            }

            var totalLength = cycles.Sum(c => c.CycleLength);
            var averageLength = totalLength / cycles.Count();

            return averageLength;
        }

        ///////Show History of Previous Periods
        ///
        public List<MenstrualCycle> GetPeriodHistory()
        {
            var userId = GetCurrentUserId(); // Get the ID of the currently logged-in user
            return _context.MenstrualCycles.Where(c => c.UserId == userId).OrderByDescending(c => c.StartDate).ToList();
        }

        /////Make Predictions of Future Periods
        ///

        public DateTime? PredictNextPeriod()
        {
            var userId = GetCurrentUserId();
            var averageLength = CalculateAverageCycleLength();
            var lastCycle = _context.MenstrualCycles
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.StartDate)
                .FirstOrDefault();

            // Check if the last cycle's StartDate and EndDate are default DateTime values
            // and if averageLength has a value
            if (lastCycle != null && lastCycle.StartDate != DateTime.MinValue && lastCycle.EndDate != DateTime.MinValue && averageLength.HasValue)
            {
                return lastCycle.StartDate.AddDays(averageLength.Value); // Use Value property to get the non-nullable double
            }
            return null; // Return null if no valid cycle data is available or averageLength is null
        }

        public List<MenstrualCycle>? CalculateFutureCycles()
        {
            var userId = GetCurrentUserId();
            var averageCycleLength = CalculateAverageCycleLength();
            var periodLength = _context.MenstrualCycles
                .Where(c => c.UserId == userId)
                .Select(c => c.PeriodLength)
                .FirstOrDefault(); 
               
            var lastCycle = _context.MenstrualCycles
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.StartDate)
                .FirstOrDefault();

            if (lastCycle == null || lastCycle.StartDate == DateTime.MinValue || lastCycle.EndDate == DateTime.MinValue || !averageCycleLength.HasValue)
            {
                return null; // No valid data to predict future cycles
            }

            var futureCycles = new List<MenstrualCycle>();

            for (int i = 0; i < 6; i++)
            {
                var nextCycleStartDate = lastCycle.StartDate.AddDays(averageCycleLength.Value);
                var nextCycleEndDate = nextCycleStartDate.AddDays(periodLength);
                var nextCyclePeriodLength = nextCycleEndDate.Subtract(nextCycleStartDate).Days;
                

                var predictedCycle = new MenstrualCycle
                {
                    StartDate = nextCycleStartDate,
                    EndDate = nextCycleEndDate,
                    CycleLength = (int)averageCycleLength,
                    PeriodLength = nextCyclePeriodLength
                };

                futureCycles.Add(predictedCycle);
                lastCycle = predictedCycle; // Update the last cycle for the next iteration
            }

            return futureCycles;
        }



        public DateTime? CalculateOvulationDay()
        {
            // Retrieve the current user's ID
            var userId = GetCurrentUserId();

            // Get the last menstrual cycle for the current user
            var lastCycle = _context.MenstrualCycles
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.StartDate)
                .FirstOrDefault();

            // Calculate the average cycle length
            var averageCycleLength = CalculateAverageCycleLength();
            if (lastCycle != null && lastCycle.StartDate != DateTime.MinValue && lastCycle.EndDate != DateTime.MinValue && averageCycleLength.HasValue)
            {
                // Estimate the ovulation day
                var ovulationDay = lastCycle.StartDate.AddDays((int)(averageCycleLength * 0.51));
                return ovulationDay;
            }

            return null;

        }


        public List<DateTime>? CalculateFertileWindow()
        {
            // Check if PredictNextPeriod is null
            DateTime? nextPeriodStartDate = PredictNextPeriod();
            if (nextPeriodStartDate == null)
            {
                return null; // Return null if PredictNextPeriod is null
            }

            // Calculate the fertile window start date
            DateTime fertileWindowStartDate = nextPeriodStartDate.Value.AddDays(-14);

            // Generate a list of dates for the fertile window
            List<DateTime> fertileWindowDates = new List<DateTime>();
            for (int i = 0; i < 14; i++)
            {
                fertileWindowDates.Add(fertileWindowStartDate.AddDays(i));
            }

            return fertileWindowDates;
        }

        public int? DaysLeftForNextPeriod()
        {
            // Retrieve the current user's ID
            var userId = GetCurrentUserId();

            // Predict the next period
            var predictedNextPeriod = PredictNextPeriod();

            // Check if the predicted next period is not null
            if (predictedNextPeriod.HasValue)
            {
                // Calculate the difference between the current date and the predicted next period
                var daysLeft = (predictedNextPeriod.Value - DateTime.Now).Days;

                // Return the number of days left if it's a positive number, otherwise return null
                return daysLeft > 0 ? daysLeft : (int?)null;
            }

            // Return null if no valid data is available to predict the next period
            return null;
        }


        //public List<List<DateTime>> CalculateFertileWindow()
        //{
        //    var fertileWindows = new List<List<DateTime>>();

        //    for (int cycleNumber = 0; cycleNumber < 7; cycleNumber++)
        //    {
        //        // Calculate the start date of the next cycle
        //        DateTime nextPeriodStartDate;
        //        if (cycleNumber == 0)
        //        {
        //            // For the next cycle
        //            nextPeriodStartDate = PredictNextPeriod() ?? DateTime.Today;
        //        }
        //        else
        //        {
        //            // For future cycles
        //            var lastCycle = _context.MenstrualCycles
        //                .Where(c => c.UserId == GetCurrentUserId())
        //                .OrderByDescending(c => c.StartDate)
        //                .Skip(cycleNumber - 1) // Skip previous cycles
        //                .FirstOrDefault();
        //            nextPeriodStartDate = lastCycle?.StartDate.AddDays(CalculateAverageCycleLength() ?? 0) ?? DateTime.Today;
        //        }

        //        // Calculate the fertile window start date
        //        DateTime fertileWindowStartDate = nextPeriodStartDate.AddDays(-14);

        //        // Generate a list of dates for the fertile window
        //        List<DateTime> fertileWindowDates = new List<DateTime>();
        //        for (int i = 0; i < 14; i++)
        //        {
        //            fertileWindowDates.Add(fertileWindowStartDate.AddDays(i));
        //        }

        //        // Add the fertile window dates to the list
        //        fertileWindows.Add(fertileWindowDates);
        //    }

        //    return fertileWindows;
        //}





        // Example action to show details of a single cycle
        public async Task<IActionResult> Details()
        {
            // Retrieve the user's ID using UserManager
            var userId = GetCurrentUserId();

            // Fetch the latest menstrual cycle for the current user
            var menstrualCycle = await _context.MenstrualCycles
                .Include(c => c.User)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.StartDate)
                .FirstOrDefaultAsync();

            if (menstrualCycle == null)
            {
                return NotFound();
            }

            // Populate the ViewModel with the menstrual cycle and other data
            var viewModel = new MenstrualCycleViewModel
            {
                CurrentCycle = menstrualCycle,
                UserName = GetCurrentUserName(),
                AverageCycleLength = CalculateAverageCycleLength(),
                PreviousCycles = GetPeriodHistory(),
                PredictedNextPeriod = PredictNextPeriod(),
                //ReminderSent = CheckIfReminderSent(userId) // Uncomment if you want to check for reminders
            };

            return View(viewModel);
        }

     

        // GET: MenstrualCycles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //var userId = GetCurrentUserId();
            if (id ==null)
            {
                return NotFound();
            }

            
            var menstrualCycle = await _context.MenstrualCycles.FindAsync(id);
            if (menstrualCycle == null|| menstrualCycle.UserId != GetCurrentUserId())
            {
                return NotFound();
            }

            return View(menstrualCycle);
        }



        public IActionResult Calendar()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var currentCycle = _context.MenstrualCycles
        .Where(m => m.UserId == userId)
        .OrderByDescending(m => m.StartDate)
        .FirstOrDefault();
            // Use the current date if no previous cycles are found
            //var cycleToUse = currentCycle ?? new MenstrualCycle
            //{
            //    StartDate = DateTime.Today, // Set the start date to the current date
            //                              // ... set other properties as needed
            //};
            // Check if the currentCycle has default StartDate and EndDate
            if (currentCycle?.StartDate == DateTime.MinValue && currentCycle.EndDate == DateTime.MinValue)
            {
                currentCycle = null;
            }
            var viewModel = new MenstrualCycleViewModel
            {
                CurrentCycle = currentCycle, // Initialize with the retrieved current cycle or a new empty cycle,
                //UserName = _userManager.GetUserName(User),
                //AverageCycleLength = CalculateAverageCycleLength(),
                PreviousCycles = GetPeriodHistory(),
                PredictedNextPeriod = PredictNextPeriod(),
                OvulationDay = CalculateOvulationDay(),
                FertileWindow = CalculateFertileWindow(),
                FutureCycles = CalculateFutureCycles(),
                UserId = _userManager.GetUserId(User)

            };

            return View(viewModel);
        }



        // POST: MenstrualCycles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,CycleLength,PeriodLength")] MenstrualCycle menstrualCycle)
        {
            if (id != menstrualCycle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menstrualCycle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenstrualCycleExists(menstrualCycle.Id))
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
            return View(menstrualCycle);
        }


        // GET: MenstrualCycles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menstrualCycle = await _context.MenstrualCycles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menstrualCycle == null || menstrualCycle.UserId != GetCurrentUserId())
            {
                return NotFound();
            }

            return View(menstrualCycle);
        }

        // POST: MenstrualCycles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var menstrualCycle = await _context.MenstrualCycles.FindAsync(id);
            _context.MenstrualCycles.Remove(menstrualCycle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Report
        public async Task<IActionResult> ReportGenerate()
        {
            // Assuming you have a way to identify the current user, e.g., through a logged-in user ID
            var userId = _userManager.GetUserId(User);
            var weight = _context.MenstrualProfiles
                .Where(c => c.UserId == userId)
                .Select(c => c.Weight)
                .FirstOrDefault();
            var height = _context.MenstrualProfiles
                .Where(c => c.UserId == userId)
                .Select(c => c.Height)
                .FirstOrDefault();
            // Fetch the user's details
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Handle case where user is not found
                return NotFound();
            }

            // Prepare the ViewModel
            var reportInfoViewModel = new ReportInfoViewModel
            {
                User = user,
                MenstrualCycleViewModel = new MenstrualCycleViewModel
                {
                    // Populate the MenstrualCycleViewModel properties based on your data source
                    AverageCycleLength = CalculateAverageCycleLength(), // Example value
                    PreviousCycles = GetPeriodHistory(), // Example value
                    PredictedNextPeriod = PredictNextPeriod(), // Example value
                },
                menstrualProfile = new MenstrualProfile
                {
                    Weight = weight, // Example value
                    Height = height, // Example value
                }
            };

            return View(reportInfoViewModel);
        }


        public MenstrualCycleViewModel PopulateMenstrualCycleViewModel()
        {
            // Retrieve the user's ID using UserManager
            var userId = GetCurrentUserId();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            //var menstrualProfile = _context.MenstrualProfiles.FirstOrDefault(mp => mp.UserId == userId);
            var menstrualCycles = _context.MenstrualCycles.Where(mc => mc.UserId == userId).ToList();

          
            // Populate the ViewModel
            MenstrualCycleViewModel viewModel = new MenstrualCycleViewModel
            {
               
                UserName = user?.FirstName + " "+user?.LastName,
                AverageCycleLength = CalculateAverageCycleLength(),
                PreviousCycles = GetPeriodHistory(),
                PredictedNextPeriod = PredictNextPeriod(),
                UserId = userId
            };

            return viewModel;
        }
    


        private bool MenstrualCycleExists(int id)
        {
            return _context.MenstrualCycles.Any(e => e.Id == id);
        }

        private bool MenstrualProfileExists(int id)
        {
            return _context.MenstrualCycles.Any(e => e.Id == id);
        }



       




    }
}

