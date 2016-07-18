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
/// DoctorScheduleCore.cs handles details for building 
/// </summary>
public class DoctorScheduleCore
{  
    #region DATA SECTION

    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    public int rowCount = 0;
    public int rowLast = 0;
    public string TestQuery;

    #region PRIVATE
    // Values based from ProgramID (see SQL_GetProgramArea).
    private int programArea_ProgramID;

    private string programAreaName;
    private int roomGroupID;
    private string roomGroupName;
    //
    private int selected_TermCalendarID;
    //
    // Lists and arrays.
    private List<Doctor> doctorAvailable_List = new List<Doctor>(); // Doctors that CAN BE scheduled.
    private List<Doctor> doctorScheduled_List = new List<Doctor>(); // Doctors that ARE scheduled.
    private List<ShiftType> shiftType_List = new List<ShiftType>();
    //
    private List<Room> room_List = new List<Room>();  // Entry for each room in roomgroup, match columns in shiftGrid
    private List<List<DoctorShift>> shiftGrid = new List<List<DoctorShift>>(); // 2D array of room detail rooms (col) * Day/hour (row)
    private DataTable shiftGridAsTable = new DataTable();
    #endregion

    #region PUBLIC
    public int ProgramArea_ProgramID { get { return programArea_ProgramID; } }
    public int Selected_TermCalendarID { get { return selected_TermCalendarID; } }
    public string ProgramAreaName { get { return programAreaName; } }
    public int RoomGroupID { get { return roomGroupID; } }
    public string RoomGroupName { get { return roomGroupName; } }
    //
    public List<Doctor> DoctorAvailable_List { get { return this.doctorAvailable_List; } }
    public List<Doctor> DoctorScheduled_List { get { return this.doctorScheduled_List; } }
    public List<ShiftType> ShiftType_List { get { return this.shiftType_List; } }
    //
    public List<Room> Room_List { get { return this.room_List; } }
    public List<List<DoctorShift>> ShiftGrid { get { return this.shiftGrid; } }
    public DataTable ShiftGridAsTable {get{return this.shiftGridAsTable;} }

    #endregion
    #endregion

#region METHOD SECTION
    #region Constructors
    public DoctorScheduleCore()  { }

    public DoctorScheduleCore(int programArea_RoomGroupID, int selected_TermCalendarID, int selected_CampusID)
    {
        SQL_GetProgramArea(programArea_RoomGroupID); // Sets values for programArea_* variables.
        this.selected_TermCalendarID = selected_TermCalendarID;

// SetScheduleCoreValuesBasedOn(programArea_ProgramID, selected_TermCalendarID);

        // Make calls to database.
        Doctor ds = new Doctor();
        ds.SQL_GetDoctorsAvailableToScheduleInto(this.doctorAvailable_List, programArea_RoomGroupID, programArea_ProgramID ,selected_TermCalendarID, selected_CampusID);
        ds.SQL_GetDoctorsCurrentlyInScheduleInto(this.doctorScheduled_List, programArea_RoomGroupID, selected_TermCalendarID, selected_CampusID);

        ShiftType st = new ShiftType();
        st.SQL_GetShiftTypeListInto(this.shiftType_List, programArea_ProgramID, selected_CampusID);

// errorMessage = st.errorMessage;

        SQL_GetScheduleRoomListInto(this.room_List, selected_TermCalendarID, this.roomGroupID, selected_CampusID);
        SQL_GetScheduleGridArrayInto(this.ShiftGrid, selected_TermCalendarID, this.roomGroupID, selected_CampusID);
        CopyShiftGridListIntoDataTable(this.shiftGridAsTable,  this.shiftGrid, this.room_List);

 // errorMessage = aa;
    }
    #endregion 

