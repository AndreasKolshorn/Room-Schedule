using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ProgReqItem
/// </summary>
public class ProgReqItem
{
    private int bufferRow; private int BufferRow { get { return bufferRow; } }
    private int b1;                     public int B1 { get { return b1;}}
    private int orderAcrossGridview;    public int OrderAcrossGridview { get { return  orderAcrossGridview;}}
    private string programReqItemName;  public string ProgramReqItemName { get { return  programReqItemName;}}
    private int programReqItemTypeID;   public int ProgramReqItemTypeID  { get {return programReqItemTypeID;}}
    private string dataTypeSQL;         public string DataTypeSQL { get { return dataTypeSQL;}}
    private string dataTypeCS;          public string DataTypeCS { get { return dataTypeCS;}}
    private bool isNumber;              public bool IsNumber { get { return isNumber;}}
    private int programReqItemUIWidth;  public int ProgramReqItemUIWidth { get { return programReqItemUIWidth;}}
    private string programReqItemUIName; public string ProgramReqItemUIName { get { return programReqItemUIName;}}


    // In the database we store all values as decimal 
    private decimal courseData_Decimal; public decimal CourseData_Decimal { get { return courseData_Decimal; } set { courseData_Decimal = value; } }
    private string courseData_NoteText; public string CourseData_NoteText { get { return courseData_NoteText; } set { courseData_NoteText = value; } }
    private int courseData_NoteID; public int CourseData_NoteID { get { return courseData_NoteID; } }
    private decimal programReqTotal; public decimal ProgramReqTotal { get { return programReqTotal; } set { programReqTotal = value; } }


    private decimal oldCourseData_Decimal; public decimal OldCourseData_Decimal { get { return oldCourseData_Decimal; } set { oldCourseData_Decimal = value; } }
    private string oldCourseData_NoteText; public string OldCourseData_NoteText { get { return oldCourseData_NoteText; } set { oldCourseData_NoteText = value; } }
    private int oldCourseData_NoteID; public int OldCourseData_NoteID { get { return oldCourseData_NoteID; } set { oldCourseData_NoteID = value; } }

    private int studentCourseDataID; public int StudentCourseDataID { get { return studentCourseDataID; } }
    private int programReqColumnSetID; public int ProgramReqColumnSetID { get { return programReqColumnSetID; } }
    private bool itemErrorFlag; public bool ItemErrorFlag { get { return itemErrorFlag; } set { itemErrorFlag = value; } }

    // Metadata on this column item

    private bool canBeSaved; public bool CanBeSaved { get { return canBeSaved; } }
    public bool calculateRowItem; public bool CanDoMath { get { return calculateRowItem; } }

    public ProgReqItem(
        int bufferRow,
        int b1,
        int orderAcrossGridview,
        string programReqItemName,
        int programReqItemTypeID, 
        string dataTypeSQL, 
        string dataTypeCS, 
        bool isNumber,
        int programReqItemUIWidth,
        string programReqItemUIName,

        decimal courseData_Decimal,
        string courseData_NoteText,
        int courseData_NoteID,
        int studentCourseDataID,
        int programReqColumnSetID,
        decimal programReqTotal,
        bool canBeSaved,            // Object Flag true for editable/save data. false for calculate columns.
        bool calculateRowItem
        )
	    {
            this.bufferRow = bufferRow;
            this.b1 = b1;
            this.orderAcrossGridview=orderAcrossGridview;
            this.programReqItemName=programReqItemName.TrimEnd();
		    this.programReqItemTypeID =programReqItemTypeID;
            this.dataTypeSQL = dataTypeSQL;
            this.dataTypeCS = dataTypeCS;
            this.isNumber = isNumber;
            this.studentCourseDataID = studentCourseDataID;
            this.programReqColumnSetID = programReqColumnSetID;

            this.programReqItemUIWidth = programReqItemUIWidth;
            this.programReqItemUIName = programReqItemUIName;

            this.oldCourseData_Decimal = courseData_Decimal;
            this.oldCourseData_NoteID = courseData_NoteID;
            this.oldCourseData_NoteText = courseData_NoteText;
            this.programReqTotal = programReqTotal;


            this.canBeSaved = canBeSaved;
            this.calculateRowItem = calculateRowItem;

            if( courseData_Decimal==-1)
                this.courseData_Decimal = 0;
            else
                this.courseData_Decimal = courseData_Decimal;

            if (courseData_NoteID == 1)
            {
                this.courseData_NoteText = "";
                this.courseData_NoteID = courseData_NoteID;
            }
            else
            {
                this.courseData_NoteText = courseData_NoteText;
                this.courseData_NoteID = courseData_NoteID;
            }
        }
}




