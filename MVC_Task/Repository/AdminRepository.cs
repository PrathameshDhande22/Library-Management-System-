using MVC_Task.Database;
using System.Data.SqlClient;
using System;
using Dapper;
using System.Data;
using System.Reflection;
using System.Collections.Generic;

namespace MVC_Task.Repository
{
    /// <summary>
    /// Repository Result can be Used where Result can be a Generic Type to encapsulate both the result and the success status.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class RepositoryResult<TResult>
    {
        /// <summary>
        /// Holds the generic result of the Operation.
        /// </summary>
        public TResult Result { get; set; }

        /// <summary>
        /// Denotes the Success of the Operation.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Holds any message returned by any Database operations.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Dynamic Object Property to hold any type of Result returned by the Operation.
        /// </summary>
        public dynamic ReturnResult { get; set; }
    }

    /// <summary>
    /// Repository ensures that all the Admin base Method are enclosed in these Class.
    /// </summary>
    public class AdminRepository
    {
        /// <summary>
        /// Retrieves or updates the library settings. If an integer is passed (as parameter T), it fetches the setting by ID. 
        /// If a LibrarySetting model is passed, it updates the setting.
        /// </summary>
        /// <typeparam name="T">Can be either an integer (setting ID) or a LibrarySetting model (for update).</typeparam>
        /// <typeparam name="TResult">Type of the result returned after the operation.</typeparam>
        /// <param name="t">Either the setting ID or a LibrarySetting model depending on whether the operation is to get or update a setting.</param>
        /// <returns>A RepositoryResult object containing the result of the operation and the success status.</returns>
        public static RepositoryResult<TResult> GetUpdateLibrarySetting<T, TResult>(T t)
        {
            try
            {
                DynamicParameters parameters = new DynamicParameters();

                if (typeof(T) == typeof(int))
                {
                    parameters.Add("@settingid", t, DbType.Int32);
                    parameters.Add("@issuccess", 0, DbType.Int32, ParameterDirection.Output);

                    using (SqlConnection conn = Connection.GetConn())
                    {
                        conn.Open();
                        TResult tresult = conn.QueryFirstOrDefault<TResult>("spLibrarySetting", parameters, commandType: CommandType.StoredProcedure);
                        return new RepositoryResult<TResult> { Result = tresult, Success = tresult != null };
                    }
                }
                else
                {
                    PropertyInfo[] properties = typeof(T).GetProperties();
                    List<PropertyInfo> props = properties.AsList();
                    props.RemoveAt(4); // removing the last property which is Updated On

                    foreach (PropertyInfo pi in props)
                    {
                        var value = pi.GetValue(t);
                        parameters.Add($"@{pi.Name.ToLower()}", value);
                    }
                    parameters.Add("@issuccess", 0, DbType.Int32, ParameterDirection.Output);
                    using (SqlConnection conn = Connection.GetConn())
                    {
                        int affectedRows = conn.Execute("spLibrarySetting", parameters, commandType: CommandType.StoredProcedure);
                        return new RepositoryResult<TResult> { Success = affectedRows == 1 };
                    }
                }
            }
            catch (Exception)
            {
                return new RepositoryResult<TResult> { Success = false };
            }
        }

    }
}