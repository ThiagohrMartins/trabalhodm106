﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TraballhoDM106.Models;

namespace TraballhoDM106.Controllers
{
    public class ProductsController : ApiController
    {
        private TraballhoDM106Context db = new TraballhoDM106Context();

        // GET: api/Products
        [Authorize]
        public IQueryable<Product> GetProducts()
        {
            return db.Products;
        }

        // GET: api/Products/5
        [Authorize]
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            if (ProductCodeExists(product.code))
            {
                return BadRequest("Product Code exist");
            }

            if (ProductModelExists(product.model))
            {
                return BadRequest("Product Model exist");
            }

            try
            {
                db.Products.Add(product);
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (ProductCodeExists(product.code) || ProductModelExists(product.model))
                {
                    return BadRequest("Modelo ou Código já existente");
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

        


        // DELETE: api/Products/5
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }

        private bool ProductCodeExists(string code)
        {
            return db.Products.Count(e => e.code == code) > 0;
        }

        private bool ProductModelExists(string model)
        {
            return db.Products.Count(e => e.model == model) > 0;
        }
    }
}