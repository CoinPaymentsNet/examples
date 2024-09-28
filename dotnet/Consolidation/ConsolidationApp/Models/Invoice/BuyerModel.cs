using ConsolidationApp.Models.Invoice;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsolidationApp.Models.Buyer;

public class BuyerModel
{
    /// <summary>
    /// the company name of the buyer
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// the buyer's first name
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// the buyer's last name
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// the buyer's email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// the buyer's phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// the buyer's address
    /// </summary>
    public AddressModel? Address { get; set; }

    public bool HasData => CompanyName != null || FirstName != null || LastName != null || Email != null ||
                           PhoneNumber != null || Address is {HasData: true};
}
public class AddressModel
{

    /// <summary>
    /// the first line of the address. For example, number or street.
    /// </summary>
    /// <example>123 Fake street</example>
    public string? Address1 { get; set; }

    /// <summary>
    /// the second line of the address. For example, suite or apartment number.
    /// </summary>
    /// <example>Apartment 42</example>
    public string? Address2 { get; set; }

    /// <summary>
    /// the third line of the address, if needed. For example, a street complement for Brazil, direction text 
    /// such as 'next to Walmart', or a landmark in an Indian address.
    /// </summary>
    public string? Address3 { get; set; }

    /// <summary>
    /// the highest level sub-division in a country, which is usually a province, state, or ISO-3166-2 subdivision.
    ///
    /// Format for postal delivery. For example, `CA` instead of `California`
    /// 
    ///  - UK: county
    ///  - US: state
    ///  - Canada: province
    ///  - Japan: prefecture
    ///  - Switzerland: kanton
    /// </summary>
    /// <example>BC</example>
    public string? ProvinceOrState { get; set; }

    /// <summary>
    /// the city, town or village
    /// </summary>
    /// <example>Vancouver</example>
    public string? City { get; set; }

    /// <summary>
    /// the neighborhood, suburb or district.
    ///   - Brazil: Suburb, bairoo, or neighborhood
    ///   - India: Sub-locality or district, Street name information is not always available but a sub-locality
    ///            or district can be a very small area
    /// </summary>
    public string? SuburbOrDistrict { get; set; }

    /// <summary>
    /// the two-character IS0-3166-1 country code
    /// </summary>
    /// <example>CA</example>
    public string? CountryCode { get; set; }

    /// <summary>
    /// the postal code, which is the zip code or equivalent.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// value indicating whether any data has been set on the object
    /// </summary>
    public bool HasData =>
        Address1 != null || Address2 != null || Address3 != null || ProvinceOrState != null ||
        City != null || SuburbOrDistrict != null || CountryCode != null || PostalCode != null;
}
public class ShippingModel
{

    /// <summary>
    /// the shipping method
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// the company name of the party to ship the items to
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// the first name of the party to ship the items to
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// the last name of the party to ship the items to
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// the email of the party to ship the items to
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// the phone number of the party to ship the items to
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// the address of the party to ship the items to
    /// </summary>
    public AddressModel? Address { get; set; }

    /// <summary>
    /// value indicating whether any data has been set on the object
    /// </summary>
    public bool HasData => Method != null || CompanyName != null || FirstName != null || LastName != null
                           || Email != null || PhoneNumber != null || Address is {HasData: true};
}
public sealed class InvoiceShippingDetailDto
{
    /// <summary>
    /// the shipping method
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), StringLength(maximumLength: 127)]
    public string? Method { get; set; }

    /// <summary>
    /// the company name of the party to ship the items to
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? CompanyName { get; set; }

    /// <summary>
    /// the name of the party to ship the items to
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public FullNameDto? Name { get; set; }

    /// <summary>
    /// the email address of the party to ship the items to
    /// </summary>
    /// <example>customer@example.com</example>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), EmailAddress]
    public string? EmailAddress { get; set; }

    /// <summary>
    /// the phone number
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), RegularExpression(@"^[.()\+\-0-9 ]*$", MatchTimeoutInMilliseconds = 1000)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// the address of the party to ship the items to
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public AddressDto? Address { get; set; }

    public bool HasData => Method != null || Name?.FirstName != null || Name?.LastName != null || 
                           CompanyName != null || EmailAddress != null || PhoneNumber != null || Address != null;
}

public class InvoiceEmailDeliveryOptionsDto
{
    /// <summary>
    /// the email `to` field, multiple addresses separated by semicolons
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? To { get; set; }

    /// <summary>
    /// the email `cc` field, multiple addresses separated by semicolons
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Cc { get; set; }

    /// <summary>
    /// the email `bcc` field, multiple addresses separated by semicolons
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Bcc { get; set; }
}

public sealed class InvoiceMetadataDto
{
    /// <summary>
    /// the integration from which the invoice was created
    /// </summary>
    public string? Integration { get; set; }

    /// <summary>
    /// the hostname on which the invoice was created
    /// </summary>
    public string? Hostname { get; set; }
}