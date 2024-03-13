using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using periodTracker.Data;
using periodTracker.Models;
using periodTracker.Models.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace periodTracker.Controllers
{
    public class UserSymptomController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly PeriodDbContext _context;

        public UserSymptomController(UserManager<User> userManager, PeriodDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> SaveSubtypes()
        {
            var userId = _userManager.GetUserId(User);

            // Fetch symptom types from the database
            var symptomTypes = await _context.SymptomTypes.OrderBy(st => st.Name).ToListAsync();

            var viewModel = new List<SymptomViewModel>();

            // Fetch severities from the database
            var severities = await _context.Severities.ToListAsync();

            // Iterate over each symptom type
            foreach (var symptomType in symptomTypes)
            {
                // Fetch subtypes for the current symptom type
                var subtypes = await _context.Subtypes
                    .Include(s => s.Severity)
                    .Where(s => s.SymptomTypeId == symptomType.Id)
                    .OrderBy(s => s.Name)
                    .Select(s => new SubtypeViewModel
                    {
                        SubtypeId = s.Id,
                        SubtypeName = s.Name,
                        SeverityId = s.Severity.Id,
                        SeverityLevel = s.Severity.Level,
                        UserId = userId,
                        IsSelected = false // Assuming you want all checkboxes unchecked initially
                })
                    .ToListAsync();

                // Add the current symptom type with its subtypes to the view model
                viewModel.Add(new SymptomViewModel
                {
                    SymptomTypeId = symptomType.Id,
                    SymptomTypeName = symptomType.Name,
                    Subtypes = subtypes
                });
            }

            // Pass the severities to the view using ViewData
            ViewData["Severities"] = severities;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSubtypes(List<SymptomViewModel> model)
        {
            // Get the current user's ID
            var userId = _userManager.GetUserId(User);

            // Loop through the submitted model to find selected subtypes
            foreach (var symptomViewModel in model)
            {
                foreach (var subtypeViewModel in symptomViewModel.Subtypes)
                {
                    // Only process subtypes that were selected
                    if (subtypeViewModel.IsSelected)
                    {
                        // If severity is null, assign a default severity (assuming severity can't be null)
                        if (subtypeViewModel.SeverityId == null)
                        {
                            // Assign a default severity ID or handle it appropriately
                            subtypeViewModel.SeverityId = GetDefaultSeverityId();
                        }
                        // Create a new UserSymptom record
                        var userSymptom = new UserSymptom
                        {
                            UserId = userId,
                            //SymptomTypeId = symptomViewModel.SymptomTypeId,
                            SubTypeId = subtypeViewModel.SubtypeId,
                            SeverityId = subtypeViewModel.SeverityId,
                            Timestamp = DateTime.UtcNow
                        };

                        // Add the new UserSymptom record to the database
                        _context.UserSymptoms.Add(userSymptom);
                    }
                }
            }

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Redirect to a suitable action after saving
            return RedirectToAction("EnterCycle","MenstrualCycle");
        }

        // Inside UserSymptomController

        // GET: UserSymptom/SelectedSymptoms
        public async Task<IActionResult> ShowSelectedSymptoms()
        {
            var userId = _userManager.GetUserId(User);

            // Fetch selected symptoms from the database
            var selectedSymptoms = await _context.UserSymptoms
                .Where(us => us.UserId == userId)
                .Include(us => us.Subtype) // Include Subtype to get SymptomSubtypeNames
                .Include(us=>us.Severities)
                //.Select(us=>us.Timestamp)
                .ToListAsync();
            return View(selectedSymptoms);
        }




            private int GetDefaultSeverityId()
        {
            // Return default severity ID based on your logic
            return 1;
        }

    }


  


















    //[HttpGet]
    //public async Task<IActionResult> Index()
    //{
    //    var symptomTypes = await _context.SymptomTypes.Include(st => st.Subtypes).ToListAsync();
    //    var severities = await _context.Severities.ToListAsync();

    //    var viewModel = new SymptomSelectionViewModel
    //    {
    //        SymptomTypes = symptomTypes,
    //        Subtypes = symptomTypes.SelectMany(st => st.Subtypes).ToList(),
    //        Severities = severities
    //    };

    //    return View(viewModel);
    //}

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> SelectSymptom(SymptomSelectionViewModel model)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        // If the model state is invalid, return the view with the current model to show errors
    //        return View(model);
    //    }

    //    try
    //    {
    //        // Retrieve the selected SymptomType and Subtype objects from the database
    //        var selectedSymptomType = await _context.SymptomTypes
    //            .Include(st => st.Subtypes)
    //            .SingleOrDefaultAsync(st => st.Id == model.SelectedSymptomTypeId);

    //        var selectedSubtype = selectedSymptomType?.Subtypes
    //            .SingleOrDefault(s => s.Id == model.SelectedSubtypeId);

    //        // Retrieve the selected Severity object from the database
    //        var selectedSeverity = await _context.Severities
    //            .SingleOrDefaultAsync(sev => sev.Id == model.SelectedSeverityId);

    //        if (selectedSymptomType != null && selectedSubtype != null && selectedSeverity != null)
    //        {
    //            // Create a new Symptom object with the selected values
    //            var usersymptom = new UserSymptom
    //            {
    //                SymptomTypeId = selectedSymptomType.Id,
    //                SymptomType = selectedSymptomType,
    //                SubtypeId = selectedSubtype.Id,
    //                Subtype = selectedSubtype,
    //                SeverityId = selectedSeverity.Id,
    //                Severity = selectedSeverity,
    //                UserId = _userManager.GetUserId(User),
    //                Timestamp = DateTime.UtcNow
    //            };

    //            // Add the new Symptom object to the database
    //            _context.UserSymptoms.Add(usersymptom);
    //            await _context.SaveChangesAsync();

    //            // Redirect to a success page after successfully processing the form
    //            return RedirectToAction("Success");
    //        }
    //        else
    //        {
    //            // Handle the case where the selected IDs do not correspond to any existing entities
    //            ModelState.AddModelError(string.Empty, "Invalid selection.");
    //            return View(model);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // Handle exceptions that occur during the database operations
    //        ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
    //        return View(model);
    //    }
    //}






    //// GET: /<controller>/
    //public async Task<IActionResult> Index()
    //{
    //    var userId = _userManager.GetUserId(User);
    //    if (userId == null)
    //    {
    //        return Unauthorized();
    //    }

    //    // Fetch SymptomTypes along with their Subtypes
    //    var symptomTypesWithSubtypes = await _context.SymptomTypes
    //        .Include(st => st.Subtypes)
    //        .ToListAsync();

    //    // Group subtypes by SymptomTypeId
    //    var groupedSubtypes = symptomTypesWithSubtypes
    //        .SelectMany(st => st.Subtypes, (st, s) => new { st.Name, Subtype = s })
    //        .GroupBy(x => x.Name!)
    //        .ToDictionary(g => g.Key, g => g.Select(x => x.Subtype).ToList());
    //    int symptomTypeId = model.SymptomTypeIds[selectedSubtypeId];

    //    var severities = await _context.Severities.ToListAsync();

    //    var viewModel = new SymptomViewModel
    //    {
    //        SymptomTypes = symptomTypesWithSubtypes,
    //        SymptomTypeIds=
    //        GroupedSubtypes = groupedSubtypes,
    //        Severities = severities,
    //        UserId=userId
    //    };


    //    return View(viewModel);
    //}

    //// Controllers/SymptomsController.cs

    //// Controllers/SymptomsController.cs

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Index(SymptomViewModel model)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        // If validation fails, redisplay the form with the existing values
    //        // Fetch SymptomTypes and Subtypes again to repopulate the view model
    //        model.SymptomTypes = await _context.SymptomTypes
    //            .Include(st => st.Subtypes!)
    //            .ThenInclude(s => s.Severity)
    //            .ToListAsync();

    //        model.GroupedSubtypes = model.SymptomTypes
    //            .SelectMany(st => st.Subtypes, (st, s) => new { st.Name, Subtype = s })
    //            .GroupBy(x => x.Name!)
    //            .ToDictionary(g => g.Key, g => g.Select(x => x.Subtype).ToList());

    //        //model.Severities = await _context.Severities.ToListAsync();

    //        return View(model);
    //    }

    //    // Retrieve the current user ID from the user claims or session
    //    var userId = _userManager.GetUserId(User);

    //    // Assume that the form sends an array of selected subtype IDs and their corresponding severity IDs
    //    // The keys of the dictionary would be the subtype IDs and the values would be the severity IDs
    //    //var selectedSeverityId = model.SelectedSeverities.FirstOrDefault(id => id.Key == selectedSubtypeId).Value;

    //    foreach (var selectedSubtypeId in model.SelectedSubtypes)
    //    {
    //        var subtype = await _context.Subtypes
    //            .Include(s => s.Severity)
    //            .SingleOrDefaultAsync(s => s.Id == selectedSubtypeId);

    //        if (subtype != null)
    //        {
    //            // Assuming model.SelectedSeverities is a Dictionary<int, int>
    //            // where the key is the subtype ID and the value is the severity ID
    //            int selectedSeverityId = model.SelectedSeverities[selectedSubtypeId];
    //            int symptomTypeId = model.SymptomTypeIds[selectedSubtypeId];
    //            var usersymptom = new UserSymptom
    //            {
    //                SymptomTypeId = symptomTypeId,
    //                SeverityId = selectedSeverityId,
    //                UserId = userId,
    //                Timestamp = DateTime.UtcNow
    //            };

    //            _context.UserSymptoms.Add(usersymptom);
    //        }
    //    }

    //    await _context.SaveChangesAsync();

    //    // Redirect to another action or view indicating success
    //    return RedirectToAction("EnterCycle", "MenstrualCycle");
    //}

    //private async Task<int> GetSeverityIdForLevel(string severityLevel)
    //{
    //    // Look up the severity ID based on the severity level string
    //    var severity = await _context.Severities
    //        .Where(s => s.Level == severityLevel)
    //        .FirstOrDefaultAsync();

    //    // Return the ID of the found severity level, or throw an exception if not found
    //    if (severity != null)
    //    {
    //        return severity.Id;
    //    }
    //    else
    //    {
    //        throw new InvalidOperationException($"No severity level found for '{severityLevel}'.");
    //    }
    //}

}


