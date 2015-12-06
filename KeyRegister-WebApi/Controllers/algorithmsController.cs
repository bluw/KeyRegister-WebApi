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
    public class algorithmsController : ApiController
    {
        private KeyRegisterEntities db = new KeyRegisterEntities();

        [HttpGet, ActionName("getAlgorithm")]
        public IEnumerable<Algorithm> getAlgorithm()
        { 
            List<Algorithm> list = new List<Algorithm>();

            var algos = (from a
                         in db.algorithms
                         select a).ToList();

            foreach (var a in algos) {
                Algorithm algo = new Algorithm();
                algo.IdAlgorithm = a.idAlgorithm;
                algo.Type = a.type;
                list.Add(algo);
            }

            return (list.ToArray());
        }

        /*
        private KeyRegisterEntities db = new KeyRegisterEntities();

        // GET: api/algorithms
        public IQueryable<algorithm> Getalgorithms()
        {
            return db.algorithms;
        }

        // GET: api/algorithms/5
        [ResponseType(typeof(algorithm))]
        public IHttpActionResult Getalgorithm(int id)
        {
            algorithm algorithm = db.algorithms.Find(id);
            if (algorithm == null)
            {
                return NotFound();
            }

            return Ok(algorithm);
        }

        // PUT: api/algorithms/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putalgorithm(int id, algorithm algorithm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != algorithm.idAlgorithm)
            {
                return BadRequest();
            }

            db.Entry(algorithm).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!algorithmExists(id))
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

        // POST: api/algorithms
        [ResponseType(typeof(algorithm))]
        public IHttpActionResult Postalgorithm(algorithm algorithm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.algorithms.Add(algorithm);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = algorithm.idAlgorithm }, algorithm);
        }

        // DELETE: api/algorithms/5
        [ResponseType(typeof(algorithm))]
        public IHttpActionResult Deletealgorithm(int id)
        {
            algorithm algorithm = db.algorithms.Find(id);
            if (algorithm == null)
            {
                return NotFound();
            }

            db.algorithms.Remove(algorithm);
            db.SaveChanges();

            return Ok(algorithm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool algorithmExists(int id)
        {
            return db.algorithms.Count(e => e.idAlgorithm == id) > 0;
        }*/
    }
}