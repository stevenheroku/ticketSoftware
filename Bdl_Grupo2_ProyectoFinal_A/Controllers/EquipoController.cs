using Bdl_Grupo2_ProyectoFinal_A.Conexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.Mvc;

namespace Bdl_Grupo2_ProyectoFinal_A.Controllers
{
    [Authorize]
    public class EquipoController : Controller
    {
        private Bdl_ProyectoFinal_AEntities2 db = new Bdl_ProyectoFinal_AEntities2();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CrearEquipo()
        {
            if (!(Session["UserSession"] is null))
            {
                GenerateNewCode generate = new GenerateNewCode();
                ClsConexion con = new ClsConexion();
                Equipos eq = new Equipos();
                string query = "SELECT Fabric_Id FROM Fabricante";
                var reader1 = con.ConsultaBd(query);
                eq.Eqs_Codigo = generate.codigofinal("Equipos", "Eqs_Codigo");
                if (reader1.Read())
                {
                    eq.Hardware = new Hardware();
                    eq.Hardware.Fabricante = new Fabricante();
                    eq.Hardware.Fabric_Id = Convert.ToInt32(reader1["Fabric_Id"].ToString());
                    eq.Usr_Id = Convert.ToInt32(Session["UserSession"]);
                    eq.Sfw_Id = 1;
                }
                ViewBag.Fabric_Id = new SelectList(db.Fabricante, "Fabric_Id", "Fabric_Nombre", eq.Hardware.Fabric_Id);
                ViewBag.Sfw_Id = new SelectList(db.Software, "Sfw_Id", "Sfw_Nombre", eq.Sfw_Id);
                con.cerrarConexion();
                return View(eq);
            }
            else
            {
                FormsAuthentication.SignOut();
                Session.RemoveAll();
                return RedirectToAction("Index", "Usuarios");
            }
        }

        [HttpPost]
        public ActionResult CrearEquipo([Bind(Include = "Sfw_Id,Fabric_Id")] Equipos equipos, string Hdw_Serie, 
            string Hdw_Modelo, string Eqs_Codigo, string Usr_Id, string Fabric_Id, string Sfw_Id)
        {
            if (!(Session["UserSession"] is null))
            {
                SendEmail sendEmail = new SendEmail();
                ClsConexion con = new ClsConexion();
                try
                {
                    string query = "SELECT Hdw_Serie FROM Hardware WHERE Hdw_Serie = '" + Hdw_Serie + "'";
                    var oReader = con.ConsultaBd(query);
                    if (oReader.Read())
                    {
                        Equipos eq = new Equipos();
                        eq.Hardware = new Hardware();
                        eq.Hardware.Fabricante = new Fabricante();
                        eq.Hardware.Fabric_Id = Convert.ToInt32(Fabric_Id);
                        eq.Usr_Id = Convert.ToInt32(Usr_Id);
                        eq.Sfw_Id = Convert.ToInt32(Sfw_Id);
                        ViewBag.Fabric_Id = new SelectList(db.Fabricante, "Fabric_Id", "Fabric_Nombre", eq.Hardware.Fabric_Id);
                        ViewBag.Sfw_Id = new SelectList(db.Software, "Sfw_Id", "Sfw_Nombre", eq.Sfw_Id);
                        eq.aceptado = 1;
                        con.cerrarConexion();
                        return View(eq);
                    }
                    else
                    {
                        query = "INSERT INTO Hardware VALUES ('" + Hdw_Modelo + "', '" + Hdw_Serie + "', " + Fabric_Id + ")";
                        con.ConsultaBd(query);
                        con.cerrarConexion();
                        query = "SELECT Hdw_Id FROM Hardware WHERE Hdw_Serie = '" + Hdw_Serie + "'";
                        oReader = con.ConsultaBd(query);
                        if (oReader.Read())
                        {
                            query = "INSERT INTO Equipos VALUES('" + Eqs_Codigo + "', " + oReader["Hdw_Id"].ToString() + ", " + Sfw_Id + ", " + Usr_Id + ")";
                            con.ConsultaBd(query);
                            con.cerrarConexion();
                            query = "SELECT Usr_Correo FROM Usuarios WHERE Usr_Id = " + Usr_Id;
                            oReader = con.ConsultaBd(query);
                            if (oReader.Read())
                            {
                                sendEmail.sendMail(oReader["Usr_Correo"].ToString(), "Nuevo Equipo", "", Eqs_Codigo, "Nuevo código de Equipo:");
                            }
                            return RedirectToAction("Crear", "Ticket");
                        }
                        else
                        {
                            return RedirectToAction("CrearEquipo", "Equipo");
                        }
                    }
                }
                catch
                {
                    return RedirectToAction("CrearEquipo", "Equipo");
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                Session.RemoveAll();
                return RedirectToAction("Index", "Usuarios");
            }

        }
    }
}
