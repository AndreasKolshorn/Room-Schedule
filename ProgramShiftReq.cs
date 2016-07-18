using System;
using System.Collections.Generic;
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

/// <summary>
/// Summary description for ProgramShiftReq
/// </summary>
public class ProgramShiftReq
{
    private static int getProgcount;
    public int GetProgCount { get { return getProgcount; } }

    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    // Basic properties
    private int collectionID;   // An internal offset into collection of combined primary and secondary or more shift records.
    private int programShiftReqID;
    private int studentShiftHistoryID;
    private int termCalendarID;

    public int ShiftStatus;

    private string courseName;
    private decimal credits;
    private int srMasterID;

    private string courseType;
    private string courseID;   
    private string grade;
    private string studentNote;

    private string department;
    private string section;
    private string facultyName;
    private string termName;
    private int noteID;
    public string noteText;
    private bool studentHasCompleted = false;

    private int sracademicID;
    private int srofferID;
    private int shiftSlotTypeID;
    private string shiftSlotTypeName;
    private decimal subHoursApplied;
    private string subHoursFrom;

    private decimal mainHours;
    private decimal subHours;

    private int primaryContacts;
    private int totalContacts;
    private int fPI;

    public decimal MainHours { get { return mainHours; } set { mainHours = value; } }
    public decimal SubHours { get { return subHours; } set { subHours = value; } }
    public int PrimaryContacts { get { return primaryContacts; } set { primaryContacts = value; } }
    public int TotalContacts { get { return totalContacts; } set { totalContacts = value; } }
    public int FPI { get { return fPI; } set { fPI = value; } }

    public decimal SubHoursApplied { get { return this.subHoursApplied; } set { this.subHoursApplied = value; } }
    public string SubHoursFrom { get { return this.subHoursFrom; } set { this.subHoursFrom = value; } }

    // Structure or List properties. 
    public ContactReq ContactReqForStudent = new ContactReq();
    public int StudentShiftHistoryID { get { return studentShiftHistoryID; } }
    public string ErrorMessage { get { return errorMessage; } }
    public int ErrorCode { get { return errorCode; } }
    //
    // Accessors
    public int CollectionID { get { return this.collectionID; } }
    public int ProgramShiftReqID { get { return this.programShiftReqID; } }
    public string CourseName { get { return this.courseName; } }
    public string CourseID { get { return this.courseID; } }

    public string Department { get { return this.department; } }
    public string Section { get { return this.section; } }
    public string FacultyName { get { return this.facultyName; } }
    public string TermName { get { return this.termName; } }

    public decimal Credits { get { return this.credits; } set { this.credits = value; } }
    public int SRMasterID { get { return this.srMasterID; } }
    //
    public int SracademicID { get { return this.sracademicID; } }
    public int TermCalendarID { get { return this.termCalendarID; } }
    public int SROfferID { get { return this.srofferID; } }
    //
    public string Grade { get { return this.grade; } }
    public bool StudentHasCompleted { get { return this.studentHasCompleted; } }
    public string StudentNote { get { return this.studentNote; } }

    public int ShiftSlotTypeID { get { return this.shiftSlotTypeID; } }
    public string ShiftSlotTypeName { get { return this.shiftSlotTypeName; } }
    
    public ProgramShiftReq(
        int collectionID
        , int programShiftReqID
        , int studentShiftHistoryID
        , string courseName
        , int srMasterID
        , int credits
        , string grade
        , int shiftStatus
        , string department
        , string section
        , string facultyName
        , string termName
        , string courseID
        , string courseType
        , int noteID
        , string noteText
        , decimal ssh_MainHours
        , decimal ssh_SubHours
        , decimal ssh_SubHoursApplied
        , string ssh_SubHoursFrom 
        , int ssh_FPI
        , int ssh_PrimaryContacts
        , int ssh_TotalContacts
        , int shiftSlotTypeID
        , string shiftSlotTypeName
        )
    {
        this.collectionID = collectionID;
        this.programShiftReqID = programShiftReqID;
        this.studentShiftHistoryID = studentShiftHistoryID;
        this.courseName = courseName;
        this.srMasterID = srMasterID;
        this.credits = credits;
        this.grade = grade;
        this.ShiftStatus = shiftStatus;
        this.department = department;
        this.section = section;
        this.facultyName = facultyName;
        this.termName = termName;
        this.courseID = courseID;
        this.courseType = courseType;
        this.noteID = noteID;
        this.noteText = noteText;
        this.mainHours = ssh_MainHours;
        this.subHours = ssh_SubHours;
        this.subHoursApplied = ssh_SubHoursApplied;
        this.subHoursFrom = ssh_SubHoursFrom;
        this.primaryContacts = ssh_PrimaryContacts;
        this.totalContacts = ssh_TotalContacts;
        this.fPI = ssh_FPI; 

        this.ContactReqForStudent.MainHours = 9999;
        this.ContactReqForStudent.SubHours = 9999;
        this.ContactReqForStudent.PrimaryContacts = 9999;
        this.ContactReqForStudent.TotalContacts = 9999;
        this.ContactReqForStudent.SubHours = 9999;
        this.ContactReqForStudent.FPI = 9999;
        this.shiftSlotTypeID = shiftSlotTypeID;
        this.shiftSlotTypeName = shiftSlotTypeName;
    }

