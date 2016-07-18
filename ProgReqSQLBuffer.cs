using System;
using System.Collections.Generic;

/// <summary>
/// Summary description for ProgReqSQLRecord
/// </summary>
public class ProgReqSQLBuffer
{
    public int BufferRow;
    public int B1000;
    public int ProgramsID;
    public string ProgramDescription;
    public string ProgramsCode;
    public int B100;
    public int CourseTypeID;
    public string CourseTypeName;			
    public int ClassRoleTypeID;
    public string ClassRoleTypeName;
    public int B10;
    public string CourseName;
    public int B1;
    public int OrderAcrossGridview;
    public string ProgramReqItemName;
    public int EnrollmentTypeID;
    public string EnrollmentTypeName;  
    public int ShiftSlotTypeID;
    public string ShiftSlotTypeName;

    public string ProgramReqNote;
    public string StudentCourseNote;
    public string DoctorLastName;
    public string DoctorFirstName;
    public string CourseTerm;
    public decimal CourseData_Decimal;
    public string CourseData_NoteText;
    public int CourseData_NoteID;
    public int StudentCourseDataID;
    public bool CalculateColumnTotal;    

    public bool CalculateRowItem;

    public int ProgramReqItemID_Left;
    public int ProgramReqItemID_Right;
    public string ProgramReqItemName_Left;
    public string ProgramReqItemName_Right;
    public string Description_Left;
    public string Description_Right;
        
    public string ProgramReqItemUIName_Left;
    public string ProgramReqItemUIName_Right;

    public int ProgramReqItemUIWidth_Left;
    public int ProgramReqItemUIWidth_Right;
    
    public int ProgramReqRowOpTypeID;

    public int StudentUID;
    public int StudentCourseID;
    public int ProgramReqColumnSetID;

    public decimal ProgramReqTotal;

    public int ProgramShiftReqID;
    public int ProgramReqItemTypeID;
    public decimal ShiftHours;
    public string Department;

    public string ProgramReqItemUIName;
    public int ProgramReqItemUIWidth;

    public string DataTypeSQL;
    public string DataTypeCS;
    public bool IsNumber;
    public string ExpectedGradTerm;

	public ProgReqSQLBuffer(
        int BufferRow,
        int B1000,
        int ProgramsID,
        string ProgramDescription,
        string ProgramsCode,
        int B100,
        int CourseTypeID,
        string CourseTypeName,
        int ClassRoleTypeID,
        string ClassRoleTypeName,
        int B10,
        string CourseName,
        int B1,
        int OrderAcrossGridview,
        string ProgramReqItemName,
        int EnrollmentTypeID,
        string EnrollmentTypeName,
        int ShiftSlotTypeID,
        string ShiftSlotTypeName,

        string DoctorLastName,
        string DoctorFirstName,
        string CourseTerm,
        decimal CourseData_Decimal,
        string CourseData_NoteText,
        int CourseData_NoteID,
        int StudentCourseDataID,

        bool CalculateColumnTotal,
        int StudentUID,
        int StudentCourseID,
        int ProgramReqColumnSetID,

        decimal ProgramReqTotal,

        int ProgramShiftReqID,
        int ProgramReqItemTypeID,

        decimal ShiftHours,
        string Department,

        string ProgramReqItemUIName,
        int ProgramReqItemUIWidth,

        string DataTypeSQL,
        string DataTypeCS,
        bool IsNumber,
        string ExpectedGradTerm,

        bool CalculateRowItem,
        int ProgramReqItemID_Left,
        int ProgramReqItemID_Right,
        string ProgramReqItemName_Left,
        string ProgramReqItemName_Right,
        string Description_Left,
        string Description_Right,
        int ProgramReqRowOpTypeID,

        string ProgramReqItemUIName_Left,
        string ProgramReqItemUIName_Right,

        int ProgramReqItemUIWidth_Left,
        int ProgramReqItemUIWidth_Right
        )
	{
        this.BufferRow = BufferRow;
        this.B1000=B1000;
        this.ProgramsID = ProgramsID;
        this.ProgramDescription = ProgramDescription;
        this.ProgramsCode = ProgramsCode;
        this.B100 = B100;
        this.CourseTypeID = CourseTypeID;
        this.CourseTypeName = CourseTypeName;
        this.ClassRoleTypeID = ClassRoleTypeID;
        this.ClassRoleTypeName = ClassRoleTypeName;
        this.B10 = B10;
        this.CourseName = CourseName;
        this.B1 = B1;
        this.OrderAcrossGridview = OrderAcrossGridview;
        this.ProgramReqItemName = ProgramReqItemName;
        this.EnrollmentTypeID = EnrollmentTypeID;
        this.EnrollmentTypeName = EnrollmentTypeName;
        this.ShiftSlotTypeID = ShiftSlotTypeID;
        this.ShiftSlotTypeName = ShiftSlotTypeName;

        this.DoctorLastName = DoctorLastName;
        this.DoctorFirstName = DoctorFirstName;
        this.CourseTerm = CourseTerm;
        this.CourseData_Decimal=CourseData_Decimal;
        this.CourseData_NoteText = CourseData_NoteText;
        this.CourseData_NoteID = CourseData_NoteID;
        this.StudentCourseDataID = StudentCourseDataID;

        this.CalculateColumnTotal = CalculateColumnTotal;

        this.StudentUID = StudentUID;
        this.StudentCourseID = StudentCourseID;
        this.ProgramReqColumnSetID = ProgramReqColumnSetID;

        this.ProgramReqTotal = ProgramReqTotal;

        this.ProgramShiftReqID = ProgramShiftReqID;
        this.ProgramReqItemTypeID = ProgramReqItemTypeID;

        this.ShiftHours = ShiftHours;
        this.Department = Department;

        this.ProgramReqItemUIName = ProgramReqItemUIName;
        this.ProgramReqItemUIWidth = ProgramReqItemUIWidth;

        this.DataTypeSQL = DataTypeSQL;
        this.DataTypeCS = DataTypeCS;
        this.IsNumber = IsNumber;
        this.ExpectedGradTerm = ExpectedGradTerm;

        this.CalculateRowItem = CalculateRowItem;
        this.ProgramReqItemID_Left =ProgramReqItemID_Left;
        this.ProgramReqItemID_Right =ProgramReqItemID_Right;
        this.ProgramReqItemName_Left =ProgramReqItemName_Left;
        this.ProgramReqItemName_Right =ProgramReqItemName_Right;
        this.Description_Left =Description_Left;
        this.Description_Right =Description_Right;
        this.ProgramReqRowOpTypeID = ProgramReqRowOpTypeID;
        this.ProgramReqItemUIName_Left=ProgramReqItemUIName_Left;
        this.ProgramReqItemUIName_Right=ProgramReqItemUIName_Right;
        this.ProgramReqItemUIWidth_Left=ProgramReqItemUIWidth_Left;
        this.ProgramReqItemUIWidth_Right = ProgramReqItemUIWidth_Right;
	}
}

/*
CalculateRowItem
ProgramReqItemID_Left
ProgramReqItemID_Right
ProgramReqItemName_Left
ProgramReqItemName_Right
Description_Left
Description_Right
ProgramReqRowOpTypeID
*/