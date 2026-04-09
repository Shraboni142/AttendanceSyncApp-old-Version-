using AttendanceSyncApp.Models.DTOs.GpsSystem;
using AttendanceSyncApp.Services;
using AttendanceSyncApp.Services.Interfaces;
using System.Web.Mvc;

namespace AttendanceSyncApp.Controllers
{
    public class GpsTrackingController : Controller
    {
        private readonly IGpsSystemService _gpsSystemService;

        public GpsTrackingController()
        {
            _gpsSystemService = new GpsSystemService();
        }

        public ActionResult Index()
        {
            ViewBag.Title = "GPS Tracking";
            return View();
        }

        [HttpGet]
        public JsonResult GetTrackerUserByMobileNo(string mobileNo)
        {
            var data = _gpsSystemService.GetTrackerUserByMobileNo(mobileNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveFieldVisit(GpsFieldVisitSaveDto dto)
        {
            if (dto == null)
            {
                return Json(new { success = false, message = "Invalid request." });
            }

            if (string.IsNullOrWhiteSpace(dto.MobileNo))
            {
                return Json(new { success = false, message = "Mobile No is required." });
            }

            if (dto.TrackerUserId <= 0)
            {
                return Json(new { success = false, message = "Invalid tracker user." });
            }

            string deviceInfo = Request.UserAgent;
            string ipAddress = Request.UserHostAddress;

            var success = _gpsSystemService.SaveFieldVisit(dto, deviceInfo, ipAddress);

            return Json(new
            {
                success = success,
                message = success ? "Field visit saved successfully." : "Failed to save field visit."
            });
        }
    }
}