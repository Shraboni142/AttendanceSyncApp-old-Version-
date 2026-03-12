using System.Collections.Generic;
using System.Web.Mvc;
using AttendanceSyncApp.Models.DTOs.Reports;
using AttendanceSyncApp.Services;
using AttendanceSyncApp.Services.Interfaces;

namespace AttendanceSyncApp.Controllers
{
    public class AdminEmployeeStatusReportController : Controller
    {

        private readonly IAdminEmployeeStatusReportService _service;

        public AdminEmployeeStatusReportController()
        {
            _service = new AdminEmployeeStatusReportService();
        }

        public ActionResult ServerSelection()
        {
            return View();
        }

        public ActionResult DatabaseSelection(int serverId)
        {
            Session["ServerId"] = serverId;
            return View();
        }

        public ActionResult Parameter()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ShowReport(EmployeeStatusReportFilterDto model)
        {
            int serverId = (int)Session["ServerId"];

            var data = _service.GetEmployeeStatusReport(
                serverId,
                model.SelectedDatabases,
                model.Status);

            return View("EmployeeStatusReportResult", data);
        }
    }
}