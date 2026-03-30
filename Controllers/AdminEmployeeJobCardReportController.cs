using AttendanceSyncApp.Models.DTOs.Reports;
using AttendanceSyncApp.Services;
using AttendanceSyncApp.Services.Interfaces;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Web.Mvc;

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

        [HttpGet]
        public ActionResult Parameter(int serverId, string databaseName)
        {
            Session["JobCardServerId"] = serverId;
            Session["JobCardDatabaseName"] = databaseName;

            var employees = _service.GetEmployees(serverId, databaseName);
            ViewBag.ServerId = serverId;
            ViewBag.DatabaseName = databaseName;

            return View(employees);
        }
        [HttpGet]
        public JsonResult GetBranches(int serverId, string databaseName)
        {
            var data = _service.GetBranches(serverId, databaseName);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEmployeesByBranchAndStatus(int serverId, string databaseName, int branchId, int status)
        {
            var data = _service.GetEmployeesByBranchAndStatus(serverId, databaseName, branchId, status);
            return Json(data, JsonRequestBehavior.AllowGet);
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

            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;
            viewer.SizeToReportContent = true;
            viewer.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            viewer.Height = System.Web.UI.WebControls.Unit.Pixel(1200);

            viewer.LocalReport.ReportPath = Server.MapPath("~/Reports/EmployeeJobCardReport.rdlc");
            viewer.LocalReport.DataSources.Clear();

            viewer.LocalReport.DataSources.Add(
                new ReportDataSource("HeaderDataSet", new List<EmployeeJobCardHeaderDto> { header })
            );

            viewer.LocalReport.DataSources.Add(
                new ReportDataSource("DetailDataSet", details)
            );

            viewer.LocalReport.DataSources.Add(
                new ReportDataSource("SummaryDataSet", new List<EmployeeJobCardSummaryDto> { summary })
            );

            viewer.LocalReport.Refresh();

            ViewBag.ReportViewer = viewer;
            return View("ReportViewer");
        }
    }
}