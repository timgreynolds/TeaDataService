using System.Collections.Generic;

namespace com.mahonkin.tim.TeaDataService.DataModel;

/// <summary>
/// HTTP Response message returned by the API.
/// </summary>
public class TeaRestResponse
{
    /// <summary>
    /// The result of the API call.
    /// </summary>    
    public bool Success = default;

    /// <summary>
    /// Optional additional information message. If the value of 'Success' is false this should provide more error details.
    /// </summary>
    public string Message = string.Empty;

    /// <summary>
    /// A list of zero or more teas. This is the result of the API call.
    /// </summary>
    public List<TeaModel> Teas = new List<TeaModel>();
}
