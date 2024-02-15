using BankaApp.BazaPodatakaStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class Zaposlenici : Osoba
{
    // prema instrukcijama sa stranice https://www.cbbh.ba/Content/Read/609?lang=bs
    // Menadzer banke ima privilegije mijenjanja oznake BBB i PPP
    private static string bbb = "123"; // nebitne default vrijednosti za bbb i ppp
    private static string ppp = "456"; // oznaka poslovnice
    private string pozicija;
    public int ID { get; set; }
    public Zaposlenici(int id, string ime, string prezime, string adresa, string brojTel, 
        string email, string licnaKarta, string korisnickoIme, string pozicija, string lozinka) : base(ime, prezime, adresa, brojTel, email, licnaKarta, korisnickoIme, lozinka) {
        this.pozicija = pozicija;
        this.ID = id;
    }
    internal string Pozicija { get => this.pozicija; set => pozicija = value; }
    public static void setBBB(string noviBBB)
    {
        if (bbb.Length != 3) { throw new LogicError("BBB treba da bude tacno 3 znamenke"); }
        bbb = noviBBB;
    }
    public static void setPPP(string noviPPP)
    {
        if (ppp.Length != 3) { throw new LogicError("PPP treba da bude tacno 3 znamenke"); }
        ppp = noviPPP;
    }

    public class Racun : IRacun
    {
        private static int brojac = 10000010;// + BazaPodatakaDAO.getNextValue(); // ovo predstavlja KKKKKKKK
        private string brojRacuna;
        public double StanjeRacuna { get; set; }

        public int IDZaposlenika { get; set; }
        public string BBB { get { return bbb; } }
        public string PPP {  get { return ppp; } }
        public Racun(string brojRacuna)
        {
            this.brojRacuna = brojRacuna;
            StanjeRacuna = 0;
        }
        // za test tj za metodu kreirajBazu u klasi BazaPodatakaDAO
        public Racun() {}
        public Racun(Zaposlenici zapo) {
            IDZaposlenika = zapo.ID;
        }
        public Racun izracunajIBANCC()
        {
            brojac++;
            int rezultat = (98 - ((brojac * 100) % 97)) % 97;
            // ako je rezultat jednocifren prosiri ga na dvije cifre
            string cc = rezultat.ToString();
            if ((rezultat / 10) == 0)
            {
                cc = "0" + rezultat.ToString();
            }
            return new Racun("BA" + cc + bbb + ppp + brojac.ToString() + cc);
        }
        public override string ToString()
        {
            return brojRacuna;
        }
    }
}
