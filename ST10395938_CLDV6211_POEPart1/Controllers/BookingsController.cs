using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ST10395938_CLDV6211_POEPart1.Models;

namespace ST10395938_CLDV6211_POEPart1.Controllers
{
    public class BookingsController : Controller
    {
        // The code was adapted from "MVC, Entity Framework & SQL Azure" by Adeol Adisa
        private readonly ApplicationDbContext _context;
        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var bookings = _context.Bookings
                .Include(i => i.Venues)
                .Include(i => i.Events)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(i =>
                i.Venues.VenueName.Contains(searchString) ||
                i.Events.EventName.Contains(searchString));
            }

            return View(await bookings.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.Venues = _context.Venues.ToList();
            ViewBag.Events = _context.Events.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bookings bookings)
        {
            var selectedEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventId == bookings.EventId);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event not found.");
                ViewBag.Venues = _context.Venues.ToList();
                ViewBag.Events = _context.Events.ToList();
                return View(bookings);
            }

            // Step 2: Check for booking conflict at the same venue and event date
            var conflict = await _context.Bookings
                .Include(b => b.Events)
                .AnyAsync(b => b.VenueId == bookings.VenueId &&
                               b.Events.EventDate.Date == selectedEvent.EventDate.Date &&
                               b.EventId != bookings.EventId);

            if (conflict)
            {
                ModelState.AddModelError("", "This venue is already booked for that date.");
                ViewBag.Venues = _context.Venues.ToList();
                ViewBag.Events = _context.Events.ToList();
                return View(bookings);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bookings.Venues = null;  // Prevent EF from thinking this is a new Venue object
                    bookings.Events = null;

                    _context.Add(bookings);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking created successfully.";
                    return RedirectToAction(nameof(Index));

                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "An error occurred while saving the booking. Please try again.");
                }

            }

            ViewBag.Venues = _context.Venues.ToList();
            ViewBag.Events = _context.Events.ToList();
            return View(bookings);
        }      

        public async Task<IActionResult> Delete(int? bookingId)
        {
            if (bookingId == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings
                .Include(f => f.Venues)
                .Include(f => f.Events)// Include related venue data
                .FirstOrDefaultAsync(e => e.BookingId == bookingId);

            if (bookings == null)
            {
                return NotFound();
            }

            return View(bookings);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int bookingId)
        {
            var bookings = await _context.Bookings.FindAsync(bookingId);

            if (bookings != null)
            {
                _context.Bookings.Remove(bookings);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Booking deleted successfully!";

            }
            else
            {
                TempData["ErrorMessage"] = "Booking not found.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? bookingId)
        {
            if (bookingId == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(f => f.Venues)   // Include related venue data
                .Include(f => f.Events)   // Include related event data
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);  // Return the booking details to the view
        }

        public async Task<IActionResult> Edit(int? bookingId)
        {
            if (bookingId == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                return NotFound();
            }

            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            ViewBag.Events = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int bookingId, Bookings booking)
        {
            if (bookingId != booking.BookingId)
            {
                return NotFound();
            }

            var selectedEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventId == booking.EventId);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event not found.");
            }
            else
            {
                var conflict = await _context.Bookings
                    .Include(b => b.Events)
                    .AnyAsync(b => b.VenueId == booking.VenueId &&
                                   b.Events.EventDate.Date == selectedEvent.EventDate.Date &&
                                   b.BookingId != booking.BookingId); // exclude current booking

                if (conflict)
                {
                    ModelState.AddModelError("", "This venue is already booked for that date.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    booking.Venues = null;
                    booking.Events = null;

                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "An error occurred while updating the booking.");
                }
            }

            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            ViewBag.Events = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            return View(booking);
        }



    }
}
