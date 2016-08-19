using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eCheck3.Models;

namespace eCheck3.Controllers
{
    public class GroupController : Controller
    {
        private EMSDataAccessEntities db = new EMSDataAccessEntities();

        // GET: Group
        public ActionResult Index()
        {
            return View(db.tbAccess_Group.ToList());
        }

        // GET: Group/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbAccess_Group tbAccess_Group = db.tbAccess_Group.Find(id);
            if (tbAccess_Group == null)
            {
                return HttpNotFound();
            }
            return View(tbAccess_Group);
        }

        // GET: Group/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Group/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,GroupName,GroupDescription,CompanyID,DisplayOrder,IsActive")] tbAccess_Group tbAccess_Group)
        {
            if (ModelState.IsValid)
            {
                db.tbAccess_Group.Add(tbAccess_Group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbAccess_Group);
        }

        // GET: Group/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbAccess_Group tbAccess_Group = db.tbAccess_Group.Find(id);
            if (tbAccess_Group == null)
            {
                return HttpNotFound();
            }
            return View(tbAccess_Group);
        }

        // POST: Group/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,GroupName,GroupDescription,CompanyID,DisplayOrder,IsActive")] tbAccess_Group tbAccess_Group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbAccess_Group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbAccess_Group);
        }

        // GET: Group/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbAccess_Group tbAccess_Group = db.tbAccess_Group.Find(id);
            if (tbAccess_Group == null)
            {
                return HttpNotFound();
            }
            return View(tbAccess_Group);
        }

        // POST: Group/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbAccess_Group tbAccess_Group = db.tbAccess_Group.Find(id);
            db.tbAccess_Group.Remove(tbAccess_Group);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
