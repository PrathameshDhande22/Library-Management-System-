using Dapper;
using MVC_Task.Database;
using MVC_Task.Models;
using MVC_Task.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace MVC_Task.Repository
{
    public class UserRepository
    {

        /// <summary>
        /// Register the User into the System by passing the User Detail by User Object.
        /// </summary>
        /// <param name="user">User object Model with the Details to Register into the System.</param>
        /// <returns>True if the User Registration is successful other wise false.</returns>
        public static bool RegisterUser(User user)
        {
            try
            {
                using (SqlConnection conn = Connection.GetConn())
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@username", user.Username, DbType.String, size: 80);
                    parameters.Add("@password", user.Password, DbType.String, size: 80);
                    parameters.Add("@firstname", user.FirstName, DbType.String, size: 80);
                    parameters.Add("@lastname", user.LastName, DbType.String, size: 80);
                    parameters.Add("@gender", user.Gender, DbType.String, size: 30);
                    parameters.Add("@email", user.Email, DbType.String, size: 90);
                    parameters.Add("@address", user.Address, DbType.String);
                    parameters.Add("@pincode", user.Pincode, DbType.Int32);
                    parameters.Add("@state", user.State, DbType.String, size: 100);
                    parameters.Add("@city", user.City, DbType.String, size: 80);
                    parameters.Add("@phoneno", user.PhoneNo, DbType.String, size: 20);
                    parameters.Add("@issuccess", 0, DbType.Int32, ParameterDirection.Output);

                    int affectedRows = conn.Execute("spRegisterUser", parameters, commandType: CommandType.StoredProcedure);
                    int issuccess = parameters.Get<int>("@issuccess");
                    return issuccess == 1;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Logins the user into the System and returns their details by LoginViewModel.
        /// </summary>
        /// <param name="login">LoginViewModel containing Username and Password along.</param>
        /// <returns>LoginViewModel filled with Details if the Login is Successful otherwise <see langword="null"/>.</returns>
        public static LoginViewModel LoginUser(LoginViewModel login)
        {
            try
            {
                using (SqlConnection connection = Connection.GetConn())
                {

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@username", login.Username, dbType: DbType.String);
                    parameters.Add("@password", login.Password, DbType.String);
                    LoginViewModel data = connection.QuerySingleOrDefault<LoginViewModel>("spLoginUser", parameters, commandType: CommandType.StoredProcedure);
                    return data;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if the Provided Old Password matches with the Passed username and Id.
        /// </summary>
        /// <param name="oldpassword">The Old Password to Validate</param>
        /// <param name="username">The username for which want to validate the oldpassword</param>
        /// <param name="id">The Id of the user to which want to valiadate the Oldpassword.</param>
        /// <returns>True if the Old password is correct, otherwise false.</returns>
        public static bool IsPasswordCorrect(string oldpassword, int id)
        {
            try
            {
                using (SqlConnection conn = Connection.GetConn())
                {
                    conn.Open();

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@password", oldpassword, DbType.String);
                    parameters.Add("@id", id, DbType.Int32);

                    string password = conn.QueryFirstOrDefault<string>("select password from users where password=@password and id=@id and isdeleted=0", parameters);

                    return password == oldpassword;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Updates the Password of the user.
        /// </summary>
        /// <param name="username">The Username of the user to which want to Update the password.</param>
        /// <param name="id">The id of the password want to Update.</param>
        /// <param name="password">The Password want to Update.</param>
        /// <param name="oldpassword">The Old password to verify.</param>
        /// <returns>True if updation password is successful otherwise false. </returns>
        public static bool UpdatePassword(int id, string password, string oldpassword)
        {
            try
            {
                using (SqlConnection conn = Connection.GetConn())
                {
                    conn.Open();

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@password", password, DbType.String);
                    parameters.Add("@id", id, DbType.Int32);
                    parameters.Add("@oldpassword", oldpassword, DbType.String);
                    parameters.Add("@issuccess", 0, DbType.Int32, ParameterDirection.Output);

                    int affectedRows = conn.Execute("spChangePassword", parameters, commandType: CommandType.StoredProcedure);
                    int issuccess = parameters.Get<int>("@issuccess");

                    return issuccess == 1;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the username or Email is available in the system.
        /// </summary>
        /// <param name="identity">The Identity of the userLogged in</param>
        /// <param name="username">Username to check</param>
        /// <param name="email">Email to Check</param>
        /// <param name="ignoreuserid">The User to ignore in the check.</param>
        /// <returns><see langword="true"/>if the username or email does not exists and can be used to register. otherwise false which means the username or email already taken by other user.</returns>
        public static UsernameEmailValidation IsUsernameOrEmailExist(string username = null, string email = null, int ignoreuserid = 0)
        {
            using (SqlConnection connection = Connection.GetConn())
            {
                connection.Open();
                UsernameEmailValidation fetchedrecords = connection.QueryFirstOrDefault<UsernameEmailValidation>("spGetUserNameOrEmail", new { username = username,useremail=email, loginid = ignoreuserid }, commandType: CommandType.StoredProcedure);

                return fetchedrecords;
            }
        }

        /// <summary>
        /// Retrieve user details based user ID while excluding the AdminId
        /// </summary>
        /// <typeparam name="TResult">The type of the result. can be List<User> or User</typeparam>
        /// <param name="adminId">The ID of the admin to Exclude while returning the data of User.</param>
        /// <param name="userid">The ID of the user to retrieve.</param>
        /// <param name="issingleuser">Tfrue if retrieving a single user; otherwise, false.</param>
        /// <returns>A list of users or a single user based on the parameters.</returns>
        public static TResult GetUsersOrSingleDetails<TResult>(int userid = 0, bool issingleuser = false)
        {
            try
            {
                using (SqlConnection conn = Connection.GetConn())
                {
                    DynamicParameters parameters = new DynamicParameters();

                    parameters.Add("@userid", userid, DbType.Int32);

                    IEnumerable<User> results = conn.Query<User, Roles, User>("spGetUsersOrSingle", (user, roles) =>
                    {
                        user.Roles = roles;
                        return user;

                    }, param: parameters, splitOn: "roleid", commandType: CommandType.StoredProcedure);

                    return issingleuser ? (TResult)(object)results.FirstOrDefault() : (TResult)(object)results.ToList();
                }
            }
            catch (Exception)
            {
                return (TResult)(object)new List<User>();
            }
        }

        /// <summary>
        /// Deletes the User from the System.
        /// </summary>
        /// <param name="id">The Id of user want to delete.</param>
        /// <returns>True if the Deletion of the User is successful otherwise false.</returns>
        public static bool DeleteUser(int id)
        {
            try
            {
                using (SqlConnection conn = Connection.GetConn())
                {
                    DynamicParameters parameters = new DynamicParameters();

                    parameters.Add("@id", id, DbType.Int32);
                    parameters.Add("@issuccess", 0, DbType.Int32, ParameterDirection.Output);

                    int affectedRows = conn.Execute("spDeleteUser", parameters, commandType: CommandType.StoredProcedure);
                    return parameters.Get<int>("@issuccess") == 1;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Updates the user profile with the Provided user details.
        /// </summary>
        /// <param name="user">The Details of the User want to update</param>
        /// <param name="updaterid">The Updater Id who is updating the User.</param>
        /// <param name="rolename">The Role Want to Assign to the user.</param>
        /// <returns>True if the updation of the user is successful otherwise false.</returns>
        public static bool UpdateUserProfile(User user, int updaterid = 0, string rolename = null)
        {
            try
            {
                using (SqlConnection conn = Connection.GetConn())
                {
                    conn.Open();
                    DynamicParameters parameters = new DynamicParameters();


                    parameters.Add("@id", user.Id, DbType.Int32);
                    parameters.Add("@username", user.Username, DbType.String, size: 80);
                    parameters.Add("@password", user.Password, DbType.String);
                    parameters.Add("@firstname", user.FirstName, DbType.String, size: 80);
                    parameters.Add("@lastname", user.LastName, DbType.String, size: 80);
                    parameters.Add("@gender", user.Gender, DbType.String, size: 30);
                    parameters.Add("@email", user.Email, DbType.String, size: 90);
                    parameters.Add("@address", user.Address, DbType.String);
                    parameters.Add("@pincode", user.Pincode, DbType.Int32);
                    parameters.Add("@state", user.State, DbType.String, size: 100);
                    parameters.Add("@city", user.City, DbType.String, size: 80);
                    parameters.Add("@phoneno", user.PhoneNo, DbType.String, size: 20);
                    parameters.Add("@userid", updaterid, DbType.Int32);
                    parameters.Add("@rolename", rolename, DbType.String, size: 40);
                    parameters.Add("@issuccess", 0, DbType.Int32, ParameterDirection.Output);

                    int affectedRows = conn.Execute("spUpdateUserProfile", parameters, commandType: CommandType.StoredProcedure);
                    int issuccess = parameters.Get<int>("@issuccess");
                    return issuccess == 1;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}