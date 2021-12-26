using System;

namespace Web.Models
{
    public class StaffTitle
    {
        public long Id { get; set; }

        public long StaffId { get; set; }
        public virtual Staff Staff { get; set; }

        public long TitleId { get; set; }
        public virtual Title Title { get; set; }

        public DateTime StartDate { get; set; }
    }
}