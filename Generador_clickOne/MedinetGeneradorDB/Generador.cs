using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace MedinetGeneradorDB
{
    public class Generador
    {
        SQLiteConnection sqliteConexion;
        SqlConnection sqlConexion;
        SqlCommand sqlComando;
        SQLiteCommand sqlComandoLite;
        SqlDataReader sqlReader;
        List<string> listaDeTablasACopiar;
        List<string> listaDeTablasACopiarHistorica;

        Data.Visitador miVisitador;
        StreamWriter archivo;
        string sqlSelect;
        string sqlInsert;
        int ano = 0;
        int ciclo = 0;
        bool limpia;
        bool cicloAbierto;
        public List<string> errores = new List<string>();

        public Generador(string rutaBD, string rutaFile, bool limpia, Data.Visitador visitador, int ano, int ciclo, bool cicloAbierto, string dataBase)
        {

            this.ano = ano;
            this.ciclo = ciclo;
            this.miVisitador = visitador;
            sqliteConexion = new SQLiteConnection("Data Source=" + rutaBD);
            sqliteConexion.Open();
            archivo = new StreamWriter(rutaFile, true);
            this.limpia = limpia;
            this.cicloAbierto = cicloAbierto;

#if DEBUG
            sqlConexion = new SqlConnection("Data Source=38.190.187.149;Initial Catalog=Medinet_PR;User ID=sa;Password=Medinet2013;TrustServerCertificate=True;");

#else
            sqlConexion = new SqlConnection("Data Source=38.190.187.149;Initial Catalog=" + dataBase + ";User ID=sa;Password=Medinet2013");
#endif
            sqlConexion.Open();

            listaDeTablasACopiar = new List<string>();
            listaDeTablasACopiarHistorica = new List<string>();


            listaDeTablasACopiar.Add("MD_Farmacias");
            listaDeTablasACopiar.Add("MD_Farmacias_Personal");
            listaDeTablasACopiar.Add("MD_Fichero");
            listaDeTablasACopiar.Add("MD_Fichero_Farmacias");
            listaDeTablasACopiar.Add("MD_Fichero_Horarios");
            listaDeTablasACopiar.Add("MD_Fichero_Hospital");
            listaDeTablasACopiar.Add("MD_Hoja_Ruta");
            listaDeTablasACopiar.Add("MD_Hoja_Ruta_Propuesta");
            listaDeTablasACopiar.Add("MD_Hospital");

            if (!limpia)
            {
                listaDeTablasACopiar.Add("MD_Eliminar");
                listaDeTablasACopiar.Add("MD_Farmacias_Detalles");
                listaDeTablasACopiar.Add("MD_Farmacias_Detalles_Productos");
                listaDeTablasACopiar.Add("MD_HistorialConceptoDias");
                listaDeTablasACopiar.Add("MD_Hospital_Detalles");
                listaDeTablasACopiar.Add("MD_Hospital_Detalles_Medicos");
                listaDeTablasACopiar.Add("MD_Inclusiones");
                listaDeTablasACopiar.Add("MD_Solicitudes");
                listaDeTablasACopiar.Add("MD_Visita_Detalles");
                listaDeTablasACopiar.Add("MD_Visitas");
                listaDeTablasACopiar.Add("MD_Ayuda_Visual");
                listaDeTablasACopiar.Add("MD_Ayuda_Visual_FE");
                listaDeTablasACopiar.Add("MD_Ayuda_Visual_MP4");
                listaDeTablasACopiar.Add("MD_Ayuda_Visual_MP4_FE");
                listaDeTablasACopiarHistorica.Add("MD_Visita_Detalles");
                listaDeTablasACopiarHistorica.Add("MD_Visitas");
                listaDeTablasACopiarHistorica.Add("MD_Solicitudes");
                listaDeTablasACopiarHistorica.Add("MD_Hospital_Detalles");
                listaDeTablasACopiarHistorica.Add("MD_Hospital_Detalles_Medicos");
                listaDeTablasACopiarHistorica.Add("MD_Farmacias_Detalles");
                listaDeTablasACopiarHistorica.Add("MD_Farmacias_Detalles_Productos");
            }

            copiarData();
            copiarDataHistoricoCiclos();
            copiarDataHistorica();
            otrosPasos(rutaFile);
            terminarConexiones();
        }
        public static string GetCurrentMethodName()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);

            return stackFrame.GetMethod().Name;
        }
        private void otrosPasos(string rutaFile)
        {
            StreamWriter archivo = new StreamWriter(rutaFile, true);


            List<string> listaInstrucciones = new List<string>();
            listaInstrucciones.Add("INSERT INTO identificacion (NOMBRE,CLAVE,ZONA,REGION,LINEA,S_CLAVE,S_NOMBRE,A_CLAVE,A_NOMBRE,ESPERA_TONO) VALUES ('" + miVisitador.nombre + "','" + miVisitador.nombre.Substring(0, 4) + "','" + miVisitador.id + "','" + miVisitador.region + "', '" + miVisitador.linea + "','.','.','.','.','1');");
            listaInstrucciones.Add("UPDATE Farmacias SET CICLO=" + ciclo + ";");
            listaInstrucciones.Add("UPDATE Fichero_Hospital SET CICLO=" + ciclo + ";");
            listaInstrucciones.Add("UPDATE Fichero_Farmacias SET CICLO=" + ciclo + ";");
            listaInstrucciones.Add("UPDATE Hoja_Ruta SET CICLO=" + ciclo + ";");
            listaInstrucciones.Add("UPDATE Hoja_Ruta_Propuesta SET CICLO=" + ciclo + ";");
            listaInstrucciones.Add("UPDATE Hospital SET CICLO=" + ciclo + ";");
            listaInstrucciones.Add("DELETE FROM contador;");
            listaInstrucciones.Add("INSERT INTO contador VALUES(0);");
            listaInstrucciones.Add("UPDATE contador SET contador=(SELECT MAX(cast(REGISTRO as integer))+1 FROM FICHERO);");

            if (limpia)
            {
                listaInstrucciones.Add("DELETE FROM Farmacias_Detalles;");
                listaInstrucciones.Add("DELETE FROM Hospital_Detalles;");
                listaInstrucciones.Add("DELETE FROM Visita_Detalles;");
                listaInstrucciones.Add("DELETE FROM Visitas;");
                listaInstrucciones.Add("DELETE FROM HVisita_Detalles;");
                listaInstrucciones.Add("DELETE FROM HVisitas;");
                listaInstrucciones.Add("DELETE FROM Solicitudes;");
                listaInstrucciones.Add("DELETE FROM HSolicitudes;");
                listaInstrucciones.Add("DELETE FROM Eliminar;");
                listaInstrucciones.Add("DELETE FROM Farmacias_Detalles;");
                listaInstrucciones.Add("UPDATE Fichero SET CICLO_C=0, REVISITA_C=0, FECHA_AGENDA='.', SEMANA_AGENDA=0, AMPM_AGENDA='.',NRO_FECHA_AGENDA=0,CICLO_AGENDA=0,RFECHA_AGENDA='.', RSEMANA_AGENDA=0, RAMPM_AGENDA='.',RNRO_FECHA_AGENDA=0,RCICLO_AGENDA=0, HORA='XX:XX';");
            }

            foreach (string instruccion in listaInstrucciones)
            {
                sqlComandoLite = new SQLiteCommand(instruccion, sqliteConexion);
                sqlComandoLite.ExecuteNonQuery();
                archivo.WriteLine(instruccion);
            }
            if (cicloAbierto)
            {


                string sqlSelect = "SELECT * FROM MW_Ciclos WHERE NU_Ano=" + ano;

                sqlComando = new SqlCommand(sqlSelect, sqlConexion);

                if (sqlReader.IsClosed)
                    sqlReader.Close();

                sqlReader = sqlComando.ExecuteReader();

                if (sqlReader.HasRows)
                {
                    while (sqlReader.Read())
                    {
                        DateTime fechaInicio = sqlReader.GetDateTime(3);
                        DateTime fechaFin = sqlReader.GetDateTime(4);

                        string fechaInicioS = ((fechaInicio.Day.ToString().Length > 1) ? fechaInicio.Day.ToString() : "0" + fechaInicio.Day.ToString()) + "/" + ((fechaInicio.Month.ToString().Length > 1) ? fechaInicio.Month.ToString() : "0" + fechaInicio.Month) + "/" + fechaInicio.Year;
                        string fechaFinS = ((fechaFin.Day.ToString().Length > 1) ? fechaFin.Day.ToString() : "0" + fechaFin.Day.ToString()) + "/" + ((fechaFin.Month.ToString().Length > 1) ? fechaFin.Month.ToString() : "0" + fechaFin.Month) + "/" + fechaFin.Year;

                        if (sqlReader.GetInt16(2) < ciclo)
                        {
                            sqlInsert = "INSERT INTO Ciclos (FECHAI_CICLO,FECHAF_CICLO,NRO_CICLO,CICLO_CERRADO,DIAS_HABILES,ESTATUS) VALUES ('" + fechaInicioS + "','" + fechaFinS + "'," + sqlReader.GetInt16(2) + ",'C'," + sqlReader.GetInt16(7) + ",'C');";
                        }
                        else
                        {
                            if (sqlReader.GetInt16(2) == ciclo)
                            {
                                sqlInsert = "INSERT INTO Ciclos (FECHAI_CICLO,FECHAF_CICLO,NRO_CICLO,CICLO_CERRADO,DIAS_HABILES,ESTATUS) VALUES ('" + fechaInicioS + "','" + fechaFinS + "'," + sqlReader.GetInt16(2) + ",'A'," + sqlReader.GetInt16(7) + ",'A');";
                            }
                            else
                            {
                                sqlInsert = "INSERT INTO Ciclos (FECHAI_CICLO,FECHAF_CICLO,NRO_CICLO,CICLO_CERRADO,DIAS_HABILES,ESTATUS) VALUES ('" + fechaInicioS + "','" + fechaFinS + "'," + sqlReader.GetInt16(2) + ",'P'," + sqlReader.GetInt16(7) + ",'P');";
                            }
                        }

                        sqlComandoLite = new SQLiteCommand(sqlInsert, sqliteConexion);
                        sqlComandoLite.ExecuteNonQuery();

                        archivo.WriteLine(sqlInsert);

                    }
                }


            }
            archivo.Flush();
            archivo.Close();

        }
        private void copiarDataHistoricoCiclos()
        {

            string nombreTablaSqlite = "cierreciclo";
            sqlSelect = "SELECT * FROM [MD_CierreCiclo] WHERE(ZONA='" + miVisitador.id + "' AND(NU_ANO = " + (ano - 1) + "  AND CICLO >= " + ciclo + ")) OR(ZONA='" + miVisitador.id + "' AND NU_ANO = " + ano + ")  ORDER BY NU_ANO DESC, CICLO DESC";
            sqlComando = new SqlCommand(sqlSelect, sqlConexion);
            sqlReader = sqlComando.ExecuteReader();

            DataTable sqlDataTable = new DataTable();
            if (sqlReader != null)
                sqlDataTable.Load(sqlReader);
            StringBuilder insertSql = new StringBuilder();

            using (SQLiteCommand sqlDelete = new SQLiteCommand("DELETE FROM " + nombreTablaSqlite, sqliteConexion))
            {
                sqlDelete.ExecuteNonQuery();
            }
            try
            {
                string[] sCampos = new string[sqlDataTable.Columns.Count];
                int i = 0;
                foreach (DataColumn columna in sqlDataTable.Columns)
                {
                    sCampos[i] = columna.ColumnName;
                    i++;
                }
                bool tieneCiclo = validarSiTieneCiclo(nombreTablaSqlite);
                int cont = 1;
                foreach (DataRow linea in sqlDataTable.Rows)
                {

                    String cadenaValores = "";
                    int indexCampo = 0;
                    insertSql.Clear();
                    insertSql.Append("INSERT INTO " + nombreTablaSqlite + " (");

                    foreach (string dataColumn in sCampos)
                    {

                        try
                        {
                            if (!dataColumn.Equals("NU_ANO") && !dataColumn.Equals("NU_CICLO"))
                            {
                                if (!dataColumn.Equals("CICLO") || (dataColumn.Equals("CICLO") && tieneCiclo))
                                {

                                    string valor = "";
                                    switch (sqlDataTable.Columns[dataColumn].DataType.Name)
                                    {

                                        case "Int16":
                                            if (sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO") || sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO_VISITADO"))
                                            {
                                                cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : Int16.Parse(linea[dataColumn].ToString());
                                            }
                                            else
                                            {
                                                cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : Int16.Parse(linea[dataColumn].ToString());
                                            }
                                            //cadenaValores += sqlDataTable.Columns[dataColumn] == "." ? 0 : Int16.Parse(sqlDataTable.Columns[dataColumn]);
                                            break;
                                        case "Int32":
                                            if (sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO") || sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO_VISITADO"))
                                            {
                                                cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : ciclo;
                                            }
                                            else
                                            {
                                                cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : Int32.Parse(linea[dataColumn].ToString());
                                            }
                                            //cadenaValores += sqlDataTable.Columns[dataColumn] == "." ? 0 : Int32.Parse(sqlDataTable.Columns[dataColumn]);
                                            break;
                                        case "Int64":
                                            if (sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO") || sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO_VISITADO"))
                                            {
                                                cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : ciclo;
                                            }
                                            else
                                            {
                                                cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : Int64.Parse(linea[dataColumn].ToString());
                                            }
                                            //cadenaValores += sqlDataTable.Columns[dataColumn] == "." ? 0 : Int64.Parse(sqlDataTable.Columns[dataColumn]);
                                            break;
                                        case "Boolean":
                                            cadenaValores += linea[dataColumn].ToString().Equals(".") ? false : Boolean.Parse(linea[dataColumn].ToString());
                                            break;
                                        case "Float":
                                            float mfloat = linea[dataColumn].ToString().Equals(".") ? 0 : float.Parse(linea[dataColumn].ToString());
                                            cadenaValores += Convert.ToString(mfloat).Replace(",", ".");
                                            break;
                                        case "Double":
                                            valor = linea[dataColumn].ToString();//.Replace(".", ",");
                                            float mdoble = valor == "," ? 0 : float.Parse(valor);
                                            cadenaValores += Convert.ToString(mdoble).Replace(",", ".");
                                            //cadenaValores += sqlDataTable.Columns[dataColumn] == "." ? 0 : Double.Parse(sqlDataTable.Columns[dataColumn]);
                                            break;
                                        case "Decimal":
                                            Decimal mdecimal = linea[dataColumn].ToString().Equals(".") ? 0 : Decimal.Parse(linea[dataColumn].ToString());
                                            cadenaValores += Convert.ToString(mdecimal).Replace(",", ".");
                                            //cadenaValores += sqlDataTable.Columns[dataColumn] == "." ? 0 : Decimal.Parse(sqlDataTable.Columns[dataColumn]);
                                            break;
                                        case "String":
                                            //cadenaValores += "'" + sqlDataTable.Columns[dataColumn] + "'";
                                            if (linea[dataColumn].ToString().Length > sqlDataTable.Columns[dataColumn].MaxLength)
                                            {
                                                cadenaValores += "'" + linea[dataColumn].ToString().Substring(0, sqlDataTable.Columns[dataColumn].MaxLength - 1).Trim().Replace("\n", "").Replace("\r", "") + "'";
                                            }
                                            else
                                            {
                                                cadenaValores += "'" + linea[dataColumn].ToString().Trim().Replace("\n", "").Replace("\r", "") + "'";
                                            }
                                            break;
                                        case "Date":
                                        case "DateTime":
                                            cadenaValores += "CONVERT(DATETIME,'" + linea[dataColumn].ToString() + "',103)";
                                            break;
                                        default:
                                            cadenaValores += "'" + linea[dataColumn].ToString() + "'";
                                            break;

                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            string error = e.Message + " " + dataColumn;
                            throw;
                        }
                        if (!dataColumn.Equals("NU_ANO") && !dataColumn.Equals("NU_CICLO"))
                        {
                            if (!dataColumn.Equals("CICLO") || (dataColumn.Equals("CICLO") && tieneCiclo))
                                if (indexCampo < sqlDataTable.Columns.Count - 1)
                                {
                                    cadenaValores += ", ";
                                }
                        }
                        indexCampo++;
                    }
                    string cadenaCampos = "";
                    for (int h = 0; h < sCampos.Length; h++)
                    {

                        if ((!sCampos[h].Equals("NU_ANO") && !sCampos[h].Equals("NU_CICLO")))
                        {
                            if (!sCampos[h].Equals("CICLO") || (sCampos[h].Equals("CICLO") && tieneCiclo))
                            {
                                cadenaCampos += sCampos[h];
                                if (h < sCampos.Length - 1)
                                {
                                    cadenaCampos += ", ";
                                }
                            }
                        }

                    }

                    insertSql.Append(cadenaCampos + ") VALUES (");
                    insertSql.Append(cadenaValores + ");");
                    insertSql.Replace(",.,", ",'.',");


                    try
                    {
                        SQLiteCommand sqlInsert = new SQLiteCommand(insertSql.ToString(), sqliteConexion);
                        int registroInsertado = sqlInsert.ExecuteNonQuery();

                        archivo.WriteLine(insertSql.ToString());
                        archivo.Flush();
                    }
                    catch (IOException ioe)
                    {
                        ioe.Message.ToString();
                        string cuerpo = "Este es un mensaje generado por el generador de bases de datos de MEDINET " + " <br /> " +
                                       "----------------------------------------------------------------------------------------------  " + " <br /> " +
                                       "<br /> " +
                                       "<b>Visitador:</b> " + miVisitador.nombre + " <br /> " +
                                       "<b>Tabla:</b> " + nombreTablaSqlite + " <br /> " +
                                       "<b>Error:</b> " + ioe.Message.ToString() + " <br /> " +
                                       "<b>Instrucción SQL:</b> " + insertSql.ToString() + " <br /> ";

                        registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={ioe.Message}");
                    }
                    catch (SqlException se)
                    {
                        se.Errors.ToString();
                        string cuerpo = "Este es un mensaje generado por el generador de bases de datos de MEDINET " + " <br /> " +
                                       "----------------------------------------------------------------------------------------------  " + " <br /> " +
                                       "<br /> " +
                                       "<b>Visitador:</b> " + miVisitador.nombre + " <br /> " +
                                       "<b>Tabla:</b> " + nombreTablaSqlite + " <br /> " +
                                       "<b>Error:</b> " + se.Message + " <br /> " +
                                       "<b>Numero de Error:</b> " + se.Number + " <br /> " +
                                       "<b>Instrucción SQL:</b> " + insertSql.ToString() + " <br /> ";

                        registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={se.Message}");
                    }
                    catch (SQLiteException sex)
                    {
                        sex.Message.ToString();
                        string cuerpo = "Este es un mensaje generado por el generador de bases de datos de MEDINET " + " <br /> " +
                                       "----------------------------------------------------------------------------------------------  " + " <br /> " +
                                       "<br /> " +
                                       "<b>Visitador:</b> " + miVisitador.nombre + " <br /> " +
                                       "<b>Tabla:</b> " + nombreTablaSqlite + " <br /> " +
                                       "<b>Error:</b> " + sex.Message + " <br /> " +
                                       "<b>Numero de Error:</b> " + sex.ErrorCode + " <br /> " +
                                       "<b>Instrucción SQL:</b> " + insertSql.ToString() + " <br /> ";

                        registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={sex.Message}");
                    }
                    catch (Exception e)
                    {
                        e.Message.ToString();
                        string cuerpo = "Este es un mensaje generado por el generador de bases de datos de MEDINET " + " <br /> " +
                                       "----------------------------------------------------------------------------------------------  " + " <br /> " +
                                       "<br /> " +
                                       "<b>Visitador:</b> " + miVisitador.nombre + " <br /> " +
                                       "<b>Tabla:</b> " + nombreTablaSqlite + " <br /> " +
                                       "<b>Error:</b> " + e.Message.ToString() + " <br /> " +
                                       "<b>Instrucción SQL:</b> " + insertSql.ToString() + " <br /> ";

                        registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={e.Message}");

                    }
                    cont++;
                }
            }




            catch (Exception e)
            {


                string cuerpo = "Este es un mensaje generado por el generador de bases de datos de MEDINET " + " <br /> " +
                               "----------------------------------------------------------------------------------------------  " + " <br /> " +
                               "<br /> " +
                               "<b>Visitador:</b> " + miVisitador.nombre + " <br /> " +
                               "<b>Tabla:</b> " + nombreTablaSqlite + " <br /> " +
                               "<b>Error:</b> " + e.Message + " <br /> " +

                               "<b>Instrucción SQL:</b> " + insertSql.ToString() + " <br /> ";

                registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={e.Message}");

            }
            sqlReader.Close();

            //archivo.Flush();
            //archivo.Close();
        }
        private void copiarData()
        {
            foreach (string tabla in listaDeTablasACopiar)
            {
                string nombreTablaSqlite = tabla.Remove(0, 3).ToLower();

                switch (tabla)
                {
                    case "MD_Hoja_Ruta_Propuesta":
                        sqlSelect = "SELECT HRPP.* " +
                                    "FROM MD_Hoja_Ruta_Propuesta HRPP " +
                                    "CROSS APPLY( " +
                                    "    SELECT TOP 1 CAST(CAST(NU_ANO AS varchar(4)) + CAST(CICLO AS varchar(2)) AS INT) NCICLO " +
                                    "    FROM MD_Hoja_Ruta_Propuesta HRP " +
                                    "    WHERE HRP.ZONA = HRPP.ZONA " +
                                    "    ORDER BY HRP.NU_ANO DESC, HRP.CICLO DESC " +
                                    ") M " +
                                    "WHERE HRPP.ZONA = '" + miVisitador.id + "' AND CAST(CAST(NU_ANO AS varchar(4)) +CAST(CICLO AS varchar(2)) AS INT) = M.NCICLO";
                        break;
                    default:
                        sqlSelect = "SELECT * FROM " + tabla + " WHERE NU_ANO=" + ano + " AND CICLO=" + ciclo + " AND ZONA='" + miVisitador.id + "'";
                        break;
                }
                //sqlSelect = "SELECT * FROM " + tabla + " WHERE NU_ANO=" + ano + " AND CICLO=" + ciclo + " AND ZONA='" + miVisitador.id + "'";
                sqlComando = new SqlCommand(sqlSelect, sqlConexion);
                sqlReader = sqlComando.ExecuteReader();

                DataTable sqlDataTable = new DataTable();
                if (sqlReader != null)
                    sqlDataTable.Load(sqlReader);
                StringBuilder insertSql = new StringBuilder();

                using (SQLiteCommand sqlDelete = new SQLiteCommand("DELETE FROM " + nombreTablaSqlite, sqliteConexion))
                {
                    sqlDelete.ExecuteNonQuery();
                }
                try
                {
                    string[] sCampos = new string[sqlDataTable.Columns.Count];
                    int i = 0;
                    foreach (DataColumn columna in sqlDataTable.Columns)
                    {
                        sCampos[i] = columna.ColumnName;
                        i++;
                    }
                    bool tieneCiclo = validarSiTieneCiclo(nombreTablaSqlite);
                    int cont = 1;
                    foreach (DataRow linea in sqlDataTable.Rows)
                    {

                        String cadenaValores = "";
                        int indexCampo = 0;
                        insertSql.Clear();
                        insertSql.Append("INSERT INTO " + nombreTablaSqlite + " (");

                        foreach (string dataColumn in sCampos)
                        {

                            try
                            {
                                if (!dataColumn.Equals("NU_ANO") && !dataColumn.Equals("NU_CICLO"))
                                {
                                    if (!dataColumn.Equals("CICLO") || (dataColumn.Equals("CICLO") && tieneCiclo))
                                    {

                                        string valor = "";
                                        switch (sqlDataTable.Columns[dataColumn].DataType.Name)
                                        {

                                            case "Int16":
                                                if (sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO") || sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO_VISITADO"))
                                                {
                                                    cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : ciclo;
                                                }
                                                else
                                                {
                                                    cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : Int16.Parse(linea[dataColumn].ToString());
                                                }
                                                //cadenaValores += sqlDataTable.Columns[dataColumn] == "." ? 0 : Int16.Parse(sqlDataTable.Columns[dataColumn]);
                                                break;
                                            case "Int32":
                                                if (sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO") || sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO_VISITADO"))
                                                {
                                                    cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : ciclo;
                                                }
                                                else
                                                {
                                                    cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : Int32.Parse(linea[dataColumn].ToString());
                                                }
                                                //cadenaValores += sqlDataTable.Columns[dataColumn] == "." ? 0 : Int32.Parse(sqlDataTable.Columns[dataColumn]);
                                                break;
                                            case "Int64":
                                                if (sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO") || sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO_VISITADO"))
                                                {
                                                    cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : ciclo;
                                                }
                                                else
                                                {
                                                    cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : Int64.Parse(linea[dataColumn].ToString());
                                                }
                                                //cadenaValores += sqlDataTable.Columns[dataColumn] == "." ? 0 : Int64.Parse(sqlDataTable.Columns[dataColumn]);
                                                break;
                                            case "Boolean":
                                                cadenaValores += linea[dataColumn].ToString().Equals(".") ? false : Boolean.Parse(linea[dataColumn].ToString());
                                                break;
                                            case "Float":
                                                float mfloat = linea[dataColumn].ToString().Equals(".") ? 0 : float.Parse(linea[dataColumn].ToString());
                                                cadenaValores += Convert.ToString(mfloat).Replace(",", ".");
                                                break;
                                            case "Double":
                                                valor = linea[dataColumn].ToString().Replace(".", ",");
                                                float mdoble = valor == "," ? 0 : float.Parse(valor);
                                                cadenaValores += Convert.ToString(mdoble).Replace(",", ".");
                                                //cadenaValores += sqlDataTable.Columns[dataColumn] == "." ? 0 : Double.Parse(sqlDataTable.Columns[dataColumn]);
                                                break;
                                            case "Decimal":
                                                Decimal mdecimal = linea[dataColumn].ToString().Equals(".") ? 0 : Decimal.Parse(linea[dataColumn].ToString());
                                                cadenaValores += Convert.ToString(mdecimal).Replace(",", ".");
                                                //cadenaValores += sqlDataTable.Columns[dataColumn] == "." ? 0 : Decimal.Parse(sqlDataTable.Columns[dataColumn]);
                                                break;
                                            case "String":
                                                //cadenaValores += "'" + sqlDataTable.Columns[dataColumn] + "'";
                                                if (linea[dataColumn].ToString().Length > sqlDataTable.Columns[dataColumn].MaxLength)
                                                {
                                                    cadenaValores += "'" + linea[dataColumn].ToString().Substring(0, sqlDataTable.Columns[dataColumn].MaxLength - 1).Trim().Replace("\n", "").Replace("\r", "") + "'";
                                                }
                                                else
                                                {
                                                    cadenaValores += "'" + linea[dataColumn].ToString().Trim().Replace("\n", "").Replace("\r", "") + "'";
                                                }
                                                break;
                                            case "Date":
                                            case "DateTime":
                                                cadenaValores += "CONVERT(DATETIME,'" + linea[dataColumn].ToString() + "',103)";
                                                break;
                                            default:
                                                cadenaValores += "'" + linea[dataColumn].ToString() + "'";
                                                break;

                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                string error = e.Message + " " + dataColumn;
                                throw new Exception(error);
                            }
                            if (!dataColumn.Equals("NU_ANO") && !dataColumn.Equals("NU_CICLO"))
                            {
                                if (!dataColumn.Equals("CICLO") || (dataColumn.Equals("CICLO") && tieneCiclo))
                                    if (indexCampo < sqlDataTable.Columns.Count - 1)
                                    {
                                        cadenaValores += ", ";
                                    }
                            }
                            indexCampo++;
                        }
                        string cadenaCampos = "";
                        for (int h = 0; h < sCampos.Length; h++)
                        {

                            if ((!sCampos[h].Equals("NU_ANO") && !sCampos[h].Equals("NU_CICLO")))
                            {
                                if (!sCampos[h].Equals("CICLO") || (sCampos[h].Equals("CICLO") && tieneCiclo))
                                {
                                    cadenaCampos += sCampos[h];
                                    if (h < sCampos.Length - 1)
                                    {
                                        cadenaCampos += ", ";
                                    }
                                }
                            }

                        }

                        insertSql.Append(cadenaCampos + ") VALUES (");
                        insertSql.Append(cadenaValores + ");");
                        insertSql.Replace(",.,", ",'.',");


                        try
                        {
                            SQLiteCommand sqlInsert = new SQLiteCommand(insertSql.ToString(), sqliteConexion);
                            int registroInsertado = sqlInsert.ExecuteNonQuery();

                            archivo.WriteLine(insertSql.ToString());
                            archivo.Flush();
                        }
                        catch (IOException ioe)
                        {
                            ioe.Message.ToString();
                            string cuerpo = "Este es un mensaje generado por el generador de bases de datos de MEDINET " + " <br /> " +
                                           "----------------------------------------------------------------------------------------------  " + " <br /> " +
                                           "<br /> " +
                                           "<b>Visitador:</b> " + miVisitador.nombre + " <br /> " +
                                           "<b>Tabla:</b> " + nombreTablaSqlite + " <br /> " +
                                           "<b>Error:</b> " + ioe.Message.ToString() + " <br /> " +
                                           "<b>Instrucción SQL:</b> " + insertSql.ToString() + " <br /> ";

                            registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={ioe.Message}");
                        }
                        catch (SqlException se)
                        {
                            se.Errors.ToString();
                            string cuerpo = "Este es un mensaje generado por el generador de bases de datos de MEDINET " + " <br /> " +
                                           "----------------------------------------------------------------------------------------------  " + " <br /> " +
                                           "<br /> " +
                                           "<b>Visitador:</b> " + miVisitador.nombre + " <br /> " +
                                           "<b>Tabla:</b> " + nombreTablaSqlite + " <br /> " +
                                           "<b>Error:</b> " + se.Message + " <br /> " +
                                           "<b>Numero de Error:</b> " + se.Number + " <br /> " +
                                           "<b>Instrucción SQL:</b> " + insertSql.ToString() + " <br /> ";

                            registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={se.Message}");
                        }
                        catch (SQLiteException sex)
                        {
                            sex.Message.ToString();
                            string cuerpo = "Este es un mensaje generado por el generador de bases de datos de MEDINET " + " <br /> " +
                                           "----------------------------------------------------------------------------------------------  " + " <br /> " +
                                           "<br /> " +
                                           "<b>Visitador:</b> " + miVisitador.nombre + " <br /> " +
                                           "<b>Tabla:</b> " + nombreTablaSqlite + " <br /> " +
                                           "<b>Error:</b> " + sex.Message + " <br /> " +
                                           "<b>Numero de Error:</b> " + sex.ErrorCode + " <br /> " +
                                           "<b>Instrucción SQL:</b> " + insertSql.ToString() + " <br /> ";

                            registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={sex.Message}");
                        }
                        catch (Exception e)
                        {
                            e.Message.ToString();
                            string cuerpo = "Este es un mensaje generado por el generador de bases de datos de MEDINET " + " <br /> " +
                                           "----------------------------------------------------------------------------------------------  " + " <br /> " +
                                           "<br /> " +
                                           "<b>Visitador:</b> " + miVisitador.nombre + " <br /> " +
                                           "<b>Tabla:</b> " + nombreTablaSqlite + " <br /> " +
                                           "<b>Error:</b> " + e.Message.ToString() + " <br /> " +
                                           "<b>Instrucción SQL:</b> " + insertSql.ToString() + " <br /> ";

                            registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={e.Message}");

                        }
                        cont++;
                    }
                }
                catch (Exception e)
                {
                    string cuerpo = "Este es un mensaje generado por el generador de bases de datos de MEDINET " + " <br /> " +
               "----------------------------------------------------------------------------------------------  " + " <br /> " +
               "<br /> " +
               "<b>Visitador:</b> " + miVisitador.nombre + " <br /> " +
               "<b>Tabla:</b> " + nombreTablaSqlite + " <br /> " +
               "<b>Error:</b> " + e.Message + " <br /> " +

               "<b>Instrucción SQL:</b> " + insertSql.ToString() + " <br /> ";

                    registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={e.Message}");

                }
                sqlReader.Close();
            }
            //archivo.Flush();
            //archivo.Close();
        }

        private void copiarDataHistorica()
        {
            foreach (string tabla in listaDeTablasACopiarHistorica)
            {
                string nombreTablaSqlite = "h" + tabla.Remove(0, 3).ToLower();
                using (SQLiteCommand sqlDelete = new SQLiteCommand("DELETE FROM " + nombreTablaSqlite, sqliteConexion))
                {
                    sqlDelete.ExecuteNonQuery();
                }
                int cicloHistorico = ciclo == 1 ? 12 : ciclo;

                for (int cicloR = 1; cicloR < 3; cicloR++)
                {
                    cicloHistorico -= 1;
                    cicloHistorico = cicloHistorico == -1 ? 11 : cicloHistorico;
                    int anoTemp = (cicloHistorico == 10 || cicloHistorico == 11) && (ciclo == 1 || ciclo == 2) ? ano - 1 : ano;

                    sqlSelect = "SELECT * FROM " + tabla + " WHERE NU_ANO=" + anoTemp + " AND CICLO=" + cicloHistorico + " AND ZONA='" + miVisitador.id + "'";
                    sqlComando = new SqlCommand(sqlSelect, sqlConexion);
                    sqlReader = sqlComando.ExecuteReader();

                    DataTable sqlDataTable = new DataTable();
                    if (sqlReader.Read())
                        sqlDataTable.Load(sqlReader);

                    StringBuilder insertSql = new StringBuilder();

                    try
                    {
                        string[] sCampos = new string[sqlDataTable.Columns.Count];
                        int i = 0;
                        foreach (DataColumn columna in sqlDataTable.Columns)
                        {
                            sCampos[i] = columna.ColumnName;
                            i++;
                        }
                        bool tieneCiclo = validarSiTieneCiclo(nombreTablaSqlite);
                        int cont = 1;
                        foreach (DataRow linea in sqlDataTable.Rows)
                        {

                            String cadenaValores = "";
                            int indexCampo = 0;
                            insertSql.Clear();
                            insertSql.Append("INSERT INTO " + nombreTablaSqlite + " (");

                            foreach (string dataColumn in sCampos)
                            {

                                try
                                {
                                    if (!dataColumn.Equals("NU_ANO") && !dataColumn.Equals("NU_CICLO"))
                                    {
                                        if (!dataColumn.Equals("CICLO") || (dataColumn.Equals("CICLO") && tieneCiclo))
                                        {

                                            string valor = "";
                                            switch (sqlDataTable.Columns[dataColumn].DataType.Name)
                                            {

                                                case "Int16":
                                                    if (sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO") || sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO_VISITADO"))
                                                    {
                                                        cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : cicloHistorico;
                                                    }
                                                    else
                                                    {
                                                        cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : Int16.Parse(linea[dataColumn].ToString());
                                                    }
                                                    //cadenaValores += sqlDataTable.Columns[dataColumn] == "." ? 0 : Int16.Parse(sqlDataTable.Columns[dataColumn]);
                                                    break;
                                                case "Int32":
                                                    if (sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO") || sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO_VISITADO"))
                                                    {
                                                        cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : cicloHistorico;
                                                    }
                                                    else
                                                    {
                                                        cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : Int32.Parse(linea[dataColumn].ToString());
                                                    }
                                                    //cadenaValores += sqlDataTable.Columns[dataColumn] == "." ? 0 : Int32.Parse(sqlDataTable.Columns[dataColumn]);
                                                    break;
                                                case "Int64":
                                                    if (sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO") || sqlDataTable.Columns[dataColumn].ColumnName.ToUpper().Equals("CICLO_VISITADO"))
                                                    {
                                                        cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : cicloHistorico;
                                                    }
                                                    else
                                                    {
                                                        cadenaValores += linea[dataColumn].ToString().Equals(".") ? 0 : Int64.Parse(linea[dataColumn].ToString());
                                                    }
                                                    //cadenaValores += sqlDataTable.Columns[dataColumn] == "." ? 0 : Int64.Parse(sqlDataTable.Columns[dataColumn]);
                                                    break;
                                                case "Boolean":
                                                    cadenaValores += linea[dataColumn].ToString().Equals(".") ? false : Boolean.Parse(linea[dataColumn].ToString());
                                                    break;
                                                case "Float":
                                                    float mfloat = linea[dataColumn].ToString().Equals(".") ? 0 : float.Parse(linea[dataColumn].ToString());
                                                    cadenaValores += Convert.ToString(mfloat).Replace(",", ".");
                                                    break;
                                                case "Double":
                                                    valor = linea[dataColumn].ToString().Replace(".", ",");
                                                    float mdoble = valor == "," ? 0 : float.Parse(valor);
                                                    cadenaValores += Convert.ToString(mdoble).Replace(",", ".");
                                                    //cadenaValores += sqlDataTable.Columns[dataColumn] == "." ? 0 : Double.Parse(sqlDataTable.Columns[dataColumn]);
                                                    break;
                                                case "Decimal":
                                                    Decimal mdecimal = linea[dataColumn].ToString().Equals(".") ? 0 : Decimal.Parse(linea[dataColumn].ToString());
                                                    cadenaValores += Convert.ToString(mdecimal).Replace(",", ".");
                                                    //cadenaValores += sqlDataTable.Columns[dataColumn] == "." ? 0 : Decimal.Parse(sqlDataTable.Columns[dataColumn]);
                                                    break;
                                                case "String":
                                                    //cadenaValores += "'" + sqlDataTable.Columns[dataColumn] + "'";
                                                    if (linea[dataColumn].ToString().Length > sqlDataTable.Columns[dataColumn].MaxLength)
                                                    {
                                                        cadenaValores += "'" + linea[dataColumn].ToString().Substring(0, sqlDataTable.Columns[dataColumn].MaxLength - 1).Trim().Replace("\n", "").Replace("\r", "") + "'";
                                                    }
                                                    else
                                                    {
                                                        cadenaValores += "'" + linea[dataColumn].ToString().Trim().Replace("\n", "").Replace("\r", "") + "'";
                                                    }
                                                    break;
                                                case "Date":
                                                case "DateTime":
                                                    cadenaValores += "CONVERT(DATETIME,'" + linea[dataColumn].ToString() + "',103)";
                                                    break;
                                                default:
                                                    cadenaValores += "'" + linea[dataColumn].ToString() + "'";
                                                    break;

                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    string error = e.Message + " " + dataColumn;
                                    registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={e.Message}-Campo={dataColumn}");

                                    throw;
                                }
                                if (!dataColumn.Equals("NU_ANO") && !dataColumn.Equals("NU_CICLO"))
                                {
                                    if (!dataColumn.Equals("CICLO") || (dataColumn.Equals("CICLO") && tieneCiclo))
                                        if (indexCampo < sqlDataTable.Columns.Count - 1)
                                        {
                                            cadenaValores += ", ";
                                        }
                                }
                                indexCampo++;
                            }
                            string cadenaCampos = "";
                            for (int h = 0; h < sCampos.Length; h++)
                            {

                                if ((!sCampos[h].Equals("NU_ANO") && !sCampos[h].Equals("NU_CICLO")))
                                {
                                    if (!sCampos[h].Equals("CICLO") || (sCampos[h].Equals("CICLO") && tieneCiclo))
                                    {
                                        cadenaCampos += sCampos[h];
                                        if (h < sCampos.Length - 1)
                                        {
                                            cadenaCampos += ", ";
                                        }
                                    }
                                }

                            }

                            insertSql.Append(cadenaCampos + ") VALUES (");
                            insertSql.Append(cadenaValores + ");");
                            insertSql.Replace(",.,", ",'.',");


                            try
                            {
                                SQLiteCommand sqlInsert = new SQLiteCommand(insertSql.ToString(), sqliteConexion);
                                int registroInsertado = sqlInsert.ExecuteNonQuery();

                                archivo.WriteLine(insertSql.ToString());
                                archivo.Flush();
                            }
                            catch (IOException ioe)
                            {
                                ioe.Message.ToString();
                                string cuerpo = "Este es un mensaje generado por el generador de bases de datos de MEDINET " + " <br /> " +
                                               "----------------------------------------------------------------------------------------------  " + " <br /> " +
                                               "<br /> " +
                                               "<b>Visitador:</b> " + miVisitador.nombre + " <br /> " +
                                               "<b>Tabla:</b> " + nombreTablaSqlite + " <br /> " +
                                               "<b>Error:</b> " + ioe.Message.ToString() + " <br /> " +
                                               "<b>Instrucción SQL:</b> " + insertSql.ToString() + " <br /> ";

                                registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={ioe.Message}");
                            }
                            catch (SqlException se)
                            {
                                se.Errors.ToString();
                                string cuerpo = "Este es un mensaje generado por el generador de bases de datos de MEDINET " + " <br /> " +
                                               "----------------------------------------------------------------------------------------------  " + " <br /> " +
                                               "<br /> " +
                                               "<b>Visitador:</b> " + miVisitador.nombre + " <br /> " +
                                               "<b>Tabla:</b> " + nombreTablaSqlite + " <br /> " +
                                               "<b>Error:</b> " + se.Message + " <br /> " +
                                               "<b>Numero de Error:</b> " + se.Number + " <br /> " +
                                               "<b>Instrucción SQL:</b> " + insertSql.ToString() + " <br /> ";

                                registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={se.Message}");
                            }
                            catch (SQLiteException sex)
                            {
                                sex.Message.ToString();
                                string cuerpo = "Este es un mensaje generado por el generador de bases de datos de MEDINET " + " <br /> " +
                                               "----------------------------------------------------------------------------------------------  " + " <br /> " +
                                               "<br /> " +
                                               "<b>Visitador:</b> " + miVisitador.nombre + " <br /> " +
                                               "<b>Tabla:</b> " + nombreTablaSqlite + " <br /> " +
                                               "<b>Error:</b> " + sex.Message + " <br /> " +
                                               "<b>Numero de Error:</b> " + sex.ErrorCode + " <br /> " +
                                               "<b>Instrucción SQL:</b> " + insertSql.ToString() + " <br /> ";

                                registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={sex.Message}");
                            }
                            catch (Exception e)
                            {
                                e.Message.ToString();
                                string cuerpo = "Este es un mensaje generado por el generador de bases de datos de MEDINET " + " <br /> " +
                                               "----------------------------------------------------------------------------------------------  " + " <br /> " +
                                               "<br /> " +
                                               "<b>Visitador:</b> " + miVisitador.nombre + " <br /> " +
                                               "<b>Tabla:</b> " + nombreTablaSqlite + " <br /> " +
                                               "<b>Error:</b> " + e.Message.ToString() + " <br /> " +
                                               "<b>Instrucción SQL:</b> " + insertSql.ToString() + " <br /> ";

                                registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={e.Message}");

                            }
                            cont++;
                        }
                    }




                    catch (Exception e)
                    {


                        string cuerpo = "Este es un mensaje generado por el generador de bases de datos de MEDINET " + " <br /> " +
                                       "----------------------------------------------------------------------------------------------  " + " <br /> " +
                                       "<br /> " +
                                       "<b>Visitador:</b> " + miVisitador.nombre + " <br /> " +
                                       "<b>Tabla:</b> " + nombreTablaSqlite + " <br /> " +
                                       "<b>Error:</b> " + e.Message + " <br /> " +

                                       "<b>Instrucción SQL:</b> " + insertSql.ToString() + " <br /> ";

                        registrarError(GetCurrentMethodName(), $"Tabla={nombreTablaSqlite}, Error={e.Message}");

                    }
                    sqlReader.Close();
                }
            }
            archivo.Flush();
            archivo.Close();
        }
        public bool registrarError(string lugar, string error)
        {
            #region Emails
            ////SmtpClient smtpClient = new SmtpClient("autodiscover.zuoz.com", 25);

            ////smtpClient.Credentials = new System.Net.NetworkCredential("zuoz-ccs\\rvelazquez", "Sebastian01");
            ////smtpClient.UseDefaultCredentials = false;
            ////smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //////smtpClient.EnableSsl = true;
            ////MailMessage mail = new MailMessage();


            //////Setting From , To and CC
            ////mail.From = new MailAddress("rvelazquez@zuoz.com", "MEDINETLA");
            ////mail.To.Add(new MailAddress("ronnervelazquez@gmail.com"));
            ////mail.CC.Add(new MailAddress("rvelazquez@zuoz.com"));
            ////mail.Subject = "Probando";
            ////mail.Body = "Este es el texto";
            ////mail.IsBodyHtml = true;
            ////smtpClient.Send(mail);

            //GMAIL
            //MailMessage o = new MailMessage("ronnervelazquez@gmail.com", "ronnervelazquez@gmail.com", "Subject", "<font color ='#cc55cc'>Body</font>");
            //NetworkCredential netCred = new NetworkCredential("ronnervelazquez@gmail.com", "1986revs");
            //SmtpClient smtpobj = new SmtpClient("smtp.gmail.com", 587);
            //smtpobj.EnableSsl = true;
            //smtpobj.Credentials = netCred;
            //o.IsBodyHtml = true;

            //HOTMAIL
            //MailMessage o = new MailMessage("revs1986@hotmail.com", "ronnervelazquez@gmail.com", "Subject", "<font color ='#cc55cc'>Body</font>");
            //NetworkCredential netCred = new NetworkCredential("revs1986@hotmail.com", "Revs0509");
            //SmtpClient smtpobj = new SmtpClient("smtp.live.com", 25);
            //smtpobj.EnableSsl = true;
            //smtpobj.Credentials = netCred;
            //o.IsBodyHtml = true;

            //var message = new MailMessage("rvelazquez@zuoz.com", "rvelazquez@zuoz.com")
            //{
            //    Subject = "Prueba",
            //    Body = "Este es un mensaje generado",
            //    IsBodyHtml = true,

            //};

            //NetworkCredential SMTPUserInfo = new NetworkCredential("rvelazquez@zuoz.com", "Sebastian01");
            //var client = new SmtpClient("autodiscover.zuoz.com", 25);
            //client.UseDefaultCredentials = true;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.Credentials = SMTPUserInfo;
            //client.Send(message); 
            #endregion

            //EXCHANGE (SOLO FUNCIONA CON CORREOS INTERNOS)
            //MailMessage mensajeMail = new MailMessage(new MailAddress("rvelazquez@zuoz.com", "Servicio Receptor MEDINET").ToString(), "rvelazquez@zuoz.com", asunto, cuerpo);
            //NetworkCredential credencialesDeRed = new System.Net.NetworkCredential("zuoz-ccs/rvelazquez", "Sebastian01");
            //SmtpClient clienteSMTP = new SmtpClient("autodiscover.zuoz.com", 25);
            //clienteSMTP.EnableSsl = false;
            //clienteSMTP.Credentials = credencialesDeRed;
            //clienteSMTP.UseDefaultCredentials = false;
            //mensajeMail.IsBodyHtml = true;
            //mensajeMail.Priority = MailPriority.High;


            //try
            //{
            //    clienteSMTP.Send(mensajeMail);
            //    //smtpClient.Send(mail);
            //}
            //catch (SmtpException se)
            //{
            //    se.Message.ToString();
            //}

            errores.Add($"Lugar={lugar} | {error}");
            return true;
        }
        public void terminarConexiones()
        {
            sqlConexion.Close();
            sqliteConexion.Close();
            sqliteConexion.Dispose();
            SQLiteConnection.ClearAllPools();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        private bool validarSiTieneCiclo(string nombreTabla)
        {
            bool tiene = false;
            switch (nombreTabla)
            {
                case "farmacias":
                case "fichero_farmacias":
                case "fichero_hospital":
                case "hoja_ruta":
                case "hoja_ruta_propuesta":
                case "hospital":
                case "solicitudes":
                case "visitas":
                case "visita_detalles":
                case "hvisitas":
                case "hvisita_detalles":
                case "hsolicitudes":
                case "cierreciclo":
                case "ayuda_visual":


                    tiene = true;
                    break;

            }
            return tiene;
        }
        private string convertirStatus(int estatus)
        {
            string estatusLetra = "";
            switch (estatus)
            {
                case 1:
                    estatusLetra = "A";
                    break;
                case 2:
                    estatusLetra = "C";
                    break;
                case 3:
                    estatusLetra = "P";
                    break;
            }
            return estatusLetra;
        }
    }
}
