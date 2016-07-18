using System;
using System.Collections.Generic;
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

/// <summary>
/// Summary
/// </summary>
public class ShiftType
{
    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    // Basic properties
    private int shiftTypeID;
    private int programID = 0;
    private string shiftTypeName = "Unknown";
    private string programName = "Unknown";

    public int ShiftTypeID { get { return shiftTypeID; } }
    public int ProgramID { get { return programID; } }
    public string ShiftTypeName { get { return shiftTypeName; } }
    public string ProgramName { get { return programName; } }

public ShiftType()
	{
		// TODO: Add constructor logic here
	}


public ShiftType(int shiftTypeID, int programID, string shiftTypeName, string programName)
    {
        this.shiftTypeID = shiftTypeID;
        this.shiftTypeName = shiftTypeName;
        this.programName = programName;
        this.programID = programID;
    }

public bool SQL_GetShiftTypeListInto(List<ShiftType> shiftType, int programArea_ProgramID, int selected_CampusID)
{
    string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
    SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

    int shiftTypeID;
    string shiftTypeName;
    string programName;

    try
    {
        connection.Open();
        command.Connection = connection;

        // Build and execute command string
        command.CommandText = "CSSP_GetShiftTypeList";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@Active", SqlDbType.Int).Value = 1;
        command.Parameters.Add("@ProgramsID", SqlDbType.Int).Value = programArea_ProgramID;
        command.Parameters.Add("@CampusID", SqlDbType.Int).Value = selected_CampusID;

        SqlDataReader reader = command.ExecuteReader();

//        shiftType.Add(new ShiftType(-1, -1, "None", "None"));
  //      errorMessage = "CSSP_GetShiftTypeList( 1,"+programArea_ProgramID.ToString()+")";

        while (reader.Read())
        {
            shiftTypeID = Convert.ToInt32(reader["ShiftTypeID"]);
            shiftTypeName = reader["ShiftTypeName"] as string;
            programName = reader["ProgramName"] as string;

            shiftType.Add(new ShiftType(shiftTypeID, programArea_ProgramID, shiftTypeName, programName));

            errorMessage += " "+shiftTypeID.ToString();
        }
    }

    catch (Exception ex)
    {
        errorMessage = ex.Message.ToString();
        shiftType.Add(new ShiftType(-1, -1, "Excep", "Excep"));
        return false;
    }

    finally
    {
        if (connection.State != ConnectionState.Closed) connection.Close();
        command.Dispose();
    }

    return true;
}


 public bool OLDSQL_GetShiftTypeList(List<ShiftType> shiftType)
 {
    string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
    SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

    int shiftTypeID;
    int programID;
    string shiftTypeName;
    string programName;
    int aa = 0;

    try
    {
        connection.Open();
        command.Connection = connection;

        // Build and execute command string
        command.CommandText = "CSSP_GetShiftTypeList";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@ShiftType_Active", SqlDbType.Int).Value = 1;
 
        SqlDataReader reader = command.ExecuteReader();

        shiftType.Add(new ShiftType(-1, -1, "None", "None"));

        while (reader.Read())
        {
            aa += 1;
            if (aa > 10) break;
            
            shiftTypeID = 12;
            programID = 13;
            shiftTypeName = "shiftTypeName";
            programName = "programName";
            shiftTypeID = Convert.ToInt32(reader["ShiftTypeID"]);
  //        programID = Convert.ToInt32(reader["ProgramID"]);

            shiftTypeName = reader["ShiftTypeName"] as string;
            programName = reader["ProgramName"] as string;
            
            shiftType.Add(new ShiftType(shiftTypeID, programID, shiftTypeName, programName));
        }
    }
    
    catch (Exception ex)
    {
        errorMessage = ex.Message.ToString();
        shiftType.Add(new ShiftType(-1, -1, "Excep", "Excep"));
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




