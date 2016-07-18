#define DebugOn

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
/// Summary description for ProgReqCore
/// </summary>
public class ProgReqCore
{
    #region DATA SECTION
    // Standard error properties
    public string ErrorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int ErrorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)
    public int Buffer=0;
    private List<ProgReqSQLBuffer> progReqSQLBuffer= new List<ProgReqSQLBuffer>() ; 
    public List<ProgReqSQLBuffer> ProgReqSQLBuffer { get {return this.progReqSQLBuffer;}}

    private List<ProgReqProgram> progReqProgram = new List<ProgReqProgram>();
    public List<ProgReqProgram> Program { get { return this.progReqProgram; } }

    #endregion

    public ProgReqCore(int programArea_ProgramID, int studentUID, int selected_TermCalendarID, int selected_CampusID)
	{

        if (SQL_GetStudentProgramReqData(progReqSQLBuffer, studentUID, programArea_ProgramID))
        {
            PopulateObjectTreeFromBuffer(progReqSQLBuffer, Program);
        }        
    }

    public void PopulateObjectTreeFromBuffer(List<ProgReqSQLBuffer> PBuffer, List<ProgReqProgram> ProgReqProgram)
    {
        int programNdx, sectionNdx, rowNdx;    // List object subscripts/indexes to track thru the various list arrays.  
        programNdx = sectionNdx = rowNdx = -1; // Use -1 so we start at 0 when actually used.  
        
        int b1000, b100, b10;
        b1000 = b100 = b10 =-100;    // Use -100 a value that should never exist in the SQLBuffer result set. 

        foreach (ProgReqSQLBuffer pb in PBuffer)
        {
            // Program Break Condition
            if (pb.B1000 !=b1000) // Programs in which a student is majoring.   e.g. AOM and ND
            {
                ProgReqProgram.Add(new ProgReqProgram
                    (pb.BufferRow, pb.B1000, pb.ProgramReqColumnSetID, pb.ProgramReqNote, 
                    pb.CalculateColumnTotal, pb.ProgramsID, pb.ProgramDescription, pb.ProgramsCode,
                    pb.ExpectedGradTerm));

                programNdx += 1;       // Generate offset to track which program we're building.
                sectionNdx = -1;       // Setup to start a new section below.

                b1000 = pb.B1000;   // Remember what program break we're on.
                b100 = -100;        // Force next break condition to start a new screen section.

                // Snap in total
                // Add a single section with a single row. Below populate each numeric item with totalRequire value.
                progReqProgram[programNdx].TotalRequired.RowHeader= "Total Required";
                progReqProgram[programNdx].TotalRequired.RowNote= "Program Total";

            } 

            // Section Break Condition
            if (pb.B100 !=b100)     // Screen Section e.g Required/primary, required/secondary
            {
                progReqProgram[programNdx].Section.Add(new ProgReqSection(programNdx, pb.BufferRow, pb.B100, 
                    pb.CourseTypeID, pb.CourseTypeName, pb.ClassRoleTypeID, pb.ClassRoleTypeName, 
                    pb.EnrollmentTypeID, pb.EnrollmentTypeName, pb.ShiftSlotTypeID, 
                    pb.ShiftSlotTypeName));

                sectionNdx += 1;       // Generate offset to this section in current program being built.
                rowNdx = -1;           // Setup to start first row or course in this section.

                b100 = pb.B100;     // Remember what section break we're on. 
                b10 = -100;         // Force next break condition so we can start a new row.

                // Snap in total
     //           progReqProgram[programNdx].Section[sectionNdx].TotalInSection.RowHeader = "Section Total Req";
   //             progReqProgram[programNdx].Section[sectionNdx].TotalInSection.RowNote = "Section Total Req";
            
            }

            // Row Break Condition
            if (pb.B10 != b10)      // Course's row.    e.g. AOM Clinic, AOM Preceptor..
            {
                progReqProgram[programNdx].Section[sectionNdx].Row.Add(new ProgReqRow
                    (pb.BufferRow, pb.B10, pb.StudentCourseNote, 
                    pb.DoctorLastName, pb.DoctorFirstName, pb.CourseTerm, pb.StudentCourseID,
                    pb.ProgramShiftReqID, pb.CourseName,
                    pb.Department, pb.ShiftHours
                    ));
                rowNdx += 1;           // Generate offset for this row.

                b10 = pb.B10;       // Remember what row we're on
            }

            // Add the Cell Data item to current row.
    

            if (!pb.CalculateRowItem)
                progReqProgram[programNdx].Section[sectionNdx].Row[rowNdx].Item.Add(new
                    ProgReqItem(pb.BufferRow, pb.B1, pb.OrderAcrossGridview, pb.ProgramReqItemName,
                    pb.ProgramReqItemTypeID, pb.DataTypeSQL, pb.DataTypeCS, pb.IsNumber,
                    pb.ProgramReqItemUIWidth, pb.ProgramReqItemUIName, pb.CourseData_Decimal,
                    pb.CourseData_NoteText, pb.CourseData_NoteID, pb.StudentCourseDataID,
                    pb.ProgramReqColumnSetID, 0, true, pb.CalculateRowItem));

           else
                {

                // Left Item
                progReqProgram[programNdx].Section[sectionNdx].Row[rowNdx].Item.Add(new
                    ProgReqItem(pb.BufferRow, 
                    pb.B1, pb.OrderAcrossGridview, 
                    
                    pb.ProgramReqItemName_Left,
                    pb.ProgramReqItemTypeID, pb.DataTypeSQL, pb.DataTypeCS, pb.IsNumber,
                    pb.ProgramReqItemUIWidth_Left, pb.ProgramReqItemUIName_Left, pb.ShiftHours,
                    pb.CourseData_NoteText, pb.CourseData_NoteID, pb.StudentCourseDataID,
                    pb.ProgramReqColumnSetID, 0, false, pb.CalculateRowItem));
 
                // Center Item
                progReqProgram[programNdx].Section[sectionNdx].Row[rowNdx].Item.Add(new
                    ProgReqItem(pb.BufferRow, pb.B1, 
                    pb.OrderAcrossGridview, 
                    pb.ProgramReqItemName,
                    pb.ProgramReqItemTypeID, pb.DataTypeSQL, pb.DataTypeCS, pb.IsNumber,
                    pb.ProgramReqItemUIWidth, pb.ProgramReqItemUIName, pb.CourseData_Decimal,
                    pb.CourseData_NoteText, pb.CourseData_NoteID, pb.StudentCourseDataID,
                    pb.ProgramReqColumnSetID, 0, true, pb.CalculateRowItem));
             
                // Right Item
                progReqProgram[programNdx].Section[sectionNdx].Row[rowNdx].Item.Add(new
                    ProgReqItem(pb.BufferRow,
                    pb.B1, pb.OrderAcrossGridview,
                    pb.ProgramReqItemName_Right,
                    pb.ProgramReqItemTypeID, pb.DataTypeSQL, pb.DataTypeCS, pb.IsNumber,
                    pb.ProgramReqItemUIWidth_Right, pb.ProgramReqItemUIName_Right, pb.CourseData_Decimal- pb.ShiftHours,
                    pb.CourseData_NoteText, pb.CourseData_NoteID, pb.StudentCourseDataID,
                    pb.ProgramReqColumnSetID, 0, false, pb.CalculateRowItem));
            }

            // Snap in total value adders
             if (rowNdx == 1) // Skim programReqTotal from first row of items, ignore remaining redundent rows.
             {
                 // We know totReq so put in value, does not change.
                 progReqProgram[programNdx].TotalRequired.Item.Add(new
                     ProgReqItem(pb.BufferRow, pb.B1, pb.OrderAcrossGridview, pb.ProgramReqItemName,
                     pb.ProgramReqItemTypeID, pb.DataTypeSQL, pb.DataTypeCS, pb.IsNumber,
                     pb.ProgramReqItemUIWidth, pb.ProgramReqItemUIName, pb.CourseData_Decimal,
                     pb.CourseData_NoteText, pb.CourseData_NoteID, pb.StudentCourseDataID,
                     pb.ProgramReqColumnSetID, pb.ProgramReqTotal, false, pb.CalculateRowItem));
                
                 // Rows used to hold CalculateSubToTals() values so put in 0 for now.
                 progReqProgram[programNdx].SubTotalDeficit.Item.Add(new
                     ProgReqItem(pb.BufferRow, pb.B1, pb.OrderAcrossGridview, pb.ProgramReqItemName,
                     pb.ProgramReqItemTypeID, pb.DataTypeSQL, pb.DataTypeCS, pb.IsNumber,
                     pb.ProgramReqItemUIWidth, pb.ProgramReqItemUIName, pb.CourseData_Decimal,
                     pb.CourseData_NoteText, pb.CourseData_NoteID, pb.StudentCourseDataID,
                     pb.ProgramReqColumnSetID, 0, false, pb.CalculateRowItem));

                 progReqProgram[programNdx].SubTotalTodate.Item.Add(new
                    ProgReqItem(pb.BufferRow, pb.B1, pb.OrderAcrossGridview, pb.ProgramReqItemName,
                    pb.ProgramReqItemTypeID, pb.DataTypeSQL, pb.DataTypeCS, pb.IsNumber,
                    pb.ProgramReqItemUIWidth, pb.ProgramReqItemUIName, pb.CourseData_Decimal,
                    pb.CourseData_NoteText, pb.CourseData_NoteID, pb.StudentCourseDataID,
                    pb.ProgramReqColumnSetID, 0, false, pb.CalculateRowItem));

                 progReqProgram[programNdx].Section[sectionNdx].SubTotalInSection.Item.Add(new
                     ProgReqItem(pb.BufferRow, pb.B1, pb.OrderAcrossGridview, pb.ProgramReqItemName,
                     pb.ProgramReqItemTypeID, pb.DataTypeSQL, pb.DataTypeCS, pb.IsNumber,
                     pb.ProgramReqItemUIWidth, pb.ProgramReqItemUIName, pb.CourseData_Decimal,
                     pb.CourseData_NoteText, pb.CourseData_NoteID, pb.StudentCourseDataID,
                     pb.ProgramReqColumnSetID, 0, false, pb.CalculateRowItem));
             }
        }
    }

    public string CalculateSubTotals()
    {
        int itemIndex=0;
        string cnt = "";
        try
        {
            // Zero out the Sub Total accumulators
            foreach (ProgReqProgram prp in this.Program)
                foreach (ProgReqSection prs in prp.Section)
                    foreach (ProgReqRow row in prs.Row)
                        for (itemIndex = 0; itemIndex < row.Item.Count; itemIndex++)
                            prs.SubTotalInSection.Item[itemIndex].CourseData_Decimal =
                            prp.SubTotalTodate.Item[itemIndex].CourseData_Decimal = 0;
        }
        catch
        {
            cnt = "FirstBrk";
        }

        // Populate the accumulators
        itemIndex = 0;
        try
        {
            foreach (ProgReqProgram prp in this.Program)
                foreach (ProgReqSection prs in prp.Section)
                    foreach (ProgReqRow row in prs.Row)
                        for (itemIndex = 0; itemIndex < row.Item.Count; itemIndex++)
                        {
                            prs.SubTotalInSection.Item[itemIndex].CourseData_Decimal +=
                                row.Item[itemIndex].CourseData_Decimal;

                            prp.SubTotalTodate.Item[itemIndex].CourseData_Decimal +=
                                row.Item[itemIndex].CourseData_Decimal;
                        }
            cnt+="secondwork";
        }
        catch
        {
            cnt += "SecondBrk";
        }

        return cnt;
    }


    private bool SQL_GetStudentProgramReqData(
        List<ProgReqSQLBuffer> progReqSQLBuffer, 
        int studentUID, 
        int programsID)
    {
        int bufferRow = 0;
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_GetStudentProgramReqData";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@StudentUID", SqlDbType.Int).Value = studentUID;
            command.Parameters.Add("@ProgramsID", SqlDbType.Int).Value = programsID;
            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            ErrorCode = Convert.ToInt32(reader["errorCode"]);
            ErrorMessage = reader["errorMessage"] as string;

            if (ErrorCode != 0) return false;

            do
            {
                progReqSQLBuffer.Add(new 
                    ProgReqSQLBuffer(
                    bufferRow,
                    Convert.ToInt32(reader["B1000"]),
                    Convert.ToInt32(reader["ProgramsID"]),
                    reader["ProgramDescription"] as string,
                    reader["ProgramsCode"] as string,
                    Convert.ToInt32(reader["B100"]),
                    Convert.ToInt32(reader["CourseTypeID"]),
                    reader["CourseTypeName"] as string,
                    Convert.ToInt32(reader["ClassRoleTypeID"]),
                    reader["ClassRoleTypeName"] as string,
                    Convert.ToInt32(reader["B10"]),
                    reader["CourseName"] as string,
                    Convert.ToInt32(reader["B1"]),
                    Convert.ToInt32(reader["OrderAcrossGridview"]),
                    reader["ProgramReqItemName"] as string,
                    Convert.ToInt32(reader["EnrollmentTypeID"]),
                    reader["EnrollmentTypeName"] as string,
                    Convert.ToInt32(reader["ShiftSlotTypeID"]),
                    reader["ShiftSlotTypeName"] as string,

                    reader["DoctorLastName"] as string,
                    reader["DoctorFirstName"] as string,
                    reader["CourseTerm"] as string,
                    Convert.ToDecimal(reader["CourseData_Decimal"]),
                    reader["CourseData_NoteText"] as string,
                    Convert.ToInt32(reader["CourseData_NoteID"]),
                    Convert.ToInt32(reader["StudentCourseDataID"]),

                    Convert.ToBoolean(reader["CalculateColumnTotal"]),
                    Convert.ToInt32(reader["StudentUID"]),
                    Convert.ToInt32(reader["StudentCourseID"]),
                    Convert.ToInt32(reader["ProgramReqColumnSetID"]),

                    Convert.ToDecimal(reader["ProgramReqTotal"]),

                    Convert.ToInt32(reader["ProgramShiftReqID"]),
                    Convert.ToInt32(reader["ProgramReqItemTypeID"]),

                    Convert.ToDecimal(reader["ShiftHours"]),
                    reader["Department"] as string,

                    reader["ProgramReqItemUIName"] as string,
                    Convert.ToInt32(reader["ProgramReqItemUIWidth"]),

                    reader["DataTypeSQL"] as string,
                    reader["DataTypeCS"] as string,
                    Convert.ToBoolean(reader["IsNumber"]),
                    reader["ExpectedGradTerm"] as string,

                    Convert.ToBoolean(reader["CalculateRowItem"]),

                    Convert.ToInt32(reader["ProgramReqItemID_Left"]),
                    Convert.ToInt32(reader["ProgramReqItemID_Right"]),
                    reader["ProgramReqItemName_Left"] as string,
                    reader["ProgramReqItemName_Right"] as string,
                    reader["Description_Left"] as string,
                    reader["Description_Right"] as string,
                    Convert.ToInt32(reader["ProgramReqRowOpTypeID"]),

                    reader["ProgramReqItemUIName_Left"] as string,
                    reader["ProgramReqItemUIName_Right"] as string,

                    Convert.ToInt32(reader["ProgramReqItemUIWidth_Left"]),
                    Convert.ToInt32(reader["ProgramReqItemUIWidth_Right"])
                    ));
                bufferRow++;
            }
            while (reader.Read());
            Buffer = bufferRow;

        }

        catch (Exception ex)
        {
            ErrorCode = -1;
            ErrorMessage += "Request failed: ProgReqCore.cs/CSSP_GetStudentProgramReqData/Catch --> " + ex.Message;
            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }
        return true;
    }

    public int SQL_GetStudentCourseDataToken(
        int studentUID, 
        string userID)
    {
        int token = 0;
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_GetStudentCourseDataToken";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@StudentUID", SqlDbType.Int).Value = studentUID;
            command.Parameters.Add("@UserID", SqlDbType.VarChar).Value = userID;
            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            ErrorCode = Convert.ToInt32(reader["errorCode"]);
            ErrorMessage = reader["errorMessage"] as string;

            if (ErrorCode == 0) 
                token = Convert.ToInt32(reader["StudentCourseDataTokenID"]);
        }

        catch (Exception ex)
        {
            ErrorCode = -1;
            ErrorMessage += "Request failed: ProgReqCore.cs/CSSP_GetStudentCourseDataToken/Catch --> " + ex.Message;
            return token;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }
        return token;
    }
    
    public int SQL_InsertStudentCourseDataBuffer(
        int studentCourseDataTokenID,
	    int studentCourseDataID,
	    int studentUID,
	    int studentCourseID,
	    int programReqColumnSetID,
	    decimal newCourseData_Decimal,
	    string newCourseData_NoteText,
	    decimal oldCourseData_Decimal,
	    string oldCourseData_NoteText,
	    int oldCourseData_NoteID,
	    int termCalendarID)
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_InsertStudentCourseDataBuffer";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@studentCourseDataTokenID", SqlDbType.Int).Value = studentCourseDataTokenID;
            command.Parameters.Add("@studentCourseDataID", SqlDbType.Int).Value = studentCourseDataID;
            command.Parameters.Add("@studentUID", SqlDbType.Int).Value = studentUID;
            command.Parameters.Add("@studentCourseID", SqlDbType.Int).Value = studentCourseID;
            command.Parameters.Add("@programReqColumnSetID", SqlDbType.Int).Value = programReqColumnSetID;
            command.Parameters.Add("@newCourseData_Decimal", SqlDbType.Int).Value = newCourseData_Decimal;
            command.Parameters.Add("@newCourseData_NoteText", SqlDbType.VarChar).Value = newCourseData_NoteText;
            command.Parameters.Add("@oldCourseData_Decimal", SqlDbType.Decimal).Value = oldCourseData_Decimal;
            command.Parameters.Add("@oldCourseData_NoteText", SqlDbType.VarChar).Value = oldCourseData_NoteText;
            command.Parameters.Add("@oldCourseData_NoteID", SqlDbType.Int).Value = oldCourseData_NoteID;
            command.Parameters.Add("@termCalendarID", SqlDbType.Int).Value = termCalendarID;
            
            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            ErrorCode = Convert.ToInt32(reader["errorCode"]);
            ErrorMessage = reader["errorMessage"] as string;
        }

        catch (Exception ex)
        {
            ErrorCode = -1;
            ErrorMessage += "Request failed: ProgReqCore.cs/CSSP_InsertStudentCourseDataBuffer/Catch --> " + ex.Message;
            return 1 ;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }
        return 0;
    }

    public bool SQL_UpdateStudentCourseData(
        int studentUID, 
        int studentCourseDataTokenID)
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_UpdateStudentCourseData";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@StudentUID", SqlDbType.Int).Value = studentUID;
            command.Parameters.Add("@StudentCourseDataTokenID", SqlDbType.VarChar).Value = studentCourseDataTokenID;
            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            ErrorCode = Convert.ToInt32(reader["errorCode"]);
            ErrorMessage = reader["errorMessage"] as string;
        }

        catch (Exception ex)
        {
            ErrorCode = -1;
            ErrorMessage += "Request failed: ProgReqCore.cs/CSSP_UpdateStudentCourseData/Catch --> " + ex.Message;
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