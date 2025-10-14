using System;
using System.Data;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

public partial class InsertTeamHistory
{
	
	static public void Insert(string TableName, string gHTeamID, string gHOldID, string gHOld, string gHNewID, string gHNew, string Modifier)
	{
		SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);
		SqlCommand cmd = new SqlCommand("INSERT INTO "+ TableName + " (HTeamID, HCDate,HOldID,  HOld,HNewID, HNew, HStatus, HCreate, HCreateDT) VALUES (@HTeamID, @HCDate, @HOldID, @HOld, @HNewID, @HNew, @HStatus, @HCreate, @HCreateDT)", ConStr);

		ConStr.Open();
		//cmd.Parameters.AddWithValue("@TableName", TableName);
		cmd.Parameters.AddWithValue("@HTeamID", gHTeamID);
		cmd.Parameters.AddWithValue("@HCDate", DateTime.Now.ToString("yyyy/MM/dd"));
		cmd.Parameters.AddWithValue("@HOldID", gHOldID);
		cmd.Parameters.AddWithValue("@HOld", gHOld);
		cmd.Parameters.AddWithValue("@HNewID", gHNewID);
		cmd.Parameters.AddWithValue("@HNew", gHNew);
		cmd.Parameters.AddWithValue("@HStatus", 1);
		cmd.Parameters.AddWithValue("@HCreate", Modifier);
		cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
		cmd.ExecuteNonQuery();
		ConStr.Close();
		cmd.Cancel();

	}

	
}
