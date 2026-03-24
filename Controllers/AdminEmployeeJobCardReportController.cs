using System.Web.Mvc;
using AttendanceSyncApp.Models.DTOs.Reports;
using AttendanceSyncApp.Services;
using AttendanceSyncApp.Services.Interfaces;

namespace AttendanceSyncApp.Controllers
{
    public class AdminEmployeeJobCardReportController : Controller
    {
        private readonly IAdminEmployeeJobCardReportService _service;

        public AdminEmployeeJobCardReportController()
        {
            _service = new AdminEmployeeJobCardReportService();
        }

        public ActionResult ServerSelection()
        {
            return View();
        }

        public ActionResult DatabaseSelection(int serverId)
        {
            Session["JobCardServerId"] = serverId;

            var databases = _service.GetDatabasesForServer(serverId);

            return View(databases);
        }
        public ActionResult Parameter(int serverId, string databaseName)
        {
            Session["JobCardServerId"] = serverId;
            Session["JobCardDatabaseName"] = databaseName;

            var employees = _service.GetEmployees(serverId, databaseName);
            ViewBag.ServerId = serverId;
            ViewBag.DatabaseName = databaseName;

            return View(employees);
        }
        [HttpPost]
        public ActionResult GenerateReport(EmployeeJobCardReportFilterDto model)
        {
            var header = _service.GetHeaderData(
                model.ServerId,
                model.DatabaseName,
                model.EmployeeId,
                model.FromDate.ToString("dd-MM-yyyy"),
                model.ToDate.ToString("dd-MM-yyyy")
            );

            var details = _service.GetDetailData(
                model.ServerId,
                model.DatabaseName,
                model.EmployeeId,
                model.FromDate.ToString("yyyy-MM-dd"),
                model.ToDate.ToString("yyyy-MM-dd")
            );

            var summary = _service.GetSummaryData(details);

            ViewBag.Header = header;
            ViewBag.Details = details;
            ViewBag.Summary = summary;

            return View("ReportViewer");
        }
    }
}