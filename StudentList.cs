using System;
using System.Collections.Generic;
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;


/// <summary>
/// Summary description for StudentList
/// </summary>
public class StudentList
{
    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)    //    
    // Basic properties. 

    // Accessors
    public int ErrorCode { get { return errorCode; } }
    public string ErrorMessage { get { return errorMessage; } }

    private List<StudentListItem> items = new List<StudentListItem>();
    public List<StudentListItem> Items {get {return this.items; }}

	public StudentList()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public StudentList(int programID, int campusID)
    {
        SQL_GetStudentsInStudentCourse(programID, campusID);
    }


    private bool SQL_GetStudentsInStudentCourse(int programID, int selected_CampusID)
    {
        // Populate StudentBasic object with basic info for specific student based on a StudentUID

        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Get list of programs that student currently active in.
            command.CommandText = "CSSP_GetStudentsInStudentCourse";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@programsid", SqlDbType.Int).Value = programID;
            command.Parameters.Add("@campusid", SqlDbType.Int).Value = selected_CampusID;

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())    // Build a new program object for each programID 
            {
                Items.Add(new StudentListItem(Convert.ToInt32(reader["studentUID"]), reader["lastname"] as string, reader["firstname"] as string));
            }
            reader.Close();
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed: Set '#define DebugOn' in StudentSchedule.cs for details.";
#if DebugOn
            errorMessage = "Request failed: StudentSchedule.cs/SQL_GetStudentsEligibleForClinic/Catch --> " + ex.Message;
#endif
            return false;
        }

        finally
        {
            //    errorMessage = "CSSP_GetStudentsEligibleForClinic(" + termCalendarID.ToString() + "," + programID.ToString() + ")";

            if (connection.State != ConnectionState.Closed) connection.Close();
        }

        return true;
    }

}