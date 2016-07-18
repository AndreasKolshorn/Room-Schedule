using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ProgReqRow
/// </summary>
public class ProgReqRow
{
    private int bufferRow; private int BufferRow { get { return bufferRow; } }
    private int b10;                  public int B10 {get{return B10;}}
    private string studentCourseNote; public string StudentCourseNote {get {return studentCourseNote;}}
    private string doctorLastName;    public string DoctorLastName {get {return doctorLastName;}}
    private string doctorFirstName;   public string DoctorFirstName {get {return doctorFirstName;}}
    private string courseTerm;        public string CourseTerm {get {return courseTerm;}}
    private int studentCourseID;      public int StudentCourseID {get {return studentCourseID;}}
    private int programShiftReqID;    public int ProgramShiftReqID {get {return programShiftReqID;}}
    private string courseName;       public string CourseName {get {return courseName;}}

    private decimal contactHours; public decimal ContactHours { get { return contactHours; } } 
    private string department; public string Department { get { return department; } }

    private bool calculateRowItem; public bool CalculateRowItem { get { return calculateRowItem; } }
    private int programReqItemID_Left; public int ProgramReqItemID_Left { get { return programReqItemID_Left; } }
    private int programReqItemID_Right; public int ProgramReqItemID_Right { get { return programReqItemID_Right; } }
    private string programReqItemName_Left; public string  ProgramReqItemName_Left {get {return programReqItemName_Left;}}
    private string programReqItemName_Right; public string ProgramReqItemName_Right {get {return programReqItemName_Right ;}}
    private string description_Left; public string Description_Left {get {return description_Left ;}}
    private string description_Right; public string Description_Right {get {return description_Right;}}
    private int programReqRowOpTypeID; public int ProgramReqRowOpTypeID { get { return programReqRowOpTypeID; } }

    private List<ProgReqItem> progReqItem = new List<ProgReqItem>();
    public List<ProgReqItem> Item { get { return this.progReqItem; } }

    public ProgReqRow() { }

	public ProgReqRow(
        int bufferRow,
        int b10,
        string studentCourseNote,
        string doctorLastName,
        string doctorFirstName,
        string courseTerm,
        int studentCourseID,
        int programShiftReqID,
        string courseName,
        string department,
        decimal contactHours
        )
	{
        this.bufferRow = bufferRow;
        this.b10=b10;
        this.studentCourseNote=studentCourseNote;
        this.doctorLastName=doctorLastName;
        this.doctorFirstName=doctorFirstName;
        this.courseTerm=courseTerm;
        this.studentCourseID=studentCourseID;
        this.programShiftReqID=programShiftReqID;
        this.courseName = courseName;
        this.department= department;
        this.contactHours = contactHours;
    }
}





