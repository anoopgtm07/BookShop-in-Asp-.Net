using Demo.DataAccess.Data;
using Demo.DataAccess.Repository;
using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Demo.Models.ViewModels;


namespace Demo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        //this constructor is receiving two dependencies IUnitOfWork and IWebHostEnvironment

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BookController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Book> objBookList = _unitOfWork.Book.GetAll().ToList();
            
            return View(objBookList);
        }
        public IActionResult Upsert(int? id)
        {
            BookVM bookVM = new BookVM()
            {
                CategoryList = _unitOfWork.Category.
               GetAll().Select(u => new SelectListItem
               {
                   Text = u.Name,
                   Value = u.Id.ToString(),
               }),
                Book = new Book()
            };
            if (id == null || id == 0)
            { 
                //create
                return View(bookVM);
            }
            else
            {
                //update
                bookVM.Book = _unitOfWork.Book.Get(u => u.Id == id);
                return View(bookVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(BookVM bookVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string bookPath = Path.Combine(wwwRootPath, @"images\book");

                    if (bookVM.Book.ImageUrl != null)
                    {
                        //delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, bookVM.Book.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(bookPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    bookVM.Book.ImageUrl = @"\images\book\" + fileName;
                }

                _unitOfWork.Book.Update(bookVM.Book);

                _unitOfWork.Save();
                var successMessage = $" '{bookVM.Book.Title}' was updated successfully";
                TempData["success"] = successMessage;
                return RedirectToAction("Index");
            }
            else
            {
                bookVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });
                return View(bookVM);
            }
        }



        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Book? bookFromDb = _unitOfWork.Book.Get(u => u.Id == id);

        //    if (bookFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(bookFromDb);
        //}

        //[HttpPost]
        //public IActionResult Edit(Book obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Book.Update(obj);
        //        _unitOfWork.Save();
        //        var successMessage = $" '{obj.Title}' was updated successfully";
        //        TempData["success"] = successMessage;
        //        return RedirectToAction("Index");
        //    }
        //    return View();

        //}


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Book? bookFromDb = _unitOfWork.Book.Get(u => u.Id == id);
            if (bookFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.Book.Remove(bookFromDb);
            _unitOfWork.Save();
            var successMessage = $" '{bookFromDb.Title}' was deleted successfully";
            TempData["success"] = successMessage;
            return RedirectToAction("Index");

        }

    }
}
