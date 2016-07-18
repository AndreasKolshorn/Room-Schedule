#define DebugOn
using System;
using System.Collections.Generic;
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

/// <summary>
/// Summary description for StudentCourseCLM as use in the Course List Management tool (CLM)
/// </summary>
public class StudentEditCoursesCurrent
{
    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    // Essential Attributes
    private int studentCourseID;
    private int programsID=0;
    private int programShiftReqID=0;
    private string courseID="";
    private string courseName="";
    private string programName="";
    private string department="";
    private string grade = "";
    private string term = "";
    private int courseTypeID = 0;
    private int statusTypeID = 0;
    private int srOfferID = 0;
    // Flags to track model state against changes in the UI checkboxes.
    private bool courseIsInDB = false;        // Indicates this course is in tblStudentCourse. True if exists there.
                                              // Used to differentiate from courses user wants to add to tblStudentCourse.
    private bool removeCourseFromDB = false;  // Action to perform on this course if CourseIsInDB=true. 
    private bool courseInSchedule = false;
    // Publics   
    public int StudentCourseID { get { return studentCourseID; }}
    public int ProgramsID { get { return programsID; } }
    public int ProgramShiftReqID { get {return programShiftReqID;}}
    public string CourseID { get { return courseID; } } 
    public string CourseName { get {return courseName;}}
    public string ProgramName  { get {return programName;}}
    public string Department { get {return department;}}
    public string Grade { get { return grade; } }
    public string Term { get { return term; } }
    public int CourseTypeID { get { return courseTypeID; } }
    public int StatusTypeID {  get { return statusTypeID; } }
    public int SROfferID { get { return srOfferID; } }

    //
    public bool CourseIsInDB { get { return courseIsInDB; } }
    public bool RemoveCourseFromDB { get { return removeCourseFromDB; } }
    public bool CourseInSchedule { get { return courseInSchedule; } }

    public  StudentEditCoursesCurrent () 	{}

    
       
    public StudentEditCoursesCurrent
    (   int studentCourseID,
        int programsID,
        int programShiftReqID, 
        string courseID,
        string courseName,
        string programName,
        string department,
        bool courseIsInDB,          // Course came from DB and not one just moved from possible courses list.
        bool removeCourseFromDB,     // This course ready to be removed from students DB course list.
        bool courseInSchedule,      // This course has been scheduled somewhere and cannot be removed. 
        string grade,
        int courseTypeID,
        string textTerm,
        int statusTypeID,
        int srOfferID
    )
    {
        this.studentCourseID = studentCourseID;
        this.programsID = programsID;
        this.programShiftReqID= programShiftReqID;
        this.courseID = courseID;
        this.courseName = courseName;
        this.programName = programName;
        this.department = department;
        this.courseIsInDB = courseIsInDB;
        this.removeCourseFromDB = removeCourseFromDB;
        this.courseInSchedule = courseInSchedule;
        this.grade = grade;
        this.courseTypeID = courseTypeID;
        this.term=textTerm;
        this.statusTypeID = statusTypeID;
        this.srOfferID = srOfferID;
    }
       
    public bool SQL_GetStudentsCurrentCourseList(List<StudentEditCoursesCurrent> studentCourse, int studentUID)
    {
        // Populate program object with all program shifts required.
        bool haveRecord = false;

        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            command.CommandText = "CSSP_GetStudentsCurrentCourseList";
            command.CommandType = CommandType.StoredProcedure;
            // command.Parameters.Add("@ProgramsID", SqlDbType.Int).Value = programID;
            command.Parameters.Add("@StudentUid", SqlDbType.Int).Value = studentUID;
            //    command.Parameters.Add("@ShiftSlotTypeID", SqlDbType.Int).Value = shiftSlotTypeID;

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                studentCourse.Add(new StudentEditCoursesCurrent(
                      Convert.ToInt32(reader["StudentCourseID"])
                    , ProgramsID
                    , Convert.ToInt32(reader["ProgramShiftReqID"])
                    , reader["CourseID"] as String
                    , reader["CourseName"] as String
                    , reader["ProgramName"] as String
                    , reader["Department"] as String
                    , true       // Course is from the Database, it wasn't inserted from possible course list.
                    , false      // Course default is not to delete it during a save.
                    , Convert.ToBoolean(reader["CourseInSchedule"])
                    , reader["Grade"] as String
                    , Convert.ToInt32(reader["CourseTypeID"])
                    , reader["textterm"] as String
                    , Convert.ToInt32(reader["StatusTypeID"])
                    , Convert.ToInt32(reader["SROfferID"])
                    ));
                haveRecord = true;
            }

            if (!haveRecord)        // Just in case.
            {
                errorCode = -1;
                errorMessage = "CSSP_GetStudentsCurrentCourseList (" + studentUID.ToString() + ") Returned no records";
            }
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed:";
#if DebugOn
            errorMessage = "Request failed: StudentShiftReq.cs/SQL_GetStudentShiftReq/Catch --> " + ex.Message;
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

