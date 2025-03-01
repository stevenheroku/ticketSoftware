using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Bdl_Grupo2_ProyectoFinal_A.Conexion
{
    public class GenerateNewCode
    {
        public static string crearrandom()
        {
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var Charsarr = new char[4];
            var random = new Random();

            for (int i = 0; i < Charsarr.Length; i++)
            {
                Charsarr[i] = characters[random.Next(characters.Length)];
            }

            var resultString = new String(Charsarr);


            return resultString;
        }

        public string codigofinal(string tabla, string campo)
        {
            SqlDataReader dr;
            Tickets tx = new Tickets();
            string codigo = crearrandom();
            ClsConexion con = new ClsConexion();
            string queryselect = "SELECT * FROM "+tabla+" WHERE "+campo+" ='" + codigo + "'";

            dr = con.ConsultaBd(queryselect);

            while (dr.Read())
            {
                if (dr.Read())
                {
                    codigo = crearrandom();
                }
            }
            con.cerrarConexion();
            return codigo;
        }
    }
}