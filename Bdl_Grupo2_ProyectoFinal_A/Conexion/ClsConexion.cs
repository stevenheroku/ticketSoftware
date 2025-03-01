using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Bdl_Grupo2_ProyectoFinal_A.Conexion
{
    public class ClsConexion
    {
        public SqlConnection con;
        private String _conexion { get; }

        public ClsConexion()
        {
           // _conexion = "Data Source=SQL5110.site4now.net;Initial Catalog=db_a9970d_devsolutionsjsrr;User Id=db_a9970d_devsolutionsjsrr_admin;Password=d5g@8GrDF7bwxuf";
           // _conexion = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;

             _conexion = "Data Source=DESKTOP-88PQ0ST\\SQLEXPRESS; Initial Catalog=Bdl_ProyectoFinal_A; Persist Security Info=True; User ID=steven; Password=123456; Connect Timeout = 500";
        }

        public SqlDataReader ConsultaBd(String query)
        {
            abrirConexion();
            SqlCommand connection = new SqlCommand(query);
            connection.Connection = con;
            SqlDataReader oReader = connection.ExecuteReader();
            return oReader;
        }

        public void abrirConexion()
        {
            con = new SqlConnection(_conexion);
            con.Open();
        }

        public void cerrarConexion()
        {
            con.Close();
        }

    }

}
