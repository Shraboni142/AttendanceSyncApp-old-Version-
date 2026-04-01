using AttendanceSyncApp.Models.DTOs.Reports;
using AttendanceSyncApp.Services;
using AttendanceSyncApp.Services.Interfaces;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Web.Mvc;
using System;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
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

        [HttpPost]
        public ActionResult GenerateAllEmployeeHtmlReport(EmployeeJobCardReportFilterDto model)
        {
            var viewModel = _service.GetAllEmployeeHtmlReport(
                model.ServerId,
                model.DatabaseName,
                model.BranchId,
                model.Status,
                model.FromDate.ToString("dd-MM-yyyy"),
                model.ToDate.ToString("dd-MM-yyyy"),
                model.FromDate.ToString("yyyy-MM-dd"),
                model.ToDate.ToString("yyyy-MM-dd")
            );

            return View("AllEmployeeHtmlReport", viewModel);
        }
        [HttpGet]
        public FileResult DownloadAllEmployeeHtmlReportExcel(
    int serverId,
    string databaseName,
    int branchId,
    int status,
    DateTime fromDate,
    DateTime toDate)
        {
            var model = _service.GetAllEmployeeHtmlReport(
                serverId,
                databaseName,
                branchId,
                status,
                fromDate.ToString("dd-MM-yyyy"),
                toDate.ToString("dd-MM-yyyy"),
                fromDate.ToString("yyyy-MM-dd"),
                toDate.ToString("yyyy-MM-dd")
            );

            var sb = new StringBuilder();

            sb.Append("<html><head><meta charset='utf-8' /></head><body>");
            sb.Append("<table>");
            sb.Append("<tr><td colspan='6' style='text-align:center;font-size:20px;font-weight:bold;'>Employee Job Card Report</td></tr>");
            sb.Append("<tr><td colspan='6' style='text-align:center;'>" + model.CompanyName + "</td></tr>");
            sb.Append("<tr><td colspan='6' style='text-align:center;'>" + model.Address + "</td></tr>");
            sb.Append("<tr><td colspan='6' style='text-align:center;'>Phone: " + model.Phone + ", Fax: " + model.Fax + ", Email: " + model.Email + "</td></tr>");
            sb.Append("<tr><td colspan='6' style='text-align:center;'>From " + model.FromDateText + " to " + model.ToDateText + "</td></tr>");
            sb.Append("<tr></tr>");
            sb.Append("</table>");

            sb.Append("<table border='1' style='border-collapse:collapse;width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th>Employee Name</th>");
            sb.Append("<th>Employee Code</th>");
            sb.Append("<th>Designation</th>");
            sb.Append("<th>Department</th>");
            sb.Append("<th>Present</th>");
            sb.Append("<th>Absent</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<tbody>");

            foreach (var item in model.Rows)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + item.EmployeeName + "</td>");
                sb.Append("<td>" + item.EmployeeCode + "</td>");
                sb.Append("<td>" + item.Designation + "</td>");
                sb.Append("<td>" + item.Department + "</td>");
                sb.Append("<td style='text-align:center;'>" + item.Present + "</td>");
                sb.Append("<td style='text-align:center;'>" + item.Absent + "</td>");
                sb.Append("</tr>");
            }

            sb.Append("</tbody></table>");
            sb.Append("</body></html>");

            byte[] fileBytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(fileBytes, "application/vnd.ms-excel", "EmployeeJobCardReport.xls");
        }

        [HttpGet]
        public FileResult DownloadAllEmployeeHtmlReportPdf(
            int serverId,
            string databaseName,
            int branchId,
            int status,
            DateTime fromDate,
            DateTime toDate)
        {
            var model = _service.GetAllEmployeeHtmlReport(
                serverId,
                databaseName,
                branchId,
                status,
                fromDate.ToString("dd-MM-yyyy"),
                toDate.ToString("dd-MM-yyyy"),
                fromDate.ToString("yyyy-MM-dd"),
                toDate.ToString("yyyy-MM-dd")
            );

            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
                PdfWriter.GetInstance(document, ms);
                document.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);

                var title = new Paragraph("Employee Job Card Report", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                var company = new Paragraph(model.CompanyName ?? "", normalFont);
                company.Alignment = Element.ALIGN_CENTER;
                document.Add(company);

                var address = new Paragraph(model.Address ?? "", normalFont);
                address.Alignment = Element.ALIGN_CENTER;
                document.Add(address);

                var contact = new Paragraph(
                    "Phone: " + (model.Phone ?? "") + ", Fax: " + (model.Fax ?? "") + ", Email: " + (model.Email ?? ""),
                    normalFont);
                contact.Alignment = Element.ALIGN_CENTER;
                document.Add(contact);

                var dateRange = new Paragraph("From " + model.FromDateText + " to " + model.ToDateText, normalFont);
                dateRange.Alignment = Element.ALIGN_CENTER;
                dateRange.SpacingAfter = 15f;
                document.Add(dateRange);

                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 28f, 14f, 16f, 20f, 11f, 11f });

                table.AddCell(new PdfPCell(new Phrase("Employee Name", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Employee Code", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Designation", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Department", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Present", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Absent", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                foreach (var item in model.Rows)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.EmployeeName ?? "", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.EmployeeCode ?? "", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.Designation ?? "", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.Department ?? "", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.Present.ToString(), normalFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.Absent.ToString(), normalFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                }

                document.Add(table);
                document.Close();

                return File(ms.ToArray(), "application/pdf", "EmployeeJobCardReport.pdf");
            }
        }
        [HttpGet]
        public ActionResult PreviewAllEmployeeHtmlReportExcel(
    int serverId,
    string databaseName,
    int branchId,
    int status,
    DateTime fromDate,
    DateTime toDate)
        {
            var model = _service.GetAllEmployeeHtmlReport(
                serverId,
                databaseName,
                branchId,
                status,
                fromDate.ToString("dd-MM-yyyy"),
                toDate.ToString("dd-MM-yyyy"),
                fromDate.ToString("yyyy-MM-dd"),
                toDate.ToString("yyyy-MM-dd")
            );

            return View("AllEmployeeExcelPreview", model);
        }

        [HttpGet]
        public ActionResult PreviewAllEmployeeHtmlReportPdf(
            int serverId,
            string databaseName,
            int branchId,
            int status,
            DateTime fromDate,
            DateTime toDate)
        {
            var model = _service.GetAllEmployeeHtmlReport(
                serverId,
                databaseName,
                branchId,
                status,
                fromDate.ToString("dd-MM-yyyy"),
                toDate.ToString("dd-MM-yyyy"),
                fromDate.ToString("yyyy-MM-dd"),
                toDate.ToString("yyyy-MM-dd")
            );

            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
                PdfWriter.GetInstance(document, ms);
                document.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);

                var title = new Paragraph("Employee Job Card Report", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                var company = new Paragraph(model.CompanyName ?? "", normalFont);
                company.Alignment = Element.ALIGN_CENTER;
                document.Add(company);

                var address = new Paragraph(model.Address ?? "", normalFont);
                address.Alignment = Element.ALIGN_CENTER;
                document.Add(address);

                var contact = new Paragraph(
                    "Phone: " + (model.Phone ?? "") + ", Fax: " + (model.Fax ?? "") + ", Email: " + (model.Email ?? ""),
                    normalFont);
                contact.Alignment = Element.ALIGN_CENTER;
                document.Add(contact);

                var dateRange = new Paragraph("From " + model.FromDateText + " to " + model.ToDateText, normalFont);
                dateRange.Alignment = Element.ALIGN_CENTER;
                dateRange.SpacingAfter = 15f;
                document.Add(dateRange);

                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 28f, 14f, 16f, 20f, 11f, 11f });

                table.AddCell(new PdfPCell(new Phrase("Employee Name", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Employee Code", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Designation", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Department", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Present", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Absent", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                foreach (var item in model.Rows)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.EmployeeName ?? "", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.EmployeeCode ?? "", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.Designation ?? "", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.Department ?? "", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.Present.ToString(), normalFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.Absent.ToString(), normalFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                }

                document.Add(table);
                document.Close();

                Response.AddHeader("Content-Disposition", "inline; filename=EmployeeJobCardReport.pdf");
                return File(ms.ToArray(), "application/pdf");
            }
        }
    }
}