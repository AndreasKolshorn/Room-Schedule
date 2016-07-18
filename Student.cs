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
/// e.g.  Student.StudentReqHistory
///       Student.ProgramList
///
/// </summary>
public class Student
{
    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)    //    

    // Basic properties. 
    private int studentUId =-1;             // StudentUId assigned to this object by constructor.
    private string lastname = "Unknown";
    private string firstname = "Unknown";
    private string stNotes = "Unknown";
 //   private bool ferpa = false;
   // private int programCount;           // Number of programs student active in as recorded in CAMS.
    private int programsID = 0;

    private int selected_ProgramShiftReqID = -1;  // Default if user not yet selected a course requirement. 
    private string selected_ProgramShiftName = "N/A";
    private int selected_ShiftSlotTypeID = -1;
    private string selected_ShiftSlotTypeName = "sstn n/a";
    // Structure or List properties. 
    private List<Program> program_List = new List<Program>();
    private List<StudentShiftReq> studentShiftReq_List = new List<StudentShiftReq>();

    private ContactReq contactReqTotalForStudent = new ContactReq();
   
    // Accessors
    public int ErrorCode { get { return errorCode; } }
    public string ErrorMessage { get { return errorMessage; } }

    public int StudentUID { get { return studentUId; } }
    public string Lastname { get { return lastname; } }
    public string Firstname { get { return firstname; } }
    public string StNotes { get { return stNotes; } }
//    public int ProgramCount { get { return programCount; } }

    public string ListName { get { return prettyListName(firstname, lastname, ""); } }


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
    
    public List<Program> ProgramList { get { return this.program_List; } }
    public List<StudentShiftReq> StudentShiftReq_List { get { return this.studentShiftReq_List; } }   
    public ContactReq ContactReqTotalForStudent { get { return contactReqTotalForStudent; } }


    // METHODS SECTION
    public Student( int studentUId ,string lastname,    string firstname)
    {
        this.studentUId = studentUId;
        this.lastname = lastname;
        this.firstname = firstname;
    }

    public Student() 
        { }
 
    // Constructor builds out student and program details.
    public Student(int studentUID)              // Populate student object based on StudentUID.
    {
        this.studentUId = studentUID;           // Assign StudentUID to this object.

        if (SQL_GetStudent(studentUID))         // Attempt to get student record basics.
        {
            SQL_GetStudentPrograms(studentUID); // Attach programs and shifts required, completed, in progress.
            Calculate(contactReqTotalForStudent);
        }
        else
        {
            errorCode = -10;    // Messages recent error code (OK=0, ASPX=-1, SQL=-10)    //    
            errorMessage = "Request failed: Student.cs/Student()/StudentUID >" + studentUID.ToString() + "<";   // Recent message from SQL displayed in orange UI warning bar. 
        }

    }
    /*
    // Constructor builds out student and program details.
    public Student(int studentUID, int programsID)              // Populate student object based on StudentUID.
    {
        this.studentUId = studentUID;           // Assign StudentUID to this object.
        this.programsID = programsID;

        if (SQL_GetStudent(studentUID))         // Attempt to get student record basics.
        {
            StudentShiftReq ssr = new StudentShiftReq();
            if (!ssr.SQL_GetStudentShiftsInto(studentShiftReq_List, programsID, studentUID))
            {
                errorCode = -10;    // Messages recent error code (OK=0, ASPX=-1, SQL=-10)    //    
                errorMessage = "Request failed: Student.cs/Student(,)SQL_GetStudentShiftsInto/StudentUID= >" + studentUID.ToString() + "<";   // Recent message from SQL displayed in orange UI warning bar. 
            }
        }
        else
        {
            errorCode = -10;    // Messages recent error code (OK=0, ASPX=-1, SQL=-10)    //    
            errorMessage = "Request failed: Student.cs/Student(,)/SQL_GetStudent()/StudentUID >" + studentUID.ToString() + "<";   // Recent message from SQL displayed in orange UI warning bar. 
        }
    }
    */

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


    private void Calculate(ContactReq contactReqTotalForStudent)
    {
        contactReqTotalForStudent.MainHours = 0;
        contactReqTotalForStudent.PrimaryContacts = 0;
        contactReqTotalForStudent.SubHours = 0;
        contactReqTotalForStudent.TotalContacts = 0;
        
        foreach (Program program in ProgramList)
        {
            contactReqTotalForStudent.MainHours += program.ContactReqInProgram.MainHours ;
            contactReqTotalForStudent.PrimaryContacts += program.ContactReqInProgram.PrimaryContacts;
            contactReqTotalForStudent.SubHours += program.ContactReqInProgram.SubHours;
            contactReqTotalForStudent.TotalContacts += program.ContactReqInProgram.TotalContacts;
        }
    }
    
