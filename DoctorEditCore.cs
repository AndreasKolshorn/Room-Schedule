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
public class DoctorEditCore
{
    const string BUTTON = "ButtonEdit";
    const string TERM = "Term";
    const string ACTIVE = "Active";
    const string TYPE = "Type";
    const string CREDENTIALS = "Credential";
    const string OTHER = "Other";

    public int TempCount = 0;
    public string grid_BUTTON { get { return BUTTON; } }
    public string grid_TERM { get { return TERM; } }
    public string grid_ACTIVE { get { return ACTIVE; } }
    public string grid_TYPE { get { return TYPE; } }
    public string grid_CREDENTIALS { get { return CREDENTIALS; } }
    public string grid_OTHER { get { return OTHER; } }

    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    // Basic properties from tblDoctor 
    private int doctorID; public int DoctorID { get{ return doctorID;}}
    private string lastName = "Unknown"; public string LastName { get { return lastName; } }
    private string firstName = "Unknown"; public string FirstName { get { return firstName; } }
    private string middleName = "Unknown"; public string MiddleName { get { return middleName; } }
    private string fullName = "Unknown"; public string FullName { get { return fullName; } }
    private int campusID = 0; public int CampusID { get { return campusID; } }

    private bool active = false; public bool Active { get { return active; } }
    private int noteID = 0; public int NoteID { get { return noteID; } }
    private string noteText; public string NoteText { get { return noteText; } }
    
    // Diagnostic variables
    public int doctor2ProgramsCount = 0;
    public int doctorStatusCount=0;
    
    public int doctorCredentialCountKill = 0;

    // Dynamic properties that associate to a particular quarter when they may change.
    private List<DoctorStatus> doctorStatus_List = new List<DoctorStatus>();
    public List<DoctorStatus> DoctorStatus_List { get { return doctorStatus_List; } }

    private List<Doctor2Programs> doctor2Programs_List = new List<Doctor2Programs>();
    public List<Doctor2Programs> Doctor2Programs_List { get { return doctor2Programs_List; } }

    private DataTable doctorStatusAsTable = new DataTable();
    public DataTable DoctorStatusAsTable { get { return this.doctorStatusAsTable; } }
   
    //
    public DoctorEditCore() {}

    public DoctorEditCore(int doctorID)
    {
        SQL_GetDoctor(doctorID);
        SQL_GetDoctor2Programs(doctorID, doctor2Programs_List);
        SQL_GetDoctorStatus(doctorID, doctorStatus_List);
        SQL_GetDoctorStatus2CredentialType(doctorID, doctorStatus_List);  // Assumes that doctorStatus_List already populated.

        CopyDoctorStatusListIntoDataTable(this.DoctorStatusAsTable, this.doctorStatus_List);
    }

    public void CopyDoctorStatusListIntoDataTable(DataTable doctorStatusAsTable, List<DoctorStatus> doctorStatus_List)
    {
        // Add standard row header column
        DataColumn dcol = new DataColumn(BUTTON, typeof(System.String));
        doctorStatusAsTable.Columns.Add(dcol);

        dcol = new DataColumn(TERM, typeof(System.String));
        doctorStatusAsTable.Columns.Add(dcol);

        dcol = new DataColumn(ACTIVE, typeof(System.String));
        doctorStatusAsTable.Columns.Add(dcol);

        dcol = new DataColumn(TYPE, typeof(System.String));
        doctorStatusAsTable.Columns.Add(dcol);

        dcol = new DataColumn(CREDENTIALS, typeof(System.String));
        doctorStatusAsTable.Columns.Add(dcol);

        dcol = new DataColumn(OTHER, typeof(System.String));
        doctorStatusAsTable.Columns.Add(dcol);

        foreach (DoctorStatus DS in doctorStatus_List)
        {
            TempCount += 1;
            DataRow drow = doctorStatusAsTable.NewRow();
            drow[TERM] = DS.StatusAsOf_TermText;
            drow[ACTIVE] = (DS.Active? "+" :"");
            drow[TYPE] = DS.DoctorTypeName;
            drow[CREDENTIALS] = "";
            drow[OTHER] = DS.DoctorDegree;
            //
            doctorStatusAsTable.Rows.Add(drow);
        }
    }

