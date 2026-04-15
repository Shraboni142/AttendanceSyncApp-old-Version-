using AttendanceSyncApp.Models.DTOs.GpsSystem;
using AttendanceSyncApp.Services;
using AttendanceSyncApp.Services.Interfaces;
using System;
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
        public ActionResult LiveTracking()
        {
            ViewBag.Title = "Employee Live Tracking";
            return View("~/Views/GpsTracking/LiveTracking.cshtml");
        }
        public JsonResult GetCurrentLiveLocations()
        {
            try
            {
                var data = _gpsSystemService.GetCurrentLiveLocations();

                return Json(new
                {
                    success = true,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetTrackerUserByMobileNo(string mobileNo)
        {
            var data = _gpsSystemService.GetTrackerUserByMobileNo(mobileNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveLiveLocation(GpsLiveLocationSaveDto dto)
        {
            try
            {
                dto.IpAddress = Request.UserHostAddress;
                dto.DeviceInfo = Request.UserAgent;

                var success = _gpsSystemService.SaveLiveLocation(dto);

                return Json(new
                {
                    success = success,
                    message = success ? "Live location saved successfully." : "Failed to save live location."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
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