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
public class ShiftSlotType
{
    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    // Basic properties
    private int shiftSlotTypeID=-1;
    private string shiftSlotTypeName = "N/A";   // Name displayed in dropdown list.
    private string timeSheetName = "";          // Alternate name used on time sheet.
    private string gridName = "";               // Abreviated name used in students grid detail.
    private bool useSubSection = false;       // True means CAMS section code is built from a base + modifier value. e.g. M1+A or M1+B
    private int subSectionTypeID = 0;
    private int programsID = -1;

    public int ProgramsID {get {return programsID;}}  
    public int ShiftSlotTypeID { get { return shiftSlotTypeID; } }
    public string ShiftSlotTypeName { get { return shiftSlotTypeName; } }
    public string TimeSheetName { get { return timeSheetName; } }
    public string GridName { get { return gridName; } }
    public bool UseSubSection { get { return useSubSection; } }
    public int SubSectionTypeID { get { return subSectionTypeID; } }

public ShiftSlotType()
	{
	}

public ShiftSlotType(int programsID, int shiftSlotTypeID, string shiftSlotTypeName, string timeSheetName, string gridName, bool useSubSection, int subSectionTypeID)
    {
        this.programsID = programsID;
        this.shiftSlotTypeID = shiftSlotTypeID;
        this.shiftSlotTypeName = shiftSlotTypeName;
        this.timeSheetName = timeSheetName;
        this.gridName = gridName;
        this.useSubSection = useSubSection;
        this.subSectionTypeID = subSectionTypeID;
}

public bool SQL_GetShiftSlotTypeListInto(List<ShiftSlotType> shiftSlotType, int programsID)
{
    string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
    SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

    int shiftSlotTypeID;
    string shiftSlotTypeName;

    string timeSheetName;
    string gridName;
    bool useSubSection;
    int subSectionTypeID;

    try
    {
        connection.Open();
        command.Connection = connection;

        // Build and execute command string
        command.CommandText = "CSSP_GetShiftSlotTypeList";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add("@ProgramsID", SqlDbType.Int).Value = programsID;
        
        SqlDataReader reader = command.ExecuteReader();

        errorMessage = "CSSP_GetShiftSlotTypeList()";

        while (reader.Read())
        {
            programsID = Convert.ToInt32(reader["programsID"]);

            shiftSlotTypeID = Convert.ToInt32(reader["ShiftSlotTypeID"]);
            shiftSlotTypeName = reader["ShiftSlotTypeName"] as string;
            timeSheetName = reader["timeSheetName"] as string; ;
            gridName = reader["gridName"] as string;
            useSubSection = Convert.ToBoolean(reader["UseSubSection"]);
            subSectionTypeID = Convert.ToInt32(reader["SubSectionTypeID"]);          

            shiftSlotType.Add(new ShiftSlotType(programsID, shiftSlotTypeID, shiftSlotTypeName,
            timeSheetName, gridName, useSubSection, subSectionTypeID));
        }
    }

    catch (Exception ex)
    {
        errorMessage += " Catch Exception="+ ex.Message.ToString();
        shiftSlotType.Add(new ShiftSlotType(0,0,  "No Results","","",false,0));
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




