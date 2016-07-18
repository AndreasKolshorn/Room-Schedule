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
public class StudentEditCoursesPossible
{
    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    private int ProgramsID=0;
    private int programShiftReqID=0;
    private string courseID="";

    private string courseName="";
    private string programName="";
    private string department="";

    private int courseTypeID=0;
    private string courseTypeName = "";
    //
    //    public int ProgramShiftReqID { get {return programShiftReqID;}}
    public string CourseID { get { return courseID; } } 
    public string CourseName { get {return courseName;}}
    public string ProgramName  { get {return programName;}}
    public string Department { get {return department;}}
    public int ProgramShiftReqID { get { return programShiftReqID; } }

    private bool checkbox = false;
    public bool Checkbox { get { return checkbox; } }


    public int CourseTypeID { get { return courseTypeID; } }
    public string CourseTypeName { get { return courseTypeName; } }

    public StudentEditCoursesPossible()
	{
	}

    public StudentEditCoursesPossible(
        int programsID,
        int programShiftReqID, 
        string courseID,

        string courseName,
        string programName,
        string department,
        
        int courseTypeID,
        string courseTypeName
        )
    {
        this.ProgramsID = programsID;
        this.programShiftReqID= programShiftReqID;
        this.courseID = courseID;

        this.courseName = courseName;
        this.programName = programName;
        this.department = department;

        this.courseTypeID = courseTypeID;
        this.courseTypeName = courseTypeName;
    }

    public bool SQL_GetStudentsPossibleCourseList(List<StudentEditCoursesPossible> studentCourse, int studentUID)
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

            command.CommandText = "CSSP_GetStudentsPossibleCourseList";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@StudentUid", SqlDbType.Int).Value = studentUID;
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                studentCourse.Add(new StudentEditCoursesPossible(
                    ProgramsID
                    , Convert.ToInt32(reader["ProgramShiftReqID"])
                    , reader["CourseID"] as String

                    , reader["CourseName"] as String
                    , reader["ProgramName"] as String
                    , reader["Department"] as String

                    , Convert.ToInt32(reader["CourseTypeID"])
                    , reader["courseTypeName"] as String
                    ));
                haveRecord = true;
            }

            if (!haveRecord)        // Just in case.
            {
                errorCode = -1;
                errorMessage = "CSSP_GetStudentsPossibleCourseList (" + studentUID.ToString() + ") Returned no records";
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