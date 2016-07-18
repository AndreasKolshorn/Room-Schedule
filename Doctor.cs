using System;
using System.Collections.Generic;
using System.Web;

using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Doctor
/// </summary>
public class Doctor
{
    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    // Basic properties - These typically live in tblDoctor which goes 1 to many in tblDoctorStatus. 
    //                    Note some attributes are more dynamic and tblDoctorStatus a snapshot history.  
    private int doctorStatusID;        // This represents the active instance (credentials) of the doctor at particular time and links to particular schedule.
    private int doctorID;              // The not time dependant part of a doctor instance. e.g. name but not current credentials.
    private string lastname = "Unknown";
    private string firstname= "Unknown";
    private string middlename = "Unknown";
    private string fullName = "Unknown";
    private string basicNote = "Unknown";
    private bool doctorActive = false;
    private string doctorName = "Unknown";

    // This and DoctorShiftItem.cs are start of getting some details, namely which shifts 
    // the doctor is currently connected to. Pushing out to v2.0.
    private List<DoctorShiftItem> doctorShift_List = new List<DoctorShiftItem>();
    public List<DoctorShiftItem> DoctorShift_List { get { return doctorShift_List; } }
      
    // Date Dependent Attributes  - Programatically they become effective at the beginning of a quarter
    //                              They are stored in or connect to a tblDoctorStatus record that
    //                              keys off DoctorId and statusAsOf_TermCalendarID
    private int statusAsOf_TermCalendarID=0;
    private string statusAsOf_TermCalendarText="";
    private bool credentialType_MAC=false, credentialType_PHD=false,
                 credentialType_MD = false, credentialType_ND = false,
                 credentialType_LAC = false, credentialType_RD = false;

    private int doctorCanProctorID = 0;
    private string doctorDegree = "Unknown";
    private int doctorTypeID= 0;
    private string doctorTypeName= "Unknown";
    //

    public int DoctorStatusID { 
        get { return doctorStatusID; }
        set { doctorStatusID = value; }
    }
    public int DoctorID { get { return doctorID; } }
    public string Firstname { get { return firstname; } }
    public string Lastname { get { return lastname; } }

    public string Middlename { get { return middlename; } }
    public string BasicNote { get { return basicNote; } }

    public bool DoctorActive { get { return doctorActive; } set { doctorActive = value; } }

    public string ListName { get { return prettyListName(firstname, lastname, middlename) ; } }

    
    public string DoctorName { get { return doctorName; } }

    public int StatusAsOf_TermCalendarID { get { return statusAsOf_TermCalendarID; } }
    public string StatusAsOf_TermCalendarText { get { return statusAsOf_TermCalendarText; } }

    public bool CredentialType_Mac { get { return credentialType_MAC; } set { credentialType_MAC = value; } }
    public bool CredentialType_PHD { get { return credentialType_PHD; } set { credentialType_PHD = value; } }
    public bool CredentialType_MD { get { return credentialType_MD; } set { credentialType_MD = value; } }
    public bool CredentialType_ND { get { return credentialType_ND; } set { credentialType_ND = value; } }

    public bool CredentialType_LAC { get { return credentialType_LAC; } set { credentialType_LAC = value; } }
    public bool CredentialType_RD { get { return credentialType_RD; } set { credentialType_RD = value; } }

    public string DoctorDegree { get { return doctorDegree; } }
    public int DoctorTypeID { get { return doctorTypeID; } }
  
    public string DoctorTypeName { get { return doctorTypeName; } }
    public int DoctorProgramID { get { return doctorCanProctorID; } }

    public Doctor() {}

    public Doctor(int doctorID, int termCalendarID)
    {
        SQL_GetDoctorBasics(doctorID);
        SQL_GetDoctorScheduledInRoomList(doctorID, termCalendarID);
    }

    public Doctor(int doctorID)
    {
        SQL_GetDoctorBasics(doctorID);
    }

    public Doctor(int doctorID, string firstName, string lastName, string doctorDegree)
    {
        this.doctorID = doctorID;
        this.firstname = firstName;
        this.lastname = lastName;
        this.doctorDegree = doctorDegree;
        this.fullName = lastname + ", " + firstname.Substring(0, 1) + "(" + doctorDegree.TrimEnd() + ")";
    }

    private string prettyListName(string firstname, string lastname, string middlename)
    {
        string prettyName="";
        if (lastname.Length > 15)
            prettyName = lastname.Substring(0, 15);
        else
            prettyName = lastname;

        prettyName += ",";

        if (firstname.Length > 15)
            prettyName += " " + firstname.Substring(0, 15);
        else
            prettyName += " "+firstname;

        if (middlename.Trim().Length>0)
            prettyName += " " + middlename.Trim().Substring(0, 1)+".";

        return prettyName;
    }


    public Doctor(int doctorID, int doctorStatusID, string firstName, string lastName, string  middleName, string doctorDegree)
    {
        this.doctorID = doctorID;
        this.doctorStatusID = doctorStatusID;
        this.firstname = firstName;
        this.lastname = lastName;
        this.middlename = middleName;
        this.doctorDegree = doctorDegree;
        this.fullName = lastname + ", " + firstname.Substring(0, 1) + "(" + doctorDegree.TrimEnd() + ")";
    }

