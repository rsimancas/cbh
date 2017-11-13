using Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Utilidades;

namespace CBHWA.Models
{
    public class UserRepository : IUserRepository
    {
        //DBContext dbcontext;
        public UserRepository()
        {
            //dbcontext = new DBContext();
        }

        public IList<User> GetAll(int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string where = (page != 0) ? " WHERE row>@start and row<=@limit " : "";

            string sql = "SELECT * FROM ( " +
                         "SELECT EmployeeLogin as UserName,EmployeePassword as UserPassword, EmployeeKey, " +
                         "  RTRIM(EmployeeLastName)+' '+RTRIM(EmployeeFirstName) as UserFullName, " +
                         "  EmployeeLocationKey, EmployeeAccessLevel" +
                         "  ROW_NUMBER() OVER (ORDER BY FileCreatedDate DESC) as row,  " +
                         "  IsNull((select count(*) from tblEmployees),0)  as TotalRecords   " +
                         " FROM tblEmployees) a  " +
                         where +
                         " ORDER BY UserName DESC";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            if (page != 0)
            {
                da.SelectCommand.Parameters.Add("@start", SqlDbType.Int).Value = Convert.ToInt32(start);
                da.SelectCommand.Parameters.Add("@limit", SqlDbType.Int).Value = Convert.ToInt32(limit);
            };

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            totalRecords = (int)dt.Rows[0]["TotalRecords"];

            IList<User> data = dt.AsEnumerable()
                          .Select(row => new User
                          {
                              UserName = Convert.ToString(row["UserName"]),
                              UserFullName = Convert.ToString(row["UserFullName"]),
                              EmployeeKey = Convert.ToInt32(row["EmployeeKey"]),
                              EmployeeAccessLevel = Convert.ToInt32(row["EmployeeAccessLevel"])
                          }).ToList<User>();

            return data;
        }

        public User Get(string id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            if (!id.ToUpper().Contains(@"CBH\"))
            {
                id = @"CBH\" + id;
            }

            string sql = @"SELECT * FROM ( 
                         SELECT EmployeeLogin as UserName,EmployeePassword as UserPassword, EmployeeKey, 
                           RTRIM(EmployeeLastName)+' '+RTRIM(EmployeeFirstName) as UserFullName, 
                           EmployeeLocationKey, EmployeeAccessLevel
                          FROM tblEmployees
                          WHERE EmployeeLogin = @id) a";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@id", SqlDbType.VarChar).Value = id;

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            User data = dt.AsEnumerable()
                          .Select(row => new User
                          {
                              UserName = Convert.ToString(row["UserName"]),
                              UserFullName = Convert.ToString(row["UserFullName"]),
                              EmployeeKey = Convert.ToInt32(row["EmployeeKey"]),
                              EmployeeAccessLevel = Convert.ToInt32(row["EmployeeAccessLevel"])
                          }).FirstOrDefault<User>();

            return data;
        }

        public User Add(User User)
        {
            //dbcontext.Configuration.ProxyCreationEnabled = false;
            //if (User == null)
            //{
            //    throw new ArgumentNullException("item");
            //}
            //dbcontext.User.Add(User);
            //dbcontext.SaveChanges();
            return User;
        }

        public void Remove(string id)
        {
            //User User = Get(id);
            //if (User != null)
            //{
            //    dbcontext.User.Remove(User);
            //    dbcontext.SaveChanges();
            //}
        }

        public bool Update(User User)
        {
            //if (User == null)
            //{
            //    throw new ArgumentNullException("User");
            //}

            //User UserInDB = Get(User.UserCode);

            //if (UserInDB == null)
            //{
            //    return false;
            //}

            //dbcontext.User.Remove(UserInDB);
            //dbcontext.SaveChanges();

            //dbcontext.User.Add(User);
            //dbcontext.SaveChanges();

            return true;
        }

        public User ValidLogon(string userName, string userPassword)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            var userWithDomain = userName;
            if (!userWithDomain.ToUpper().Contains(@"CBH\"))
            {
                userWithDomain = @"CBH\" + userWithDomain;
            }

            string sql = "select EmployeeLogin as UserName,EmployeePassword as UserPassword, " +
                "rtrim(EmployeeLastName)+' '+rtrim(EmployeeFirstName) as UserFullName, EmployeeKey, EmployeeAccessLevel "  + 
            " from tblEmployees where (EmployeeLogin=@UserWithDomain OR EmployeeLogin=@UserName) and EmployeePassword=@pwd";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = userName;
            da.SelectCommand.Parameters.Add("@UserWithDomain", SqlDbType.NVarChar, 100).Value = userWithDomain;
            da.SelectCommand.Parameters.Add("@pwd", SqlDbType.NVarChar,50).Value = userPassword;

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];
            
            IList<User> data = EnumExtension.ToList<User>(dt);
            
            return data.FirstOrDefault<User>();
        }

        public string GenToken(User usr)
        {

            string id = Utils.Encrypt(usr.UserName),
                   password = Utils.Encrypt(usr.UserPassword);

            return String.Format("{0},{1}", id, password);
        }
    }
}