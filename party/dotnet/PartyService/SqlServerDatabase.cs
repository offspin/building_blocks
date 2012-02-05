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
            return new SqlConnection(this.connectionString);
        }

        private void Disconnect(SqlConnection cn)
        {
            if (cn != null) { cn.Close(); }
        }

        public DataRow GetParty(int id)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = Connect();
            try
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("GetParty", cn) )        
                {
                    cmd.Parameters.AddWithValue("@Id", id);
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

        public DataTable GetPartyByName(string name)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = Connect();
            try
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("GetPartyByName", cn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
                return FirstTable(ds);
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
                cn.Open();
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
                cn.Open();
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

        public int CreateBusiness(string name)
        {
            SqlConnection cn = Connect();
           
            try
            {
                cn.Open();
                object newId;

                using (SqlCommand cmd = new SqlCommand("CreateBusiness",cn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int);
                    cmd.Parameters["@Id"].Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Name", name);
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

        public void UpdateBusiness(int id, string name)
        {
            SqlConnection cn = Connect();

            try
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("UpdateBusiness", cn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
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

        public void DeleteParty(int id)
        {
            SqlConnection cn = Connect();

            try
            {
                cn.Open();
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

        public DataRow GetContact(int id)
        {
            SqlConnection cn = Connect();
            DataSet ds = new DataSet();
            try
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("GetContact", cn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
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

        public DataTable GetContactsByPartyId(int partyId)
        {
            SqlConnection cn = Connect();
            DataSet ds = new DataSet();
            try
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("GetContactsByPartyId", cn))
                {
                    cmd.Parameters.AddWithValue("@PartyId", partyId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }

                return FirstTable(ds);
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
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("CreateAddress", cn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int);
                    cmd.Parameters["@Id"].Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Street", street);
                    cmd.Parameters.AddWithValue("@Town", town);
                    cmd.Parameters.AddWithValue("@County", county);
                    cmd.Parameters.AddWithValue("@PostCode", postCode);
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
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("UpdateAddress", cn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Street", street);
                    cmd.Parameters.AddWithValue("@Town", town);
                    cmd.Parameters.AddWithValue("@County", county);
                    cmd.Parameters.AddWithValue("@PostCode", postCode);
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

        public DataRow GetPartyContact(int partyId, int contactId)
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
                return FirstRow(ds);
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
