using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.mahonkin.tim.TeaDataService.Services
{
    /// <summary>
    /// Represents an interface that can be used to provide platform-specific
    /// access to an underlying data provider. Type T is the type of the data
    /// object or mode returned by the data provider.
    /// </summary>
    /// <typeparamref name="T" />
    public interface IDataService<T> where T : class
    {
        /// <summary>
        /// Provide initial setup of the data provider. If you need to setup
        /// connections to a data provider, for example, do it here.
        /// </summary>
        /// <param name="locator">
        /// The location of the backing data store. This could be a file, connection
        /// string, REST API endpoint, etc.
        /// </param>
        public void Initialize(string locator);

        /// <summary>
        /// Retrieve the contents of the data provider.
        /// </summary>
        /// <returns>
        /// A list of type T objects.
        /// </returns>
        public List<T> Get();

        /// <summary>
        /// Retrieve the contents of the data provider in an asynchrous manner.
        /// </summary>
        /// <returns>
        /// A Task representing the retrieval operation. The Task result
        /// contains a list of type T objects.
        /// </returns>
        public Task<List<T>> GetAsync();

        /// <summary>
        /// Retrieve a specific object from the data provider by its unique
        /// identifier.
        /// </summary>
        /// <param name="id">
        /// The unigue identifier of the object to be retrieved.
        /// </param>
        /// <returns>
        /// The type T object represented by <paramref name="id">id</paramref>
        /// </returns>
        public T FindById(object? id);

        /// <summary>
        /// Retrieve a specific object from the data provider by its unique
        /// identifier in an asynchrous manner.
        /// </summary>
        /// <param name="id">
        /// The unigue identifier of the object to be retrieved.
        /// </param>
        /// <returns>
        /// A Task representing the retrieval operation. The task result
        /// contains the type T object found.
        /// </returns>
        public Task<T> FindByIdAsync(object? id);

        /// <summary>
        /// Add a new object to the data provider.
        /// </summary>
        /// <param name="obj">
        /// An object to be added to the data provider.
        /// </param>
        /// <returns>
        /// The type T object that represents the result of the add operation.
        /// </returns>
        public T Add(object? obj);

        /// <summary>
        /// Add a new object to the data provider in an asynchrous manner.
        /// </summary>
        /// <param name="obj">
        /// An object to be added to the data provider.
        /// </param>
        /// <returns>
        /// A Task representing the add operation. The task result contains the result of the operation.
        /// </returns>
        public Task<T> AddAsync(object? obj);

        /// <summary>
        /// Update an existing object in the data provider.
        /// </summary>
        /// <param name="obj">
        /// An object to be added to the data provider.
        /// </param>
        /// <returns>
        /// The result of the update operation.
        /// </returns>
        public T Update(object? obj);

        /// <summary>
        /// Update an existing object in the data provider in an asynchrous
        /// manner.
        /// </summary>
        /// <param name="obj">
        /// An object to be added to the data provider.
        /// </param>
        /// <returns>
        /// A Task representing the update operation. The task result contains
        /// the result of the update operation.
        /// </returns>
        public Task<T> UpdateAsync(object? obj);

        /// <summary>
        /// Delete an existing object from the data provider.
        /// </summary>
        /// <param name="obj">
        /// The object to be deleted.
        /// </param>
        /// <returns>
        /// The result of the delete operation.
        /// </returns>
        public object Delete(object? obj);

        /// <summary>
        /// Delete an existing object from the data provider in an asynchronous
        /// manner.
        /// </summary>
        /// <param name="obj">
        /// The object to be deleted.
        /// </param>
        /// <returns>
        /// A Task representing the delete operation. The task result contais
        /// the result of the operation.
        /// </returns>
        public Task<object> DeleteAsync(object? obj);
    }
}