using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows;

namespace Ocene
{
    class Ucenik
    {
        public int? broj { get; set; }
        public int? brojReg { get; set; }
        public Int64? jmbg { get; set; }
        public string naziv { get; set; }
        public string mestoRodj { get; set; }
        public string opstinaRodj { get; set; }
        public string datumRodj { get; set; }
        public string otac { get; set; }
        public string majka { get; set; }
        public int? upisanU { get; set; }
        public int? redovan { get; set; }
        public Smer smer { get; set; }
        public int? trajanje { get; set; }
        public int? veronauka { get; set; }

        

        public static bool ValidateJmbg(string jmbg)
        {
            if (jmbg.Length != 13) return false;
            int s = 7 * (jmbg.Cifra(0) + jmbg.Cifra(6)) + 6 * (jmbg.Cifra(1) + jmbg.Cifra(7)) + 5 * (jmbg.Cifra(2) + jmbg.Cifra(8)) + 4 * (jmbg.Cifra(3) + jmbg.Cifra(9)) + 3 * (jmbg.Cifra(4) + jmbg.Cifra(10)) + 2 * (jmbg.Cifra(5) + jmbg.Cifra(11));            
            int m = s % 11;
            int k = 11 - m;
            if (m == 0) k = 0;
            if (k == 10) k = 0;
            return k == jmbg.Cifra(12);
        }

        public static bool ValidateBroj(string broj)
        {            
            int xx = 0;
            try { xx = Convert.ToInt32(broj); }
            catch {  return false; }

            if (broj.Length != 7) return false;           
            else
            {                                           
                var xxx = Ucenik.Daj().Where(x => x.broj == xx).ToList();
                if (xxx.Count > 0) return false;
            }
            return true;
        }

        public int? god_upisa
        {
            get
            {
                return broj % 100 + 2000;
            }
        }

        public int? odeljenje
        {
            get
            {
                return (broj / 100) % 100;
            }
        }

        public int? rbr
        {
            get
            {
                return broj / 100000;
            }
        }

        public int? razred
        {
            get
            {                
                return  upisanU + App.Godina() - god_upisa;
            }
        }

        public string sraz
        {
            get
            {
                if (razred > 4 || razred < 1) return "0. razred";
                return razred.ToString() + ". razred";
            }
        }

        public bool maturirao
        {
            get
            {
                return (upisanU + App.Godina() - god_upisa) > 4;
            }
        }

        public string sjmbg
        {
            get
            {
                string s = jmbg.ToString();
                if (s.Length == 12) s = "0" + s;
                return s;
            }
            set
            {
                jmbg = Convert.ToInt64(value);
            }
        }

        public string sbroj
        {
            get
            {
                string s = broj.ToString();
                if (s.Length == 6) s = "0" + s;
                return s;
            }
            set
            {
                broj = Convert.ToInt32(value);
            }
        }

        public string razod
        {
            get
            {
                if (razred > 4) return "4-" + odeljenje.ToString() + " - maturirali " + (god_upisa+4).ToString()+".";
                else if(razred < 1) return "1-" + odeljenje.ToString() + " - upisuju " + god_upisa.ToString() + ".";
                return razred.ToString() + "-" + odeljenje.ToString();
            }
        }
        
        public static double Prosek(Ucenik uc)
        {
            int suma = 0, del = 0;
            foreach (Predmet pr in Smer.DajPredmete(uc.smer, (int)uc.razred))
            {
                if(pr.prosek)
                {
                    int? ocena = Ucenik.OcenaIz((int)uc.broj, pr.id, App.Godina());                    
                    if (ocena != null)
                    {
                        suma += (int)ocena;                       
                        del++;
                    }
                }
            }
            if (del == 0) return 1;
            return (double)suma / del;
        }

        public static int? OcenaIz(int ucenik, int predmet, int godina)
        {
            ObservableCollection<Ucenik> ucenici = new ObservableCollection<Ucenik>();
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand(String.Format("SELECT * FROM ocene WHERE id_ucenika = {0} AND id_predmeta = {1} AND godina = {2}", ucenik, predmet, godina), con);
            SQLiteDataReader read = com.ExecuteReader();
            int? ocena = null;
            while (read.Read()) ocena = Convert.ToInt32(read["ocena"]);
            return ocena;
        }            

        public static void Dodaj(Ucenik uc)
        {
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand(String.Format("INSERT INTO ucenici VALUES ({0},{1},{2},'{3}','{4}','{13}','{5}','{6}','{7}',{8},{9},'{10}',{11},{12})",
                uc.broj, uc.brojReg, uc.jmbg, uc.naziv, uc.mestoRodj, uc.datumRodj, uc.otac, uc.majka, uc.upisanU, uc.redovan, uc.smer.id, uc.trajanje, uc.veronauka, uc.opstinaRodj), con);           
            com.ExecuteNonQuery();
            con.Close();
            MessageBox.Show("Dodat učenik " + uc.naziv);
        }

