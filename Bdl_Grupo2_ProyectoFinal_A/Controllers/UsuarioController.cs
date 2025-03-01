using Bdl_Grupo2_ProyectoFinal_A.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bdl_Grupo2_ProyectoFinal_A.Controllers
{
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult Users()
        {

            return View();
        }

        public ActionResult Registrarse()
        {

            return View();
        }

        public ActionResult Login(string email, string password)
        {

           if(!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                

                return View();

            }
            else
            {
                return View();
            }
        }

        // GET: Usuario/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Usuario/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        public ActionResult Create(User collection)
        {
            try
            {
                if (ModelState.IsValid)
                {

                }

                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // GET: Usuario/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Usuario/Delete/5


        // POST: Usuario/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
