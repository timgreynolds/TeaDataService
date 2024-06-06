using System;
using SQLite;

namespace com.mahonkin.tim.TeaDataService.Exceptions
{
    /// <inheritdoc cref="ApplicationException" />
    public class TeaSqlException : ApplicationException
    {
        /// <inheritdoc cref="SQLite3.Result" />
        public SQLite3.Result Result;
        /// <inheritdoc cref="SQLite3.ExtendedResult"/>
        public SQLite3.ExtendedResult ExtendedResult;

        /// <inheritdoc cref="ApplicationException()"/>
        public TeaSqlException() : base() { }

        /// <inheritdoc cref="SQLiteException(SQLite3.Result, string)"/>
        public TeaSqlException(SQLite3.Result result, string? message) : base(message)
        {
            Result = result;
        }

        /// <inheritdoc cref="TeaSqlException(SQLite3.Result, string, SQLiteException)"/>
        public TeaSqlException(SQLite3.Result result, string? message, SQLiteException? inner) : base(message, inner)
        {
            Result = result;
        }

        /// <inheritdoc cref="TeaSqlException(SQLite3.Result, SQLite3.ExtendedResult, string)"/>
        public TeaSqlException(SQLite3.Result result, SQLite3.ExtendedResult extendedResult, string? message) : base(message)
        {
            Result = result;
            ExtendedResult = extendedResult;
        }

         /// <inheritdoc cref="TeaSqlException(SQLite3.Result, SQLite3.ExtendedResult, string, SQLiteException)"/>
        public TeaSqlException(SQLite3.Result result, SQLite3.ExtendedResult extendedResult, string? message, SQLiteException? inner) : base(message, inner)
        {
            Result = result;
            ExtendedResult = extendedResult;
        }
    }
}