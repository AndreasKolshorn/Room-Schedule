using System;
using System.Collections.Generic;
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

/// <summary>
/// This object contains the detail about a specific competency.
/// The sublety is that compentency completion happens in a particular shift
/// but the total is for the particular program.
/// </summary>
public class StudentGeneralHistory
{
    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)
    //
    public int recCount;
    //
    private  int studentUID;
    private  int studentGeneralHistoryID;
    private decimal student_GeneralReqValue;
    private DateTime student_GeneralReqDate;
    
    private bool student_WaivedReq;
    private int student_WaivedReqValue;

    private int student_TermCalendarID;     // When student met this competency.
    private string student_TermText;
    private int student_NoteID;
    private string student_NoteText;
    //
    private int programsID;
    private int programGeneralReqID;
    private int programStart_TermCalendarID;   // The program reqs used based on enrollment in program.
    private string programStart_TermText;
    private decimal program_GeneralReqValue;

    private int sqlDataTypeID;
    private int program_isWaivableReq;
    private string program_NoteText;
    private int generalReqTypeID;
    private string generalReqTypeName;
    private string generalReq_Description;
    private decimal needed_GeneralReqValue;

    public int StudentUID {get;set;}
    public int StudentGeneralHistoryID { get { return this.studentGeneralHistoryID; } }
    public int SQLDateTypeID { get { return this.sqlDataTypeID; } }

    public decimal Student_GeneralReqValue { 
        get { return this.student_GeneralReqValue; } 
        set { this.student_GeneralReqValue=Student_GeneralReqValue;}}

    public  DateTime Student_GeneralReqDate {get;set;}  

    public  bool Student_WaivedReq {get;set;}
    public  int Student_WaivedReqValue {get;set;}

    public  int Student_TermCalendarID {get;set;}     // When student met this competency.
    public  string Student_TermText {get;set;}
    public  int Student_NoteID {get;set;}
    public  string Student_NoteText {get;set;}
    //
    public  int ProgramsID {get;set;}
    public int ProgramGeneralReqID { get { return this.programGeneralReqID; } }
    public  int ProgramStart_TermCalendarID {get;set;} // The program reqs used based on enrollment in program.
    public  string ProgramStart_TermText {get;set;}
    
    public decimal Program_GeneralReqValue { get { return this.program_GeneralReqValue; } }

    public  int Program_isWaivableReq {get;set;}
    public  string Program_NoteText {get;set;}
    public  int GeneralReqTypeID {get;set;}
    public string GeneralReqTypeName { get { return this.generalReqTypeName; } }
    public  string GeneralReq_Description {get;set;}
    public decimal Needed_GeneralReqValue { get { return this.needed_GeneralReqValue; } }
 
    // Accessors
    public int ErrorCode { get { return errorCode; } }
    public string ErrorMessage { get { return errorMessage; } }
    
    public StudentGeneralHistory() { }

    public StudentGeneralHistory(
         int studentUID
        ,int studentGeneralHistoryID
        ,decimal student_GeneralReqValue
        ,DateTime student_GeneralReqDate
        ,bool student_WaivedReq
        ,int student_WaivedReqValue

        ,int student_TermCalendarID     // When student met this competency.
        ,string student_TermText
        ,int student_NoteID
        ,string student_NoteText
    //
        ,int programsID
        ,int programGeneralReqID
        ,int programStart_TermCalendarID   // The program reqs used based on enrollment in program.
        ,string programStart_TermText
        ,decimal program_GeneralReqValue

        ,int program_isWaivableReq
        ,string program_NoteText
        ,int generalReqTypeID
        ,string generalReqTypeName
        ,string generalReq_Description
        , int sqlDataTypeID
        )
    {
        this.studentUID = studentUID;
        this.studentGeneralHistoryID = studentGeneralHistoryID;

        this.student_GeneralReqValue = student_GeneralReqValue;
        this.student_GeneralReqDate = student_GeneralReqDate;

        this.student_WaivedReq = student_WaivedReq;
        this.student_WaivedReqValue = student_WaivedReqValue;

        this.student_TermCalendarID = student_TermCalendarID;
        this.student_TermText = student_TermText;
        this.student_NoteID = student_NoteID;
        this.student_NoteText = student_NoteText;
    //
        this.programsID = programsID;
        this.programGeneralReqID = programGeneralReqID;
        this.program_GeneralReqValue = program_GeneralReqValue;
        this.programStart_TermCalendarID = programStart_TermCalendarID;
        this.programStart_TermText = programStart_TermText;


        this.program_isWaivableReq = program_isWaivableReq;
        this.program_NoteText = program_NoteText;
        this.generalReqTypeID = generalReqTypeID;
        this.generalReqTypeName = generalReqTypeName;
        this.generalReq_Description = generalReq_Description;
        this.sqlDataTypeID = sqlDataTypeID;

        // Calculate difference of required and completed. Pretty up negatives to zero.
        this.needed_GeneralReqValue = program_GeneralReqValue - student_GeneralReqValue;
        if (this.needed_GeneralReqValue < 0) this.needed_GeneralReqValue = 0; 
    }

    // G E T //////////////////////////////////////////////////////////////
    
    // Get ProgramGeneralReq - Details about state of competencies.
    public bool SQL_GetStudentGeneralHistory(
        List<StudentGeneralHistory> studentGeneralHistory, int studentUID, int programID, int programStart_TermCalendarID,
        List<ProgramGeneralReq> programGeneralReq_List)
    {
        // Populate Student object with basic info for specific student based on a StudentUID
        bool haveRecord = false;

        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_GetStudentGeneralHistory";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@ProgramsID", SqlDbType.Int).Value = programID;     //27
            command.Parameters.Add("@StudentUID", SqlDbType.Int).Value = studentUID;
            command.Parameters.Add("@ProgramStart_TermCalendarID", SqlDbType.Int).Value = programStart_TermCalendarID;  // 120;

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                studentGeneralHistory.Add(new StudentGeneralHistory(
                     Convert.ToInt32(reader["StudentUID"])
                    ,Convert.ToInt32(reader["StudentGeneralHistoryID"])
                    ,Convert.ToDecimal(reader["Student_GeneralReqValue"])
                    ,Convert.ToDateTime(reader["Student_GeneralReqDate"])

                    ,Convert.ToBoolean(reader["Student_WaivedReq"])
                    ,Convert.ToInt32(reader["Student_WaivedReqValue"])

                    ,Convert.ToInt32(reader["Student_TermCalendarID"])
                    ,reader["Student_TermText"] as String
                    ,Convert.ToInt32(reader["Student_NoteID"])
                    ,reader["Student_NoteText"] as String
                    //
                    ,Convert.ToInt32(reader["programsID"])
                    ,Convert.ToInt32(reader["ProgramGeneralReqID"])
                    ,Convert.ToInt32(reader["programStart_TermCalendarID"])
                    ,reader["programStart_TermText"] as String
                    ,Convert.ToDecimal(reader["program_GeneralReqValue"])

                    ,Convert.ToInt32(reader["program_isWaivableReq"])
                    ,reader["program_NoteText"] as String
                    ,Convert.ToInt32(reader["generalReqTypeID"])
                    ,reader["generalReqTypeName"] as String
                    ,reader["generalReq_Description"] as String
                    ,Convert.ToInt32(reader["SQLDataTypeID"])
                    ));

                // Grab sum of requirements in program and save for display info.
                foreach (ProgramGeneralReq pgr in programGeneralReq_List)
                    if (pgr.GeneralReqTypeID == Convert.ToInt32(reader["generalReqTypeID"]))
                        pgr.GeneralReqValue_StudentSum += (Convert.ToDecimal(reader["Student_GeneralReqValue"])+1);
                
                haveRecord = true;
            }

            if (!haveRecord)        // Just in case..
            {
                studentGeneralHistory.Add(new StudentGeneralHistory( 
                    0, 0, 0, Convert.ToDateTime(0) 
                    ,Convert.ToBoolean(0), 0
                    ,0, "Error: No Requirements Found",0,"N/A"
                    ,0,0,0,"",0
                    ,0,"",0,"","",0
                    ));
            }
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed:";
//#if DebugOn
            errorMessage = "Request failed: StudentGeneralHistory.cs/SQL_GetStudentGeneralHistory/Catch --> " + ex.Message;
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

    // U P D A T E  ///////////////////////////////////////////////////////
    // U P D A T E  ///////////////////////////////////////////////////////
    public bool SQL_UpdateStudentGeneralHistory(
        int studentGeneralHistoryID,
        decimal student_GeneralReqValue)
    {
        bool haveRecord = false;

        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            command.CommandText = "CSSP_UpdateStudentGeneralHistory";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@StudentGeneralHistoryID", SqlDbType.Int).Value = studentGeneralHistoryID;
            command.Parameters.Add("@Student_GeneralReqValue", SqlDbType.Decimal).Value = student_GeneralReqValue;

            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            // errorCode Values: 0=ok, -1=check actual message, -11= Modified by another user, -10=No Record Found
            errorCode = Convert.ToInt32(reader["errorCode"].ToString());
            errorMessage = reader["errorMessage"].ToString();
            haveRecord = true;

            if (!haveRecord)        // Just in case.
            {
                //    errorCode = -1;    // errorMessage = "Request failed: StudentGeneralHistory.cs/SQL_UpdateStudentShiftHistory-->No result from DB"; 
                errorCode = -99;   // Possible Values: 0=ok, -1=check actual message, -11=Record modified by another user 
                errorMessage = "PROBLEM";
                haveRecord = false;
            }
        }

        catch (Exception ex)
        {
            errorMessage = "Request failed to update changes.";
            //#if DebugOn
            errorMessage = "Request failed: StudentGeneralHistory.cs/SQL_UpdateStudentShiftHistory/Catch --> " + ex.Message;
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


    // I N S E R T ///////////////////////////////////////////////////////
    // I N S E R T ///////////////////////////////////////////////////////
    public bool SQL_InsertStudentGeneralHistory(
        int studentUID, 
        int termcalendarID,
        int programGeneralReqID,
        decimal generalReqValue)
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;
            //
            command.CommandText = "CSSP_InsertStudentGeneralHistory";
            command.CommandType = CommandType.StoredProcedure;
            //
            command.Parameters.Add("@StudentUID", SqlDbType.Int).Value = studentUID;
            command.Parameters.Add("@TermCalendarID", SqlDbType.Int).Value = termcalendarID;
            command.Parameters.Add("@ProgramGeneralReqID", SqlDbType.Int).Value = programGeneralReqID;
            command.Parameters.Add("@UpdateUserID", SqlDbType.Int).Value = 0;
            command.Parameters.Add("@GeneralReqValue", SqlDbType.Decimal).Value = generalReqValue;
            //
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            //
            errorCode = Convert.ToInt32(reader["errorCode"]);
            errorMessage = reader["errorMessage"].ToString();
        }

        catch (Exception ex)
        {
            //errorCode = -1;
            errorMessage = "Request failed to update changes. " + ex.Message.ToString();
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

    public void SetStudentGeneralHistory
        (string GeneralReqTypeName ,
        int Program_GeneralReqValue,
        int Student_GeneralReqValue,
        decimal Needed_GeneralReqValue)
        { }
}
