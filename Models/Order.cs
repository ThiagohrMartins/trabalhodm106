using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TraballhoDM106.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }
        public int Id { get; set; }
        public string userName { get; set; }
        public decimal ShippingPrice { get; set; }
        public DateTime DateOrder { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Status { get; set; }
        public virtual ICollection<OrderItem> OrderItems
        {
            get; set;
        }
        public decimal Value {get;set;}
        public decimal Weight { get; set; }
    }
}