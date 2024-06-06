using System.Collections.Generic;

namespace com.mahonkin.tim.TeaDataService.DataModel;

/// <summary>
/// HTTP Response message returned by the API.
/// </summary>
public class RestResponse
{
    /// <summary>
    /// Whether or not the API call completed without error. This is not the result of the API call.
    /// </summary>    
    public bool Success = default;

    /// <summary>
    /// Optional additional information message. Is the value of 'Success' is false this should provide more details.
    /// </summary>
    public string Message = string.Empty;

    /// <summary>
    /// If the API call is successful this will be a list of zero or more teas. This is the result of the API call.
    /// </summary>
    public List<TeaModel> Teas = new List<TeaModel>();
}
