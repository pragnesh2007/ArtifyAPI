using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("checkout")]
public class CheckoutController : ControllerBase
{
    private readonly IStripeClient _stripeClient;

    public CheckoutController(IStripeClient stripeClient)
    {
        _stripeClient = stripeClient;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CartPayload payload)
    {
        var lineItems = new List<SessionLineItemOptions>();

        foreach (var item in payload.Items)
        {
            lineItems.Add(new SessionLineItemOptions
            {
                Price = item.Id,
                Quantity = item.Quantity
            });
        }

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = "http://localhost:3000/success", // Replace with your success URL
            CancelUrl = "http://localhost:3000/cancel"    // Replace with your cancel URL
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return Ok(new { Url = session.Url });
    }
}

public class CartItem
{
    public string Id { get; set; }
    public int Quantity { get; set; }
}

public class CartPayload
{
    public List<CartItem> Items { get; set; }
}
