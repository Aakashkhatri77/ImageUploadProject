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
            if (ModelState.IsValid == true)
            {
                // Save image to wwwroot/image
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(student.ImageFile.FileName);
                string extension = Path.GetExtension(student.ImageFile.FileName);
                IFormFile postedFile = student.ImageFile;
                long length = postedFile.Length;
                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".png" || extension.ToLower() == ".jpeg")
                {
                    if (length <= 1000000)
                    {
                        student.Profile = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await student.ImageFile.CopyToAsync(fileStream);
                        }

                        //Insert Record
                       
                        db.Students.Add(student);
                        int Student = await db.SaveChangesAsync();
                        if (Student > 0)
                        {
                            TempData["CreateMessage"] = "<script>alert('Data Inserted Successfully.')</script>";
                            ModelState.Clear();
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            TempData["CreateMessage"] = "<script>alert('Data Not Inserted.')</script>";

                        }
                    }
                    else
                    {
                        TempData["SizeMessage"] = "<script>alert('Image file should be less than 1mb')</script>";
                    }
                }
                else
                {
                    TempData["ExtentionMessage"] = "<script>alert('Format Not Supported')</script>";
                }

            }
  
            return View(student);

        }

        //Get: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var student = await db.Students.FindAsync(id);
            TempData["imgPath"] = student.Profile;
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (ModelState.IsValid == true)
            {
                if (student.ImageFile != null)
                {
                    // Update image to wwwroot/image
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(student.ImageFile.FileName);
                string extension = Path.GetExtension(student.ImageFile.FileName);
                IFormFile postedFile = student.ImageFile;
                long length = postedFile.Length;
                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".png" || extension.ToLower() == ".jpeg")
                {
                    if (length <= 1000000)
                    {
                        student.Profile = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/Image/" + fileName);
                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                await student.ImageFile.CopyToAsync(fileStream);
                            }

                            db.Entry(student).State = EntityState.Modified;
                            string imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Image/", TempData["imgPath"].ToString());
                            var _student = await db.SaveChangesAsync();

                        if (_student > 0)
                        {
                                /*  var std = await db.Students.FindAsync(id);
                                  var imagePath = std.Profile.ToString();*/
                                /*                                string imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Image/", imagePath);
                                */
                                if (System.IO.File.Exists(imagePath))
                                {
                                    System.IO.File.Delete(imagePath);
                                }
                                TempData["UpdateMessage"] = "<script>alert('Data Updatxed Successfully.')</script>";
                                ModelState.Clear();
                                return RedirectToAction(nameof(Index));
                            }
                            else
                        {
                            TempData["UpdateMessage"] = "<script>alert('Data Not Updated.')</script>";
                        }
                    }
                    else
                    {
                        TempData["SizeMessage"] = "<script>alert('Image Size should be less than 1 MB')</script>";
                            
                    }
                }
                else
                {
                    TempData["ExtensionMessage"] = "<script>alert('Format Not Supported')</script>";
                        return View(student);
                }
            }
                else
                {
                    student.Profile = TempData["imgPath"].ToString();
                    db.Entry(student).State = EntityState.Modified;
                    int Student = await db.SaveChangesAsync();
                    if (Student > 0)
                    {
                        TempData["UpdateMessage"] = "<script>alert('Data Updated Successfully.')</script>";
                        ModelState.Clear();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["UpdateMessage"] = "<script>alert('Data Not Updated.')</script>";
                    }

                }
            }
            return View(student);
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
            var imagePath = student.Profile.ToString();
            imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Image/", imagePath);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            db.Students.Remove(student);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            return View();
        }

    }
}