    public void CopyDoctorStatusListIntoDataTable1(DataTable doctorStatusAsTable, List<DoctorStatus> doctorStatus_List)
    {
        // Add standard row header column
        DataColumn dcol = new DataColumn("TERM", typeof(System.String));
        doctorStatusAsTable.Columns.Add(dcol);

        DataRow drow = doctorStatusAsTable.NewRow();
        drow["TERM"] = "Test";
        doctorStatusAsTable.Rows.Add(drow);

        /*
        foreach (DoctorStatus DS in doctorStatus_List)
        {
            TempCount += 1;
            DataRow drow = doctorStatusAsTable.NewRow();
            drow["TERM"] = DS.StatusAsOf_TermText;
            drow[ACTIVE] = (DS.Active ? "+" : "");
            drow[TYPE] = DS.DoctorTypeName;
            drow[CREDENTIALS] = "";
            drow[OTHER] = DS.DoctorDegree;
            //
            doctorStatusAsTable.Rows.Add(drow);
        }
        */
    }

    private bool SQL_GetDoctor(int DoctorID)
    {
            string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
            SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
            System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

            try
            {
                connection.Open();
                command.Connection = connection;

                // Build and execute command string
                command.CommandText = "CSSP_GetDoctor";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = DoctorID;

                SqlDataReader reader = command.ExecuteReader();

                // Expect one record
                reader.Read();
                this.doctorID = Convert.ToInt32(reader["doctorID"]);
                    
                this.lastName = reader["lastname"] as string;
                this.firstName = reader["firstname"] as string;
                this.middleName = reader["middlename"] as string;
                this.active = Convert.ToBoolean(reader["Active"]);
                this.noteID = Convert.ToInt32(reader["noteID"]);
                this.noteText = reader["noteText"] as string;
                this.campusID = Convert.ToInt32(reader["CampusID"]);

                this.fullName = this.firstName + " " + this.lastName;

                reader.Close();
            }

            catch (Exception ex)
            {
                errorCode = -1;
                errorMessage = "Request failed: DoctorEditCore.cs/SQL_GetDoctor/Catch --> " + ex.Message;
                return false;
            }

            finally
            {
                if (connection.State != ConnectionState.Closed) connection.Close();
                command.Dispose();
            }

            return true;
        }

    private bool SQL_GetDoctor2Programs(int DoctorID, List<Doctor2Programs> doctor2Programs)
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_GetDoctor2Programs";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = DoctorID;

            SqlDataReader reader = command.ExecuteReader();

             while (reader.Read())    // Build a new program object for each programID 
            {
                doctor2ProgramsCount += 1;

                 doctor2Programs.Add(new 
                     Doctor2Programs(Convert.ToInt32(reader["doctor2ProgramsID"]), 
                     Convert.ToInt32(reader["doctorID"]), 
                     Convert.ToInt32(reader["programsID"]),
                     reader["programsCode"] as string, 
                     reader["description"] as string));
             }

             reader.Close();
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed: DoctorEditCore.cs/SQL_GetDoctor2Programs/Catch --> " + ex.Message;
            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }

        return true;
    }

    private bool SQL_GetDoctorStatus(int DoctorID, List<DoctorStatus> doctorStatus)
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();
       //  doctorStatusCount += 1;
        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_GetDoctorStatus";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = DoctorID;

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())    // Build a new program object for each programID 
            {
                doctorStatusCount += 1;
                doctorStatus.Add(new DoctorStatus (
                    Convert.ToInt32(reader["doctorStatusID"]),
                    Convert.ToInt32(reader["doctorID"]),
                    Convert.ToInt32(reader["statusAsOf_TermCalendarID"]),
                    reader["statusAsOf_TermText"] as String,    
                    Convert.ToBoolean(reader["active"]),
                    Convert.ToInt32(reader["doctorTypeID"]),
                    reader["doctorTypeName"] as String,
                    reader["doctorDegree"] as String
                    ));
            }
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed: DoctorEditCore.cs/SQL_GetDoctorStatus/Catch --> " + ex.Message;
            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }

        return true;
    }

    private bool SQL_GetDoctorStatus2CredentialType(int DoctorID, List<DoctorStatus> doctorStatus_List)
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "[CSSP_GetDoctorStatus2CredentialType]";
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader;

            foreach (DoctorStatus ds in doctorStatus_List) // Loop thru multiple possible status recs.
            {
                command.Parameters.Clear();
                command.Parameters.Add("@DoctorStatusID", SqlDbType.Int).Value = ds.DoctorStatusID;
                reader = command.ExecuteReader();

                while (reader.Read())    // Add credential list to each doctor status record.
                {
                        ds.CredentialType_List.Add(new CredentialType(
                        Convert.ToInt32(reader["CredentialTypeID"]),
                        reader["credentialTypeName"] as string,
                        reader["description"] as string));
                }

                reader.Close();

            }
        }

        catch (Exception ex)
        {
            errorCode = -1;
            errorMessage = "Request failed: DoctorEditCore.cs/SQL_GetDoctorStatus2CredentialType/Catch --> " + ex.Message;
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

