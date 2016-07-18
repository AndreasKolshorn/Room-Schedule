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
/// Container for data entities representing a single shift. Used by 2d shift grid array which corresponds to the 
/// the 2D gridview UI. Within this container an important entity is the collection of 3 or more ShiftSlot objects.
/// </summary>
public class StudentShift
{
    // Standard error properties
    private string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    private int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    // Basic properties
    private int rowNum = 0;   // alias for Section Ties object to grid cell.
    private int colNum = 0;   // alias for Room

    private int scheduleID = 0;
    private int termCalendarID = 0;
    private int dayTypeID = 0;
    private int shiftTimeID = 0;
    private int roomID = 0;
    private int sectionID = 0;
    private int doctorTypeID = 0;
    private string doctorTypeName="";
    private string doctorTypeCode="";
    private string sroffer_Section = "Unknown";

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
    private string shiftTimeName = "";
    private string shiftStartEnd = "";
    private bool supervisorNeeded = false;

    private int roomGroupID =-1;
    private bool blendedRoom = false;
    private int programsID=-1;
    private string programsCode="";
    private string programName = "";

    private List<ShiftSlot> shiftSlot=new List<ShiftSlot>();
    public List<ShiftSlot> ShiftSlot { get { return shiftSlot; } }

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

    public int RoomID { get { return roomID; } }
    public int SectionID { get { return sectionID; } }
    public string SROffer_Section { get { return sroffer_Section; } }

    public int RoomGroupID { get { return roomGroupID; } }
  //  private bool BlendedRoom { get { return blendedRoom; } }
    private int ProgramsID { get { return programsID; } }
    private string ProgramsCode { get { return programsCode; } }
    private string ProgramName { get { return programName; } }

    public int ShiftTypeID { get { return shiftTypeID; } }
   // public int ShiftTypeID_Old { get { return shiftTypeID_Old; } }

    public int DoctorStatusID { get { return doctorStatusID; } }
   // public int DoctorStatusID_Old { get { return doctorStatusID_Old; } }

    public string DoctorLastName { get { return doctorLastName; } }
    public string DoctorFirstName { get { return doctorFirstName; } }
    public string DoctorDegree { get { return doctorDegree; } }

    public string DoctorTypeName { get { return doctorTypeName; } }
    public string DoctorTypeCode { get { return doctorTypeCode; } }
    public int DoctorTypeID { get { return doctorTypeID; } }

    public string RoomName { get { return roomName; } }
    public string ShiftTypeName { get { return shiftTypeName; } }
    public string ShiftTimeName { get { return shiftTimeName; } }
    public string ShiftStartEnd { get { return shiftStartEnd; } }
    public bool SupervisorNeeded { get { return supervisorNeeded; } }
    
    public string DayName { get { return dayName; } }
    public string DayShort { get { return dayShort; } }

    // CONSTRUCTOR
    public StudentShift() { }

