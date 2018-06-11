using System;
using Microsoft.Office.Interop.Word;
using System.Diagnostics;
using WORD = Microsoft.Office.Interop.Word;
using System.Collections.Generic;
using System.Windows;

namespace Ocene
{
    public static class StringExtension
    {
        public static string Cir(this string lat)
        {
            string cir = lat;

            string[] latinica = { "A", "B", "V", "G", "D", "Đ", "E", "Dž", "Ž", "Z", "I", "K", "Lj", "L", "M", "Nj", "J", "N", "O", "P", "R", "S", "T", "Ć", "U", "F", "H", "C", "Č", "Š", "dž", "a", "b", "v", "g", "d", "đ", "e", "ž", "z", "i", "k", "lj", "l", "m", "nj", "j", "n", "o", "p", "r", "s", "t", "ć", "u", "f", "h", "c", "č", "š" };
            string[] cirilica = { "А", "Б", "В", "Г", "Д", "Ђ", "Е", "Џ", "Ж", "З", "И", "К", "Љ", "Л", "М", "Њ", "Ј", "Н", "О", "П", "Р", "С", "Т", "Ћ", "У", "Ф", "Х", "Ц", "Ч", "Ш", "џ", "а", "б", "в", "г", "д", "ђ", "е", "ж", "з", "и", "к", "љ", "л", "м", "њ", "ј", "н", "о", "п", "р", "с", "т", "ћ", "у", "ф", "х", "ц", "ч", "ш" };

            for (int i = 0; i < 60; i++) cir = cir.Replace(latinica[i], cirilica[i]);

            return cir;

        }

        public static int Cifra(this string x, int i)
        {
            return Convert.ToInt32(x.Substring(i, 1));
        }

        public static string Rec(this int n)
        {
            switch (n)
            {
                case 3:
                    return "Tri";
                case 4:
                    return "Četiri";
                default:
                    return "";
            }
        }

    }

    class Stampa
    {
        List<string> ucenici = new List<string>();
        object missing = Type.Missing;
        Microsoft.Office.Interop.Word.Application app;
        Microsoft.Office.Interop.Word.Document doc;
        string temp = Podesavanje.Vred("template");
        string ext = "";
        string pp = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string _putanja;

