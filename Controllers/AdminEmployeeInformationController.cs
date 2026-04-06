using AttendanceSyncApp.Models.DTOs.EmployeeInformation;
using AttendanceSyncApp.Services;
using AttendanceSyncApp.Services.Interfaces;
using System;
using System.Web.Mvc;

namespace AttendanceSyncApp.Controllers
{
    public class AdminEmployeeInformationController : Controller
    {
        private readonly IAdminEmployeeInformationService _service;

        public AdminEmployeeInformationController()
        {
            _service = new AdminEmployeeInformationService();
        }

        public ActionResult Index(string employeeCode = "")
        {
            ViewBag.EmployeeCode = employeeCode;
            return View();
        }
        public ActionResult EmployeeProfile()
        {
            var data = _service.GetEmployeeProfiles();
            return View(data);
        }

        [HttpGet]
        public ActionResult EditProfile(string employeeCode)
        {
            if (string.IsNullOrWhiteSpace(employeeCode))
            {
                return RedirectToAction("EmployeeProfile");
            }

            return RedirectToAction("Index", new { employeeCode = employeeCode });
        }

        [HttpGet]
        public JsonResult GetEmployees()
        {
            return Json(_service.GetEmployees(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEmployeeGeneralInfo(string employeeCode)
        {
            return Json(_service.GetEmployeeGeneralInfo(employeeCode), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateEmployeeGeneralInfo(EmployeeInfoGeneralDto dto)
        {
            return Json(_service.UpdateEmployeeGeneralInfo(dto));
        }

        [HttpGet]
        public JsonResult GetEmployeeAddressInfo(string employeeCode)
        {
            return Json(_service.GetEmployeeAddressInfo(employeeCode), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateEmployeeAddressInfo(string employeeCode, EmployeeInfoAddressDto dto)
        {
            try
            {
                var success = _service.UpdateEmployeeAddressInfo(employeeCode, dto);

                return Json(new
                {
                    success = success,
                    message = success ? "Address saved successfully." : "Address save failed."
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

        [HttpGet]
        public JsonResult GetEmployeeEducations(int employeeId)
        {
            return Json(_service.GetEmployeeEducations(employeeId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEducationDropdown()
        {
            return Json(_service.GetEducationDropdown(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEducationFieldDropdowns()
        {
            return Json(_service.GetEducationFieldDropdowns(), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetDesignations()
        {
            var data = _service.GetDesignations();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDepartments()
        {
            var data = _service.GetDepartments();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveEducation(string employeeCode, EmployeeEducationDto dto)
        {
            try
            {
                var success = _service.SaveEducation(employeeCode, dto);

                return Json(new
                {
                    success = success,
                    message = success ? "Education saved successfully." : "Education save failed."
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
        public JsonResult DeleteEducation(int id)
        {
            return Json(_service.DeleteEducation(id));
        }
        [HttpPost]
        public JsonResult SaveAllEmployeeInformation(EmployeeFullInformationSaveDto dto)
        {
            try
            {
                var success = _service.SaveAllEmployeeInformation(dto);

                return Json(new
                {
                    success = success,
                    message = success ? "All employee information saved successfully." : "Save failed."
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
    }
}