    public StudentShift(int rowNum, int colNum,
        int scheduleID,
        int termCalendarID,
        int dayTypeID,
        int shiftTimeID,
        int roomID,

        int sectionID,
        string sroffer_Section,
        //
        int SROfferID1, int SROfferID2, int SROfferID3, int SROfferID4,
        //
        int shiftTypeID,
        int doctorStatusID,
        //
        int s2SID_1, int s2SID_2, int s2SID_3, int s2SID_4,
        int EnrollmentTypeID_1, int EnrollmentTypeID_2, int EnrollmentTypeID_3, int EnrollmentTypeID_4,
        int studentUID1, int studentUID2, int studentUID3, int studentUID4,
        int programShiftReqID_1, int programShiftReqID_2, int programShiftReqID_3, int programShiftReqID_4,


        int shiftSlotRowID_1, int shiftSlotRowID_2, int shiftSlotRowID_3, int shiftSlotRowID_4,
        int shiftSlotTypeID_1, int shiftSlotTypeID_2, int shiftSlotTypeID_3, int shiftSlotTypeID_4,
        bool timeCardRequired_1, bool timeCardRequired_2, bool timeCardRequired_3, bool timeCardRequired_4,
        bool supervisorNeeded,

        string shiftSlotTypeName1, string shiftSlotTypeName2, string shiftSlotTypeName3, string shiftSlotTypeName4,

        string roomName,
        string shiftTypeName,

        string doctorFirstName,
        string doctorLastName,
        string doctorDegree,

        int doctorTypeID,
        string doctorTypeName,
        string doctorTypeCode,

        string studentName1, string studentName2, string studentName3, string studentName4,
        string studentLName1, string studentLName2, string studentLName3, string studentLName4,

        string dayName,
        string dayShort,
        string shiftTimeName,
        string shiftStartEnd,

        string courseName1,
        string courseName2,
        string courseName3,
        string courseName4,
        
        //        
        int roomGroupID,
        bool blendedRoom,
        int programsID,
        string programsCode,
        string programName,

        int ProgramsID_1,
        int ProgramsID_2,
        int ProgramsID_3,
        int ProgramsID_4,

        string ProgramsCode_1,
        string ProgramsCode_2,
        string ProgramsCode_3,
        string ProgramsCode_4,

        string ProgramsName_1,
        string ProgramsName_2,
        string ProgramsName_3,
        string ProgramsName_4
        )
    {
        this.rowNum = rowNum;
        this.colNum = colNum;
        this.scheduleID = scheduleID;
        this.termCalendarID = termCalendarID;

        this.dayTypeID = dayTypeID;                                                     
        this.shiftTimeID = shiftTimeID;
        this.roomID = roomID;

        this.roomGroupID = roomGroupID;
        this.blendedRoom= blendedRoom;
        this.programsID =programsID;
        this.programsCode =programsCode;
        this.programName = programName;

        this.shiftTypeID = shiftTypeID;
        this.doctorStatusID = this.doctorStatusID_Old = doctorStatusID;

        if (studentUID1 >0 )
            shiftSlot.Add(new 
                ShiftSlot(s2SID_1, EnrollmentTypeID_1, studentUID1, 
                shiftSlotRowID_1, shiftSlotTypeID_1, shiftSlotTypeName1,
                studentName1, programShiftReqID_1, courseName1, timeCardRequired_1,SROfferID1,
                ProgramsID_1, ProgramsCode_1, ProgramsName_1
                ));

        if (studentUID2 > 0)
            shiftSlot.Add(new ShiftSlot(s2SID_2, EnrollmentTypeID_2, studentUID2, 
                shiftSlotRowID_2, shiftSlotTypeID_2, shiftSlotTypeName2,
                studentName2, programShiftReqID_2, courseName2, timeCardRequired_2,SROfferID2,
                ProgramsID_2, ProgramsCode_2, ProgramsName_2
                ));

        if (studentUID3 > 0)
            shiftSlot.Add(new ShiftSlot(s2SID_3, EnrollmentTypeID_3, studentUID3,
                shiftSlotRowID_3, shiftSlotTypeID_3, shiftSlotTypeName3,
                studentName3, programShiftReqID_3, courseName3, timeCardRequired_3,SROfferID3,
                ProgramsID_3, ProgramsCode_3, ProgramsName_3
                ));

        if (studentUID4 > 0)
            shiftSlot.Add(new ShiftSlot(s2SID_4, EnrollmentTypeID_4, studentUID4,
                shiftSlotRowID_4, shiftSlotTypeID_4, shiftSlotTypeName4,
                studentName4, programShiftReqID_4, courseName4, timeCardRequired_4, SROfferID4,
                ProgramsID_4, ProgramsCode_4, ProgramsName_4
                ));

        this.supervisorNeeded = supervisorNeeded;
        this.roomName = roomName;
        this.shiftTypeName = shiftTypeName;
        this.shiftStartEnd = shiftStartEnd;

        this.doctorFirstName = doctorFirstName;
        this.doctorLastName = doctorLastName;

        if (doctorDegree.StartsWith(",") )
            this.doctorDegree = doctorDegree.Substring(1).Trim();
        else
            this.doctorDegree = doctorDegree;

        this.doctorTypeID =  doctorTypeID ;
        this.doctorTypeName = doctorTypeName;
        this.doctorTypeCode= doctorTypeCode;

        this.dayName = dayName;
        this.dayShort = dayShort;
        this.shiftTimeName = shiftTimeName;

        this.sroffer_Section = sroffer_Section;
        this.sectionID = sectionID;
    }
  
    public void SetPrimaryStudentSlot(
        int programShiftReqID, 
        int studentID, 
        int shiftSlotTypeID1)
        {
        /*
        student1_ProgramShiftReqID_Old = this.student1_ProgramShiftReqID;
            this.student1_ProgramShiftReqID = programShiftReqID;

            studentID1_Old = this.studentID1;
            this.studentID1 = studentID;

            this.shiftSlotTypeID1 = shiftSlotTypeID1;
         */
    }
    
    public void SetShiftTypeID(int new_ShiftTypeID)
    {
        // Guarantee the ..Old value is updated.
        this.shiftTypeID_Old = this.shiftTypeID;
        this.shiftTypeID = new_ShiftTypeID;
    }

