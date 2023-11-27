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
        private readonly IUnitOfWork _unitOfWork;
        public BookController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Book> objBookList = _unitOfWork.Book.GetAll().ToList();
            
            return View(objBookList);
        }
        public IActionResult Create()
        {  
            BookVM bookVM = new BookVM ()
            {
               CategoryList = _unitOfWork.Category.
               GetAll().Select(u => new SelectListItem
               {
                   Text = u.Name,
                   Value = u.Id.ToString(),
               }),
                Book = new Book()
            };
            return View(bookVM);
        }

        [HttpPost]
        public IActionResult Create(Book obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Book.Add(obj);
                _unitOfWork.Save();
                var successMessage = $" '{obj.Title}' was added successfully";
                TempData["success"] = successMessage;
                return RedirectToAction("Index");
            }
            return View();

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Book? bookFromDb = _unitOfWork.Book.Get(u => u.Id == id);
            //Book? bookFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id == id);
            //Book? bookFromDb2 = _db.Categories.Where(u=>u.Id == id).FirstOrDefault;

            if (bookFromDb == null)
            {
                return NotFound();
            }
            return View(bookFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Book obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Book.Update(obj);
                _unitOfWork.Save();
                var successMessage = $" '{obj.Title}' was updated successfully";
                TempData["success"] = successMessage;
                return RedirectToAction("Index");
            }
            return View();

        }

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
