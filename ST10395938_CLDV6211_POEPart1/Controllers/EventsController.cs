using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ST10395938_CLDV6211_POEPart1.Models;

namespace ST10395938_CLDV6211_POEPart1.Controllers
{
    public class EventsController : Controller
    {
        // The code was adapted from "MVC, Entity Framework & SQL Azure" by Adeol Adisa
        private readonly ApplicationDbContext _context;
        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _context.Events
                .Include(i => i.Venues)
                .ToListAsync();

            return View(events);
        }

        public IActionResult Create()
        {
            ViewBag.Venues = _context.Venues.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Events events)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(events);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "An error occurred while creating the event.";
                    // Re-display the form with the current model and venues
                    ViewBag.Venues = _context.Venues.ToList();
                    return View(events);
                }
            }

            // Model validation failed
            TempData["ErrorMessage"] = "Please correct the errors and try again.";
            ViewBag.Venues = _context.Venues.ToList();
            return View(events);
        }


        public async Task<IActionResult> Delete(int? eventId)
        {
            if (eventId == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .Include(e => e.Venues) // Include related venue data
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int eventId)
        {
            var eventItem = await _context.Events
                .Include(e => e.Bookings)  // Include bookings to check for dependencies
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            // Check if the event has any bookings
            if (eventItem.Bookings.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete event. It has existing bookings.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Events.Remove(eventItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event deleted successfully!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the event.";
            }

            return RedirectToAction(nameof(Index));
        }


        private bool EventExists(int eventId)
        {
            return _context.Events.Any(e => e.EventId == eventId);
        }

        public async Task<IActionResult> Edit(int? eventId)
        {
            if (eventId == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound();
            }

            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName", eventItem.VenueId);
            return View(eventItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int eventId, Events eventItem)
        {
            if (eventId != eventItem.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventItem);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(eventItem.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "A concurrency error occurred.";
                        throw;
                    }
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the event.";
                }
            }

            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName", eventItem.VenueId);
            return View(eventItem);
        }


        public async Task<IActionResult> Details(int? eventId)
        {
            if (eventId == null)
            {
                return NotFound();
            }

            var events = await _context.Events
                .Include(f => f.Venues)   // Include related venue data               
                .FirstOrDefaultAsync(b => b.EventId == eventId);

            if (events == null)
            {
                return NotFound();
            }

            return View(events);  // Return the booking details to the view
        }

    }
}
