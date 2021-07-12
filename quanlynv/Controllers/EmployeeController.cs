using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quanlynv.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
namespace quanlynv.Controllers
{
    public class EmployeeController : Controller
    {
        private qlnvContext _context;
        public EmployeeController(qlnvContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Employees);
        }
        public IActionResult Create()
        {

            return View(new Employee());
        }
        [HttpPost]
        public IActionResult Create([FromForm] Employee model)
        {
            model.Created = DateTime.Now;
            _context.Employees.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Detail(int id)
        {
            var employee = _context.Employees.Where(o => o.Id == id).FirstOrDefault();

            return View(employee);
        }
        public IActionResult Edit(int id)
        {
            var employee = _context.Employees.Where(o => o.Id == id).FirstOrDefault();

            return View(employee);
        }
        [HttpPost]
        public IActionResult Edit(int id, [FromForm] Employee model)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Update(model);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }
        public IActionResult Delete(int id)
        {
            var employee = _context.Employees.Where(o => o.Id == id).FirstOrDefault();

            return View(employee);
        }
        [HttpPost]
        public IActionResult Delete(string btn, int id)
        {
            var employee = _context.Employees.Where(o => o.Id == id).FirstOrDefault();
            if (btn == "Delete")
            {

                _context.Employees.Remove(employee);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employee);
        }
        public IActionResult EditPicture(int id)
        {
            ViewBag.ImageUrl = null;
            var empPict = this._context.EmployeePictures.Where(a => a.EmpId == id).FirstOrDefault();
            if (empPict == null)
                empPict = new EmployeePicture { EmpId = id };
            else
            {
                string imgString = Convert.ToBase64String(empPict.ImageData);
                string imageDataURL = string.Format("data:{0};base64,{1}", empPict.ImageType, imgString);

                ViewBag.ImageUrl = imageDataURL;
            }

            return View(empPict);
        }
        [HttpPost]
        public async Task<IActionResult> EditPicture([FromForm] EmployeePicture model, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (files.Count > 0)
                {
                    var formFile = files[0];

                    MemoryStream ms = new();
                    await formFile.CopyToAsync(ms);

                    model.ImageData = ms.ToArray();
                    model.ImageType = formFile.ContentType;
                }
                model.Created = DateTime.Now;

                var empPict = this._context.EmployeePictures.Where(a => a.EmpId == model.EmpId).FirstOrDefault();
                if (empPict == null)
                {
                    this._context.EmployeePictures.Add(model);
                    this._context.SaveChanges();
                }
                else if (empPict != null && ModelState.IsValid) 
                {
                    this._context.EmployeePictures.Update(model);
                    this._context.SaveChanges(model);
                }

                return RedirectToAction("ImageDetails", new { id = model.EmpId });
            }
            return View(model);
        }
        public IActionResult ImageDetails(int id)
        {
            ViewBag.ImageUrl = null;
            var empPict = this._context.EmployeePictures.Where(a => a.EmpId == id).FirstOrDefault();
            if (empPict != null)
            {
                string imgString = Convert.ToBase64String(empPict.ImageData);
                string imageDataURL = string.Format("data:{0};base64,{1}", empPict.ImageType, imgString);

                ViewBag.ImageUrl = imageDataURL;
            }

            var emp = this._context.Employees.Where(a => a.Id == id).FirstOrDefault();

            return View(emp);
        }
    }
   
}
