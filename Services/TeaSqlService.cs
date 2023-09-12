﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.mahonkin.tim.TeaDataService.DataModel;
using com.mahonkin.tim.TeaDataService.Exceptions;
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
        private static string _dbFile = string.Empty;
        #endregion Private Fields

        #region Public Methods
        /// <summary>
        /// Ensures that the database exists and contains at least one tea
        /// variety.
        /// </summary>
        /// <param name="dbFile">The full path of the database file to be used/created.</param>
        /// <remarks>
        /// </remarks>
        /// <exception cref="TeaSqlException" />
        /// <exception cref="Exception" />
        public void Initialize(string dbFile)
        {
            // The DbFile must be created, and populated with at least one initial tea variety.
            // The routines *should* all be non-destructive, relying on 'CreateIfNotExist' patterns, but I added some extra checks just to be sure.
            try
            {
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

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
        /// <param name="obj">
        /// A <see cref="TeaModel">Tea</see> to add to the database.
        /// </param>
        /// <returns>
        /// A Task representing the add operation. The task result contains the
        /// tea as added to the database including its auto-assigned unique key.
        /// </returns>
        /// <exception cref="TeaSqlException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<TeaModel> AddAsync(TeaModel tea)
        {
            try
            {
                await new SQLiteAsyncConnection(_dbFile, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex).InsertAsync(tea).ConfigureAwait(false);
                return tea;
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
        /// <param name="obj">
        /// The <see cref="TeaModel">Tea</see> to be updated.
        /// </param>
        /// <returns>
        /// A Task representing the update operation. The task result contains
        /// the tea as it was updated.
        /// </returns>
        /// <exception cref="TeaSqlException" />
        /// <exception cref="Exception" />
        public async Task<TeaModel> UpdateAsync(TeaModel tea)
        {
            try
            {
                tea = TeaModel.ValidateTea(tea);
                await new SQLiteAsyncConnection(_dbFile, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex).UpdateAsync(tea).ConfigureAwait(false);
                return tea;
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
        /// <exception cref="TeaSqlException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<bool> DeleteAsync(TeaModel tea)
        {
            try
            {
                List<TeaModel> teas = await new SQLiteAsyncConnection(_dbFile, SQLiteOpenFlags.ReadOnly | SQLiteOpenFlags.FullMutex).Table<TeaModel>().ToListAsync().ConfigureAwait(false);
                if (teas.Count <= 1)
                {
                    throw new ApplicationException("Cannot delete the only tea in the database.");
                }
                bool deleted = false;
                tea = TeaModel.ValidateTea(tea);
                if (await new SQLiteAsyncConnection(_dbFile, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex).DeleteAsync(tea).ConfigureAwait(false) == 1)
                {
                    deleted = true;
                }
                return deleted;
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
        /// Gets all the teas from the database in an asynchronous manner.
        /// </summary>
        /// <returns>
        /// A Task representing the get operation. The task result contains a
        /// List of all the teas in the database.
        /// </returns>
        /// <exception cref="TeaSqlException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<List<TeaModel>> GetAsync()
        {
            try
            {
                return await new SQLiteAsyncConnection(_dbFile, SQLiteOpenFlags.ReadOnly | SQLiteOpenFlags.FullMutex).Table<TeaModel>().ToListAsync().ConfigureAwait(false);
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
        /// Use the async method if possible.
        /// </summary>
        public TeaModel? FindById(object id)
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
        /// database in an asynchronous manner. Use of this method requires that
        /// the given tea have a primary key.
        /// </summary>
        /// <param name="obj">The primary key of the tea to retrieve.</param>
        /// <returns>
        /// A Task representing the retrieve operation. The task result contains
        /// the tea retrieved or null if not found.
        /// </returns>
        /// <exception cref="TeaSqlException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<TeaModel> FindByIdAsync(object obj)
        {
            try
            {
                return await new SQLiteAsyncConnection(_dbFile, SQLiteOpenFlags.ReadOnly | SQLiteOpenFlags.FullMutex).FindAsync<TeaModel>(obj).ConfigureAwait(false);
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
        #endregion Public Methods
    }
}