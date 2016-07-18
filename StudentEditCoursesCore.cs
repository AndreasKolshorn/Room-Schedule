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
/// Summary description for StudentEditCoursesCore
/// </summary>
public class StudentEditCoursesCore
{
    const string COURSEID = "CourseID";
    const string COURSENAME = "CourseName";
    const string PROGRAMNAME = "ProgramName";
    const string DEPARTMENT = "Department";
    const string GRADE = "Grade";
    const string PROG_DEPT = "ProgDept";
    const string TERM = "Term";

    public string CourseID { get { return COURSEID; } }
    public string CourseName { get { return COURSENAME; } }
    public string ProgramName { get { return PROGRAMNAME; } }
    public string Department { get { return DEPARTMENT; } }
    public string Grade { get { return GRADE; } }
    public string Prog_Dept { get { return PROG_DEPT; } }
    public string Term { get { return TERM; } }


    #region DATA SECTION
    // Standard error properties
    private string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    private int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)    //    
    private int studentUID;

    public int ErrorCode { get { return errorCode; } }
    public string ErrorMessage { set { errorMessage = value; } get { return errorMessage; } }

    // Basic properties. 
    // Lists and arrays.
    private List<StudentEditCoursesCurrent> currentCourse_List = new List<StudentEditCoursesCurrent>(); // Courses currently in the students course list.
    private List<StudentEditCoursesPossible> possibleCourse_List = new List<StudentEditCoursesPossible>(); // Courses that can be added to students course list.

    private DataTable currentCourseListAsTable = new DataTable();   // Used as gridview.datasource friendly version of shiftGrid.
    private DataTable possibleCourseListAsTable = new DataTable();   // Used as gridview.datasource friendly version of shiftGrid.

    // Accessors
    public List<StudentEditCoursesCurrent> CurrentCourse_List { get { return this.currentCourse_List; } }
    public List<StudentEditCoursesPossible> PossibleCourse_List { get { return this.possibleCourse_List; } }

    public DataTable CurrentCourseListAsTable { get { return this.currentCourseListAsTable; } }
    public DataTable PossibleCourseListAsTable { get { return this.possibleCourseListAsTable; } }

    public int StudentUID { get { return this.studentUID; } }

    #endregion

    public StudentEditCoursesCore()
	{
	}

    public StudentEditCoursesCore(int studentUID)
	{
        StudentEditCoursesCurrent cc = new StudentEditCoursesCurrent();
        StudentEditCoursesPossible cp = new StudentEditCoursesPossible();
        this.studentUID = studentUID;

        cc.SQL_GetStudentsCurrentCourseList(CurrentCourse_List, studentUID);
        cp.SQL_GetStudentsPossibleCourseList(PossibleCourse_List, studentUID);

        Copy_CurrentCourseList_IntoDataTable(currentCourseListAsTable, CurrentCourse_List);
        Copy_PossibleCourseList_IntoDataTable(possibleCourseListAsTable, PossibleCourse_List);
    }

    public void SaveStudentInfo()
    {
    }

    public void Copy_CurrentCourseList_IntoDataTable(DataTable currentCourseListAsTable, List<StudentEditCoursesCurrent> currentCourse_List)
    {
        currentCourseListAsTable.Clear();
        currentCourseListAsTable.Columns.Clear();

        // Add standard row header column
        DataColumn dcol = new DataColumn(COURSEID, typeof(System.String));
        currentCourseListAsTable.Columns.Add(dcol);

        dcol = new DataColumn(COURSENAME, typeof(System.String));
        currentCourseListAsTable.Columns.Add(dcol);

        dcol = new DataColumn(PROGRAMNAME, typeof(System.String));
        currentCourseListAsTable.Columns.Add(dcol);

        dcol = new DataColumn(DEPARTMENT, typeof(System.String));
        currentCourseListAsTable.Columns.Add(dcol);

        dcol = new DataColumn(GRADE, typeof(System.String));
        currentCourseListAsTable.Columns.Add(dcol);

        dcol = new DataColumn(PROG_DEPT, typeof(System.String));
        currentCourseListAsTable.Columns.Add(dcol);

        dcol = new DataColumn(TERM, typeof(System.String));
        currentCourseListAsTable.Columns.Add(dcol);

        foreach (StudentEditCoursesCurrent ccl in currentCourse_List)
        {
            DataRow drow = currentCourseListAsTable.NewRow();

            drow[COURSEID] = ccl.CourseID;
            drow[COURSENAME] = ccl.CourseName;
            drow[PROGRAMNAME] = ccl.ProgramName;
            drow[DEPARTMENT] = ccl.Department;
            drow[PROG_DEPT] = ccl.ProgramName + " (" + ccl.Department + ")";
            drow[GRADE] = ccl.Grade;
            drow[TERM] = ccl.Term;
            currentCourseListAsTable.Rows.Add(drow);
        }
    }

    public void Copy_PossibleCourseList_IntoDataTable(DataTable possibleCourseListAsTable, List<StudentEditCoursesPossible> possibleCourse_List)
    {
        possibleCourseListAsTable.Clear();
        possibleCourseListAsTable.Columns.Clear();

        // Add standard row header column
        DataColumn dcol = new DataColumn(COURSEID, typeof(System.String));
        possibleCourseListAsTable.Columns.Add(dcol);

        dcol = new DataColumn(COURSENAME, typeof(System.String));
        possibleCourseListAsTable.Columns.Add(dcol);

        dcol = new DataColumn(PROGRAMNAME, typeof(System.String));
        possibleCourseListAsTable.Columns.Add(dcol);

        dcol = new DataColumn(DEPARTMENT, typeof(System.String));
        possibleCourseListAsTable.Columns.Add(dcol);

        dcol = new DataColumn(PROG_DEPT, typeof(System.String));
        possibleCourseListAsTable.Columns.Add(dcol);

        foreach (StudentEditCoursesPossible ccl in possibleCourse_List)
        {
            DataRow drow = possibleCourseListAsTable.NewRow();

            drow[COURSEID] = ccl.CourseID;
            drow[COURSENAME] = ccl.CourseName;
            drow[PROGRAMNAME] = ccl.ProgramName;
            drow[DEPARTMENT] = ccl.Department;
            drow[PROG_DEPT] = ccl.ProgramName + " (" + ccl.Department + ")";

            possibleCourseListAsTable.Rows.Add(drow);
        }
    }

    /*
    public void Copy_CurrentCoursesCheckboxes_IntoModel(GridView StudentCurrentCoursesGridview, List<StudentEditCoursesCurrent> currentCourse_List )
    {
    }

    public void Copy_PossibleCurrentCoursesCheckboxes_IntoModel(GridView StudentPossibleCoursesGridview, List<StudentEditCoursesPossible> possibleCourse_List)
    {
    }

    */

    public bool SQL_InsertStudentCourseInCurrentList(int studentUID, int programShiftReqID)
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_InsertStudentCourseInCurrentList";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@StudentUID", SqlDbType.Int).Value = studentUID;
            command.Parameters.Add("@ProgramShiftReqID", SqlDbType.Int).Value = programShiftReqID;
      
            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            this.errorCode = Convert.ToInt32(reader["errorCode"]);
            this.errorMessage = reader["errorMessage"] as string;
                     

            if (errorCode != 0) return false;
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage += "Request failed: StudentEditCoursesCore.cs/SQL_InsertStudentCourseInCurrentList/Catch --> " + ex.Message;
            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }
        return true;
    }

    public bool SQL_DelStudentCourseFromCurrentList(int studentUID, int studentCourseID)
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();
        /*  Note:   This action requires that the course is not linked to a schedule grid.
         *          The called sproc will verify that is true and return error if not.
         */

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_DelStudentCourseFromCurrentList";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@StudentUID", SqlDbType.Int).Value = studentUID;
            command.Parameters.Add("@StudentCourseID", SqlDbType.Int).Value = studentCourseID;

            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            errorCode = Convert.ToInt32(reader["errorCode"]);
            errorMessage = reader["errorMessage"] as string;

            if (errorCode != 0) return false;
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage += "Request failed: StudentEditCoursesCore.cs/SQL_DelStudentCourseFromCurrentList/Catch --> " + ex.Message;
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