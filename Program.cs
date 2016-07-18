#define DebugOn
using System;
using System.Collections.Generic;
using System.Web;
//
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

/// <summary>
///  Program object describes the requirements of a program associated with a particular student. 
///  Actual requirements depend when student entered program and determine which reqs they still
///  need to meet.
/// </summary>

    public class Program
    {   
        // Standard error properties
        private string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
        private int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

        // Basic properties
        private int programID;              // One ID per instance of Program object.
        private string programCode;
        private string programName;
        private string department;          // DWR: Currently not ret val from SP. Probably unnecessary!!
        private int noteID;
        private string noteText;
        private int programStart_TermCalendarID;
        private decimal program_GeneralReqValue_SUM;
        private decimal student_GeneralReqValue_SUM;

        // Structure or List properties. 
        private ContactReq contactReqInProgram = new ContactReq();
        private ContactReq contactReqSumForStudent = new ContactReq();

        private List<ProgramShiftReq> programShiftReq_List = new List<ProgramShiftReq>();   // Program Shift reqs come from tblProgramShiftReq and depend on when student entered that program. e.g. clinic 1, clinic 2 etc. 
        private List<ProgramShiftReq> programShiftReq_ListSecondary = new List<ProgramShiftReq>();   // Program Shift reqs come from tblProgramShiftReq and depend on when student entered that program. e.g. clinic 1, clinic 2 etc. 
        private List<ProgramGeneralReq> programGeneralReq = new List<ProgramGeneralReq>();   // Do
        private List<StudentGeneralHistory> studentGeneralHistory_List = new List<StudentGeneralHistory>();   // Do
     //   private List<ProgramGeneralReq> studentGeneralReq = new List<ProgramGeneralReq>();   // Do
public int CntTemp;
public int CntField;

        // Accessors
        public int ProgramID { get { return programID; } }
        public string ProgramCode { get { return programCode; } }
        public string ProgramName { get { return programName; } }
        public string Department { get { return department; } }
        public string NoteText { get { return noteText; } }
        public int NoteID { get { return noteID; } }   

        public string ErrorMessage { get { return errorMessage; } }
        public int ErrorCode { get { return errorCode; } }

        public ContactReq ContactReqInProgram { get { return this.contactReqInProgram; } }
        public ContactReq ContactReqSumForStudent { get { return this.contactReqSumForStudent; } }
        
        public List<ProgramShiftReq> ProgramShiftReq { get { return this.programShiftReq_List; } }
 
        public List<ProgramGeneralReq> ProgramGeneralReq { get { return this.programGeneralReq; } }
    //  public List<ProgramGeneralReq> StudentGeneralReq { get { return this.studentGeneralReq; } }
        public List<StudentGeneralHistory> StudentGeneralHistory { get { return this.studentGeneralHistory_List; } }

        // Methods
        // Constructor gets details about a specific programs student is enrolled in.
        public Program(int studentUID, int programID, int programStart_TermCalendarID)      // Start Term in program
        {
            this.programID = programID;
            this.programStart_TermCalendarID = programStart_TermCalendarID;

            // List of general requirements, some also called competencies.
            ProgramGeneralReq pgr = new ProgramGeneralReq();           // What is required by program.
            ProgramShiftReq psr = new ProgramShiftReq();
            StudentGeneralHistory sgh = new StudentGeneralHistory();   // What has been completed by student.

            // Load Student's program data from database.
            pgr.SQL_GetProgramGeneralReq(programGeneralReq, programID, programStart_TermCalendarID);
            psr.SQL_GetProgramShiftReq (programShiftReq_List, programID, studentUID, programStart_TermCalendarID, (int) Globals.ShiftSlotTypeID_Primary);
 
            sgh.SQL_GetStudentGeneralHistory(studentGeneralHistory_List, studentUID, programID, programStart_TermCalendarID, ProgramGeneralReq);

            // CntTemp = programShiftReq_List.Count; 
            //programGeneralReq.Count;   
            CntTemp = studentGeneralHistory_List.Count;

            errorMessage = psr.errorMessage;
            errorCode = psr.errorCode;

            SQL_GetProgramContactReq(programID, programStart_TermCalendarID);
            // SQL_GetStudentGeneralReq (programID, programStart_TermCalendarID);
            // SQL_GetStudentShiftReq (programID, programStart_TermCalendarID);
        }
        
    private bool SQL_GetProgramContactReq(int programID, int startIn_TermCalendarID)
    {
            bool haveRecord = false;

            string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
            SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
            System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

            try
            {
                connection.Open();
                command.Connection = connection;

                // Build and execute command string
                command.CommandText = "CSSP_GetProgramContactReq";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@ProgramsID", SqlDbType.Int).Value = programID;
                command.Parameters.Add("@StartIn_TermCalendarID", SqlDbType.Int).Value = startIn_TermCalendarID;

                SqlDataReader reader = command.ExecuteReader();

                // Expect one record
                while (reader.Read())
                {
                    this.programCode = reader["ProgramCode"] as string;
                    this.programName = reader["ProgramName"] as string;
                    this.department = reader["Department"] as string;

                    contactReqInProgram.MainHours = Convert.ToDecimal(reader["MainHours"]);
                    contactReqInProgram.SubHours = Convert.ToDecimal(reader["SubHours"]);
                    contactReqInProgram.PrimaryContacts = Convert.ToInt32(reader["PrimaryContacts"]);
                    contactReqInProgram.TotalContacts = Convert.ToInt32(reader["TotalContacts"]);
                    
                    this.noteID = Convert.ToInt32(reader["noteID"]);
                    this.noteText = reader["noteText"] as string;
                    haveRecord = true;
                }

                if (!haveRecord)        // Just in case..
                {
                    this.programCode = "ProgramCode N/A";
                    this.programName = "ProgramName N/A";
                    this.department = "Department N/A";
                }
            }

            catch (Exception ex)
            {
                this.programCode = "ProgramCode??";
                this.programName = "ProgramName??";
                this.department = "Department??";

                errorCode = -1;
                errorMessage = "Request failed:";
                #if DebugOn
                    errorMessage = "Request failed: Program.cs/SQL_GetProgramContactReq/Catch --> " + ex.Message;
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


