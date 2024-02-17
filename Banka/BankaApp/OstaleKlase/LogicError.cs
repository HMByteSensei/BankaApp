using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class LogicError : Exception
{
    public LogicError(string poruka) : base(poruka) { }
}

