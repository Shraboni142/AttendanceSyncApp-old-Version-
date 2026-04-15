using System.Web.Mvc;
using AttendanceSyncApp.Models.DTOs.Payroll;
using AttendanceSyncApp.Services.Interfaces.Payroll;
using AttendanceSyncApp.Services.Payroll;

namespace AttendanceSyncApp.Controllers
{
    public class AdminBonusTypeController : Controller
    {
        private readonly IBonusTypeService _service;

        public AdminBonusTypeController()
        {
            _service = new BonusTypeService();
        }

        public ActionResult Index()
        {
            ViewBag.ActiveMenu = "BonusType";
            var data = _service.GetAll();
            return View("~/Views/AdminBonusType/Index.cshtml", data);
        }

        public ActionResult Create()
        {
            ViewBag.ActiveMenu = "BonusType";
            return View("~/Views/AdminBonusType/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BonusTypeDto dto)
        {
            ViewBag.ActiveMenu = "BonusType";

            if (!ModelState.IsValid)
                return View("~/Views/AdminBonusType/Create.cshtml", dto);

            var success = _service.Create(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Bonus Type saved successfully.";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Save failed.";
            return View("~/Views/AdminBonusType/Create.cshtml", dto);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.ActiveMenu = "BonusType";
            var data = _service.GetById(id);
            if (data == null) return HttpNotFound();

            return View("~/Views/AdminBonusType/Edit.cshtml", data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BonusTypeDto dto)
        {
            ViewBag.ActiveMenu = "BonusType";

            if (!ModelState.IsValid)
                return View("~/Views/AdminBonusType/Edit.cshtml", dto);

            var success = _service.Update(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Bonus Type updated successfully.";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Update failed.";
            return View("~/Views/AdminBonusType/Edit.cshtml", dto);
        }

        public ActionResult Details(int id)
        {
            ViewBag.ActiveMenu = "BonusType";
            var data = _service.GetById(id);
            if (data == null) return HttpNotFound();

            return View("~/Views/AdminBonusType/Details.cshtml", data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var success = _service.Delete(id);

            if (success)
                TempData["SuccessMessage"] = "Bonus Type deleted successfully.";
            else
                TempData["ErrorMessage"] = "Delete failed.";

            return RedirectToAction("Index");
        }
    }
}