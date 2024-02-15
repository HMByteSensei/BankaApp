using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Klient : Osoba
{
    IRacun brojRacuna;
    // 0 nije doslo do skidanja/dodavanja sredstava na racun, 1 skidanje sredstava
    // sa racuna, 2 dodavanje sredstava na racun
    int indikatorUplate;
    public Klient(string ime, string prezime, string adresa, string brojTel, 
        string email, string licnaKarta, string korisnickoIme, string lozinka, IRacun brRac, int indUplate=0) : base(ime, prezime, adresa, brojTel, email, licnaKarta, korisnickoIme,
            lozinka) 
    {
        brojRacuna = brRac;
        indikatorUplate = indUplate;
    }
    internal IRacun BrojRacuna { get => brojRacuna; set => brojRacuna = value; }
    internal int IndikatorUplate { get => indikatorUplate; set => indikatorUplate = value; }
}
