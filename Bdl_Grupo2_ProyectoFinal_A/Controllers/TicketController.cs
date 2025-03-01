using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Bdl_Grupo2_ProyectoFinal_A;
using Bdl_Grupo2_ProyectoFinal_A.Conexion;

namespace Bdl_Grupo2_ProyectoFinal_A.Controllers
{
    public class TicketController : Controller
    {
        private Bdl_ProyectoFinal_AEntities2 db = new Bdl_ProyectoFinal_AEntities2();

        public ActionResult View_Ticket(int? id)
        {
            if (Session["Tec_Id"] != null)
            {
                Tickets n = new Tickets();
                ClsConexion con = new ClsConexion();
                string query = "SELECT *, CONVERT(NVARCHAR,DATEPART(DAY, Tkt_Fecha_Hora))+'/'+" +
                                "CONVERT(NVARCHAR,DATEPART(MONTH, Tkt_Fecha_Hora))+'/'+" +
                                "CONVERT(NVARCHAR,DATEPART(YEAR, Tkt_Fecha_Hora)) AS DATE " +
                                "FROM Tickets WHERE Tkt_Id = " + id.ToString();
                var oReader1 = con.ConsultaBd(query);

                if (oReader1.HasRows && oReader1.Read())
                {
                    n.Tkt_Id = Convert.ToInt32(oReader1["Tkt_Id"]);
                    n.Tkt_Fecha_Hora = DateTime.Parse( oReader1["DATE"].ToString());
                    n.Tkt_Codigo = oReader1["Tkt_Codigo"].ToString();

                    var oReader2 = con.ConsultaBd("SELECT Prb_Descripcion " +
                                                        "FROM Problemas_Tecnicos " +
                                                        "WHERE PrbTc_Id = " + oReader1["PrbTc_Id"].ToString());
                    if (oReader2.Read())
                    {
                        n.Problemas_Tecnicos = new Problemas_Tecnicos();
                        n.Problemas_Tecnicos.Prb_Descripcion = oReader2["Prb_Descripcion"].ToString();
                        con.cerrarConexion();
                    }
                    n.Tkt_Descripcion = oReader1["Tkt_Descripcion"].ToString();
                    oReader2 = con.ConsultaBd("SELECT (SELECT Dpt_Tipo FROM Departamentos AS Dp WHERE Dp.Dpt_Id = Us.Dpt_Id) " +
                                                   "AS Dpt_Tipo, (SELECT Dpt_Nivel FROM Departamentos AS Dp WHERE Dp.Dpt_Id = Us.Dpt_Id)" +
                                                   " AS Dpt_Nivel, Usr_NombreUsuario, Usr_Correo " +
                                                   "FROM Usuarios AS Us " +
                                                   "WHERE Usr_Id = " + oReader1["Usr_Id"].ToString());
                    if (oReader2.Read())
                    {
                        n.Usuarios = new Usuarios();
                        n.Usuarios.Usr_Correo = oReader2["Usr_Correo"].ToString();
                        n.Usuarios.Usr_NombreUsuario = oReader2["Usr_NombreUsuario"].ToString();
                        n.Usuarios.Departamentos = new Departamentos();
                        n.Usuarios.Departamentos.Dpt_Tipo = oReader2["Dpt_Tipo"].ToString();
                        n.Usuarios.Departamentos.Dpt_Nivel = Convert.ToInt32(oReader2["Dpt_Nivel"]);
                        con.cerrarConexion();
                    }
                    oReader2 = con.ConsultaBd("SELECT Eq.Eqs_Codigo, H.Hdw_Modelo, H.Hdw_Serie, Sf.Sfw_Nombre " +
                                                   "FROM Equipos AS Eq INNER JOIN Hardware AS H " +
                                                   "ON H.Hdw_Id = Eq.Hdw_Id INNER JOIN Software AS Sf " +
                                                   "ON Sf.Sfw_Id = Eq.Sfw_Id WHERE Eq.Eqs_Id =" +
                                                   oReader1["Eqs_Id"].ToString());
                    if (oReader2.Read())
                    {
                        n.Equipos = new Equipos
                        {
                            Hardware = new Hardware(),
                            Software = new Software(),
                            Eqs_Codigo = oReader2["Eqs_Codigo"].ToString()
                        };
                        n.Equipos.Hardware.Hdw_Modelo = oReader2["Hdw_Modelo"].ToString();
                        n.Equipos.Hardware.Hdw_Serie = oReader2["Hdw_Serie"].ToString();
                        n.Equipos.Software.Sfw_Nombre = oReader2["Sfw_Nombre"].ToString();
                        con.cerrarConexion();
                    }
                    oReader2 = con.ConsultaBd("SELECT (SELECT Fabric_Nombre FROM Fabricante WHERE Fabric_Id = Sf.Fabric_Id) " +
                                                            "AS Fabric_Nombre," +
                                                            "(SELECT TpStw_Tipo FROM Tipo_Software WHERE TpStw_Id = Sf.TpStw_Id) " +
                                                            "AS TpStw_Tipo " +
                                                   "FROM Software AS Sf WHERE Sf.Sfw_Id =" +
                                                   "(SELECT Eq.Sfw_Id FROM Equipos AS Eq WHERE Eq.Eqs_Id = " + oReader1["Eqs_Id"].ToString() + ")");
                    if (oReader2.Read())
                    {
                        n.Equipos.Software.Fabricante = new Fabricante();
                        n.Equipos.Software.Tipo_Software = new Tipo_Software();
                        n.Equipos.Software.Fabricante.Fabric_Nombre = oReader2["Fabric_Nombre"].ToString();
                        n.Equipos.Software.Tipo_Software.TpStw_Tipo = oReader2["TpStw_Tipo"].ToString();
                        con.cerrarConexion();
                    }
                    n.EstdTick_Id = Convert.ToInt32(oReader1["EstdTick_Id"]);
                    ViewBag.EstdTick_Id = new SelectList(db.Estado_Ticket, "EstdTick_Id", "EstdTick_Tipo", n.EstdTick_Id);
                }
                con.cerrarConexion();
                return View(n);
            }
            else
            {
                return RedirectToAction("InicioSesion", "UsrTecnicos");
            }
        }
        
