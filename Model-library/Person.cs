using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_library
{
    public class Person 
    {
        public string lastName { get; set; }
        public string firstName { get; set; }
        public int keyLength { get; set; }
        public string keyUsed { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public Algorithm typeAlgo { get; set; }
        public Company company { get; set; }
    }
}