private bool SQL_GetStudent(int studentUID)
    {
        // DWR: Should clean up this function using more standard form e.g. program.SQL_GetProgramContactReq

        // Populate Student object with basic info for specific student based on a StudentUID
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

                // Populate Student object with data.
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
                errorMessage = "Request failed: Student.cs/SQL_GetStudent/Catch --> " + ex.Message;
            #endif

            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
        }
        return true;
    }

private bool SQL_GetStudentPrograms(int studentUID)
    {
        // Populate Student object with basic info for specific student based on a StudentUID
          int programCount = 0;
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Get list of programs that student currently active in.
            command.CommandText = "CSSP_SelectStudentPrograms";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@StudentUid", SqlDbType.Int).Value = studentUID;

            SqlDataReader reader = command.ExecuteReader();
  

            while (reader.Read())    // Build a new program object for each programID 
            {                                                           
                program_List.Add(new Program(studentUID, Convert.ToInt32(reader["ProgramsID"]), Convert.ToInt32(reader["StartIn_TermCalendarID"] )));
            programCount+=1;     
            }
            reader.Close();
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed:";
            #if DebugOn
                errorMessage = "Request failed: Student.cs/SQL_GetStudent/Catch --> " + ex.Message;
            #endif

            return false;
        }

            
        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            errorMessage = "Err "+program_List[0].ErrorMessage;

            errorMessage = "Load Student Programs("+programCount.ToString()+")";

        }

        return true;
    }


public void SQL_DeleteStudentOnSchedule(int studentUID, int scheduleID, int ShiftSlotTypeID)
{
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
        command.Parameters.Add("@StudentUID", SqlDbType.Int).Value = studentUID;
        command.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = scheduleID;
        command.Parameters.Add("@ShiftSlotTypeID", SqlDbType.Int).Value = ShiftSlotTypeID;

        SqlDataReader reader = command.ExecuteReader();
        reader.Read();                 //  val = "Read" + reader["ErrorMessage"];
    }

    catch (Exception ex)
    {
        errorCode = -1;
        errorMessage = "Request failed:";

#if DebugOn
        errorMessage = "Request failed: Student.cs/SQL_DeleteStudentOnSchedule/Catch --> " + ex.Message;
#endif

    }

    finally
    {
        if (connection.State != ConnectionState.Closed) connection.Close();
        command.Dispose();

        errorMessage = "CSSP_DelStudentInSchedule";
    }

}

public bool SQL_AddStudentToSchedule(int studentID, int scheduleID,  int shiftSlotTypeID, int programShiftReqID, int SROfferID)
{
    string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
    SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

    try
    {
        connection.Open();
        command.Connection = connection;

        // Build and execute command string
        command.CommandText = "CSSP_InsertStudentInSchedule";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@StudentUID", SqlDbType.Int).Value = studentID;
        command.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = scheduleID;
       // command.Parameters.Add("@Slot", SqlDbType.Int).Value = slot;

        command.Parameters.Add("@ShiftSlotTypeID", SqlDbType.Int).Value = shiftSlotTypeID;
        command.Parameters.Add("@programShiftReqID", SqlDbType.Int).Value = programShiftReqID;
        command.Parameters.Add("@SROfferID", SqlDbType.Int).Value = SROfferID;

        SqlDataReader reader = command.ExecuteReader();

        reader.Read();
    }

    catch (Exception ex)
    {
        errorCode = -1;
        errorMessage = "Request failed:";
#if DebugOn
        errorMessage = "Request failed: Student.cs/SQL_AddStudentToSchedule/Catch --> " + ex.Message;
#endif
        return false;
    }

    finally
    {
        if (connection.State != ConnectionState.Closed) connection.Close();
        command.Dispose();
        errorMessage = "CSSP_InsertStudentInSchedule(SID " +
            studentID.ToString() + ", schid " +
            scheduleID.ToString() + ", slot " +
      //      slot.ToString() + ", shiftslot " +
            shiftSlotTypeID.ToString() + ", progshifreqid " +
            programShiftReqID.ToString() + ", sroffer " +
            SROfferID.ToString();

    }
    return true;
}

}


