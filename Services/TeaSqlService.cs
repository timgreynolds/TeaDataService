using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.mahonkin.tim.TeaDataService.DataModel;
using SQLite;

namespace com.mahonkin.tim.TeaDataService.Services.TeaSqLiteService
{
    /// <summary>
    /// Implementation of <see cref="IDataService{T}">IDataService"</see> using
    /// SQLite and a database of teas.
    /// </summary>
    public class TeaSqlService : IDataService<TeaModel>
    {
        #region Private Fields
        private static bool _initialized;
        private static SQLiteAsyncConnection _asyncConnection;
        #endregion Private Fields

        #region Public Methods
        /// <summary>
        /// Ensures that the database exists and contains at least one tea
        /// variety.
        /// </summary>
        /// <param name="dbFile">The full path of the database file to be used/created.</param>
        /// <remarks>
        /// Creates the DB file in the platform-specific <see
        /// cref="Environment.SpecialFolder.LocalApplicationData">Local
        /// Application Data</see> directory and populates it with 'Earl Grey'
        /// tea. If the DB file already exists and contains at least one entry
        /// this should be a no-op.
        /// </remarks>
        /// <exception cref="SQLiteException" />
        /// <exception cref="Exception" />
        public async Task Initialize(string dbFile)
        {
            // The DbFile must be created, and populated with at least one initial tea variety.
            // The routines *should* all be non-destructive, relying on 'CreateIfNotExist' patterns, but I added some extra checks just to be sure.
            try
            {
                _asyncConnection = new SQLiteAsyncConnection(dbFile, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);

                TableMapping? mapping = _asyncConnection.TableMappings.FirstOrDefault(m => m.TableName.Equals("TeaVarieties", StringComparison.OrdinalIgnoreCase));
                if (mapping is null)
                {
                    CreateTableResult createTableResult = await _asyncConnection.CreateTableAsync<TeaModel>();
                }
                if (await _asyncConnection.Table<TeaModel>().CountAsync() < 1)
                {
                    await _asyncConnection.InsertAsync(new TeaModel("Earl Grey"));
                }
                _initialized = true;

            }
            catch (SQLiteException ex)
            {
                throw SQLiteException.New(ex.Result, ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Use the async method if possible.
        /// </summary>
        /// <remarks>
        /// This wraps the async method in a continuation using ContinueWith.
        /// </remarks>
        public TeaModel Add(TeaModel tea)
        {
            try
            {
                AddAsync(tea).ContinueWith((t) => { tea = t.Result; })
                    .ConfigureAwait(false);
                return tea;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message, exception);
            }
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
        /// <exception cref="SQLiteException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<TeaModel> AddAsync(TeaModel tea)
        {
            try
            {
                await _asyncConnection.InsertAsync(tea).ConfigureAwait(false);
                return tea;
            }
            catch (SQLiteException ex)
            {
                throw SQLiteException.New(ex.Result, ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Use the async method if possible.
        /// </summary>
        /// <remarks>
        /// This wraps the async method in a continuation using ContinueWith.
        /// </remarks>
        public TeaModel Update(TeaModel tea)
        {
            UpdateAsync(tea).ContinueWith((t) => tea = t.Result)
                .ConfigureAwait(false);
            return tea;
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
        /// <exception cref="SQLiteException" />
        /// <exception cref="Exception" />
        public async Task<TeaModel> UpdateAsync(TeaModel tea)
        {
            try
            {
                tea = TeaModel.ValidateTea(tea);
                await _asyncConnection.UpdateAsync(tea).ConfigureAwait(false);
                return tea;
            }
            catch (SQLiteException ex)
            {
                throw SQLiteException.New(ex.Result, ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Use the async method if possible.
        /// </summary>
        /// <remarks>
        /// This wraps the async method in a continuation using ContinueWith.
        /// </remarks>
        public bool Delete(TeaModel tea)
        {
            bool deleted = false;
            DeleteAsync(tea).ContinueWith((t) => deleted = t.Result)
                .ConfigureAwait(false);
            return deleted;
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
        /// <exception cref="SQLiteException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<bool> DeleteAsync(TeaModel tea)
        {
            try
            {
                List<TeaModel> teas = await _asyncConnection.Table<TeaModel>().ToListAsync().ConfigureAwait(false);
                if (teas.Count <= 1)
                {
                    throw new ApplicationException("Cannot delete the only tea in the database.");
                }
                bool deleted = false;
                tea = TeaModel.ValidateTea(tea);
                if (await _asyncConnection.DeleteAsync(tea).ConfigureAwait(false) == 1)
                {
                    deleted = true;
                }
                return deleted;
            }
            catch (SQLiteException ex)
            {
                throw SQLiteException.New(ex.Result, ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Use the async method if possible.
        /// </summary>
        /// <remarks>
        /// This wraps the async method in a continuation using ContinueWith.
        /// </remarks>
        public List<TeaModel> Get()
        {
            List<TeaModel> teas = new List<TeaModel>();
            GetAsync().ContinueWith((t) => teas = t.Result)
                .ConfigureAwait(false);
            return teas;
        }

        /// <summary>
        /// Gets all the teas from the database in an asynchronous manner.
        /// </summary>
        /// <returns>
        /// A Task representing the get operation. The task result contains a
        /// List of all the teas in the database.
        /// </returns>
        /// <exception cref="SQLiteException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<List<TeaModel>> GetAsync()
        {
            try
            {
                return await _asyncConnection.Table<TeaModel>().ToListAsync().ConfigureAwait(false);
            }
            catch (SQLiteException ex)
            {
                throw SQLiteException.New(ex.Result, ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Use the async method if possible.
        /// </summary>
        /// <remarks>
        /// This wraps the async method in a continuation using ContinueWith.
        /// </remarks>
        public TeaModel FindById(object id)
        {
            TeaModel? tea = null;
            FindByIdAsync(id).ContinueWith((t) => tea = t.Result)
                .ConfigureAwait(false);
            return tea;
        }

        /// <summary>
        /// Attempts to retrieve the tea with the given primary key from the
        /// database in an asynchronous manner. Use of this method requires that
        /// the given tea have a primary key.
        /// </summary>
        /// <param name="obj">The primary key of the tea to retrieve.</param>
        /// <returns>
        /// A Task representing the retrieve operation. The task result contains
        /// the tea retrieved or null if not found.
        /// </returns>
        /// <exception cref="SQLiteException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<TeaModel> FindByIdAsync(object obj)
        {
            try
            {
                return await _asyncConnection.FindAsync<TeaModel>(obj).ConfigureAwait(false);
            }
            catch (SQLiteException ex)
            {
                throw SQLiteException.New(ex.Result, ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion Public Methods
    }
}