    public ProgramShiftReq() { }

    // G E T  ///////////////////////////////////////////////////////
    public bool SQL_GetProgramShiftReq(List<ProgramShiftReq> programShiftReq_List, int programID, int studentUID, int programStart_TermCalendarID, int shiftSlotTypeID)
    {
        // Populate program object with all program shifts required.
        getProgcount++;
        int collectionID = 0;
        bool haveRecord = false;

        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            command.CommandText = "CSSP_SelectShiftsInProgramAndTaken";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@ProgramsID", SqlDbType.Int).Value = programID;
            command.Parameters.Add("@StudentUid", SqlDbType.Int).Value = studentUID;
            //    command.Parameters.Add("@ShiftSlotTypeID", SqlDbType.Int).Value = shiftSlotTypeID;

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                programShiftReq_List.Add(new ProgramShiftReq(
                    collectionID
                    , Convert.ToInt32(reader["programShiftReqID"])
                    , Convert.ToInt32(reader["studentShiftHistoryID"])
                    , reader["CourseName"] as String
                    , Convert.ToInt32(reader["SRMasterID"])
                    , Convert.ToInt32(reader["credits"])
                    , reader["grade"] as String
                    , Convert.ToInt32(reader["shiftStatus"])
                    , reader["department"] as String
                    , reader["section"] as String
                    , reader["facultyName"] as String
                    , reader["termName"] as String

                    , reader["CourseID"] as string
                    , reader["CourseType"] as string
                    , Convert.ToInt32(reader["ssh_noteID"])
                    , reader["ssh_noteText"] as string
                    , Convert.ToDecimal(reader["ssh_MainHours"])
                    , Convert.ToDecimal(reader["ssh_SubHours"])

                    , Convert.ToDecimal(reader["ssh_SubHoursApplied"])
                    , reader["ssh_SubHoursFrom"] as string

                    , Convert.ToInt32(reader["ssh_PrimaryContacts"])
                    , Convert.ToInt32(reader["ssh_TotalContacts"])
                    , Convert.ToInt16(reader["ssh_FPI"])
                    , Convert.ToInt32(reader["shiftSlotTypeID"])
                    , reader["shiftSlotTypeName"] as string
                    ));

                collectionID++;
                haveRecord = true;

            }

