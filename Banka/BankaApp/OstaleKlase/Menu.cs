using BankaApp.BazaPodatakaModel;

internal class Menu
{
    public Osoba login(BazaPodatakaDAO model, string tabela)
    {
        Console.Write("Unesite korisnicko ime: ");
        string korisnicko_ime = Console.ReadLine();
        Console.Write("Unesite lozinku: ");
        string lozinka = Console.ReadLine();
        Console.Write("\n");

        return model.validacijaKorisnika(korisnicko_ime, lozinka, tabela);
    }

    public Osoba dodajNovuOsobu(string tip)
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
        if (tip == "Klijent" || tip == "klijent" || tip == "K")
        {
            // Broj Racuna je trenutno nebitan buduci da ce se automacki dodati iz fije koja poziva ovu fiju
            rezultat = new Klient(ime, prezime, adresa, brojTel, email, jmbg, kIme, lozinka, null, 0);
        }
        else
        {
            Console.Write("Unesite poziciju zaposlenika: ");
            String pozicija = Console.ReadLine();
            // id zapo je nebitan jer imamo sekvencu u BP koja automacki povecava id za svakog novog zapo
            rezultat = new Zaposlenici(0, ime, prezime, adresa, brojTel, email, jmbg, kIme, pozicija, lozinka);
        }
        return rezultat;
    }

    public int opcijeZaposlenika(Zaposlenici zapo, BazaPodatakaDAO model)
    {
        Console.WriteLine("\nIzaberite neku od opcija(0 za izlaz): ");
        Console.WriteLine("\t\t1. Dodaj novog klijenta;");
        Console.WriteLine("\t\t2. Dodaj novac na racun klijenta;");
        Console.WriteLine("\t\t3. Pregled transakcija svih korisnika;");
        Console.WriteLine("\t\t4. Pregled transakcija specificnog korisnika;");
        Console.WriteLine("\t\t5. Nadji klijenta sa datim JMBG;");
        Console.WriteLine("\t\t6. Obrisi klijenta sa datim JMBG;");
        Console.WriteLine("\t\t7. Ispisi svoje podatke;");
        Console.WriteLine("\t\t8. Ispisi sve klijente;");
        if (zapo.Pozicija == "Menadzer")
        {
            Console.WriteLine("\t\t9. Nadji zaposlenika sa zadanim ID-om;");
            Console.WriteLine("\t\t10. Azuriraj podatke zaposlenika sa datim ID-om;");
            Console.WriteLine("\t\t11. Obrisi zaposlenika sa datim ID-om;");
            Console.WriteLine("\t\t12. Dodaj novog zaposlenika;");
            Console.WriteLine("\t\t13. Ispisi sve zaposlene;");
        }
        if (int.TryParse(Console.ReadLine(), out int opcija))
        {
            return opcija;
        }
        return -1;
    }

    public int opcijeKlijenta(Klient k, BazaPodatakaDAO model)
    {
        Console.WriteLine("\nIzaberite neku od opcija(0 za izlaz): ");
        Console.WriteLine("\t\t1. Podignite novac s racuna;");
        Console.WriteLine("\t\t2. Pregled svojih transakcija;");
        Console.WriteLine("\t\t3. Azurirajte podatke;");
        Console.WriteLine("\t\t4. Ispisi svoje podatke;");

        if (int.TryParse(Console.ReadLine(), out int opcija))
        {
            return opcija;
        }
        return -1;
    }

    public void zaposlenik(BazaPodatakaDAO model)
    {
        Zaposlenici zapo = (Zaposlenici)login(model, "Zaposlenici");
        while (zapo == null)
        {
            Console.WriteLine("Pogresno korisnicko ime ili lozinka!");
            zapo = (Zaposlenici)login(model, "Zaposlenici");
        }
        Console.WriteLine($"Dobrodosli {zapo.KorisnickoIme}");
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
            else if (opcija == 2)
            {
                Console.Write("Unesite racun klijenta kojem se dodaje novac: ");
                string racun = Console.ReadLine();
                Console.Write("Unesite iznos: ");
                double iznos;
                if (double.TryParse(Console.ReadLine(), out iznos))
                {
                    try
                    {
                        model.dodajNovacNaRacun(zapo, racun, iznos);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            else if (opcija == 3)
            {
                model.pregledajLogTransakcija();
            }
            else if (opcija == 4)
            {
                Console.Write("Unesite ime klijenta: ");
                string ime = Console.ReadLine();
                Console.Write("Unesite prezime klijenta: ");
                string prezime = Console.ReadLine();
                model.pregledajKorisnikovihTransakcija(ime, prezime);
            }
            else if (opcija == 5)
            {
                Console.Write("Unesite JMBG klijenta: ");
                string unos = Console.ReadLine();
                Klient k = model.nadjiKlijenta(unos);
                if (k != null)
                {
                    Console.WriteLine("Klijent sa datim JMBG: ");
                    Console.WriteLine(k.ToString());
                }
            }
            else if (opcija == 6)
            {
                Console.Write("Unesite JMBG klijenta: ");
                string unos = Console.ReadLine();
                model.obrisiKlienta(unos);
            }
            else if (opcija == 7)
            {
                model.vratiPodatkeTrenutneOsobe(zapo, "Zaposlenici");
            }
            else if(opcija == 8)
            {
                model.vratiSveIzBaze("Klienti");
            }
            else if (zapo.Pozicija == "Menadzer")
            {
                if (opcija == 9)
                {
                    Console.Write("Unesite ID zaposlenika: ");
                    int zapoID;
                    if (int.TryParse(Console.ReadLine(), out zapoID))
                    {
                        Zaposlenici traziZapo = model.nadjiZaposlenika(zapoID);
                        if (traziZapo != null)
                        {
                            Console.WriteLine(model.nadjiZaposlenika(zapoID).ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Neispravan unos!");
                    }
                }
                else if (opcija == 10)
                {
                    Console.Write("Unesite ID zaposlenika: ");
                    int zapoID;
                    if (int.TryParse(Console.ReadLine(), out zapoID))
                    {
                        Zaposlenici zaposlenik = model.nadjiZaposlenika(zapoID);
                        model.azurirajZaposlenika(zaposlenik);
                    }
                    else
                    {
                        Console.WriteLine("Neispravan unos!");
                    }
                }
                else if (opcija == 11)
                {
                    Console.Write("Unesite ID zaposlenika: ");
                    int zapoID;
                    if (int.TryParse(Console.ReadLine(), out zapoID))
                    {
                        model.obrisiZaposlenika(zapoID);
                    }
                    else
                    {
                        Console.WriteLine("Neispravan unos!");
                    }
                }
                else if (opcija == 12)
                {
                    Zaposlenici noviZaposlenik = (Zaposlenici)dodajNovuOsobu("zaposlenici");
                    model.dodajNovogZaposlenika(noviZaposlenik);
                }
                else if(opcija == 13)
                {
                    model.vratiSveIzBaze("Zaposlenici");
                }
            }
            else
            {
                Console.WriteLine("Greska: pogresna opcija!");
            }
        } while (opcija != 0);

    }

    public void klijent(BazaPodatakaDAO model)
    {
        Klient k = (Klient)login(model, "Klienti");
        while (k == null)
        {
            Console.WriteLine("Pogresno korisnicko ime ili lozinka!");
            k = (Klient)login(model, "Klienti");
        }
        Console.WriteLine($"Dobrodosli {k.KorisnickoIme}");
        int opcija;
        do
        {
            opcija = opcijeKlijenta(k, model);
            switch (opcija)
            {
                case 0:
                    break;
                case 1:
                    double iznos;
                    bool nekorektanUnos;
                    do
                    {
                        Console.Write("Unesite sumu koju zelite podici(0 za ponistavanje opcije): ");
                        nekorektanUnos = !double.TryParse(Console.ReadLine(), out iznos);
                        if (nekorektanUnos)
                        {
                            Console.WriteLine("Pogresan unos, molimo pokusajte ponovo!");
                        }
                    } while (nekorektanUnos);
                    try
                    {
                        model.skiniNovacSaRacuna(k.BrojRacuna, iznos);
                    }
                    catch (LogicError e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case 2:
                    model.pregledajKorisnikovihTransakcija(k.Ime, k.Prezime);
                    break;
                case 3:
                    model.azurirajKlijenta(k);
                    break;
                case 4:
                    model.vratiPodatkeTrenutneOsobe(k, "Klienti");
                    break;
                default:
                    Console.WriteLine("Greska: pogresna opcija!");
                    break;
            }
        } while (opcija != 0);
    }

}
