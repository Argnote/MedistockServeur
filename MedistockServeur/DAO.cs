using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace MedistockServeur
{
    class DAO
    {
        string message;
        string retour;
        private List<string> profil;
        private MySqlConnection MyConnection;
        private string connexion_string = "SERVER=127.0.0.1; DATABASE=medistock; UID=root; PASSWORD=";       // Création d'un string permettant d'ouvrir la dB avec des parametres prédéfinis 

        public DAO()
        {
            try
            {
                MyConnection = new MySqlConnection(connexion_string);
                MyConnection.Open();
                //Console.WriteLine("connection établie\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public string connection(string p_identifiant)
        {
            int indexY = 0;
            profil = new List<string>();
            MySqlCommand request = new MySqlCommand("Select Nom,Prenom,Fonction,Permission,Identifiant,MotDePasse from personnel,permission WHERE personnel.IdPermission = permission.IdPermission", MyConnection);
            MySqlDataReader reader = request.ExecuteReader();
            string list = null;
            while (reader.Read()) // Pour chaque enregistrement de l'objet ciblé 
            {
                for (indexY = 0; indexY < reader.FieldCount; indexY++) // Pour chaque attribut de l'enregistrement ciblé
                {
                    list = list + reader[indexY].ToString() + ';';
                    //profil.Add(reader[indexY].ToString() + ","); // Stockage dans la liste "dataList" du tout le contenu de la base de données en String
                }
                list = list.Substring(0, list.Length - 1);
                profil.Add(list); // Ajout des séparations entre chaque enregistrement
                list = null;
                indexY = 0; // Remise à zéro de l'index 
            }
            reader.Close(); // Fermeture du DataReader
            MyConnection.Close(); // Fermeture de la connexion
            //profil.Add("Croix;Pierre;medecin;2;test;azerty");
            //profil.Add("Dubois;Victor;magasinier;5;admin;azerty");
            foreach (string element in profil)
            {
                string[] split = element.Split(';');
                string identification = split[4] + ';' + split[5];
                if(p_identifiant == identification)
                {
                    message = element;
                    return message;
                }
            }
            message = "faux";
            return message;
        }
        public string getmessage()
        {
            return message;
        }
        public void setRetour(string p_retour)
        {
            retour = p_retour;
        }
    }
}
