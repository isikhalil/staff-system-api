using System;
using Web.Models;

namespace Web.ViewModels
{
    public class StaffFilterModel
    {
        public long StaffId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public int Age { get; set; }

        public string Province { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }
    }
}