// See https://aka.ms/new-console-template for more information
//MenadzerBanke m = new MenadzerBanke("I", "P", "adresa", "05478", "mail", "23wqi", "kime", "l");
//IRacun iBr = new MenadzerBanke.Racun();
////MenadzerBanke.setBBB("956");
////Console.WriteLine(iBr.izracunajIBANCC());

//Klient k1 = new Klient("I", "P", "adresa", "05478", "mail", "23wqi", "kime", "l", iBr.izracunajIBANCC());
//Klient k2 = new Klient("k2", "P2", "adresa2", "2", "mail2", "23wqi2", "kime2", "l2", iBr.izracunajIBANCC());

//Console.WriteLine(k1.BrojRacuna + " " + k2.BrojRacuna);

using BankaApp.BazaPodatakaStuff;
using System.Configuration;
using System.Data.SQLite;
using static System.Net.Mime.MediaTypeNames;

//BazaPodatakaDAO test = BazaPodatakaDAO.getInstance();
////test.kreirajBazu();
////test.vratiSveIzBaze("Klienti");
////test.vratiSveIzBaze("Racun");
//test.vratiSveIzBaze("Zaposlenici");
//BazaPodatakaDAO.removeInstance();

Osoba login(BazaPodatakaDAO model, string tabela)
{
    Console.Write("Unesite korisnicko ime: ");
    string korisnicko_ime = Console.ReadLine();
    Console.Write("Unesite lozinku: ");
    string lozinka = Console.ReadLine();
    Console.Write("\n");
    
    return model.validacijaKorisnika(korisnicko_ime, lozinka, tabela);
}

Osoba dodajNovuOsobu(string tip)
{
    Console.WriteLine("Unesite sljedece podatke klijenta: ");
    Console.Write("Ime: ");
    string ime = Console.ReadLine();
    Console.Write("Prezime: ");
    string prezime = Console.ReadLine();
    Console.Write("Adresa: ");
    string adresa = Console.ReadLine();
    Console.Write("Broj telefona: ");
    string brojTel = Console.ReadLine();
    Console.Write("Email: ");
    string email = Console.ReadLine();
    Console.Write("JMBG: ");
    string jmbg = Console.ReadLine();
    Console.Write("Korisnicko Ime: ");
    string kIme = Console.ReadLine();
    Console.Write("Lozinka: ");
    string lozinka = Console.ReadLine();
    Osoba rezultat;
    if(tip == "Klijent" || tip == "klijent" || tip == "K")
    {
        // Broj Racuna je nebitan trenutno buduci da ce se automacki dodati iz fije koja poziva ovu fiju
        rezultat = new Klient(ime, prezime, adresa, brojTel, email, jmbg, kIme, lozinka, null, 0);
    } else
    {
        Console.Write("Unesite poziciju zaposlenika: ");
        String pozicija = Console.ReadLine();
        // id zapo je nebitan jer imamo sekvencu u BP koja automacki povecava id za svakog novog zapo
        rezultat = new Zaposlenici(0, ime, prezime, adresa, brojTel, email, jmbg, kIme, pozicija, lozinka);
    }
    return rezultat;
}

int opcijeZaposlenika(Zaposlenici zapo, BazaPodatakaDAO model)
{
    Console.WriteLine($"Dobrodosli {zapo.KorisnickoIme}");
    Console.WriteLine("Izaberite neku od opcija(0 za izlaz): ");
    Console.WriteLine("\t\t1. Dodaj novog klijenta;");
    Console.WriteLine("\t\t2. Dodaj novac na racun klijenta;");
    Console.WriteLine("\t\t3. Pregled transakcija svih korisnika;");
    Console.WriteLine("\t\t4. Pregled transakcija specificnog korisnika;");

    if(zapo.Pozicija=="Menadzer")
    {
        // spec opcije
    }
    if(int.TryParse(Console.ReadLine(), out int opcija)) 
    {
        return opcija;
    }
    return -1;
}

int opcijeKlijenta(Klient k, BazaPodatakaDAO model)
{
    Console.WriteLine($"Dobrodosli {k.KorisnickoIme}");
    Console.WriteLine("Izaberite neku od opcija(0 za izlaz): ");
    Console.WriteLine("\t\t1. Podignite novac s racuna;");
    Console.WriteLine("\t\t2. Pregled svojih transakcija;");
    Console.WriteLine("\t\t3. Azurirajte podatke;");

    if (int.TryParse(Console.ReadLine(), out int opcija))
    {
        return opcija;
    }
    return -1;
}

