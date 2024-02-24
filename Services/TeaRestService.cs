using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using com.mahonkin.tim.TeaDataService.DataModel;
using com.mahonkin.tim.TeaDataService.Exceptions;
using SQLite;

namespace com.mahonkin.tim.TeaDataService.Services.TeaRestService
{
    public class TeaRestService : IDataService<TeaModel>
    {
        private static readonly HttpClient _client = new HttpClient();

        /// <summary>
        /// Use the async method if possible.
        /// </summary>
        public TeaModel Add(TeaModel tea)
        {
            try
            {
                return AddAsync(tea).Result;
            }
            catch (SQLiteException ex)
            {
                throw new TeaSqlException(ex.Result, ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Adds the given tea to the database in an asynchronous manner.
        /// </summary>
        /// <param name="tea">
        /// A <see cref="TeaModel">Tea</see> to add to the database.
        /// </param>
        /// <returns>
        /// A Task representing the add operation. The task result contains the
        /// tea as added to the database including its auto-assigned unique key.
        /// </returns>
        /// <exception cref="TeaHttpRequestException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<TeaModel> AddAsync(TeaModel tea)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/teas", tea);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TeaModel>() ?? tea;
            }
            Console.WriteLine($"An error occurred: {response.StatusCode} - {response.ReasonPhrase}");
            throw new TeaHttpRequestException(response.ReasonPhrase, null, response.StatusCode);
        }

        /// <summary>
        /// Use the async method if possible.
        /// </summary>
        public bool Delete(TeaModel tea)
        {
            try
            {
                return DeleteAsync(tea).Result;
            }
            catch (SQLiteException ex)
            {
                throw new TeaSqlException(ex.Result, ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Deletes the given tea from the database using its primary key in an
        /// asynchronous manner. The object is required to have a primary key.
        /// </summary>
        /// <param name="tea">
        /// The <see cref="TeaModel">Tea</see> to be deleted.
        /// </param>
        /// <returns>
        /// A Task representing the delete operation. The task result contains
        /// true if the tea was deleted and false otherwise.
        /// </returns>
        /// <exception cref="TeaHttpRequestException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<bool> DeleteAsync(TeaModel tea)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, "api/teas");
            request.Content = JsonContent.Create(tea, MediaTypeHeaderValue.Parse("application/json"));
            HttpResponseMessage response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            bool success = await response.Content.ReadFromJsonAsync<bool>();
            return success;
        }

        /// <summary>
        /// Use the async method if possible.
        /// </summary>
        public TeaModel FindById(object id)
        {
            try
            {
                return FindByIdAsync(id).Result;
            }
            catch (SQLiteException ex)
            {
                throw new TeaSqlException(ex.Result, ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
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
        /// <exception cref="TeaHttpRequestException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<TeaModel> FindByIdAsync(object id)
        {
            return (await _client.GetFromJsonAsync<TeaModel>($"api/teas/{id}").ConfigureAwait(false)) ?? new TeaModel();
        }

        /// <summary>
        /// Use the async method if possible.
        /// </summary>
        public List<TeaModel> Get()
        {
            try
            {
                return GetAsync().Result;
            }
            catch (SQLiteException ex)
            {
                throw new TeaSqlException(ex.Result, ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Gets all the teas from the web service in an asynchronous manner.
        /// </summary>
        /// <returns>
        /// A Task representing the get operation. The task result contains a
        /// List of all the teas in the database.
        /// </returns>
        /// <exception cref="TeaHttpRequestException" />
        /// <exception cref="Exception" />
        public async Task<List<TeaModel>> GetAsync()
        {
            return (await _client.GetFromJsonAsync<List<TeaModel>>("api/teas")
                .ConfigureAwait(false)) ?? new List<TeaModel>();
        }

        /// <summary>
        /// Initialize the HTTP client.
        /// </summary>
        /// <param name="baseAddress">
        /// The Base Address of the REST API endpoint. 
        /// </param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="UriFormatException" />
        public void Initialize(string baseAddress)
        {
            if (_client.BaseAddress is null)
            {
                try
                {
                    _client.BaseAddress = new Uri(baseAddress);
                }
                catch (ArgumentNullException ex)
                {
                    throw new ArgumentNullException(ex.Message, ex);
                }
                catch (UriFormatException ex)
                {
                    throw new UriFormatException(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Use the async method if possible.
        /// </summary>
        public TeaModel Update(TeaModel tea)
        {
            try
            {
                return UpdateAsync(tea).Result;
            }
            catch (SQLiteException ex)
            {
                throw new TeaSqlException(ex.Result, ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Updates all of the columns of a table using the given tea except
        /// for its primary key in an asynchronous manner. The object is 
        /// required to have a primary key.
        /// </summary>
        /// <param name="tea">
        /// The <see cref="TeaModel">Tea</see> to be updated.
        /// </param>
        /// <returns>
        /// A Task representing the update operation. The task result contains
        /// the tea as it was updated.
        /// </returns>
        /// <exception cref="TeaHttpRequestException" />
        /// <exception cref="Exception" />
        public async Task<TeaModel> UpdateAsync(TeaModel tea)
        {
            HttpResponseMessage responseMessage = (
                await _client.PutAsJsonAsync("api/teas", (TeaModel)tea)
                .ConfigureAwait(false)
                ).EnsureSuccessStatusCode();
            return (await responseMessage.Content.ReadFromJsonAsync<TeaModel>().ConfigureAwait(false)) ?? tea;
        }
    }
}

