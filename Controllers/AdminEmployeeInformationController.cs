using System.Web.Mvc;
using AttendanceSyncApp.Models.DTOs.EmployeeInformation;
using AttendanceSyncApp.Services;
using AttendanceSyncApp.Services.Interfaces;

namespace AttendanceSyncApp.Controllers
{
    public class AdminEmployeeInformationController : Controller
    {
        private readonly IAdminEmployeeInformationService _service;

        public AdminEmployeeInformationController()
        {
            _service = new AdminEmployeeInformationService();
        }

        public ActionResult Index()
        {
            return View();
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
        public JsonResult UpdateEmployeeAddressInfo(int employeeId, EmployeeInfoAddressDto dto)
        {
            return Json(_service.UpdateEmployeeAddressInfo(employeeId, dto));
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
        [HttpPost]
        public JsonResult SaveEducation(EmployeeEducationDto dto)
        {
            return Json(_service.SaveEducation(dto));
        }

        [HttpPost]
        public JsonResult DeleteEducation(int id)
        {
            return Json(_service.DeleteEducation(id));
        }
    }
}