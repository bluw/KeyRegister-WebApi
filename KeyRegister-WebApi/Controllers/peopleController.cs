using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using KeyRegister_WebApi.Models;
using Model_library;

namespace KeyRegister_WebApi.Controllers
{
    public class peopleController : ApiController
    {
        private KeyRegisterEntities db = new KeyRegisterEntities();

        [HttpPut, ActionName("updatePerson")]
        public void updatePersonInDatabase(string email, string newPersonJSON)
        {
            Person newPerson = Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(newPersonJSON);
            
            var query = (from p
                            in db.people
                            where p.email == email
                            select p).Single();

            fillPersonToDatabase(query, newPerson);

            db.SaveChanges();
        }

        [HttpPost, ActionName("addPerson")]
        public void addPersonInDatabase(string newPersonJSON)
        {
            Person newPerson = Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(newPersonJSON);

                            // class starting with a lower character -> come from table name in database
            var query = new person();
            fillPersonToDatabase(query, newPerson);

            db.people.Add(query);
            db.SaveChanges();
        }

        [HttpGet, ActionName("searchPersonByName")]
        public IEnumerable<Person> searchPersonByNameInDatabase(string lastName)
        {
            List<Person> list = new List<Person>();

            var query = (from p
                         in db.people
                         where p.lastName == lastName
                         select p).ToList();

            foreach (var p in query) {
                Person person = new Person();
                fillPerson(person, p);
                list.Add(person);
            }

            return (list.ToArray());
        }

        [HttpGet, ActionName("searchPersonByEmail")]
        public Person searchPersonByEmailInDatabase(string email)
        {
            Person person = new Person();

            var query = (from p
                         in db.people
                         where p.email == email
                         select p).Single();

            fillPerson(person, query);

            return person;
        }

        [HttpGet, ActionName("searchPersonByCompany")]
        public IEnumerable<Person> searchPersonByCompanyInDatabase(string nameCompany)
        {
            int idCompany = searchCompany(nameCompany);
            List<Person> list = new List<Person>();

            var query = (from p
                         in db.people
                         where p.FK_company == idCompany
                         select p).ToList();

            foreach (var p in query) {
                Person person = new Person();
                fillPerson(person, p);
                list.Add(person);
            }

            return (list.ToArray());
        }

        [HttpGet, ActionName("getFavorite")]
        public IEnumerable<Person> getFavoriteFromDatabase(string email)
        {
            List<Person> list = new List<Person>();

            var query = (from p in db.people
                         join f in db.favorites
                         on p.email equals f.personFavorite
                         where p.email == email
                         select p).ToList();

            foreach (var p in query) {
                Person person = new Person();
                fillPerson(person, p);
                list.Add(person);
            }

            return (list.ToArray());
        }

        private void fillPerson(Person person, person p)
        {
            person.Email = p.email;
            person.Password = p.password;
            person.LastName = p.lastName;
            person.FirstName = p.firstName;
            person.KeyUsed = p.keyUsed;
            person.KeyLength = p.keyLength;
            int idCompany = searchCompany(p.company.nameCompany);
            person.Company.IdCompany = idCompany;
            person.Company.NameCompany = p.company.nameCompany;
            person.TypeAlgo.IdAlgorithm = p.FK_algorithm;
            person.TypeAlgo.Type = p.algorithm.type;
        }

        private void fillPersonToDatabase(person p, Person newPerson)
        {
            p.email = newPerson.Email;
            p.password = newPerson.Password;
            p.lastName = newPerson.LastName;
            p.firstName = newPerson.FirstName;
            p.keyUsed = newPerson.KeyUsed;
            p.keyLength = newPerson.KeyLength;
            p.FK_company = newPerson.Company.IdCompany;
            p.FK_algorithm = newPerson.TypeAlgo.IdAlgorithm;
        }

        private int searchCompany(string nameCompany)
        {
            int idCompany;

            try {
                var query = (from c
                             in db.companies
                             where c.nameCompany == nameCompany
                             select c).Single();

                idCompany = query.idCompany;

            } catch (Exception e) { // exception throw by .Single() if there isnt one row
                                    // that means the company doesnt exist in the DB
                idCompany = addCompanyInDatabase(nameCompany);
            }

            return idCompany;
        }

        private int addCompanyInDatabase(string nameCompany)
        {
            var query = new company();
            query.nameCompany = nameCompany;

            db.companies.Add(query);
            db.SaveChanges();
            return db.companies.Count(); //meh . . . 
        }

        /*
        // GET: api/people
        public IQueryable<person> Getpeople()
        {
            return db.people;
        }

        // GET: api/people/5
        [ResponseType(typeof(person))]
        public IHttpActionResult Getperson(string id)
        {
            person person = db.people.Find(id);
            if (person == null)
            {
                return NotFound();
            }

            return Ok(person);
        }

        // PUT: api/people/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putperson(string id, person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != person.email)
            {
                return BadRequest();
            }

            db.Entry(person).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!personExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/people
        [ResponseType(typeof(person))]
        public IHttpActionResult Postperson(person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.people.Add(person);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (personExists(person.email))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = person.email }, person);
        }

        // DELETE: api/people/5
        [ResponseType(typeof(person))]
        public IHttpActionResult Deleteperson(string id)
        {
            person person = db.people.Find(id);
            if (person == null)
            {
                return NotFound();
            }

            db.people.Remove(person);
            db.SaveChanges();

            return Ok(person);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool personExists(string id)
        {
            return db.people.Count(e => e.email == id) > 0;
        }
        */
    }
}