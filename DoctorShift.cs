using System;
using System.Collections.Generic;
using System.Web;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Shift
/// </summary>
public class DoctorShift
{
    // Standard error properties
    private string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    private int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    // Basic properties
    private int rowNum = 0;   // alias for Day/Shift. Ties object to grid cell.
    private int colNum = 0;   // alias for Room

    private int scheduleID = 0;
    private int termCalendarID = 0;
    private int dayTypeID = 0;
    private int shiftTimeID = 0;
    private int roomID = 0;

    private int shiftTypeID = -1;  //ND AOM or whatever
    private int doctorStatusID = -1;

    private string roomName = "";
    private string shiftTypeName = "";

    private string doctorFirstName = "";
    private string doctorLastName = "";
    private string doctorDegree = "";
    private string doctorFullName = "";

    private string dayName = "";
    private string dayShort = "";
    public string shiftTimeName = "";
    private string shiftStartEnd = "";
    private bool supervisorNeeded = false;

    private bool conflict = false; public bool Conflict { get { return conflict; } }
    private int conflictProgramID = 0; public int ConflictProgramID { get { return conflictProgramID; } }
    private string conflictProgramName = ""; public string ConflictProgramName { get { return conflictProgramName; } }

    
    // Used to determine if user selected a new dropdown value.
    private int shiftTypeID_Old = 0;
    private int doctorStatusID_Old = 0;
    
    public int Test;
    public string testText="..";
    // PUBLIC
    public int RowNum { get { return rowNum; } }
    public int ColNum { get { return colNum; } }

    public int ScheduleID { get { return scheduleID; } }
    public int TermCalendarID { get { return termCalendarID; } }

    public int DayTypeID { get { return dayTypeID; } }
    public int ShiftTimeID { get { return shiftTimeID; } }
    public bool SupervisorNeeded { get { return supervisorNeeded; } }

    public int RoomID { get { return roomID; } }

    public int ShiftTypeID { get { return shiftTypeID; } }
    public int ShiftTypeID_Old { get { return shiftTypeID_Old; } }

    public int DoctorStatusID { get { return doctorStatusID; } }
    public int DoctorStatusID_Old { get { return doctorStatusID_Old; } }

    public string DoctorFirstName { get { return doctorFirstName; } set { doctorFirstName = value; } }
    public string DoctorLastName { get { return doctorLastName; } set { doctorLastName = value; } }
  //  public string DoctorFullName { get { return doctorFullName; } set { doctorFullName = value; } }

    public string DoctorDegree { get { return doctorDegree; } }
    public string RoomName { get { return roomName; } }
    public string ShiftTypeName { get { return shiftTypeName; } }
    public string ShiftTimeName { get { return shiftTimeName; } }
    public string ShiftStartEnd { get { return shiftStartEnd; } }

    public string DayName { get { return dayName; } }
    public string DayShort { get { return dayShort; } }

    // CONSTRUCTOR
    public DoctorShift() { }

    public DoctorShift(int rowNum,
        int colNum,
        int scheduleID,
        int termCalendarID,
        int dayTypeID,
        int shiftTimeID,
        int roomID,
        //
        int shiftTypeID,
        int doctorStatusID,
        //
        int studentID1, int studentID2, int studentID3,
        int programShiftReqID_1, int programShiftReqID_2, int programShiftReqID_3, 
        int shiftSlotTypeID_1, int shiftSlotTypeID_2, int shiftSlotTypeID_3,
        string shiftSlotTypeName1, string shiftSlotTypeName2, string shiftSlotTypeName3,

        string roomName,
        string shiftTypeName,
        bool supervisorNeeded,

        string doctorFirstName,
        string doctorLastName,
        string doctorDegree,

        string studentName1, string studentName2, string studentName3,
        string studentLName1, string studentLName2, string studentLName3,


        string dayName,
        string dayShort,
        string shiftTimeName,
        string shiftStartEnd,
        string courseName1,
        string courseName2,
        string courseName3,
        bool conflict, 
        int conflictProgramID,
        string conflictProgramName)
    {
        this.rowNum = rowNum;
        this.colNum = colNum;
        this.scheduleID = scheduleID;
        this.termCalendarID = termCalendarID;

        this.dayTypeID = dayTypeID;                                                     
        this.shiftTimeID = this.shiftTypeID_Old = shiftTimeID;  // prime the pipeline.
        this.roomID = roomID;

        this.shiftTypeID = shiftTypeID;
        this.doctorStatusID = this.doctorStatusID_Old = doctorStatusID;

        this.supervisorNeeded = supervisorNeeded;
        this.roomName = roomName;
        this.shiftTypeName = shiftTypeName;
        this.shiftStartEnd = shiftStartEnd;


        this.doctorFirstName = doctorFirstName;
        this.doctorLastName = doctorLastName;
        this.doctorDegree = doctorDegree;

        if (doctorDegree.StartsWith(","))
            this.doctorDegree = doctorDegree.Substring(1).Trim();
        else
            this.doctorDegree = doctorDegree.Trim();

        this.dayName = dayName;
        this.dayShort = dayShort;
        this.shiftTimeName = shiftTimeName;
        this.conflict = conflict;
        this.conflictProgramID = conflictProgramID;
        this.conflictProgramName = conflictProgramName;
    }

    public void SetDoctorSelected(int ddl_DoctorStatusID, string ddl_DoctorFullName)
    {
        this.doctorStatusID_Old = this.doctorStatusID;
        this.doctorStatusID = ddl_DoctorStatusID;
        this.doctorFullName = ddl_DoctorFullName;
    }

    public void SetShiftTypeSelected(int ddl_ShiftTypeID, string ddl_ShiftTypeName)
    {
        this.shiftTypeID_Old = this.shiftTypeID;
        this.shiftTypeID = ddl_ShiftTypeID;
        this.shiftTypeName = ddl_ShiftTypeName;
    }

   
    private void SetDoctorStatusID(int new_DoctorStatusID)
    {
        // Guarantee the ..Old value is updated.
        this.doctorStatusID_Old = this.doctorStatusID;
        this.doctorStatusID = new_DoctorStatusID;
    }
    
    private void SetShiftTypeID(int new_ShiftTypeID)
    {
        // Guarantee the ..Old value is updated.
        this.shiftTypeID_Old = this.shiftTypeID;
        this.shiftTypeID = new_ShiftTypeID;
    }

    /// <summary>
    /// Method to pass user selected values from screen controls to Student data model objects. 
    /// Run this before saving to database.
    /// </summary>
 
    public string SQL_UpdateCellShiftType(DoctorShift shift)
    {
        string val = "Done";
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_UpdateScheduleCell";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = shift.ScheduleID;
            command.Parameters.Add("@ShiftTypeID", SqlDbType.Int).Value = shift.ShiftTypeID;

            SqlDataReader reader = command.ExecuteReader();
            reader.Read();                 //  val = "Read" + reader["ErrorMessage"];
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed:";
            val = errorMessage;
#if DebugOn
      errorMessage = "Request failed: Doctor.cs/SQL_DeleteDoctorOnSchedule/Catch --> " + ex.Message;
#endif
            return val;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }

        return val;
    }
}


