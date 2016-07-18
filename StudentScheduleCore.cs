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

public class StudentScheduleCore 
{  
    #region DATA SECTION
    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    #region PRIVATE
    // Values based from ProgramID (see setScheduleCoreValuesBasedOn).
    private int programArea_ProgramID=0;
    private int selected_TermCalendarID;

    private string programAreaName;
    private int roomGroupID;
    private string roomGroupName;
    //
    // Lists and arrays.
    private List<Doctor> doctorScheduled_List = new List<Doctor>(); // Doctors that ARE scheduled. Display in Spotlight dropdown.
    private List<ShiftType> shiftType_List = new List<ShiftType>();
    private List<EnrollmentType> enrollmentType_List = new List<EnrollmentType>();
    private List<ShiftSlotType> shiftSlotType_List = new List<ShiftSlotType>();
    //
    private List<Room> room_List = new List<Room>();  // Entry for each room in roomgroup, match columns in shiftGrid
    private List<List<StudentShift>> shiftGrid = new List<List<StudentShift>>(); // 2D array of room detail rooms (col) * Day/hour (row)
    private DataTable shiftGridAsTable = new DataTable();   // Used as gridview.datasource friendly version of shiftGrid.
    //
    // StudentBasic Specific
    private List<StudentBasic> studentEligible_List = new List<StudentBasic>();

    #endregion

    #region PUBLIC
    public int ProgramArea_ProgramID { get { return programArea_ProgramID; } }
    public int Selected_TermCalendarID { get { return selected_TermCalendarID; } }
    public string ProgramAreaName { get { return programAreaName; } }
    public int RoomGroupID { get { return roomGroupID; } }
    public string RoomGroupName { get { return roomGroupName; } }
    //
    public List<Doctor> DoctorScheduled_List { get { return this.doctorScheduled_List; } }
    public List<ShiftType> ShiftType_List { get { return this.shiftType_List; } }
    public List<EnrollmentType> EnrollmentType_List { get { return this.enrollmentType_List; } }
    public List<ShiftSlotType> ShiftSlotType_List { get { return this.shiftSlotType_List; } }
    //
    public List<Room> Room_List { get { return this.room_List; } }
    public List<List<StudentShift>> ShiftGrid { get { return this.shiftGrid; } }
    public DataTable ShiftGridAsTable {get{return this.shiftGridAsTable;} }
    //
    public List<StudentBasic> StudentEligible_List { get { return this.studentEligible_List; } }
    
    #endregion
    #endregion
    
    #region Constructors
    public StudentScheduleCore()
    {
      
    }

    public StudentScheduleCore(int programArea_RoomGroupID, int selected_TermCalendarID, int selected_CampusID, bool UseAllCampus)
    {
        SQL_GetProgramArea(programArea_RoomGroupID); // Sets values for programArea_* variables.

        this.selected_TermCalendarID = selected_TermCalendarID;

        // The schedule core contains one particular program for one particular term.


        // Get data for various dropdowns.
        Doctor ds = new Doctor();
        if (!ds.SQL_GetDoctorsCurrentlyInScheduleInto(this.doctorScheduled_List, programArea_RoomGroupID, selected_TermCalendarID, selected_CampusID))
        {
            errorMessage = "Error: SSCore Load=-5 >" + ds.errorMessage;
            errorCode = -1;
        };

        ShiftType st = new ShiftType();
        st.SQL_GetShiftTypeListInto(this.shiftType_List, programArea_ProgramID, selected_CampusID);

        EnrollmentType et = new EnrollmentType();
        et.SQL_GetEnrollmentTypeListInto(this.enrollmentType_List);

        ShiftSlotType sst = new ShiftSlotType();
        if (!sst.SQL_GetShiftSlotTypeListInto(this.shiftSlotType_List, programArea_ProgramID))
        {
            errorMessage = "Error: SSCore Load=-1 >" + sst.errorMessage;
            errorCode = -1;
        }
            
        if (!SQL_GetScheduleRoomListInto(this.room_List, selected_TermCalendarID, this.roomGroupID, selected_CampusID ))
        {
            errorMessage = "Error: SSCore Load=-2 >" + errorMessage;
            errorCode = -1;
        }

        if (!SQL_GetScheduleDataTableInfoInto(this.ShiftGrid, selected_TermCalendarID, this.roomGroupID, selected_CampusID))
        {
            errorMessage = "Error: SSCore Load=-3 >" + errorMessage;
            errorCode = -1;
        }

        if (!SQL_GetStudentsEligibleForClinicInto(studentEligible_List, programArea_ProgramID, selected_CampusID, UseAllCampus))
        {
            errorMessage = "Error: SSCore Load=-4 >" + errorMessage;
            errorCode = -1;
        }

        if (errorCode==0)
            CopyShiftGridInto(this.shiftGridAsTable,  this.shiftGrid, this.room_List);
    }
    #endregion 
      
