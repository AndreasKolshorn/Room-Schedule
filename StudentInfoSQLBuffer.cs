using System;
using System.Collections.Generic;

/// <summary>
/// Summary description for ProgReqSQLRecord
/// </summary>
public class StudentInfoSQLBuffer
{
    public int BufferRow;
    public int StudentInfoID;
    public int StudentUID;
    
    public int StartProgram_TermCalendarID;
    public int ExpectedGrad_TermCalendarID;

    public bool ClinicContact;
    public bool Active;
    public string StudentInfo_NoteText;

    public int StudentCertTypeID;
    public string StudentCertTypeName;
    public DateTime AniversaryDate;
    public DateTime ExpireDate;
    public bool Waived;
    public string Firstname;
    public string Lastname;

	public StudentInfoSQLBuffer(
        int BufferRow,
        int StudentInfoID,
        int StudentUID,
    
        int StartProgram_TermCalendarID,
        int ExpectedGrad_TermCalendarID,
        bool ClinicContact,

        bool Active,
        string StudentInfo_NoteText,
        int StudentCertTypeID,

        string StudentCertTypeName,
        DateTime AniversaryDate,
        DateTime ExpireDate,

        bool Waived,
        string Firstname,
        string Lastname

        )
	{
        this.BufferRow = BufferRow;
        this.StudentInfoID= StudentInfoID;
        this.StudentUID= StudentUID;
        this.StartProgram_TermCalendarID= StartProgram_TermCalendarID;
        this.ExpectedGrad_TermCalendarID = ExpectedGrad_TermCalendarID;
        this.ClinicContact = ClinicContact;
        
        this.Active=Active;
        this.StudentInfo_NoteText=StudentInfo_NoteText;
        this.StudentCertTypeID=StudentCertTypeID;
        this.StudentCertTypeName=StudentCertTypeName;
        this.AniversaryDate=AniversaryDate;
        this.ExpireDate=ExpireDate;
        this.Waived=Waived;
        this.Firstname=Firstname;
        this.Lastname=Lastname;
    }
}