    /// <summary>
    /// Method to pass user selected values from screen controls to StudentBasic data model objects. 
    /// Run this before saving to database.
    /// </summary>
    public string SetStudentSlotKILL (int slotNumber, CheckBox Student_CheckBox_New, StudentBasic selectedStudent_New)
    {
        /*
        switch (slotNumber)
        {
            case 1:
                this.student1_CheckBox_Old = this.student1_CheckBox;
                this.student1_CheckBox = Student_CheckBox_New.Checked;

                studentID1_Old = this.studentID1;
                student1_ProgramShiftReqID_Old = this.student1_ProgramShiftReqID;

                shiftSlotTypeID1_Old = this.shiftSlotTypeID1;
                shiftSlotTypeName1_Old = this.shiftSlotTypeName1;

                if (this.Student1_CheckBox != this.Student1_CheckBox_Old)
                    if (Student_CheckBox_New.Checked == true)
                    {
                        studentID1 = selectedStudent_New.StudentUID;
                        student1_ProgramShiftReqID = selectedStudent_New.Selected_ProgramShiftReqID;

                        studentName1 = selectedStudent_New.Firstname.Trim() + " " + selectedStudent_New.Lastname.Trim();
                        studentLName1 = selectedStudent_New.Lastname.Trim();

                        shiftSlotTypeID1 = selectedStudent_New.Selected_ShiftSlotTypeID;
                        shiftSlotTypeName1 = selectedStudent_New.Selected_ShiftSlotTypeName;

                        Student_CheckBox_New.Text = selectedStudent_New.Lastname.Trim();
                        //  + "(" +ShiftSlotTypeName1.Substring(0,1) + ")";
                    }
                    else 
                    {
                        student1_ProgramShiftReqID = 0;
                        studentID1 = -1;

                        shiftSlotTypeID1 = 0;
                        shiftSlotTypeName1 = "";
                        Student_CheckBox_New.Text = "Add";

                        studentName1 = "" ;
                        studentLName1 = "";
                    }
                
                break;
            case 2:
                this.student2_CheckBox_Old = this.student2_CheckBox;
                this.student2_CheckBox = Student_CheckBox_New.Checked;

                studentID2_Old = this.studentID2;
                student2_ProgramShiftReqID_Old = this.student2_ProgramShiftReqID;

                shiftSlotTypeID2_Old = this.shiftSlotTypeID2;
                shiftSlotTypeName2_Old = this.shiftSlotTypeName2;

                if (this.Student2_CheckBox != this.Student2_CheckBox_Old)
                    if (Student_CheckBox_New.Checked == true)
                    {
                        studentID2 = selectedStudent_New.StudentUID;
                        student2_ProgramShiftReqID = selectedStudent_New.Selected_ProgramShiftReqID;

                        studentName2 = selectedStudent_New.Firstname.Trim() + " " + selectedStudent_New.Lastname.Trim();
                        studentLName2 = selectedStudent_New.Lastname.Trim();

                        shiftSlotTypeID2 = selectedStudent_New.Selected_ShiftSlotTypeID;
                        shiftSlotTypeName2 = selectedStudent_New.Selected_ShiftSlotTypeName;

                        Student_CheckBox_New.Text = selectedStudent_New.Lastname.Trim();
                        // + "(" + ShiftSlotTypeName2.Substring(0, 1) + ")";
                    }
                    else 
                    {
                        student2_ProgramShiftReqID = 0;
                        studentID2 = -1;

                        shiftSlotTypeID2 = 0;
                        shiftSlotTypeName2 = "";
                        Student_CheckBox_New.Text = "Add";

                        studentName2 = "";
                        studentLName2 = "";
                    }

                break;
            case 3:
                this.student3_CheckBox_Old = this.student3_CheckBox;
                this.student3_CheckBox = Student_CheckBox_New.Checked;

                studentID3_Old = this.studentID3;
                student3_ProgramShiftReqID_Old = this.student3_ProgramShiftReqID;

                shiftSlotTypeID3_Old = this.shiftSlotTypeID3;
                shiftSlotTypeName3_Old = this.shiftSlotTypeName3;

                if (this.Student3_CheckBox != this.Student3_CheckBox_Old)
                    if (Student_CheckBox_New.Checked == true)
                    {
                        studentID3 = selectedStudent_New.StudentUID;
                        student3_ProgramShiftReqID = selectedStudent_New.Selected_ProgramShiftReqID;
                        studentName3 = selectedStudent_New.Firstname + " " + selectedStudent_New.Lastname;
                        studentLName3 = selectedStudent_New.Lastname.Trim();

                        shiftSlotTypeID3 = selectedStudent_New.Selected_ShiftSlotTypeID;
                        shiftSlotTypeName3 = selectedStudent_New.Selected_ShiftSlotTypeName;

                        Student_CheckBox_New.Text = selectedStudent_New.Lastname.Trim();
                        // +"(" + ShiftSlotTypeName3.Substring(0, 1) + ")";
                    }
                    else 
                    {
                        student3_ProgramShiftReqID = 0;
                        studentID3 = -1;

                        shiftSlotTypeID3 = 0;
                        shiftSlotTypeName3 = "";
                        Student_CheckBox_New.Text = "Add";

                        studentName3 = "" ;
                        studentLName3 = "";
                    }                 
                
                break;
            default:
                break;
               

        }
    */
   //     testText = shiftSlotTypeID1.ToString() +" =="+ shiftSlotTypeName1.ToString();
     //   testText += " " + selectedStudent_New.Selected_ShiftSlotTypeID.ToString() + "===" + selectedStudent_New.Selected_ShiftSlotTypeName.ToString();

        return "testText";
    }

    public string SQL_UpdateScheduleCell(StudentShift shift)
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


