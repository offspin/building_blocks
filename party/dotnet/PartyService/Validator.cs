using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IdentityModel;
using System.IdentityModel.Selectors;
using System.Security.Cryptography;
using System.Data;

namespace PartyService
{
    public class Validator : UserNamePasswordValidator
    {
        private IDatabase database;
        private string realm;

        public Validator()
        {
            string dbClassName = Properties.Settings.Default.DatabaseClass;
            string dbConnStr = Properties.Settings.Default.DatabaseConnectionString;

            if (dbClassName == string.Empty || dbConnStr == String.Empty)
            {
                throw (new Exception("Security Configuration Error"));
            }

            database = (IDatabase)Activator.CreateInstance(Type.GetType(dbClassName));
            database.setConnectionString(dbConnStr);

            DataRow r = database.GetSystemConfig("REALM");

            if (r == null)
            {
                throw (new Exception("Security Configuration Error"));
            }

            realm = Convert.ToString(r["string_value"]);

            if (realm == String.Empty)
            {
                throw (new Exception("Security Configuration Error"));
            }

        }

        public override void Validate(string userName, string password)
        {

            string hashInput = string.Format("{0}:{1}:{2}", userName, realm, password);
            byte[] hashBytes;

            using (MD5 md5 = MD5.Create())
            {
                hashBytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hashInput));
            }

            string passwordHash = BitConverter.ToString(hashBytes).ToLower().Replace("-", "");

            DataRow r = database.GetUser(userName);

            if (r == null || Convert.ToString(r["password_hash"]) != passwordHash)
            {
                throw (new Exception("Authorisation Failed"));
            }
            
        }

    }
}
