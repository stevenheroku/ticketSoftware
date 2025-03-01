using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Security;
using System.Web.Mvc;
using Bdl_Grupo2_ProyectoFinal_A;
using Bdl_Grupo2_ProyectoFinal_A.Conexion;
using System.Device.Location;

namespace Bdl_Grupo2_ProyectoFinal_A.Controllers
{
    public class UsuariosController : Controller
    {
        private Bdl_ProyectoFinal_AEntities2 db = new Bdl_ProyectoFinal_AEntities2();
        private GenerateNewCode generate = new GenerateNewCode();
        public ActionResult Index()
        {
            Usuarios usuarios = new Usuarios();
            usuarios.enviado = 0;
            usuarios.Dpt_Id = 1;
            usuarios.Usr_CodigoUsuario = generate.codigofinal("Usuarios", "Usr_CodigoUsuario");
            ViewBag.Dpt_Id = new SelectList(db.Departamentos, "Dpt_Id", "Dpt_Tipo", usuarios.Dpt_Id);
            return View(usuarios);
        }


        [HttpPost]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            Session.RemoveAll();
            return RedirectToAction("Index", "Usuarios");
        }

        [HttpPost]
        public ActionResult Verificar(string usr, string pwd)
        {
            Usuarios usuarios = new Usuarios();
            ClsConexion con = new ClsConexion();
            SqlDataReader dr;
            var query = "SELECT Usr_Id FROM Usuarios WHERE Usr_Correo = '" + usr + "' AND Usr_Password = '" + pwd + "'";
            dr = con.ConsultaBd(query);
            
            if (dr.Read())
            {
                Session.Add("UserSession", dr["Usr_Id"].ToString());
                FormsAuthentication.SetAuthCookie(usr, false);
                con.cerrarConexion();
                return RedirectToAction("Crear", "Ticket");
            }
            else
            {
                usuarios.enviado = 1;
                usuarios.Dpt_Id = 1;
                usuarios.Usr_CodigoUsuario = generate.codigofinal("Usuarios", "Usr_CodigoUsuario");
                ViewBag.Dpt_Id = new SelectList(db.Departamentos, "Dpt_Id", "Dpt_Tipo", usuarios.Dpt_Id);
                con.cerrarConexion();
                return View("Index", usuarios);
            }
        }

        [HttpPost]
        public ActionResult Register([Bind(Include = "Dpt_Id")] string codigo, string usuario, string correo, string password, Usuarios usuarios)
        {
            ClsConexion con = new ClsConexion();

            SqlDataReader dr;

            string query2 = "SELECT Usr_Correo FROM Usuarios WHERE Usr_Correo = '" + correo + "' OR Usr_CodigoUsuario ='"+codigo+"'";
            dr = con.ConsultaBd(query2);

            if (dr.Read())
            {
                usuarios.Dpt_Id = usuarios.Dpt_Id;
                ViewBag.Dpt_Id = new SelectList(db.Departamentos, "Dpt_Id", "Dpt_Tipo", usuarios.Dpt_Id);
                usuarios.Usr_CodigoUsuario = generate.codigofinal("Usuarios", "Usr_CodigoUsuario");
                con.cerrarConexion();
                usuarios.enviado = 2;
                return View("Index", usuarios);
            }
            else
            {
                SendEmail email = new SendEmail();
                string query = "insert into Usuarios(Usr_CodigoUsuario,Usr_NombreUsuario,Usr_Correo,Usr_Password,Dpt_Id) " +
                "values('" + codigo + "','" + usuario + "','" + correo + "','" + password + "','" + usuarios.Dpt_Id + "')";


                string em = "Su registros se ha completado.";
                email.sendMail(correo, "Aviso de registro", em, codigo, "Su código de unico de usuario:");
                con.ConsultaBd(query);

                con.cerrarConexion();
                return RedirectToAction("Index", "Usuarios");
            }
        }

        public ActionResult SendEmail()
        {
            if (Session["Tec_Id"] != null)
            {
                Usuarios us = new Usuarios();
                try
                {
                    if (Session["correoUsr"] != null && Session["Id_Ticket"] != null)
                    {
                        us.Usr_Correo = HttpContext.Session["correoUsr"].ToString();
                        us.Usr_Id = Convert.ToInt32(HttpContext.Session["Id_Ticket"]); ;
                    }
                    else
                    {
                        return View(us);
                    }
                    return View(us);
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("InicioSesion", "UsrTecnicos");
            }
        }
        public ActionResult RedirectSendEmail()
        {
            HttpContext.Session.Add("correoUsr", Request.QueryString["usr"]);
            HttpContext.Session.Add("Id_Ticket", Request.QueryString["id"]);
            return Redirect("/Usuarios/SendEmail");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SendEmail([Bind(Include = "Usr_Correo")] Usuarios usuarios, string id, String email, String asunto)
        {
            if (Session["Tec_Id"] != null)
            {
                ClsConexion con = new ClsConexion();
                var oReader = con.ConsultaBd("SELECT Tkt_Codigo FROM Tickets WHERE Tkt_Id =" + id);
                string code = "";
                if (oReader.Read())
                {
                    code = oReader["Tkt_Codigo"].ToString();
                }
                SendEmail objLogic = new SendEmail();
                bool em = objLogic.sendMail(usuarios.Usr_Correo, asunto, email, code, "Usted recibio una notificación sobre el Ticket");
                if (em)
                {
                    try
                    {
                        string query = "INSERT INTO Recordatorios ([Rcdt_Descripcion],[Tkt_Id]) VALUES('" + email + "', " + id + ")";
                        con.ConsultaBd(query);
                        con.cerrarConexion();
                        usuarios.enviado = 1;
                    }
                    catch
                    {
                        usuarios.enviado = 2;
                    }
                }
                else
                {
                    usuarios.enviado = 2;
                }
                HttpContext.Session.RemoveAll();
                usuarios.Usr_Id = Convert.ToInt32(id);
                return View(usuarios);
            }
            else
            {
                return RedirectToAction("InicioSesion", "UsrTecnicos");
            }
        }

        public ActionResult ListaUsuarios()
        {
            if (!(Session["Tec_Id"] is null) && Convert.ToInt32(Session["Prf_Id"]) == 2)
            {
                ClsConexion n = new ClsConexion();
                Usuarios us;
                string query = "SELECT Usr_NombreUsuario, Usr_CodigoUsuario ,COUNT(Us.Usr_Id) AS Cant_TicketGenerados " +
                               "FROM Usuarios AS Us INNER JOIN Tickets AS Tk ON  Tk.Usr_Id = Us.Usr_Id GROUP BY Us.Usr_NombreUsuario, Us.Usr_CodigoUsuario";
                var reader1 = n.ConsultaBd(query);
                List<Usuarios> nn = new List<Usuarios>();

                while (reader1.Read())
                {
                    us = new Usuarios();
                    us.Usr_NombreUsuario = reader1["Usr_NombreUsuario"].ToString();
                    us.Usr_CodigoUsuario = reader1["Usr_CodigoUsuario"].ToString();
                    us.Cant_TicketGenerados = Convert.ToInt32(reader1["Cant_TicketGenerados"].ToString());
                    nn.Add(us);

                }
                return View(nn);
            }
            else
            {
                return RedirectToAction("ListTickets", "Ticket");
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
