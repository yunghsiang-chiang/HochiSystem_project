using System;
using System.Data;
using System.Web;
using System.Data.SqlClient;

public partial class SQLdatabase
{
    #region Connection
    static public string ConnectionString
    {
		get
		{
			return System.Configuration.ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
		}
	}

    static public SqlConnection CreateConnection()
    {
        return new SqlConnection(ConnectionString);
    }
    static public SqlConnection OpenConnection()
    {
        SqlConnection conn = CreateConnection();
        conn.Open();
        return conn;
    }

    static private SqlCommand CreateCommand(SqlConnection conn, SqlTransaction trans, string text, params object[] args)
    {
        SqlCommand cmd = conn.CreateCommand();
        if (args != null && args.Length != 0)
        {
            object[] argnames = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                string name = "@AutoArg" + (i + 1);
                argnames[i] = name;
                cmd.Parameters.AddWithValue(name, args[i]);
            }
            text = string.Format(text, argnames);
        }
        cmd.Connection = conn;
        cmd.Transaction = trans;
        cmd.CommandText = text;
        return cmd;
    }
    static public SqlCommand CreateCommand(SqlConnection conn, string text, params object[] args)
    {
        if (conn == null) throw (new ArgumentNullException("conn"));
        if (text == null) throw (new ArgumentNullException("text"));
        return CreateCommand(conn, null, text, args);
    }
    static public SqlCommand CreateCommand(SqlTransaction trans, string text, params object[] args)
    {
        if (trans == null) throw (new ArgumentNullException("trans"));
        if (text == null) throw (new ArgumentNullException("text"));
        return CreateCommand(trans.Connection, trans, text, args);
    }

    #region No Transaction
    static public SqlDataReader ExecuteReader(string text, params object[] args)
    {
        if (text == null) throw (new ArgumentNullException("text"));
        SqlConnection conn = CreateConnection();
        SqlCommand cmd = CreateCommand(conn, text, args);
        conn.Open();
        return cmd.ExecuteReader(CommandBehavior.CloseConnection);
       //conn.Close();
    }
    static public void ExecuteNonQuery(string text, params object[] args)
    {
        if (text == null) throw (new ArgumentNullException("text"));
        using (SqlConnection conn = OpenConnection())
        {
            SqlCommand cmd = CreateCommand(conn, text, args);
            cmd.ExecuteNonQuery();
        }
    }
    static public object ExecuteScalar(string text, params object[] args)
    {
        if (text == null) throw (new ArgumentNullException("text"));
        using (SqlConnection conn = OpenConnection())
        {

            SqlCommand cmd = CreateCommand(conn, text, args);
            return cmd.ExecuteScalar();
        }
    }

    static public int ExecuteInt32(string text, params object[] args)
    {
        return Convert.ToInt32(ExecuteScalar(text, args));
    }
    static public string ExecuteString(string text, params object[] args)
    {
        object obj = ExecuteScalar(text, args);
        if (obj == null)
            return null;
        return obj.ToString();
    }

    static public DataTable ExecuteDataTable(string text, params object[] args)
    {
        using (SqlDataReader reader = ExecuteReader(text, args))
        {
            DataTable table = new DataTable();
            table.Load(reader);
            return table;
        }
    }
    static public DataRow ExecuteDataRow(string text, params object[] args)
    {
        return GetDataRow(ExecuteDataTable(text, args), false);
    }
    static public DataRow ExecuteDataRow(bool checkOnlyOne, string text, params object[] args)
    {
        return GetDataRow(ExecuteDataTable(text, args), checkOnlyOne);
    }
    #endregion

    #region With Transaction
    static public SqlDataReader ExecuteReader(SqlTransaction trans, string text, params object[] args)
    {
        if (text == null) throw (new ArgumentNullException("text"));
        SqlCommand cmd = CreateCommand(trans, text, args);
        return cmd.ExecuteReader();
    }
    static public void ExecuteNonQuery(SqlTransaction trans, string text, params object[] args)
    {
        if (text == null) throw (new ArgumentNullException("text"));
        SqlCommand cmd = CreateCommand(trans, text, args);
        cmd.ExecuteNonQuery();
    }
    static public object ExecuteScalar(SqlTransaction trans, string text, params object[] args)
    {
        if (text == null) throw (new ArgumentNullException("text"));
        SqlCommand cmd = CreateCommand(trans, text, args);
        return cmd.ExecuteScalar();
    }

    static public int ExecuteInt32(SqlTransaction trans, string text, params object[] args)
    {
        return Convert.ToInt32(ExecuteScalar(trans, text, args));
    }
    static public string ExecuteString(SqlTransaction trans, string text, params object[] args)
    {
        object obj = ExecuteScalar(trans, text, args);
        if (obj == null)
            return null;
        return obj.ToString();
    }

    static public DataTable ExecuteDataTable(SqlTransaction trans, string text, params object[] args)
    {
        using (SqlDataReader reader = ExecuteReader(trans, text, args))
        {
            DataTable table = new DataTable();
            table.Load(reader);
            return table;
        }
    }
    static public DataRow ExecuteDataRow(SqlTransaction trans, string text, params object[] args)
    {
        return GetDataRow(ExecuteDataTable(trans, text, args), false);
    }
    static public DataRow ExecuteDataRow(SqlTransaction trans, bool checkOnlyOne, string text, params object[] args)
    {
        return GetDataRow(ExecuteDataTable(trans, text, args), checkOnlyOne);
    }
    #endregion

    static public DataRow GetDataRow(DataTable table, bool checkOnlyOne)
    {
        if (table == null) throw (new ArgumentNullException("table"));
        int rc = table.Rows.Count;
        if (checkOnlyOne && rc != 1)
        {
            if (rc == 0)
                throw (new Exception("Table contains no row"));
            throw (new Exception("Table contains multi row"));
        }
        if (rc == 0)
            return null;
        return table.Rows[0];
    }
    #endregion

    static public HttpContext Context
    {
        get
        {
            return HttpContext.Current;
        }
    }

    //static public DataRow getCurrentUserRow(string session)
    //{

    //    var row = ExecuteDataRow("SELECT * FROM PMember WHERE paemail={0}", session);
    //    if (row == null) throw (new Exception("No This User"));
    //    return row;
    //}


}
