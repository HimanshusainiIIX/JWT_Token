using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JWT_Token.Models
{
    public class UserModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DeviceId { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsLoggedIn { get; set; }
        public string LoginToken { get; set; }
        public DateTime Loginexpiry { get; set; }
        public bool success { get; set; }
        public object token { get; set; }
    }


    public class Connection
    {
        public static string GetConnectionString()
        {
            string gv_GlobalString = "Server=**********;Network Library=DBMSSOCN;Initial Catalog=*********;User Id=******;Password=**********";
            return gv_GlobalString;
        }
    }



    public class Result
    {
        public bool Success { get; set; }
        public object data { get; set; }

    }

    public class Errorlist
    {
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public string ErrorName { get; set; }
        public string solution { get; set; }
    }



}