    public int getDoctorProgramID(int scheduleAreaProgramID)
    {
        int doctorProgramID;

        // Map the schedule type to doctors available to teach in that area.
        switch (scheduleAreaProgramID)
        {
            case (int)Globals.ProgramID_MSAOM:
                doctorProgramID = (int)Globals.DoctorProgramID_AOM; break;
            case (int)Globals.ProgramID_MSA:
                doctorProgramID = (int)Globals.DoctorProgramID_MSA; break;
            case (int)Globals.ProgramID_ND:
                doctorProgramID = (int)Globals.DoctorProgramID_AOM_ND; break;
            case (int)Globals.ProgramID_MSN:
                doctorProgramID = (int)Globals.DoctorProgramID_MSN; break;
            default:
                doctorProgramID = 0; break;
        }
        return doctorProgramID;
    }

    private bool SQL_GetDoctorBasics(int DoctorID)
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
                command.CommandText = "CSSP_GetDoctorBasics";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = DoctorID;

                SqlDataReader reader = command.ExecuteReader();

                // Expect one record
                while (reader.Read())
                {
                    this.doctorStatusID = Convert.ToInt32(reader["doctorStatusID"]);
                    this.doctorID = Convert.ToInt32(reader["doctorID"]);
                    
                    this.lastname = reader["lastname"] as string;
                    this.firstname = reader["firstname"] as string;
                    this.middlename = reader["middlename"] as string;
                    this.doctorName = reader["displayName"] as string;
                    this.basicNote = reader["BasicNote"] as string;
                    this.doctorActive = Convert.ToBoolean(reader["Active"]);

                    this.statusAsOf_TermCalendarID = Convert.ToInt32(reader["statusAsOf_TermCalendarID"]);
                  // this.statusAsOf_TermCalendarText = reader["statusAsOf_TermCalendarText"] as string;

                    this.doctorDegree = reader["doctorDegree"] as string;
                    this.doctorTypeName = reader["doctorTypeName"] as string;
                    haveRecord = true;
                    this.doctorCanProctorID = Convert.ToInt32(reader["DoctorProgramID"]);
                    this.doctorTypeID = Convert.ToInt32(reader["DoctorTypeID"]);

                    this.credentialType_MAC = Convert.ToBoolean(reader["credentialType_MAC"]);
                    this.credentialType_PHD = Convert.ToBoolean(reader["credentialType_PHD"]);
                    this.credentialType_MD = Convert.ToBoolean(reader["credentialType_MD"]);
                    this.credentialType_ND = Convert.ToBoolean(reader["credentialType_ND"]);
                    this.credentialType_LAC = Convert.ToBoolean(reader["credentialType_LAC"]);
                    this.credentialType_RD = Convert.ToBoolean(reader["credentialType_RD"]);
                    }