    #region SaveStudentScheduleShiftArray
    public void KILL_SaveStudentScheduleShiftArray(StudentShift shift)
    {
       
        StudentBasic student = new StudentBasic();



       /*
        if (shift.Student1_ProgramShiftReqID!= shift.Student1_ProgramShiftReqID_Old)
            if (shift.Student1_ProgramShiftReqID != 0)
            {
                student.SQL_AddStudentToSchedule(
                    shift.StudentID1,
                    shift.ScheduleID,
          //          (int)Globals.ShiftSlotTypeID_Primary,
                    shift.ShiftSlotTypeID1,
                    shift.Student1_ProgramShiftReqID, 0);
            }
            else
                student.SQL_DeleteStudentOnSchedule(shift.StudentID1_Old, shift.ScheduleID, (int)Globals.ShiftSlotTypeID_Primary);

        if (shift.Student2_ProgramShiftReqID != shift.Student2_ProgramShiftReqID_Old)
            if (shift.Student2_ProgramShiftReqID != 0)
                student.SQL_AddStudentToSchedule(shift.StudentID2, shift.ScheduleID,
                    //(int)Globals.ShiftSlotTypeID_Secondary, 
                    shift.ShiftSlotTypeID2, shift.Student2_ProgramShiftReqID, 0);
            else
                student.SQL_DeleteStudentOnSchedule(shift.StudentID2_Old, shift.ScheduleID, (int)Globals.ShiftSlotTypeID_Secondary);

        if (shift.Student3_ProgramShiftReqID != shift.Student3_ProgramShiftReqID_Old)
            if (shift.Student3_ProgramShiftReqID != 0)
                student.SQL_AddStudentToSchedule(shift.StudentID3, shift.ScheduleID, 
                   // (int)Globals.ShiftSlotTypeID_Observer, 
                    shift.ShiftSlotTypeID3, shift.Student3_ProgramShiftReqID, 0);
            else
                student.SQL_DeleteStudentOnSchedule(shift.StudentID3_Old, shift.ScheduleID, (int)Globals.ShiftSlotTypeID_Observer);
    
        */
    }
    #endregion

