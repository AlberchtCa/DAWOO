using DAWOO.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWOO.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Article
        [Authorize(Roles = "User,Guest,Administrator")]
        public ActionResult Index()
        {
            var photo = db.Photos.Include("Category").Include("User");

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            ViewBag.Articles = photo;

            return View();
        }

        [Authorize(Roles = "User,Guest,Administrator")]
        public ActionResult Show(int id)
        {
            Photo photo = db.Photos.Find(id);

            ViewBag.afisareButoane = false;
            if (photo.UserId == User.Identity.GetUserId() ||
                User.IsInRole("Administrator"))
            {
                ViewBag.afisareButoane = true;
            }

            ViewBag.esteAdmin = User.IsInRole("Administrator");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();


            return View(photo);

        }

        [Authorize(Roles = "User,Administrator")]
        public ActionResult New()
        {
            Photo photo = new Photo();

            // preluam lista de categorii din metoda GetAllCategories()
            photo.Categories = GetAllCategories();

            // Preluam ID-ul utilizatorului curent
            photo.UserId = User.Identity.GetUserId();


            return View(photo);

        }

        [HttpPost]
        [Authorize(Roles = "User,Administrator")]
        public ActionResult New(Photo photo)
        {
            photo.Categories = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Photos.Add(photo);
                    db.SaveChanges();
                    TempData["message"] = "Poza a fost adaugata!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(photo);
                }
            }
            catch (Exception e)
            {
                return View(photo);
            }
        }

        [Authorize(Roles = "User,Administrator")]
        public ActionResult Edit(int id)
        {
            Photo photo = db.Photos.Find(id);
            ViewBag.Article = photo;
            photo.Categories = GetAllCategories();

            if (photo.UserId == User.Identity.GetUserId() ||
                User.IsInRole("Administrator"))
            {
                return View(photo);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei poze care nu va apartine!";
                return RedirectToAction("Index");
            }

        }

        [HttpPut]
        [Authorize(Roles = "User,Administrator")]
        public ActionResult Edit(int id, Photo requestPhoto)
        {
            requestPhoto.Categories = GetAllCategories();

            try
            {
                if (ModelState.IsValid)
                {
                    Photo photo = db.Photos.Find(id);
                    if (photo.UserId == User.Identity.GetUserId() ||
                        User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(photo))
                        {
                            photo.Description = requestPhoto.Description;
                            photo.PostDate = requestPhoto.PostDate;
                            photo.CategoryId = requestPhoto.CategoryId;
                            photo.Image = requestPhoto.Image;
                            db.SaveChanges();
                            TempData["message"] = "Poza a fost modificata!";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei poze care nu va apartine!";
                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    return View(requestPhoto);
                }

            }
            catch (Exception e)
            {
                return View(requestPhoto);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "User,Administrator")]
        public ActionResult Delete(int id)
        {
            Photo photo = db.Photos.Find(id);
            if (photo.UserId == User.Identity.GetUserId() ||
                User.IsInRole("Administrator"))
            {
                db.Photos.Remove(photo);
                db.SaveChanges();
                TempData["message"] = "Poza a fost stearsa!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti o poza care nu va apartine!";
                return RedirectToAction("Index");
            }

        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();

            // Extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // Adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }

            // returnam lista de categorii
            return selectList;
        }
    }
}