using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventEaseApp.ViewModel
{
    public class BookingSearchResultVM
    {
        // Booking
        public int BookingID { get; set; }

        // Event
        public int EventID { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string EventNotes { get; set; }

        // Venue
        public int VenueID { get; set; }
        public string VenueName { get; set; }
        public string VenueLocation { get; set; }
        public int VenueCapacity { get; set; }
        public string VenueImage { get; set; }

        // Booking Dates
        public DateTime BookingStart { get; set; }
        public DateTime BookingEnd { get; set; }

        // Calculated fields
        public int DurationHours { get; set; }  // DATEDIFF(HOUR, Start, End)
        public string BookingStatus { get; set; }  // Completed / Upcoming / Future Booking
    }
}