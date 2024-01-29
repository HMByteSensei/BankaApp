using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Klient : Osoba
{
    IRacun brojRacuna;
    public Klient(string ime, string prezime, string adresa, string brojTel, 
        string email, string licnaKarta, string korisnickoIme, string lozinka, IRacun brRac) : base(ime, prezime, adresa, brojTel, email, licnaKarta, korisnickoIme,
            lozinka) 
    {
        brojRacuna = brRac;
    }
    internal IRacun BrojRacuna { get => brojRacuna; set => brojRacuna = value; }
}
