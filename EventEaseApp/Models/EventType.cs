using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventEaseApp.Models
{
    public class EventType
    {
        public int EventTypeID { get; set; }

        [Required(ErrorMessage = "Please select an Event Type.")]
        public string TypeName { get; set; }

        // Navigation property to related events
        public virtual ICollection<Event> Events { get; set; }
    }
}