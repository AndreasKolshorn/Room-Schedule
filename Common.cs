
#define DebugOn
// #define DebugOff

using System;
using System.Collections.Generic;
using System.Web;

public enum Globals: int
{
    DiagnosticMode=0,   // 0=off, 1=on  Set to toggle diagnostic messaging on or off.

    SQLDataType_Bool = 1,           // Values defined in DB ClinicSchedule table tblSQLDataType. 
    SQLDataType_Float = 2,
    SQLDataType_DateTime = 3,

    ColWidthMain = 100,
    ColWidthOffSite = 100,
    ColWidthCell=140,

    
    // Values from tblCredentialType (CredentialTypeID_ + CredentialTypeName = CredentialTypeID; )  
    CredentialTypeID_MAC = 1,
    CredentialTypeID_PhD = 2,
    CredentialTypeID_MD = 3,
    CredentialTypeID_ND = 4,
    CredentialTypeID_LaC = 5,
    CredentialTypeID_Rd = 6,
    CredentialTypeID_MDChina = 7,

    CredentialTypeID_PsyD = 8,
    CredentialTypeID_LMHC = 9,
    CredentialTypeID_DAOM = 14,
                
    // Should deprecate these values on cleanup (but..Doctor.cs.. etc have references)
    DoctorProgramID_AOM = 1,        // Derived from values in tblDoctorProgram
    DoctorProgramID_ND = 2,         
    DoctorProgramID_AOM_ND = 3,
    DoctorProgramID_CHM = 4,
    DoctorProgramID_MSN = 5,
    DoctorProgramID_MSA = 6,
    //
     //
    // tblProgramID
    // e.g. used in StudentSchedule.aspx.cs to determine program to work on  
    ProgramID_AOM = 23,   // Naturopathic Med/ Acup & Oriental Medicine
    ProgramID_ND = 27,      // Naturopathic Medicine
    ProgramID_CHM = 29,     // Naturopathic Medicine/ Chinese Herbal Medicine 
    ProgramID_NTM = 38,     // Nutrition - Master
    ProgramID_MACP = 75,    // MACP
    ProgramID_AY=71,        
    ProgramID_DAOM=2,


    // These should be deleted if nothing breaks 
    ProgramID_MSAOM = 23,   // Naturopathic Med/ Acup & Oriental Medicine
    ProgramID_MSA = 24,     // Naturopathic Med/ Acupuncture
    ProgramID_MSN = 38,     // Nutrition - Master

    // tblCourseType
    CourseTypeID_Required=1,
    CourseTypeID_Elective = 2,
    CourseTypeID_Interim = 3,

    //tblEnrollmentType
    EnrollmentTypeID_Credit=1,      // Credit (C)   
    EnrollmentTypeID_Elective=3,    // Elective (E)
    EnrollmentTypeID_Audit = 4,     // Elective Audit (EA)   

    // ProgramID_ProTran=xx  // New program need definition.

    // Should deprecate these values on cleanup
    // See Schedule.cs for typical usage.
 //   scheduleDoctor = 1,     // Major area in app focus on maintaining Doctor schedule
 //   scheduleStudent = 2,     // Major area in app focus on maintaining Student schedule

    DoctorCredentialLength = 8,  //Truncate display of credentials longer than x chars.
    DoctorStatusID_Empty =-1,
    CellFontSize =8,          // Default font size e.g. text in a cell.
    ShiftListFontSize = 8,
    //
    // See tblShiftStatus for actual definitions (tbl.StatusType.StatustypeID)
    StatusTypeID_Unknown=0,
    StatusTypeID_Complete = 1,          // + Course passed. Recorded in CAMS.
    StatusTypeID_Needed = 2,            // + Shift needed. Never taken.
    StatusTypeID_ScheduledAndReg = 3,   // + Shift scheduled and entered in CAMS.
    StatusTypeID_ScheduledNotReg = 4,   // Shift scheduled but NOT entered in CAMS.
    StatusTypeID_Failed = 5,            // + Shift taken but failed. Entered in CAMS.
    StatusTypeID_RegNotScheduled = 6,   // We have no grade so registered but no SROfferID so not schedueled.
    StatusTypeID_ScheduledAndRegWith_IP =8,    // CAM returning an IP code and have SROfferID.
    StatusTypeID_RegNotScheduledWith_IP = 9,    // CAM returning an IP code and No SROfferID.
    StatusTypeID_ScheduledNotRegButPassed = 10, // User rescheduled a previously passed course.
    StatusTypeID_ScheduledAndRegWith_I = 11,    // CAM returning an I code and have SROfferID.
    StatusTypeID_RegNotScheduledWith_I = 12,    // CAM returning an I code and No SROfferID.
    StatusTypeID_ScheduledAndRegWith_PC = 13,    // CAM returning an PC code and have SROfferID.
    StatusTypeID_RegNotScheduledWith_PC = 14,    // CAM returning an PC code and No SROfferID.
  
/*
3	Early Primary (1EP)      
5	Obs (2nd 1/2)            
4	Obs (First 1/2)          
1	Primary (1)              
2	Secondary/obsvr (2)          
*/

    //
    // See tblShiftSlotType for actual definitions (tblShiftSlotType.ShiftSlotTypeID).
    ShiftSlotTypeID_NA = 0,       
    ShiftSlotTypeID_Primary=1,
    ShiftSlotTypeID_Secondary=2,
    ShiftSlotTypeID_1EP=3,

    ShiftSlotTypeID_ObsSecondHalf=5,
    ShiftSlotTypeID_ObsFirstHalf=4,

    //
    // 
    ShiftSlotRow_MAX = 4,   // Maximum # of possible shiftslots we can fill.
                            // ..Select ShiftSlotRow_MAX =count(*) from tblShiftSlotRow

    // Used in RequirementsCore and related files as row display hints.
    RowTypeBlank=0,        
    RowTypeData=1,
    RowTypeTotal=2,
    RowTypeRequired=3,

    // BreadCrumbRoot
    BreadCrumbRoot_All=1,
    BreadCrumbRoot_Campus=2,
    BreadCrumbRoot_None = 3
}

public class CommonOLD
{
   private int selected_TermCalendarID;
   private int selected_DoctorProgramID;


   public int Selected_TermCalendarID {
       get { return selected_TermCalendarID; } 
       set { selected_TermCalendarID = value; } 
   }

   public int Selected_DoctorProgramID { 
       get { return selected_DoctorProgramID; } 
       set { selected_DoctorProgramID = value; } 
   }

}
