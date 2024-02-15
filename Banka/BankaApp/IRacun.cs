using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

/*  u sustini da bismo omogucim da klasa menadzer banke moze mijenjati 
 *  odredena polja, a da druge klase moge samo dobiti odredene informacije
 *  napravili smo interfejss
*/
interface IRacun
{
    string BBB { get; }
    string PPP { get; }
    public Zaposlenici.Racun izracunajIBANCC();
    double StanjeRacuna { get; set; }
}
