using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praktika
{
    class Instances
    {
        private static PaymentDBEntities _db = null;
        public static PaymentDBEntities db {
            get
            {
                if (_db == null)
                {
                    _db = new PaymentDBEntities();
                }
                return _db;
            }
        }
    }
}
