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

        [HttpGet]
        public ActionResult Parameter(string selectedDatabases)
        {
            Session["SelectedDatabases"] = new string[] { selectedDatabases };
            return View();
        }

        [HttpPost]
        public ActionResult ShowReport(EmployeeStatusReportFilterDto model)
        {
            int serverId = (int)Session["ServerId"];
            var selectedDatabases = Session["SelectedDatabases"] as string[];

            var data = _service.GetEmployeeStatusReport(
                serverId,
                selectedDatabases != null ? new System.Collections.Generic.List<string>(selectedDatabases) : new System.Collections.Generic.List<string>(),
                model.Status);

            return View("EmployeeStatusReportResult", data);
        }
    }
}