using System.Collections.Generic;

namespace com.mahonkin.tim.TeaDataService.DataModel;

public class RestResponse
{
    public bool Success = default;
    public string Message = string.Empty;
    public List<TeaModel> Teas = new List<TeaModel>();
}
