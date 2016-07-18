#define DebugOn
using System;
using System.Collections.Generic;
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

/// <summary>
/// This object models a single student uniquely identified by the CAMS StudentUID attribute. 
/// The basic properties of the class appear below and collections are attached with 
/// detail from both ClinicSchedule and CAMS_Enterprise. These lists are available with 
/// public accessors as follows.
/// e.g.  StudentBasic.StudentReqHistory
///       StudentBasic.ProgramList
///
/// </summary>
public class StudentBasic
{
    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)    //    

    // Basic properties. 
    private int studentUId =-1;             // StudentUId assigned to this object by constructor.
    private string lastname = "Unknown";
    private string firstname = "Unknown";
    private string fullname="Unknown";
    private string stNotes = "Unknown";

    private int programCount;           // Number of programs student active in as recorded in CAMS.
    private int programsID = 0;

    private int selected_ProgramShiftReqID = -1;  // Default if user not yet selected a course requirement. 
    private string selected_ProgramShiftName = "N/A";
    private int selected_ShiftSlotTypeID = -1;
    private string selected_ShiftSlotTypeName = "sstn n/a";
    // Structure or List properties. 
    private List<StudentShiftReq> studentShiftReq_List = new List<StudentShiftReq>();
   
    // Accessors
    public int ErrorCode { get { return errorCode; } }
    public string ErrorMessage { get { return errorMessage; } }

    public int StudentUID { get { return studentUId; } }
    public string Lastname { get { return lastname; } }
    public string Firstname { get { return firstname; } }


    public string StNotes { get { return stNotes; } }
//    public string ListName { get { return prettyListName( firstname,lastname, ""); } }
    public string ListName { get { return fullname; } }


    public int ProgramCount { get { return programCount; } }

    public int Selected_ShiftSlotTypeID {
        get { return selected_ShiftSlotTypeID; }
        set { selected_ShiftSlotTypeID = value; }
    }

    public string Selected_ProgramShiftName
    {
        get { return selected_ProgramShiftName; }
        set { selected_ProgramShiftName = value; }
    }

    public string Selected_ShiftSlotTypeName
    {
        get { return selected_ShiftSlotTypeName; }
        set { selected_ShiftSlotTypeName = value; }
    }

    public int Selected_ProgramShiftReqID
    {
        get { return selected_ProgramShiftReqID; }
        set { selected_ProgramShiftReqID = value; }
    }
    
    public List<StudentShiftReq> StudentShiftReq_List { get { return this.studentShiftReq_List; } }   

    // METHODS SECTION
    public StudentBasic(int studentUId, string lastname, string firstname, string fullname)
    {
        this.studentUId = studentUId;
        this.lastname = lastname;
        this.firstname = firstname;
        this.fullname = fullname;
    }

    public StudentBasic() 
        { }

    // Constructor builds out student and program details.
    public StudentBasic(int studentUID, int programsID)              // Populate student object based on StudentUID.
    {
        this.studentUId = studentUID;           // Assign StudentUID to this object.
        this.programsID = programsID;

        if (SQL_GetStudent(studentUID))         // Attempt to get student record basics.
        {
            StudentShiftReq ssr = new StudentShiftReq();
            if (!ssr.SQL_GetStudentShiftsInto(studentShiftReq_List, programsID, studentUID))
            {
                errorCode = -10;    // Messages recent error code (OK=0, ASPX=-1, SQL=-10)    //    
                errorMessage = "Request has failed: StudentBasic.cs/StudentBasic/SQL_GetStudentShiftsInto/StudentUID="
                    + studentUID.ToString() + "< ProgramsID= > "+programsID.ToString()+"<"
                    
                    
                    ;   // Recent message from SQL displayed in orange UI warning bar. 
            }
        }
        else
        {
            errorCode = -10;    // Messages recent error code (OK=0, ASPX=-1, SQL=-10)    //    
            errorMessage = "Request failed: StudentBasic.cs/StudentBasic/SQL_GetStudent/StudentUID=>" + studentUID.ToString() + "<";   // Recent message from SQL displayed in orange UI warning bar. 
        }
    }

    private string prettyListName(string firstname, string lastname, string middlename)
    {
        string prettyName = "";
        if (lastname.Length > 15)
            prettyName = lastname.Substring(0, 15);
        else
            prettyName = lastname;

        if (firstname.Length > 15)
            prettyName += " " + firstname.Substring(0, 15);
        else
            prettyName += " " + firstname;

        if (middlename.Trim().Length > 0)
            prettyName += " " + middlename.Trim().Substring(0, 1) + ".";

        return prettyName;
    }
    
private bool SQL_GetStudent(int studentUID)
    {
        // DWR: Should clean up this function using more standard form e.g. program.SQL_GetProgramContactReq

        // Populate StudentBasic object with basic info for specific student based on a StudentUID
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_SelectStudentByID";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@StudentUid", SqlDbType.Int).Value = studentUID;

            SqlDataReader reader = command.ExecuteReader();

            // Expect one data record or one error record.
            while (reader.Read()) 
            {
                errorCode = (int)reader["ErrorCode"];

                // Populate StudentBasic object with data.
                if (ErrorCode == 0)
                {
                    lastname = reader["LastName"].ToString();
                    firstname = reader["FirstName"].ToString();
                    stNotes = reader["stNotes"].ToString();
                }
                else
                {
                    errorMessage = "No details found for this student";
                    #if DebugOn
                       errorMessage = "Debug: " +reader["ErrorMessage"].ToString();
                    #endif
                }
            }
            reader.Close();
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed:";
            #if DebugOn
                errorMessage = "Request failed: StudentBasic.cs/SQL_GetStudent/Catch --> " + ex.Message;
            #endif

            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
        }
        return true;
    }