        [Authorize]
        public ActionResult ListTicketsUser()
        {
            ClsConexion con = new ClsConexion();
            string query = "SELECT * FROM Tickets";
            var oReader1 = con.ConsultaBd(query);
            List<Tickets> salida = new List<Tickets>();
            if (oReader1.HasRows)
            {
                while (oReader1.Read())
                {

                    if (oReader1["Usr_Id"].ToString() == Session["UserSession"].ToString())
                    {
                        Tickets n = new Tickets();
                        n.Tecnicos = new Tecnicos();
                        n.Problemas_Tecnicos = new Problemas_Tecnicos();
                        n.Ticket_Prioridad = new Ticket_Prioridad();
                        n.Tkt_Id = Convert.ToInt32(oReader1["Tkt_Id"]);
                        n.Tkt_Codigo = oReader1["Tkt_Codigo"].ToString();

                        var oReader2 = con.ConsultaBd("SELECT TktPrd_Tipo FROM Ticket_Prioridad " +
                                                        "WHERE TktPrd_Id =" + oReader1["TktPrd_Id"].ToString());
                        if (oReader2.Read())
                        {
                            n.Ticket_Prioridad.TktPrd_Tipo = oReader2["TktPrd_Tipo"].ToString();
                            con.cerrarConexion();
                        }
                        oReader2 = con.ConsultaBd("SELECT EstdTick_Tipo FROM Estado_Ticket " +
                                                        "WHERE EstdTick_Id =" + oReader1["EstdTick_Id"].ToString());
                        if (oReader2.Read())
                        {
                            n.Estado_Ticket = new Estado_Ticket();
                            n.Estado_Ticket.EstdTick_Tipo = oReader2["EstdTick_Tipo"].ToString();
                            con.cerrarConexion();
                        }
                        oReader2 = con.ConsultaBd("SELECT Prb_Descripcion " +
                                                    "FROM Problemas_Tecnicos " +
                                                    "WHERE PrbTc_Id = " + oReader1["PrbTc_Id"].ToString());
                        if (oReader2.Read())
                        {
                            n.Problemas_Tecnicos.Prb_Descripcion = oReader2["Prb_Descripcion"].ToString();
                            con.cerrarConexion();
                        }
                        n.TktPrd_Id = Convert.ToInt32(oReader1["TktPrd_Id"]);
                        salida.Add(n);
                    }
                }
            }
            else
            {
                Console.WriteLine("No rows found.");
            }
            oReader1.Close();
            con.cerrarConexion();
            return View("ListTicketsUser", salida);
        }