    #region SQL_GetRoomGroupID()
    private bool SQL_GetRoomGroupIDOLD(int programArea_ProgramID)
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_GetRoomGroupID";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@ProgramsID", SqlDbType.Int).Value = programArea_ProgramID;

            SqlDataReader reader = command.ExecuteReader();
            reader.Read();


            // Set the values in ScheduleCore
            roomGroupID = Convert.ToInt32(reader["RoomGroupID"]);
            roomGroupName = reader["RoomGroupName"] as string;
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed: DoctorScheduleCore.cs/SQL_GetRoomGroupID/Catch --> " + ex.Message;
            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }
        return true;
    }


    private bool SQL_GetProgramArea(int RoomGroupID)
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_GetRoomGroupDetail";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@RoomGroupID", SqlDbType.Int).Value = RoomGroupID;

            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            // Set the values in ScheduleCore
            roomGroupID = Convert.ToInt32(reader["RoomGroupID"]);
            roomGroupName = reader["RoomGroupName"] as string;
            programArea_ProgramID = Convert.ToInt32(reader["ProgramsID"]);
            programAreaName = reader["ProgramsName"] as string;
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed: DoctorScheduleCore.cs/SQL_GetRoomGroupID/Catch --> " + ex.Message;
            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }
        return true;
    }
    
    
    
    #endregion

    #region SQL_GetScheduleDataTableInfoInto()
    private bool SQL_GetScheduleDataTableInfoInto(List<List<StudentShift>> shiftGrid, int selected_TermCalendarID, int roomGroupID, int selected_CampusID)
    {
        // This SP will return shift records order on (DayTypeID, ShiftTypeID, RoomID)
        // They are parsed into a 2 dimensional array List<List<>>
        // List<Shift> contains rooms corresponding to a single Day/Shift row.
        // A new row in List<List<>> is created as Day/Shift changes.
        
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;


            // TODO: See note in StudentSchedule.aspx.cs/GetEnrollmentTypeCode..GetShiftSlotTypeCode about adding retrieveal of abbreviation. 

            // Build and execute command string
            command.CommandText = "CSSP_GetScheduleGrid";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@TermCalendarID", SqlDbType.Int).Value = selected_TermCalendarID;
            command.Parameters.Add("@RoomGroupID", SqlDbType.Int).Value = roomGroupID;
            command.Parameters.Add("@campusid", SqlDbType.Int).Value = selected_CampusID;
            command.Parameters.Add("@PurgeEmptyRow", SqlDbType.Bit).Value = 1;
            SqlDataReader reader = command.ExecuteReader();

            int current_dayTypeID = 0;
            int current_shiftTimeID = 0;
            int new_dayTypeID = 0;
            int new_shiftTimeID = 0;
            int rowCounter = 0;
            int colCounter = 0;

            List<StudentShift> rowOfShiftCells = null;

            while (reader.Read())
            {
                new_dayTypeID = Convert.ToInt32(reader["DayTypeID"]);
                new_shiftTimeID = Convert.ToInt32(reader["ShiftTimeID"]);

                if (!((current_dayTypeID == new_dayTypeID) & (current_shiftTimeID == new_shiftTimeID)))
                {
                    rowOfShiftCells = new List<StudentShift>();
                    shiftGrid.Add(rowOfShiftCells);
 
                    colCounter = 0;
                    rowCounter++;
                }

                #region AddRowShift 
                // Add room detail as first item in new row list.
                rowOfShiftCells.Add(new
                    StudentShift(rowCounter, colCounter,
                    Convert.ToInt32(reader["ScheduleID"]),
                    Convert.ToInt32(reader["TermCalendarID"]),
                    Convert.ToInt32(reader["DayTypeID"]),
                    Convert.ToInt32(reader["ShiftTimeID"]),
                    Convert.ToInt32(reader["RoomID"]),

                    Convert.ToInt32(reader["SectionID"]),
                    reader["SROffer_Section"] as string,

                    Convert.ToInt32(reader["SROfferID1"]),
                    Convert.ToInt32(reader["SROfferID2"]),
                    Convert.ToInt32(reader["SROfferID3"]),
                    Convert.ToInt32(reader["SROfferID4"]),

                    Convert.ToInt32(reader["ShiftTypeID"]),
                    Convert.ToInt32(reader["DoctorStatusID"]),

                    Convert.ToInt32(reader["s2SID_1"]),
                    Convert.ToInt32(reader["s2SID_2"]),
                    Convert.ToInt32(reader["s2SID_3"]),
                    Convert.ToInt32(reader["s2SID_4"]),

                    Convert.ToInt32(reader["EnrollmentTypeID_1"]),
                    Convert.ToInt32(reader["EnrollmentTypeID_2"]),
                    Convert.ToInt32(reader["EnrollmentTypeID_3"]),
                    Convert.ToInt32(reader["EnrollmentTypeID_4"]),

                    Convert.ToInt32(reader["StudentUID1"]),
                    Convert.ToInt32(reader["StudentUID2"]),
                    Convert.ToInt32(reader["StudentUID3"]),
                    Convert.ToInt32(reader["StudentUID4"]),

                    Convert.ToInt32(reader["programShiftReqID_1"]),
                    Convert.ToInt32(reader["programShiftReqID_2"]),
                    Convert.ToInt32(reader["programShiftReqID_3"]),
                    Convert.ToInt32(reader["programShiftReqID_4"]),

                    Convert.ToInt32(reader["shiftSlotRowID_1"]),
                    Convert.ToInt32(reader["shiftSlotRowID_2"]),
                    Convert.ToInt32(reader["shiftSlotRowID_3"]),
                    Convert.ToInt32(reader["shiftSlotRowID_4"]),

                    Convert.ToInt32(reader["shiftSlotTypeID_1"]),
                    Convert.ToInt32(reader["shiftSlotTypeID_2"]),
                    Convert.ToInt32(reader["shiftSlotTypeID_3"]),
                    Convert.ToInt32(reader["shiftSlotTypeID_4"]),

                    Convert.ToBoolean(reader["TimeCardRequired_1"]),
                    Convert.ToBoolean(reader["TimeCardRequired_2"]),
                    Convert.ToBoolean(reader["TimeCardRequired_3"]),
                    Convert.ToBoolean(reader["TimeCardRequired_4"]),

                    Convert.ToBoolean(reader["SupervisorNeeded"]),

                    reader["shiftSlotTypeName_1"] as string,
                    reader["shiftSlotTypeName_2"] as string,
                    reader["shiftSlotTypeName_3"] as string,
                    reader["shiftSlotTypeName_4"] as string,

                    reader["RoomName"] as string,
                    reader["ShiftTypeName"] as string,

                    reader["DoctorFirstName"] as string,
                    reader["DoctorLastName"] as string,
                    reader["DoctorDegree"] as string,

                    Convert.ToInt32(reader["doctorTypeID"]),
                    reader["doctorTypeName"] as string,
                    reader["doctorTypeCode"] as string,

                    reader["StudentName1"] as string,
                    reader["StudentName2"] as string,
                    reader["StudentName3"] as string,
                    reader["StudentName4"] as string,
          
                    reader["StudentLName1"] as string,
                    reader["StudentLName2"] as string,
                    reader["StudentLName3"] as string,
                    reader["StudentLName4"] as string,

                    reader["DayName"] as string,
                    reader["DayShort"] as string,
                    reader["ShiftTimeName"] as string,
                    reader["ShiftStartEnd"] as string,

                    reader["coursename_1"] as string,
                    reader["coursename_2"] as string,
                    reader["coursename_3"] as string,
                    reader["coursename_4"] as string,
                    //
                    Convert.ToInt32(reader["RoomGroupID"]),
                    Convert.ToBoolean(reader["BlendedRoom"]),
                    Convert.ToInt32(reader["ProgramsID"]),
                    reader["ProgramsCode"] as string,
                    reader["ProgramName"] as string,
                    
                    Convert.ToInt32(reader["programsID_1"]),
                    Convert.ToInt32(reader["programsID_2"]),
                    Convert.ToInt32(reader["programsID_3"]),
                    Convert.ToInt32(reader["programsID_4"]),

                    reader["ProgramsCode_1"] as string,
                    reader["ProgramsCode_2"] as string,
                    reader["ProgramsCode_3"] as string,
                    reader["ProgramsCode_4"] as string,

                    reader["ProgramsName_1"] as string,
                    reader["ProgramsName_2"] as string,
                    reader["ProgramsName_3"] as string,
                    reader["ProgramsName_4"] as string

                   ) );
#endregion  

                colCounter++;
                current_dayTypeID = new_dayTypeID;
                current_shiftTimeID = new_shiftTimeID;
            }
        }

        catch (Exception ex)
        {
            errorCode = -1;

            errorMessage += "Request failed: StudentScheduleCore.cs/SQL_GetScheduleDataTableInfoInto/Catch --> " + ex.Message;
            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }
        return true;
    }
    #endregion

    #region SQL_GetScheduleRoomListInto()
    private bool SQL_GetScheduleRoomListInto(List<Room> roomList, int termCalendarID, int roomGroupID, int selected_CampusID)
    {
        int colCounter = 0;

        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_GetScheduleRooms";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@TermCalendarID", SqlDbType.Int).Value = termCalendarID;
            command.Parameters.Add("@RoomGroupID", SqlDbType.Int).Value = roomGroupID;
            command.Parameters.Add("@CampusID", SqlDbType.Int).Value = selected_CampusID;

            SqlDataReader reader = command.ExecuteReader();
           
            while (reader.Read())
            {
                roomList.Add(new Room(
                    colCounter, 
                    Convert.ToInt32(reader["RoomID"]),
                    reader["RoomName"] as string,
                    reader["RoomColumn"] as string));

                colCounter++;
          //      errorMessage+=" "+ reader["RoomID"];
            }
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed: DoctorScheduleCore.cs/SQL_GetScheduleRooms/Catch --> " + ex.Message;
            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();

            command.Dispose();
        }
        return true;
    }
    #endregion

    #region CopyShiftGridInto
    private void CopyShiftGridInto(DataTable shiftGridAsTable, List<List<StudentShift>> shiftGrid, List<Room> room_List)
    {
        const string SHIFTTIME = "ShiftTime";

        // Create table with standard and dynamic columns. Result can be visulized
        // as a grid with defined top header line and defined left column header; where 
        // cells within the spreadsheet are empty at this time. Later use this
        // object as a datasource to a gridview with matching column names. 
        // The cells in the gridview can then be filled and colored appropriately from
        // StudentShift data in shiftgrid object.
         
        // Add standard row header column
        DataColumn dcol = new DataColumn(SHIFTTIME, typeof(System.String));
        shiftGridAsTable.Columns.Add(dcol);

        // Add a variable number of columns, one for each room in schedule.
        foreach (Room room in room_List)
        {
            DataColumn dcolm = new DataColumn(room.RoomID.ToString(), typeof(System.String));
            shiftGridAsTable.Columns.Add(dcolm);
        }

        // Add a row for each day/shift combo in schedule and include row name.
        foreach (List<StudentShift> shifts in shiftGrid)  
        {
            DataRow drow = shiftGridAsTable.NewRow();
            drow[SHIFTTIME] = shifts[0].DayName + " (" + shifts[0].ShiftTimeName.Trim() + ") " + shifts[0].ShiftStartEnd;
            shiftGridAsTable.Rows.Add(drow);
        }
    }

    #endregion

    #region SQL_GetStudentsEligibleForClinicInto
    public bool SQL_GetStudentsEligibleForClinicInto(List<StudentBasic> student_List, int programID, int selected_CampusID, bool UseAllCampus)
    {
        // Populate StudentBasic object with basic info for specific student based on a StudentUID

        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Get list of programs that student currently active in.
            command.CommandText = "CSSP_GetStudentsEligibleForClinic";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@programsid", SqlDbType.Int).Value = programID;
            command.Parameters.Add("@campusid", SqlDbType.Int).Value = selected_CampusID;
            command.Parameters.Add("@UseAllCampus", SqlDbType.Bit).Value = UseAllCampus;

            SqlDataReader reader = command.ExecuteReader();
    
            while (reader.Read())    // Build a new program object for each programID 
            {
                student_List.Add(new StudentBasic(Convert.ToInt32(reader["studentUID"]), reader["lastname"] as string, reader["firstname"] as string, reader["fullname"] as string));
            }
            reader.Close();
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed: StudentSchedule.cs/SQL_GetStudentsEligibleForClinic/Catch --> " + ex.Message;
            return false;
        }

        finally
        {
        //    errorMessage = "CSSP_GetStudentsEligibleForClinic(" + termCalendarID.ToString() + "," + programID.ToString() + ")";

            if (connection.State != ConnectionState.Closed) connection.Close();

            command.Dispose();
        }

        return true;
    }
    #endregion
}