    public void SaveShiftDetail(DoctorShift shift)
    {
        if (shift.ShiftTypeID>1)
            shift.SQL_UpdateCellShiftType(shift);

        // Handle Doctor dropdown
        if (shift.DoctorStatusID>1)
            if (shift.DoctorStatusID != shift.DoctorStatusID_Old)
                UpdateDatabase_DoctorToScheduleLink(shift);
    }

    
    #region UpdateDatabase_DoctorToScheduleLink()
    public void UpdateDatabase_DoctorToScheduleLink(DoctorShift shift)
    {
        Doctor doctor = new Doctor();

        int doctorStatusID_New = shift.DoctorStatusID;
        int doctorStatusID_Old = shift.DoctorStatusID_Old;
        int scheduleID = shift.ScheduleID;

        /* Possible doctorID change states and Del/Add action.
         
          ID_new  ID_Old   Del Add
           3        3       0   0      do nothing
          -1       -1       0   0      do nothing
          
          -1        3       x          call sp
           3       -1           x      call sp
           3        2       x   x      call sp's
         * 
         * FYI: For Doctor dropdown where ..ID = -1, it is just a placeholder value for None in UI. 
         * In the DB none 'shows' as no link record to tblschedule in tblSchedule2DoctorStatus.
         * This is different from ShiftTypeID in tblSchedule where a -1 is used. 
         */

        // If doctor dropdown changed, update something.
        //   if (doctorID_New != doctorID_Old)
        // {
        // If doctor changed to none (-1), delete old link record.
        if (doctorStatusID_New == -1)
        {
            doctor.SQL_DeleteDoctorOnSchedule(doctorStatusID_Old, scheduleID);
            //                doctor.SQL_DeleteDoctorOnSchedule(doctor.DoctorStatusID, scheduleID);

        }
        else
        {
            if (doctorStatusID_Old != -1)  // Delete old link record
            {
                doctor.SQL_DeleteDoctorOnSchedule(doctorStatusID_Old, scheduleID);
                //                    doctor.SQL_DeleteDoctorOnSchedule(doctor.DoctorStatusID, scheduleID);
            }

            // Add a new linking record.
           doctor.SQL_AddDoctorToSchedule(shift.DoctorStatusID, scheduleID);
        }
        // }
    }
    #endregion

    #region SetScheduleCoreValuesBasedOn()
    /*
    private void SetScheduleCoreValuesBasedOn(int programArea_ProgramID, int selected_TermCalendarID)
    {
  //     this.programArea_ProgramID = programArea_ProgramID;
        this.selected_TermCalendarID = selected_TermCalendarID;

 //       SQL_GetRoomGroupID(programArea_ProgramID);  // Set this.roomGroupID, this.roomGroupName.

        switch (programArea_ProgramID)              // Set the pretty names.
        {
            case (int)Globals.ProgramID_AOM:
                this.programAreaName = "AOM"; break;
            case (int)Globals.ProgramID_ND:
                this.programAreaName = "ND"; break;
            case (int)Globals.ProgramID_CHM:
                this.programAreaName = "CHM"; break;
            case (int)Globals.ProgramID_NTM:
                this.programAreaName = "Nutrician"; break;
            case (int)Globals.ProgramID_MACP:
                this.programAreaName = "MACP"; break;

            default:
                this.programAreaName = "setScheduleCoreValuesBasedOn"; break;
        }
    }
     */ 
    #endregion

    #region SQL_GetRoomGroupID()
    public bool SQL_GetRoomGroupIDOLD(int programArea_ProgramID)
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

            // rg.RoomGroupID, rg.RoomGroupName, rg.ProgramsID, p.ProgramsCode, p.Description ProgramsName
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

    #region SQL_GetScheduleGridArrayInto()
    public bool SQL_GetScheduleGridArrayInto(List<List<DoctorShift>> shiftArray, int selected_TermCalendarID, int roomGroupID, int selected_CampusID )
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_GetScheduleGrid";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@TermCalendarID", SqlDbType.Int).Value = selected_TermCalendarID;
            command.Parameters.Add("@RoomGroupID", SqlDbType.Int).Value = roomGroupID;
            command.Parameters.Add("@CampusID", SqlDbType.Int).Value = selected_CampusID;
            

   //        errorMessage = "CSSP_GetScheduleGrid (" + selected_TermCalendarID.ToString() + "," + roomGroupID.ToString()+") ";
            SqlDataReader reader = command.ExecuteReader();
            // This SP will return shift records order on (DayTypeID, ShiftTypeID, RoomID)
            // They are parsed into a 2 dimensional array List<List<>>
            // List<Shift> contains rooms corresponding to a single Day/Shift row.
            // A new row in List<List<>> is created as Day/Shift changes.

            int current_dayTypeID = 0;
            int current_shiftTimeID = 0;

            int new_dayTypeID = 0;
            int new_shiftTimeID = 0;
            int rowCounter = 0;
            int colCounter = 0;

            List<DoctorShift> shiftList = null;

         //   reader.Read();

            // Comparison basis for finding all rooms in same day/shift slot or row of shiftArray<>.
//            int current_dayTypeID = Convert.ToInt32(reader["DayTypeID"]);
  //          int current_shiftTimeID = Convert.ToInt32(reader["ShiftTimeID"]);