void zaposlenik(BazaPodatakaDAO model)
{
    Zaposlenici zapo = (Zaposlenici)login(model, "Zaposlenici");
    while (zapo == null)
    {
        Console.WriteLine("Pogresno korisnicko ime ili lozinka!");
        zapo = (Zaposlenici)login(model, "Zaposlenici");
    }
    //standardno sta zaposlenik moze raditi
    int opcija;
    do
    {
        opcija = opcijeZaposlenika(zapo, model);
        // ne koristim switch da lakse omogucim da menadzer moze raditi sve sto i zaposlenik
        // i plus jos neke opcije
        if (opcija == 0)
        {
            break;
        }
        else if (opcija == 1)
        {
            dodajNovuOsobu("K");
        }
        else if(opcija == 2)
        {
            Console.Write("Unesite racun klijenta kojem se dodaje novac: ");
            string racun = Console.ReadLine();
            Console.Write("Unesite iznos: ");
            double iznos;
            if(double.TryParse(Console.ReadLine(), out iznos))
            {
                try
                {
                    model.dodajNovacNaRacun(zapo, racun, iznos);
                } catch(ArgumentOutOfRangeException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        else if(opcija == 3)
        {
            model.pregledajLogTransakcija();
        }
        else if(opcija == 4)
        {
            Console.Write("Unesite ime klijenta: ");
            string ime = Console.ReadLine();
            Console.Write("Unesite prezime klijenta: ");
            string prezime = Console.ReadLine();
            model.pregledajKorisnikovihTransakcija(ime, prezime);
        }
        else if (opcija == 10 && zapo.Pozicija == "Menadzer")
        {
            Zaposlenici noviZaposlenik = (Zaposlenici)dodajNovuOsobu("zaposlenici");
            model.dodajNovogZaposlenika(noviZaposlenik);
        }
        else
        {
            Console.WriteLine("Greska: pogresna opcija!");
        }
    } while (opcija != 0);

}

void klijent(BazaPodatakaDAO model)
{
    Klient k = (Klient)login(model, "K");
    while (k == null)
    {
        Console.WriteLine("Pogresno korisnicko ime ili lozinka!");
        k = (Klient)login(model, "K");
    }
    int opcija;
    do
    {
        opcija = opcijeKlijenta(k, model);
        switch(opcija)
        {
            case 0:
                Environment.Exit(0);
                break;
            case 1:
                double iznos;
                do {
                    Console.WriteLine("Unesite sumu koju zelite podici(0 za ponistavanje opcije): ");
                    if (!double.TryParse(Console.ReadLine(), out iznos))
                    {
                        Console.WriteLine("Pogresan unos, molimo pokusajte ponovo!");
                    }
                } while (!double.TryParse(Console.ReadLine(), out iznos));
                model.skiniNovacSaRacuna(k.BrojRacuna, iznos);
                break;
            case 2:
                model.pregledajKorisnikovihTransakcija(k.Ime, k.Prezime);
                break;
            case 3:
                model.azurirajKlijenta(k);
                break;
            default:
                Console.WriteLine("Greska: pogresna opcija!");
                break;
        }
    } while (opcija != 0);
}

while (true)
{
    BazaPodatakaDAO model = BazaPodatakaDAO.getInstance();

    Console.WriteLine("\tMENU");
    Console.WriteLine("Izaberite status(0 za izlaz): ");
    Console.WriteLine("\t0. Menadzer");
    Console.WriteLine("\t1. Zaposlenik");
    Console.WriteLine("\t2. Klijent");
    Console.Write("Unesite opciju: ");

    int status;
    if(int.TryParse(Console.ReadLine(), out status))
    {
        switch(status)
        {
            case 0:
                Environment.Exit(0);
                break;
            case 1:
                zaposlenik(model);
                break;
            case 2:
                klijent(model);
                break;
            case 10:
                model.vratiSveIzBaze("Klienti");
                model.vratiSveIzBaze("Racun");
                model.vratiSveIzBaze("Zaposlenici");
                break;
            default:
                Console.WriteLine("Greska: pogresna opcija!");
                break;
        }
    } else
    {
        Console.WriteLine("Potrebno je da unesete odgovarajuci broj!");
    }

    BazaPodatakaDAO.removeInstance();
}
