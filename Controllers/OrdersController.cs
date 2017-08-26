using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Description;
using TraballhoDM106.Models;

namespace TraballhoDM106.Controllers
{
    public class OrdersController : ApiController
    {
        private TraballhoDM106Context db = new TraballhoDM106Context();

        // GET: api/Orders
        [Authorize(Roles = "ADMIN")]
        public IQueryable<Order> GetOrders()
        {
            return db.Orders;
        }

        // GET: api/Orders/5
        [Authorize]
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {  
            Order order = db.Orders.Find(id);

            if (checkUserFromOrder(User, order))
            {

                if (order == null)
                {
                    return NotFound();
                }

                return Ok(order);
            }
            else
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("byemail")]
        public IHttpActionResult GetOrdersByEmail(string email)
        {
            
            var orders = db.Orders.Where(o => o.userName == email);
            if(checkUserFromEmail(User, email))
            {
                if (orders == null)
                {
                    return NotFound();
                }

                return Ok(orders);
            }

            return StatusCode(HttpStatusCode.Forbidden);
        }

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            order.Status = "Novo";
            order.Weight = 0;
            order.ShippingPrice = 0;
            order.Value = 0;
            order.DateOrder = new DateTime();
            order.DeliveryDate = new DateTime();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }            

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.Id }, order);
        }

        [Authorize]
        [HttpPut]
        [Route("CalculateShipping")]
        public IHttpActionResult CalculateShipping(int id)
        {
            Order order = db.Orders.Find(id);

            if (checkUserFromOrder(User, order))
            {
                if (order == null)
                {
                    return NotFound();
                }
            }
            else
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        [Authorize]
        [HttpPut]
        [Route("Checkout")]
        public IHttpActionResult Checkout(int id)
        {
            Order order = db.Orders.Find(id);

            if (checkUserFromOrder(User, order))
            {
                if (order == null)
                {
                    return NotFound();
                }
            }
            else
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            foreach(OrderItem item in order.OrderItems)
            {
                Product product =  db.Products.Find(item.Id);
                order.Value = order.Value + product.price;
            }

            order.Status = "Fechado";

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (checkUserFromOrder(User, order))
            {
                if (order == null)
                {
                    return NotFoundOrder();
                }
            }
            else
            {
                return Unauthorized("Usuário não pode deletar este pedido!");
            }
            db.Orders.Remove(order);
            db.SaveChanges();

            return Ok(order);
        }        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }

        private bool checkUserFromOrder(IPrincipal user, Order order)
        {
            return ((user.Identity.Name.Equals(order.userName)) || (user.IsInRole("ADMIN")));
        }

        private bool checkUserFromEmail(IPrincipal user, string email)
        {
            return ((user.Identity.Name.Equals(email)) || (user.IsInRole("ADMIN")));
        }
        private IHttpActionResult NotFoundOrder()
        {
            var msg = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Pedido não encontrado!"),
                ReasonPhrase = "Erro ao buscar pedido."
            };
            return ResponseMessage(msg);
        }
        private IHttpActionResult Unauthorized(string msg)
        {
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(msg),
                ReasonPhrase = "Acesso não autorizado"
            };
            return ResponseMessage(response);
        }
    }
}