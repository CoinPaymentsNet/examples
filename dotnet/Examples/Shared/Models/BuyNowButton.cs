using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class CreateBuyNowButtonDto
    {
        public string? IPNUrl { get; set; }

        public string ButtonStyle { get; set; } = "blue";

        public string ButtonWidth { get; set; } = "210";

        public string BuyerDataCollectionMessage { get; set; } = null!;

        public bool EmailNotifications { get; set; }

        public bool IsEmailDelivery { get; set; }

        public bool RequireBuyerNameAndEmail { get; set; }

        public string? SuccessUrl { get; set; }

        public string? CancelUrl { get; set; }

        public required CreateBuyNowButtonAmountDto Amount { get; set; } = null!;

        public required CreateBuyNowButtonItem[] Items { get; set; } = null!;

        public static CreateBuyNowButtonDto CreateDefault(int amount)
        {
            return new CreateBuyNowButtonDto
            {
                Amount = new CreateBuyNowButtonAmountDto
                {
                    BreakDown = new BreakDownDto
                    {
                        Handling = new AmountWithCurrencyDto(),
                        Shipping = new AmountWithCurrencyDto(),
                        Subtotal = new AmountWithCurrencyDto { Value = amount },
                        TaxTotal = new AmountWithCurrencyDto(),
                        Value = amount
                    },
                    Value = amount
                },
                Items = new[]
                {
                new CreateBuyNowButtonItem
                {
                    Amount = new AmountWithCurrencyDto
                    {
                        Value = amount,
                    },
                    Name = "Test item",
                    Quantity = new CreateBuyNowButtonItemQuantity(),
                    Tax = new AmountWithCurrencyDto()
                }
            }
            };
        }
    }

    public class CreateBuyNowButtonItem
    {
        public required string Name { get; set; } = null!;

        public required AmountWithCurrencyDto Amount { get; set; } = null!;

        public required CreateBuyNowButtonItemQuantity Quantity { get; set; } = null!;

        public required AmountWithCurrencyDto Tax { get; set; } = null!;
    }

    public class CreateBuyNowButtonItemQuantity
    {
        public int Type { get; set; } = 2;

        public int Value { get; set; } = 1;
    }

    public class CreateBuyNowButtonAmountDto
    {
        public int CurrencyId { get; set; } = 5057;

        public int Value { get; set; } = 20_00;

        public required BreakDownDto BreakDown { get; set; } = null!;
    }

    public class BreakDownDto
    {
        public required AmountWithCurrencyDto Handling { get; set; } = null!;

        public required AmountWithCurrencyDto Shipping { get; set; } = null!;

        public required AmountWithCurrencyDto Subtotal { get; set; } = null!;

        public required AmountWithCurrencyDto TaxTotal { get; set; } = null!;

        public int CurrencyId { get; set; } = 5057;

        public required int Value { get; set; }
    }

    public class AmountWithCurrencyDto
    {
        public int CurrencyId { get; set; } = 5057;

        public int Value { get; set; } = 0;
    }

}
