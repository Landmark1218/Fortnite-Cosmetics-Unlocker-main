using Fiddler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortnite_Cosmetics_Unlocker
{
    internal class Fiddler
    {
        public static bool Setup()
        {
            return CertMaker.createRootCert() && CertMaker.trustRootCert();
        }
    }
}
