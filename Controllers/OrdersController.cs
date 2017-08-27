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
using TraballhoDM106.br.com.correios.ws;
using TraballhoDM106.CRMClient;
using TraballhoDM106.Models;

namespace TraballhoDM106.Controllers
{
    [RoutePrefix("api/orders")]
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
            string cepOrigem = "69096010";
            string frete;
            string cepDestino;
            decimal peso = 0;
            int formato = 1;
            decimal comprimento = 0;
            decimal altura = 0;
            decimal largura = 0;
            decimal diamentro = 0;
            string entregaMaoPropria = "N";
            //decimal valorDeclarado= 0;
            string avisoRecebimento = "S";
            decimal shipping;


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

            if (order.OrderItems.Count == 0)
            {
                return BadRequest("Pedido sem itens");
            }
            ICollection<OrderItem> produtos = order.OrderItems;
            CRMRestClient crmClient = new CRMRestClient();
            Customer customer = crmClient.GetCustomerByEmail(User.Identity.Name);
            if (customer != null)
            {
                cepDestino = customer.zip;
            }
            else
            {
                return BadRequest("Falha ao	consultar o	CRM");
            }


            foreach (OrderItem item in produtos)
            {
                Product product = db.Products.Find(item.ProductId);
                peso = (item.Quantity * product.weight) + peso;
                comprimento = (item.Quantity * product.lenght) + comprimento;
                altura = (item.Quantity * product.height) + altura;
                largura = (item.Quantity * product.width) + largura;
                diamentro = (item.Quantity * product.diameter) + diamentro;
               order.Value = (item.Quantity * order.Value) + product.price;
            }
            order.Weight = peso;

            
            CalcPrecoPrazoWS correios = new CalcPrecoPrazoWS();
            cResultado resultado = correios.CalcPrecoPrazo("", "", "40010", cepOrigem, cepDestino, Convert.ToString(peso), formato, Decimal.ToInt32(comprimento), Decimal.ToInt32(altura), Decimal.ToInt32(largura), Decimal.ToInt32(diamentro), entregaMaoPropria, Decimal.ToInt32(order.Value), avisoRecebimento);
            if (resultado.Servicos[0].Erro.Equals("0"))
            {
                frete = "Valor	do	frete:	" + resultado.Servicos[0].Valor + "	-	Prazo	de	entrega:	" + resultado.Servicos[0].PrazoEntrega + "	dia(s)";
                shipping = Convert.ToDecimal(resultado.Servicos[0].Valor);
                order.DeliveryDate = order.DateOrder.AddDays(Int32.Parse(resultado.Servicos[0].PrazoEntrega));
            }
            else
            {
                return BadRequest("Código	do	erro:	" + resultado.Servicos[0].Erro + "-" + resultado.Servicos[0].MsgErro);
            }

            

            if (!order.Status.Equals("NOVO"))
            {
                BadRequest("Pedido com Status diferente de 'NOVO'");
            }

            if (id != order.Id)
            {
                return BadRequest();
            }
            order.ShippingPrice = shipping;
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
            order.DateOrder = DateTime.Today;
            order.DeliveryDate = DateTime.Today;
            order.userName = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }            

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.Id }, order);
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

        [ResponseType(typeof(string))]
        [HttpGet]
        [Route("frete")]
        public IHttpActionResult CalculaFrete()
        {
            string frete;
            CalcPrecoPrazoWS correios = new CalcPrecoPrazoWS();
            cResultado resultado = correios.CalcPrecoPrazo("", "","40010", "37540000", "37002970", "1", 1, 30, 30, 30, 30, "N", 100, "S");
            if (resultado.Servicos[0].Erro.Equals("0"))
            {
                frete = "Valor	do	frete:	" + resultado.Servicos[0].Valor + "	-	Prazo	de	entrega:	" + resultado.Servicos[0].PrazoEntrega + "	dia(s)";
                return Ok(frete);
            }
            else
            {
                return BadRequest("Código	do	erro:	" + resultado.Servicos[0].Erro + "-" + resultado.Servicos[0].MsgErro);
            }
        }


        [ResponseType(typeof(string))]
        [HttpGet]
        [Route("cep")]
        public IHttpActionResult ObtemCEP()
        {
            CRMRestClient crmClient = new CRMRestClient();
            Customer customer = crmClient.GetCustomerByEmail(User.Identity.Name);
            if (customer != null)
            {
                return Ok(customer.zip);
            }
            else
            {
                return BadRequest("Falha ao	consultar o	CRM");
            }
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