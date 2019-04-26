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
        private List<string> profil;
        private List<string> medicament;
        private List<string> medicamentAction;
        private MySqlConnection MyConnection;
        private string connexion_string = "SERVER=127.0.0.1; DATABASE=medistock; UID=root; PASSWORD=pass";       // Création d'un string permettant d'ouvrir la dB avec des parametres prédéfinis 

        public DAO()
        {
            try
            {
                MyConnection = new MySqlConnection(connexion_string);
                MyConnection.Open();
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
            MySqlCommand request = new MySqlCommand("Select IdPersonnel,Nom,Prenom,Fonction,Permission,Identifiant,MotDePasse from personnel,permission WHERE personnel.IdPermission = permission.IdPermission", MyConnection);
            MySqlDataReader reader = request.ExecuteReader();
            string enregistrement = null;
            while (reader.Read()) // Pour chaque enregistrement de l'objet ciblé 
            {
                for (indexY = 0; indexY < reader.FieldCount; indexY++) // Pour chaque attribut de l'enregistrement ciblé
                {
                    enregistrement = enregistrement + reader[indexY].ToString() + ';';
                    //profil.Add(reader[indexY].ToString() + ","); // Stockage dans la liste "dataList" du tout le contenu de la base de données en String
                }
                enregistrement = enregistrement.Substring(0, enregistrement.Length - 1);
                profil.Add(enregistrement); // Ajout des séparations entre chaque enregistrement
                enregistrement = null;
                indexY = 0; // Remise à zéro de l'index 
            }
            reader.Close(); // Fermeture du DataReader

            //profil.Add("Croix;Pierre;medecin;2;test;azerty");
            //profil.Add("Dubois;Victor;magasinier;5;admin;azerty");
            foreach (string element in profil)
            {
                string[] split = element.Split(';');
                string identification = split[5] + ';' + split[6];
                if (p_identifiant == identification)
                {
                    message = element + '*' + listeMedicament() + "/" + listeArmoire();
                    MyConnection.Close(); // Fermeture de la connexion
                    return message;
                }
            }
            MyConnection.Close();
            message = "faux";
            return message;
        }

        private string listeMedicament()
        {
            string liste = null;

            int indexY = 0;
            medicament = new List<string>();
            medicamentAction = new List<string>();
            MySqlCommand request = new MySqlCommand("Select medicament.IdMedicament,medicament.Nom,medicament.Type,medicament.TypeMesure,medicament.PrincipeActif,medicament.Stock,medicament.SeuilCritique,armoire.Abscisse,armoire.Ordonne,salle.nom from medicament,armoire,salle WHERE medicament.IdArmoire = armoire.IdArmoire AND armoire.IdSalle = salle.IdSalle ORDER BY medicament.IdMedicament", MyConnection);
            MySqlDataReader reader = request.ExecuteReader();
            string enregistrement = null;
            int IdMedicament = 0;
            while (reader.Read()) // Pour chaque enregistrement de l'objet ciblé 
            {
                for (indexY = 0; indexY < reader.FieldCount; indexY++) // Pour chaque attribut de l'enregistrement ciblé
                {
                    enregistrement = enregistrement + reader[indexY].ToString() + ',';
                    //profil.Add(reader[indexY].ToString() + ","); // Stockage dans la liste "dataList" du tout le contenu de la base de données en String
                }
                //list = list.Substring(0, list.Length - 1);
                medicament.Add(enregistrement); // Ajout des séparations entre chaque enregistrement
                enregistrement = null;
                indexY = 0; // Remise à zéro de l'index 
            }
            reader.Close(); // Fermeture du DataReader

            foreach (string medicament in medicament)
            {
                IdMedicament++;
                MySqlCommand requestAction = new MySqlCommand("Select TypeAction,Quantité FROM effectueraction Where IdMedicament = " + IdMedicament + "", MyConnection);
                MySqlDataReader readerAction = requestAction.ExecuteReader();
                while (readerAction.Read()) // Pour chaque enregistrement de l'objet ciblé 
                {
                    for (indexY = 0; indexY < readerAction.FieldCount; indexY++) // Pour chaque attribut de l'enregistrement ciblé
                    {
                        enregistrement = enregistrement + readerAction[indexY].ToString() + 'ù';
                        //profil.Add(reader[indexY].ToString() + ","); // Stockage dans la liste "dataList" du tout le contenu de la base de données en String
                    }
                    enregistrement = enregistrement.Substring(0, enregistrement.Length - 1);
                    enregistrement = enregistrement + ",";
                    indexY = 0;
                }

                readerAction.Close();
                enregistrement = medicament + enregistrement;
                enregistrement = enregistrement.Substring(0, enregistrement.Length - 1);
                medicamentAction.Add(enregistrement);
                enregistrement = null;

                //profil[IdMedicament - 1] = profil[IdMedicament - 1].Substring(0, list.Length - 1);
            }

            foreach (string medicament in medicamentAction)
            {
                liste = liste + medicament + '*';
            }
            liste = liste.Substring(0, liste.Length - 1);
            return liste;
        }

        private string listeArmoire()
        {
            int indexY = 0;
            MySqlCommand request = new MySqlCommand("Select armoire.IdArmoire,armoire.Abscisse,armoire.Ordonne,salle.Nom from armoire, salle WHERE armoire.IdSalle = salle.IdSalle ORDER BY armoire.IdSalle", MyConnection);
            MySqlDataReader reader = request.ExecuteReader();
            string enregistrement = null;
            while (reader.Read()) // Pour chaque enregistrement de l'objet ciblé 
            {
                for (indexY = 0; indexY < reader.FieldCount; indexY++) // Pour chaque attribut de l'enregistrement ciblé
                {
                    enregistrement = enregistrement + reader[indexY].ToString() + ',';
                    //profil.Add(reader[indexY].ToString() + ","); // Stockage dans la liste "dataList" du tout le contenu de la base de données en String
                }
                enregistrement = enregistrement.Substring(0, enregistrement.Length - 1);
                enregistrement = enregistrement + "/";
                indexY = 0; // Remise à zéro de l'index 
            }
            enregistrement = enregistrement.Substring(0, enregistrement.Length - 1);
            reader.Close(); // Fermeture du DataReader 
            return enregistrement;
        }
        public string getmessage()
        {
            return message;
        }
        public void ajoutAction(string demande)
        {
            try
            {
                string[] splitmessage = demande.Split(',');
                MyConnection = new MySqlConnection(connexion_string);
                MyConnection.Open();
                MySqlCommand cmd = MyConnection.CreateCommand();
                MySqlCommand request = new MySqlCommand("Select*FROM effectueraction", MyConnection);
                MySqlDataReader reader = request.ExecuteReader();
                int id = 0;
                while (reader.Read())
                {
                    id++;
                }
                id++;
                reader.Close();
                string sql = "Insert into effectueraction values (" + id + "," + splitmessage[0] + "," + splitmessage[1] + ",'" + splitmessage[2] + "','" + splitmessage[3] + "'," + splitmessage[4] + ")";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                MyConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        public void ajoutMedicament(string demande)
        {
            try
            {
                string[] splitmessage = demande.Split(',');
                MyConnection = new MySqlConnection(connexion_string);
                MyConnection.Open();
                MySqlCommand cmd = MyConnection.CreateCommand();
                MySqlCommand request = new MySqlCommand("Select*FROM medicament", MyConnection);
                MySqlDataReader reader = request.ExecuteReader();
                int id = 0;
                while (reader.Read())
                {
                    id++;
                }
                id++;
                reader.Close();
                string sql = "Insert into medicament values (" + id + ",'" + splitmessage[1] + "','" + splitmessage[2] + "','" + splitmessage[3] + "','" + splitmessage[4] + "'," + splitmessage[5] + "," + splitmessage[6] + "," + splitmessage[7] + ")";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                MyConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        public void modificationMedicament(string demande)
        {
            try
            {
                string[] splitmessage = demande.Split(',');
                MyConnection = new MySqlConnection(connexion_string);
                MyConnection.Open();
                MySqlCommand cmd = MyConnection.CreateCommand();
                string sql = "UPDATE medicament SET Nom = '" + splitmessage[1] + "', Type = '" + splitmessage[2] + "', TypeMesure = '" + splitmessage[3] + "',PrincipeActif = '" + splitmessage[4] + "', SeuilCritique = " + splitmessage[5] + ",IdArmoire = " + splitmessage[6] + " WHERE IdMedicament = " + splitmessage[0];
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                MyConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