        public static void Izmeni(Ucenik uc, int stariId)
        {
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand(String.Format("UPDATE ucenici SET broj={0}, broj_reg={1}, jmbg={2}, naziv='{3}', mesto_rodj='{4}', opstina_rodj='{13}', datum_rodj='{5}', otac='{6}', majka='{7}', upisan_u={8}, redovan={9}, id_smera='{10}',trajanje={11}, veronauka = {12} WHERE broj='"+stariId.ToString()+"'",
                uc.broj, uc.brojReg, uc.jmbg, uc.naziv, uc.mestoRodj, uc.datumRodj, uc.otac, uc.majka, uc.upisanU, uc.redovan, uc.smer.id, uc.trajanje, uc.veronauka, uc.opstinaRodj), con);
            com.ExecuteNonQuery();
            con.Close();
        }

        public static void Izbrisi(Ucenik uc)
        {
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand("DELETE FROM ucenici WHERE broj=" + uc.broj.ToString(), con);
            com.ExecuteNonQuery();
            com = new SQLiteCommand("DELETE FROM ocene WHERE id_ucenika=" + uc.broj.ToString(), con);
            com.ExecuteNonQuery();
            con.Close();
        }

        public static ObservableCollection<Ucenik> Daj()
        {
            ObservableCollection<Ucenik> ucenici = new ObservableCollection<Ucenik>();
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand("SELECT * FROM ucenici", con);
            SQLiteDataReader read = com.ExecuteReader();
            while (read.Read())
            {
                ucenici.Add(new Ucenik() { broj = Convert.ToInt32(read["broj"]),
                brojReg = Convert.ToInt32(read["broj_reg"]),
                jmbg = Convert.ToInt64(read["jmbg"]),
                naziv = read["naziv"].ToString(),
                mestoRodj = read["mesto_rodj"].ToString(),
                opstinaRodj = read["opstina_rodj"].ToString(),
                datumRodj = read["datum_rodj"].ToString(),
                otac = read["otac"].ToString(),
                majka = read["majka"].ToString(),
                upisanU = Convert.ToInt32(read["upisan_u"]),
                redovan = Convert.ToInt32(read["redovan"]),
                smer = Smer.Daj().Where(x => x.id == Convert.ToInt32(read["id_smera"])).ToList()[0],
                trajanje = Convert.ToInt32(read["trajanje"]),
                veronauka = Convert.ToInt32(read["veronauka"])
                });                
            }
            
            con.Close();
            return ucenici;
        }

        public static ObservableCollection<Ucenik> Daj(string query)
        {
            ObservableCollection<Ucenik> ucenici = new ObservableCollection<Ucenik>();
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            SQLiteCommand com = new SQLiteCommand("SELECT * FROM ucenici WHERE "+query, con);
            SQLiteDataReader read = com.ExecuteReader();
            while (read.Read())
            {
                ucenici.Add(new Ucenik()
                {
                    broj = Convert.ToInt32(read["broj"]),
                    brojReg = Convert.ToInt32(read["broj_reg"]),
                    jmbg = Convert.ToInt64(read["jmbg"]),
                    naziv = read["naziv"].ToString(),
                    mestoRodj = read["mesto_rodj"].ToString(),
                    datumRodj = read["datum_rodj"].ToString(),
                    otac = read["otac"].ToString(),
                    majka = read["majka"].ToString(),
                    upisanU = Convert.ToInt32(read["upisan_u"]),
                    redovan = Convert.ToInt32(read["redovan"]),
                    smer = Smer.Daj().Where(x => x.id == Convert.ToInt32(read["id_smera"])).ToList()[0],
                    trajanje = Convert.ToInt32(read["trajanje"])
                });
            }
            con.Close();
            return ucenici;
        }

        public static List<Polje> DajPolja(bool znacajna)
        {
            List<Polje> p = new List<Polje>();
            p.Add(new Polje("Broj", "sbroj"));
            p.Add(new Polje("Registarski broj", "brojReg"));
            p.Add(new Polje("JMBG", "sjmbg"));
            p.Add(new Polje("Prezime i ime", "naziv"));
            if(!znacajna)
            {
                p.Add(new Polje("Mesto rodjenja", "mesto_rodj"));
                p.Add(new Polje("Datum rodjenja", "datum_rodj"));
                p.Add(new Polje("Prezime i ime oca", "otac"));
                p.Add(new Polje("Prezime i ime majke", "majka"));
                p.Add(new Polje("Upisan u (razred)", "upisanU"));
                p.Add(new Polje("Redovan/vandredan", "redovan"));
                p.Add(new Polje("Smer/profil", "smer"));
                p.Add(new Polje("Trajanje skolovanja", "trajanje"));
            }
            return p;
        }

        public static List<int> DajRazrede()
        {
            HashSet<int> razredi = new HashSet<int>();
            foreach (Ucenik u in Ucenik.Daj()) razredi.Add((int)u.razred);
            var lista = razredi.ToList();
            lista.Sort();
            return lista;
        }

        public static List<int> DajOdeljenja(int razred)
        {
            HashSet<int> od = new HashSet<int>();
            foreach (Ucenik u in Ucenik.Daj().Where(x => x.razred == razred)) od.Add((int)u.odeljenje);
            var lista = od.ToList();
            lista.Sort();
            return lista;
        }

    }
}
