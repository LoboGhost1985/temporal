using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace sabuesomicro
{
    class Program
    {
        static void Main(string[] args)
        {
            string json = @"";
            String LogType = "";
            var cliente = "";
            var equipo = "";
            var proceso = "";
            string[] separators = { "|" };
            string connectionString = string.Empty;
            connectionString = "DefaultEndpointsProtocol=https;EndpointSuffix=core.windows.net;AccountName=ciberit;AccountKey=ih344y6sZQaf/+TEF69agsXA7lmntpiyvcuixVA+m73DpMD8aKvoQJMJeRkDFKTTte30+7wmD7+x+AStuVuAOA==";
            try
            {
                string containerName = "archivossabueso";
                string dirName = "CustomLogs";

                BlobContainerClient blobContainerClient = new BlobContainerClient(connectionString, containerName);
                blobContainerClient.CreateIfNotExists();
                Console.WriteLine("Listing blobs...");
                // List all blobs in the container
                var blobs = blobContainerClient.GetBlobs();

                //ShareClient share = new ShareClient(connectionString, shareName);
                //share.CreateIfNotExistsAsync();
                //ShareDirectoryClient directory = share.GetDirectoryClient(dirName);
                //directory.CreateIfNotExistsAsync();
                foreach (var files in (blobs))
                {
                    //ShareFileClient file = files.GetFileClient(files.Name);
                    equipo = files.Name;
                    if (equipo == "Parametros.txt")
                    {
                        goto BreakLoops;
                    }
                    proceso = DateTime.UtcNow.ToString();
                    try
                    {
                        if (!Directory.Exists(@"c:\temp\"))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(@"c:\temp\");
                        }
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    //ShareFileDownloadInfo download = file.Download();
                    string contenido = string.Empty;
                    using (FileStream stream = File.OpenWrite(@"c:\temp\" + equipo))
                    {
                        BlobClient blobClient = blobContainerClient.GetBlobClient(files.Name);
                        //string downloadFilePath = localFilePath.Replace(".txt", "Parametros.txt");

                        blobClient.DownloadTo(stream);
                        //if (blobClient.Exists())
                        //{
                        //    BlobDownloadInfo download = blobClient.Download(contenido,);
                        //    byte[] result = new byte[download.ContentLength];
                        //    download.Content.Read(result, 0, (int)download.ContentLength);

                        //    contenido= Encoding.UTF8.GetString(result);
                        //}
                        //download.Content.CopyTo(stream);
                    }               
                        using (var streamReader = File.OpenText(@"c:\temp\" + equipo))
                        {                         
                         var lines = streamReader.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        foreach (string line in lines)                          
                        {
                           Console.WriteLine(line);
                            if (!string.IsNullOrEmpty(line))
                            {
                                string[] registros = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                switch (registros[0])
                                {
                                    case "Cliente":
                                        cliente = "ACEVEDO";
                                        LogType = "SabuesoT_Cliente";
                                        if (!(registros.Length > 3))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo.Replace(".txt","") + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""Revision"":""" + (registros.Length > 2 ? registros[2] : "1.0.0") + @"""";
                                        json = json + @",""Version"":""" + (registros.Length > 2 ? registros[3] : "V1") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "OperatingSystem":
                                        LogType = "SabuesoT_OperatingSystem";
                                        if (!(registros.Length > 4))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""Architecture"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""Caption"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""SystemDrive"":""" + (registros.Length > 2 ? registros[4] : "") + @"""";
                                        json = json + @",""Version"":""" + (registros.Length > 2 ? registros[5] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "PhysicalMemory":

                                        LogType = "SabuesoT_PhysicalMemory";
                                        if (!(registros.Length > 1))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""Capacity"":""" + (registros.Length > 1 ? registros[1] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "LogicalDisk":
                                        LogType = "SabuesoT_LogicalDisk";
                                        if (!(registros.Length > 2))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""FreeSpace"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""TotalSpace"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "Processor":
                                        LogType = "SabuesoT_Processor";
                                        if (!(registros.Length > 2))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""procesador"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""Manufacturer"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "BIOS":
                                        LogType = "SabuesoT_BIOS";
                                        if (!(registros.Length > 2))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""Serial"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""Manufacturer"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "APLICACION":
                                        LogType = "SabuesoT_Aplicacion";
                                        if (!(registros.Length > 4))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""DisplayName"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""DisplayVersion"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""InstallDate"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""Publisher"":""" + (registros.Length > 2 ? registros[4] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "UPDATEH":
                                        LogType = "SabuesoT_UPDATEH";
                                        if (!(registros.Length > 2))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""Title"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""UpdateID"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "wsusequipo":
                                        LogType = "SabuesoT_wsusequipo";
                                        if (!(registros.Length > 3))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""clientew"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""UpdateID"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""estado"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "WSUSUPDATE":
                                        LogType = "SabuesoT_WSUSUPDATE";
                                        if (!(registros.Length > 6))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""Title"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""tipo"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""estado"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""id"":""" + (registros.Length > 2 ? registros[4] : "") + @"""";
                                        json = json + @",""Reemplazado"":""" + (registros.Length > 2 ? registros[5] : "") + @"""";
                                        json = json + @",""kb"":""" + (registros.Length > 2 ? registros[6] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "service":
                                        LogType = "SabuesoT_service";
                                        if (!(registros.Length > 2))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""servicio"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""estado"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "CEstado":
                                        LogType = "SabuesoT_CEstado";
                                        if (!(registros.Length > 2))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""tipo"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""estado"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "BROWSER":
                                        LogType = "SabuesoT_BROWSER";
                                        if (!(registros.Length > 4))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""browser"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""user"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""dataType"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""data"":""" + (registros.Length > 2 ? registros[4] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "TRAFICORED":
                                        LogType = "SabuesoT_TRAFICORED";
                                        if (!(registros.Length > 5))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""nombreinterfaz"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""bytesrecibidos"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""paquetesrecibidos"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""bytesenviados"":""" + (registros.Length > 2 ? registros[4] : "") + @"""";
                                        json = json + @",""paquetesenviados"":""" + (registros.Length > 2 ? registros[5] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "UNINSTALLEVENT":
                                        LogType = "SabuesoT_UNINSTALLEVENT";
                                        if (!(registros.Length > 4))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""descripcion"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""fecha"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""tipo"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""origen"":""" + (registros.Length > 2 ? registros[4] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "ApplicationResourceD":
                                        LogType = "SabuesoT_ApplicationResource";
                                        if (!(registros.Length > 20))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""AutoIncId"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""TimeStamp"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""AppId"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""UserId"":""" + (registros.Length > 2 ? registros[4] : "") + @"""";
                                        json = json + @",""ForegroundCycleTime"":""" + (registros.Length > 2 ? registros[5] : "") + @"""";
                                        json = json + @",""BackgroundCycleTime"":""" + (registros.Length > 2 ? registros[6] : "") + @"""";
                                        json = json + @",""FaceTime"":""" + (registros.Length > 2 ? registros[7] : "") + @"""";
                                        json = json + @",""ForegroundContextSwitches"":""" + (registros.Length > 2 ? registros[8] : "") + @"""";
                                        json = json + @",""BackgroundContextSwitches"":""" + (registros.Length > 2 ? registros[9] : "") + @"""";
                                        json = json + @",""ForegroundBytesRead"":""" + (registros.Length > 2 ? registros[10] : "") + @"""";
                                        json = json + @",""ForegroundBytesWritten"":""" + (registros.Length > 2 ? registros[11] : "") + @"""";
                                        json = json + @",""ForegroundNumReadOperations"":""" + (registros.Length > 2 ? registros[12] : "") + @"""";
                                        json = json + @",""ForegroundNumWriteOperations"":""" + (registros.Length > 2 ? registros[13] : "") + @"""";
                                        json = json + @",""ForegroundNumberOfFlushes"":""" + (registros.Length > 2 ? registros[14] : "") + @"""";
                                        json = json + @",""BackgroundBytesRead"":""" + (registros.Length > 2 ? registros[15] : "") + @"""";
                                        json = json + @",""BackgroundBytesWritten"":""" + (registros.Length > 2 ? registros[16] : "") + @"""";
                                        json = json + @",""BackgroundNumReadOperations"":""" + (registros.Length > 2 ? registros[17] : "") + @"""";
                                        json = json + @",""BackgroundNumWriteOperations"":""" + (registros.Length > 2 ? registros[18] : "") + @"""";
                                        json = json + @",""BackgroundNumberOfFlushes"":""" + (registros.Length > 2 ? registros[19] : "") + @"""";
                                        json = json + @",""convertido"":""" + (registros.Length > 2 ? registros[20] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "NetworkConnectivityD":
                                        LogType = "SabuesoT_NetworkConnectivity";
                                        if (!(registros.Length > 10))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""AutoIncId"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""TimeStamp"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""AppId"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""UserId"":""" + (registros.Length > 2 ? registros[4] : "") + @"""";
                                        json = json + @",""InterfaceLuid"":""" + (registros.Length > 2 ? registros[5] : "") + @"""";
                                        json = json + @",""L2ProfileId"":""" + (registros.Length > 2 ? registros[6] : "") + @"""";
                                        json = json + @",""ConnectedTime"":""" + (registros.Length > 2 ? registros[7] : "") + @"""";
                                        json = json + @",""ConnectStartTime"":""" + (registros.Length > 2 ? registros[8] : "") + @"""";
                                        json = json + @",""L2ProfileFlags"":""" + (registros.Length > 2 ? registros[9] : "") + @"""";
                                        json = json + @",""convertido"":""" + (registros.Length > 2 ? registros[10] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "Perfil":
                                        LogType = "SabuesoT_Perfil";
                                        if (!(registros.Length > 1))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""perfil"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "Usuariolocal":
                                        LogType = "SabuesoT_Usuariolocal";
                                        if (!(registros.Length > 3))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""Usuario"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""login"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""logoff"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "DispositivosUSB":
                                        LogType = "SabuesoT_DispositivosUSB";
                                        if (!(registros.Length > 3))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""DeviceID"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""PNPDeviceID"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""Description"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "perfilwifi":
                                        LogType = "SabuesoT_perfilwifi";
                                        if (!(registros.Length > 1))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""perfil"":""" + (registros.Length > 1 ? registros[1] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "registroeventos":
                                        LogType = "SabuesoT_registroeventos";
                                        if (!(registros.Length > 6))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""descripcion"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""fecha"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""tipo"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""origen"":""" + (registros.Length > 2 ? registros[4] : "") + @"""";
                                        json = json + @",""tipoo"":""" + (registros.Length > 2 ? registros[5] : "") + @"""";
                                        json = json + @",""evento"":""" + (registros.Length > 2 ? registros[6] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "Sharefolder":
                                        LogType = "SabuesoT_Sharefolder";
                                        if (!(registros.Length > 10))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""caption"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""Description"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""InstallDate"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""Status"":""" + (registros.Length > 2 ? registros[4] : "") + @"""";
                                        json = json + @",""AccessMask"":""" + (registros.Length > 2 ? registros[5] : "") + @"""";
                                        json = json + @",""AllowMaximum"":""" + (registros.Length > 2 ? registros[6] : "") + @"""";
                                        json = json + @",""MaximumAllowed"":""" + (registros.Length > 2 ? registros[7] : "") + @"""";
                                        json = json + @",""Name"":""" + (registros.Length > 2 ? registros[8] : "") + @"""";
                                        json = json + @",""Path"":""" + (registros.Length > 2 ? registros[9] : "") + @"""";
                                        json = json + @",""tipo"":""" + (registros.Length > 2 ? registros[10] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "Sharefoldersecurity":
                                        LogType = "SabuesoT_Sharefoldersecurity";
                                        if (!(registros.Length > 4))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""Name"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""Path"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""IdentityReference"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""tiposeg"":""" + (registros.Length > 2 ? registros[4] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "Sharefolderpermissions":
                                        LogType = "SabuesoT_Sharefolderpermissions";
                                        if (!(registros.Length > 7))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""Name"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""caption"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""Domain"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""Trustee"":""" + (registros.Length > 2 ? registros[4] : "") + @"""";
                                        json = json + @",""AceFlags"":""" + (registros.Length > 2 ? registros[5] : "") + @"""";
                                        json = json + @",""AccessMask"":""" + (registros.Length > 2 ? registros[6] : "") + @"""";
                                        json = json + @",""AceType"":""" + (registros.Length > 2 ? registros[7] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "LogicalDiskD":
                                        LogType = "SabuesoT_LogicalDisk";
                                        if (!(registros.Length > 7))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""Caption"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""Description"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""DeviceID"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""FileSystem"":""" + (registros.Length > 2 ? registros[4] : "") + @"""";
                                        json = json + @",""tipodissk"":""" + (registros.Length > 2 ? registros[5] : "") + @"""";
                                        json = json + @",""Freespace"":""" + (registros.Length > 2 ? registros[6] : "") + @"""";
                                        json = json + @",""Size"":""" + (registros.Length > 2 ? registros[7] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "Error":
                                        LogType = "SabuesoT_Error";
                                        if (!(registros.Length > 2))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""Tipo"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""mensaje"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "userprofiles":
                                        LogType = "SabuesoT_userprofiles";
                                        if (!(registros.Length > 1))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""Perfil"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "Zhoraria":
                                        LogType = "SabuesoT_Zhoraria";
                                        if (!(registros.Length > 5))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""zonaId"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""nombreZona"":""" + (registros.Length > 2 ? registros[2] : "") + @"""";
                                        json = json + @",""nombreStandar"":""" + (registros.Length > 2 ? registros[3] : "") + @"""";
                                        json = json + @",""zonaHorariaVerano"":""" + (registros.Length > 2 ? registros[4] : "") + @"""";
                                        json = json + @",""horaEquipo"":""" + (registros.Length > 2 ? registros[5] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                    case "KBZonaHoraria":
                                        LogType = "SabuesoT_KBZonaHoraria";
                                        if (!(registros.Length > 1))
                                        {
                                            break;
                                        }
                                        json = @"[{""Computer"":""" + equipo + @"""";
                                        json = json + @",""Cliente"":""" + cliente + @"""";
                                        json = json + @",""KBZonaHoraria"":""" + (registros.Length > 2 ? registros[1] : "") + @"""";
                                        json = json + @",""Proceso"":""" + proceso + @"""";
                                        json = json + "}]";
                                        m_cargarAsync(json, LogType);
                                        break;
                                }
                            }
                        }
                    }
                    File.Delete(@"c:\temp\" + equipo);
                    //file.DeleteIfExistsAsync();
                    BreakLoops:;

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            async Task<string> m_cargarAsync(string json, string LogType)
            {
                String WorkspaceId = "28689776-2d54-4965-ab35-20b592b7a278";
                String SharedKey = "0h7ZRbpzGaSpnFxURA9oXTTe9nFXe7pXPvXQmENm6STCLuqAm2jEAqrbfG4F/FkDiv1SGCZ/r8AF0mov3Zr+BA==";
                var encoding = new System.Text.ASCIIEncoding();
                var date = DateTime.UtcNow.ToString("r");
                var jsonBytes = Encoding.UTF8.GetBytes(json);
                string stringToHash = "POST\n" + jsonBytes.Length + "\napplication/json\n" + "x-ms-date:" + date + "\n/api/logs";
                byte[] messageBytes = encoding.GetBytes(stringToHash);
                messageBytes = encoding.GetBytes(stringToHash);
                string signature = "";
                byte[] keyByte = Convert.FromBase64String(SharedKey);
                string url = "https://" + WorkspaceId + ".ods.opinsights.azure.com/api/logs?api-version=2016-04-01";
                using (var hmacsha256 = new HMACSHA256(keyByte))
                {
                    byte[] hash = hmacsha256.ComputeHash(messageBytes);
                    string hashedString = Convert.ToBase64String(hash);
                    signature = "SharedKey " + WorkspaceId + ":" + hashedString;
                }
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Log-Type", LogType);
                client.DefaultRequestHeaders.Add("Authorization", signature);
                client.DefaultRequestHeaders.Add("x-ms-date", date);
                client.DefaultRequestHeaders.Add("time-generated-field", date);
                System.Net.Http.HttpContent httpContent = new StringContent(json, Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                //Task<System.Net.Http.HttpResponseMessage> response = await client.PostAsync(new Uri(url), httpContent);
                var response = await client.PostAsync(new Uri(url), httpContent);
                System.Net.Http.HttpContent responseContent = response.Content;//.Result.Content;
                string result = responseContent.ReadAsStringAsync().Result;
                return result;
            }
            DateTime ValidarFechas(string date)
            {
                DateTime dt = new DateTime();
                string dia = string.Empty, mes = string.Empty, anio = string.Empty, hora = string.Empty;
                Regex co2 = new Regex(@"\d{1,2}(\/|-)\d{1,2}(\/|-)\d{2,4}", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
                Match m2 = co2.Match(date);
                if (m2.Success)
                {
                    string[] rep = m2.Value.Split('-');
                    if (rep.Length > 1)
                    {
                        dia = int.Parse(rep[1]) > 12 ? rep[1] : rep[0];
                        mes = int.Parse(rep[1]) > 12 ? rep[0] : rep[1];
                        anio = rep[2];
                    }
                    else
                    {
                        rep = m2.Value.Split('/');
                        if (rep.Length > 1)
                        {
                            dia = int.Parse(rep[1]) > 12 ? rep[1] : rep[0];
                            mes = int.Parse(rep[1]) > 12 ? rep[0] : rep[1];
                            anio = rep[2];
                        }
                    }
                    hora = date.Replace(m2.Value, "");
                    dt = DateTime.Parse(dia + "-" + mes + "-" + anio + hora);
                }
                return dt;
            }
        }
    }
}
