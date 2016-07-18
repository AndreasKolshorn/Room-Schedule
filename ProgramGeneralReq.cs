using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

/// <summary>
/// Summary description for ProgramGeneralReq
/// </summary>
public class ProgramGeneralReq
{
    // Standard error properties
    private string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    private int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    private int programGeneralReqID;
    private int generalReqTypeID;
    private string generalReqTypeName;
    private decimal generalReqValue;           // Always populated using Program Requirement definition.
  //  private decimal generalReqValueStudentSum; // Students to date quantity towards requirement.
  //  private decimal generalReqValueDifference; // Student to date total minus program requirement. Positive value when program req met.

//private decimal generalReqValueMetByShift;  // Populate if built as part of a shift program requirement list.

 //   private bool waivableReq;
    // Accessors
    public int ErrorCode { get { return errorCode; } }
    public string ErrorMessage { get { return errorMessage; } }


    private string noteText;
    private string description;
    private int sqlDataTypeID;
    private int programShiftReqID;

    public decimal GeneralReqValue_StudentSum;

    public int ProgramGeneralReqID { get { return this.programGeneralReqID; } }
    public int GeneralReqTypeID { get { return this.generalReqTypeID; } }
    public string GeneralReqTypeName { get { return this.generalReqTypeName; } }
    public decimal GeneralReqValue { get { return this.generalReqValue; } }
  //  public decimal GeneralReqValueStudentSum { get { return this.generalReqValueStudentSum; } }
//public decimal GeneralReqValueDifference { get { return this.generalReqValueDifference; } }
   // public decimal GeneralReqValueMetByShift { get { return this.generalReqValueMetByShift; } }

  //  public bool WaivableReq { get { return this.waivableReq; } }

    public int SQLDateTypeID { get { return this.sqlDataTypeID; } }
    public int ProgramShiftReqID { get { return this.programShiftReqID; } }
    public string NoteText { get { return this.noteText; } }
    public string Description { get { return this.description; } }

    public ProgramGeneralReq() { }

    public  ProgramGeneralReq (
        int programGeneralReqID,
        int generalReqTypeID,
        decimal generalReqValue,
        bool waivableReq,
        string noteText,
        string generalReqTypeName,
        string description,
        decimal generalReqValueStudentSum,
        int sqlDataTypeID)
        {
            this.generalReqTypeID = generalReqTypeID;
            this.generalReqTypeName = generalReqTypeName;
            this.generalReqValue = generalReqValue;
        //  this.generalReqValueStudentSum = generalReqValueStudentSum;
        //  this.generalReqValueDifference = generalReqValueStudentSum - GeneralReqValue;
        //  this.waivableReq = waivableReq;

            this.noteText = noteText;
            this.description = description;
            this.sqlDataTypeID = sqlDataTypeID;
         }


    // Get ProgramGeneralReq - Details about state of competencies.
    public bool SQL_GetProgramGeneralReq(List<ProgramGeneralReq> programGeneralReq, int programID, int programTermCalendarID)
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
            command.CommandText = "CSSP_SelectProgramRequirements";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@ProgramsID", SqlDbType.Int).Value = programID;
            command.Parameters.Add("@TermCalendarID", SqlDbType.Int).Value = programTermCalendarID;

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                programGeneralReq.Add(new ProgramGeneralReq(
                    Convert.ToInt32(reader["ProgramGeneralReqID"])
                    , Convert.ToInt32(reader["GeneralReqTypeID"])
                    , Convert.ToDecimal(reader["GeneralReqValue"])
                    , Convert.ToBoolean(reader["WaivableReq"])
                    , reader["noteText"] as String
                    , reader["generalReqTypeName"] as String
                    , reader["description"] as String
                    , 0
                    , Convert.ToInt32(reader["SQLDataTypeID"])
                    ));

                haveRecord = true;
            }

            if (!haveRecord)        // Just in case..
            {
                programGeneralReq.Add(new ProgramGeneralReq(
                    0, 0, 0, Convert.ToBoolean(0),
                    "Error: No Requirements Found",
                    "Error: No Requirements Found",
                    "Error: No Requirements Found", 0, 0));
            }
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed:";
#if DebugOn
                errorMessage = "Request failed: Program.cs/SQL_GetProgramGeneralReq/Catch --> " + ex.Message;
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





}
