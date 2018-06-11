using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows;

namespace Ocene
{
    public class Podesavanje
    {
        public int? id { get; set; }
        public string naziv { get; set; }
        public string vrednost { get; set; }

        public static void  Sacuvaj(List<Podesavanje> lista)
        {
            SQLiteConnection con = new SQLiteConnection("Data Source = baza.sqlite");
            con.Open();
            SQLiteCommand com;
            foreach (Podesavanje pod in lista)
            {
                com = new SQLiteCommand("UPDATE podesavanja SET vrednost = '" + pod.vrednost + "' WHERE id="+pod.id.ToString(), con);
                com.ExecuteNonQuery();
            }
            con.Close();
        }
        
        public static string Vred(string naziv)
        {
            try
            { return Podesavanje.Daj().Where(x => x.naziv == naziv).ToList()[0].vrednost; }
            catch { return "Greska"; }
            
        }

        public static List<Podesavanje> Daj()
        {
            List<Podesavanje> lista = new List<Podesavanje>();
            SQLiteConnection con = new SQLiteConnection("Data Source = baza.sqlite");
            con.Open();

            SQLiteCommand com = new SQLiteCommand("SELECT * FROM podesavanja", con);
            SQLiteDataReader read = com.ExecuteReader();
            while(read.Read())
            {
                lista.Add(new Podesavanje()
                {
                    id = Convert.ToInt32(read["id"]),
                    naziv = read["naziv"].ToString(),
                    vrednost = read["vrednost"].ToString()
                });
            }
            con.Close();            
            return lista;
        }
    }
}
