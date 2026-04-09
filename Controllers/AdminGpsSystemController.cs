using AttendanceSyncApp.Models.DTOs.GpsSystem;
using AttendanceSyncApp.Services;
using AttendanceSyncApp.Services.Interfaces;
using System;
using System.Web.Mvc;

namespace AttendanceSyncApp.Controllers
{
    public class AdminGpsSystemController : Controller
    {
        private readonly IGpsSystemService _gpsSystemService;

        public AdminGpsSystemController()
        {
            _gpsSystemService = new GpsSystemService();
        }

        public ActionResult TrackerUsers()
        {
            ViewBag.Title = "GPS Tracker Users";
            ViewBag.ActiveMenu = "GpsTrackerUsers";
            return View("~/Views/AdminGpsSystem/TrackerUsers.cshtml");
        }
        public ActionResult VisitHistory()
        {
            ViewBag.Title = "Field Visit History";
            ViewBag.ActiveMenu = "GpsVisitHistory";
            return View("~/Views/AdminGpsSystem/VisitHistory.cshtml");
        }
        public ActionResult LiveMap()
        {
            ViewBag.Title = "Live Tracking Map";
            ViewBag.ActiveMenu = "GpsLiveMap";
            return View("~/Views/AdminGpsSystem/LiveMap.cshtml");
        }
        public ActionResult FieldVisitEntry()
        {
            ViewBag.Title = "Field Visit GPS Entry";
            ViewBag.ActiveMenu = "GpsTrackerUsers";
            return View("~/Views/AdminGpsSystem/FieldVisitEntry.cshtml");
        }

        [HttpGet]
        public JsonResult GetTrackerUsers()
        {
            var data = _gpsSystemService.GetTrackerUsers();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveTrackerUser(GpsTrackerUserDto dto)
        {
            if (dto == null)
            {
                return Json(new { success = false, message = "Invalid request." });
            }

            if (string.IsNullOrWhiteSpace(dto.MobileNo))
            {
                return Json(new { success = false, message = "Mobile No is required." });
            }

            if (string.IsNullOrWhiteSpace(dto.EmployeeName))
            {
                return Json(new { success = false, message = "Employee Name is required." });
            }

            var success = _gpsSystemService.SaveTrackerUser(dto);

            return Json(new
            {
                success = success,
                message = success ? "Tracker user saved successfully." : "This mobile number already exists."
            });
        }

        [HttpGet]
        public JsonResult GetFieldVisitHistory()
        {
            try
            {
                var data = _gpsSystemService.GetFieldVisitHistory();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = true,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}