        private void Spajanje()
        {            
            object template = pp.Substring(0, pp.LastIndexOf(@"\") + 1) + @"Normal.dot";            
            string fullName = _putanja + @"\" + "Spojeno" + ext;
            object fname = fullName;
            object missing = System.Type.Missing;            
            object sectionBreak = WORD.WdBreakType.wdSectionBreakNextPage;            
            try
            {
                WORD._Document doca = app.Documents.Add(ref template, ref missing, ref missing, ref missing);
                doca.PageSetup.TopMargin = 1.46F;
                WORD.Selection selection = app.Selection;
                String insertFile = "";
                foreach(string s in ucenici)
                {
                    insertFile = _putanja + @"\" + s;
                    selection.InsertFile(insertFile, ref missing, ref missing, ref missing, ref missing);
                }
                doca.SaveAs(ref fname);
            }
            finally
            {
                app.Quit();
            }            
        }

        public void init()
        {            
            app = new Microsoft.Office.Interop.Word.Application();
            doc = app.Documents.Open(pp.Substring(0, pp.LastIndexOf(@"\") + 1) + temp, ref missing, true);
            ext = temp.Substring(temp.IndexOf("."));
        }

        public void kraj()
        {
            doc.Close();        
            if (Podesavanje.Vred("Spajanje") == "Da") Spajanje();
            app.Quit();
        }

        public Stampa(string putanja)
        {
            _putanja = putanja;
        }


        public void StampajZaUcenika(Ucenik uc)
        {
            FormFields fields = doc.FormFields;
            ucenici.Add(uc.naziv + ext);
            fields["Text1"].Result = Podesavanje.Vred("Naziv skole").Cir();
            fields["Text2"].Result = Podesavanje.Vred("Sediste").Cir();
            fields["Text5"].Result = Podesavanje.Vred("Resenje br").Cir();
            fields["Text6"].Result = Podesavanje.Vred("Resenje od").Cir();
            fields["Text3"].Result = Podesavanje.Vred("Delovodni broj i datum").Cir();
            fields["Text4"].Result = Podesavanje.Vred("Ministarstvo").Cir();

            string maticni = uc.broj.ToString();
            if (maticni.Length == 6) maticni = "0" + maticni;
            fields["Text7"].Result = maticni;

            string ime = uc.naziv.Substring(uc.naziv.LastIndexOf(" ") + 1);
            ime += " ";
            ime += uc.naziv.Substring(0, uc.naziv.LastIndexOf(" "));

            fields["Text8"].Result = ime.Cir();
            fields["Text9"].Result = uc.otac.Substring(uc.otac.LastIndexOf(" ") + 1).Cir();
            fields["Text10"].Result = uc.datumRodj;
            fields["Text12"].Result = uc.mestoRodj.Cir();

            string opstina = uc.opstinaRodj;
            string drzava = "Republika Srbija";
            if(opstina.Contains(","))
            {
                drzava = opstina.Substring(opstina.IndexOf(",") + 1);
                opstina = opstina.Substring(0, opstina.IndexOf(","));
            }

            fields["Text13"].Result = opstina.Cir();
            fields["Text14"].Result = drzava.Cir();
            fields["Text15"].Result = App.Godina().ToString();
            fields["Aaa"].Result = (App.Godina() + 1).ToString();

            fields["Dropdown7"].DropDown.Value = 2;
            fields["Dropdown3"].DropDown.Value = (int)uc.razred + 1;
            fields["Dropdown6"].DropDown.Value = (int)uc.razred + 1;
            fields["Text19"].Result = ((int)uc.trajanje).Rec().ToLower().Cir();
            fields["Text20"].Result = uc.smer.naziv.Cir();

            var listaPredmeta = Smer.DajPredmete(uc.smer, (int)uc.razred);
            int? ocenaVladanje = null;

            int i = 1, j = 1;
            foreach (Predmet pr in listaPredmeta)
            {
                fields["p" + j.ToString()].Result = "";
                fields["o" + j.ToString()].DropDown.Value = 1;
                if (pr.naziv == "Vladanje")
                {                    
                    ocenaVladanje = Ucenik.OcenaIz((int)uc.broj, pr.id, App.Godina());
                }
                else
                {                    
                    int? ocena = Ucenik.OcenaIz((int)uc.broj, pr.id, App.Godina());
                    if (ocena != null)
                    {
                        fields["p" + i.ToString()].Result = pr.naziv.Cir();
                        fields["o" + i.ToString()].DropDown.Value = 7 - (int)ocena;                        
                    }
                    else
                    {                        
                        fields["p" + i.ToString()].Result = "";
                        fields["o" + i.ToString()].DropDown.Value = 1;
                        i--;
                    }
                    i++;
                    j++;
                }

            }
            if (uc.veronauka == 0)
            {
                fields["ver"].DropDown.Value = 3;
                fields["over"].DropDown.Value = 5;
            }
            else
            {
                fields["ver"].DropDown.Value = 2;
                fields["over"].DropDown.Value = 2;
            }

            if (ocenaVladanje != null) fields["vladanje"].DropDown.Value = 7 - (int)ocenaVladanje;
            else fields["vladanje"].DropDown.Value = 1;

            double prosek = Ucenik.Prosek(uc);
            int rounded = (int)Math.Round(prosek + 0.01, 0);

            fields["Text29"].Result = String.Format("{0:0.00}", prosek);
            fields["Dropdown2"].DropDown.Value = 7 - rounded;
            

            doc.SaveAs(_putanja + @"\" + uc.naziv + ext);
        }

    }
}
