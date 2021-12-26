using System;
using System.Linq;
using Web.Models;

namespace Web.ViewModels
{
    public class StaffInfoModel
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public string Email { get; set; }

        public string Department { get; set; }

        public string Title { get; set; }

        internal static StaffInfoModel From(Staff staff)
        {
            var infoModel = new StaffInfoModel
            {
                Id = staff.Id,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                BirthDate = staff.BirthDate,
                Email = staff.Email
                //Title = staff.Title?.Name,
                //Department = staff.Title?.Department?.Name
            };

            if (staff.StaffTitles != null && staff.StaffTitles.Count > 0)
            {
                var lastStaffTitle = staff.StaffTitles.OrderByDescending(x => x.StartDate).First();
                if (lastStaffTitle != null && lastStaffTitle.Title != null)
                {
                    infoModel.Title = lastStaffTitle.Title.Name;

                    if (lastStaffTitle.Title.Department != null)
                    {
                        infoModel.Department = lastStaffTitle.Title.Department.Name;
                    }
                }
            }


            return infoModel;
        }
    }
}