//            List<DoctorShift> shiftList = new List<DoctorShift>(); // Empty list of rooms related by day/shift.

       //     do
                while (reader.Read())
            {
                new_dayTypeID = Convert.ToInt32(reader["DayTypeID"]);
                new_shiftTimeID = Convert.ToInt32(reader["ShiftTimeID"]);

                if (!((current_dayTypeID == new_dayTypeID) & (current_shiftTimeID == new_shiftTimeID)))
                {
                    shiftList = new List<DoctorShift>();
                    // Add current shiftList row of rooms to array (vertical dimension 2D array)
                    shiftArray.Add(shiftList);

                    // Set up new row of rooms.

                    colCounter = 0;
                    rowCounter++;
                }

                // Add room detail as first item in new row list.
                shiftList.Add(new
                    DoctorShift(rowCounter, colCounter,
                    Convert.ToInt32(reader["ScheduleID"]),
                    Convert.ToInt32(reader["TermCalendarID"]),
                    Convert.ToInt32(reader["DayTypeID"]),
                    Convert.ToInt32(reader["ShiftTimeID"]),
                    Convert.ToInt32(reader["RoomID"]),
                    Convert.ToInt32(reader["ShiftTypeID"]),
                    Convert.ToInt32(reader["DoctorStatusID"]),
                    Convert.ToInt32(reader["StudentUID1"]),
                    Convert.ToInt32(reader["StudentUID2"]),
                    Convert.ToInt32(reader["StudentUID3"]),
                    Convert.ToInt32(reader["programShiftReqID_1"]),
                    Convert.ToInt32(reader["programShiftReqID_2"]),
                    Convert.ToInt32(reader["programShiftReqID_3"]),

                    Convert.ToInt32(reader["shiftSlotTypeID_1"]),
                    Convert.ToInt32(reader["shiftSlotTypeID_2"]),
                    Convert.ToInt32(reader["shiftSlotTypeID_3"]),

                    reader["shiftSlotTypeName_1"] as string,
                    reader["shiftSlotTypeName_2"] as string,
                    reader["shiftSlotTypeName_3"] as string,


                    reader["RoomName"] as string,
                    reader["ShiftTypeName"] as string,
                   Convert.ToBoolean(reader["SupervisorNeeded"]),

                    reader["DoctorFirstName"] as string,
                    reader["DoctorLastName"] as string,
                    reader["DoctorDegree"] as string,

                    reader["StudentName1"] as string,
                    reader["StudentName2"] as string,
                    reader["StudentName3"] as string,

                    reader["StudentLName1"] as string,
                    reader["StudentLName2"] as string,
                    reader["StudentLName3"] as string,

                    reader["DayName"] as string,
                    reader["DayShort"] as string,
                    reader["ShiftTimeName"] as string,
                    reader["ShiftStartEnd"] as string,
                    reader["coursename_1"] as string,
                    reader["coursename_2"] as string,
                    reader["coursename_3"] as string,

                    Convert.ToBoolean(reader["Conflict"]),
                    Convert.ToInt32(reader["ConflictProgramID"]),
                    reader["ConflictProgramName"] as string
                    )
                    );

                colCounter++;
                current_dayTypeID = new_dayTypeID;
                current_shiftTimeID = new_shiftTimeID;
            }

        }

        catch (Exception ex)
        {
            errorCode = -1;
           
            errorMessage += "Request failed: DoctorScheduleCore.cs/CSSP_GetScheduleGrid/Catch --> " + ex.Message;
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
    public bool SQL_GetScheduleRoomListInto(List<Room> roomList, int termCalendarID, int roomGroupID, int selected_CampusID )
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
            errorMessage = "CSSP_GetScheduleRooms ("
                + termCalendarID.ToString() + "," + roomGroupID.ToString() + ")";

            while (reader.Read())
            {
                roomList.Add(new Room(
                    colCounter, 
                    Convert.ToInt32(reader["RoomID"]),
                    reader["RoomName"] as string,
                    reader["RoomColumn"] as string));

                colCounter++;
                errorMessage+=" "+ reader["RoomID"];
            }
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage += "Request failed: DoctorScheduleCore.cs/SQL_GetScheduleRooms/Catch --> " + ex.Message;
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

    #region CopyGridArrayIntoDataTable
    public void CopyShiftGridListIntoDataTable(DataTable shiftGridAsTable, List<List<DoctorShift>> shiftGrid, List<Room> room_List)
    {
        const string SHIFTTIME = "ShiftTime";

        // Create table with standard and dynamic columns. Result can be visulized
        // as a grid with defined top header line and defined left column header; where 
        // cells within the spreadsheet are empty at this time. Later we feed this
        // object as a datasource to a column matched gridview, in effect the UI Schedule grid.
 
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
        foreach (List<DoctorShift> shifts in shiftGrid)  
        {
            DataRow drow = shiftGridAsTable.NewRow();
            drow[SHIFTTIME] = shifts[0].DayName + " (" + shifts[0].ShiftTimeName.Trim() +")  "+ shifts[0].ShiftStartEnd;
            shiftGridAsTable.Rows.Add(drow);
        }
    }

    #endregion
#endregion

}
