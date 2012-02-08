using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PartyService
{
    public class SqlServerDatabase : IDatabase
    {
        private string connectionString;

        public void setConnectionString(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private SqlConnection Connect()
        {
            SqlConnection cn = new SqlConnection(this.connectionString);
            cn.Open();
            return cn;
        }

        private void Disconnect(SqlConnection cn)
        {
            if (cn != null) { cn.Close(); }
        }

        public Party GetParty(int id)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = Connect();
            try
            {
                
                using (SqlCommand cmd = new SqlCommand("GetParty", cn) )        
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
                
                DataRow r = FirstRow(ds);

                if (r == null) { return null; }

                string type = Convert.ToString(r["type"]);

                switch (type)
                {
                    case "P":
                        string firstName = Convert.ToString(r["first_name"]);
                        string lastName = Convert.ToString(r["last_name"]);
                        DateTime dateOfBirth = Convert.ToDateTime(r["date_of_birth"]);
                        return new Person(id, firstName, lastName, dateOfBirth);
                    case "B":
                        string name = Convert.ToString(r["name"]);
                        string regNumber = Convert.ToString(r["reg_number"]);
                        return new Business(id, name, regNumber);
                    default:
                        throw (new Exception(
                            string.Format("Unknown party type '{0}' retrieved", type)));
                }
            }
            finally
            {
                Disconnect(cn);
            }
        }

        public PartyResults GetPartyByName(string name, string baseUrl)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = Connect();
            try
            {
                
                using (SqlCommand cmd = new SqlCommand("GetPartyByName", cn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
                
                PartyResults pr = new PartyResults();
                
                DataTable t = FirstTable(ds);

                if (t == null) { return pr; }

                foreach (DataRow r in t.Rows)
                {
                    int id = Convert.ToInt32(r["id"]);
                    string type = Convert.ToString(r["type"]);
                    string partyName = Convert.ToString(r["name"]);
                    string link = string.Format("{0}party/{1}", baseUrl, id);

                    PartySummary ps = new PartySummary(id, type, partyName, link);
                    pr.PartyList.Add(ps);
                }

                return pr;

            }

            finally
            {
                Disconnect(cn);
            }
        }

        public int CreatePerson(string firstName, string lastName, DateTime dateOfBirth)
        {
            SqlConnection cn = Connect();
            object newId;

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("CreatePerson", cn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int);
                    cmd.Parameters["@Id"].Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    newId = (int?)cmd.Parameters["@Id"].Value;
                }

                return newId == null ? 0 : Convert.ToInt32(newId);
            }
            finally
            {
                Disconnect(cn);
            }

        }

        public void UpdatePerson(int id, string firstName, string lastName, DateTime dateOfBirth)
        {
            SqlConnection cn = Connect();

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("UpdatePerson", cn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                Disconnect(cn);
            }
        }

        public int CreateBusiness(string name, string regNumber)
        {
            SqlConnection cn = Connect();
           
            try
            {
                
                object newId;

                using (SqlCommand cmd = new SqlCommand("CreateBusiness",cn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int);
                    cmd.Parameters["@Id"].Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@RegNumber", regNumber);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    newId = cmd.Parameters["@Id"].Value;
                }

                return newId == null ? 0 : Convert.ToInt32(newId);
            }
            finally
            {
                Disconnect(cn);
            }
        }

        public void UpdateBusiness(int id, string name, string regNumber)
        {
            SqlConnection cn = Connect();

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("UpdateBusiness", cn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@RegNumber", regNumber);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                Disconnect(cn);
            }
        }

        public void DeleteParty(int id)
        {
            SqlConnection cn = Connect();

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("DeleteParty", cn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                Disconnect(cn);
            }

        }

        public Contact GetContact(int id)
        {
            SqlConnection cn = Connect();
            DataSet ds = new DataSet();
            try
            {
                
                using (SqlCommand cmd = new SqlCommand("GetContact", cn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }

                DataRow r = FirstRow(ds);

                if (r == null) { return null; }

                string type = Convert.ToString(r["type"]);
                string subType = Convert.ToString(r["sub_type"]);
                string street = Convert.ToString(r["street"]);
                string town = Convert.ToString(r["town"]);
                string county = Convert.ToString(r["county"]);
                string postCode = Convert.ToString(r["post_code"]);
                string emailAddress = Convert.ToString(r["email_address"]);
                string telephoneNumber = Convert.ToString(r["telephone_number"]);

                switch (type)
                {
                    case "A":
                        return new Address(id, street, town, county, postCode);
                    case "E":
                        return new Email(id, subType, emailAddress);
                    case "T":
                        return new Telephone(id, subType, telephoneNumber);
                    default:
                        throw (new Exception(
                        string.Format("Unknown contact type '{0}' retrieved", type)));
                }
            }
            finally
            {
                Disconnect(cn);
            }

        }

        public ContactResults GetContactByPartyId(int partyId, string baseUrl)
        {
            SqlConnection cn = Connect();
            DataSet ds = new DataSet();
            try
            {
                
                using (SqlCommand cmd = new SqlCommand("GetContactByPartyId", cn))
                {
                    cmd.Parameters.AddWithValue("@PartyId", partyId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }

                DataTable t = FirstTable(ds);

                if (t == null) { return null; }

                ContactResults cr = new ContactResults();

                foreach (DataRow r in t.Rows)
                {
                    int contactId = Convert.ToInt32(r["id"]);
                    string type = Convert.ToString(r["sub_type"]);
                    string detail = Convert.ToString(r["detail"]);
                    string link = string.Format("{0}contacts/{1}",
                        baseUrl, contactId);

                    cr.ContactList.Add
                        (new ContactSummary(contactId, type, detail, link));
                }

                return cr;

            }
            finally
            {
                Disconnect(cn);
            }
        }

        public int CreateAddress(string street, string town, string county, string postCode)
        {
            SqlConnection cn = Connect();
            object newId;

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("CreateAddress", cn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int);
                    cmd.Parameters["@Id"].Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Street", street);
                    cmd.Parameters.AddWithValue("@Town", town);
                    cmd.Parameters.AddWithValue("@County", DBNull.Value);
                    if (county != null && county != string.Empty)
                    {
                        cmd.Parameters["@County"].Value = county;
                    }
                    cmd.Parameters.AddWithValue("@PostCode", DBNull.Value);
                    if (postCode != null && postCode != string.Empty)
                    {
                        cmd.Parameters["@PostCode"].Value = postCode;
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    newId = cmd.Parameters["@Id"].Value;
                }
                return newId == null ? 0 : Convert.ToInt32(newId);
            }
            finally
            {
                Disconnect(cn);
            }
        }

        public void UpdateAddress(int id, string street, string town, string county, string postCode)
        {
            SqlConnection cn = Connect();

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("UpdateAddress", cn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Street", street);
                    cmd.Parameters.AddWithValue("@Town", town);
                    cmd.Parameters.AddWithValue("@County", DBNull.Value);
                    if (county != null && county != string.Empty)
                    {
                        cmd.Parameters["@County"].Value = county;
                    }
                    cmd.Parameters.AddWithValue("@PostCode", DBNull.Value);
                    if (postCode != null && postCode != string.Empty)
                    {
                        cmd.Parameters["@PostCode"].Value = postCode;
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                Disconnect(cn);
            }

        }
       
        public int CreateEmail(string type, string address)
        {
            SqlConnection cn = Connect();
            object newId;

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("CreateEmail", cn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int);
                    cmd.Parameters["@Id"].Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Type", type);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    newId = cmd.Parameters["@Id"].Value;
                }

                return newId == null ? 0 : Convert.ToInt32(newId);
            }
            finally
            {
                Disconnect(cn);
            }
        }

        public void UpdateEmail(int id, string type, string address)
        {
            SqlConnection cn = Connect();

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("UpdateEmail", cn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Type", type);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                Disconnect(cn);
            }
        }

        public int CreateTelephone(string type, string number)
        {
            SqlConnection cn = Connect();
            object newId;

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("CreateTelephone", cn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int);
                    cmd.Parameters["@Id"].Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Type", type);
                    cmd.Parameters.AddWithValue("@Number", number);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    newId = cmd.Parameters["@Id"].Value;
                }

                return newId == null ? 0 : Convert.ToInt32(newId);
            }
            finally
            {
                Disconnect(cn);
            }
        }

        public void UpdateTelephone(int id, string type, string number)
        {
            SqlConnection cn = Connect();

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("UpdateTelephone", cn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Type", type);
                    cmd.Parameters.AddWithValue("@Number", number);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                Disconnect(cn);
            }
        }


        public void DeleteContact(int id)
        {
            SqlConnection cn = Connect();

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("DeleteContact", cn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                Disconnect(cn);
            }
        }

        public PartyContact GetPartyContact(int partyId, int contactId)
        {
            SqlConnection cn = Connect();
            DataSet ds = new DataSet();

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("GetPartyContact", cn))
                {
                    cmd.Parameters.AddWithValue("@PartyId", partyId);
                    cmd.Parameters.AddWithValue("@ContactId", contactId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
                DataRow r = FirstRow(ds);

                if (r == null) { return null; }

                return new PartyContact(
                   Convert.ToInt32(r["party_id"]),
                   Convert.ToInt32(r["contact_id"]),
                   Convert.ToDateTime(r["valid_from"]),
                   Convert.ToDateTime(r["valid_until"]));

            }
            finally
            {
                Disconnect(cn);
            }

        }

        public void CreatePartyContact(int partyId, int contactId, DateTime validFrom, DateTime validUntil)
        {
            SqlConnection cn = Connect();

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("CreatePartyContact", cn))
                {
                    cmd.Parameters.AddWithValue("@PartyId", partyId);
                    cmd.Parameters.AddWithValue("@ContactId", contactId);
                    cmd.Parameters.AddWithValue("@ValidFrom", validFrom);
                    cmd.Parameters.AddWithValue("@ValidUntil", validUntil);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
            finally
            {
                Disconnect(cn);
            }
        }

        public void UpdatePartyContact(int partyId, int contactId, DateTime validFrom, DateTime validUntil)
        {
            SqlConnection cn = Connect();

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("UpdatePartyContact", cn))
                {
                    cmd.Parameters.AddWithValue("@PartyId", partyId);
                    cmd.Parameters.AddWithValue("@ContactId", contactId);
                    cmd.Parameters.AddWithValue("@ValidFrom", validFrom);
                    cmd.Parameters.AddWithValue("@ValidUntil", validUntil);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
            finally
            {
                Disconnect(cn);
            }
        }

        public void DeletePartyContact(int partyId, int contactId)
        {

            SqlConnection cn = Connect();

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("DeletePartyContact", cn))
                {
                    cmd.Parameters.AddWithValue("@PartyId", partyId);
                    cmd.Parameters.AddWithValue("@ContactId", contactId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
            finally
            {
                Disconnect(cn);
            }
        }

        public DataRow GetSystemConfig(string name)
        {
            SqlConnection cn = Connect();
            DataSet ds = new DataSet();

            try
            {
                
                using (SqlCommand cmd = new SqlCommand("GetSystemConfig", cn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
                return FirstRow(ds);
            }
            finally
            {
                Disconnect(cn);
            }

        }

        public DataRow GetUser(string name)
        {
            SqlConnection cn = Connect();
            DataSet ds = new DataSet();

            try
            {
                using (SqlCommand cmd = new SqlCommand("GetUser", cn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
                return FirstRow(ds);
            }
            finally
            {
                Disconnect(cn);
            }

        }

        public void CreateUser(string name, string fullName, string passwordHash)
        {
            SqlConnection cn = Connect();

            try
            {
                using (SqlCommand cmd = new SqlCommand("CreateUser", cn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
            finally
            {
                Disconnect(cn);
            }

        }

        public void UpdateUser(string name, string fullName, string passwordHash)
        {
            SqlConnection cn = Connect();

            try
            {
                using (SqlCommand cmd = new SqlCommand("UpdateUser", cn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
            finally
            {
                Disconnect(cn);
            }
        }

        public void DeleteUser(string name)
        {
            SqlConnection cn = Connect();

            try
            {
                using (SqlCommand cmd = new SqlCommand("DeleteUser", cn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
            finally
            {
                Disconnect(cn);
            }
        }

        private DataRow FirstRow(DataSet ds)
        {
            return
                ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 ?
                ds.Tables[0].Rows[0] : null;
        }

        private DataTable FirstTable(DataSet ds)
        {
            return
                ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

    }
}