        [Authorize]
        public ActionResult Crear()
        {
            if (!(Session["UserSession"] is null))
            {
                ClsConexion con = new ClsConexion();
                GenerateNewCode generate = new GenerateNewCode();
                Tickets tickets = new Tickets();
                string query = "SELECT TOP 1 T.Tec_Id, U.UsrTecnico_Correo FROM Tecnicos AS T " +
                               "INNER JOIN [dbo].[UsrTecnicos] AS U ON	U.Tec_Id = T.Tec_Id " +
                               "WHERE U.Tec_Id = T.Tec_Id AND T.Prf_Id = 1 ORDER BY NEWID()";
                var oReader = con.ConsultaBd(query);
                if (oReader.Read())
                {
                    tickets.Tec_Id = Convert.ToInt32(oReader["Tec_Id"]);
                }
                tickets.aceptado = 0;
                tickets.Usr_Id = Convert.ToInt32(Session["UserSession"]);
                tickets.Tkt_Codigo = generate.codigofinal("Tickets", "Tkt_Codigo");
                ViewBag.PrbTc_Id = new SelectList(db.Problemas_Tecnicos, "PrbTc_Id", "Prb_Descripcion", tickets.PrbTc_Id);
                ViewBag.TktPrd_Id = new SelectList(db.Ticket_Prioridad, "TktPrd_Id", "TktPrd_Tipo", tickets.TktPrd_Id);
                return View(tickets);
            }
            else
            {
                FormsAuthentication.SignOut();
                Session.RemoveAll();
                return RedirectToAction("Index", "Usuarios");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Crear(string Tkt_Codigo, string TktPrd_Id, string Usr_Id,
            string PrbTc_Id, string Tkt_Descripcion, string Eqs_Codigo)
        {
            if (!(Session["UserSession"] is null))
            {
                Tickets tickets = new Tickets();
                ClsConexion con = new ClsConexion();
                string query = "SELECT TOP 1 T.Tec_Id, U.UsrTecnico_Correo FROM Tecnicos AS T " +
                               "INNER JOIN [dbo].[UsrTecnicos] AS U ON	U.Tec_Id = T.Tec_Id " +
                               "WHERE U.Tec_Id = T.Tec_Id AND T.Prf_Id = 1 ORDER BY NEWID()";
                var oReader = con.ConsultaBd(query);
                string Tec_Id = "";
                string UsrTecnico_Correo = "";
                if (oReader.Read())
                {
                    UsrTecnico_Correo = oReader["UsrTecnico_Correo"].ToString();
                    Tec_Id = oReader["Tec_Id"].ToString();
                }
                oReader = con.ConsultaBd("SELECT Eqs_Id FROM Equipos WHERE Eqs_Codigo = '" + Eqs_Codigo + "' AND Usr_Id =" + Usr_Id);

                if (oReader.Read())
                {
                    string Eqs_Id = oReader["Eqs_Id"].ToString();
                    query = "INSERT INTO [dbo].[Tickets] ([Tkt_Codigo],[Tec_Id],[TktPrd_Id]" +
                                            ",[PrbTc_Id],[Usr_Id],[Tkt_Descripcion],[Eqs_Id]) " +
                             "VALUES		('" + Tkt_Codigo + "'," + Tec_Id + ", " + TktPrd_Id + ", " + PrbTc_Id + ", " + Usr_Id + ", " +
                                            "'" + Tkt_Descripcion + "', " + Eqs_Id + ")";
                    try
                    {
                        SendEmail sendEmail = new SendEmail();
                        con.ConsultaBd(query);
                        con.cerrarConexion();
                        oReader = con.ConsultaBd("SELECT Usr_NombreUsuario, Usr_Correo FROM Usuarios WHERE Usr_Id = " + Usr_Id);
                        if (oReader.Read())
                        {
                            string email = "¡Hola, " + oReader["Usr_NombreUsuario"].ToString() + "! Su Ticket se ha enviado exitosamente, el equipo de soporte DevSolutions le dara seguimiento lo antes posible.";
                            bool em = sendEmail.sendMail(oReader["Usr_Correo"].ToString(), "Nuevo Ticket", email, Tkt_Codigo, "El código de su ticket:");
                            if (em)
                            {
                                query = "INSERT INTO [dbo].[Recordatorios]([Rcdt_Descripcion],[Tkt_Id])" +
                                        " VALUES ('" + email + "',(SELECT Tkt_Id FROM Tickets WHERE Tkt_Codigo = '" + Tkt_Codigo + "'))";
                                con.ConsultaBd(query);
                                tickets.aceptado = 1;
                                email = "Se le ha asignado un nuevo ticket.";
                                sendEmail.sendMail(UsrTecnico_Correo, "Nuevo Ticket", email, Tkt_Codigo, "El código del ticket:", true);
                            }
                        }
                    }
                    catch
                    {
                        tickets.aceptado = 2;
                    }
                }
                else
                {
                    tickets.aceptado = 3;
                }
                query = "SELECT TOP 1 Tec_Id FROM Tecnicos WHERE Prf_Id = 1 ORDER BY NEWID()";
                oReader = con.ConsultaBd(query);
                if (oReader.Read())
                {
                    tickets.Tec_Id = Convert.ToInt32(oReader["Tec_Id"]);
                }
                GenerateNewCode generate = new GenerateNewCode();
                tickets.Tkt_Codigo = generate.codigofinal("Tickets", "Tkt_Codigo");
                tickets.Usr_Id = Convert.ToInt32(Session["UserSession"]);
                ViewBag.PrbTc_Id = new SelectList(db.Problemas_Tecnicos, "PrbTc_Id", "Prb_Descripcion", tickets.PrbTc_Id);
                ViewBag.TktPrd_Id = new SelectList(db.Ticket_Prioridad, "TktPrd_Id", "TktPrd_Tipo", tickets.TktPrd_Id);
                return View(tickets);
            }
            else
            {
                FormsAuthentication.SignOut();
                Session.RemoveAll();
                return RedirectToAction("Index", "Usuarios");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult View_Ticket([Bind(Include = "Tkt_Id,EstdTick_Id")] Tickets tickets, string correo, string date)
        {
            if (Session["Tec_Id"] != null)
            {
                ClsConexion con = new ClsConexion();
                string query = "UPDATE Tickets SET EstdTick_Id = " + tickets.EstdTick_Id + " WHERE Tkt_Id = " + tickets.Tkt_Id;
                if (tickets.EstdTick_Id == 3)
                {
                    var oReader = con.ConsultaBd("SELECT Tkt_Codigo FROM Tickets WHERE Tkt_Id =" + tickets.Tkt_Id);
                    string code = "";
                    if (oReader.Read())
                    {
                        code = oReader["Tkt_Codigo"].ToString();
                    }
                    SendEmail email = new SendEmail();
                    string emailbody = "¡Hola!, le saludamos cordialmete del Equipo de DevSolutions. Le informamos que, el Ticket enviado la fecha " + date + " " +
                                        "ha sido resuelto en la totalidad sin ningun inconveniente. Espero tenga un muy buen día.";
                    var em = email.sendMail(correo, "Ticket Resuelto", emailbody, code, "Usted recibio una notificación sobre el Ticket");
                    if (em)
                    {
                        try
                        {
                            string query1 = "INSERT INTO Recordatorios (Rcdt_Descripcion, Tkt_Id) VALUES('" + emailbody + "', " + tickets.Tkt_Id + ")";
                            con.ConsultaBd(query1);
                            con.cerrarConexion();
                        }
                        catch
                        {
                            return Redirect("/Ticket/View_Ticket/" + tickets.Tkt_Id);
                        }
                        finally
                        {
                            con.cerrarConexion();
                        }
                    }
                }
                try
                {
                    con.ConsultaBd(query);
                    con.cerrarConexion();
                    return Redirect("/Ticket/View_Ticket/" + tickets.Tkt_Id);
                }
                catch
                {
                    return Redirect("/Ticket/View_Ticket/" + tickets.Tkt_Id);
                }
            }
            else
            {
                return RedirectToAction("InicioSesion", "UsrTecnicos");
            }
        }
    
        public ActionResult ListTickets()
        {
            if (!(Session["Tec_Id"] is null))
            {
                string Tec_Id = Session["Tec_Id"].ToString();
                string Prf_Id = "";
                ClsConexion con = new ClsConexion();
                var oReader1 = con.ConsultaBd("SELECT Prf_Id FROM Tecnicos WHERE Tec_Id = "+ Tec_Id);
                if (oReader1.Read()) 
                {
                    if(Session["Prf_Id"] is null) Session.Add("Prf_Id", oReader1["Prf_Id"].ToString());
                    Prf_Id = oReader1["Prf_Id"].ToString();
                }
                con.cerrarConexion();
                string query = "SELECT * FROM Tickets";
                oReader1 = con.ConsultaBd(query);
                List<Tickets> salida = new List<Tickets>();
                if (oReader1.HasRows)
                {
                    while (oReader1.Read())
                    {
                        if ((Tec_Id == oReader1["Tec_Id"].ToString() || Prf_Id== "2" 
                            || Prf_Id == "3") && Convert.ToInt32(oReader1["EstdTick_Id"]) != 3)
                        {
                            Tickets n = new Tickets();
                            n.Tecnicos = new Tecnicos();
                            n.Problemas_Tecnicos = new Problemas_Tecnicos();
                            n.Ticket_Prioridad = new Ticket_Prioridad();
                            n.Tkt_Id = Convert.ToInt32(oReader1["Tkt_Id"]);
                            n.Tkt_Codigo = oReader1["Tkt_Codigo"].ToString();
                            var oReader2 = con.ConsultaBd("SELECT Tec_Nombre1, Tec_Apellido1 " +
                                                                "FROM Tecnicos WHERE Tec_Id ="
                                                                + oReader1["Tec_Id"].ToString());
                            if (oReader2.Read())
                            {
                                n.Tecnicos.Tec_Nombre1 = oReader2["Tec_Nombre1"].ToString() + " "
                                                            + oReader2["Tec_Apellido1"].ToString();
                                con.cerrarConexion();
                            }
                            oReader2 = con.ConsultaBd("SELECT TktPrd_Tipo FROM Ticket_Prioridad " +
                                                            "WHERE TktPrd_Id =" + oReader1["TktPrd_Id"].ToString());
                            if (oReader2.Read())
                            {
                                n.Ticket_Prioridad.TktPrd_Tipo = oReader2["TktPrd_Tipo"].ToString();
                                con.cerrarConexion();
                            }
                            oReader2 = con.ConsultaBd("SELECT Prb_Descripcion " +
                                                        "FROM Problemas_Tecnicos " +
                                                        "WHERE PrbTc_Id = " + oReader1["PrbTc_Id"].ToString());
                            if (oReader2.Read())
                            {
                                n.Problemas_Tecnicos.Prb_Descripcion = oReader2["Prb_Descripcion"].ToString();
                                con.cerrarConexion();
                            }
                            n.TktPrd_Id = Convert.ToInt32(oReader1["TktPrd_Id"]);
                            salida.Add(n);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                oReader1.Close();
                con.cerrarConexion();
                return View("ListTickets", salida);
            }
            else
            {
                return RedirectToAction("InicioSesion", "UsrTecnicos");
            }
        }

        public ActionResult ListTicketsTerminados()
        {
            if (Session["Tec_Id"] != null)
            {
                string Tec_Id = Session["Tec_Id"].ToString();
                ClsConexion con = new ClsConexion();
                string Prf_Id = Session["Prf_Id"].ToString();
                string query = "SELECT * FROM Tickets";
                var oReader1 = con.ConsultaBd(query);
                List<Tickets> salida = new List<Tickets>();
                if (oReader1.HasRows)
                {
                    while (oReader1.Read())
                    {
                        if ((Tec_Id == oReader1["Tec_Id"].ToString() || Prf_Id == "2"
                            || Prf_Id == "3") && Convert.ToInt32(oReader1["EstdTick_Id"]) == 3)
                        {
                            Tickets n = new Tickets();
                            n.Tecnicos = new Tecnicos();
                            n.Ticket_Prioridad = new Ticket_Prioridad();
                            n.Problemas_Tecnicos = new Problemas_Tecnicos();
                            n.Tkt_Id = Convert.ToInt32(oReader1["Tkt_Id"]);
                            n.Tkt_Codigo = oReader1["Tkt_Codigo"].ToString();
                            var oReader2 = con.ConsultaBd("SELECT Tec_Nombre1, Tec_Apellido1 FROM Tecnicos WHERE Tec_Id ="
                                                            + oReader1["Tec_Id"].ToString());
                            if (oReader2.Read())
                            {
                                n.Tecnicos.Tec_Nombre1 = oReader2["Tec_Nombre1"].ToString() + " "
                                                            + oReader2["Tec_Apellido1"].ToString();
                                con.cerrarConexion();
                            }
                            oReader2 = con.ConsultaBd("SELECT TktPrd_Tipo FROM Ticket_Prioridad WHERE TktPrd_Id ="
                                                            + oReader1["TktPrd_Id"].ToString());
                            if (oReader2.Read())
                            {
                                n.Ticket_Prioridad.TktPrd_Tipo = oReader2["TktPrd_Tipo"].ToString();
                                con.cerrarConexion();
                            }
                            oReader2 = con.ConsultaBd("SELECT Prb_Descripcion " +
                                                       "FROM Problemas_Tecnicos " +
                                                       "WHERE PrbTc_Id = " + oReader1["PrbTc_Id"].ToString());
                            if (oReader2.Read())
                            {
                                n.Problemas_Tecnicos.Prb_Descripcion = oReader2["Prb_Descripcion"].ToString();
                                con.cerrarConexion();
                            }
                            n.TktPrd_Id = Convert.ToInt32(oReader1["TktPrd_Id"]);

                            salida.Add(n);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                oReader1.Close();
                con.cerrarConexion();
                return View("ListTicketsTerminados", salida);
            }
            else
            {
                return RedirectToAction("InicioSesion", "UsrTecnicos");
            }
        } 

        public ActionResult Edit(int? id)
        {
            if (Convert.ToInt32(Session["Prf_Id"]) == 2)
            {
                if (Session["Tec_Id"] != null)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Tickets tickets = db.Tickets.Find(id);
                    if (tickets == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.EstdTick_Id = new SelectList(db.Estado_Ticket, "EstdTick_Id", "EstdTick_Tipo", tickets.EstdTick_Id);
                    ViewBag.PrbTc_Id = new SelectList(db.Problemas_Tecnicos, "PrbTc_Id", "Prb_Descripcion", tickets.PrbTc_Id);
                    ViewBag.Tec_Id = new SelectList(db.Tecnicos, "Tec_Id", "DPI", tickets.Tec_Id);
                    ViewBag.TktPrd_Id = new SelectList(db.Ticket_Prioridad, "TktPrd_Id", "TktPrd_Tipo", tickets.TktPrd_Id);
                    ViewBag.Usr_Id = new SelectList(db.Usuarios, "Usr_Id", "Usr_CodigoUsuario", tickets.Usr_Id);
                    ViewBag.Eqs_Id = new SelectList(db.Equipos, "Eqs_Id", "Eqs_Codigo", tickets.Eqs_Id);
                    return View(tickets);
                }
                else
                {
                    return RedirectToAction("InicioSesion", "UsrTecnicos");
                }
            }
            else
            {
                return RedirectToAction("ListTickets", "Ticket");
            }
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "Tkt_Id,Tec_Id,TktPrd_Id,PrbTc_Id,EstdTick_Id,Usr_Id,Eqs_Id,Tkt_Descripcion")] Tickets tickets)
        {
            if (Session["Tec_Id"] != null)
            {
                ClsConexion con = new ClsConexion();
                string query = "UPDATE Tickets SET Tec_Id = "+tickets.Tec_Id+", TktPrd_Id = "+tickets.TktPrd_Id+", " +
                                "PrbTc_Id = "+tickets.PrbTc_Id+", EstdTick_Id = "+tickets.EstdTick_Id+", Usr_Id = "+tickets.Usr_Id+","+ 
                                "Tkt_Descripcion = '"+tickets.Tkt_Descripcion+"', Eqs_Id = "+tickets.Eqs_Id+" WHERE Tkt_Id = "+tickets.Tkt_Id;
                try
                {
                    con.ConsultaBd(query);
                    con.cerrarConexion();
                    return RedirectToAction("ListTickets", "Ticket");
                }
                catch
                {
                    return RedirectToAction("ListTickets", "Ticket");
                }
            }
            else
            {
                return RedirectToAction("InicioSesion", "UsrTecnicos");
            }
        }

        public ActionResult Delete(int? id)
        {
            if (Convert.ToInt32(Session["Prf_Id"]) == 2)
            {
                if (Session["Tec_Id"] != null)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Tickets tickets = db.Tickets.Find(id);
                    if (tickets == null)
                    {
                        return HttpNotFound();
                    }
                    return View(tickets);
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Tec_Id"] != null)
            {
                ClsConexion con = new ClsConexion();
                try
                {
                    string query1 = "SELECT EstdTick_Id FROM Tickets WHERE Tkt_Id ="+id;
                    var oReader = con.ConsultaBd(query1);
                    if (oReader.Read())
                    {
                        string query = "DELETE FROM Recordatorios WHERE Tkt_Id =" + id;
                        con.ConsultaBd(query);
                        con.cerrarConexion();
                        query = "DELETE FROM Tickets WHERE Tkt_Id =" + id;
                        con.ConsultaBd(query);
                        con.cerrarConexion();
                        if (Convert.ToInt32(oReader["EstdTick_Id"]) == 3)
                        {
                            return RedirectToAction("ListTicketsTerminados", "Ticket");
                        }
                        else
                        {
                            return RedirectToAction("ListTickets", "Ticket");
                        }
                    }
                    return RedirectToAction("ListTickets", "Ticket");
                }
                catch
                {
                    return RedirectToAction("ListTickets", "Ticket");
                }
            }
            else
            {
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
