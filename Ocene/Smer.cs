using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Collections.ObjectModel;

namespace Ocene
{
    class Smer
    {
        public int id { get; set; }
        public string naziv { get; set; }

        public static ObservableCollection<Predmet> DajPredmete(Smer sm, int razred)
        {
            ObservableCollection<Predmet> predmeti = new ObservableCollection<Predmet>();
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            ObservableCollection<Predmet> svi = Predmet.Daj();
            SQLiteCommand com = new SQLiteCommand("SELECT * FROM smer_predmet WHERE id_smera=" + sm.id.ToString() + " AND razred=" + razred.ToString(), con);
            SQLiteDataReader read = com.ExecuteReader();
            while (read.Read())
            {
                Predmet pr = new Predmet();
                var col = svi.Where(x => x.id == Convert.ToInt32(read["id_predmeta"])).ToList();
                if (col.Count != 0)
                {
                    pr = col[col.Count - 1];
                    pr.prosek = (read["prosek"].ToString() == "1");
                    predmeti.Add(pr);
                }
            }
            return predmeti;
        }

        public static void SacuvajListuPredmeta(ObservableCollection<Predmet> lista, Smer sm, int razred)
        {
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand("DELETE FROM smer_predmet WHERE id_smera=" + sm.id.ToString() + " AND razred="+razred.ToString(), con);
            com.ExecuteNonQuery();
            int i = 1;
            foreach(Predmet p in lista)
            {
                int pros = 0;
                if (p.prosek) pros = 1;
                com = new SQLiteCommand(String.Format("INSERT INTO smer_predmet VALUES ({0}, {1}, {2}, {3}, {4})", i, sm.id, p.id, razred, pros), con);
                com.ExecuteNonQuery();
                i++;
            }
            con.Close();
        }


        public static void Dodaj(Smer pr)
        {
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand("INSERT INTO smerovi VALUES (Null, '" + pr.naziv + "')", con);
            com.ExecuteNonQuery();
            con.Close();
        }

        public static void Izmeni(Smer pr)
        {
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand("UPDATE smerovi SET naziv='" + pr.naziv + "' WHERE id='" + pr.id + "'", con);
            com.ExecuteNonQuery();
            con.Close();
        }

        public static void Izbrisi(Smer pr)
        {
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand("DELETE FROM smerovi WHERE id='" + pr.id.ToString() + "'", con);
            com.ExecuteNonQuery();
            con.Close();
        }

        public static ObservableCollection<Smer> Daj()
        {
            ObservableCollection<Smer> smerovi = new ObservableCollection<Smer>();
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand("SELECT * FROM smerovi", con);
            SQLiteDataReader read = com.ExecuteReader();
            while (read.Read())
            {
                smerovi.Add(new Smer() { id = Convert.ToInt32(read["id"]), naziv = read["naziv"].ToString() });
            }
            con.Close();
            return smerovi;
        }
    }
}