                if (!haveRecord)        // Just in case..
                {
                    this.doctorStatusID = 0;
                    this.doctorID = 0;
                    
                    this.lastname = "LastName N/A";
                    this.firstname = "FirstName N/A";
                    this.doctorName = "Doctor Name N/A";

                    this.statusAsOf_TermCalendarID = 0;
                    this.statusAsOf_TermCalendarText = "LastName N/A";

                    this.doctorDegree = "DoctorDegree N/A";
                    this.doctorTypeName = "DoctorTypeName N/A";

                    this.doctorCanProctorID = 0;
                    this.doctorTypeID = 0;
                }
            }

            catch (Exception ex)
            {
                errorCode = -1;
                errorMessage = "Request failed:";
                errorMessage = "Request failed: Program.cs/SQL_GetDoctorBasics/Catch --> " + ex.Message;
                return false;
            }

            finally
            {

                errorMessage = "SQL_GetDoctorBasics(" + DoctorID.ToString() + ")";
                if (connection.State != ConnectionState.Closed) connection.Close();
                command.Dispose();
            }

            return true;
        }

 public bool SQL_GetDoctorScheduledInRoomList(int doctorID, int termCalendarID)
 {
     bool haveRecord = false;

     Int32 scheduleID;
     string dayName;
     string shiftName;
     string roomName;

     string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
     SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
     System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

     try
     {
         connection.Open();
         command.Connection = connection;

         // Build and execute command string
         command.CommandText = "CSSP_GetDoctorScheduleDetail";
         command.CommandType = CommandType.StoredProcedure;
         command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = doctorID;
         command.Parameters.Add("@TermCalendarID", SqlDbType.Int).Value = termCalendarID;

         SqlDataReader reader = command.ExecuteReader();

         while (reader.Read())
         {
             dayName = reader["DayName"] as string;
             shiftName = reader["ShiftName"] as string;
             roomName = reader["RoomName"] as string;
             scheduleID = Convert.ToInt32(reader["ScheduleID"]);

             doctorShift_List.Add(new DoctorShiftItem(scheduleID, dayName, shiftName, roomName));
             haveRecord = true;
         }

         if (!haveRecord)        // Just in case..
         {
             this.doctorStatusID = 0;
             this.doctorID = 0;

             this.lastname = "LastName N/A";
             this.firstname = "FirstName N/A";

             this.statusAsOf_TermCalendarID = 0;
             this.statusAsOf_TermCalendarText = "LastName N/A";

             this.doctorDegree = "DoctorDegree N/A";
             this.doctorTypeName = "DoctorTypeName N/A";
         }
     }

     catch (Exception ex)
     {
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

 public bool SQL_AddDoctorToSchedule(int DoctorID, int ScheduleID )
 {
     string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
     SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
     System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

     try
     {
         connection.Open();
         command.Connection = connection;

         // Build and execute command string
         command.CommandText = "CSSP_InsertDoctorInSchedule";
         command.CommandType = CommandType.StoredProcedure;
         command.Parameters.Add("@DoctorStatusID", SqlDbType.Int).Value = DoctorID;
         command.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = ScheduleID;

         SqlDataReader reader = command.ExecuteReader();

         reader.Read();
         }

     catch (Exception ex)
     {
         errorCode = -1;
         errorMessage = "Request failed:";
#if DebugOn
      errorMessage = "Request failed: Doctor.cs/SQL_AddDoctorToSchedule/Catch --> " + ex.Message;
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

 public string SQL_DeleteDoctorOnSchedule(int DoctorStatusID, int ScheduleID)
 {
     string val="fine";

     string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
     SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
     System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

     try
     {
         connection.Open();
         command.Connection = connection;

         // Build and execute command string
         command.CommandText = "CSSP_DelDoctorInSchedule";
         command.CommandType = CommandType.StoredProcedure;
         command.Parameters.Add("@DoctorStatusID", SqlDbType.Int).Value = DoctorStatusID;
         command.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = ScheduleID;

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

 public bool SQL_GetDoctorsAvailableToScheduleInto(List<Doctor> doctor, int programArea_RoomGroupID, int programArea_ProgramID, int selected_TermCalendarID, int selected_CampusID)
 {
    string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
    SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();
     
    int doctorID=0;
    int doctorStatusID=0;

    string firstName="";
    string lastName = "";
    string middleName = "";
    string doctorDegree = "";

    try
    {
        connection.Open();
        command.Connection = connection;

        // Build and execute command string
        command.CommandText = "CSSP_GetDoctorsAvailableToSchedule";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@ProgramsID", SqlDbType.Int).Value = programArea_ProgramID;
        command.Parameters.Add("@TermCalendarID", SqlDbType.Int).Value = selected_TermCalendarID;
        command.Parameters.Add("@CampusID", SqlDbType.Int).Value = selected_CampusID;
        command.Parameters.Add("@RoomGroupID", SqlDbType.Int).Value = programArea_RoomGroupID;



        SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            doctorStatusID = Convert.ToInt32(reader["doctorStatusID"]);
            doctorID = Convert.ToInt32(reader["doctorID"]);
            firstName= reader["firstName"] as string;
            lastName = reader["lastName"] as string;
            middleName = reader["middleName"] as string;
            doctorDegree = reader["DoctorDegree"] as string;
            doctor.Add(new Doctor( doctorID, doctorStatusID, firstName,  lastName, middleName,  doctorDegree));
        }
    }
    
    catch (Exception ex)
    {
        errorMessage = ex.Message.ToString();
        return false;
    }
    
    finally
    {
        if (connection.State != ConnectionState.Closed) connection.Close();
        command.Dispose();
    }

    return true;
 }


 public bool SQL_GetDoctorsCurrentlyInScheduleInto(List<Doctor> doctor, int programArea_RoomGroupID, int termCalendarID, int selected_CampusID)
 {
    string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
    SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

    int doctorID = 0;
    int doctorStatusID = 0;

    string firstName = "";
    string lastName = "";
    string middleName = "";
    string doctorDegree = "";

    try
    {
        connection.Open();
        command.Connection = connection;
         
        // Build and execute command string
        command.CommandText = "CSSP_GetDoctorsCurrentlyInSchedule";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@RoomGroupID", SqlDbType.Int).Value = programArea_RoomGroupID;
        command.Parameters.Add("@TermCalendarID", SqlDbType.Int).Value = termCalendarID;
        command.Parameters.Add("@CampusID", SqlDbType.Int).Value = selected_CampusID;

        SqlDataReader reader = command.ExecuteReader();

        errorMessage += "CSSP_GetDoctorsCurrentlyInSchedule ";
        while (reader.Read())
        {
            doctorStatusID = Convert.ToInt32(reader["doctorStatusID"]);
            doctorID = Convert.ToInt32(reader["doctorID"]);
            firstName= reader["firstName"] as string;
            lastName = reader["lastName"] as string;
            middleName = reader["middleName"] as string;
          //  doctorDegree = reader["DoctorDegree"] as string;

            doctor.Add(new Doctor( doctorID, doctorStatusID, firstName, lastName, middleName, doctorDegree));
        }
     }

     catch (Exception ex)
     {
         errorCode = -1;
         errorMessage = ex.Message.ToString();
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




