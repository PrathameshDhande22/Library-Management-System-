using System.Configuration;
using System.Data.SqlClient;

namespace MVC_Task.Database
{
    public class Connection
    {
        /// <summary>
        /// Returns the Connection String.
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["laptopdb"].ConnectionString;
            }
        }

        /// <summary>
        /// SQl Connection by connecting to the Connection String
        /// </summary>
        /// <returns>SQL Connection</returns>
        public static SqlConnection GetConn()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}