using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MedistockServeur
{
    public class TcpServer
    {
        // on crée le service d'écoute
        Int32 port;
        TcpListener ecoute = null;
        DAO dao;
        public TcpServer(Int32 p_port)
        {
            port = p_port;
            try
            {
                // on crée le service - il écoutera sur toutes les interfaces réseau de la machine
                ecoute = new TcpListener(IPAddress.Any, port);
                // on le lance
                ecoute.Start();
                // boucle de service
                TcpClient tcpClient = null;
                // boucle infinie - sera arrêtée par Ctrl-C
                while (true)
                {
                    // attente d'un client
                    Console.WriteLine("En attente d'un client");
                    tcpClient = ecoute.AcceptTcpClient();
                    // le service est assuré par une autre tâche
                    ThreadPool.QueueUserWorkItem(Service, tcpClient);
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                ecoute.Stop();
            }
        }

        // -------------------------------------------------------
        // assure le service à un client
        public void Service(Object infos)
        {
            dao = new DAO();
            string demande = null;
            string reponse = null;
            int etape = 0;
            // on récupère le client qu'il faut servir
            TcpClient client = infos as TcpClient;
            // exploitation liaison TcpClient
            try
            {
                Console.WriteLine("Client connected");
                using (NetworkStream networkStream = client.GetStream())
                {
                    using (StreamReader reader = new StreamReader(networkStream))
                    {
                        using (StreamWriter writer = new StreamWriter(networkStream))
                        {
                            demande = reader.ReadLine();
                            etape = (int)Char.GetNumericValue(demande[0]);
                            demande = demande.Substring(1);
                            // flux de sortie non bufferisé
                            writer.AutoFlush = true;
                            // boucle lecture demande/écriture réponse
                            //reponse = dao.getmessage();
                            //writer.WriteLine(reponse);
                            while (true)
                            {
                                switch(etape)
                                {
                                    case 1:
                                        reponse = dao.connection(demande);
                                        writer.WriteLine(reponse);
                                        Console.WriteLine(reponse);
                                        etape = 0;
                                        break;

                                    case 2:
                                        dao.ajoutAction(demande);
                                        writer.WriteLine("effectuer");
                                        etape = 0;
                                        break;

                                    case 3:
                                        Console.WriteLine(demande);
                                        dao.ajoutMedicament(demande);
                                        writer.WriteLine("effectuer");
                                        etape = 0;
                                        break;

                                    case 4:
                                        dao.modificationMedicament(demande);
                                        writer.WriteLine("effectuer");
                                        etape = 0;
                                        break;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
            finally
            {
                // fin client

            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            TcpServer tcpServer = new TcpServer(22);


        }
    }

}

