using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bdl_Grupo2_ProyectoFinal_A;
using Bdl_Grupo2_ProyectoFinal_A.Conexion;

namespace Bdl_Grupo2_ProyectoFinal_A.Controllers
{
    public class UsrTecnicosController : Controller
    {
        private Bdl_ProyectoFinal_AEntities2 db = new Bdl_ProyectoFinal_AEntities2();
        public ActionResult InicioSesion()
        {
            UsrTecnicos usrs = new UsrTecnicos();
            usrs.aceptado = 0;
            return View(usrs);
        }

        [HttpPost]
        public ActionResult SingOut()
        {
            Session["Tec_Id"] = null;
            Session["Prf_Id"] = null;
            return RedirectToAction("InicioSesion", "UsrTecnicos");
        }

        public ActionResult RegistrarseTecnicos()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegistrarTecnico(string DPI)
        {
            ClsConexion con = new ClsConexion();
            SqlDataReader dr, dr2;
            Tecnicos usrs = new Tecnicos();
            if (DPI != null)
            {
                string query = "SELECT Tec_Id, Tec_Nombre1, Tec_Apellido1 FROM Tecnicos WHERE DPI = '" + DPI + "'";
                dr = con.ConsultaBd(query);
                if (dr.Read())
                {
                    usrs.Tec_Id = Convert.ToInt32(dr["Tec_Id"]);
                    string query2 = "SELECT Tec_Id FROM UsrTecnicos WHERE Tec_Id = " + usrs.Tec_Id;
                    dr2 = con.ConsultaBd(query2);
                    if (dr2.Read())
                    {

                        UsrTecnicos usrt = new UsrTecnicos();
                        usrt.aceptado = 3;
                        return View("InicioSesion", usrt);
                    }
                    else
                    {
                        usrs.Tec_Nombre1 = dr["Tec_Nombre1"].ToString() + " " + dr["Tec_Apellido1"].ToString();
                        usrs.aceptado = 0;
                        return View("Registrarse", usrs);
                    }
                }
                else
                {
                    UsrTecnicos usrt = new UsrTecnicos();
                    usrt.aceptado = 2;
                    return View("InicioSesion", usrt);
                }
            }
            else
            {
                return RedirectToAction("InicioSesion", "UsrTecnicos");
            }
        }
        [HttpPost]
        public ActionResult VerificacionTecnico(string usr, string pwd)
        {
            ClsConexion con = new ClsConexion();
            SqlDataReader dr;
            UsrTecnicos usrs = new UsrTecnicos();
            string query = "SELECT Tec_Id, UsrTecnico_Correo, UsrTecnico_Password FROM UsrTecnicos WHERE UsrTecnico_Correo = '" + usr + "' AND UsrTecnico_Password = '" + pwd + "'";
            dr = con.ConsultaBd(query);
            if (dr.Read())
            {
                Session.Add("Tec_Id", dr["Tec_Id"].ToString());
                con.cerrarConexion();
                return RedirectToAction("ListTickets", "Ticket");
            }
            else
            {
                con.cerrarConexion();
                usrs.aceptado = 1;
                return View("InicioSesion", usrs);
            }
        }

        [HttpPost]
        public ActionResult Registrarse(string correo, string pwd, string tec_id)
        {
            SqlDataReader dr;
            ClsConexion con = new ClsConexion();
            string query = "SELECT UsrTecnico_Correo FROM UsrTecnicos WHERE UsrTecnico_Correo = '" + correo + "'";
            dr = con.ConsultaBd(query);

            if (dr.Read())
            {
                con.cerrarConexion();
                Tecnicos usrs = new Tecnicos();
                usrs.aceptado = 1;
                return View("Registrarse", usrs);
            }
            else
            {
                query = "INSERT INTO UsrTecnicos(UsrTecnico_Correo,UsrTecnico_Password,Tec_Id) " +
                "VALUES('" + correo + "','" + pwd + "'," + tec_id + ")";
                con.ConsultaBd(query);
                con.cerrarConexion();
                return RedirectToAction("InicioSesion", "UsrTecnicos");
            }
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