            if (!haveRecord)        // Just in case.
            {
                errorCode = -1;
                errorMessage = "CSSP_SelectShiftsInProgramAndTaken " + programID.ToString() + "," + studentUID.ToString();
            }
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed:";
#if DebugOn
                errorMessage = "Request failed: ProgramShiftReq.cs/SQL_GetProgramShiftReq/Catch --> " + ex.Message;
#endif

            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }

        return true;
    }

    #region SQL_UpdateStudentShiftHistory
    public bool SQL_UpdateStudentShiftHistory (
        int studentUID, 
        int studentShiftHistoryID,
        int programShiftReqID,
        int updateUserID,

        decimal mainHours, 
        decimal subHours, 
        decimal subHoursApplied,
        string subHoursFrom,
        int primaryContacts, 
        int fPI
        )
    {
        bool haveRecord = false;

        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            command.CommandText = "CSSP_UpdateStudentShiftHistory";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@studentShiftHistoryID", SqlDbType.Int).Value = studentShiftHistoryID;

            command.Parameters.Add("@TermCalendarID_Old", SqlDbType.Int).Value = 0;
            command.Parameters.Add("@MainHours_Old", SqlDbType.Decimal).Value = 0;
            command.Parameters.Add("@SubHours_Old", SqlDbType.Decimal).Value = 0;
            command.Parameters.Add("@SubHoursApplied_Old", SqlDbType.Decimal).Value = 0;
            command.Parameters.Add("@SubHoursFrom_Old", SqlDbType.VarChar).Value = 0;
            command.Parameters.Add("@PrimaryContacts_Old", SqlDbType.Int).Value = 0;
            command.Parameters.Add("@TotalContacts_Old", SqlDbType.Int).Value = 0;
            command.Parameters.Add("@FPI_Old", SqlDbType.Int).Value = 0;

            command.Parameters.Add("@TermCalendarID", SqlDbType.Int).Value = 0;
            //
            command.Parameters.Add("@MainHours", SqlDbType.Decimal).Value = mainHours;
            command.Parameters.Add("@SubHours", SqlDbType.Decimal).Value = subHours;
            command.Parameters.Add("@SubHoursApplied", SqlDbType.Decimal).Value = subHoursApplied;
            command.Parameters.Add("@SubHoursFrom", SqlDbType.VarChar).Value = subHoursFrom;

            command.Parameters.Add("@PrimaryContacts", SqlDbType.Int).Value = primaryContacts;
            command.Parameters.Add("@TotalContacts", SqlDbType.Int).Value = 0;
            command.Parameters.Add("@FPI", SqlDbType.Int).Value = fPI;



            SqlDataReader reader = command.ExecuteReader();

//            while (reader.Read())
          //  {
            reader.Read();

                // errorCode Values: 0=ok, -1=check actual message, -11= Modified by another user, -10=No Record Found
                errorCode = Convert.ToInt32( reader["errorCode"].ToString() );   
                errorMessage =  reader["errorMessage"].ToString();
                haveRecord = true;
          //  }

            if (!haveRecord)        // Just in case.
            {
            //    errorCode = -1;    // errorMessage = "Request failed: ProgramShiftReq.cs/SQL_UpdateStudentShiftHistory-->No result from DB"; 
                errorCode = -99;   // Possible Values: 0=ok, -1=check actual message, -11=Record modified by another user 
                errorMessage = "PROBLEM";
                haveRecord = false;
            }
        }

        catch (Exception ex)
        {
            errorMessage = "Request failed to update changes.";
//#if DebugOn
                errorMessage = "Request failed: ProgramShiftReq.cs/SQL_UpdateStudentShiftHistory/Catch --> " + ex.Message;
//#endif

            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }

        return true;
    }
    #endregion

    #region SQL_InsertStudentShiftHistory
    public bool SQL_InsertStudentShiftHistory(
        int studentUID, 
        int programShiftReqID,
        int updateUserID,

        decimal mainHours, 
        decimal subHours, 
        decimal subHoursApplied,
        string subHoursFrom,

        decimal primaryContacts, 
        decimal fPI)
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            command.CommandText = "CSSP_InsertStudentShiftHistory";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@StudentUID", SqlDbType.Int).Value = studentUID; 
            command.Parameters.Add("@ProgramShiftReqID", SqlDbType.Int).Value = programShiftReqID ;
            command.Parameters.Add("@UpdateUserID", SqlDbType.Int).Value = updateUserID;

//            command.Parameters.Add("@TermCalendarID", SqlDbType.Int).Value = 0;  //TDO
            command.Parameters.Add("@MainHours", SqlDbType.Decimal).Value = mainHours;
            command.Parameters.Add("@SubHours", SqlDbType.Decimal).Value = subHours;

            command.Parameters.Add("@SubHoursApplied", SqlDbType.Decimal).Value = subHoursApplied;
            command.Parameters.Add("@SubHoursFrom", SqlDbType.VarChar).Value = subHoursFrom;
            command.Parameters.Add("@PrimaryContacts", SqlDbType.Int).Value = primaryContacts;

//            command.Parameters.Add("@TotalContacts", SqlDbType.Int).Value = 0;
            command.Parameters.Add("@FPI", SqlDbType.Int).Value = fPI;

            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            errorCode = Convert.ToInt32(reader["errorCode"]);   
            errorMessage = reader["errorMessage"].ToString();
            this.studentShiftHistoryID = Convert.ToInt32(reader["StudentShiftHistoryID"]);
        }

        catch (Exception ex)
        {
            //            errorCode = -1;
            errorMessage = "Request failed to update changes. "+ ex.Message.ToString();
#if DebugOn
                errorMessage = "Request failed: ProgramShiftReq.cs/SQL_UpdateStudentShiftHistory/Catch --> " + ex.Message;
#endif

            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }

        return true;

    }
    #endregion


}   // End of class ProgramShiftReq






