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
    public class TecnicosController : Controller
    {
        private Bdl_ProyectoFinal_AEntities2 db = new Bdl_ProyectoFinal_AEntities2();

        public ActionResult ListaTecnicos()
        {
            if (Convert.ToInt32(Session["Prf_Id"]) == 2)
            {
                if (Session["Tec_Id"] != null)
                {
                    ClsConexion n = new ClsConexion();
                    Tecnicos us;
                    string query = " SELECT Tc.Tec_Nombre1, Tc.Tec_Apellido1 ,Tc.DPI ,COUNT(Tk.Tec_Id)     AS Cant_TicketAsignados "
                        + " FROM Tecnicos       AS Tc INNER JOIN Tickets AS Tk ON    Tc.Tec_Id = Tk.Tec_Id GROUP BY Tc.Tec_Nombre1, Tc.DPI, Tc.Tec_Apellido1";
                    var reader1 = n.ConsultaBd(query);
                    List<Tecnicos> nn = new List<Tecnicos>();

                    while (reader1.Read())
                    {
                        us = new Tecnicos();
                        us.Tec_Nombre1 = reader1["Tec_Nombre1"].ToString();
                        us.Tec_Apellido1 = reader1["Tec_Apellido1"].ToString();
                        us.DPI = reader1["DPI"].ToString();
                        us.Cant_TicketAsignados = Convert.ToInt32(reader1["Cant_TicketAsignados"].ToString());
                        nn.Add(us);
                    }

                    return View(nn);
                }
                else
                {
                    return RedirectToAction("InicioSesion", "UsrTecnicos");
                }
            }
            else
            {
                return RedirectToAction("ListaTickets", "Ticket");
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
