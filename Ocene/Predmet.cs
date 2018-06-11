using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Collections.ObjectModel;

namespace Ocene
{
    public class Predmet
    {        
        public int id { get; set; }        
        public string naziv { get; set; }
        public int tip { get; set; }
        public bool prosek { get; set; }

        public static void Dodaj(Predmet pr)
        {
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand("INSERT INTO predmeti VALUES (Null, '"+pr.naziv+"', "+pr.tip+")", con);
            com.ExecuteNonQuery();
            con.Close();
        }

        public static void Izmeni(Predmet pr)
        {
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand("UPDATE predmeti SET naziv='"+pr.naziv+"', tip="+pr.tip+" WHERE id='"+pr.id+"'", con);
            com.ExecuteNonQuery();
            con.Close();
        }

        public static void Izbrisi(Predmet pr)
        {
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand("DELETE FROM predmeti WHERE id='"+pr.id.ToString()+"'", con);
            com.ExecuteNonQuery();
            con.Close();
        }

        public static ObservableCollection<Predmet> Daj()
        {
            ObservableCollection<Predmet> predmeti = new ObservableCollection<Predmet>();
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand("SELECT * FROM predmeti", con);
            SQLiteDataReader read = com.ExecuteReader();
            while (read.Read())
            {
                predmeti.Add(new Predmet() { id = Convert.ToInt32(read["id"]), naziv = read["naziv"].ToString(), tip = Convert.ToInt32(read["tip"]) });
            }
            con.Close();            
            return predmeti;
        }

        public static ObservableCollection<Predmet> Daj(string query)
        {
            ObservableCollection<Predmet> predmeti = new ObservableCollection<Predmet>();
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand("SELECT * FROM predmeti WHERE "+query, con);
            SQLiteDataReader read = com.ExecuteReader();
            while (read.Read())
            {
                predmeti.Add(new Predmet() { id = Convert.ToInt32(read["id"]), naziv = read["naziv"].ToString(), tip = Convert.ToInt32(read["tip"]) });
            }
            con.Close();
            return predmeti;
        }
    }
}
