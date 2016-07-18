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
/// Summary description for StudentInfoReqCore
/// </summary>
public class StudentInfoReqsCore
{
    #region DATA SECTION
    // Standard error properties
    private string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    private int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)    //    

    public int ErrorCode { get { return errorCode; } }
    public string ErrorMessage { get { return errorMessage; } } 


    const int StartProg_TermCalendarID = 100;   // Default starting point, when select from CAMS TermCalendar.

    const string PROGRAM = "Program";
    const string TERM = "Term";
    const string PROGSTATUS = "ProgramStatus";

    public string grid_PROGRAM { get { return PROGRAM; } }
    public string grid_TERM { get { return TERM; } }
    public string grid_PROGSTATUS { get { return PROGSTATUS; } }
 
    // Basic properties. 
    private int studentInfoID = 0;
    private int studentUID = 0;

    private int startProgram_TermCalendarID = 0;    // When Student Started into program.
    private int expectedGrad_TermCalendarID = 0;

    private bool clinicContact = false;
    private bool active = false;
    private string studentInfo_NoteText = "";
    private string lastname = "Unknown";
    private string firstname = "Unknown";
    
    // Accessors
    public int StudentInfoID { get { return studentInfoID; } set { studentInfoID = value; } }
    public int StudentUID { get { return studentUID; } }
    public int StartProgram_TermCalendarID { get { return startProgram_TermCalendarID; } set { startProgram_TermCalendarID = value; } }
    public int ExpectedGrad_TermCalendarID { get { return expectedGrad_TermCalendarID; } set { expectedGrad_TermCalendarID = value; } }

    public bool ClinicContact { get { return clinicContact; } set { clinicContact = value; } }

    public bool Active { get { return active; } set { active = value; } }
    public string StudentInfo_NoteText { get { return studentInfo_NoteText; } set { studentInfo_NoteText = value; } }

    public string Lastname { get { return lastname; } }
    public string Firstname { get { return firstname; } }
 /* OLD DATA
    private List<StudentInfoStatus> studentInfoStatusSQLBuffer = new List<StudentInfoStatus>();
    public List<StudentInfoStatus> StudentInfoStatusSQLBuffer { get { return this.studentInfoStatusSQLBuffer; } }

    private List<StudentInfoStatus> studentInfoStatus = new List<StudentInfoStatus>();
    public List<StudentInfoStatus> StudentInfoStatus { get { return this.studentInfoStatus; } }
    
    private List<ProgramStatus> programStatusSQLBuffer = new List<ProgramStatus>();
    public List<ProgramStatus> ProgramStatusSQLBuffer { get { return this.programStatusSQLBuffer; } }

    private List<StudentInfoSQLBuffer> studentInfoSQLBuffer = new List<StudentInfoSQLBuffer>();
    public List<StudentInfoSQLBuffer> StudentInfoSQLBuffer { get { return this.studentInfoSQLBuffer; } }
   
    // Each entry in tblStudentCertType should be represented with a list.
    private List<StudentInfoCert> studentInfoCert_CPR = new List<StudentInfoCert>();
    public List<StudentInfoCert> StudentInfoCert_CPR { get { return this.studentInfoCert_CPR; } }

    private List<StudentInfoCert> studentInfoCert_FirstAid = new List<StudentInfoCert>();
    public List<StudentInfoCert> StudentInfoCert_FirstAid { get { return this.studentInfoCert_FirstAid; } }

    private List<StudentInfoCert> studentInfoCert_WSP = new List<StudentInfoCert>();
    public List<StudentInfoCert> StudentInfoCert_WSP { get { return this.studentInfoCert_WSP; } }

    private List<StudentInfoCert> studentInfoCert_HepB = new List<StudentInfoCert>();
    public List<StudentInfoCert> StudentInfoCert_HepB { get { return this.studentInfoCert_HepB; } }

    private List<StudentInfoCert> studentInfoCert_TB = new List<StudentInfoCert>();
    public List<StudentInfoCert> StudentInfoCert_TB { get { return this.studentInfoCert_TB; } }

    private List<TermCalendarItem> startProgram = new List<TermCalendarItem>();
    public List<TermCalendarItem> StartProgram { get { return this.startProgram; } }
    */

    private List<StudentInfoReqsList> studentInfoSQLBuffer = new List<StudentInfoReqsList>();
    public List<StudentInfoReqsList> StudentInfoSQLBuffer { get { return this.studentInfoSQLBuffer; } }

    private DataTable studentInfoStatusAsTable = new DataTable();
    public DataTable StudentInfoStatusAsTable { get { return this.studentInfoStatusAsTable; } }


    #endregion

    public StudentInfoReqsCore()
	{
	}

    public StudentInfoReqsCore(int studentUID)
	{
/*

        SQL_GetStudentProgramReqData(studentInfoSQLBuffer, studentUID);


      //  SQL_GetStudentStatusData(studentInfoStatusSQLBuffer, StudentInfoStatus, studentUID);
      //  SQL_GetProgramStatusData(programStatusSQLBuffer);

        PopulateObjectsFromBuffer(studentInfoSQLBuffer);
        Copy_StudentStatusList_Into_DataTable(studentInfoStatusAsTable, studentInfoStatus);

        TermCalendarItem startProg = new TermCalendarItem();
        startProg.SQL_GetTermsAfter(startProgram, StartProg_TermCalendarID);
  
 * 
 */ }

    public void SaveStudentInfo()
    {
        /*
     //   if (StudentInfoID > 0)
      //  {
            SQL_UpdateStudentInfo();

            int rowcount = 0;
            for (rowcount = 0; rowcount < studentInfoStatus.Count; rowcount++)
                SQL_UpdateStudentStatus(
                    studentInfoStatus[rowcount].StudentStatusID
                    ,studentInfoStatus[rowcount].ProgramStatusID
                    ,studentInfoStatusSQLBuffer[rowcount].ProgramStatusID);
     /*   }
        else
        {
            SQL_InsertStudentInfo();
        }
         */
    }

    public void Copy_StudentStatusList_Into_DataTable(DataTable studentStatusAsTable, List<StudentInfoStatus> studentInfoStatus)
    {
        studentStatusAsTable.Clear();
        studentStatusAsTable.Columns.Clear();

        // Add standard row header column
        DataColumn dcol = new DataColumn(PROGRAM, typeof(System.String));
        studentStatusAsTable.Columns.Add(dcol);

        dcol = new DataColumn(TERM, typeof(System.String));
        studentStatusAsTable.Columns.Add(dcol);

        dcol = new DataColumn(PROGSTATUS, typeof(System.Int16));
        studentStatusAsTable.Columns.Add(dcol);

        foreach (StudentInfoStatus DS in studentInfoStatus)
        {
            DataRow drow = studentStatusAsTable.NewRow();
            drow[PROGRAM] =  DS.ProgramsCode;
            drow[TERM] = "All";// DS.TermCalendarName;
            drow[PROGSTATUS] = DS.ProgramStatusID;
            //
            studentStatusAsTable.Rows.Add(drow);
        }
    }


    private bool SQL_GetProgramStatusData(List<ProgramStatus> programStatusSQLBuffer)
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_GetProgramStatus";
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            errorCode = Convert.ToInt32(reader["errorCode"]);
            errorMessage = reader["errorMessage"] as string;

            if (errorCode != 0) return false;

            do
            {
                programStatusSQLBuffer.Add(new ProgramStatus(
                    Convert.ToInt32(reader["ProgramStatusID"]),
                    Convert.ToString(reader["ProgramStatusName"])
                    ));
            }
            while (reader.Read());
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage += "Request failed: StudentInfoCore.cs/SQL_GetProgramtStatusData/Catch --> " + ex.Message;
            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }
        return true;
    }

    private bool SQL_UpdateStudentInfo()
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_UpdateStudentInfo";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@StudentUID", SqlDbType.Int).Value = this.studentUID;
            //
            /*
            command.Parameters.Add("@newStartProgram_TermCalendarID", SqlDbType.Int).Value = this.startProgram_TermCalendarID;
            command.Parameters.Add("@oldStartProgram_TermCalendarID", SqlDbType.Int).Value = studentInfoSQLBuffer[0].StartProgram_TermCalendarID;

            command.Parameters.Add("@newStudentInfo_NoteText", SqlDbType.VarChar).Value = this.studentInfo_NoteText;
            command.Parameters.Add("@oldStudentInfo_NoteText", SqlDbType.VarChar).Value = studentInfoSQLBuffer[0].StudentInfo_NoteText;

            command.Parameters.Add("@newExpectedGrad_TermCalendarID", SqlDbType.Int).Value = this.ExpectedGrad_TermCalendarID;
            command.Parameters.Add("@oldExpectedGrad_TermCalendarID", SqlDbType.Int).Value = studentInfoSQLBuffer[0].ExpectedGrad_TermCalendarID;

            command.Parameters.Add("@newActive", SqlDbType.Bit).Value = this.active;
            command.Parameters.Add("@oldActive", SqlDbType.Bit).Value = studentInfoSQLBuffer[0].Active;


            command.Parameters.Add("@newClinicContact", SqlDbType.Bit).Value = this.clinicContact;
            command.Parameters.Add("@oldClinicContact", SqlDbType.Bit).Value = studentInfoSQLBuffer[0].ClinicContact;

            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            errorCode = Convert.ToInt32(reader["errorCode"]);
            errorMessage = reader["errorMessage"] as string;

            errorMessage +=
            this.studentUID.ToString()
            + "stProg= " +
                this.startProgram_TermCalendarID.ToString() + " " +
              studentInfoSQLBuffer[0].StartProgram_TermCalendarID.ToString() + ">" +

this.studentInfo_NoteText.ToString() + "< NOTE >" +
    studentInfoSQLBuffer[0].StudentInfo_NoteText + "<  ExpGrad>" +

            this.ExpectedGrad_TermCalendarID.ToString() + "<>" +
studentInfoSQLBuffer[0].ExpectedGrad_TermCalendarID.ToString() + "< Active>" +
            this.active.ToString() + "<>" +
            studentInfoSQLBuffer[0].Active.ToString();
*/
            if (errorCode != 0) return false;

        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage += "Request failed: StudentInfoCore.cs/CSSP_UpdateStudentInfo/Catch --> " + ex.Message;
            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }
              
             
        return true;
    }
    

    private bool SQL_UpdateStudentStatus(int studentStatusID, int newProgramStatusID, int oldProgramStatusID)
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;


            // Build and execute command string
            command.CommandText = "CSSP_UpdateStudentStatus";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@StudentStatusID", SqlDbType.Int).Value = studentStatusID;
            command.Parameters.Add("@StudentUID", SqlDbType.Int).Value = this.studentUID;
            //
            command.Parameters.Add("@newProgramStatusID", SqlDbType.Int).Value = newProgramStatusID;
            command.Parameters.Add("@oldProgramStatusID", SqlDbType.Int).Value = oldProgramStatusID;
            command.Parameters.Add("@UserID", SqlDbType.VarChar).Value = "Guest";
            
            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            errorCode = Convert.ToInt32(reader["errorCode"]);
            errorMessage += reader["errorMessage"] as string;

            if (errorCode != 0) return false;
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage += "Request failed: StudentInfoCore.cs/SQL_UpdateStudentStatus/Catch --> " + ex.Message;
            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }
        return true;
    }
    
    private bool SQL_InsertStudentInfo()
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_InsertStudentInfo";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@StudentUID", SqlDbType.Int).Value = this.studentUID;
            //
            command.Parameters.Add("@newStartProgram_TermCalendarID", SqlDbType.Int).Value = this.startProgram_TermCalendarID;
            command.Parameters.Add("@newStudentInfo_NoteText", SqlDbType.VarChar).Value = this.studentInfo_NoteText;
            command.Parameters.Add("@newExpectedGrad_TermCalendarID", SqlDbType.Int).Value = this.startProgram_TermCalendarID;

            command.Parameters.Add("@newClinicContact", SqlDbType.Bit).Value = this.ClinicContact;
            command.Parameters.Add("@newActive", SqlDbType.Bit).Value = this.active;
            command.Parameters.Add("@UserID", SqlDbType.VarChar).Value = "DWR:No User";


            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            errorCode = Convert.ToInt32(reader["errorCode"]);
            errorMessage = reader["errorMessage"] as string;

            errorMessage +=
            this.studentUID.ToString()
            + "stProg= ";
            /*+
            this.startProgram_TermCalendarID.ToString() + " " +
              studentInfoSQLBuffer[0].StartProgram_TermCalendarID.ToString() + ">" +

this.studentInfo_NoteText.ToString() + "< NOTE >" +
    studentInfoSQLBuffer[0].StudentInfo_NoteText + "<  ExpGrad>" +

            this.ExpectedGrad_TermCalendarID.ToString() + "<>" +
studentInfoSQLBuffer[0].ExpectedGrad_TermCalendarID.ToString() + "< Active>" +
            this.active.ToString() + "<>" +
            studentInfoSQLBuffer[0].Active.ToString();
         */   

            if (errorCode != 0) return false;

            studentInfoID = Convert.ToInt32(reader["StudentInforID"]);