public bool SQL_DeleteStudentInSchedule(int s2SID)
{
    bool retVal = false;

    string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
    SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

    try
    {
        connection.Open();
        command.Connection = connection;

        // Build and execute command string
        command.CommandText = "CSSP_DelStudentInSchedule";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@S2SID", SqlDbType.Int).Value = s2SID;

        SqlDataReader reader = command.ExecuteReader();
        reader.Read();

        errorCode = Convert.ToInt32(reader["ErrorCode"]);
        errorMessage = reader["ErrorMessage"] as string;

        if (ErrorCode == 0)
        {
            retVal = true;
        }
    }

    catch (Exception ex)
    {
        errorCode = -1;
        errorMessage = "Request failed:";

#if DebugOn
        errorMessage = "Request failed: StudentBasic.cs/SQL_DeleteStudentOnSchedule/Catch --> " + ex.Message;
#endif

    }

    finally
    {
        if (connection.State != ConnectionState.Closed) connection.Close();
        command.Dispose();

       // errorMessage = "CSSP_DelStudentInSchedule";
    }

    return retVal;
}

public bool SQL_AddStudentToSchedule(
    ref ShiftSlot shiftSlot, 
    int scheduleID,
    int sectionID,
    int termCalendarID,
    string UserID)
{
    string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
    SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

    bool retVal = false;

    try
    {
        connection.Open();
        command.Connection = connection;

        // Build and execute command string
        command.CommandText = "CSSP_InsertStudentInSchedule";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = scheduleID;
        command.Parameters.Add("@StudentUID", SqlDbType.Int).Value =shiftSlot.StudentUID;
        command.Parameters.Add("@ShiftSlotRowID", SqlDbType.Int).Value = shiftSlot.ShiftSlotRowID;
        command.Parameters.Add("@ShiftSlotTypeID", SqlDbType.Int).Value = shiftSlot.ShiftSlotTypeID;
        command.Parameters.Add("@EnrollmentTypeID", SqlDbType.Int).Value = shiftSlot.EnrollmentTypeID;
        command.Parameters.Add("@programShiftReqID", SqlDbType.Int).Value = shiftSlot.ProgramShiftReqID;

        command.Parameters.Add("@SectionID", SqlDbType.Int).Value = sectionID;
        command.Parameters.Add("@TermCalendarID", SqlDbType.Char).Value = termCalendarID;
        command.Parameters.Add("@timeCardRequired", SqlDbType.Bit).Value = shiftSlot.TimeCardRequired;
        command.Parameters.Add("@UserID", SqlDbType.Char).Value = UserID;
 
        SqlDataReader reader = command.ExecuteReader();
        reader.Read();

        errorCode = Convert.ToInt32(reader["ErrorCode"]);
        errorMessage = reader["ErrorMessage"] as string;

        if (ErrorCode == 0)
        {
            shiftSlot.S2SID = Convert.ToInt32(reader["S2SID"]);
            shiftSlot.ShiftSlotRowID = Convert.ToInt32(reader["NewShiftSlotRowID"]);
            shiftSlot.SROfferID = Convert.ToInt32(reader["SROfferID"]);
            retVal = true;
        }
    }

    catch (Exception ex)
    {
        errorCode = -1;
        errorMessage = "Request failed:";
#if DebugOn
        errorMessage = "Request failed: StudentBasic.cs/SQL_InsertStudentInSchedule/Catch --> " + ex.Message;
#endif
        return false;
    }

    finally
    {
        if (connection.State != ConnectionState.Closed) connection.Close();
        command.Dispose();
     }
    return retVal;
}

public bool SQL_UpdateStudentShiftSlot(
    int s2SID,  int OldShiftSlotTypeID, int OldEnrollmentTypeID, bool OldTimeCardRequired, int sectionID,
    int NewShiftSlotTypeID, int NewEnrollmentTypeID, bool NewTimeCardRequired, string userID)
{
    bool retVal = false;

    string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
    SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

    try
    {
        connection.Open();
        command.Connection = connection;

        // Build and execute command string
        command.CommandText = "CSSP_UpdateShiftSlot";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@S2SID", SqlDbType.Int).Value = s2SID;
        command.Parameters.Add("@OldShiftSlotTypeID", SqlDbType.Int).Value = OldShiftSlotTypeID;
        command.Parameters.Add("@OldEnrollmentTypeID", SqlDbType.Int).Value = OldEnrollmentTypeID;
        command.Parameters.Add("@OldTimeCardRequired", SqlDbType.Bit).Value = OldTimeCardRequired;
        command.Parameters.Add("@NewShiftSlotTypeID", SqlDbType.Int).Value = NewShiftSlotTypeID;
        command.Parameters.Add("@NewEnrollmentTypeID", SqlDbType.Int).Value = NewEnrollmentTypeID;
        command.Parameters.Add("@NewTimeCardRequired", SqlDbType.Bit).Value = NewTimeCardRequired;
        command.Parameters.Add("@SectionID", SqlDbType.Int).Value = sectionID;

        command.Parameters.Add("@UserID", SqlDbType.Char).Value = userID;

        SqlDataReader reader = command.ExecuteReader();
        reader.Read();

        errorCode = Convert.ToInt32(reader["ErrorCode"]);
        errorMessage = reader["ErrorMessage"] as string;

        if (ErrorCode == 0)
        {
            retVal = true;
        }
    }

    catch (Exception ex)
    {
        errorCode = -1;
        errorMessage = "Request failed:";
#if DebugOn
        errorMessage = "Request failed: StudentBasic.cs/SQL_UpdateStudentShiftSlot/Catch --> " + ex.Message;
#endif
        return false;
    }

    finally
    {
        if (connection.State != ConnectionState.Closed) connection.Close();
        command.Dispose();
    }
    return retVal;
}



}


