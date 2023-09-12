using System;
using SQLite;

namespace com.mahonkin.tim.TeaDataService.Exceptions
{
    /// <inheritdoc cref="ApplicationException" />
    public class TeaSqlException : ApplicationException
    {
        public SQLite3.Result Result;

        public TeaSqlException() : base() { }

        public TeaSqlException(SQLite3.Result result, string? message) : base(message)
        {
            Result = result;
        }

        public TeaSqlException(SQLite3.Result result, string? message, SQLiteException? inner) : base(message, inner)
        {
            Result = result;
        }
    }
}