using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace std
{
    public class TcpServer
    {
        // on crée le service d'écoute
        Int32 port;
        TcpListener ecoute = null;

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
        public static void Service(Object infos)
        {

            String demande = null;
            String reponse = null;
            // on récupère le client qu'il faut servir
            TcpClient client = infos as TcpClient;

            // exploitation liaison TcpClient
            try
            {
                String temp = null;
                Console.WriteLine("Client connected");
                using (NetworkStream networkStream = client.GetStream())
                {
                    using (StreamReader reader = new StreamReader(networkStream))
                    {
                        using (StreamWriter writer = new StreamWriter(networkStream))
                        {
                            // flux de sortie non bufferisé
                            writer.AutoFlush = true;
                            // boucle lecture demande/écriture réponse
                            bool fini = false;
                            while (demande != "exit")
                            {
                                demande = reader.ReadLine();

                                if (demande.ToLower().Equals("bonjour"))
                                {
                                    reponse = "Bonjour, t'es qui ?";
                                    temp = demande;
                                    writer.WriteLine(reponse);
                                }
                                else if (demande.ToLower().Equals("ça va et toi ?") && temp.ToLower().Equals("bonjour"))
                                {
                                    reponse = "ça va";
                                    temp = demande;
                                    writer.WriteLine(reponse);
                                }
                                else
                                {
                                    reponse = "Session non ouverte";
                                    temp = demande;
                                    writer.WriteLine(reponse);
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

