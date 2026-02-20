using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class GeneradorService
    {
        private readonly string _connectionString = string.Empty;
        private readonly DataService _dataService;

        public GeneradorService(IConfiguration configuration, DataService dataService)
        {
            _connectionString = configuration.GetConnectionString("MedinetWeb") ?? string.Empty;
            _dataService = dataService;
        }

        public async Task<string> GenerarCarteraAsync(long visitadorId, int ano, int ciclo, bool limpia = false, bool cicloAbierto = false)
        {
            var visitador = await _dataService.BuscarVisitadorAsync(visitadorId);
            if (visitador == null)
            {
                throw new Exception($"No se encontró el visitador con ID {visitadorId}");
            }

            var contenido = new StringBuilder();
            await using var sqlConnection = new SqlConnection(_connectionString);
            await sqlConnection.OpenAsync();
            
            await GenerarContenidoCarteraAsync(sqlConnection, contenido, visitador, ano, ciclo, limpia, cicloAbierto);
            
            return contenido.ToString();
        }

        private async Task GenerarContenidoCarteraAsync(SqlConnection sqlConnection, StringBuilder contenido, Visitador visitador, int ano, int ciclo, bool limpia, bool cicloAbierto)
        {
            // Primero generar las definiciones de esquema
            GenerarEsquemaTablas(contenido);
            
            long visitadorId = visitador.ID_VisitadoresHistorial;

            var tablasACopiar = new List<string>
            {
                "MD_Farmacias", "MD_Farmacias_Personal", "MD_Fichero", "MD_Fichero_Farmacias",
                "MD_Fichero_Horarios", "MD_Fichero_Hospital", "MD_Hoja_Ruta", "MD_Hoja_Ruta_Propuesta", "MD_Hospital"
            };

            if (!limpia)
            {
                tablasACopiar.AddRange(new[]
                {
                    "MD_Eliminar", "MD_Farmacias_Detalles", "MD_Farmacias_Detalles_Productos", "MD_HistorialConceptoDias",
                    "MD_Hospital_Detalles", "MD_Hospital_Detalles_Medicos", "MD_Inclusiones", "MD_Solicitudes",
                    "MD_Visita_Detalles", "MD_Visitas", "MD_Ayuda_Visual", "MD_Ayuda_Visual_FE",
                    "MD_Ayuda_Visual_MP4", "MD_Ayuda_Visual_MP4_FE"
                });
            }

            foreach (var tabla in tablasACopiar)
            {
                await ObtenerDatosTablaAsync(sqlConnection, contenido, tabla, visitadorId, ano, ciclo);
            }

            // Agregar datos estáticos de puntos
            contenido.AppendLine("-- Datos estáticos de puntos");
            contenido.AppendLine("INSERT INTO puntos (TX_Concepto, NU_Puntos) VALUES ('RE', 9);");
            contenido.AppendLine("INSERT INTO puntos (TX_Concepto, NU_Puntos) VALUES ('C3', 1);");
            contenido.AppendLine("INSERT INTO puntos (TX_Concepto, NU_Puntos) VALUES ('C2', 2);");
            contenido.AppendLine("INSERT INTO puntos (TX_Concepto, NU_Puntos) VALUES ('C1', 3);");
            contenido.AppendLine("INSERT INTO puntos (TX_Concepto, NU_Puntos) VALUES ('B3', 4);");
            contenido.AppendLine("INSERT INTO puntos (TX_Concepto, NU_Puntos) VALUES ('B2', 5);");
            contenido.AppendLine("INSERT INTO puntos (TX_Concepto, NU_Puntos) VALUES ('B1', 6);");
            contenido.AppendLine("INSERT INTO puntos (TX_Concepto, NU_Puntos) VALUES ('A3', 7);");
            contenido.AppendLine("INSERT INTO puntos (TX_Concepto, NU_Puntos) VALUES ('A2', 8);");
            contenido.AppendLine("INSERT INTO puntos (TX_Concepto, NU_Puntos) VALUES ('A1', 9);");

            if (!limpia)
            {
                await GenerarContenidoCierreCiclosAsync(sqlConnection, contenido, visitadorId, ano, ciclo);
                await GenerarContenidoHistoricoAsync(sqlConnection, contenido, visitadorId, ano, ciclo);
            }

            await GenerarOtrosPasosAsync(sqlConnection, contenido, visitador, ano, ciclo, limpia, cicloAbierto);
        }
        
        private async Task GenerarOtrosPasosAsync(SqlConnection sqlConnection, StringBuilder contenido, Visitador visitador, int ano, int ciclo, bool limpia, bool cicloAbierto)
        {
            contenido.AppendLine("-- INICIO: Pasos finales de configuración");

            string nombreCorto = visitador.TX_Nombre.Length > 4 ? visitador.TX_Nombre.Substring(0, 4) : visitador.TX_Nombre;
            contenido.AppendLine($"INSERT INTO identificacion (NOMBRE,CLAVE,ZONA,REGION,LINEA,S_CLAVE,S_NOMBRE,A_CLAVE,A_NOMBRE,ESPERA_TONO) VALUES ('{visitador.TX_Nombre.Replace("'", "''")}','{nombreCorto}',{visitador.ID_VisitadoresHistorial},{visitador.ID_Empresa},{visitador.ID_Linea},'.','.','.','.','1');");
            
            contenido.AppendLine($"UPDATE Farmacias SET CICLO={ciclo};");
            contenido.AppendLine($"UPDATE Fichero_Hospital SET CICLO={ciclo};");
            contenido.AppendLine($"UPDATE Fichero_Farmacias SET CICLO={ciclo};");
            contenido.AppendLine($"UPDATE Hoja_Ruta SET CICLO={ciclo};");
            contenido.AppendLine($"UPDATE Hoja_Ruta_Propuesta SET CICLO={ciclo};");
            contenido.AppendLine($"UPDATE Hospital SET CICLO={ciclo};");
            contenido.AppendLine("DELETE FROM contador;");
            contenido.AppendLine("INSERT INTO contador VALUES(0);");
            contenido.AppendLine("UPDATE contador SET contador=(SELECT MAX(cast(REGISTRO as integer))+1 FROM FICHERO);");

            // UPDATE statements críticos para la app Android
            contenido.AppendLine($"UPDATE Farmacias SET CICLO={ciclo};");
            contenido.AppendLine($"UPDATE Fichero_Hospital SET CICLO={ciclo};");
            contenido.AppendLine($"UPDATE Fichero_Farmacias SET CICLO={ciclo};");
            contenido.AppendLine($"UPDATE Hoja_Ruta SET CICLO={ciclo};");
            contenido.AppendLine($"UPDATE Hoja_Ruta_Propuesta SET CICLO={ciclo};");
            contenido.AppendLine($"UPDATE Hospital SET CICLO={ciclo};");

            if (limpia)
            {
                contenido.AppendLine("DELETE FROM Farmacias_Detalles;");
                contenido.AppendLine("DELETE FROM Hospital_Detalles;");
                contenido.AppendLine("DELETE FROM Visita_Detalles;");
                contenido.AppendLine("DELETE FROM Visitas;");
                contenido.AppendLine("DELETE FROM HVisita_Detalles;");
                contenido.AppendLine("DELETE FROM HVisitas;");
                contenido.AppendLine("DELETE FROM Solicitudes;");
                contenido.AppendLine("DELETE FROM HSolicitudes;");
                contenido.AppendLine("DELETE FROM Eliminar;");
                contenido.AppendLine(@"UPDATE Fichero SET CICLO_C=0, REVISITA_C=0, FECHA_AGENDA='.', SEMANA_AGENDA=0, AMPM_AGENDA='.',NRO_FECHA_AGENDA=0,CICLO_AGENDA=0,RFECHA_AGENDA='.', RSEMANA_AGENDA=0, RAMPM_AGENDA='.',RNRO_FECHA_AGENDA=0,RCICLO_AGENDA=0, HORA='XX:XX';");
            }
            
            if (cicloAbierto)
            {
                contenido.AppendLine("-- Poblando tabla de Ciclos");
                await using var sqlCommand = new SqlCommand("SELECT * FROM MW_Ciclos WHERE NU_Ano=" + ano, sqlConnection);
                await using var reader = await sqlCommand.ExecuteReaderAsync();
                while(await reader.ReadAsync())
                {
                    string estatus = (reader.GetInt16(2) < ciclo) ? "C" : (reader.GetInt16(2) == ciclo) ? "A" : "P";
                    int anoValue = reader.GetInt16(1); // NU_Ano
                    int kpiVisitaMedica = reader.GetInt32(13);
                    int kpiVisitaFarmacia = reader.GetInt32(14);
                    contenido.AppendLine($"INSERT INTO Ciclos (FECHAI_CICLO,FECHAF_CICLO,NRO_CICLO,CICLO_CERRADO,DIAS_HABILES,ESTATUS,ANO,KPI_Visita_Medica,KPI_Visita_Farmacia) VALUES ('{reader.GetDateTime(3):dd/MM/yyyy}','{reader.GetDateTime(4):dd/MM/yyyy}',{reader.GetInt16(2)},'{estatus}',{reader.GetInt16(7)},'{estatus}',{anoValue},{kpiVisitaMedica},{kpiVisitaFarmacia});");
                }
            }
            contenido.AppendLine("-- FIN: Pasos finales de configuración");
        }

        private async Task GenerarContenidoCierreCiclosAsync(SqlConnection sql, StringBuilder contenido, long visitadorId, int ano, int ciclo)
        {
            contenido.AppendLine("-- Datos de Cierre de Ciclo");
            string query = @"SELECT * FROM [MD_CierreCiclo] 
                             WHERE (ZONA = CAST(@visitadorId AS VARCHAR(10)) AND (NU_ANO = @ano - 1 AND CICLO >= @ciclo)) 
                                OR (ZONA = CAST(@visitadorId AS VARCHAR(10)) AND NU_ANO = @ano)  
                             ORDER BY NU_ANO DESC, CICLO DESC";
            await ObtenerDatosTablaAsync(sql, contenido, "MD_CierreCiclo", visitadorId, ano, ciclo, query, "cierreciclo");
        }

        private async Task GenerarContenidoHistoricoAsync(SqlConnection sql, StringBuilder contenido, long visitadorId, int ano, int ciclo)
        {
            contenido.AppendLine("-- Datos Históricos");
            var tablas = new List<string>
            {
                "MD_Visita_Detalles", "MD_Visitas", "MD_Solicitudes", "MD_Hospital_Detalles",
                "MD_Hospital_Detalles_Medicos", "MD_Farmacias_Detalles", "MD_Farmacias_Detalles_Productos"
            };

            int cicloHistorico = ciclo;
            for (int i = 0; i < 2; i++) 
            {
                cicloHistorico--;
                if (cicloHistorico == 0) cicloHistorico = 12;
                int anoTemp = (cicloHistorico >= 11) && (ciclo <= 2) ? ano - 1 : ano;
                foreach (var tabla in tablas)
                {
                    await ObtenerDatosTablaAsync(sql, contenido, tabla, visitadorId, anoTemp, cicloHistorico, null, "h" + tabla.Remove(0, 3).ToLower());
                }
            }
        }

        private string FormatearValor(object valor, Type tipo, string columnName, int ciclo)
        {
            if (valor == DBNull.Value || valor == null)
            {
                return "NULL";
            }

            switch (Type.GetTypeCode(tipo))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    // La columna Zona debe formatearse como texto aunque sea numérica en SQL Server
                    if (columnName.Equals("Zona", StringComparison.Ordinal))
                    {
                        return "'" + valor.ToString() + "'";
                    }
                    if (columnName.Equals("CICLO", StringComparison.OrdinalIgnoreCase) || columnName.Equals("CICLO_VISITADO", StringComparison.OrdinalIgnoreCase))
                    {
                        return ciclo.ToString();
                    }
                    return valor.ToString();
                case TypeCode.Boolean:
                    return (bool)valor ? "1" : "0";
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    // Asegurar que SIEMPRE use punto como separador decimal
                    var decimalValue = Convert.ToDecimal(valor);
                    return decimalValue.ToString("0.##", CultureInfo.InvariantCulture);
                case TypeCode.String:
                    var stringValue = valor.ToString().Replace("'", "''")
                                                      .Replace("\n", " ")
                                                      .Replace("\r", " ")
                                                      .Trim();
                    // Si el string parece un número decimal con coma, convertirlo a punto
                    if (decimal.TryParse(stringValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedDecimal))
                    {
                        return parsedDecimal.ToString("0.##", CultureInfo.InvariantCulture);
                    }
                    return "'" + stringValue + "'";
                case TypeCode.DateTime:
                     return "'" + ((DateTime)valor).ToString("dd/MM/yyyy") + "'";
                default:
                    var defaultValue = valor.ToString().Replace("'", "''")
                                                       .Replace("\n", " ")
                                                       .Replace("\r", " ")
                                                       .Trim();
                    // Verificar si es un número decimal disfrazado
                    if (decimal.TryParse(defaultValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal defaultDecimal))
                    {
                        return defaultDecimal.ToString("0.##", CultureInfo.InvariantCulture);
                    }
                    return "'" + defaultValue + "'";
            }
        }

        private async Task ObtenerDatosTablaAsync(SqlConnection sqlConn, StringBuilder contenido, string nombreTabla, long visitadorId, int ano, int ciclo, string queryOverride = null, string nombreTablaSqliteOverride = null)
        {
            string query = queryOverride ?? GetDefaultQuery(nombreTabla);
            
            await using var command = new SqlCommand(query, sqlConn);
            if (query.Contains("@visitadorId")) command.Parameters.AddWithValue("@visitadorId", visitadorId.ToString());
            if (query.Contains("@ano")) command.Parameters.AddWithValue("@ano", ano);
            if (query.Contains("@ciclo")) command.Parameters.AddWithValue("@ciclo", ciclo);

            await using var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows) return;

            var columnNames = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
            string nombreTablaSqlite = nombreTablaSqliteOverride ?? nombreTabla.Remove(0, 3).ToLower();
            
            var finalColumns = new List<string>();
            foreach (var col in columnNames)
            {
                // Excluir NU_ANO y NU_CICLO siempre
                if (col.Equals("NU_ANO", StringComparison.OrdinalIgnoreCase) || 
                    col.Equals("NU_CICLO", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                
                // Para cierreciclo, solicitudes, hsolicitudes, visitas, hvisitas, hoja_ruta y hoja_ruta_propuesta, INCLUIR la columna CICLO
                bool esCiclo = col.Equals("CICLO", StringComparison.OrdinalIgnoreCase);
                bool tablaPermiteCiclo = nombreTablaSqlite == "cierreciclo" || 
                                        nombreTablaSqlite == "solicitudes" || 
                                        nombreTablaSqlite == "hsolicitudes" ||
                                        nombreTablaSqlite == "visitas" ||
                                        nombreTablaSqlite == "hvisitas" ||
                                        nombreTablaSqlite == "hoja_ruta" ||
                                        nombreTablaSqlite == "hoja_ruta_propuesta";
                
                // Si es CICLO y la tabla NO permite CICLO, excluir
                if (esCiclo && !tablaPermiteCiclo)
                {
                    continue;
                }
                
                // Excluir REGISTRO y ZONA para todas las tablas de ayuda_visual
                if ((nombreTablaSqlite == "ayuda_visual" || 
                     nombreTablaSqlite == "ayuda_visual_fe" || 
                     nombreTablaSqlite == "ayuda_visual_mp4" || 
                     nombreTablaSqlite == "ayuda_visual_mp4_fe") && 
                    (col.Equals("REGISTRO", StringComparison.OrdinalIgnoreCase) || 
                     col.Equals("ZONA", StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                
                // Si llegamos aquí, incluir la columna
                finalColumns.Add(col);
            }
            
            if (!finalColumns.Any()) return;

            while (await reader.ReadAsync())
            {
                var valores = new List<string>();
                foreach (var col in finalColumns)
                {
                    int cicloParaFormateo = (nombreTablaSqlite.StartsWith("h") || nombreTablaSqlite == "cierreciclo") ? Convert.ToInt32(reader["CICLO"]) : ciclo;
                    valores.Add(FormatearValor(reader[col], reader.GetFieldType(reader.GetOrdinal(col)), col, cicloParaFormateo));
                }
                
                // Generar INSERT con columnas explícitas pero sin comillas en nombre de tabla
                string columnasStr = string.Join(", ", finalColumns);
                string valoresStr = string.Join(", ", valores);
                contenido.AppendLine($"INSERT INTO {nombreTablaSqlite} ({columnasStr}) VALUES ({valoresStr});");
            }
        }

        private string GetDefaultQuery(string nombreTabla)
        {
            if (nombreTabla == "MD_Hoja_Ruta_Propuesta")
            {
                return @"SELECT HRPP.* 
                         FROM MD_Hoja_Ruta_Propuesta HRPP 
                         CROSS APPLY( 
                             SELECT TOP 1 CAST(CAST(NU_ANO AS varchar(4)) + RIGHT('00' + CAST(CICLO AS varchar(2)), 2) AS INT) NCICLO 
                             FROM MD_Hoja_Ruta_Propuesta HRP WHERE HRP.ZONA = HRPP.ZONA 
                             ORDER BY HRP.NU_ANO DESC, HRP.CICLO DESC 
                         ) M 
                         WHERE HRPP.ZONA = @visitadorId AND CAST(CAST(NU_ANO AS varchar(4)) + RIGHT('00' + CAST(CICLO AS varchar(2)), 2) AS INT) = M.NCICLO";
            }
            return $"SELECT * FROM {nombreTabla} WHERE NU_ANO=@ano AND CICLO=@ciclo AND ZONA=@visitadorId";
        }

        private void GenerarEsquemaTablas(StringBuilder contenido)
        {
            // Todas las 114 definiciones de tablas del archivo ClickOnce embebidas directamente
            var todasLasDefiniciones = @"DROP TABLE IF EXISTS ""altas_bajas"";
CREATE TABLE ""altas_bajas"" (""Altas_Aprobado"" INTEGER(11), ""Altas_Postulados"" INTEGER(11), ""Bajas_Aprobado"" INTEGER(11), ""Bajas_Postulados"" INTEGER(11));
DROP TABLE IF EXISTS ""alteraciones"";
CREATE TABLE ""alteraciones"" (""REGISTRO"" TEXT(5), ""DOCTOR"" TEXT(40), ""RUTA"" TEXT(40), ""CLINICA"" TEXT(50), ""ESPECIALIDAD"" TEXT(25), ""CLASIFICACION"" TEXT(2), ""REVISITA"" TEXT(2), ""ALTERADO"" TEXT(1), ""ZONA"" TEXT(7));
DROP TABLE IF EXISTS ""banner"";
CREATE TABLE ""banner"" (""CICLO"" INTEGER(11));
DROP TABLE IF EXISTS ""ciclos"";
CREATE TABLE ""ciclos"" (""FECHAI_CICLO"" TEXT(8), ""FECHAF_CICLO"" TEXT(8), ""NRO_CICLO"" INTEGER(11), ""CICLO_CERRADO"" TEXT(255), ""DIAS_HABILES"" INTEGER(11), ""ESTATUS"" TEXT(255), ""ANO"" integer(11), ""KPI_Visita_Medica"" INTEGER(11), ""KPI_Visita_Farmacia"" INTEGER(11));
DROP TABLE IF EXISTS ""ciclospropuesto"";
CREATE TABLE ""ciclospropuesto"" (""FECHAINI"" TEXT(8), ""DIAI"" TEXT(10), ""FECHAFIN"" TEXT(8), ""DIAF"" TEXT(10), ""SEMANA"" TEXT(2), ""CICLO"" TEXT(2));
DROP TABLE IF EXISTS ""cierreciclo"";
CREATE TABLE ""cierreciclo"" (""Zona"" TEXT(7), ""Ciclo"" INTEGER(11), ""Fecha"" TEXT(40), ""Hora"" TEXT(15), ""Fichados"" INTEGER(11), ""Visitados"" INTEGER(11), ""PorcentajeFichadoVisitados"" REAL(8), ""Revisitas"" INTEGER(11), ""Revisitados"" INTEGER(11), ""PorcentajeRevisitasRevisitados"" REAL(8), ""DiasDeCiclos"" INTEGER(11), ""DiasTranscurridos"" INTEGER(11), ""DiasTrabajados"" REAL(8), ""DiasDescontados"" REAL(8), ""PDE"" REAL(8), ""PDR"" REAL(8), ""PDP"" REAL(8), ""PR"" REAL(8), ""PP"" REAL(8), ""Mix"" REAL(8), ""FichadosPtos"" INTEGER(11), ""VisitadosPtos"" INTEGER(11), ""PorcentajeFichadoVisitadosPtos"" REAL(8), ""RevisitasPtos"" INTEGER(11), ""RevisitadosPtos"" INTEGER(11), ""PorcentajeRevisitasRevisitadosPtos"" REAL(8), ""PDEPtos"" REAL(8), ""PDRPtos"" REAL(8), ""PDPPtos"" REAL(8), ""PRPtos"" REAL(8), ""PPPtos"" REAL(8), ""MixPtos"" REAL(8), ""DescontadoS"" REAL(8), ""DescontadoN"" REAL(8), ""Ficha"" REAL(8), ""AltasPostulados"" INTEGER(11), ""AltasAprobados"" INTEGER(11), ""BajasPostulados"" INTEGER(11), ""BajasAprobados"" INTEGER(11));
DROP TABLE IF EXISTS ""contador"";
CREATE TABLE ""contador"" (""Contador"" INTEGER(11));
DROP TABLE IF EXISTS ""eliminar"";
CREATE TABLE ""eliminar"" (""Registro"" INTEGER(11), ""Medico"" TEXT(40), ""Zona"" TEXT(7), ""Clave"" TEXT(6), ""Eliminado"" TEXT(2));
DROP TABLE IF EXISTS ""esquemahibrido"";
CREATE TABLE ""esquemahibrido"" (""ORDEN"" INTEGER(11), ""ESPECIALIDAD"" TEXT(25), ""MARCA"" TEXT(40), ""POSICIONAMIENTO"" TEXT(200));
DROP TABLE IF EXISTS ""farmacias"";
CREATE TABLE ""farmacias"" (""NUMERO"" INTEGER(11), ""ZONA"" TEXT(7), ""FARMACIA"" TEXT(100), ""RUTA"" TEXT(255), ""CADENA"" TEXT(100), ""CLASIFICACION"" TEXT(3), ""TELEFONO"" TEXT(30), ""DIRECCION"" TEXT(255), ""UBICACION"" TEXT(30), ""REGENTE"" TEXT(100), ""AUXILIAR"" TEXT(100), ""PROPIETARIO"" TEXT(100), ""COMPRADOR"" TEXT(100), ""HORA_SUGERIDA_AM"" TEXT(30), ""HORA_SUGERIDA_PM"" TEXT(30), ""RIF"" TEXT(15), ""ESTADO"" TEXT(255), ""BRICK"" TEXT(8), ""CLIENTE_LAB_VARGAS"" TEXT(2), ""DROGUERIA1"" TEXT(30), ""DROGUERIA2"" TEXT(30), ""DROGUERIA3"" TEXT(30), ""DROGIERIA4"" TEXT(50), ""TRABAJA_CON_TRANSF"" TEXT(2), ""CON_CUAL_DROGUERIA1"" TEXT(30), ""CON_CUAL_DROGUERIA2"" TEXT(50), ""CICLO_VISITADO"" INTEGER(11), ""QUIEN_TRANSFIERE"" TEXT(25), ""MEDICO_PRESCRIPTIVO1_FICHADO"" TEXT(40), ""MEDICO_PRESCRIPTIVO2_FICHADO"" TEXT(40), ""MEDICO_PRESCRIPTIVO3_FICHADO"" TEXT(40), ""MEDICO_PRESCRIPTIVO1_NOFICHADO"" TEXT(40), ""MEDICO_PRESCRIPTIVO2_NOFICHADO"" TEXT(40), ""MEDICO_PRESCRIPTIVO3_NOFICHADO"" TEXT(40), ""INSTITUCION_PRESCRIPTIVA1"" TEXT(35), ""INSTITUCION_PRESCRIPTIVA2"" TEXT(35), ""INSTITUCION_PRESCRIPTIVA3"" TEXT(35), ""INSTITUCION_OTRAS_PRESCRIPTIVA1"" TEXT(50), ""INSTITUCION_OTRAS_PRESCRIPTIVA2"" TEXT(50), ""INSTITUCION_OTRAS_PRESCRIPTIVA3"" TEXT(50), ""ACEPTA_PROMOTORAS"" TEXT(2), ""ACEPTA_FOLLETOS"" TEXT(2), ""ACEPTA_AUTOSERVICIO"" TEXT(2), ""ACEPTA_FRENTILLOS"" TEXT(2), ""ACEPTA_PAGO_PESTANAS"" TEXT(2), ""TIENE_FRENTILLOS"" TEXT(2), ""ACEPTA_DECORACION"" TEXT(2), ""ACEPTA_AFICHES"" TEXT(2), ""ACEPTA_HABLADORES"" TEXT(2), ""ACEPTA_JORNADA_SALUD"" TEXT(2), ""REQUISITOS_FRENTILLOS"" TEXT(35), ""REQUISITOS_FRENTILLOS1"" TEXT(35), ""ACEPTA_EXHIBIDOR"" TEXT(2), ""FECHA_AGENDA"" TEXT(10), ""SEMANA_AGENDA"" TEXT(2), ""AMPM_AGENDA"" TEXT(2), ""NRO_FECHA_AGENDA"" INTEGER(11), ""CICLO_AGENDA"" INTEGER(11), ""HORA"" TEXT(8), ""L_TRABAJO"" TEXT(60), ""N_REVISITAS"" INTEGER(11), ""REGENTE_CUMPLEANO"" TEXT(10), ""PROPIETARIO_CUMPLEANO"" TEXT(10), ""AUXILIAR_CUMPLEANO"" TEXT(10), ""COMPRADOR_CUMPLEANO"" TEXT(10), ""FARMACEUTICO"" TEXT(100), ""FARMACEUTICO_CUMPLEANO"" TEXT(10), ""N_REGISTRADORA"" INTEGER(11), ""P_VEHICULO"" TEXT(2), ""N_PUESTO"" INTEGER(11), ""F_VENTAS_1"" INTEGER(11), ""F_VENTAS_2"" INTEGER(11), ""D_PEDIDO"" TEXT(10), ""COMPETIDOR_1"" TEXT(20), ""COMPETIDOR_2"" TEXT(20), ""ACT_PROMO_ACEPTA"" TEXT(255), ""REQ_ACT_PROMO1"" TEXT(30), ""REQ_ACT_PROMO2"" TEXT(30), ""COSTO_ACT_PUBLI"" TEXT(5), ""CIUDAD"" TEXT(255), ""BRICK_ZONA"" TEXT(100), ""CODIGO_DROGUERIA"" TEXT(15), ""NOMBRE_DROGUERIA"" TEXT(60), ""DIA_PEDIDO"" TEXT(50), ""CONVENIO"" TEXT(50), ""OBSERVACION"" TEXT(50), ""CICLO"" INTEGER(11), ""CORREO""	TEXT(255));
DROP TABLE IF EXISTS ""farmacias_detalles"";
CREATE TABLE ""farmacias_detalles"" (""NUMERO"" INTEGER(11), ""CICLO_VISITA"" INTEGER(11), ""ZONA"" TEXT(7), ""FARMACIA"" TEXT(255), ""FECHA_VISITA"" TEXT(255), ""MOTIVO"" TEXT(255), ""ATENDIO"" TEXT(255), ""ATENDIO_NOMBRE"" TEXT(255), ""PRODUCTO_EVALUADO1"" TEXT(255), ""COMENTARIOS"" TEXT(255));
DROP TABLE IF EXISTS ""farmacias_master"";
CREATE TABLE ""farmacias_master"" (""NUMERO"" INTEGER(11), ""BRICK"" TEXT(5), ""RUTA"" TEXT(255), ""FARMACIA"" TEXT(255), ""DIRECCION"" TEXT(255), ""CIUDAD"" TEXT(255), ""ESTADO"" TEXT(255), ""CADENA"" TEXT(255), ""TELEFONO_1"" TEXT(100), ""TELEFONO_2"" TEXT(100), ""TELEFONO_3"" TEXT(100));
DROP TABLE IF EXISTS ""fichas"";
CREATE TABLE ""fichas"" (""Codigo"" TEXT(7), ""V1"" INTEGER(11), ""V2"" INTEGER(11), ""V3"" INTEGER(11), ""E1"" INTEGER(11), ""E2"" INTEGER(11), ""E3"" INTEGER(11), ""R1"" INTEGER(11), ""R2"" INTEGER(11), ""R3"" INTEGER(11));
DROP TABLE IF EXISTS ""fichero"";
CREATE TABLE ""fichero"" (""REGISTRO"" TEXT(5), ""DOCTOR"" TEXT(40), ""RUTA"" TEXT(40), ""CLINICA"" TEXT(50), ""ESPECIALIDAD"" TEXT(25), ""CLASIFICACION"" TEXT(2), ""DIRECCION"" TEXT(150), ""ZONA"" TEXT(7), ""TELEFONO"" TEXT(35), ""OBSERVACION"" TEXT(60), ""BRICK"" TEXT(8), ""CODIGO_DDD"" TEXT(9), ""HORA_SUGERIDA"" TEXT(9), ""MODO"" TEXT(2), ""NRO_VISITADORES"" TEXT(1), ""HACE_ESPERAR"" TEXT(2), ""COLOCACION_MALETA"" TEXT(10), ""ESTADO"" TEXT(20), ""REVISITA"" TEXT(2), ""TIPO_MEDICO"" TEXT(11), ""TIPO_MEDICO1"" TEXT(11), ""ESTADO_CIVIL"" TEXT(10), ""HIJO_MASCULINO"" TEXT(1), ""HIJO_FEMENINO"" TEXT(1), ""DEPORTES"" TEXT(10), ""CUMPLEANO_DIA"" TEXT(2), ""CUMPLEANO_MES"" TEXT(12), ""IDIOMAS"" TEXT(12), ""IDIOMAS2"" TEXT(12), ""OTRO_SITIO"" TEXT(30), ""OCUPACION"" TEXT(17), ""COSTO_CONSULTA"" TEXT(6), ""PATOLOGIA_FRECUENTE1"" TEXT(40), ""PATOLOGIA_FRECUENTE2"" TEXT(40), ""PATOLOGIA_FRECUENTE3"" TEXT(40), ""PRODUCTO_ACOPLADOS1"" TEXT(30), ""PRODUCTO_ACOPLADOS2"" TEXT(30), ""PRODUCTO_ACOPLADOS3"" TEXT(30), ""INFLUENCIA_HOSPITAL"" TEXT(3), ""INFLUENCIA_CLINICA"" TEXT(3), ""INVESTIGADOR"" TEXT(3), ""FORMADOR"" TEXT(3), ""SPEAKER"" TEXT(3), ""CONGRESO"" TEXT(3), ""CONSIDERA_PRIMERO"" TEXT(15), ""CONSIDERA_SEGUNDO"" TEXT(15), ""CONSIDERA_TERCERO"" TEXT(15), ""TRATO_ZUOZ"" TEXT(3), ""REGISTRO_ACTIVO"" TEXT(2), ""ULTIMA_FECHA_V"" TEXT(30), ""CICLO_C"" INTEGER(11), ""REVISITA_C"" INTEGER(11), ""PERFIL_PRESCRIPTIVO1"" TEXT(35), ""PERFIL_PRESCRIPTIVO2"" TEXT(35), ""PERFIL_PRESCRIPTIVO3"" TEXT(35), ""SECRETARIA"" TEXT(100), ""EDAD_EN_TIEMPO"" TEXT(15), ""FECHA_AGENDA"" TEXT(10), ""SEMANA_AGENDA"" TEXT(2), ""AMPM_AGENDA"" TEXT(2), ""NRO_FECHA_AGENDA"" INTEGER(11), ""CICLO_AGENDA"" INTEGER(11), ""RFECHA_AGENDA"" TEXT(10), ""RSEMANA_AGENDA"" TEXT(2), ""RAMPM_AGENDA"" TEXT(2), ""RNRO_FECHA_AGENDA"" INTEGER(11), ""RCICLO_AGENDA"" INTEGER(11), ""CODIGO_IMI"" INTEGER(11), ""EMAIL"" TEXT(40), ""HORA"" TEXT(8), ""NRO_SANITARIO"" TEXT(7), ""CIUDAD"" TEXT(20), ""BRICK_ZONA"" TEXT(100), ""PARETO"" TEXT(2), ""TWITTER"" TEXT(100), ""FACEBOOK"" TEXT(100), ""INSTAGRAM"" TEXT(100), ""LINKEDIN"" TEXT(100), ""TELEGRAM"" TEXT(100), ""TELEFONO_ADICIONAL"" TEXT(100));
DROP TABLE IF EXISTS ""ficherop"";
CREATE TABLE ""ficherop"" (""Fichados"" INTEGER(11), ""Visitados"" INTEGER(11), ""Revisitas"" INTEGER(11), ""Revisitados"" INTEGER(11), ""PtosFichados"" INTEGER(11), ""PtosVisitados"" INTEGER(11), ""PtosRevisitas"" INTEGER(11), ""PtosRevisitados"" INTEGER(11), ""SDescontados"" REAL(8), ""NDescontados"" REAL(8), ""NoAplicaDias"" INTEGER(11));
DROP TABLE IF EXISTS ""fichero_farmacias"";
CREATE TABLE ""fichero_farmacias"" (""NUMERO"" INTEGER(11), ""REGISTRO"" INTEGER(11), ""ZONA"" TEXT(7), ""FARMACIA"" TEXT(100), ""RUTA"" TEXT(255), ""CADENA"" TEXT(100), ""CLASIFICACION"" TEXT(3), ""TELEFONO"" TEXT(30), ""DIRECCION"" TEXT(255), ""UBICACION"" TEXT(30), ""REGENTE"" TEXT(100), ""AUXILIAR"" TEXT(100), ""PROPIETARIO"" TEXT(100), ""COMPRADOR"" TEXT(100), ""HORA_SUGERIDA_AM"" TEXT(30), ""HORA_SUGERIDA_PM"" TEXT(30), ""RIF"" TEXT(15), ""ESTADO"" TEXT(50), ""BRICK"" TEXT(8), ""CLIENTE_LAB_VARGAS"" TEXT(2), ""DROGUERIA1"" TEXT(100), ""DROGUERIA2"" TEXT(100), ""DROGUERIA3"" TEXT(100), ""DROGIERIA4"" TEXT(100), ""TRABAJA_CON_TRANSF"" TEXT(2), ""CON_CUAL_DROGUERIA1"" TEXT(30), ""CON_CUAL_DROGUERIA2"" TEXT(50), ""CICLO_VISITADO"" INTEGER(11), ""QUIEN_TRANSFIERE"" TEXT(25), ""MEDICO_PRESCRIPTIVO1_FICHADO"" TEXT(40), ""MEDICO_PRESCRIPTIVO2_FICHADO"" TEXT(40), ""MEDICO_PRESCRIPTIVO3_FICHADO"" TEXT(40), ""MEDICO_PRESCRIPTIVO1_NOFICHADO"" TEXT(40), ""MEDICO_PRESCRIPTIVO2_NOFICHADO"" TEXT(40), ""MEDICO_PRESCRIPTIVO3_NOFICHADO"" TEXT(40), ""INSTITUCION_PRESCRIPTIVA1"" TEXT(35), ""INSTITUCION_PRESCRIPTIVA2"" TEXT(35), ""INSTITUCION_PRESCRIPTIVA3"" TEXT(35), ""INSTITUCION_OTRAS_PRESCRIPTIVA1"" TEXT(50), ""INSTITUCION_OTRAS_PRESCRIPTIVA2"" TEXT(50), ""INSTITUCION_OTRAS_PRESCRIPTIVA3"" TEXT(50), ""ACEPTA_PROMOTORAS"" TEXT(2), ""ACEPTA_FOLLETOS"" TEXT(2), ""ACEPTA_AUTOSERVICIO"" TEXT(2), ""ACEPTA_FRENTILLOS"" TEXT(2), ""ACEPTA_PAGO_PESTANAS"" TEXT(2), ""TIENE_FRENTILLOS"" TEXT(2), ""ACEPTA_DECORACION"" TEXT(2), ""ACEPTA_AFICHES"" TEXT(2), ""ACEPTA_HABLADORES"" TEXT(2), ""ACEPTA_JORNADA_SALUD"" TEXT(2), ""REQUISITOS_FRENTILLOS"" TEXT(35), ""REQUISITOS_FRENTILLOS1"" TEXT(35), ""ACEPTA_EXHIBIDOR"" TEXT(2), ""FECHA_AGENDA"" TEXT(10), ""SEMANA_AGENDA"" TEXT(2), ""AMPM_AGENDA"" TEXT(2), ""NRO_FECHA_AGENDA"" INTEGER(11), ""CICLO_AGENDA"" INTEGER(11), ""HORA"" TEXT(8), ""L_TRABAJO"" TEXT(60), ""N_REVISITAS"" INTEGER(11), ""REGENTE_CUMPLEANO"" TEXT(10), ""PROPIETARIO_CUMPLEANO"" TEXT(10), ""AUXILIAR_CUMPLEANO"" TEXT(10), ""COMPRADOR_CUMPLEANO"" TEXT(10), ""FARMACEUTICO"" TEXT(100), ""FARMACEUTICO_CUMPLEANO"" TEXT(10), ""N_REGISTRADORA"" INTEGER(11), ""P_VEHICULO"" TEXT(2), ""N_PUESTO"" INTEGER(11), ""F_VENTAS_1"" INTEGER(11), ""F_VENTAS_2"" INTEGER(11), ""D_PEDIDO"" TEXT(100), ""COMPETIDOR_1"" TEXT(20), ""COMPETIDOR_2"" TEXT(20), ""ACT_PROMO_ACEPTA"" TEXT(255), ""REQ_ACT_PROMO1"" TEXT(30), ""REQ_ACT_PROMO2"" TEXT(30), ""COSTO_ACT_PUBLI"" TEXT(5), ""CIUDAD"" TEXT(255), ""BRICK_ZONA"" TEXT(100), ""CODIGO_DROGUERIA"" TEXT(15), ""NOMBRE_DROGUERIA"" TEXT(100), ""DIA_PEDIDO"" TEXT(50), ""CONVENIO"" TEXT(100), ""OBSERVACION"" TEXT(100), ""CICLO"" INTEGER(11));
DROP TABLE IF EXISTS ""fichero_horarios"";
CREATE TABLE ""fichero_horarios"" (""REGISTRO"" TEXT(5), ""DOCTOR"" TEXT(40), ""ZONA"" TEXT(7), ""HORARIO_LUNES_AM0"" TEXT(4), ""HORARIO_LUNES_AM1"" TEXT(4), ""HORARIO_MARTES_AM0"" TEXT(4), ""HORARIO_MARTES_AM1"" TEXT(4), ""HORARIO_MIERCOLES_AM0"" TEXT(4), ""HORARIO_MIERCOLES_AM1"" TEXT(4), ""HORARIO_JUEVES_AM0"" TEXT(4), ""HORARIO_JUEVES_AM1"" TEXT(4), ""HORARIO_VIERNES_AM0"" TEXT(4), ""HORARIO_VIERNES_AM1"" TEXT(4), ""HORARIO_LUNES_PM0"" TEXT(4), ""HORARIO_LUNES_PM1"" TEXT(4), ""HORARIO_MARTES_PM0"" TEXT(4), ""HORARIO_MARTES_PM1"" TEXT(4), ""HORARIO_MIERCOLES_PM0"" TEXT(4), ""HORARIO_MIERCOLES_PM1"" TEXT(4), ""HORARIO_JUEVES_PM0"" TEXT(4), ""HORARIO_JUEVES_PM1"" TEXT(4), ""HORARIO_VIERNES_PM0"" TEXT(4), ""HORARIO_VIERNES_PM1"" TEXT(4));
DROP TABLE IF EXISTS ""fichero_hospital"";
CREATE TABLE ""fichero_hospital"" (""NUMERO"" INTEGER(11), ""REGISTRO"" INTEGER(11), ""DOCTOR"" TEXT(100), ""RUTA"" TEXT(100), ""LTRABAJO"" TEXT(100), ""CARGO"" TEXT(100), ""ESPECIALIDAD"" TEXT(100), ""CLASIFICACION"" TEXT(2), ""DIRECCION"" TEXT(120), ""ZONA"" TEXT(7), ""TELEFONO"" TEXT(30), ""OBSERVACION"" TEXT(100), ""BRICK"" TEXT(8), ""CODIGO_DDD"" TEXT(9), ""HORA_SUGERIDA"" TEXT(9), ""MODO"" TEXT(2), ""NRO_VISITADORES"" TEXT(1), ""HACE_ESPERAR"" TEXT(2), ""COLOCACION_MALETA"" TEXT(10), ""ESTADO"" TEXT(20), ""REVISITA"" TEXT(2), ""TIPO_MEDICO"" TEXT(11), ""TIPO_MEDICO1"" TEXT(11), ""ESTADO_CIVIL"" TEXT(10), ""HIJO_MASCULINO"" TEXT(1), ""HIJO_FEMENINO"" TEXT(1), ""DEPORTES"" TEXT(10), ""CUMPLEANO_DIA"" TEXT(2), ""CUMPLEANO_MES"" TEXT(12), ""IDIOMAS"" TEXT(12), ""IDIOMAS2"" TEXT(12), ""OTRO_SITIO"" TEXT(25), ""OCUPACION"" TEXT(17), ""COSTO_CONSULTA"" TEXT(6), ""PATOLOGIA_FRECUENTE1"" TEXT(30), ""PATOLOGIA_FRECUENTE2"" TEXT(30), ""PATOLOGIA_FRECUENTE3"" TEXT(30), ""PRODUCTO_ACOPLADOS1"" TEXT(30), ""PRODUCTO_ACOPLADOS2"" TEXT(30), ""PRODUCTO_ACOPLADOS3"" TEXT(30), ""INFLUENCIA_HOSPITAL"" TEXT(2), ""INFLUENCIA_CLINICA"" TEXT(2), ""INVESTIGADOR"" TEXT(2), ""FORMADOR"" TEXT(2), ""SPEAKER"" TEXT(2), ""CONGRESO"" TEXT(2), ""CONSIDERA_PRIMERO"" TEXT(15), ""CONSIDERA_SEGUNDO"" TEXT(15), ""CONSIDERA_TERCERO"" TEXT(15), ""TRATO_ZUOZ"" TEXT(2), ""REGISTRO_ACTIVO"" TEXT(2), ""ULTIMA_FECHA_V"" TEXT(30), ""CICLO_C"" INTEGER(11), ""REVISITA_C"" INTEGER(11), ""PERFIL_PRESCRIPTIVO1"" TEXT(35), ""PERFIL_PRESCRIPTIVO2"" TEXT(35), ""PERFIL_PRESCRIPTIVO3"" TEXT(35), ""SECRETARIA"" TEXT(30), ""EDAD_EN_TIEMPO"" TEXT(15), ""FECHA_AGENDA"" TEXT(10), ""SEMANA_AGENDA"" TEXT(2), ""AMPM_AGENDA"" TEXT(2), ""NRO_FECHA_AGENDA"" INTEGER(11), ""CICLO_AGENDA"" INTEGER(11), ""RFECHA_AGENDA"" TEXT(10), ""RSEMANA_AGENDA"" TEXT(2), ""RAMPM_AGENDA"" TEXT(2), ""RNRO_FECHA_AGENDA"" INTEGER(11), ""RCICLO_AGENDA"" INTEGER(11), ""CODIGO_IMI"" INTEGER(11), ""EMAIL"" TEXT(25), ""HORA"" TEXT(8), ""NRO_SANITARIO"" TEXT(7), ""CIUDAD"" TEXT(50), ""BRICK_ZONA"" TEXT(100), ""INSTITUCION"" TEXT(100), ""DOCENTE"" TEXT(2), ""CLIENTE"" TEXT(2), ""CICLO"" INTEGER(11));
DROP TABLE IF EXISTS ""hfarmacias_detalles"";
CREATE TABLE ""hfarmacias_detalles"" (""NUMERO"" INTEGER(11), ""CICLO_VISITA"" INTEGER(11), ""ZONA"" TEXT(7), ""FARMACIA"" TEXT(255), ""FECHA_VISITA"" TEXT(255), ""MOTIVO"" TEXT(255), ""ATENDIO"" TEXT(255), ""ATENDIO_NOMBRE"" TEXT(255), ""PRODUCTO_EVALUADO1"" TEXT(255), ""COMENTARIOS"" TEXT(255));
DROP TABLE IF EXISTS ""hhospital_detalles"";
CREATE TABLE ""hhospital_detalles"" (""NUMERO"" INTEGER(11), ""CICLO_VISITA"" INTEGER(11), ""ZONA"" TEXT(7), ""HOSPITAL"" TEXT(100), ""FECHA_VISITA"" TEXT(50), ""SERVICIO"" TEXT(50), ""MOTIVO"" TEXT(100), ""ACOMPANADO"" TEXT(100), ""NUM_MEDICO"" INTEGER(11), ""ATENDIDO"" TEXT(255), ""COMENTARIOS"" TEXT(255));
DROP TABLE IF EXISTS ""hhospital_detalles_medicos"";
CREATE TABLE ""hhospital_detalles_medicos"" (""NUMERO"" INTEGER(11), ""CICLO_VISITA"" INTEGER(11), ""ZONA"" TEXT(7), ""HOSPITAL"" TEXT(100), ""FECHA_VISITA"" TEXT(50), ""SERVICIO"" TEXT(50), ""MOTIVO"" TEXT(100), ""REGISTRO"" INTEGER(11), ""DOCTOR"" TEXT(100));
DROP TABLE IF EXISTS ""historialconceptodias"";
CREATE TABLE ""historialconceptodias"" (""CONCEPTOS"" TEXT(30), ""DESCONTABLES"" TEXT(2), ""FECHA_DESDE"" TEXT(15), ""FECHA_HASTA"" TEXT(15), ""DIAS"" REAL(8), ""ZONA"" TEXT(7), ""TIPO"" TEXT(1), ""COMENTARIO"" TEXT(30));
DROP TABLE IF EXISTS ""hoja_ruta"";
CREATE TABLE ""hoja_ruta"" (""CICLO"" INTEGER(11), ""ZONA"" TEXT(7), ""SEMANA"" INTEGER(11), ""DIA"" TEXT(10), ""AM"" TEXT(255), ""PM"" TEXT(255));
DROP TABLE IF EXISTS ""hoja_ruta_propuesta"";
CREATE TABLE ""hoja_ruta_propuesta"" (""CICLO"" INTEGER(11), ""ZONA"" TEXT(7), ""SEMANA"" INTEGER(11), ""DIA"" TEXT(10), ""AM"" TEXT(255), ""PM"" TEXT(255));
DROP TABLE IF EXISTS ""hospital"";
CREATE TABLE ""hospital"" (""NUMERO"" INTEGER(11), ""ZONA"" TEXT(7), ""HOSPITAL"" TEXT(100), ""RUTA"" TEXT(255), ""CADENA"" TEXT(100), ""CLASIFICACION"" TEXT(3), ""TELEFONO"" TEXT(30), ""DIRECCION"" TEXT(255), ""UBICACION"" TEXT(30), ""REGENTE"" TEXT(100), ""AUXILIAR"" TEXT(100), ""PROPIETARIO"" TEXT(100), ""COMPRADOR"" TEXT(100), ""HORA_SUGERIDA_AM"" TEXT(30), ""HORA_SUGERIDA_PM"" TEXT(30), ""RIF"" TEXT(15), ""ESTADO"" TEXT(20), ""BRICK"" TEXT(8), ""CLIENTE_LAB_VARGAS"" TEXT(2), ""DROGUERIA1"" TEXT(30), ""DROGUERIA2"" TEXT(30), ""DROGUERIA3"" TEXT(30), ""DROGIERIA4"" TEXT(50), ""TRABAJA_CON_TRANSF"" TEXT(2), ""CON_CUAL_DROGUERIA1"" TEXT(30), ""CON_CUAL_DROGUERIA2"" TEXT(50), ""CICLO_VISITADO"" INTEGER(11), ""QUIEN_TRANSFIERE"" TEXT(25), ""MEDICO_PRESCRIPTIVO1_FICHADO"" TEXT(40), ""MEDICO_PRESCRIPTIVO2_FICHADO"" TEXT(40), ""MEDICO_PRESCRIPTIVO3_FICHADO"" TEXT(40), ""MEDICO_PRESCRIPTIVO1_NOFICHADO"" TEXT(40), ""MEDICO_PRESCRIPTIVO2_NOFICHADO"" TEXT(40), ""MEDICO_PRESCRIPTIVO3_NOFICHADO"" TEXT(40), ""INSTITUCION_PRESCRIPTIVA1"" TEXT(35), ""INSTITUCION_PRESCRIPTIVA2"" TEXT(35), ""INSTITUCION_PRESCRIPTIVA3"" TEXT(35), ""INSTITUCION_OTRAS_PRESCRIPTIVA1"" TEXT(50), ""INSTITUCION_OTRAS_PRESCRIPTIVA2"" TEXT(50), ""INSTITUCION_OTRAS_PRESCRIPTIVA3"" TEXT(50), ""ACEPTA_PROMOTORAS"" TEXT(2), ""ACEPTA_FOLLETOS"" TEXT(2), ""ACEPTA_AUTOSERVICIO"" TEXT(2), ""ACEPTA_FRENTILLOS"" TEXT(2), ""ACEPTA_PAGO_PESTANAS"" TEXT(2), ""TIENE_FRENTILLOS"" TEXT(2), ""ACEPTA_DECORACION"" TEXT(2), ""ACEPTA_AFICHES"" TEXT(2), ""ACEPTA_HABLADORES"" TEXT(2), ""ACEPTA_JORNADA_SALUD"" TEXT(2), ""REQUISITOS_FRENTILLOS"" TEXT(35), ""REQUISITOS_FRENTILLOS1"" TEXT(35), ""ACEPTA_EXHIBIDOR"" TEXT(2), ""FECHA_AGENDA"" TEXT(10), ""SEMANA_AGENDA"" TEXT(2), ""AMPM_AGENDA"" TEXT(2), ""NRO_FECHA_AGENDA"" INTEGER(11), ""CICLO_AGENDA"" INTEGER(11), ""HORA"" TEXT(8), ""L_TRABAJO"" TEXT(60), ""N_REVISITAS"" INTEGER(11), ""REGENTE_CUMPLEANO"" TEXT(10), ""PROPIETARIO_CUMPLEANO"" TEXT(10), ""AUXILIAR_CUMPLEANO"" TEXT(10), ""COMPRADOR_CUMPLEANO"" TEXT(10), ""FARMACEUTICO"" TEXT(100), ""FARMACEUTICO_CUMPLEANO"" TEXT(10), ""N_REGISTRADORA"" INTEGER(11), ""P_VEHICULO"" TEXT(2), ""N_PUESTO"" INTEGER(11), ""F_VENTAS_1"" INTEGER(11), ""F_VENTAS_2"" INTEGER(11), ""D_PEDIDO"" TEXT(10), ""COMPETIDOR_1"" TEXT(20), ""COMPETIDOR_2"" TEXT(20), ""ACT_PROMO_ACEPTA"" TEXT(255), ""REQ_ACT_PROMO1"" TEXT(30), ""REQ_ACT_PROMO2"" TEXT(30), ""COSTO_ACT_PUBLI"" TEXT(5), ""CIUDAD"" TEXT(255), ""BRICK_ZONA"" TEXT(100), ""CODIGO_DROGUERIA"" TEXT(15), ""NOMBRE_DROGUERIA"" TEXT(60), ""DIA_PEDIDO"" TEXT(50), ""CONVENIO"" TEXT(50), ""OBSERVACION"" TEXT(50), ""INSTITUCION"" TEXT(100), ""DOCENTE"" TEXT(2), ""CLIENTE"" TEXT(2), ""CICLO"" INTEGER(11));
DROP TABLE IF EXISTS ""hospitales_master"";
CREATE TABLE ""hospitales_master"" (""NUMERO"" INTEGER(11), ""BRICK"" TEXT(5), ""RUTA"" TEXT(255), ""NOMBRE"" TEXT(255), ""INSTITUCION"" TEXT(100), ""DIRECCION"" TEXT(255), ""DOCENTE"" TEXT(2), ""CLIENTE"" TEXT(2));
DROP TABLE IF EXISTS ""hospital_detalles"";
CREATE TABLE ""hospital_detalles"" (""NUMERO"" INTEGER(11), ""CICLO_VISITA"" INTEGER(11), ""ZONA"" TEXT(7), ""HOSPITAL"" TEXT(100), ""FECHA_VISITA"" TEXT(50), ""SERVICIO"" TEXT(50), ""MOTIVO"" TEXT(100), ""ACOMPANADO"" TEXT(100), ""NUM_MEDICO"" INTEGER(11), ""ATENDIDO"" TEXT(255), ""COMENTARIOS"" TEXT(255));
DROP TABLE IF EXISTS ""hospital_detalles_medicos"";
CREATE TABLE ""hospital_detalles_medicos"" (""NUMERO"" INTEGER(11), ""CICLO_VISITA"" INTEGER(11), ""ZONA"" TEXT(7), ""HOSPITAL"" TEXT(100), ""FECHA_VISITA"" TEXT(50), ""SERVICIO"" TEXT(50), ""MOTIVO"" TEXT(100), ""REGISTRO"" INTEGER(11), ""DOCTOR"" TEXT(100));
DROP TABLE IF EXISTS ""hsolicitudes"";
CREATE TABLE ""hsolicitudes"" (""REGISTRO"" TEXT(5), ""DOCTOR"" TEXT(40), ""ZONA"" TEXT(7), ""CICLO"" INTEGER(11), ""COMENTARIOS_PERSONALES"" TEXT(255), ""COMETARIO_PUBLICOS"" TEXT(255));
DROP TABLE IF EXISTS ""hvisitas"";
CREATE TABLE ""hvisitas"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(7), ""FECHA_VISITA"" TEXT(10), ""CICLO"" INTEGER(11), ""TIPO"" TEXT(16), ""MOTIVO"" TEXT(20), ""PASADO_DESKTOP"" TEXT(1), ""PASADO_INTERNET"" TEXT(1), ""FECHA_SISTEMA"" TEXT(20), ""HORA_SISTEMA"" TEXT(20));
DROP TABLE IF EXISTS ""hvisita_detalles"";
CREATE TABLE ""hvisita_detalles"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(50), ""FECHA_VISITA"" TEXT(10), ""CICLO"" INTEGER(11), ""TIPO"" TEXT(11), ""PRODUCTO"" TEXT(255), ""MUETRAS"" INTEGER(11), ""MATERIAL_PRO"" INTEGER(11), ""MATERIAL_POP"" INTEGER(11), ""CARACTERISTICAS"" TEXT(35), ""BENEFICIOS"" TEXT(35), ""PRESENTACION"" TEXT(15), ""SOLICITA"" TEXT(25), ""SOLICITA_DETALLE"" TEXT(200), ""PRESCRIBE"" TEXT(25), ""NO_UTILIZA"" TEXT(20), ""PRESCRIBE_CARACTERISTICAS"" TEXT(35), ""PRESCRIBE_BENEFICIO"" TEXT(35));
DROP TABLE IF EXISTS ""identificacion"";
CREATE TABLE ""identificacion"" (""NOMBRE"" TEXT(40), ""CLAVE"" TEXT(5), ""ZONA"" TEXT(7), ""REGION"" TEXT(30), ""LINEA"" TEXT(30), ""S_CLAVE"" TEXT(5), ""S_NOMBRE"" TEXT(40), ""A_CLAVE"" TEXT(5), ""A_NOMBRE"" TEXT(40), ""ULTIMA_TRASMISION"" TEXT(8), ""PROCESADA"" TEXT(2), ""CICLO_PROCESADO"" INTEGER(11), ""NUMERO_TELEFONICO"" TEXT(50), ""DISTANCIA"" TEXT(10), ""BAUDIOS"" TEXT(10), ""ESPERA_TONO"" TEXT(3), ""MODEM"" TEXT(20), ""ESTADO"" TEXT(20), ""REGIONES"" TEXT(30), ""TIPO_TRANSMISION"" INTEGER(11), ""NUMERO_CELULAR"" TEXT(255));
DROP TABLE IF EXISTS ""inclusiones"";
CREATE TABLE ""inclusiones"" (""ZONA"" TEXT(7), ""DOCTOR"" TEXT(40), ""RUTA"" TEXT(40), ""CLINICA"" TEXT(40), ""CLASIFICACION"" TEXT(2), ""ESPECIALIDAD"" TEXT(25), ""DIRECCION"" TEXT(255), ""PARA_APROBACION"" TEXT(1), ""NRO_VISITAS"" REAL(8), ""ULTIMA_VISITA"" TEXT(2), ""NRO_SANITARIO"" TEXT(7));
DROP TABLE IF EXISTS ""MD_Pedidos"";
CREATE TABLE ""MD_Pedidos"" ( ""ID_Visitador""  INTEGER NOT NULL, ""ID_Pedido""  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, ""ID_Farmacia"" INTEGER NOT NULL, ""ID_TipoDescuento""  INTEGER NOT NULL, ""NU_CantTotalPedido""  REAL(18,2) NOT NULL, ""NU_CostoTotalPedido""  REAL(18,2) NOT NULL, ""NU_CantTotalBonif""  REAL(18,2) NOT NULL, ""NU_Descuento""  REAL(4,2) NOT NULL, ""TX_Contacto""  TEXT(50) NOT NULL, ""TX_TelefonoCont""  TEXT(20) NOT NULL, ""TX_Observacion""  TEXT(50), ""FE_Registro""  TEXT NOT NULL DEFAULT (datetime('now','localtime')), ""BO_Procesado""  TEXT(1) NOT NULL, ""NU_Estatus""  INTEGER NOT NULL, ""NU_EstatusProcesado""  INTEGER NOT NULL, ""ID_PedidoMedinet"" INTEGER);
DROP TABLE IF EXISTS ""MD_PedidosDetalle"";
CREATE TABLE ""MD_PedidosDetalle"" (""ID_Visitador""  INTEGER NOT NULL, ""ID_Pedido""  INTEGER NOT NULL, ""ID_Farmacia"" INTEGER NOT NULL, ""ID_DrogueriaAlmacen""  INTEGER NOT NULL, ""ID_Producto""  INTEGER NOT NULL, ""TX_CodFarmacia""  TEXT(50) NOT NULL, ""NU_CantPedido""  INTEGER NOT NULL, ""NU_PrecioProducto""  REAL(8,2) NOT NULL, ""NU_CantBonif""  INTEGER NOT NULL, ""NU_DescuentoProducto"" REAL(4,2) NOT NULL DEFAULT 0.00, ""BO_Estatus""  INTEGER NOT NULL, PRIMARY KEY (""ID_Pedido"", ""ID_Visitador"", ""ID_Producto""), FOREIGN KEY (""ID_Pedido"", ""ID_Visitador"") REFERENCES ""MD_Pedidos"" (""ID_Pedido"", ""ID_Visitador""), FOREIGN KEY (""ID_DrogueriaAlmacen"") REFERENCES ""MW_DrogueriasAlmacenes"" (""ID_DrogueriaAlmacen""), FOREIGN KEY (""ID_Producto"") REFERENCES ""MW_Productos"" (""ID_Producto""));
DROP TABLE IF EXISTS ""MD_ProductosTemp"";
CREATE TABLE ""MD_ProductosTemp"" (""ID_ProductoTemp"" INTEGER NOT NULL, ""ID_Visitador""  INTEGER NOT NULL, ""ID_Producto""  INTEGER NOT NULL, ""ID_DrogueriaAlmacen""  INTEGER NOT NULL, ""ID_Farmacia"" INTEGER NOT NULL, ""TX_IDProducto"" TEXT(10) NOT NULL, ""TX_Producto""  TEXT(50) NOT NULL, ""TX_ProductoDesc""  TEXT(100), ""TX_CodFarmacia""  TEXT(50) NULL, ""NU_CantPedido""  INTEGER NOT NULL, ""NU_PrecioProducto""  NUMERIC NOT NULL, ""NU_CantBono""  INTEGER NOT NULL, ""NU_DescuentoProducto"" REAL(4,2) NOT NULL DEFAULT 0.00, ""BO_Activo"" TEXT(1) NOT NULL DEFAULT 'N', PRIMARY KEY (""ID_ProductoTemp""));
DROP TABLE IF EXISTS ""mw_activespeciales"";
CREATE TABLE ""mw_activespeciales"" (""ID_ActivEspecial"" INTEGER(11), ""TX_ActivEspecial"" TEXT(255), ""ID_Linea"" INTEGER(11), ""ID_Marca"" INTEGER(11), ""ID_CicloIni"" INTEGER(11), ""ID_CicloFin"" INTEGER(11), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_beneficiovisitas"";
CREATE TABLE ""mw_beneficiovisitas"" (""ID_Beneficio"" INTEGER(11), ""TX_Beneficio"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_cadenafarmacias"";
CREATE TABLE ""mw_cadenafarmacias"" (""ID_Cadena"" INTEGER(11), ""TX_Cadena"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_caracteristicavisitas"";
CREATE TABLE ""mw_caracteristicavisitas"" (""ID_Caracteristica"" INTEGER(11), ""TX_Caracteristica"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_cargos"";
CREATE TABLE ""mw_cargos"" (""ID_Cargo"" INTEGER(11), ""TX_Cargo"" TEXT(255), ""BO_Visita"" INTEGER(11), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_ciclos"";
CREATE TABLE ""mw_ciclos"" (""ID_Ciclo"" INTEGER(11), ""NU_Ano"" INTEGER(11), ""NU_Ciclo"" INTEGER(11), ""FE_CicloIni"" TEXT(8), ""FE_CicloFin"" TEXT(8), ""FE_CicloProrroga"" TEXT(8), ""NU_DiasCancelarVehiculo"" INTEGER(11), ""NU_DiasHabiles"" INTEGER(11), ""FE_CargaInv"" TEXT(8), ""FE_IniDistribucion"" TEXT(8), ""FE_FinDistribucion"" TEXT(8), ""NU_Estatus"" INTEGER(11), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_clasificacionfarmacias"";
CREATE TABLE ""mw_clasificacionfarmacias"" (""ID_Clasificacion"" INTEGER(11), ""TX_Clasificacion"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_clasificacionmedicos"";
CREATE TABLE ""mw_clasificacionmedicos"" (""ID_Clasificacion"" INTEGER(11), ""TX_Clasificacion"" TEXT(255), ""NU_Puntaje"" INTEGER(11), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_conceptodescuentos"";
CREATE TABLE ""mw_conceptodescuentos"" (""ID_ConceptoDescuento"" INTEGER(11), ""TX_ConceptoDescuento"" TEXT(255), ""BO_AplicaVehiculo"" INTEGER(11), ""BO_AplicaCobertura"" INTEGER(11), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_consideracionmedicos"";
CREATE TABLE ""mw_consideracionmedicos"" (""ID_Consideracion"" INTEGER(11), ""TX_Consideracion"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_conveniofarmacias"";
CREATE TABLE ""mw_conveniofarmacias"" (""ID_Convenio"" INTEGER(11), ""TX_Convenio"" TEXT(255), ""ID_Region"" INTEGER(11), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_deportemedicos"";
CREATE TABLE ""mw_deportemedicos"" (""ID_Deporte"" INTEGER(11), ""TX_Deporte"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_droguerias"";
CREATE TABLE ""mw_droguerias"" (""ID_Drogueria"" INTEGER(11), ""TX_Drogueria"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""MW_DrogueriasAlmacenes"";
CREATE TABLE ""MW_DrogueriasAlmacenes"" (""ID_DrogueriaAlmacen""  INTEGER NOT NULL, ""ID_Drogueria""  INTEGER NOT NULL, ""TX_Almacen""  TEXT(30) NOT NULL, ""TX_IDAlmacen""  TEXT(10), ""BO_Activo""  INTEGER NOT NULL, PRIMARY KEY (""ID_DrogueriaAlmacen""), FOREIGN KEY (""ID_Drogueria"") REFERENCES ""MW_Droguerias"" (""ID_Drogueria""));
DROP TABLE IF EXISTS ""MW_Empresas"";
CREATE TABLE ""MW_Empresas"" (""ID_Empresa"" INTEGER(11), ""TX_Empresa"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""MW_EspecialidadesMedicas"";
CREATE TABLE ""MW_EspecialidadesMedicas"" (""ID_Especialidad"" INTEGER(11), ""TX_Especialidad"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""MW_Estados"";
CREATE TABLE ""MW_Estados"" (""ID_Estado"" INTEGER(11), ""TX_Estado"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_lineas"";
CREATE TABLE ""mw_lineas"" (""ID_Linea"" INTEGER(11), ""TX_Linea"" TEXT(255), ""TX_LineaAbr"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_marcas"";
CREATE TABLE ""mw_marcas"" (""ID_Marca"" INTEGER(11), ""TX_Marca"" TEXT(255), ""ID_Laboratorio"" INTEGER(11), ""TX_Posicionamiento"" TEXT(400), ""FE_Registro"" TEXT(8), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""MW_Motivos"";
CREATE TABLE ""MW_Motivos"" (""ID_Motivo"" INTEGER(11), ""TX_Motivo"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""MW_MotivosSolicitudes"";
CREATE TABLE ""MW_MotivosSolicitudes"" (""ID_MotivoSolicitud"" INTEGER(11), ""TX_MotivoSolicitud"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""MW_Productos"";
CREATE TABLE ""MW_Productos"" (""ID_Producto""  INTEGER NOT NULL, ""ID_Marca""  INTEGER NOT NULL, ""TX_Producto""  TEXT(150) NOT NULL, ""TX_IDProductoCliente""  TEXT(15) NOT NULL, ""TX_ProductoDesc""  TEXT(250) NOT NULL, ""BO_Activo""  INTEGER NOT NULL, PRIMARY KEY (""ID_Producto"" ASC));
DROP TABLE IF EXISTS ""MW_ProductosLineas"";
CREATE TABLE ""MW_ProductosLineas"" (""ID_ProductoLinea""  INTEGER NOT NULL, ""ID_Producto""  INTEGER NOT NULL, ""ID_Linea""  INTEGER NOT NULL, ""BO_Activo""  INTEGER NOT NULL, PRIMARY KEY (""ID_ProductoLinea""), FOREIGN KEY (""ID_Producto"") REFERENCES ""MW_Productos"" (""ID_Producto""), FOREIGN KEY (""ID_Linea"") REFERENCES ""mw_lineas"" (""ID_Linea""));
DROP TABLE IF EXISTS ""mw_regiones"";
CREATE TABLE ""mw_regiones"" (""ID_Region"" INTEGER(11), ""TX_Region"" TEXT(255), ""TX_RegionAbr"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""MW_TipoDescuentos"";
CREATE TABLE ""MW_TipoDescuentos"" (""ID_TipoDescuento""  INTEGER NOT NULL, ""TX_TipoDescuento""  TEXT(50) NOT NULL, ""NU_Descuento""  REAL(4,2) NOT NULL, ""BO_Activo""  INTEGER NOT NULL, PRIMARY KEY (""ID_TipoDescuento""));
DROP TABLE IF EXISTS ""mw_tipomedicos"";
CREATE TABLE ""mw_tipomedicos"" (""ID_TipoMedico"" INTEGER(11), ""TX_TipoMedico"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""MW_Visitadores"";
CREATE TABLE ""MW_Visitadores"" (""ID_Visitador"" INTEGER(11), ""TX_Nombre"" TEXT(255), ""TX_Apellido"" TEXT(255), ""TX_Usuario"" TEXT(255), ""TX_Password"" TEXT(255), ""ID_Empresa"" INTEGER(11), ""ID_Linea"" INTEGER(11), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""MW_VisitadoresHistorial"";
CREATE TABLE ""MW_VisitadoresHistorial"" (""ID_VisitadoresHistorial"" INTEGER(11), ""TX_Nombre"" TEXT(255), ""TX_Apellido"" TEXT(255), ""TX_Usuario"" TEXT(255), ""TX_Password"" TEXT(255), ""ID_Empresa"" INTEGER(11), ""ID_Linea"" INTEGER(11), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""MW_Zonas"";
CREATE TABLE ""MW_Zonas"" (""ID_Zona"" INTEGER(11), ""TX_Zona"" TEXT(255), ""ID_Region"" INTEGER(11), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""solicitudes"";
CREATE TABLE ""solicitudes"" (""REGISTRO"" TEXT(5), ""DOCTOR"" TEXT(40), ""ZONA"" TEXT(7), ""CICLO"" INTEGER(11), ""COMENTARIOS_PERSONALES"" TEXT(255), ""COMETARIO_PUBLICOS"" TEXT(255));
DROP TABLE IF EXISTS ""solicitudes_productos"";
CREATE TABLE ""solicitudes_productos"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(7), ""CICLO"" INTEGER(11), ""PRODUCTO"" TEXT(255), ""CANTIDAD"" INTEGER(11), ""MOTIVO"" TEXT(255), ""COMENTARIOS"" TEXT(255));
DROP TABLE IF EXISTS ""visitas"";
CREATE TABLE ""visitas"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(7), ""FECHA_VISITA"" TEXT(10), ""CICLO"" INTEGER(11), ""TIPO"" TEXT(16), ""MOTIVO"" TEXT(20), ""PASADO_DESKTOP"" TEXT(1), ""PASADO_INTERNET"" TEXT(1), ""FECHA_SISTEMA"" TEXT(255), ""HORA_SISTEMA"" TEXT(8));
DROP TABLE IF EXISTS ""visita_detalles"";
CREATE TABLE ""visita_detalles"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(50), ""FECHA_VISITA"" TEXT(10), ""CICLO"" INTEGER(11), ""TIPO"" TEXT(11), ""PRODUCTO"" TEXT(30), ""MUETRAS"" INTEGER(11), ""MATERIAL_PRO"" INTEGER(11), ""MATERIAL_POP"" INTEGER(11), ""CARACTERISTICAS"" TEXT(35), ""BENEFICIOS"" TEXT(35), ""PRESENTACION"" TEXT(15), ""SOLICITA"" TEXT(25), ""SOLICITA_DETALLE"" TEXT(200), ""PRESCRIBE"" TEXT(25), ""NO_UTILIZA"" TEXT(20), ""PRESCRIBE_CARACTERISTICAS"" TEXT(35), ""PRESCRIBE_BENEFICIO"" TEXT(35));
DROP TABLE IF EXISTS ""visita_detalles_productos"";
CREATE TABLE ""visita_detalles_productos"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(7), ""FECHA_VISITA"" TEXT(10), ""CICLO"" INTEGER(11), ""TIPO"" TEXT(11), ""PRODUCTO"" TEXT(255), ""MUETRAS"" INTEGER(11), ""MATERIAL_PRO"" INTEGER(11), ""MATERIAL_POP"" INTEGER(11), ""CARACTERISTICAS"" TEXT(35), ""BENEFICIOS"" TEXT(35), ""PRESENTACION"" TEXT(15), ""SOLICITA"" TEXT(25), ""SOLICITA_DETALLE"" TEXT(200), ""PRESCRIBE"" TEXT(25), ""NO_UTILIZA"" TEXT(20), ""PRESCRIBE_CARACTERISTICAS"" TEXT(35), ""PRESCRIBE_BENEFICIO"" TEXT(35));
DROP TABLE IF EXISTS ""mw_farmacias_detalles_productos"";
CREATE TABLE ""mw_farmacias_detalles_productos"" (""NUMERO"" INTEGER(11), ""CICLO_VISITA"" INTEGER(11), ""ZONA"" TEXT(7), ""FARMACIA"" TEXT(255), ""FECHA_VISITA"" TEXT(255), ""MOTIVO"" TEXT(255), ""PRODUCTO"" TEXT(255), ""CANTIDAD"" INTEGER(11), ""COMENTARIOS"" TEXT(255));
DROP TABLE IF EXISTS ""mw_hospital_detalles_medicos"";
CREATE TABLE ""mw_hospital_detalles_medicos"" (""NUMERO"" INTEGER(11), ""CICLO_VISITA"" INTEGER(11), ""ZONA"" TEXT(7), ""HOSPITAL"" TEXT(100), ""FECHA_VISITA"" TEXT(50), ""SERVICIO"" TEXT(50), ""MOTIVO"" TEXT(100), ""REGISTRO"" INTEGER(11), ""DOCTOR"" TEXT(100));
DROP TABLE IF EXISTS ""mw_inclusiones"";
CREATE TABLE ""mw_inclusiones"" (""ZONA"" TEXT(7), ""DOCTOR"" TEXT(40), ""RUTA"" TEXT(40), ""CLINICA"" TEXT(40), ""CLASIFICACION"" TEXT(2), ""ESPECIALIDAD"" TEXT(25), ""DIRECCION"" TEXT(255), ""PARA_APROBACION"" TEXT(1), ""NRO_VISITAS"" REAL(8), ""ULTIMA_VISITA"" TEXT(2), ""NRO_SANITARIO"" TEXT(7));
DROP TABLE IF EXISTS ""mw_solicitudes"";
CREATE TABLE ""mw_solicitudes"" (""REGISTRO"" TEXT(5), ""DOCTOR"" TEXT(40), ""ZONA"" TEXT(7), ""CICLO"" INTEGER(11), ""COMENTARIOS_PERSONALES"" TEXT(255), ""COMETARIO_PUBLICOS"" TEXT(255));
DROP TABLE IF EXISTS ""mw_visita_detalles"";
CREATE TABLE ""mw_visita_detalles"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(50), ""FECHA_VISITA"" TEXT(10), ""CICLO"" INTEGER(11), ""TIPO"" TEXT(11), ""PRODUCTO"" TEXT(255), ""MUETRAS"" INTEGER(11), ""MATERIAL_PRO"" INTEGER(11), ""MATERIAL_POP"" INTEGER(11), ""CARACTERISTICAS"" TEXT(35), ""BENEFICIOS"" TEXT(35), ""PRESENTACION"" TEXT(15), ""SOLICITA"" TEXT(25), ""SOLICITA_DETALLE"" TEXT(200), ""PRESCRIBE"" TEXT(25), ""NO_UTILIZA"" TEXT(20), ""PRESCRIBE_CARACTERISTICAS"" TEXT(35), ""PRESCRIBE_BENEFICIO"" TEXT(35));
DROP TABLE IF EXISTS ""mw_visitas"";
CREATE TABLE ""mw_visitas"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(7), ""FECHA_VISITA"" TEXT(10), ""CICLO"" INTEGER(11), ""TIPO"" TEXT(16), ""MOTIVO"" TEXT(20), ""PASADO_DESKTOP"" TEXT(1), ""PASADO_INTERNET"" TEXT(1), ""FECHA_SISTEMA"" TEXT(20), ""HORA_SISTEMA"" TEXT(20));
DROP TABLE IF EXISTS ""ayuda_visual"";
CREATE TABLE ""ayuda_visual"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(7), ""FECHA_VISITA"" TEXT(10), ""CICLO"" INTEGER(11), ""TIPO"" TEXT(16), ""MOTIVO"" TEXT(20), ""ESPECIALIDAD"" TEXT(25), ""CLASIFICACION"" TEXT(2), ""FECHA_SISTEMA"" TEXT(10), ""HORA_SISTEMA"" TEXT(8), ""PRODUCTO"" TEXT(30), ""POSICION"" TEXT(2), ""ORDEN"" INTEGER(11));
DROP TABLE IF EXISTS ""ayuda_visual_FE"";
CREATE TABLE ""ayuda_visual_FE"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(7), ""FECHA_VISITA"" TEXT(10), ""CICLO"" INTEGER(11), ""TIPO"" TEXT(16), ""MOTIVO"" TEXT(20), ""ESPECIALIDAD"" TEXT(25), ""CLASIFICACION"" TEXT(2), ""FECHA_SISTEMA"" TEXT(10), ""HORA_SISTEMA"" TEXT(8), ""PRODUCTO"" TEXT(30), ""POSICION"" TEXT(2), ""ORDEN"" INTEGER(11));
DROP TABLE IF EXISTS ""ayuda_visual_MP4"";
CREATE TABLE ""ayuda_visual_MP4"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(7), ""FECHA_VISITA"" TEXT(10), ""CICLO"" INTEGER(11), ""TIPO"" TEXT(16), ""MOTIVO"" TEXT(20), ""ESPECIALIDAD"" TEXT(25), ""CLASIFICACION"" TEXT(2), ""FECHA_SISTEMA"" TEXT(10), ""HORA_SISTEMA"" TEXT(8), ""PRODUCTO"" TEXT(30), ""POSICION"" TEXT(2), ""ORDEN"" INTEGER(11));
DROP TABLE IF EXISTS ""ayuda_visual_MP4_FE"";
CREATE TABLE ""ayuda_visual_MP4_FE"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(7), ""FECHA_VISITA"" TEXT(10), ""CICLO"" INTEGER(11), ""TIPO"" TEXT(16), ""MOTIVO"" TEXT(20), ""ESPECIALIDAD"" TEXT(25), ""CLASIFICACION"" TEXT(2), ""FECHA_SISTEMA"" TEXT(10), ""HORA_SISTEMA"" TEXT(8), ""PRODUCTO"" TEXT(30), ""POSICION"" TEXT(2), ""ORDEN"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_farmacias_personal"";
CREATE TABLE ""mw_farmacias_personal"" (""NUMERO"" INTEGER(11), ""ZONA"" TEXT(7), ""FARMACIA"" TEXT(100), ""PERSONAL"" TEXT(100), ""CARGO"" TEXT(50), ""TELEFONO"" TEXT(30), ""CUMPLEANO"" TEXT(10), ""OBSERVACION"" TEXT(100));
DROP TABLE IF EXISTS ""mw_hospital_personal"";
CREATE TABLE ""mw_hospital_personal"" (""NUMERO"" INTEGER(11), ""ZONA"" TEXT(7), ""HOSPITAL"" TEXT(100), ""PERSONAL"" TEXT(100), ""CARGO"" TEXT(50), ""TELEFONO"" TEXT(30), ""CUMPLEANO"" TEXT(10), ""OBSERVACION"" TEXT(100));
DROP TABLE IF EXISTS ""mw_configuracion"";
CREATE TABLE ""mw_configuracion"" (""ID_Config"" INTEGER(11), ""TX_Parametro"" TEXT(100), ""TX_Valor"" TEXT(255), ""TX_Descripcion"" TEXT(255));
DROP TABLE IF EXISTS ""mw_logs"";
CREATE TABLE ""mw_logs"" (""ID_Log"" INTEGER(11), ""FE_Fecha"" TEXT(20), ""TX_Accion"" TEXT(100), ""TX_Detalle"" TEXT(255), ""ID_Usuario"" INTEGER(11));
DROP TABLE IF EXISTS ""MW_DrogueriasProductos"";
CREATE TABLE ""MW_DrogueriasProductos"" (""ID_DrogueriaProducto"" INTEGER NOT NULL, ""ID_DrogueriaAlmacen"" INTEGER NOT NULL, ""ID_Producto"" INTEGER, ""TX_IDProductoDrogueria"" TEXT(15) NOT NULL, ""TX_ProductoDrogueria"" TEXT(150) NOT NULL, ""NU_PrecioProducto"" REAL(10,2) NOT NULL, ""NU_InvProducto"" INTEGER NOT NULL, ""TX_DrogueriaRef1"" TEXT(50), ""TX_DrogueriaRef2"" TEXT(50), ""BO_Activo"" INTEGER NOT NULL, PRIMARY KEY (""ID_DrogueriaProducto""), FOREIGN KEY (""ID_DrogueriaAlmacen"") REFERENCES ""MW_DrogueriasAlmacenes"" (""ID_DrogueriaAlmacen""), FOREIGN KEY (""ID_Producto"") REFERENCES ""MW_Productos"" (""ID_Producto""));
DROP TABLE IF EXISTS ""MW_PedidosEstatus"";
CREATE TABLE ""MW_PedidosEstatus"" (""ID_PedidoEstatus"" INTEGER(11), ""TX_PedidoEstatus"" TEXT(50), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""MW_PedidosEstatusProcesado"";
CREATE TABLE ""MW_PedidosEstatusProcesado"" (""ID_PedidoEstatusProcesado"" INTEGER(11), ""TX_PedidoEstatusProcesado"" TEXT(50), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""MW_PedidosFacturasCabeceras"";
CREATE TABLE ""MW_PedidosFacturasCabeceras"" (""ID_FacturaMedinet"" INTEGER NOT NULL, ""ID_PedidoMedinet"" INTEGER NOT NULL, ""ID_Drogueria"" INTEGER NOT NULL, ""NU_FacturaDrogueria"" INTEGER NOT NULL, ""NU_PedidoDrogueria"" INTEGER, ""FE_FacturaDrogueria"" TEXT NOT NULL, ""NU_TotalUnidades"" INTEGER, ""NU_CostoTotalFactura"" REAL(18,4) NOT NULL, ""FE_Recibido"" TEXT, ""FE_Modificado"" TEXT, PRIMARY KEY (""ID_FacturaMedinet""));
DROP TABLE IF EXISTS ""MW_PedidosFacturasDetalles"";
CREATE TABLE ""MW_PedidosFacturasDetalles"" (""ID_Detalle"" INTEGER NOT NULL, ""ID_FacturaMedinet"" INTEGER NOT NULL, ""ID_Producto"" INTEGER NOT NULL, ""TX_IDProductoDrogueria"" TEXT(15) NOT NULL, ""TX_Lote"" TEXT(50), ""NU_CantidadFacturada"" INTEGER NOT NULL, ""FE_Recibido"" TEXT, ""FE_Modificado"" TEXT, PRIMARY KEY (""ID_Detalle""));
DROP TABLE IF EXISTS ""PedidosCodVisDrog"";
CREATE TABLE ""PedidosCodVisDrog"" (""ID_Visitador"" INTEGER(11), ""ID_Drogueria"" INTEGER(11), ""TX_Codigo"" TEXT(20), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""PedidosCorreosVisCopia"";
CREATE TABLE ""PedidosCorreosVisCopia"" (""ID_Visitador"" INTEGER(11), ""TX_Email"" TEXT(100), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""PedidosFarmacias"";
CREATE TABLE ""PedidosFarmacias"" (""ID_VisitadorHistorial"" INTEGER, ""ID_Farmacia"" INTEGER PRIMARY KEY AUTOINCREMENT, ""ID_DrogueriaAlmacen"" INTEGER, ""ID_CadenaFarmacias"" INTEGER, ""ID_Clasificacion"" INTEGER, ""ID_Estado"" INTEGER, ""NU_Brick"" INTEGER, ""TX_Farmacia"" TEXT(50), ""TX_Direccion"" TEXT(250), ""TX_Contacto"" TEXT(50), ""TX_Telefono"" TEXT(11), ""TX_Rif"" TEXT(11), ""BO_Activo"" INTEGER);
DROP TABLE IF EXISTS ""ayuda_visual"";
CREATE TABLE ""ayuda_visual"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(7), ""FECHA_VISITA"" TEXT(10), ""CICLO"" INTEGER(11), ""TIPO"" TEXT(16), ""MOTIVO"" TEXT(20), ""ESPECIALIDAD"" TEXT(25), ""CLASIFICACION"" TEXT(2), ""FECHA_SISTEMA"" TEXT(10), ""HORA_SISTEMA"" TEXT(8), ""PRODUCTO"" TEXT(30), ""POSICION"" TEXT(2), ""ORDEN"" INTEGER(11));
DROP TABLE IF EXISTS ""ayuda_visual_FE"";
CREATE TABLE ""ayuda_visual_FE"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(7), ""FECHA_VISITA"" TEXT(10), ""CICLO"" INTEGER(11), ""TIPO"" TEXT(16), ""MOTIVO"" TEXT(20), ""ESPECIALIDAD"" TEXT(25), ""CLASIFICACION"" TEXT(2), ""FECHA_SISTEMA"" TEXT(10), ""HORA_SISTEMA"" TEXT(8), ""PRODUCTO"" TEXT(30), ""POSICION"" TEXT(2), ""ORDEN"" INTEGER(11));
DROP TABLE IF EXISTS ""ayuda_visual_MP4"";
CREATE TABLE ""ayuda_visual_MP4"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(7), ""FECHA_VISITA"" TEXT(10), ""CICLO"" INTEGER(11), ""TIPO"" TEXT(16), ""MOTIVO"" TEXT(20), ""ESPECIALIDAD"" TEXT(25), ""CLASIFICACION"" TEXT(2), ""FECHA_SISTEMA"" TEXT(10), ""HORA_SISTEMA"" TEXT(8), ""PRODUCTO"" TEXT(30), ""POSICION"" TEXT(2), ""ORDEN"" INTEGER(11));
DROP TABLE IF EXISTS ""ayuda_visual_MP4_FE"";
CREATE TABLE ""ayuda_visual_MP4_FE"" (""REGISTRO"" TEXT(5), ""ZONA"" TEXT(7), ""FECHA_VISITA"" TEXT(10), ""CICLO"" INTEGER(11), ""TIPO"" TEXT(16), ""MOTIVO"" TEXT(20), ""ESPECIALIDAD"" TEXT(25), ""CLASIFICACION"" TEXT(2), ""FECHA_SISTEMA"" TEXT(10), ""HORA_SISTEMA"" TEXT(8), ""PRODUCTO"" TEXT(30), ""POSICION"" TEXT(2), ""ORDEN"" INTEGER(11));
DROP TABLE IF EXISTS ""farmacias_detalles_productos"";
CREATE TABLE ""farmacias_detalles_productos"" (""NUMERO"" INTEGER(11), ""CICLO_VISITA"" INTEGER(11), ""ZONA"" TEXT(7), ""FARMACIA"" TEXT(255), ""FECHA_VISITA"" TEXT(255), ""MOTIVO"" TEXT(255), ""PRODUCTO"" TEXT(255), ""CANTIDAD"" INTEGER(11), ""COMENTARIOS"" TEXT(255));
DROP TABLE IF EXISTS ""hfarmacias_detalles_productos"";
CREATE TABLE ""hfarmacias_detalles_productos"" (""NUMERO"" INTEGER(11), ""CICLO_VISITA"" INTEGER(11), ""ZONA"" TEXT(7), ""FARMACIA"" TEXT(255), ""FECHA_VISITA"" TEXT(255), ""MOTIVO"" TEXT(255), ""PRODUCTO"" TEXT(255), ""CANTIDAD"" INTEGER(11), ""COMENTARIOS"" TEXT(255));
DROP TABLE IF EXISTS ""mw_especialidades"";
CREATE TABLE ""mw_especialidades"" (""ID_Especialidad"" INTEGER(11), ""TX_Especialidad"" TEXT(255), ""TX_EspecialidadAbr"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_farmacias"";
CREATE TABLE ""mw_farmacias"" (""ID_Farmacia"" INTEGER(11), ""TX_Farmacia"" TEXT(255), ""TX_Rif"" TEXT(255), ""ID_Estado"" INTEGER(11), ""ID_Ciudad"" INTEGER(11), ""ID_Brick"" INTEGER(11), ""TX_Ruta"" TEXT(255), ""TX_Direccion"" TEXT(255), ""TX_Telefono1"" TEXT(255), ""TX_Telefono2"" TEXT(255), ""ID_Cadena"" INTEGER(11), ""ID_Clasificacion"" INTEGER(11), ""FE_Registro"" TEXT(8), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_hospitales"";
CREATE TABLE ""mw_hospitales"" (""ID_Hospital"" INTEGER(11), ""TX_Hospital"" TEXT(255), ""ID_Estado"" INTEGER(11), ""ID_Ciudad"" INTEGER(11), ""ID_Brick"" INTEGER(11), ""ID_Institucion"" INTEGER(11), ""TX_Ruta"" TEXT(255), ""TX_Direccion"" TEXT(255), ""TX_Telefono1"" TEXT(255), ""TX_Telefono2"" TEXT(255), ""BO_Cliente"" INTEGER(11), ""BO_Docente"" INTEGER(11), ""FE_Registro"" TEXT(8), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_medicos"";
CREATE TABLE ""mw_medicos"" (""ID_Medico"" INTEGER(11), ""NU_RegistroSanitario"" INTEGER(11), ""TX_Nombre1"" TEXT(255), ""TX_Nombre2"" TEXT(255), ""TX_Apellido1"" TEXT(255), ""TX_Apellido2"" TEXT(255), ""TX_Sello"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""postular"";
CREATE TABLE ""postular"" (""ID_Postulacion"" INTEGER(11), ""ID_Visitador"" INTEGER(11), ""TX_Descripcion"" TEXT(255), ""FE_Fecha"" TEXT(10), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""puntos"";
CREATE TABLE ""puntos"" (""Clasificacion"" TEXT(2), ""Puntos"" INTEGER(11));
DROP TABLE IF EXISTS ""resumen_transmision"";
CREATE TABLE ""resumen_transmision"" (""Zona"" TEXT(7), ""Ciclo"" INTEGER(11), ""Fecha"" TEXT(40), ""Hora"" TEXT(15), ""Fichados"" INTEGER(11), ""Visitados"" INTEGER(11), ""PorcentajeFichadoVisitados"" REAL(8), ""Revisitas"" INTEGER(11), ""Revisitados"" INTEGER(11), ""PorcentajeRevisitasRevisitados"" REAL(8), ""DiasDeCiclos"" INTEGER(11), ""DiasTranscurridos"" INTEGER(11), ""DiasTrabajados"" REAL(8), ""DiasDescontados"" REAL(8), ""PDE"" REAL(8), ""PDR"" REAL(8), ""PDP"" REAL(8), ""PR"" REAL(8), ""PP"" REAL(8), ""Mix"" REAL(8), ""FichadosPtos"" INTEGER(11), ""VisitadosPtos"" INTEGER(11), ""PorcentajeFichadoVisitadosPtos"" REAL(8), ""RevisitasPtos"" INTEGER(11), ""RevisitadosPtos"" INTEGER(11), ""PorcentajeRevisitasRevisitadosPtos"" REAL(8), ""PDEPtos"" REAL(8), ""PDRPtos"" REAL(8), ""PDPPtos"" REAL(8), ""PRPtos"" REAL(8), ""PPPtos"" REAL(8), ""MixPtos"" REAL(8), ""DescontadoS"" REAL(8), ""DescontadoN"" REAL(8), ""Ficha"" REAL(8), ""AltasPostulados"" INTEGER(11), ""AltasAprobados"" INTEGER(11), ""BajasPostulados"" INTEGER(11), ""BajasAprobados"" INTEGER(11), ""Transmitir"" TEXT(2));
DROP TABLE IF EXISTS ""resumen_transmision_farmacia"";
CREATE TABLE ""resumen_transmision_farmacia"" (""Zona"" TEXT(7), ""Ciclo"" INTEGER(11), ""Fecha"" TEXT(40), ""Hora"" TEXT(15), ""Fichados"" INTEGER(11), ""Visitados"" INTEGER(11), ""PorcentajeFichadoVisitados"" REAL(8), ""Revisitas"" INTEGER(11), ""Revisitados"" INTEGER(11), ""PorcentajeRevisitasRevisitados"" REAL(8), ""DiasDeCiclos"" INTEGER(11), ""DiasTranscurridos"" INTEGER(11), ""DiasTrabajados"" REAL(8), ""DiasDescontados"" REAL(8), ""PDE"" REAL(8), ""PDR"" REAL(8), ""PDP"" REAL(8), ""PR"" REAL(8), ""PP"" REAL(8), ""Mix"" REAL(8), ""FichadosPtos"" INTEGER(11), ""VisitadosPtos"" INTEGER(11), ""PorcentajeFichadoVisitadosPtos"" REAL(8), ""RevisitasPtos"" INTEGER(11), ""RevisitadosPtos"" INTEGER(11), ""PorcentajeRevisitasRevisitadosPtos"" REAL(8), ""PDEPtos"" REAL(8), ""PDRPtos"" REAL(8), ""PDPPtos"" REAL(8), ""PRPtos"" REAL(8), ""PPPtos"" REAL(8), ""MixPtos"" REAL(8), ""DescontadoS"" REAL(8), ""DescontadoN"" REAL(8), ""Ficha"" REAL(8), ""AltasPostulados"" INTEGER(11), ""AltasAprobados"" INTEGER(11), ""BajasPostulados"" INTEGER(11), ""BajasAprobados"" INTEGER(11), ""Transmitir"" TEXT(2));
DROP TABLE IF EXISTS ""resumen_transmision_hospital"";
CREATE TABLE ""resumen_transmision_hospital"" (""Zona"" TEXT(7), ""Ciclo"" INTEGER(11), ""Fecha"" TEXT(40), ""Hora"" TEXT(15), ""Fichados"" INTEGER(11), ""Visitados"" INTEGER(11), ""PorcentajeFichadoVisitados"" REAL(8), ""Revisitas"" INTEGER(11), ""Revisitados"" INTEGER(11), ""PorcentajeRevisitasRevisitados"" REAL(8), ""DiasDeCiclos"" INTEGER(11), ""DiasTranscurridos"" INTEGER(11), ""DiasTrabajados"" REAL(8), ""DiasDescontados"" REAL(8), ""PDE"" REAL(8), ""PDR"" REAL(8), ""PDP"" REAL(8), ""PR"" REAL(8), ""PP"" REAL(8), ""Mix"" REAL(8), ""FichadosPtos"" INTEGER(11), ""VisitadosPtos"" INTEGER(11), ""PorcentajeFichadoVisitadosPtos"" REAL(8), ""RevisitasPtos"" INTEGER(11), ""RevisitadosPtos"" INTEGER(11), ""PorcentajeRevisitasRevisitadosPtos"" REAL(8), ""PDEPtos"" REAL(8), ""PDRPtos"" REAL(8), ""PDPPtos"" REAL(8), ""PRPtos"" REAL(8), ""PPPtos"" REAL(8), ""MixPtos"" REAL(8), ""DescontadoS"" REAL(8), ""DescontadoN"" REAL(8), ""Ficha"" REAL(8), ""AltasPostulados"" INTEGER(11), ""AltasAprobados"" INTEGER(11), ""BajasPostulados"" INTEGER(11), ""BajasAprobados"" INTEGER(11), ""Transmitir"" TEXT(2));
DROP TABLE IF EXISTS ""serial"";
CREATE TABLE ""serial"" (""ID_Serial"" INTEGER(11), ""TX_Serial"" TEXT(100), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""temp_hoja_ruta_propuesta"";
CREATE TABLE ""temp_hoja_ruta_propuesta"" (""Num"" INTEGER(11), ""CICLO"" INTEGER(11), ""ZONA"" TEXT(7), ""SEMANA"" INTEGER(11), ""DIA"" TEXT(10), ""AM"" TEXT(250), ""PM"" TEXT(250));
DROP TABLE IF EXISTS ""mw_umbrales"";
CREATE TABLE ""mw_umbrales"" (""ID_Umbral"" INTEGER(11), ""NU_PDR"" INTEGER(11), ""NU_Farmacias"" INTEGER(11), ""NU_Hospitales"" INTEGER(11), ""NU_Percent_PDR"" REAL(16), ""NU_Percent_Visita"" REAL(16), ""NU_Percent_Revisita"" REAL(16), ""NU_Percent_Mix"" REAL(16), ""ID_CicloIni"" INTEGER(11), ""ID_CicloFin"" INTEGER(11), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""umbrales"";
CREATE TABLE ""umbrales"" (""ID_Umbral"" INTEGER(11), ""TX_Concepto"" TEXT(100), ""NU_Valor"" REAL(10,2), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""usuario"";
CREATE TABLE ""usuario"" (""USUARIO_NOMBRE"" TEXT(35), ""USUARIO_CLAVE"" TEXT(4), ""USUARIO_ZONA"" TEXT(7), ""USUARIO_CICLO"" INTEGER(11), ""SELECCION_NRO"" INTEGER(11), ""SELECCION_NOMBRE"" TEXT(35), ""ENTRO_SISTEMA"" TEXT(2), ""SALIO_SISTEMA"" TEXT(2), ""CONTINUAR"" TEXT(2), ""ULTIMA_TRANSMISION"" TEXT(25), ""TIPO_TRANSMISION"" INTEGER(11), ""PROVIENE"" TEXT(50), ""FECHA_DESDE"" TEXT(50), ""FECHA_HASTA"" TEXT(50), ""HOJA_DE_RUTA"" INTEGER(11), ""TOTAL_TRANSMITIDO"" TEXT(8), ""VERSION_SISTEMA"" TEXT(30), ""VERSION_MERCADEO"" TEXT(60), ""MEMORIA"" INTEGER(11), ""BATERIA"" INTEGER(11), ""CIERRE_CICLO"" TEXT(2), ""INFORMACION"" TEXT(2), ""COBERTURA"" TEXT(2), ""ALTAS_BAJAS"" TEXT(2), ""TRANSMISION"" TEXT(2));
DROP TABLE IF EXISTS ""version"";
CREATE TABLE ""version"" (""ID_Version"" INTEGER(11), ""TX_Version"" TEXT(20), ""FE_Fecha"" TEXT(10), ""TX_Descripcion"" TEXT(255));
DROP TABLE IF EXISTS ""versiones"";
CREATE TABLE ""versiones"" (""Zona"" TEXT(8), ""AltasBajas"" TEXT(5), ""Bricks"" TEXT(5), ""Busquedad01"" TEXT(5), ""Busquedad02"" TEXT(5), ""Busquedad03"" TEXT(5), ""Busquedad04"" TEXT(5), ""Busquedad05"" TEXT(5), ""CicloCerrado"" TEXT(5), ""Cierre"" TEXT(5), ""Coberturas"" TEXT(5), ""ConfigurarModem"" TEXT(5), ""EliminarEsqPromo"" TEXT(5), ""EliminarMedicos"" TEXT(5), ""Farmacia01"" TEXT(5), ""Fichas"" TEXT(5), ""HojaRuta"" TEXT(5), ""IncluirMedicos"" TEXT(5), ""Informacion"" TEXT(5), ""Menu"" TEXT(5), ""OMA"" TEXT(5), ""OptAltasBajas"" TEXT(5), ""PVM"" TEXT(5), ""PVMUpdate"" TEXT(5), ""ResumenCierre"" TEXT(5), ""Retransmitir"" TEXT(5), ""TrabajoDiario"" TEXT(5), ""Transmision"" TEXT(5), ""Visitas"" TEXT(5), ""VisitasNoFichado"" TEXT(5), ""GuardarMix"" TEXT(3));
DROP TABLE IF EXISTS ""farmacias_personal"";
CREATE TABLE ""farmacias_personal"" (""ID"" INTEGER(11), ""NUMERO"" INTEGER(11), ""ZONA"" TEXT(7), ""NOMBRE"" TEXT(40), ""CARGO"" TEXT(40), ""TELEFONO"" TEXT(40), ""CORREO"" TEXT(255), ""CUMPLEANO_MES"" TEXT(25), ""CUMPLEANO_DIA"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_esquemaspromocionales"";
CREATE TABLE ""mw_esquemaspromocionales"" (""ID_EsquemaPromocional"" INTEGER(11), ""ID_Ciclo"" INTEGER(11), ""ID_Linea"" INTEGER(11), ""ID_Especialidad"" INTEGER(11), ""ID_Posicion"" INTEGER(11), ""ID_Marca"" INTEGER(11), ""FE_Registro"" TEXT(8), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_farmaciasdroguerias"";
CREATE TABLE ""mw_farmaciasdroguerias"" (""ID_FarmaciasDroguerias"" INTEGER(11), ""ID_Drogueria"" INTEGER(11), ""ID_Farmacia"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_galenicas"";
CREATE TABLE ""mw_galenicas"" (""ID_Galenica"" INTEGER(11), ""TX_Galenica"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_idiomamedicos"";
CREATE TABLE ""mw_idiomamedicos"" (""ID_Idioma"" INTEGER(11), ""TX_Idioma"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_instituciones"";
CREATE TABLE ""mw_instituciones"" (""ID_Institucion"" INTEGER(11), ""TX_Institucion"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_laboratorios"";
CREATE TABLE ""mw_laboratorios"" (""ID_Laboratorio"" INTEGER(11), ""TX_Laboratorio"" TEXT(255), ""BO_LaboratorioMedinet"" INTEGER(11), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_lineasespecialidades"";
CREATE TABLE ""mw_lineasespecialidades"" (""ID_LineasEspecialidades"" INTEGER(11), ""ID_Linea"" INTEGER(11), ""ID_Especialidad"" INTEGER(11), ""ID_CicloIni"" INTEGER(11), ""ID_CicloFin"" INTEGER(11), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_lineasespecialidadesmarcas"";
CREATE TABLE ""mw_lineasespecialidadesmarcas"" (""ID_LinEspMarcas"" INTEGER(11), ""ID_LineasEspecialidades"" INTEGER(11), ""ID_Marca"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_locacionbricks"";
CREATE TABLE ""mw_locacionbricks"" (""ID_Brick"" INTEGER(11), ""NU_Brick"" TEXT(8), ""TX_Brick"" TEXT(255), ""BO_Activo"" INTEGER(11), ""ID_Ciudad"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_locacionciudades"";
CREATE TABLE ""mw_locacionciudades"" (""ID_Ciudad"" INTEGER(11), ""TX_Ciudad"" TEXT(255), ""BO_Activo"" INTEGER(11), ""ID_Estado"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_locacionestados"";
CREATE TABLE ""mw_locacionestados"" (""ID_Estado"" INTEGER(11), ""TX_Estado"" TEXT(255), ""BO_Activo"" INTEGER(11), ""ID_Pais"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_locacionpaises"";
CREATE TABLE ""mw_locacionpaises"" (""ID_Pais"" INTEGER(11), ""TX_Pais"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_marcascompetencias"";
CREATE TABLE ""mw_marcascompetencias"" (""ID_MarcasCompentencia"" INTEGER(11), ""ID_Marca"" INTEGER(11), ""ID_Competencia"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_marcaslineas"";
CREATE TABLE ""mw_marcaslineas"" (""ID_MarcasLineas"" INTEGER(11), ""ID_Marca"" INTEGER(11), ""ID_Linea"" INTEGER(11), ""ID_CicloIni"" INTEGER(11), ""ID_CicloFin"" INTEGER(11), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_motivovisitas"";
CREATE TABLE ""mw_motivovisitas"" (""ID_Motivo"" INTEGER(11), ""TX_Motivo"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_ocupacionmedicos"";
CREATE TABLE ""mw_ocupacionmedicos"" (""ID_Ocupacion"" INTEGER(11), ""TX_Ocupacion"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_patologias"";
CREATE TABLE ""mw_patologias"" (""ID_Patologia"" INTEGER(11), ""TX_Patologia"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_posiciones"";
CREATE TABLE ""mw_posiciones"" (""ID_Posicion"" INTEGER(11), ""TX_Posicion"" TEXT(255), ""TX_PosicionAbr"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_publicaciones"";
CREATE TABLE ""mw_publicaciones"" (""ID_Publicacion"" INTEGER(11), ""TX_Publicacion"" TEXT(255), ""TX_Archivo"" TEXT(255), ""ID_Marca"" INTEGER(11), ""FE_Registro"" TEXT(8), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_regionesestados"";
CREATE TABLE ""mw_regionesestados"" (""ID_RegionesEstados"" INTEGER(11), ""ID_Region"" INTEGER(11), ""ID_Estado"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_serviciohospitales"";
CREATE TABLE ""mw_serviciohospitales"" (""ID_Servicio"" INTEGER(11), ""TX_Servicio"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_solicitudvisitas"";
CREATE TABLE ""mw_solicitudvisitas"" (""ID_Solicitud"" INTEGER(11), ""TX_Solicitud"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_tipovisitafarmacias"";
CREATE TABLE ""mw_tipovisitafarmacias"" (""ID_TipoVisita"" INTEGER(11), ""TX_TipoVisita"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_tipovisitahospitales"";
CREATE TABLE ""mw_tipovisitahospitales"" (""ID_TipoVisita"" INTEGER(11), ""TX_TipoVisita"" TEXT(255), ""BO_Activo"" INTEGER(11));
DROP TABLE IF EXISTS ""mw_usuarios"";
CREATE TABLE ""mw_usuarios"" (""ID_Usuario"" INTEGER(11), ""TX_Usuario"" TEXT(255), ""TX_Clave"" TEXT(255), ""NU_Cedula"" INTEGER(11), ""TX_Nombre"" TEXT(255), ""TX_Apellido"" TEXT(255), ""ID_Cargo"" INTEGER(11), ""TX_Email1"" TEXT(255), ""TX_Email2"" TEXT(255), ""TX_Telefono1"" TEXT(255), ""TX_Telefono2"" TEXT(255), ""TX_Direccion"" TEXT(255), ""ID_Ciudad"" INTEGER(11), ""TX_Imei"" TEXT(255), ""BO_Expiro"" INTEGER(11), ""FE_Registro"" TEXT(8), ""BO_Activo"" INTEGER(11));";

            contenido.AppendLine(todasLasDefiniciones);
        }
        
        private async Task ObtenerDatosHistoricosAsync(SqlConnection connection, StringBuilder contenido, string tablaSQL, string tablaSQLite, long visitadorId, int ano, int ciclo)
        {
            // Replicar exactamente la lógica del ClickOnce
            int cicloHistorico = ciclo == 1 ? 12 : ciclo;
            
            for (int cicloR = 1; cicloR < 3; cicloR++)
            {
                cicloHistorico -= 1;
                cicloHistorico = cicloHistorico == -1 ? 11 : cicloHistorico;
                int anoTemp = (cicloHistorico == 10 || cicloHistorico == 11) && (ciclo == 1 || ciclo == 2) ? ano - 1 : ano;

                var query = $"SELECT * FROM {tablaSQL} WHERE NU_ANO={anoTemp} AND CICLO={cicloHistorico} AND ZONA='{visitadorId}'";
                
                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();
                
                // Replicar exactamente la lógica del ClickOnce: sqlReader.Read() consume el primer registro
                if (await reader.ReadAsync())
                {
                    var columnNames = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                    var finalColumns = new List<string>();
                    
                    // Excluir NU_ANO, NU_CICLO y CICLO para tablas históricas
                    foreach (var col in columnNames)
                    {
                        if (!col.Equals("NU_ANO", StringComparison.OrdinalIgnoreCase) && 
                            !col.Equals("NU_CICLO", StringComparison.OrdinalIgnoreCase) &&
                            !col.Equals("CICLO", StringComparison.OrdinalIgnoreCase))
                        {
                            finalColumns.Add(col);
                        }
                    }
                    
                    // Procesar desde el segundo registro (el primero ya fue consumido por ReadAsync())
                    while (await reader.ReadAsync())
                    {
                        var valores = new List<string>();
                        foreach (var col in finalColumns)
                        {
                            valores.Add(FormatearValor(reader[col], reader.GetFieldType(reader.GetOrdinal(col)), col, cicloHistorico));
                        }
                        
                        // Generar INSERT sin comillas en el nombre de tabla (como ClickOnce)
                        string columnasStr = string.Join(", ", finalColumns);
                        string valoresStr = string.Join(", ", valores);
                        contenido.AppendLine($"INSERT INTO {tablaSQLite} ({columnasStr}) VALUES ({valoresStr});");
                    }
                }
            }
        }

        public Task<SyncResponse> ProcesarSincronizacionAsync(SyncRequest request)
        {
            return Task.FromResult(new SyncResponse { Success = false, Message = "No implementado" });
        }
    }
}