// TODO: Update the object
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage += "Request failed: StudentInfoCore.cs/CSSP_UpdateStudentInfo/Catch --> " + ex.Message;
            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }
        return true;
    }

    private void PopulateObjectsFromBuffer(List<StudentInfoSQLBuffer> studentInfoSQLBuffer)
    {
        // Grab non-list items from first row (or any since repeated)
        this.studentInfoID = studentInfoSQLBuffer[0].StudentInfoID;
        this.studentUID = studentInfoSQLBuffer[0].StudentUID;
        this.startProgram_TermCalendarID = studentInfoSQLBuffer[0].StartProgram_TermCalendarID;
        this.expectedGrad_TermCalendarID = studentInfoSQLBuffer[0].ExpectedGrad_TermCalendarID;
        this.clinicContact = studentInfoSQLBuffer[0].ClinicContact;

        this.active = studentInfoSQLBuffer[0].Active;
        this.studentInfo_NoteText = studentInfoSQLBuffer[0].StudentInfo_NoteText;
        this.lastname = studentInfoSQLBuffer[0].Lastname;
        this.firstname = studentInfoSQLBuffer[0].Firstname;
/*
        // Grab and sort each row item to a particular list.
        foreach (StudentInfoSQLBuffer sb in studentInfoSQLBuffer)
        {
            switch (sb.StudentCertTypeID)
            {
                case CertType_CPR:
                    studentInfoCert_CPR.Add(new StudentInfoCert (true ,sb.BufferRow, sb.StudentCertTypeID, sb.StudentCertTypeName,  sb.AniversaryDate, sb.ExpireDate, sb.Waived));
                    break;

                case CertType_FirstAid:
                    studentInfoCert_FirstAid.Add(new StudentInfoCert (true, sb.BufferRow, sb.StudentCertTypeID, sb.StudentCertTypeName, sb.AniversaryDate, sb.ExpireDate, sb.Waived));
                    break;

                case CertType_HepB:
                    studentInfoCert_HepB.Add(new StudentInfoCert (true, sb.BufferRow, sb.StudentCertTypeID, sb.StudentCertTypeName, sb.AniversaryDate, sb.ExpireDate, sb.Waived));
                    break;

                case CertType_TB:
                    studentInfoCert_TB.Add(new StudentInfoCert (true, sb.BufferRow, sb.StudentCertTypeID, sb.StudentCertTypeName, sb.AniversaryDate, sb.ExpireDate, sb.Waived));
                    break;

                case CertType_WSP:
                    studentInfoCert_WSP.Add(new StudentInfoCert (true, sb.BufferRow, sb.StudentCertTypeID, sb.StudentCertTypeName, sb.AniversaryDate, sb.ExpireDate, sb.Waived));
                    break;
                default:
                    break;
            }    
        }

        if (studentInfoCert_CPR.Count==0)
            studentInfoCert_CPR.Add(new StudentInfoCert(false ,-1, 0, "", System.DateTime.Now, System.DateTime.Now, false));

        if (studentInfoCert_FirstAid.Count == 0)
            studentInfoCert_FirstAid.Add(new StudentInfoCert(false, -1, 0, "", System.DateTime.Now, System.DateTime.Now, false));

        if (studentInfoCert_HepB.Count == 0)
            studentInfoCert_HepB.Add(new StudentInfoCert(false, -1, 0, "", System.DateTime.Now, System.DateTime.Now, false));

        if (studentInfoCert_TB.Count == 0)
            studentInfoCert_TB.Add(new StudentInfoCert(false, -1, 0, "", System.DateTime.Now, System.DateTime.Now, false));

        if (studentInfoCert_WSP.Count == 0)
            studentInfoCert_WSP.Add(new StudentInfoCert(false, -1, 0, "", System.DateTime.Now, System.DateTime.Now, false));
        */
   }




}