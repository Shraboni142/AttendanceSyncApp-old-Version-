using AttendanceSyncApp.Models.DTOs.Reports;
using AttendanceSyncApp.Services;
using AttendanceSyncApp.Services.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

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

            List<string> databases = new List<string>();

            string connectionString =
                "Server=192.168.26.242;Database=master;User Id=sa;Password=open;TrustServerCertificate=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT name
                    FROM sys.databases
                    WHERE name LIKE 'Smart_%'
                    ORDER BY name";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        databases.Add(reader["name"].ToString());
                    }
                }
            }

            return View(databases);
        }

        [HttpGet]
        public ActionResult Parameter(string databaseName)
        {
            Session["SelectedDatabases"] = new string[] { databaseName };
            return View();
        }

        [HttpPost]
        public ActionResult ShowReport(EmployeeStatusReportFilterDto model)
        {
            int serverId = (int)Session["ServerId"];
            var selectedDatabases = Session["SelectedDatabases"] as string[];

            var data = _service.GetEmployeeStatusReport(
                serverId,
                selectedDatabases != null
                    ? new List<string>(selectedDatabases)
                    : new List<string>(),
                model.Status);

            return View("EmployeeStatusReportResult", data);
        }
    }
}