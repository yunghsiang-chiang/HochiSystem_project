using System;
using System.Data;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

public partial class InsertHistory
{
	
	static public void Insert(string TableName, string gHMemberID, string gHOld, string gHNew, string Modifier)
	{
		SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);
		SqlCommand cmd = new SqlCommand("INSERT INTO "+ TableName + " (HMemberID, HCDate, HOld, HNew, HStatus, HCreate, HCreateDT) VALUES (@HMemberID, @HCDate, @HOld, @HNew, @HStatus, @HCreate, @HCreateDT)", ConStr);

		ConStr.Open();
		//cmd.Parameters.AddWithValue("@TableName", TableName);
		cmd.Parameters.AddWithValue("@HMemberID", gHMemberID);
		cmd.Parameters.AddWithValue("@HCDate", DateTime.Now.ToString("yyyy/MM/dd"));
		cmd.Parameters.AddWithValue("@HOld", gHOld);
		cmd.Parameters.AddWithValue("@HNew", gHNew);
		cmd.Parameters.AddWithValue("@HStatus", 1);
		cmd.Parameters.AddWithValue("@HCreate", Modifier);
		cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
		cmd.ExecuteNonQuery();
		ConStr.Close();
		cmd.Cancel();

	}

	
}
