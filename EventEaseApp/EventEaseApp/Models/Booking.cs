namespace EventEaseApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Booking
    {
        public int BookingID { get; set; }

        [Required(ErrorMessage = "Please select a venue.")]
        public int VenueID { get; set; }

        [Required(ErrorMessage = "Please select an event.")]
        public int EventID { get; set; }

        [Required(ErrorMessage = "Please enter a booking start date.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Please enter a booking end date.")]
        public DateTime EndDate { get; set; }

        public virtual Event Event { get; set; }

        public virtual Venue Venue { get; set; }
    }
}
