using ConsolidationApp.Models.Invoice;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsolidationApp.Models.Merchant
{

    public class LineItemModel
    {
        public Guid InvoiceId { get; set; }

        /// <summary>
        /// the unique id of the line item (PK)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// the API caller provided external ID for the item.  Appears on the Merchant dashboard and reports only.
        /// </summary>
        public string? CustomId { get; set; }

        /// <summary>
        /// the stock keeping unit (SKU) of the item
        /// </summary>
        public string? SKU { get; set; }

        /// <summary>
        /// the name or title of the item
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// the detailed description of the item
        /// </summary>
        public string? Description { get; set; }

        public QuantityModel? Quantity { get; set; }

        public LineItemType Type { get; set; }

        /// <summary>
        /// the subtotal price of the item
        /// </summary>
        public AmountModel? Amount { get; set; }

        /// <summary>
        /// the original total price of the item if <see cref="Amount"/> represents a discounted price
        /// </summary>
        public AmountModel? OriginalAmount { get; set; }

        /// <summary>
        /// the total taxes charged on this item
        /// </summary>
        public decimal? Tax { get; set; }
    }
    public enum LineItemType
    {
        Hours = 1,
        Quantity = 2
    }
    public class QuantityModel
    {
        public int Value { get; set; }
        public string? Type { get; set; }
    }

    public class AmountModel
    {
        public string CurrencyId { get; set; }
        public string DisplayValue { get; set; }
        public string Value { get; set; }
    }
    public sealed class LineItemDto
    {
        /// <summary>
        /// the API caller provided external ID for the item.  Appears on the Merchant dashboard and reports only.
        /// </summary>
        [StringLength(127)]
        public string? CustomId { get; set; }

        /// <summary>
        /// the stock keeping unit (SKU) of the item
        /// </summary>
        [StringLength(127)]
        public string? SKU { get; set; }

        /// <summary>
        /// the name or title of the item
        /// </summary>
        [Required, StringLength(127)]
        public string? Name { get; set; }

        /// <summary>
        /// the detailed description of the item
        /// </summary>
        [StringLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// the quantity of the item.  Must be greater than 0 and less than 999,999,999‬
        /// </summary>
        [Required]
        public required LineItemQuantityDto Quantity { get; set; }

        /// <summary>
        /// the original total price of the item if <see cref="Amount"/> represents a discounted price
        /// </summary>
        /// <remarks>
        /// the UI can display the discounted price on the cart / checkout screens for this item
        /// </remarks>
        public InvoiceMoneyDto? OriginalAmount { get; set; }

        /// <summary>
        /// the subtotal price of the item (note: this is not a per unit price but the total price for the total quantity)
        /// </summary>
        public required InvoiceMoneyDto Amount { get; set; }

        /// <summary>
        /// the total taxes charged on this item
        /// </summary>     
        public InvoiceMoneyDto? Tax { get; set; }

        /// <summary>
        /// Calculates the line item total (base amount + taxes + charges - discounts)
        /// </summary>
        public bool TryGetTotal(out decimal total)
        {
            if (!decimal.TryParse(Amount.Value, out total))
            {
                return false;
            }

            if (Tax != null)
            {
                if (!decimal.TryParse(Tax.Value, out var taxAmount))
                {
                    return false;
                }
                total += taxAmount;
            }

            return true;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IEnumerable<ValidationResult> GetNotSameCurrencyValidationResults(string expectedCurrencyId, params InvoiceMoneyDto?[] moneys)
            {
                foreach (var money in moneys)
                {
                    if (money == null)
                    {
                        continue;
                    }

                    if (money.CurrencyId != expectedCurrencyId)
                    {
                        yield return new ValidationResult($"All item monetary fields (charges, discounts, taxes) must have the same currency as the items base amount, expected currency '{expectedCurrencyId}' but found '{money.CurrencyId}'");
                    }
                }
            }

            foreach (var result in GetNotSameCurrencyValidationResults(Amount.CurrencyId, OriginalAmount, Tax))
            {
                yield return result;
            }
        }
    }
    public class LineItemQuantityDto : IValidatableObject
    {
        /// <summary>
        /// the quantity of the item.  Must be greater than 0 and less than 999,999,999‬.
        /// defaults to 1 if not provided.
        /// </summary>
        [Range(1, 999_999_999)]
        public int Value { get; set; } = 1;

        public LineItemQuantityTypeDto Type { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Enum.GetValues<LineItemQuantityTypeDto>().Contains(Type))
            {
                var allowedValues = string.Join(", ", Enum.GetValues<LineItemQuantityTypeDto>().Select(x => $"{(int)x} ({x.ToString()})"));
                yield return new ValidationResult($"Wrong value {Type} for item type. Allowed values are: {allowedValues}", [nameof(Type)]);
            }
        }
    }

    public enum LineItemQuantityTypeDto
    {
        Hours = 1,
        Quantity = 2
    }
    public sealed class InvoiceAmountDto : InvoiceMoneyDto
    {
        /// <summary>
        /// the breakdown of the amount, providing details such as total item amount, total tax amount, shipping, 
        /// handling, insurance, and discounts, if any.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public InvoiceAmountBreakdownDto? Breakdown { get; set; }
    }
}
