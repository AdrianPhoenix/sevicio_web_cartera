using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace MedinetGeneradorDB
{
    public class Data
    {

        SqlConnection sqlConexion;
        SqlCommand sqlComando;
        SqlDataReader sqlReader;

        public Data(string dataBase)
        {
            // Usamos el usuario nuevo para saltar el bloqueo del 'sa'
            string connString = "Data Source=38.190.187.149;Initial Catalog=Medinet_PR;User ID=sa;Password=Medinet2013;TrustServerCertificate=True;";
            sqlConexion = new SqlConnection(connString);
            sqlConexion.Open();
        }

        public string[] buscarAnnios()
        {


            sqlComando = new SqlCommand("SELECT DISTINCT NU_Ano FROM MW_Ciclos WHERE NU_Estatus=1 OR NU_Estatus=2 ORDER BY 1 DESC", sqlConexion);
            sqlReader = sqlComando.ExecuteReader();

            List<string> listadoAnnios = new List<string>();
            while (sqlReader.Read())
            {
                listadoAnnios.Add(sqlReader.GetInt16(0).ToString());
            }
            sqlReader.Close();
            return listadoAnnios.ToArray();

        }
        public string buscarGoogleRegistrationID(long idVisitador)
        {

            
            if (sqlConexion.State == System.Data.ConnectionState.Closed)
                sqlConexion.Open();

            sqlComando = new SqlCommand("SELECT dbo.MW_Usuarios.TX_IDRegistroGoogle FROM dbo.MW_Usuarios INNER JOIN " +
                                        "dbo.MW_VisitadoresHistorial ON dbo.MW_Usuarios.ID_Usuario = dbo.MW_VisitadoresHistorial.ID_Visitador " +
                                        "WHERE (dbo.MW_VisitadoresHistorial.ID_VisitadoresHistorial = " + idVisitador + ") AND (dbo.MW_VisitadoresHistorial.BO_Activo = 1)", sqlConexion);
            sqlReader = sqlComando.ExecuteReader();
            string registroGoogleID = "";
            while (sqlReader.Read())
            {
                try
                {
                    registroGoogleID = sqlReader.GetString(0);
                }
                catch (Exception )
                {
                    registroGoogleID = "";
                } 
            }
            sqlReader.Close();
            return registroGoogleID;

        }
        public string[] buscarCiclos()
        {


            sqlComando = new SqlCommand("SELECT DISTINCT NU_Ciclo FROM MW_Ciclos WHERE NU_Estatus=1 OR NU_Estatus=2  ORDER BY 1", sqlConexion);
            sqlReader = sqlComando.ExecuteReader();

            List<string> listadoCiclos = new List<string>();
            while (sqlReader.Read())
            {
                listadoCiclos.Add(sqlReader.GetInt16(0).ToString());
            }
            sqlReader.Close();
            return listadoCiclos.ToArray();

        }
        public List<Visitador> buscarVisitadores(int ano, int ciclo)
        {

            sqlComando = new SqlCommand("SP_GBD_VisitadoresActivos", sqlConexion);
            sqlComando.CommandType = System.Data.CommandType.StoredProcedure;
            sqlComando.Parameters.AddWithValue("@ano", ano);
            sqlComando.Parameters.AddWithValue("@ciclo", ciclo);

            List<Visitador> listadoVisitadores = new List<Visitador>();
            sqlReader = sqlComando.ExecuteReader();

            while (sqlReader.Read())
            {
                //listadoVisitadores.Add(new Visitador(sqlReader.GetInt32(0), sqlReader.GetString(1) + " " + sqlReader.GetString(2), sqlReader.GetInt32(3), sqlReader.GetInt32(4)));
                listadoVisitadores.Add(new Visitador(sqlReader.GetInt64(0), sqlReader.GetString(1) + " " + sqlReader.GetString(2), sqlReader.GetInt16(3), sqlReader.GetInt16(4)));
            }
            sqlReader.Close();
            return listadoVisitadores;
        }
        public class Visitador
        {
            public Visitador(Int64 id, string nombre, Int16 region, Int16 linea)
            {
                this.id = id;
                this.nombre = nombre;
                this.region = region;
                this.linea = linea;
            }

            public Visitador()
            {
                // TODO: Complete member initialization
            }

            public Int64 id { get; set; }
            public string nombre { get; set; }
            public Int16 region { get; set; }
            public Int16 linea { get; set; }
        } 
        public void terminarConexion()
        {
            sqlConexion.Close();
        }
    }
}
