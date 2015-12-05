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

            // class starting with a lower character -> come from table name in database
            person query = (from p
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

            person query = new person();
            fillPersonToDatabase(query, newPerson);

            db.people.Add(query);
            db.SaveChanges();
        }

        [HttpGet, ActionName("searchPersonByName")]
        public IEnumerable<Person> searchPersonByNameInDatabase(string lastName)
        {
            List<Person> list = new List<Person>();

            List<person> query = (from p
                                  in db.people
                                  where p.lastName == lastName
                                  select p).ToList();

            foreach (person p in query) {
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

            person query = (from p
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

            List<person> query = (from p
                                  in db.people
                                  where p.FK_company == idCompany
                                  select p).ToList();

            foreach (person p in query) {
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

            List<person> query = (from p in db.people
                                  join f in db.favorites
                                  on p.email equals f.personFavorite
                                  where p.email == email
                                  select p).ToList();

            foreach (person p in query) {
                Person person = new Person();
                fillPerson(person, p);
                list.Add(person);
            }

            return (list.ToArray());
        }

        private void fillPerson(Person person, person p)
        {
            person.email = p.email;
            person.lastName = p.lastName;
            person.firstName = p.firstName;
            person.keyUsed = p.keyUsed;
            person.keyLength = p.keyLength;
            int idCompany = searchCompany(p.company.nameCompany); //in case he changed company, need to check if it exists or not
            person.company.idCompany = idCompany;
            person.company.nameCompany = p.company.nameCompany;
            person.typeAlgo.idAlgorithm = p.FK_algorithm;
            person.typeAlgo.type = p.algorithm.type;
        }

        private void fillPersonToDatabase(person p, Person newPerson)
        {
            p.email = newPerson.email;
            p.password = newPerson.password;
            p.lastName = newPerson.lastName;
            p.firstName = newPerson.firstName;
            p.keyUsed = newPerson.keyUsed;
            p.keyLength = newPerson.keyLength;
            p.FK_company = newPerson.company.idCompany;
            p.FK_algorithm = newPerson.typeAlgo.idAlgorithm;
        }

        private int searchCompany(string nameCompany)
        {
            int idCompany;

            try {
                company query = (from c
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
            company query = new company();
            query.nameCompany = nameCompany;

            db.companies.Add(query);
            db.SaveChanges();
            return db.companies.Count(); //meh . . . 
        }

        private string getTypeAlgo(int idAlgo)
        {
            string typeAlgo;

            algorithm query = (from a
                               in db.algorithms
                               where a.idAlgorithm == idAlgo
                               select a).Single();

            typeAlgo = query.type;

            return typeAlgo;
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