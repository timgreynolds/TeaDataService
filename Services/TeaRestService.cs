using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using com.mahonkin.tim.TeaDataService.DataModel;
using Microsoft.Extensions.Configuration;

namespace com.mahonkin.tim.TeaDataService.Services.TeaRestService
{
    public class TeaRestService<T> : IDataService<T> where T : TeaModel
    {
        private static HttpClient _client = new HttpClient();

        /// <summary>
        /// Use the async method if possible.
        /// </summary>
        /// <remarks>
        /// This wraps the async method in a continuation using ContinueWith.
        /// </remarks>
        public T Add(object obj)
        {
            AddAsync(obj).ContinueWith(t => obj = t.Result).ConfigureAwait(false);
            return (T)obj;
        }

        /// <summary>
        /// Adds the given tea to the database in an asynchronous manner.
        /// </summary>
        /// <param name="obj">
        /// A <see cref="TeaModel">Tea</see> to add to the database.
        /// </param>
        /// <returns>
        /// A Task representing the add operation. The task result contains the
        /// tea as added to the database including its auto-assigned unique key.
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<T> AddAsync(object obj)
        {
            HttpResponseMessage response = (await _client.PostAsJsonAsync("api/teas", (T)obj)).EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>() ?? (T)obj;
        }

        /// <summary>
        /// Use the async method if possible.
        /// </summary>
        /// <remarks>
        /// This wraps the async method in a continuation using ContinueWith.
        /// </remarks>
        public bool Delete(object obj)
        {
            bool success = false;
            DeleteAsync(obj).ContinueWith(t => success = t.Result).ConfigureAwait(false);
            return success;
        }

        /// <summary>
        /// Deletes the given tea from the database using its primary key in an
        /// asynchronous manner. The object is required to have a primary key.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="TeaModel">Tea</see> to be deleted.
        /// </param>
        /// <returns>
        /// A Task representing the delete operation. The task result contains
        /// true if the tea was deleted and false otherwise.
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<bool> DeleteAsync(object obj)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, "api/teas");
            request.Content = JsonContent.Create((T)obj, MediaTypeHeaderValue.Parse("application/json"));
            HttpResponseMessage response = (await _client.SendAsync(request)).EnsureSuccessStatusCode();
            bool success = await response.Content.ReadFromJsonAsync<bool>();
            return success;
        }

        /// <summary>
        /// Use the async method if possible.
        /// </summary>
        /// <remarks>
        /// This wraps the async method in a continuation using ContinueWith.
        /// </remarks>
        public T FindById(object id)
        {
            TeaModel tea = new TeaModel();
            FindByIdAsync(id).ContinueWith(t => tea = t.Result)
                .ConfigureAwait(false);
            return (T)tea;
        }

        /// <summary>
        /// Attempts to retrieve the tea with the given primary key from the
        /// web service in an asynchronous manner. Use of this method requires that
        /// the given tea have a primary key.
        /// </summary>
        /// <param name="obj">The primary key of the tea to retrieve.</param>
        /// <returns>
        /// A Task representing the retrieve operation. The task result contains
        /// the tea retrieved or null if not found.
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<T> FindByIdAsync(object id)
        {
            return (await _client.GetFromJsonAsync<T>($"api/teas/{id}").ConfigureAwait(false)) ?? (T)(new TeaModel());
        }

        /// <summary>
        /// Use the async method if possible.
        /// </summary>
        /// <remarks>
        /// This wraps the async method in a continuation using ContinueWith.
        /// </remarks>
        public List<T> Get()
        {
            List<T> teas = new List<T>();
            GetAsync().ContinueWith(t => teas = t.Result).ConfigureAwait(false);
            return teas;
        }

        /// <summary>
        /// Gets all the teas from the web service in an asynchronous manner.
        /// </summary>
        /// <returns>
        /// A Task representing the get operation. The task result contains a
        /// List of all the teas in the database.
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<List<T>> GetAsync()
        {
            return (await _client.GetFromJsonAsync<List<T>>("api/teas")
                .ConfigureAwait(false)) ?? new List<T>();
        }

        /// <summary>
        /// Initialize the HTTP client.
        /// </summary>
        /// <remarks>
        /// Reads the BaseURL from the 'TeaApiUrl connection string in the
        /// appsettings.json file of the assembly.
        /// </remarks>
        public void Initialize()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false)
                    .AddJsonFile("appsettings.Development.json", true)
                    .Build();
            string? baseAddress = configuration.GetConnectionString("TeaApiUrl");

            if (_client.BaseAddress is null)
            {
                if (String.IsNullOrEmpty(baseAddress) == false)
                {
                    _client.BaseAddress = new Uri(baseAddress);
                }
                else
                {
                    throw new ArgumentNullException("TeaApiUrl", "A URL endpoint must be specified for the Tea API Web Service in the appsettings.json file.");
                }
            }
        }

        /// <summary>
        /// Use the async method if possible.
        /// </summary>
        /// <remarks>
        /// This wraps the async method in a continuation using ContinueWith.
        /// </remarks>
        public T Update(object obj)
        {
            UpdateAsync(obj).ContinueWith(t => obj = t.Result)
                .ConfigureAwait(false);
            return (T)obj;
        }

        /// <summary>
        /// Updates all of the columns of a table using the given tea except
        /// for its primary key in an asynchronous manner. The object is 
        /// required to have a primary key.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="TeaModel">Tea</see> to be updated.
        /// </param>
        /// <returns>
        /// A Task representing the update operation. The task result contains
        /// the tea as it was updated.
        /// </returns>
        /// <exception cref="HttpRequestException" />
        /// <exception cref="Exception" />
        public async Task<T> UpdateAsync(object obj)
        {
            HttpResponseMessage responseMessage = (
                await _client.PutAsJsonAsync<T>("api/teas", (T)obj)
                .ConfigureAwait(false)
                ).EnsureSuccessStatusCode();
            return (await responseMessage.Content.ReadFromJsonAsync<T>().ConfigureAwait(false)) ?? (T)obj;
        }
    }
}

