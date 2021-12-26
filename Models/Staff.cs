using System;
using System.Collections.Generic;

namespace Web.Models
{
    public class Staff
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime BirthDate { get; set; }

        public long ProvinceId { get; set; }
        public virtual Province Province { get; set; }

        public virtual List<StaffTitle> StaffTitles { get; set; }
    }
}
