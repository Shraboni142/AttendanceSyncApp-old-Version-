using AttendanceSyncApp.Models.DTOs.ComplianceAction;
using AttendanceSyncApp.Services;
using AttendanceSyncApp.Services.Interfaces;
using System;
using System.IO;
using System.Web.Mvc;

namespace AttendanceSyncApp.Controllers
{
    public class AdminComplianceActionController : Controller
    {
        private readonly IAdminComplianceActionService _service;

        public AdminComplianceActionController()
        {
            _service = new AdminComplianceActionService();
        }

        public ActionResult Create()
        {
            ViewBag.Title = "Add Compliance Action Information";
            ViewBag.ActiveMenu = "ComplianceAction";

            return View("~/Views/AdminComplianceAction/Create.cshtml", new ComplianceActionCreateDto());
        }
        public ActionResult Index()
        {
            ViewBag.Title = "Compliance Action List";
            ViewBag.ActiveMenu = "ComplianceAction";

            var data = _service.GetAllComplianceActions();

            return View("~/Views/AdminComplianceAction/Index.cshtml", data);
        }
        public ActionResult Edit(int id)
        {
            ViewBag.Title = "Edit Compliance Action Information";
            ViewBag.ActiveMenu = "ComplianceAction";

            var data = _service.GetComplianceActionById(id);

            if (data == null)
            {
                return RedirectToAction("Index");
            }

            return View("~/Views/AdminComplianceAction/Create.cshtml", data);
        }
        [HttpPost]
        public JsonResult SaveComplianceAction(ComplianceActionCreateDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.OffenceType))
                    return Json(new { success = false, message = "Offence Type is required." });

                if (string.IsNullOrWhiteSpace(dto.ComplianceActionType))
                    return Json(new { success = false, message = "Compliance Action Type is required." });

                if (dto.DateOfNotice == DateTime.MinValue)
                    return Json(new { success = false, message = "Date of Notice is required." });

                if (dto.AttachmentFile != null && dto.AttachmentFile.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/Uploads/ComplianceAction/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string fileName = Path.GetFileName(dto.AttachmentFile.FileName);
                    string uniqueFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + fileName;
                    string fullPath = Path.Combine(folderPath, uniqueFileName);

                    dto.AttachmentFile.SaveAs(fullPath);

                    dto.AttachmentFileName = fileName;
                    dto.AttachmentFilePath = "/Uploads/ComplianceAction/" + uniqueFileName;
                }
                else
                {
                    if (dto.Id > 0)
                    {
                        var oldData = _service.GetComplianceActionById(dto.Id);
                        if (oldData != null)
                        {
                            dto.AttachmentFileName = oldData.AttachmentFileName;
                            dto.AttachmentFilePath = oldData.AttachmentFilePath;
                        }
                    }
                }

                dto.CreatedBy = User.Identity.Name;

                bool result;

                if (dto.Id > 0)
                {
                    result = _service.UpdateComplianceAction(dto);
                }
                else
                {
                    result = _service.SaveComplianceAction(dto);
                }

                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = dto.Id > 0 ? "Compliance Action updated successfully." : "Compliance Action saved successfully."
                    });
                }

                return Json(new { success = false, message = "Save failed." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}