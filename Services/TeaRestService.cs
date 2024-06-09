using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using com.mahonkin.tim.TeaDataService.DataModel;

namespace com.mahonkin.tim.TeaDataService.Services
{
    /// <summary>
    /// Implementation of <see cref="IDataService{T}">IDataService</see> providing a REST API. 
    /// </summary>
    public class TeaRestService : IDataService<TeaRestResponse>
    {
        private static readonly HttpClient _client = new HttpClient();
        private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
#if NET8_0_OR_GREATER
            // Not supported on net7
            PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
#endif
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Use the <see cref="AddAsync(object)" /> method if possible.
        /// </summary>
        public TeaRestResponse Add(object? tea)
        {
            return AddAsync(tea).Result;
        }

        /// <summary>
        /// Adds the given tea to the data provider in an asynchronous manner.
        /// </summary>
        /// <param name="tea">
        /// A <see cref="TeaModel">Tea</see> to add to the data provider.
        /// </param>
        /// <returns>
        /// A Task representing the add operation. The task result contains the
        /// result of the Add operation.
        /// </returns>
        public async Task<TeaRestResponse> AddAsync(object? tea)
        {
            try
            {
                if (tea is null)
                {
                    throw new ArgumentNullException(nameof(tea), "Attempt to add an empty tea to the database.");
                }
                ((TeaModel)tea).Validate();
                HttpResponseMessage response = await _client.PostAsJsonAsync<TeaModel>("api/teas", (TeaModel)tea, _jsonOptions);
                TeaRestResponse content = await response.Content.ReadFromJsonAsync<TeaRestResponse>(_jsonOptions) ?? new TeaRestResponse
                {
                    Success = false,
                    Message = $"{response.StatusCode} {response.ReasonPhrase}"
                };
                return content;
            }
            catch (Exception ex)
            {
                return new TeaRestResponse()
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Use the <see cref="DeleteAsync(object)"/> method if possible.
        /// </summary>
        public object Delete(object? tea)
        {
            return DeleteAsync(tea).Result;
        }

        /// <summary>
        /// Deletes the given tea from the data provider in an asynchronous manner. 
        /// The object is required to have a primary key.
        /// </summary>
        /// <param name="tea">
        /// The <see cref="TeaModel">Tea</see> to be deleted.
        /// </param>
        /// <returns>
        /// A Task representing the delete operation. The task result contains
        /// the result of the operation.
        /// </returns>
        public async Task<object> DeleteAsync(object? tea)
        {
            try
            {
                if (tea is null)
                {
                    throw new ArgumentNullException(nameof(tea), "Attempt to delete an empty tea from the database.");
                }
                ((TeaModel)tea).Validate();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, "api/teas")
                {
                    Content = JsonContent.Create<TeaModel>((TeaModel)tea, MediaTypeHeaderValue.Parse("application/json"), _jsonOptions)
                };
                HttpResponseMessage response = await _client.SendAsync(request);
                TeaRestResponse content = await response.Content.ReadFromJsonAsync<TeaRestResponse>(_jsonOptions) ?? new TeaRestResponse
                {
                    Success = false,
                    Message = $"{response.StatusCode} {response.ReasonPhrase}"
                };
                return content;

            }
            catch (Exception ex)
            {
                return new TeaRestResponse()
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Use the <see cref="FindByIdAsync(object)" /> method if possible.
        /// </summary>
        public TeaRestResponse FindById(object? id)
        {
            return FindByIdAsync(id).Result;
        }

        /// <summary>
        /// Attempts to retrieve the tea with the given primary key from the
        /// web service in an asynchronous manner. Use of this method requires that
        /// the given tea have a primary key.
        /// </summary>
        /// <param name="id">The primary key of the tea to retrieve.</param>
        /// <returns>
        /// A Task representing the retrieve operation. The task result contains
        /// the tea retrieved or null if not found.
        /// </returns>
        public async Task<TeaRestResponse> FindByIdAsync(object? id)
        {
            try
            {
                if (id is null)
                {
                    throw new ArgumentNullException(nameof(id), "No ID object provided to find.");
                }
                return (await _client.GetFromJsonAsync<TeaRestResponse>($"api/teas/{id}", _jsonOptions)) ?? new TeaRestResponse()
                {
                    Success = false,
                    Message = $"Could not find tea with ID {id}"
                };
            }
            catch (Exception ex)
            {
                return new TeaRestResponse()
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Use the <see cref="GetAsync()" /> method if possible.
        /// </summary>
        public TeaRestResponse Get()
        {
            return GetAsync().Result;
        }

        /// <summary>
        /// Gets all the teas from the data provider in an asynchronous manner.
        /// </summary>
        /// <returns>
        /// A Task representing the get operation. The task result contains a
        /// List of all the teas in the data provider.
        /// </returns>
        public async Task<TeaRestResponse> GetAsync()
        {
            try
            {
                return (await _client.GetFromJsonAsync<TeaRestResponse>("api/teas", _jsonOptions).ConfigureAwait(false)) ?? new TeaRestResponse()
                {
                    Success = false,
                    Message = "No teas found in the data provider."
                };
            }
            catch (Exception ex)
            {
                return new TeaRestResponse()
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Initialize the HTTP client.
        /// </summary>
        /// <param name="baseAddress">
        /// The Base Address of the REST API endpoint. 
        /// </param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="UriFormatException" />
        /// <exception cref="Exception" />
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
        /// Use the <see cref="UpdateAsync(object)" /> method if possible.
        /// </summary>
        public TeaRestResponse Update(object? tea)
        {
            return UpdateAsync(tea).Result;
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
        public async Task<TeaRestResponse> UpdateAsync(object? tea)
        {
            try
            {
                if (tea is null) 
                {
                    throw new ArgumentNullException(nameof(tea), "Attempt to update an empty tea.");
                }
                // tea = TeaModel.ValidateTea((TeaModel)tea);
                ((TeaModel)tea).Validate();
                HttpResponseMessage response = await _client.PutAsJsonAsync<TeaModel>("api/teas", (TeaModel)tea, _jsonOptions).ConfigureAwait(false);
                return (await response.Content.ReadFromJsonAsync<TeaRestResponse>(_jsonOptions).ConfigureAwait(false)) ?? new TeaRestResponse()
                {
                    Success = false,
                    Message = $"{response.StatusCode} {response.ReasonPhrase}"
                };
            }
            catch (Exception ex)
            {
                return new TeaRestResponse()
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Use the overload <see cref="Get()" /> method that returns 
        /// a <see cref="TeaRestResponse" />  instead of a List.
        /// </summary>
        /// <returns>
        /// A one-element list containing the <see cref="TeaRestResponse" />
        /// returned from the web service. This RestResponse will contain
        /// a List of zero or more <see cref="TeaModel">teas</see> found in 
        /// the data provider.
        /// </returns>
        List<TeaRestResponse> IDataService<TeaRestResponse>.Get()
        {
            return new List<TeaRestResponse>() { Get() };
        }

        /// <summary>
        /// Use the overload <see cref="GetAsync()" /> method that returns 
        /// a <see cref="TeaRestResponse" />  instead of a List.
        /// </summary>
        /// <returns>
        /// A Task respresenting the Get operation. The result of the Task 
        /// will contain a one-element list of <see cref="TeaRestResponse" />.
        /// This RestResponse will contain a List of zero or more 
        /// <see cref="TeaModel">teas</see> found in the data provider.
        /// </returns>
        async Task<List<TeaRestResponse>> IDataService<TeaRestResponse>.GetAsync()
        {
            return new List<TeaRestResponse>() { await GetAsync() };
        }
    }
}

