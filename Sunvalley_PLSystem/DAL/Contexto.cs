using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Sunvalley_PLSystem.DAL
{
    public class Contexto: DbContext
    {
        public Contexto() : base("ConexionSunValley")
        {

        }


    }
}