using System;
using System.Collections.Generic;
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Doctor
/// </summary>
public class EnrollmentType
{
    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    // Basic properties
    private int enrollmentTypeID=0;
    private string enrollmentTypeName = "Unknown";
    private string description = "Unknown";

    public int EnrollmentTypeID { get { return enrollmentTypeID; } }
    public string EnrollmentTypeName { get { return enrollmentTypeName; } }
    public string Description { get { return description; } }

public EnrollmentType()
	{
		// TODO: Add constructor logic here
	}

public EnrollmentType(int enrollmentTypeID, string enrollmentTypeName, string description)
    {
        this.enrollmentTypeID = enrollmentTypeID;
        this.enrollmentTypeName = enrollmentTypeName;
        this.description = description;
    }

public bool SQL_GetEnrollmentTypeListInto(List<EnrollmentType> enrollmentType)
{
    string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
    SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

    try
    {
        connection.Open();
        command.Connection = connection;

        // Build and execute command string
        command.CommandText = "CSSP_GetEnrollmentTypeList";
        command.CommandType = CommandType.StoredProcedure;
        // command.Parameters.Add("@Active", SqlDbType.Int).Value = 1;
        SqlDataReader reader = command.ExecuteReader();

//      errorMessage = "CSSP_GetShiftTypeList( 1,"+programArea_ProgramID.ToString()+")";

        while (reader.Read())
        {
            enrollmentTypeID = Convert.ToInt32(reader["EnrollmentTypeID"]);
            enrollmentTypeName = reader["EnrollmentTypeName"] as string;
            description = reader["Description"] as string;

            enrollmentType.Add(new EnrollmentType(enrollmentTypeID, enrollmentTypeName, description));
            errorMessage += " "+EnrollmentTypeID.ToString();
        }
    }

    catch (Exception ex)
    {
        errorMessage = ex.Message.ToString();
        enrollmentType.Add(new EnrollmentType(-1, "-1", "Exception"));
        return false;
    }

    finally
    {
        if (connection.State != ConnectionState.Closed) connection.Close();
        command.Dispose();
    }

    return true;
}


}




