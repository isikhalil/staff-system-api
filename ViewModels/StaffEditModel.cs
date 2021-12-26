using System;

namespace Web
{
    public class StaffEditModel
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get;  set; }

        public string Email { get; set; }

        public long ProvinceId { get;  set; }

        public long TitleId { get;  set; }
    }
}
