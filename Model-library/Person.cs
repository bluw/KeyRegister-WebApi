using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_library
{
    public class Person 
    {
        private string lastName;
        public string LastName { get; set; }

        private string firstName;
        public string FirstName { get; set; }

        private int keyLength;
        public int KeyLength { get; set; }

        private string keyUsed;
        public string KeyUsed { get; set; }

        private string email;
        public string Email { get; set; }

        private string password;
        public string Password { get; set; }

        private Algorithm typeAlgo;
        public Algorithm TypeAlgo { get; set; }

        private Company company;
        public Company Company { get; set; }

        public Person()
        {
            Company = new Company();
            TypeAlgo = new Algorithm();
        }
    }
}
