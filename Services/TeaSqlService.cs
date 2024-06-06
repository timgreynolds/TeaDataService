using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.mahonkin.tim.TeaDataService.DataModel;
using com.mahonkin.tim.TeaDataService.Exceptions;
using SQLite;

namespace com.mahonkin.tim.TeaDataService.Services
{
    /// <summary>
    /// Implementation of <see cref="IDataService{T}">IDataService"</see> using SQLite and a database of teas.
    /// </summary>
    public class TeaSqlService : IDataService<TeaModel>
    {
        #region Private Fields
        private static string _dbFile = string.Empty;
        #endregion Private Fields

        #region Public Methods
        /// <summary>
        /// Use the <see cref="AddAsync(object)">async</see> method if possible.
        /// </summary>
        public TeaModel Add(object? tea)
        {
            return AddAsync(tea).Result;
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
        /// <exception cref="TeaSqlException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="Exception" /> 
        public async Task<TeaModel> AddAsync(object? tea)
        {
            try
            {
                if (tea == null)
                {
                    throw new ArgumentNullException(nameof(tea), "Attempt to add an empty tea to the database.");
                }
                await new SQLiteAsyncConnection(_dbFile, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex).InsertAsync((TeaModel)tea);
                return (TeaModel)tea;
            }
            catch (SQLiteException ex)
            {
                throw new TeaSqlException(ex.Result, ex.Message, ex);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Use the <see cref="DeleteAsync(object)">async</see> method if possible.
        /// </summary>
        public object Delete(object? tea)
        {
            return DeleteAsync(tea).Result;
        }

        /// <summary>
        /// Deletes the given tea from the database using its primary key in an
        /// asynchronous manner. The object is required to have a valid primary key.
        /// </summary>
        /// <param name="tea">
        /// The <see cref="TeaModel">Tea</see> to be deleted.
        /// </param>
        /// <returns>
        /// A Task representing the delete operation. The task result contains
        /// true if the tea was deleted an Exception if not if not.
        /// </returns>
        /// <exception cref="ApplicationException" /> 
        /// <exception cref="TeaSqlException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="Exception" />
        public async Task<object> DeleteAsync(object? tea)
        {
            try
            {
                if (tea is null)
                {
                    throw new ArgumentNullException(nameof(tea), "Attempt to delete an empty tea from the database.");
                }
                List<TeaModel> teas = await new SQLiteAsyncConnection(_dbFile, SQLiteOpenFlags.ReadOnly | SQLiteOpenFlags.FullMutex).Table<TeaModel>().ToListAsync();
                if (teas.Count <= 1)
                {
                    throw new TeaSqlException(SQLite3.Result.Error, SQLite3.ExtendedResult.ConstraintCheck, "Cannot delete the only tea in the database.");
                }
                ((TeaModel)tea).Validate();
                if (await new SQLiteAsyncConnection(_dbFile, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex).DeleteAsync(tea) == 1)
                {
                    return true;
                }
                throw new TeaSqlException(SQLite3.Result.Error, $"{tea} could not be deleted.");
            }
            catch (SQLiteException ex)
            {
                throw new TeaSqlException(ex.Result, ex.Message, ex);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Use the <see cref="FindByIdAsync(object)">async</see> method if possible.
        /// </summary>
        public TeaModel FindById(object? id)
        {
            return FindByIdAsync(id).Result;
        }

        /// <summary>
        /// Attempts to retrieve the tea with the given primary key from the
        /// database in an asynchronous manner. Use of this method requires that
        /// the given tea have a primary key.
        /// </summary>
        /// <param name="id">The primary key of the tea to retrieve.</param>
        /// <returns>
        /// A Task representing the retrieve operation. The task result contains
        /// the tea retrieved or null if not found.
        /// </returns>
        /// <exception cref="TeaSqlException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<TeaModel> FindByIdAsync(object? id)
        {
            try
            {
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id), "Attempt to find an empty ID in the database.");
                }
                return await new SQLiteAsyncConnection(_dbFile, SQLiteOpenFlags.ReadOnly | SQLiteOpenFlags.FullMutex).FindAsync<TeaModel>(id);
            }
            catch (SQLiteException ex)
            {
                throw new TeaSqlException(ex.Result, ex.Message, ex);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Use the <see cref="GetAsync()">async</see> method if possible.
        /// </summary>
        public List<TeaModel> Get()
        {
            return GetAsync().Result;
        }

        /// <summary>
        /// Gets all the teas from the database in an asynchronous manner.
        /// </summary>
        /// <returns>
        /// A Task representing the get operation. The task result contains a
        /// List of all the teas in the database.
        /// </returns>
        /// <exception cref="TeaSqlException" />
        /// <exception cref="Exception" />
        public async Task<List<TeaModel>> GetAsync()
        {
            try
            {
                return await new SQLiteAsyncConnection(_dbFile, SQLiteOpenFlags.ReadOnly | SQLiteOpenFlags.FullMutex).Table<TeaModel>().ToListAsync();
            }
            catch (SQLiteException ex)
            {
                throw new TeaSqlException(ex.Result, ex.Message, ex);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Ensures that the database exists and contains at least one tea variety.
        /// </summary>
        /// <param name="dbFile">The full path of the database file to be used/created.</param>
        /// <exception cref="TeaSqlException" />
        /// <exception cref="Exception" />
        public void Initialize(string dbFile)
        {
            // The DbFile must be created, and populated with at least one initial tea variety.
            // The routines *should* all be non-destructive, relying on 'CreateIfNotExist' patterns, but I added some extra checks just to be sure.
            try
            {
                if (string.IsNullOrWhiteSpace(dbFile))
                {
                    throw new ArgumentNullException(nameof(dbFile), "You must specify a locator, either an API URL or a Sqlite database file.");
                }
                _dbFile = dbFile;
                using (SQLiteConnection connection = new SQLiteConnection(dbFile, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex))
                {
                    TableMapping? mapping = connection.TableMappings.FirstOrDefault(m => m.TableName.Equals("TeaVarieties", StringComparison.OrdinalIgnoreCase));
                    if (mapping is null)
                    {
                        CreateTableResult createTableResult = connection.CreateTable<TeaModel>();
                    }
                    if (connection.Table<TeaModel>().Count() < 1)
                    {
                        connection.Insert(new TeaModel("Earl Grey"));
                    }
                }
            }
            catch (SQLiteException ex)
            {
                throw new TeaSqlException(ex.Result, ex.Message, ex);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Use the <see cref="UpdateAsync(object)">async</see> method if possible.
        /// </summary>
        public TeaModel Update(object? tea)
        {
            return UpdateAsync(tea).Result;
        }

        /// <summary>
        /// Updates all of the columns of a table using the given tea except
        /// for its primary key in an asynchronous manner. The object is 
        /// required to have a valid primary key.
        /// </summary>
        /// <param name="tea">
        /// The <see cref="TeaModel">Tea</see> to be updated.
        /// </param>
        /// <returns>
        /// A Task representing the update operation. The task result contains
        /// the tea as it was updated.
        /// </returns>
        /// <exception cref="TeaSqlException" />
        /// <exception cref="Exception" />
        public async Task<TeaModel> UpdateAsync(object? tea)
        {
            try
            {
                if (tea == null || ((TeaModel)tea).Id == 0)
                {
                    throw new ArgumentNullException(nameof(tea), "Attempt to update an empty tea from the database.");
                }
                ((TeaModel)tea).Validate();
                if (await new SQLiteAsyncConnection(_dbFile, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex).UpdateAsync(tea) < 1)
                {
                    TeaSqlException exception = new TeaSqlException(SQLite3.Result.Error, "The tea could not be updated.");
                    exception.Data.Add("Tea", tea);
                    throw exception;
                }
                return (TeaModel)tea;
            }
            catch (SQLiteException ex)
            {
                throw new TeaSqlException(ex.Result, ex.Message, ex);
            }
            catch
            {
                throw;
            }
        }
        #endregion Public Methods
    }
}