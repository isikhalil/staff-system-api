using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Data;
using Web.Models;
using Web.ViewModels;
using WishaLink.Models;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class StaffController : ControllerBase
    {
        private readonly ILogger<StaffController> mLogger;
        private readonly StaffDbContext mContext;

        public StaffController(
            ILogger<StaffController> logger,
            StaffDbContext dbContext
        )
        {
            mLogger = logger;
            mContext = dbContext;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StaffInfoModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> List()
        {
            var staffs = mContext.Staffs
                .Include(x => x.StaffTitles).ThenInclude(x => x.Title).ThenInclude(x => x.Department)
                .ToList();

            var staffInfos = staffs
                .Select(x => StaffInfoModel.From(x))
                .ToList();

            // Serial log türevi bileşenler dahil edilip dosyaya, harici log araçlarına veya sqle loglanabilir
            mLogger.LogInformation("Staff list");
            
            // Mevcut log tablosu kullanılarak sp aracılığı ile de loglanabilir
            await Entity.ExecAsync("EXEC AddLog @Category, @Message", new { Category = "Staff list", Message = "" });

            return Ok(staffInfos);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StaffFilterModel>), (int)HttpStatusCode.OK)]
        public IActionResult Filter(long titleId, long provinceId, int minAge)
        {
            var staffs = Entity.All<StaffFilterModel>("EXEC FilterStaffs @ProvinceId, @TitleId, @MinAge", new { ProvinceId = provinceId, TitleId = titleId, MinAge = minAge });

            mLogger.LogInformation("Staffs filtered ", titleId, provinceId, minAge);

            return Ok(staffs);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StaffInfoModel>), (int)HttpStatusCode.OK)]
        public IActionResult Info(long staffId)
        {
            var staff = mContext.Staffs
                .Include(x => x.StaffTitles).ThenInclude(x => x.Title).ThenInclude(x => x.Department)
                .FirstOrDefault(x => x.Id == staffId);

            if (staff != null)
            {
                mLogger.LogInformation("Staff info", @staff.Id);

                var info = StaffInfoModel.From(staff);
                return Ok(info);
            }
            
            mLogger.LogInformation("Staff not found ", @staff.Id);

            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<StaffInfoModel>), (int)HttpStatusCode.OK)]
        public IActionResult Create([FromBody] StaffEditModel model)
        {
            var staff = Entity.One<Staff>("EXEC CreateStaff @FirstName, @LastName, @BirthDate, @Email, @ProvinceId, @TitleId",
                new
                {
                    model.FirstName,
                    model.LastName,
                    model.BirthDate,
                    model.Email,
                    model.ProvinceId,
                    model.TitleId
                }
            );

            mLogger.LogInformation("Staff created ", @staff.Id);

            return Ok(StaffInfoModel.From(staff));
        }

        [HttpPut]
        [ProducesResponseType(typeof(IEnumerable<StaffInfoModel>), (int)HttpStatusCode.OK)]
        public IActionResult Update([FromBody] StaffEditModel model)
        {
            var existsStaff = mContext.Staffs
                .FirstOrDefault(x => x.Id == model.Id);

            if (existsStaff == null) return NotFound();

            var staff = Entity.One<Staff>("EXEC UpdateStaff @Id, @FirstName, @LastName, @BirthDate, @Email, @ProvinceId, @TitleId",
                new
                {
                    model.Id,
                    model.FirstName,
                    model.LastName,
                    model.BirthDate,
                    model.Email,
                    model.ProvinceId,
                    model.TitleId
                }
            );

            mLogger.LogInformation("Staff updated ", JsonConvert.SerializeObject(model));

            return Ok(StaffInfoModel.From(staff));
        }

        [HttpDelete]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(long staffId)
        {
            var staff = mContext.Staffs
                .FirstOrDefault(x => x.Id == staffId);

            if (staff == null) return NotFound();

            mLogger.LogInformation("Staff deleted ", JsonConvert.SerializeObject(staff));

            mContext.Staffs.Remove(staff);
            await mContext.SaveChangesAsync();

            return Ok();
        }
    }
}
