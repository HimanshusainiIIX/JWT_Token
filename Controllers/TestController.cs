using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Security.Claims;
using System.Data.SqlClient;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net;
using System.Text;
using Dapper;
using SymmetricSecurityKey = Microsoft.IdentityModel.Tokens.SymmetricSecurityKey;
using SigningCredentials = Microsoft.IdentityModel.Tokens.SigningCredentials;
using SecurityAlgorithms = Microsoft.IdentityModel.Tokens.SecurityAlgorithms;
using JWT_Token.Models;
using System.Web.Http.Cors;
using System.Web.Http.Results;

namespace JWT_Token.Controllers
{
    public class TestController : ApiController
    {
        SqlConnection cnn = new SqlConnection(Connection.GetConnectionString());

        [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-Custom-Header")]
        [HttpGet]
        public String GetName1()
        {
            if (User.Identity.IsAuthenticated)
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                }
                return "Valid";
            }
            else
            {
                return "Invalid";
            }
        }
         [Authorize]
        [HttpGet]
        public Object GetName2()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "name").FirstOrDefault()?.Value;
                var Username = claims.Where(p => p.Type == "username").FirstOrDefault()?.Value;
                return new
                {
                    name,
                   Username
                };

            }
            return null;
        }

        private UserModel AuthenticateUser(UserModel login)
        {
         
                UserModel _data = new UserModel();
                string filter = "SELECT  *  FROM Login where Username ='" + login.Username + "' and Password ='" + login.Password + "'  ";
                var output = cnn.Query<UserModel>(filter, new DynamicParameters());
                cnn.Close();
                if (output.ToList().Count > 0)
                {
                    _data.success = true;
                    _data.Username = output.ToList()[0].Username;
                    _data.Name = output.ToList()[0].Name;
                    _data.Id = output.ToList()[0].Id;
                    return _data;
                }
                else
                {
                    _data.success = false;
                    return _data;
                }

        }
        [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-Custom-Header")]
        [HttpPost]
        public HttpResponseMessage Login([FromBody] UserModel login)
        {
            object response = Unauthorized();

            if (login == null)
            {
                login = new UserModel();
                login.success = false;
                HttpResponseMessage respose = Request.CreateResponse<object>(HttpStatusCode.OK, login);
                return respose;
            }

            var user = AuthenticateUser(login);

            if (user == null)
            {
                user = new UserModel();
                user.success = false;
            }

            if (true)
            {
                if (user.success == false)
                {
                    HttpResponseMessage respose = Request.CreateResponse<object>(HttpStatusCode.OK, user);
                    return respose;
                }
                else
                {
                    string Gid = Guid.NewGuid().ToString();
                    var tokenString = GetToken(user, Gid);
                    user.token = tokenString;
                    HttpResponseMessage respose = Request.CreateResponse<object>(HttpStatusCode.OK, user);
                    return respose;
                }
            }
        }

        [HttpGet]
        public Object GetToken(UserModel user, string Gid)
        {
            string key = "ThisismyTestDemoHimanshuSecretKey"; //Secret key which will be used later during validation    
            var issuer = "http://himanshusaini.com";  //normally this will be your site URL    

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Create a List of Claims, Keep claims name short    
            var permClaims = new List<Claim>();

            permClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Username));
            permClaims.Add(new Claim("GID", Gid));
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim("userid", user.Id.ToString()));
            permClaims.Add(new Claim("name", user.Name));
            permClaims.Add(new Claim("username", user.Username));

            //Create Security Token object by giving required parameters    
            var token = new JwtSecurityToken(issuer, //Issure    
                            issuer,  //Audience    
                            permClaims,
                            expires: DateTime.Now.AddMinutes(2),
                            signingCredentials: credentials);
            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt_token;
        }



        [Authorize]
        [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
        [HttpGet]
        public HttpResponseMessage GetUserModel()
        {
            UserModel QueryList = new UserModel();

            var output = cnn.Query<UserModel>("select * from Login", new DynamicParameters());
            QueryList = output.FirstOrDefault();

            return Request.CreateResponse<UserModel>(HttpStatusCode.OK, QueryList);
        }












    }
}
