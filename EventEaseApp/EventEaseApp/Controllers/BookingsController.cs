using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventEaseApp.Models;
using EventEaseApp.ViewModel;
using Microsoft.Ajax.Utilities;

namespace EventEaseApp.Controllers
{
    public class BookingsController : Controller
    {
        private PortfolioOfEvidencePOneDBContext db = new PortfolioOfEvidencePOneDBContext();

        // GET: Bookings
        public ActionResult Index()
        {
            var bookings = db.Bookings.Include(b => b.Event).Include(b => b.Venue);
            return View(bookings.ToList());
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {
            var viewModel = new BookingViewModel
            {
                Booking = new Booking(),
                Events = db.Events.ToList(),  // Get actual entities
                Venues = db.Venues.ToList()   // Get actual entities
            };
            return View(viewModel);
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookingViewModel viewModel)
        {
            // Defensive null checks
            if (viewModel == null || viewModel.Booking == null)
            {
                ModelState.AddModelError("", "⚠️ An unexpected error occurred. Please try again.");
                viewModel.Events = db.Events.ToList();
                viewModel.Venues = db.Venues.ToList();
                return View(viewModel);
            }

            // Repopulate dropdowns
            viewModel.Events = db.Events.ToList();
            viewModel.Venues = db.Venues.ToList();

            if (ModelState.IsValid)
            {
                // Validate required fields
                if (viewModel.Booking.VenueID == 0 || viewModel.Booking.EventID == 0 ||
                    viewModel.Booking.StartDate == default || viewModel.Booking.EndDate == default)
                {
                    ModelState.AddModelError("", "⚠️ Please fill in all required fields.");
                    return View(viewModel);
                }

                try
                {
                    // Check for overlapping bookings
                    var existingBookings = db.Bookings
                        .Where(b => b.VenueID == viewModel.Booking.VenueID)
                        .ToList();

                    bool hasConflict = existingBookings.Any(b =>
                        viewModel.Booking.StartDate < b.EndDate &&
                        viewModel.Booking.EndDate > b.StartDate);

                    if (hasConflict)
                    {
                        ModelState.AddModelError("", "⛔ This venue is already booked for the selected time range.");
                        return View(viewModel);
                    }

                    // Save the booking
                    db.Bookings.Add(viewModel.Booking);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "✅ Booking created successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "⚠️ A system error occurred: " + ex.Message);
                }
            }

            return View(viewModel);
        }


        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.VenueID = new SelectList(db.Venues, "VenueID", "Name", booking.VenueID);
            ViewBag.EventID = new SelectList(db.Events, "EventID", "Name", booking.EventID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingID,VenueID,EventID,StartDate,EndDate")] Booking booking)
        {
            ViewBag.EventID = new SelectList(db.Events, "EventID", "Name", booking.EventID);
            ViewBag.VenueID = new SelectList(db.Venues, "VenueID", "Name", booking.VenueID);

            if (ModelState.IsValid)
            {
                try
                {
                    // Enhanced conflict detection (excludes current booking)
                    bool isConflict = db.Bookings.Any(b =>
                        b.BookingID != booking.BookingID &&
                        b.VenueID == booking.VenueID &&
                        ((booking.StartDate >= b.StartDate && booking.StartDate < b.EndDate) ||  // New start during existing
                         (booking.EndDate > b.StartDate && booking.EndDate <= b.EndDate) ||    // New end during existing
                         (booking.StartDate <= b.StartDate && booking.EndDate >= b.EndDate))); // Existing within new

                    if (isConflict)
                    {
                        ModelState.AddModelError("", "⚠️ This venue is already booked for the selected time period. Please adjust your booking dates or choose a different venue.");
                        return View(booking);
                    }

                    db.Entry(booking).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "✅ Booking updated successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Only show generic error if no validation errors exist
                    if (!ModelState.Any(m => m.Value.Errors.Count > 0))
                    {
                        ModelState.AddModelError("", "⚠️ We encountered an issue saving your changes. Please try again.");
                    }
                }
            }

            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private bool HasActiveBookingsForVenue(int venueId)
        {
            return db.Bookings.Any(b => b.VenueID == venueId);
        }

        private bool HasActiveBookingsForEvent(int eventId)
        {
            return db.Bookings.Any(b => b.EventID == eventId);
        }

        // GET: Bookings/ManageBookings
        public ActionResult ManageBookings(string searchTerm = null, int? bookingId = null)
        {
            var results = db.Database.SqlQuery<BookingSearchResultVM>(
                "EXEC sp_SearchBookings @SearchTerm, @BookingID, @EventName, @DateFrom, @DateTo",
                new SqlParameter("@SearchTerm", searchTerm != null ? (object)searchTerm : DBNull.Value),
                new SqlParameter("@BookingID", bookingId.HasValue ? (object)bookingId.Value : DBNull.Value),
                new SqlParameter("@EventName", DBNull.Value),
                new SqlParameter("@DateFrom", DBNull.Value),
                new SqlParameter("@DateTo", DBNull.Value)
            ).ToList();

            return View(results);
        }

        // 2. Keep the existing Dispose method below
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
