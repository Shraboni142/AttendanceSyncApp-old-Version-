using AttendanceSyncApp.Models.DTOs.ComplainAction;
using AttendanceSyncApp.Services;
using AttendanceSyncApp.Services.Interfaces;
using System;
using System.Web.Mvc;

namespace AttendanceSyncApp.Controllers
{
    public class AdminComplainActionController : Controller
    {
        private readonly IAdminComplainActionService _adminComplainActionService;

        public AdminComplainActionController()
        {
            _adminComplainActionService = new AdminComplainActionService();
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Complain List";
            ViewBag.ActiveMenu = "ComplainAction";
            ViewBag.IsReviewMode = false;

            var data = _adminComplainActionService.GetAllComplainActions();
            return View(data);
        }

        public ActionResult Review()
        {
            ViewBag.Title = "Employee Complain Review";
            ViewBag.ActiveMenu = "EmployeeComplainReview";
            ViewBag.IsReviewMode = true;

            var data = _adminComplainActionService.GetAllComplainActions();
            return View("Index", data);
        }

        public ActionResult Create()
        {
            ViewBag.Title = "Add Complain Action Information";
            ViewBag.ActiveMenu = "ComplainAction";

            var model = new ComplainActionCreateDto();
            return View(model);
        }
        [HttpPost]
        public JsonResult SaveComplainAction(ComplainActionCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid request data."
                    });
                }

                SaveAttachmentFile(dto);

                var result = _adminComplainActionService.SaveComplainAction(dto);

                return Json(new
                {
                    success = result,
                    message = result ? "Complain action saved successfully." : "Failed to save complain action."
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

        public ActionResult Edit(int id)
        {
            ViewBag.Title = "Edit Complain Action Information";
            ViewBag.ActiveMenu = "ComplainAction";

            var model = _adminComplainActionService.GetComplainActionById(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View("Create", model);
        }

        [HttpPost]
        public JsonResult UpdateComplainAction(ComplainActionCreateDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid request data."
                    });
                }

                if (dto.AttachmentFile != null && dto.AttachmentFile.ContentLength > 0)
                {
                    SaveAttachmentFile(dto);
                }

                var result = _adminComplainActionService.UpdateComplainAction(dto);

                return Json(new
                {
                    success = result,
                    message = result ? "Complain action updated successfully." : "Failed to update complain action."
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
        public JsonResult DeleteComplainAction(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid complain action id."
                    });
                }

                var result = _adminComplainActionService.DeleteComplainAction(id);

                return Json(new
                {
                    success = result,
                    message = result ? "Complain action deleted successfully." : "Failed to delete complain action."
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
        private void SaveAttachmentFile(ComplainActionCreateDto dto)
        {
            if (dto == null || dto.AttachmentFile == null || dto.AttachmentFile.ContentLength <= 0)
            {
                return;
            }

            int maxFileSize = 2 * 1024 * 1024; // 2 MB
            if (dto.AttachmentFile.ContentLength > maxFileSize)
            {
                throw new Exception("File size must be 2 MB or less.");
            }

            string folderPath = Server.MapPath("~/Uploads/ComplainAction/");

            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            string originalFileName = System.IO.Path.GetFileName(dto.AttachmentFile.FileName);
            string uniqueFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + originalFileName;
            string fullPath = System.IO.Path.Combine(folderPath, uniqueFileName);

            dto.AttachmentFile.SaveAs(fullPath);

            dto.AttachmentFileName = originalFileName;
            dto.AttachmentFilePath = "/Uploads/ComplainAction/" + uniqueFileName;
        }

        [HttpPost]
        public JsonResult UpdateComplainReviewStatus(int id, string reviewStatus)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid complain action id."
                    });
                }

                if (string.IsNullOrWhiteSpace(reviewStatus))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Review status is required."
                    });
                }

                var result = _adminComplainActionService.UpdateComplainReviewStatus(id, reviewStatus);

                return Json(new
                {
                    success = result,
                    message = result ? "Review status updated successfully." : "Failed to update review status."
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

        public ActionResult ViewComplain(int id)
        {
            ViewBag.Title = "View Complain Action Information";
            ViewBag.ActiveMenu = "EmployeeComplainReview";

            var model = _adminComplainActionService.GetComplainActionById(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }
    }
}