using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.OrderModels;
public class Order : BaseEntity<Guid>
{

    public Order()
    {
        
    }

    public Order(string userEmail, Address shippingAddress, ICollection<OrderItem> orderItems, DeliveryMethod deliveryMethod, decimal subTotal, string paymentIntentId)
    {
        Id = Guid.NewGuid();
        UserEmail = userEmail;
        ShippingAddress = shippingAddress;
        OrderItems = orderItems;
        DeliveryMethod = deliveryMethod;
        SubTotal = subTotal;
        PaymentIntentId = paymentIntentId;
    }




    //Id
    //UserEmail
    public string UserEmail { get; set; }

    //Shipping Address
    public Address ShippingAddress { get; set; }

    //Order Items
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Navigational Property

    //Delivery Method
    public DeliveryMethod DeliveryMethod { get; set; }//Navigational Property

    public int? DeliveryMethodId { get; set; } // FK


    //Payment Status
    public OrderPaymentStatus PaymentStatus { get; set; } = OrderPaymentStatus.Pending;

    //sub total

    public decimal SubTotal { get; set; }


    //Order Date

    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;

    //Payment
    public string PaymentIntentId { get; set; }



}
