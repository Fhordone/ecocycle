using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Dispositivos.Models;
using System.Collections;
using System.Data.SqlClient;
using System.Numerics;

namespace Dispositivos
{
    public static class Function1
    {
        [FunctionName("GetDevice")]
        public static async Task<IActionResult> GetDevice(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "device")] HttpRequest req,
            ILogger log)
        {
            List<DeviceModel> taskList = new List<DeviceModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    /* Abre la conexion */
                    connection.Open();

                    /* Declara Query */
                    var query = @"Select TOP (1) DeviceId,State from Device_One order by DeviceId DESC";

                    /* Establece la estructura del comando(Sentencia +conexion)*/
                    SqlCommand command = new SqlCommand(query, connection);
                    var reader = await command.ExecuteReaderAsync();

                    /* Leer los resultados y crea lista de resultado por usuario */
                    while (reader.Read())
                    {
                        DeviceModel device = new DeviceModel()
                        {
                            DeviceId = new BigInteger((Int64)reader["DeviceId"]),
                            State = new BigInteger((Int64)reader["State"])
                           
                        };
                        taskList.Add(device);
                    }
                }
            }

            /* Si es que algo sale mal, se retorna un mensaje indicando el error */
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            /* Si todo sale bien, se carga la lista de usuarios*/
            if (taskList.Count > 0)
            {
                return new OkObjectResult(taskList);
            }
            else  /* Si no sale bien, no carga la lista de usuarios*/
            {
                return new NotFoundResult();
            }
        }
    }
}
