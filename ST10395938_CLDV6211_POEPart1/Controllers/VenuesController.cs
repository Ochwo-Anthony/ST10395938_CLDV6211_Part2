using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10395938_CLDV6211_POEPart1.Models;
using Azure.Storage.Blobs;

namespace ST10395938_CLDV6211_POEPart1.Controllers
{
    public class VenuesController : Controller
    {
        // The code was adapted from "MVC, Entity Framework & SQL Azure" by Adeol Adisa
        private readonly ApplicationDbContext _context;

        public VenuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var venues = await _context.Venues.ToListAsync();
            return View(venues);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venues venue)
        {
            if (ModelState.IsValid)
            {
                if (venue.ImageFile != null)
                {
                    var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);

                    venue.ImageUrl = blobUrl;
                }
                
                _context.Add(venue);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue created successfully!";

                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        public async Task<IActionResult> Delete(int? venueId)
        {
            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == venueId);

            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int venueId)
        {

            var venue = await _context.Venues
                .Include(v => v.Bookings)  // Include bookings for check
                .Include(v => v.Events)    // Include Events
                .FirstOrDefaultAsync(v => v.VenueId == venueId);

            if (venue == null)
            {
                return NotFound();
            }

            if (venue.Bookings.Any()) 
            {
                TempData["ErrorMessage"] = "Cannot delete venue. It has existing bookings.";
                return RedirectToAction(nameof(Index));
            }

            if (venue.Events.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete venue. It is linked to an event.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue deleted successfully!";
            }

            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the venue.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VenuesExists(int venueId)
        {
            return _context.Venues.Any(e => e.VenueId == venueId);
        }

        public async Task<IActionResult> Edit(int? venueId)
        {
            if (venueId == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues.FindAsync(venueId);
            if (venueId == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int venueId, Venues venue)
        {
            if (venueId != venue.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (venue.ImageFile != null)
                    {
                        var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);

                        venue.ImageUrl = blobUrl;
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException) 
                {
                    if (!_context.Venues.Any(e => e.VenueId == venue.VenueId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Concurrency error occurred while updating the venue.";
                        throw;
                    }
                }

                catch (Exception)
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the venue.";
                }

            }
                return View(venue);      
        }

        public async Task<IActionResult> Details(int? venueId)
        {
            if (venueId == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
              
                .FirstOrDefaultAsync(b => b.VenueId == venueId);

            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);  // Return the booking details to the view
        }

        private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            var connectionString = "";
            var containerName = "cldv6211poepart2";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(imageFile.FileName));

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            return blobClient.Uri.ToString();
        }

    }
}
