using System.Data.SQLite;
using System.Data.SqlTypes;

//https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=netcore-cli

namespace BankaApp.BazaPodatakaModel
{
    internal class BazaPodatakaDAO
    {
        private static BazaPodatakaDAO instance = null;
        private static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\BazaPodataka", "BankaBP.db");
        private SQLiteConnection conn = new SQLiteConnection($"Data Source={path}");

        private SQLiteCommand skiniNovac, dodajNovac, vidiLogTransakcije, vidiLogKlienta, 
            dodajKlienta, azurirajPodatkeKlienta, zapoDodajRacun, zapoNadjiKlienta,
            menDodajZapo, menObrisiZapo, menAzurirajZapo, zapoObrisiKlienta, ispisiPodatkeOosobi,
            menNadjiZapo, validacijaKlijenta, validacijaZaposlenog, pronadjiZaposlenog;
        private static SQLiteCommand updateBrojacSeq, brojacSeq;
        private BazaPodatakaDAO()
        {
            conn.Open();
            skiniNovac = conn.CreateCommand();
            skiniNovac.CommandText = @"UPDATE Racun SET Stanje_Racuna = Stanje_Racuna - $iznos " +
                "WHERE Broj_Racuna = $BrojRacuna";

            dodajNovac = new SQLiteCommand(@"UPDATE Racun SET Stanje_Racuna = Stanje_Racuna + $iznos
                            WHERE Broj_Racuna = $BrojRacuna;", conn);

            vidiLogKlienta = new SQLiteCommand(@"SELECT * FROM Logs WHERE Ime_Klienta=$Ime_Klienta AND Prezime_Klienta=$Prezime_Klienta", conn);

            vidiLogTransakcije = new SQLiteCommand(@"SELECT * FROM Logs;", conn);

            dodajKlienta = new SQLiteCommand(@"INSERT INTO KLIENTI VALUES($Ime, $Prezime, $Adresa,
                $Broj_Telefona, $Email, $JMBG, $Korisnicko_Ime, $Lozinka, $Broj_Racuna, $Indikator_Uplate);", conn);


            azurirajPodatkeKlienta = new SQLiteCommand(@"UPDATE Klienti SET 
                            Adresa = $Adresa,
                            Email = $Email,
                            Broj_Telefona = $Broj_Telefona,
                            Korisnicko_Ime = $Korisnicko_ime,
                            Lozinka = $Lozinka
                            WHERE JMBG = $JMBG;", conn);

            zapoDodajRacun = new SQLiteCommand(@"INSERT INTO Racun(Broj_Racuna, Osnovao_Zaposlenik, Stanje_Racuna) 
                    VALUES($BrojRacuna, $Zaposlenik_ID, $Stanje_Racuna);", conn);

            menDodajZapo = new SQLiteCommand(@"INSERT INTO Zaposlenici(Ime, Prezime, Adresa, Broj_Telefona," +
                "Email, JMBG, Korisnicko_Ime, Pozicija, Lozinka) " +
                "VALUES($Ime, $Prezime, $Adresa, $Broj_Telefona, $Email, $JMBG, $Korisnicko_Ime, " +
                "$Pozicija, $Lozinka)", conn);

            menObrisiZapo = new SQLiteCommand(@"DELETE FROM Zaposlenici WHERE ID = $ID;", conn);

            // bez lozinke pa zato nije "SELECT *..."
            zapoNadjiKlienta = new SQLiteCommand(@"SELECT Ime, Prezime, Adresa, Broj_Telefona,
                            Email, JMBG, Korisnicko_Ime, Broj_Racuna
                            FROM Klienti WHERE JMBG=$JMBG", conn);

            menAzurirajZapo = new SQLiteCommand(@"UPDATE Zaposlenici SET
                            Korisnicko_Ime = $Korisnicko_ime,
                            Lozinka = $Lozinka,
                            Pozicija = $Pozicija
                            WHERE ID = $ID;", conn);

            zapoObrisiKlienta = new SQLiteCommand(@"PRAGMA foreign_keys = ON; DELETE FROM Klienti WHERE JMBG = $JMBG;", conn);
            
            menNadjiZapo = new SQLiteCommand(@"SELECT ID, Ime, Prezime, Adresa, Broj_Telefona,
                            Email, JMBG, Korisnicko_Ime, Pozicija
                            FROM Zaposlenici WHERE ID=$ID", conn);

            validacijaKlijenta = new SQLiteCommand(@"SELECT * FROM Klienti 
                                WHERE Korisnicko_Ime=$Korisnicko_Ime AND Lozinka=$Lozinka;", conn);

            validacijaZaposlenog = new SQLiteCommand(@"SELECT * FROM Zaposlenici
                            WHERE Korisnicko_Ime=$Korisnicko_Ime AND Lozinka=$Lozinka;", conn);

            pronadjiZaposlenog = new SQLiteCommand(@"SELECT ID FROM Zaposlenici
                            WHERE JMBG=$JMBG;", conn);
            updateBrojacSeq = new SQLiteCommand(@"UPDATE brojac_seq SET next_value = next_value + 1;", conn);

            brojacSeq = new SQLiteCommand(@"SELECT next_value FROM brojac_seq;", conn);
        }
        public Zaposlenici nadjiZaposlenika(int zapoID)
        {
            menNadjiZapo.Parameters.AddWithValue("$ID", zapoID);
            Zaposlenici zapo = null;
            try
            {
                using (SQLiteDataReader citac = menNadjiZapo.ExecuteReader())
                {
                    while (citac.Read())
                    {
                        zapo = new Zaposlenici(citac.GetInt32(0), citac.GetString(1),
                            citac.GetString(2), citac.GetString(3), citac.GetString(4),
                            citac.GetString(5), citac.GetString(6), citac.GetString(7),
                            citac.GetString(8), "Nije dozvoljeno vidjeti lozinku");
                    }
                }
            } catch { Console.WriteLine("Doslo je do greske prilikom pretrage!");  }
            return zapo;
        }
        public bool obrisiZaposlenika(int zapoID)
        {
            Zaposlenici zapo = nadjiZaposlenika(zapoID);
            if(zapo == null)
            {
                Console.WriteLine("Ne postoji zaposlenik sa datim ID.");
                return false;
            }
            Console.WriteLine(zapo.ToString());
            Console.Write("Da li ste sigurni da zelite obrisati datog zaposlenika? (Da/Ne): ");
            string brisanjeUnos = Console.ReadLine();
            if(brisanjeUnos == "Da" || brisanjeUnos == "da")
            {
                try
                {
                    menObrisiZapo.Parameters.AddWithValue("$ID", zapoID);
                    menObrisiZapo.ExecuteNonQuery();
                    Console.WriteLine("Uspjesno obrisan zaposlenik.");
                    return true;
                } catch { Console.WriteLine("Greska prilikom brisanja, molimo pokusajte ponovo..."); }
            }
            return false;
        }
        public void vratiPodatkeTrenutneOsobe(Osoba o, string tabela)
        {
            ispisiPodatkeOosobi = new SQLiteCommand($"SELECT * FROM {tabela} WHERE JMBG = $JMBG;", conn);
            try
            {
                ispisiPodatkeOosobi.Parameters.AddWithValue("$JMBG", o.LicnaKarta);
                using(SQLiteDataReader citac = ispisiPodatkeOosobi.ExecuteReader())
                {
                    for(int i = 0; i < citac.FieldCount; i++)
                    {
                        Console.Write($"{citac.GetName(i), -20}|");
                    }
                while (citac.Read())
                {
                    Console.WriteLine();
                        for (int i = 0; i < citac.FieldCount; i++)
                        {
                            Console.Write($"{citac.GetValue(i), -20}|");
                        }
                    }
                }
                Console.WriteLine();
            } catch(Exception e) { Console.WriteLine("Greska pri citanju podataka.");
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }
        public Klient nadjiKlijenta(string JMBG)
        {
            zapoNadjiKlienta.Parameters.AddWithValue("$JMBG", JMBG);
            Klient k = null;
            try
            {
                using (SQLiteDataReader citac = zapoNadjiKlienta.ExecuteReader())
                {
                    while (citac.Read())
                    {
                        IRacun racunKlijenta = new Zaposlenici.Racun(citac.GetString(7));
                        k = new Klient(citac.GetString(0), citac.GetString(1),
                            citac.GetString(2), citac.GetString(3), citac.GetString(4),
                            citac.GetString(5), citac.GetString(6), "Nije dozvoljeno vidjeti lozinku",
                            racunKlijenta, 0);
                    }
                }
            }
            catch(Exception e) { Console.WriteLine("Doslo je do greske prilikom pretrage!");
                Console.Write(e.Message);
            }
            return k;
        }
        public bool obrisiKlienta(string JMBG)
        {
            Klient k = nadjiKlijenta(JMBG);
            if (k == null)
            {
                Console.WriteLine("Ne postoji klijent sa datim JMBG.");
                return false;
            }
            Console.WriteLine(k.ToString());
            Console.Write("Da li ste sigurni da zelite obrisati datog klijenta? (Da/Ne): ");
            string brisanjeUnos = Console.ReadLine();
            if (brisanjeUnos == "Da" || brisanjeUnos == "da")
            {
                try
                {
                    zapoObrisiKlienta.Parameters.AddWithValue("$JMBG", JMBG);
                    zapoObrisiKlienta.ExecuteNonQuery();
                    Console.WriteLine("Uspjesno obrisan klijent.");
                    return true;
                }
                catch { Console.WriteLine("Greska prilikom brisanja, molimo pokusajte ponovo..."); }
            }
            return false;

        }
        public bool dodajNovogKlijenta(Zaposlenici zapo, Klient klijent)
        {
            IRacun racunKlijenta = new Zaposlenici.Racun(zapo).izracunajIBANCC();
            try
            {
                dodajKlienta.Parameters.AddWithValue("$Ime", klijent.Ime);
                dodajKlienta.Parameters.AddWithValue("$Prezime", klijent.Prezime);
                dodajKlienta.Parameters.AddWithValue("$Adresa", klijent.Adresa);
                dodajKlienta.Parameters.AddWithValue("$Broj_Telefona", klijent.BrojTel);
                dodajKlienta.Parameters.AddWithValue("$Email", klijent.Email);
                dodajKlienta.Parameters.AddWithValue("$JMBG", klijent.LicnaKarta);
                dodajKlienta.Parameters.AddWithValue("$Korisnicko_Ime", klijent.KorisnickoIme);
                dodajKlienta.Parameters.AddWithValue("$Lozinka", klijent.Lozinka);
                dodajKlienta.Parameters.AddWithValue("$Broj_Racuna", racunKlijenta.ToString());
                dodajKlienta.Parameters.AddWithValue("$Indikator_Uplate", 0);
                dodajKlienta.ExecuteNonQuery();

                zapoDodajRacun.Parameters.AddWithValue("$BrojRacuna", racunKlijenta.ToString());
                zapoDodajRacun.Parameters.AddWithValue("$Zaposlenik_ID", zapo.ID);
                zapoDodajRacun.Parameters.AddWithValue("$Stanje_Racuna", 0);
                zapoDodajRacun.ExecuteNonQuery();
                return true;
            } catch(Exception e)
            {
                Console.WriteLine("Doslo je do greske prilikom dodavanja korisnika!");
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public void pregledajLogTransakcija()
        {
            try
            {
                using (SQLiteDataReader rezultat = vidiLogTransakcije.ExecuteReader())
                {
                    Console.WriteLine("Rezultat:");
                    for (int i = 0; i < rezultat.FieldCount; i++)
                    {
                        Console.Write($"{rezultat.GetName(i),-20}|");
                    }
                    Console.WriteLine();
                    while (rezultat.Read())
                    {
                        for (int i = 0; i < rezultat.FieldCount; i++)
                        {
                            Console.Write($"{rezultat.GetValue(i),-20}|");
                        }
                    }
                }
            } catch
            {
                Console.WriteLine("Doslo je do greske!");
            }
        }

        public void pregledajKorisnikovihTransakcija(string ime, string prezime)
        {
            vidiLogKlienta.Parameters.AddWithValue("$Ime_Klienta", ime);
            vidiLogKlienta.Parameters.AddWithValue("$Prezime_Klienta", prezime);
            try
            {
                using (SQLiteDataReader rezultat = vidiLogKlienta.ExecuteReader())
                {
                    Console.WriteLine("Rezultat:");
                    for (int i = 0; i < rezultat.FieldCount; i++)
                    {
                        Console.Write($"{rezultat.GetName(i),-20}|");
                    }
                    Console.WriteLine();
                    while (rezultat.Read())
                    {
                        for (int i = 0; i < rezultat.FieldCount; i++)
                        {
                            Console.Write($"{rezultat.GetValue(i),-20}|");
                        }
                    }
                }
            } catch
            {
                Console.WriteLine("Doslo je do greske molimo pokusajte ponovo!");
            }
        }
        public bool azurirajZaposlenika(Zaposlenici zapo)
        {
            string[] izkazi = {
                "Unesite novo korisnicko ime (ENTER ako nema promjene): ",
                "Unesite novu lozinku (ENTER ako nema promjene): ",
                "Unesite novu poziciju (ENTER ako nema promjene): "
            };
            string[] placeholder =
            {
                "$Korisnicko_ime", "$Lozinka", "$Pozicija"
            };
            string[] vrijednosti =
            {
                zapo.KorisnickoIme, zapo.Lozinka, zapo.Pozicija
            };

            for (int i = 0; i < 3; i++)
            {
                Console.Write(izkazi[i]);
                string unesi = Console.ReadLine();
                if (unesi.Length == 0)
                {
                    menAzurirajZapo.Parameters.AddWithValue(placeholder[i], vrijednosti[i]);
                }
                else
                {
                    menAzurirajZapo.Parameters.AddWithValue(placeholder[i], unesi);
                }
            }
            menAzurirajZapo.Parameters.AddWithValue("ID", zapo.ID);
            try
            {
                menAzurirajZapo.ExecuteNonQuery();
                Console.WriteLine("Uspjesno ste azurirali podatke zaposlenika.");
                return true;
            }
            catch
            {
                Console.WriteLine("Doslo je do greske pri azuriranju podataka zaposlenika!");
                return false;
            }

        }
        public bool azurirajKlijenta(Klient k)
        {
            string[] izkazi = {
                "Unesite novu adresu (ENTER ako nema promjene): ",
                "Unesite novi email (ENTER ako nema promjene): ",
                "Unesite novi broj telefona (ENTER ako nema promjene): ",
                "Unesite novo korisnicko ime (ENTER ako nema promjene): ",
                "Unesite novu lozinku (ENTER ako nema promjene): "
            };
            string[] placeholder =
            {
                "$Adresa", "$Email", "$Broj_Telefona", "$Korisnicko_ime", "$Lozinka" 
            };
            string[] vrijednosti =
            {
                k.Adresa, k.Email, k.BrojTel, k.KorisnickoIme, k.Lozinka
            };
            azurirajPodatkeKlienta.Parameters.Clear();
            for (int i=0; i<5; i++)
            {
                Console.Write(izkazi[i]);
                string unesi = Console.ReadLine();
                if(unesi.Length == 0)
                {
                    azurirajPodatkeKlienta.Parameters.AddWithValue(placeholder[i], vrijednosti[i]);
                } else
                {
                    azurirajPodatkeKlienta.Parameters.AddWithValue(placeholder[i], unesi);
                }
            }
            azurirajPodatkeKlienta.Parameters.AddWithValue("$JMBG", k.LicnaKarta);
            
            try
            {
                azurirajPodatkeKlienta.ExecuteNonQuery();
                Console.WriteLine("Uspjesno ste azurirali svoje podatke.");
                return true;
            } catch(Exception e)
            {
                Console.WriteLine("Doslo je do greske pri azuriranju podataka korisnika!");
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool dodajNovacNaRacun(Zaposlenici zapo, string racun, double iznos)
        {
            if(iznos > 10000)
            {
                throw new ArgumentOutOfRangeException("Ne mozete dodati vise od 10000 bilo koje valute!");
            }
            try
            {
                dodajNovac.Parameters.AddWithValue("$iznos", iznos);
                dodajNovac.Parameters.AddWithValue("$BrojRacuna", racun);
                SQLiteCommand racun_logs = new SQLiteCommand(@"INSERT INTO racun_logs
                        (Ime_Zaposlenika, Prezime_Zaposlenika, ID, Broj_Racuna, Dodano_skinuto)
                        VALUES($Ime, $Prezime, $ID, $BrojRacuna, 'Dodano');", conn);
                racun_logs.Parameters.AddWithValue("$Ime", zapo.Ime);
                racun_logs.Parameters.AddWithValue("$Prezime", zapo.Prezime);
                racun_logs.Parameters.AddWithValue("$ID", zapo.ID);
                racun_logs.Parameters.AddWithValue("$BrojRacuna", racun);
                racun_logs.ExecuteNonQuery();
                dodajNovac.ExecuteNonQuery();
                Console.WriteLine($"\nUspjesno ste dodali {iznos} na dati racun.");
            } catch(Exception e) {
                Console.WriteLine("Doslo je do greske prilikom dodavanja sredstava na racun, provjerite da li ste dobro unijeli podatke!");
                Console.WriteLine(e.Message);
                return false; 
            }
            return true;
        }

        public bool dodajNovogZaposlenika(Zaposlenici noviZapo)
        {
            try
            {
                menDodajZapo.Parameters.AddWithValue("$Ime", noviZapo.Ime);
                menDodajZapo.Parameters.AddWithValue("$Prezime", noviZapo.Prezime);
                menDodajZapo.Parameters.AddWithValue("$Adresa", noviZapo.Adresa);
                menDodajZapo.Parameters.AddWithValue("$Broj_Telefona", noviZapo.BrojTel);
                menDodajZapo.Parameters.AddWithValue("$Email", noviZapo.Email);
                menDodajZapo.Parameters.AddWithValue("$JMBG", noviZapo.LicnaKarta);
                menDodajZapo.Parameters.AddWithValue("$Korisnicko_Ime", noviZapo.KorisnickoIme);
                menDodajZapo.Parameters.AddWithValue("$Pozicija", noviZapo.Pozicija);
                menDodajZapo.Parameters.AddWithValue("$Lozinka", noviZapo.Lozinka);
           
                menDodajZapo.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Doslo je do greske prilikom dodavanja novog zaposlenika!");
                Console.WriteLine(e.Message);
                return false;
            }

        }
        public Osoba validacijaKorisnika(string korisnicko_ime, string lozinka, string tabela)
        {
            if (String.Equals(tabela, "Klienti"))
            {
                validacijaKlijenta.Parameters.AddWithValue("$Korisnicko_Ime", korisnicko_ime);
                validacijaKlijenta.Parameters.AddWithValue("$Lozinka", lozinka);
                using(SQLiteDataReader osoba = validacijaKlijenta.ExecuteReader())
                {
                    if(osoba.Read())
                    {
                        IRacun racun = new Zaposlenici.Racun(osoba["Broj_Racuna"].ToString());
                        Klient korisnik = new Klient(
                                osoba["ime"].ToString(),
                                osoba["prezime"].ToString(),
                                osoba["adresa"].ToString(),
                                osoba["Broj_Telefona"].ToString(),
                                osoba["email"].ToString(),
                                osoba["JMBG"].ToString(),
                                osoba["korisnicko_Ime"].ToString(),
                                osoba["lozinka"].ToString(),
                                racun,
                                0
                            );
                        int indikator;
                        if (int.TryParse(osoba["Indikator_uplate"].ToString(), out indikator))
                        {
                            korisnik.IndikatorUplate = indikator;
                        }
                        return korisnik;
                    }
                }
            } 
            else
            {
                validacijaZaposlenog.Parameters.AddWithValue("$Korisnicko_Ime", korisnicko_ime);
                validacijaZaposlenog.Parameters.AddWithValue("$Lozinka", lozinka);
                using (SQLiteDataReader osoba = validacijaZaposlenog.ExecuteReader())
                {
                    if (osoba.Read())
                    {
                        Zaposlenici korisnik = new Zaposlenici(
                                0,
                                osoba["ime"].ToString(),
                                osoba["prezime"].ToString(),
                                osoba["adresa"].ToString(),
                                osoba["Broj_Telefona"].ToString(),
                                osoba["email"].ToString(),
                                osoba["JMBG"].ToString(),
                                osoba["korisnicko_Ime"].ToString(),
                                osoba["Pozicija"].ToString(),
                                osoba["lozinka"].ToString()
                            );
                        if (int.TryParse(osoba["ID"].ToString(), out int id))
                        {
                            korisnik.ID = id;
                        }
                        return korisnik;
                    }
                }
            }
            return null;
        }

        public void kreirajBazu()
        {
            string primjer = new Zaposlenici.Racun().izracunajIBANCC().ToString();
            var i1 = new SQLiteCommand(
                @"INSERT INTO Klienti VALUES('Klijent1', 'Klijent1', 'Test', '359-758/452', " +
                "'mail', '78549498412', 'kIme', 'lozinka1', $broj_racuna, 0);", conn);
            i1.Parameters.AddWithValue("$broj_racuna", primjer);

            var i2 = new SQLiteCommand(
                @"INSERT INTO Racun(Broj_Racuna, Osnovao_Zaposlenik, Stanje_Racuna)
                  VALUES($broj_rac, 1, 1000);                  
                ", conn);
            i2.Parameters.AddWithValue("$broj_rac", primjer);

            var i3 = new SQLiteCommand(
                @"INSERT INTO Zaposlenici 
                  VALUES(1, 'Zapo1', 'ZapoPrezime', 'Adresa', '487-897/471', 'emial2', 
                  '6987453578', 'kIme2', 'Menadzer', 'lozinka2');
                ", conn);

            var i4 = new SQLiteCommand(@"INSERT INTO brojac_seq VALUES(10);", conn);
            try
            {
                i1.ExecuteNonQuery();
            } catch (SQLiteException e) { }
            try
            {
                i2.ExecuteNonQuery();
            }
            catch (SQLiteException e) { }
            try
            {
                i3.ExecuteNonQuery();
            }
            catch (SQLiteException e) { }
            try
            {
                i4.ExecuteNonQuery();
            }
            catch (SQLiteException e) { }
        }
        // Za testiranje, komentirati ovu fiju nakon zavrsetka test faze
        //------------------POCETAK FIJA KOJE SLUZE ZA TESTIRANJE-----------------
        public void vratiSveIzBaze(string tabela)
        {
            var rez = new SQLiteCommand($"SELECT * FROM {tabela}", conn);
            using (var citac = rez.ExecuteReader())
            {
                for (int i = 0; i < citac.FieldCount; i++)
                {
                    Console.Write($"{citac.GetName(i), -20}|");
                }
                while (citac.Read())
                {
                    Console.WriteLine();
                    for (int i = 0; i < citac.FieldCount; i++)
                    {
                        Console.Write($"{citac.GetValue(i), -20}|");
                    }
                }
            }
            Console.WriteLine("\n-------------------");

        }

        public void obrisiPodatkeSvihTabela()
        {
            SQLiteCommand brisiKlient = new SQLiteCommand(@"DELETE FROM Klienti;", conn);
            SQLiteCommand brisiZaposlenike = new SQLiteCommand(@"DELETE FROM Zaposlenici;", conn);
            SQLiteCommand brisiLogs = new SQLiteCommand(@"DELETE FROM Logs;", conn);
            SQLiteCommand brisiRacun = new SQLiteCommand(@"DELETE FROM Racun;", conn);
            SQLiteCommand brisiRacunLogs = new SQLiteCommand(@"DELETE FROM racun_logs;", conn);
            
            brisiKlient.ExecuteNonQuery();
            brisiLogs.ExecuteNonQuery();
            brisiRacun.ExecuteNonQuery();
            brisiZaposlenike.ExecuteNonQuery();
            brisiRacunLogs.ExecuteNonQuery();
        }
        //------------------KRAJ FIJA KOJE SLUZE ZA TESTIRANJE-----------------
        public static BazaPodatakaDAO getInstance()
        {
            if (instance == null) {
                try
                {
                    instance = new BazaPodatakaDAO();
                } catch(SqlTypeException e)
                {
                    instance = null;
                    Console.WriteLine("Greska pri radu s bazom podataka: " + e.Message);
                }
            }
            return instance;        
        }

        public static void removeInstance()
        {
            if(instance != null)
            {
                instance.conn.Close();
                instance = null;
            }
        }

        public int vratiIDZaposlenog(string jmbg)
        {
            pronadjiZaposlenog.Parameters.AddWithValue("$JMBG", jmbg);
            object rezultat = pronadjiZaposlenog.ExecuteScalar();
            if(rezultat != null && int.TryParse(rezultat.ToString(), out int nadjenID))
            {
                return nadjenID;
            }
            return -1;
        }

        public void skiniNovacSaRacuna(IRacun racun, double iznos)
        {
            skiniNovac.Parameters.AddWithValue("$iznos", iznos);
            skiniNovac.Parameters.AddWithValue("$BrojRacuna", racun.ToString());
            if(racun.StanjeRacuna - iznos < 0)
            {
                throw new LogicError("Ne mozete skinuti vise novca nego sto imate na racunu!");
            }
            try
            {
                skiniNovac.ExecuteNonQuery();
                Console.WriteLine($"Uspjesno ste podigli {iznos} sa vaseg racuna.");
            } catch
            {
                Console.WriteLine("Doslo je do greske prilikom skidanja sredstava datom korisniku!");
            }
        }


        public static int getNextValue()
        {
            // magicna konstanta je samo tu da se kompajler ne buni kako koristimo
            // ne inicijalizovanu variablu
            int brojac = 0;
            updateBrojacSeq.ExecuteNonQuery();
            using (SQLiteDataReader rezultat = brojacSeq.ExecuteReader())
            {
                while (rezultat.Read())
                {
                    brojac = Convert.ToInt32(rezultat["next_value"]);
                }
            }
            return brojac;
        }

    }
}
