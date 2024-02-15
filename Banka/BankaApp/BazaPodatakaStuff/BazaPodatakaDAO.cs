/* TO DO:
 * 1. Kod dodaj klienta neka na pocetku bude nekakva random lozinka ili nesto pa 
 * neka on kasnije mijenja ili nesto 
 *  isto i menDodajZapo
 *  
 * 2. Zatvaranje racuna samo ako klient nije u minusu isto i za brisanje klienta
 *  ustvari to je foreign key
 * 
 * 3. Da li ce autoincrement kod dodavanja zaposlenika raditi bez da ga specificiram
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.SqlTypes;
using System.Reflection.Metadata;
using System.Data.SqlClient;

//https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=netcore-cli

namespace BankaApp.BazaPodatakaStuff
{
    internal class BazaPodatakaDAO
    {
        private static BazaPodatakaDAO instance = null;
        private static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\BazaPodataka", "BankaBP.db");
        private SQLiteConnection conn = new SQLiteConnection($"Data Source={path}");

        private SQLiteCommand skiniNovac, dodajNovac, vidiLogTransakcije, vidiLogKlienta, 
            dodajKlienta, azurirajPodatkeKlienta, zapoDodajRacun,
            menDodajZapo, menObrisiZapo, menAzurirajZapo, zapoObrisiKlienta, zapoObrisiRacun,
            menNadjiZapo, validacijaKlijenta, validacijaZaposlenog, pronadjiZaposlenog;
        private static SQLiteCommand brojacSeq;
        private BazaPodatakaDAO()
        {
            conn.Open();
            skiniNovac = conn.CreateCommand();
            skiniNovac.CommandText = @"UPDATE Racun SET Stanje_Racuna = Stanje_Racuna - $iznos " +
                "WHERE Broj_Racuna = $BrojRacuna";

            dodajNovac = new SQLiteCommand(@"UPDATE Racun SET Stanje_Racuna = Stanje_Racuna + $iznos
                            WHERE Broj_Racuna = $BrojRacuna;", conn);

            vidiLogKlienta = new SQLiteCommand(@"SELECT * FROM Logs WHERE Ime_Klienta=$Ime_Klienta AND Prezime_Klienta=$Prezime_Klienta");

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

            menAzurirajZapo = new SQLiteCommand(@"UPDATE Zaposlenici SET
                            Adresa = $Adresa,
                            Email = $Email,
                            Broj_Telefona = $Broj_Telefona,
                            Korisnicko_Ime = $Korisnicko_ime,
                            Lozinka = $Lozinka,
                            Pozicija = $Pozicija
                            WHERE ID = $ID;", conn);

            zapoObrisiKlienta = new SQLiteCommand(@"DELETE FROM Klienti WHERE JMBG = $JMBG;", conn);
            
            zapoObrisiRacun = new SQLiteCommand(@"DELETE FROM Racun WHERE Broj_Racuna=$Broj_Racuna;", conn);

            menNadjiZapo = new SQLiteCommand(@"SELECT ID, Ime, Prezime, Adresa, Broj_Telefona,
                            Email, JMBG, Korisnicko_Ime, Pozicija
                            FROM Zaposlenici WHERE ID=$ID", conn);

            validacijaKlijenta = new SQLiteCommand(@"SELECT * FROM Klienti 
                                WHERE Korisnicko_Ime=$Korisnicko_Ime AND Lozinka=$Lozinka;", conn);

            validacijaZaposlenog = new SQLiteCommand(@"SELECT * FROM Zaposlenici
                            WHERE Korisnicko_Ime=$Korisnicko_Ime AND Lozinka=$Lozinka;", conn);

            pronadjiZaposlenog = new SQLiteCommand(@"SELECT ID FROM Zaposlenici
                            WHERE JMBG=$JMBG;", conn);
            brojacSeq = new SQLiteCommand(@"SELECT next_value FROM brojac_seq;", conn);
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

            for(int i=0; i<5; i++)
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

            try
            {
                azurirajPodatkeKlienta.ExecuteNonQuery();
                return true;
            } catch
            {
                Console.WriteLine("Doslo je do greske pri azuriranju podataka korisnika!");
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
                        VALUES($Ime, $Prezime, $ID, $BrojRacuna, 'Dodano');", conn);
                racun_logs.Parameters.AddWithValue("$Ime", zapo.Ime);
                racun_logs.Parameters.AddWithValue("$Prezime", zapo.Prezime);
                racun_logs.Parameters.AddWithValue("$ID", zapo.ID);
                racun_logs.Parameters.AddWithValue("$BrojRacuna", racun);
                racun_logs.ExecuteNonQuery();
                dodajNovac.ExecuteNonQuery();
            } catch(Exception e) {
                Console.WriteLine("Doslo je do greske prilikom dodavanja sredstava na racun, provjerite da li ste dobro unijeli podatke!");
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
                @"INSERT INTO Klienti VALUES('Ime', 'Prezime', 'Adresa', '359-758/452', " +
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
                  '6987453578', 'kIme2', 'Korisnicka Podrska', 'lozinka2');
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

        public void vratiSveIzBaze(string tabela)
        {
            var rez = new SQLiteCommand($"SELECT * FROM {tabela}", conn);
            using (var citac = rez.ExecuteReader())
            {
                while (citac.Read())
                {
                    for (int i = 0; i < citac.FieldCount; i++)
                    {
                        Console.Write($"{citac.GetName(i), -20}|");
                    }
                    Console.WriteLine();
                    for (int i = 0; i < citac.FieldCount; i++)
                    {
                        Console.Write($"{citac.GetValue(i), -20}|");
                    }
                }
            }
            Console.WriteLine("\n-------------------");

        }

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
            } catch
            {
                Console.WriteLine("Doslo je do greske prilikom skidanja sredstava datom korisniku!");
            }
        }

      
        //public static int getNextValue()
        //{
        //    int brojac;
        //    using (SQLiteDataReader rezultat = brojacSeq.ExecuteReader())
        //    {
        //        brojac = Convert.ToInt32(rezultat["next_value"]);
        //    }
        //    Console.WriteLine("OVDJE JE BROJAC " + brojac);
        //    return brojac;
        //}

    }
}
