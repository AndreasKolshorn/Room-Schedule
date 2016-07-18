#define DebugOn
using System;
using System.Collections.Generic;
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

/// <summary>
/// Summary description for StudentInfoReqsList
/// </summary>
public class StudentInfoReqsList
{
    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    private int studentUID;
    private int studentCourseID;
    private int programsID;
    private string programName;
    private int programShiftReqID;
    private string department;
    private string courseID;
    private string courseName;
    private bool active;

    
    public int StudentUID { get { return studentUID; } } 
    public int StudentCourseID { get { return studentCourseID; } } 
    public int ProgramsID { get { return programsID; } } 
    public string ProgramName { get {return programName;}}
    public int ProgramShiftReqID { get { return programShiftReqID; } } 
    public string Department { get {return department;}}
    public string CourseID { get {return courseID;}}
    public string CourseName { get {return courseName ;}}
    public bool Active { get {return active;}}

    //

    public StudentInfoReqsList()
	{
	}

    private StudentInfoReqsList(
         int studentUID,
         int studentCourseID,
         int programsID,
         string programName,
         int programShiftReqID,
         string department,
         string courseID,
         string courseName,
         bool active )
    {
        this.studentUID = studentUID;
        this.studentCourseID = studentCourseID;
        this.programsID = programsID;
        this.programName = programName;
        this.programShiftReqID= programShiftReqID;
        this.department = department;
        this.courseID = courseID;
        this.courseName = courseName;
        this.active = active;
    }

    public bool SQL_GetStudentProgramReqs(List<StudentInfoReqsList> studentInfoReqsList,  int studentUID)
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

            command.CommandText = "CSSP_GetStudentProgramReqs";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@StudentUid", SqlDbType.Int).Value = studentUID;

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                studentInfoReqsList.Add(new StudentInfoReqsList(
                    studentUID,
                    Convert.ToInt32(reader["StudentCourseID"]),
                    Convert.ToInt32(reader["programsID"]),
                    reader["programName"] as String,
                    Convert.ToInt32(reader["programShiftReqID"]),
                    reader["department"] as String,
                    reader["courseID"] as String,
                    reader["courseName"] as String,
                    Convert.ToBoolean(reader["active"])
                    ));
                haveRecord = true;
            }

            if (!haveRecord)        // Just in case.
            {
                errorCode = -1;
                errorMessage = "CSSP_GetStudentProgramReqs (" + studentUID.ToString() +") Returned no records";
            }
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed:";
#if DebugOn
            errorMessage = "Request failed: StudentInfoReqsList.cs/SQL_GetStudentProgramReqs/Catch --> " + ex.Message;
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