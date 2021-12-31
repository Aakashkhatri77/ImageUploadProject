using System.IO;
using ImageUploadProject.Context;
using ImageUploadProject.Migrations;
using ImageUploadProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImageUploadProject.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public StudentController(ApplicationDbContext _db, IWebHostEnvironment hostEnvironment)
        {
            this.db = _db;
            this._hostEnvironment = hostEnvironment;   
        }

        //public StudentController(IWebHostEnvironment hostEnvironment)
        //{
        //    this._hostEnvironment = hostEnvironment;
        //}
        public IActionResult Index()
        {
            var data = db.Students.ToList();
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ImageFile")] Student student)
        {

            // Save image to wwwroot/image
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(student.ImageFile.FileName);
            string extension = Path.GetExtension(student.ImageFile.FileName);
            student.Profile = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            string path = Path.Combine(wwwRootPath + "/Image/", fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await student.ImageFile.CopyToAsync(fileStream);
            }

            //Insert Record
            db.Students.Add(student);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            return View(student);
        }

        //Get: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Student student)
        {

            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(student.ImageFile.FileName);
            string extension = Path.GetExtension(student.ImageFile.FileName);
            student.Profile = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            string path = Path.Combine(wwwRootPath + "/Image/", fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await student.ImageFile.CopyToAsync(fileStream);
            }
            //db.Entry(student).State = EntityState.Modified;
            db.Update(student);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        //Get: Details
        public IActionResult Details(int id)
        {
            var student = db.Students.Find(id);
            return View(student);
        }

        //Get:Image
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var student = await db.Students.FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await db.Students.FindAsync(id);
            db.Students.Remove(student);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            return View();
        }

    }
}
