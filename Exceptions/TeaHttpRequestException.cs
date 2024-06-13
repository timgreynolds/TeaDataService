using System.Net;
using System.Net.Http;

namespace com.mahonkin.tim.TeaDataService.Exceptions
{
    /// <inheritdoc cref="HttpRequestException" />
    public class TeaHttpRequestException : HttpRequestException
    {

        /// <inheritdoc cref="HttpRequestException()" />
        public TeaHttpRequestException() : base() { }

        /// <inheritdoc cref="HttpRequestException(string)" />
        public TeaHttpRequestException(string? message) : base(message) { }

        /// <inheritdoc cref="HttpRequestException(string, System.Exception)" />
        public TeaHttpRequestException(string? message, System.Exception? inner) : base(message, inner) { }

        /// <inheritdoc cref="HttpRequestException(string, System.Exception, HttpStatusCode?)" />
        public TeaHttpRequestException(string? message, System.Exception? inner, HttpStatusCode? statusCode) : base(message, inner, statusCode) { }
    }
}

