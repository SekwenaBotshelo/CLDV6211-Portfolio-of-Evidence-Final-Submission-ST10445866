using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventEaseApp.Models;

namespace EventEaseApp.Controllers
{
    public class EventsController : Controller
    {
        private PortfolioOfEvidencePOneDBContext db = new PortfolioOfEvidencePOneDBContext();

        // GET: Events
        public ActionResult Index()
        {
            return View(db.Events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,Name,StartDate,EndDate,Notes")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID,Name,StartDate,EndDate,Notes")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Event eventItem = db.Events.Find(id);
            if (eventItem == null)
            {
                return HttpNotFound();
            }

            // Check for active bookings
            if (HasActiveBookings(eventItem.EventID))
            {
                ViewBag.DeletionError = "⚠️ Cannot delete event - there are active bookings. Please cancel all bookings first.";
            }

            return View(eventItem);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // ❌ Prevent deleting Event if it has active bookings
            Event eventItem = db.Events.Find(id);

            // Final validation check
            if (HasActiveBookings(eventItem.EventID))
            {
                TempData["ErrorMessage"] = "Deletion failed: Event has active bookings";
                return RedirectToAction("Index");
            }

            try
            {
                db.Events.Remove(eventItem);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Event deleted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the event";
                // Log the error (ex) here if needed
                return RedirectToAction("Index");
            }
        }

        // Private helper method to check bookings
        private bool HasActiveBookings(int eventId)
        {
            return db.Bookings.Any(b => b.EventID == eventId && b.EndDate >= DateTime.Today);
        }

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
