using System.Net;
using System.Net.Http;

namespace com.mahonkin.tim.TeaDataService.Exceptions
{
    public class TeaHttpRequestException : HttpRequestException
    {
        public TeaHttpRequestException() : base() { }

        public TeaHttpRequestException(string? message) : base(message) { }

        public TeaHttpRequestException(string? message, System.Exception? inner) : base(message, inner) { }

        public TeaHttpRequestException(string? message, System.Exception? inner, HttpStatusCode? statusCode) : base(message, inner, statusCode) { }
    }
}

