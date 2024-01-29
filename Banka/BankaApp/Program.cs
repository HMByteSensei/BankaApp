// See https://aka.ms/new-console-template for more information
//MenadzerBanke m = new MenadzerBanke("I", "P", "adresa", "05478", "mail", "23wqi", "kime", "l");
IRacun iBr = new MenadzerBanke.Racun();
//MenadzerBanke.setBBB("956");
//Console.WriteLine(iBr.izracunajIBANCC());

Klient k1 = new Klient("I", "P", "adresa", "05478", "mail", "23wqi", "kime", "l", iBr.izracunajIBANCC());
Klient k2 = new Klient("k2", "P2", "adresa2", "2", "mail2", "23wqi2", "kime2", "l2", iBr.izracunajIBANCC());

Console.WriteLine(k1.BrojRacuna + " " + k2.BrojRacuna);