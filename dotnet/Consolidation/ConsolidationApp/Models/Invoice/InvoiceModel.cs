using ConsolidationApp.Models.Buyer;
using ConsolidationApp.Models.Currencies;
using ConsolidationApp.Models.Merchant;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsolidationApp.Models.Invoice;

public class InvoiceModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public string? InvoiceIdSuffix { get; set; }
    public string? InvoiceId { get; set; }
    public DateTimeOffset? InvoiceDate { get; set; }
    public DateTimeOffset? Expires { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset? Confirmed { get; set; }
    public DateTimeOffset? Completed { get; set; }
    public Guid MerchantId { get; set; }
    public string? MetaSourceIntegration { get; set; }
    public Currency Currency { get; set; }
    public decimal Total { get; set; }
    public string? Description { get; set; }
    public string? NotesToRecipient { get; set; }
    public string? Notes { get; set; }
    public BuyerModel? Buyer { get; set; }
    public InvoiceState State { get; set; }
    public ShippingModel? Shipping { get; set; }
    public InvoicePaymentModel[] Payments { get; set; } = [];
    public EmailDeliveryModel? EmailDeliveryOptions { get; set; }
    public decimal? Subtotal { get; set; }
    public decimal? ShippingTotal { get; set; }
    public decimal? HandlingTotal { get; set; }
    public decimal? DiscountTotal { get; set; }
    public decimal? TaxTotal { get; set; }
    public DateTimeOffset? Cancelled { get; set; }
    public string? MetaSourceHostname { get; set; }
    public string? PONumber { get; set; }
    public string? TermsAndConditions { get; set; }
    public object? CustomData { get; set; }
    public string? BuyerDataCollectionMessage { get; set; }
    public MerchantOptionsModel? MerchantOptions { get; set; }
    public InvoiceFlags Flags { get; set; }
    public LineItemModel[] Items { get; set; } = [];
    public Guid? MerchantClientId { get; set; }
    public PayoutInfo? PayoutConfig { get; set; }
    public bool PartialAcceptAvailable { get; set; }

    public bool IsLifeTimeFinished { get; set; }
}

public sealed class InvoiceResponseDto
{
    private string? _invoiceId;

    /// <summary>
    /// the CoinPayments id for the invoice
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// the optional API caller provided external invoice number.  Appears in screens shown to the Buyer and emails sent.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), StringLength(maximumLength: 127)]
    public string? InvoiceId
    {
        get => !string.IsNullOrEmpty(InvoiceIdSuffix) ? $"{_invoiceId}-{InvoiceIdSuffix}" : _invoiceId;
        set => _invoiceId = value;
    }

    /// <summary>
    /// the optional numeric suffix for the <see cref="InvoiceId"/>. Used when the invoice is emailed to multiple customers
    /// </summary>
    [JsonIgnore]
    public string? InvoiceIdSuffix { get; set; }

    /// <summary>
    /// the timestamp when the invoice was created
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// the date of the invoice, either the system created date or custom date specified by the merchant
    /// </summary>
    public DateTimeOffset InvoiceDate { get; set; }

    /// <summary>
    /// optional due date of the invoice
    /// </summary>
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>
    /// the timestamp when the invoice was confirmed (paid)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? Confirmed { get; set; }

    /// <summary>
    /// the timestamp when the invoice was completed (paid out to the merchant and CoinPayments fees)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? Completed { get; set; }

    /// <summary>
    /// the timestamp when the invoice was manually cancelled. Applicable for payment invoices only
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? Cancelled { get; set; }

    /// <summary>
    /// the timestamp when the invoice expires. Applicable for checkout and POS invoices only
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? Expires { get; set; }

    /// <summary>
    /// the currency the invoice is in
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InvoiceCurrencyDto? Currency { get; set; }

    /// <summary>
    /// the merchant the invoice is for
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InvoiceMerchantDto? Merchant { get; set; }

    /// <summary>
    /// options to show/hide merchant information on an invoice, or include additional merchant information specific to an invoice
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InvoiceMerchantOptionsDto? MerchantOptions { get; set; }

    /// <summary>
    /// the buyer information, if not provided it will be requested during payment so that refunds can be sent if
    /// there is a problem with the payment.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BuyerDto? Buyer { get; set; }

    /// <summary>
    /// the purchase description
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), StringLength(maximumLength: 200)]
    public string? Description { get; set; }

    /// <summary>
    /// the optional array of items and/or services that a buyer intends to purchase from the merchant
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public LineItemDto[]? Items { get; set; }

    public bool ShouldSerializeItems() => Items != null && Items.Length > 0;

    /// <summary>
    /// the total amount of the invoice, with an optional breakdown that provides details, such as the total item
    /// amount, total tax amount, shipping, handling, insurance and discounts, if any
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required InvoiceAmountDto Amount { get; set; }

    /// <summary>
    /// the shipping method and address
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InvoiceShippingDetailDto? Shipping { get; set; }

    /// <summary>
    /// any custom data the caller wishes to attach to the invoice which will be sent back in notifications
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? CustomData { get; set; }

    /// <summary>
    /// the status of the invoice (including payments received and payments confirmed)
    /// </summary>
    public InvoiceStatus Status { get; set; }

    /// <summary>
    /// flag indicating whether a buyer name and email are required, they will be requested at checkout
    /// if not provider by the caller.  The <see cref="BuyerDataCollectionMessage"/> will be displayed
    /// to the buyer when prompted.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? RequireBuyerNameAndEmail { get; set; }

    /// <summary>
    /// the message to display when collecting buyer user data
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BuyerDataCollectionMessage { get; set; }

    /// <summary>
    /// notes for the merchant only, these are not visible to the buyers
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Notes { get; set; }

    /// <summary>
    /// any additional information to share with the buyer about the transaction
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? NotesToRecipient { get; set; }

    /// <summary>
    /// any terms and conditions, e.g. a cancellation policy
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TermsAndConditions { get; set; }

    /// <summary>
    /// the invoice email delivery options if the invoice is to be emailed
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InvoiceEmailDeliveryOptionsDto? EmailDelivery { get; set; }

    /// <summary>
    /// indicates if invoice was delivered by email or to be delivered by email
    /// </summary>
    public bool IsEmailDelivery { get; set; }

    /// <summary>
    /// the invoice metadata, the integration and host where the invoice was created
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InvoiceMetadataDto? Metadata { get; set; }

    public string? PONumber { get; set; }

    public InvoicePayoutsDetailsDto? PayoutDetails { get; set; }

    public MerchantPaymentSummaryDto[] Payments { get; set; } = Array.Empty<MerchantPaymentSummaryDto>();
    
    /// <summary>
    /// Invoice in finished state more than 90 days
    /// </summary>
    public bool IsLifeTimeFinished { get; set; }

    public bool ShouldSerializeEmailDelivery() =>
        !string.IsNullOrEmpty(EmailDelivery?.To) ||
        !string.IsNullOrEmpty(EmailDelivery?.Cc) ||
        !string.IsNullOrEmpty(EmailDelivery?.Bcc);

    public bool ShouldSerializeMetadata() =>
        !string.IsNullOrEmpty(Metadata?.Integration) ||
        !string.IsNullOrEmpty(Metadata?.Hostname);
}
