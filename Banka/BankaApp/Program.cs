// See https://aka.ms/new-console-template for more information
using BankaApp.BazaPodatakaModel;


Menu pocetak = new Menu();

while (true)
{
    BazaPodatakaDAO model = BazaPodatakaDAO.getInstance();

    Console.WriteLine("\tMENU");
    Console.WriteLine("Izaberite status(0 za izlaz): ");
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
                pocetak.zaposlenik(model);
                break;
            case 2:
                pocetak.klijent(model);
                break;
            case 8:
                model.obrisiPodatkeSvihTabela();
                break;
            case 9:
                model.kreirajBazu();
                break;
            case 10:
                model.vratiSveIzBaze("Klienti");
                model.vratiSveIzBaze("Racun");
                model.vratiSveIzBaze("Zaposlenici");
                model.vratiSveIzBaze("brojac_seq");
                model.vratiSveIzBaze("Logs");
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