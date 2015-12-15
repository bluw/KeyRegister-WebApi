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
        public IHttpActionResult updatePersonInDatabase(string email, Person newPerson)
        {
            var person = new person();
            fillPersonGoingToDatabase(person, newPerson);

            if (email != person.email) {
                return BadRequest();
            }

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            db.Entry(person).State = EntityState.Modified;

            try {
                db.SaveChanges();
            } catch (DbUpdateConcurrencyException) {
                if (!personExists(email)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost, ActionName("addPerson")]
        public IHttpActionResult addPersonInDatabase(Person newPerson)
        {
            try {
                if (!ModelState.IsValid) {
                    return BadRequest(ModelState);
                }

                var person = new person();
                fillPersonGoingToDatabase(person, newPerson);
                db.people.Add(person);

                try {
                    db.SaveChanges();
                } catch (DbUpdateException) {
                    if (personExists(person.email)) {
                        return Conflict();
                    } else {
                        throw;
                    }
                }

                return CreatedAtRoute("DefaultApi", new { id = person.email }, person);

            } catch (Exception ex) {

                string message = "";
                var innerEx = ex;
                while (innerEx != null) {
                    message = message + innerEx.Message;
                    innerEx = innerEx.InnerException;
                }

                return BadRequest(message);
            }

        }

        [HttpGet, ActionName("searchPersonByName")]
        public IEnumerable<Person> searchPersonByNameInDatabase(string lastName)
        {
            var queryResult = (from p
                               in db.people
                               where p.lastName == lastName
                               select p).ToList();

            List<Person> list = new List<Person>();

            foreach (var p in queryResult) {
                Person person = new Person();
                fillPerson(person, p);
                list.Add(person);
            }

            return (list.ToArray());
        }

        [HttpGet, ActionName("searchPersonByEmail")]
        public Person searchPersonByEmailInDatabase(string email)
        {
            var queryResult = (from p
                               in db.people
                               where p.email == email
                               select p).Single();

            Person person = new Person();
            fillPerson(person, queryResult);

            return person;
        }

        [HttpGet, ActionName("searchPersonByCompany")]
        public IEnumerable<Person> searchPersonByCompanyInDatabase(string nameCompany)
        {
            int idCompany = searchCompany(nameCompany);

            var queryResult = (from p
                               in db.people
                               where p.FK_company == idCompany
                               select p).ToList();

            List<Person> list = new List<Person>();

            foreach (var p in queryResult) {
                Person person = new Person();
                fillPerson(person, p);
                list.Add(person);
            }

            return (list.ToArray());
        }

        [HttpGet, ActionName("getFavorite")]
        public IEnumerable<Person> getFavoriteFromDatabase(string email)
        {
            var queryResult = (from p in db.people
                               join f in db.favorites
                               on p.email equals f.personFavorite
                               where f.personWithFavorite == email
                               select p).ToList();

            List<Person> list = new List<Person>();

            foreach (var p in queryResult) {
                Person person = new Person();
                fillPerson(person, p);
                list.Add(person);
            }

            return (list.ToArray());
        }

        [HttpPost, ActionName("addFavorite")]
        public IHttpActionResult addFavoriteInDatabase(string email, string emailFavorite)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (favoriteLinkExists(email, emailFavorite)) {
                return Conflict();
            }

            var favorite = new favorite();
            favorite.personWithFavorite = email;
            favorite.personFavorite = emailFavorite;

            db.favorites.Add(favorite);

            try {
                db.SaveChanges();
            } catch (DbUpdateException) {
                throw;
            }

            return CreatedAtRoute("DefaultApi", new { id = favorite.personWithFavorite }, favorite);
        }

        [HttpDelete, ActionName("deleteFavorite")]
        public IHttpActionResult DeleteFavoriteFromDatabase(string email, string emailFavorite)
        {
            var favorite = (from f
                            in db.favorites
                            where f.personWithFavorite == email
                            && f.personFavorite == emailFavorite
                            select f).Single();

            if (favorite == null) {
                return NotFound();
            }

            db.favorites.Remove(favorite);
            db.SaveChanges();

            return Ok(favorite);
        }

        [HttpDelete, ActionName("deleteAccount")]
        public IHttpActionResult Deleteperson(string email)
        {
            person person = db.people.Find(email);
            if (person == null) {
                return NotFound();
            }

            db.people.Remove(person);
            db.SaveChanges();

            return Ok(person);
        }

        private void fillPerson(Person person, person p)
        {
            person.Email = p.email;
            person.Password = p.password;
            person.LastName = p.lastName;
            person.FirstName = p.firstName;
            person.KeyUsed = p.keyUsed;
            person.KeyLength = p.keyLength;
            person.Company.IdCompany = p.FK_company;
            person.Company.NameCompany = p.company.nameCompany;
            person.TypeAlgo.IdAlgorithm = p.FK_algorithm;
            person.TypeAlgo.Type = p.algorithm.type;
        }

        private void fillPersonGoingToDatabase(person p, Person newPerson)
        {
            p.email = newPerson.Email;
            p.password = newPerson.Password;
            p.lastName = newPerson.LastName;
            p.firstName = newPerson.FirstName;
            p.keyUsed = newPerson.KeyUsed;
            p.keyLength = newPerson.KeyLength;
            int idCompany = searchCompany(newPerson.Company.NameCompany);
            p.FK_company = idCompany;
            int idAlgo = searchAlgo(newPerson.TypeAlgo.Type);
            p.FK_algorithm = idAlgo;
        }

        private int searchCompany(string nameCompany)
        {
            int idCompany;

            try {
                var company = (from c
                               in db.companies
                               where c.nameCompany == nameCompany
                               select c).Single();

                idCompany = company.idCompany;

            } catch (Exception e) { // exception throw by .Single() if there isnt one row
                                    // that means the company doesnt exist in the DB
                idCompany = addCompanyInDatabase(nameCompany);
            }

            return idCompany;
        }

        private int searchAlgo(string type)
        {
            int id;

            try {
                var algo = (from a
                            in db.algorithms
                            where a.type == type
                            select a).Single();

                id = algo.idAlgorithm;

            } catch (Exception e) {
                throw;
            }

            return id;
        }

        private int addCompanyInDatabase(string nameCompany)
        {
            var company = new company();
            company.nameCompany = nameCompany;

            db.companies.Add(company);

            try {
                db.SaveChanges();
            } catch (DbUpdateException) {
                throw;
            }

            return db.companies.Last().idCompany;
        }

        private bool personExists(string id)
        {
            return db.people.Count(e => e.email == id) > 0;
        }

        private bool favoriteLinkExists(string email, string emailFavorite)
        {
            return db.favorites.Count(e => e.personWithFavorite == email && e.personFavorite == emailFavorite) > 0;
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