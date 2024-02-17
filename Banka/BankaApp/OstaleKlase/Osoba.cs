using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Osoba
{
    private string ime;
    private string prezime;
    private string adresa;
    private string brojTel;
    private string email;
    private string licnaKarta;
    // za autentifikaciju
    private string korisnickoIme;
    private string lozinka;

    protected Osoba(string ime, string prezime, string adresa, string brojTel, string email, string licnaKarta, string korisnickoIme, string lozinka)
    {
        this.ime = ime;
        this.prezime = prezime;
        this.adresa = adresa;
        this.brojTel = brojTel;
        this.email = email;
        this.licnaKarta = licnaKarta;
        this.korisnickoIme = korisnickoIme;
        this.lozinka = lozinka;
    }

    public string Ime { get => ime; set => ime = value; }
    public string Prezime { get => prezime; set => prezime = value; }
    public string Adresa { get => adresa; set => adresa = value; }
    public string BrojTel { get => brojTel; set => brojTel = value; }
    public string Email { get => email; set => email = value; }
    public string LicnaKarta { get => licnaKarta; set => licnaKarta = value; }
    public string KorisnickoIme { get => korisnickoIme; set => korisnickoIme = value; }
    public string Lozinka { get => lozinka; set => lozinka = value; }
}
