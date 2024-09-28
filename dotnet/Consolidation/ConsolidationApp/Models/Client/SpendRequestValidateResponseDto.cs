using System.Text.Json.Serialization;

namespace ConsolidationApp.Models.Client
{
    
/// <summary>
/// Encapsulates the response to a wallet spend validation request
/// </summary>
public class SpendRequestValidateResponseDto
{
    /// <summary>
    /// value indicating whether the provided request is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// the list of errors, if any
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public SpendRequestValidateResponseErrorDto[]? Errors { get; set; }
}

}