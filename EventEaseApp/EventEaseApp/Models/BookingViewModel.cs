using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventEaseApp.Models
{
    public class BookingViewModel
    {
        // Initialize Booking to prevent null reference exceptions
        public Booking Booking { get; set; } = new Booking();

        // Collections for Events and Venues dropdowns
        public IEnumerable<Event> Events { get; set; }
        public IEnumerable<Venue> Venues { get; set; }
    }
}