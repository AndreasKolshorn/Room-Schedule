﻿#define DebugOn
using System;
using System.Collections.Generic;
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

/// <summary>
/// Summary description for StudentShiftReq
/// </summary>
public class StudentShiftReq
{
    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    private int ProgramsID;
    private int programShiftReqID;
    private string courseID;
    private string courseName;
    private string programName;
    private string department;
    private string grade_Date;
    private string grade;
    private int sROfferID;
    private int statusTypeID;
    private int shiftSlotTypeID;
    private string shiftSlotTypeName;
    private int courseTypeID;


    private int roomID;
    private string roomName;

    private int dayTypeID;
    private string dayName;

    private int shiftTimeID;
    private string shiftName;

    public int RoomID { get { return roomID; } } 
    public string RoomName { get {return roomName;}}
    
    public int ShiftTimeID { get { return shiftTimeID; } } 
    public string ShiftName { get {return shiftName;}}

    public int DayTypeID { get { return dayTypeID; } } 
    public string DayName { get {return dayName;}}


    private int termCalendarID; // Term this shift is scheduled
    private string textTerm; // Term name english.

    //
    public int ProgramShiftReqID { get {return programShiftReqID;}}
    public string CourseID { get { return courseID; } } 

    public string CourseName { get {return courseName;}}
    public string ProgramName  { get {return programName;}}
    public string Department { get {return department;}}
    public string Grade_Date { get { return grade_Date; } }
    public string Grade { get {return grade;}}
    public int SROfferID { get {return sROfferID;}}
    public int StatusTypeID { get { return statusTypeID; } }
    public int ShiftSlotTypeID { get {return shiftSlotTypeID;}}
    public string ShiftSlotTypeName { get {return shiftSlotTypeName;}}
    public int CourseTypeID { get { return courseTypeID; } }

    public string TextTerm { get { return textTerm; } }
    public int TermCalendarID { get { return termCalendarID; } }

    public StudentShiftReq()
	{
	}
 
    public StudentShiftReq(
        int programsID,
        int programShiftReqID, 
        string courseID,
        string courseName,
        string programName,
        string department,
        string grade_Date,
        string grade,
        int sROfferID,
        int statusTypeID,
        int shiftSlotTypeID,
        string shiftSlotTypeName,
        int courseTypeID,
        int termCalendarID,
        string textTerm,

        int roomID,
        string roomName,
        int dayTypeID,
        string dayName,
        int shiftTimeID,
        string shiftName
        )
    {
        this.ProgramsID = programsID;
        this.programShiftReqID= programShiftReqID;
        this.courseID = courseID;

        this.courseName = courseName;
        this.programName = programName;
        this.department = department;
        this.grade = grade_Date;
        this.grade = grade;
        this.sROfferID = sROfferID;
        this.statusTypeID = statusTypeID;
        this.shiftSlotTypeID = shiftSlotTypeID;
        this.shiftSlotTypeName = shiftSlotTypeName;
        this.courseTypeID = courseTypeID;
        this.termCalendarID = termCalendarID;
        this.textTerm = textTerm;

        this.roomID=roomID;
        this.roomName =roomName;
        this.dayTypeID = dayTypeID;
        this.dayName=dayName; 
        this.shiftTimeID=shiftTimeID;
        this.shiftName = shiftName;
    }

    public bool SQL_GetStudentShiftsInto(List<StudentShiftReq> studentShiftReq, int programID, int studentUID)
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

            command.CommandText = "CSSP_SelectStudentShifts";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@ProgramsID", SqlDbType.Int).Value = programID;
            command.Parameters.Add("@StudentUid", SqlDbType.Int).Value = studentUID;
            //    command.Parameters.Add("@ShiftSlotTypeID", SqlDbType.Int).Value = shiftSlotTypeID;

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                studentShiftReq.Add(new StudentShiftReq(
                    ProgramsID
                    , Convert.ToInt32(reader["ProgramShiftReqID"])
//                    , Convert.ToInt32(reader["CourseID"])
                    , reader["CourseID"] as String
                    , reader["CourseName"] as String
                    , reader["ProgramName"] as String
                    , reader["Department"] as String
                    , reader["Grade_Date"] as String
                    , reader["Grade"] as String
                    , Convert.ToInt32(reader["SROfferID"])
                    , Convert.ToInt32(reader["StatusTypeID"])
                    , Convert.ToInt32(reader["ShiftSlotTypeID"])
                    , reader["ShiftSlotTypeName"] as String
                    , Convert.ToInt32(reader["CourseTypeID"])
                    , Convert.ToInt32(reader["TermCalendarID"])
                    , reader["TextTerm"] as String

                    , Convert.ToInt32(reader["RoomID"])
                    , reader["RoomName"] as String

                    , Convert.ToInt32(reader["DayTypeID"])
                    , reader["DayName"] as String
                    
                    , Convert.ToInt32(reader["ShiftTimeID"])
                    , reader["ShiftName"] as String
                   
                    ));
                haveRecord = true;
            }

            if (!haveRecord)        // Just in case.
            {
                errorCode = -1;
                errorMessage = "CSSP_SelectStudentShifts (" + programID.ToString() + "," + studentUID.ToString()+") Returned no records";
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