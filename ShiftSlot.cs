using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ShiftSlot holds data related to one of multiple possible students scheduled 
/// in a shift cell.
/// </summary>
public class ShiftSlot
{
    // Core attributes from tblSchedule2Student
    private int s2SID = -1;
    private int studentUID = -1;
    private int shiftSlotTypeID = -1;
    private int shiftSlotRowID = -1;
    private int statusTypeID =  -1;
    private int enrollmentTypeID = -1;
    private int programShiftReqID = -1;
    private int sROfferID = -1;
    private bool timeCardRequired = false;

    private int programsID;
    private string programsCode;
    private string programsName;

    // Related UI friendly text of Core Attributes
    private string shiftSlotTypeName = "N/A";
    private string studentName = "N/A";
    private string courseName = "N/A";
    // 
    public int S2SID { get { return s2SID; } set { s2SID = value; } }
    public int StudentUID { get { return studentUID; } set { studentUID = value; } }
    public int ShiftSlotTypeID { get { return shiftSlotTypeID; } set { shiftSlotTypeID = value; } }
    public int ShiftSlotRowID { get { return shiftSlotRowID; } set { shiftSlotRowID = value; } }
 
    public int StatusTypeID { get { return statusTypeID; } set {statusTypeID = value; } }
    public int EnrollmentTypeID { get { return enrollmentTypeID ; } set { enrollmentTypeID= value; } }
    public int ProgramShiftReqID{ get { return programShiftReqID ; } set { programShiftReqID= value; } }
    public int SROfferID { get { return sROfferID; } set { sROfferID = value; } }

    public bool TimeCardRequired { get { return timeCardRequired; } set { timeCardRequired = value; } }
    public string ShiftSlotTypeName { get { return shiftSlotTypeName; } set { shiftSlotTypeName = value; } }

    public string StudentName { get { return studentName; } set { studentName = value; } }
    public string CourseName { get { return courseName; } set { courseName = value; } }

    public int ProgramsID { get { return programsID; } set { programsID = value; } }
    public string ProgramsCode { get { return programsCode; } set { programsCode = value; } }
    public string ProgramsName { get { return programsName; } set { programsName = value; } }

    // Use when we know the srOfffID. Like loading schedule from DB and building slots.
    public ShiftSlot(
        int s2SID,
        int enrollmentTypeID,
        int studentUID,
        int shiftSlotRowID, 
        int shiftSlotTypeID, 
        string shiftSlotTypeName,
        string studentName, 
        int programShiftReqID, 
        string courseName,
        bool timeCardRequired,
        int srofferid,
        int programsID,
        string programsCode,
        string programsName
        )
    {
        this.s2SID = s2SID;
        this.enrollmentTypeID = enrollmentTypeID;
        this.studentUID = studentUID;
        this.shiftSlotRowID = shiftSlotRowID;
        this.shiftSlotTypeID = shiftSlotTypeID;
        this.shiftSlotTypeName = shiftSlotTypeName;
        this.studentName = studentName;
        this.programShiftReqID = programShiftReqID;
        this.courseName = courseName;
        this.timeCardRequired = timeCardRequired;
        this.sROfferID = srofferid;

        this.programsID =programsID;
        this.programsCode =programsCode;
        this.programsName = programsName;
    }

    // Use adding student/course combo to schedule and srOfffID is unknown.
    public ShiftSlot(
        int s2SID,
        int enrollmentTypeID,
        int studentUID,
        int shiftSlotRowID,
        int shiftSlotTypeID,
        string shiftSlotTypeName,
        string studentName,
        int programShiftReqID,
        string courseName,
        bool timeCardRequired,
        int programsID,
        string programAreaName
        )
    {
        this.s2SID = s2SID;
        this.enrollmentTypeID = enrollmentTypeID;
        this.studentUID = studentUID;
        this.shiftSlotRowID = shiftSlotRowID;
        this.shiftSlotTypeID = shiftSlotTypeID;
        this.shiftSlotTypeName = shiftSlotTypeName;
        this.studentName = studentName;
        this.programShiftReqID = programShiftReqID;
        this.courseName = courseName;
        this.timeCardRequired = timeCardRequired;
        this.programsID = programsID;
        this.programsName = programAreaName;
    }
 


	public ShiftSlot()
	{
	}
}