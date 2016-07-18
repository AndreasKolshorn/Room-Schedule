using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for StudentInfoStatus
/// </summary>
public class StudentInfoStatus
{
    public int StudentStatusID;
    public int StudentUID;
    public int ProgramsID;
    public int TermCalendarID;
    public int ProgramStatusID;
    public string TermCalendarName;
    public string ProgramStatusName;

    public string ProgramsCode;
	public string ProgramsDesciption;

    public StudentInfoStatus
        (int studentStatusID,
        int studentUID,
        int programsID,
        int termCalendarID,
        int programStatusID,
        string termCalendarName,
        string programStatusName,
        string ProgramsCode,
	    string ProgramsDesciption)
	    {
            this.StudentStatusID=studentStatusID;
            this.StudentUID= studentUID;
            this.ProgramsID= programsID;
            this.TermCalendarID= termCalendarID;
            this.ProgramStatusID= programStatusID;
            this.TermCalendarName = termCalendarName;
            this.ProgramStatusName = programStatusName;
            this.ProgramsCode = ProgramsCode;
            this.ProgramsDesciption = ProgramsDesciption;
    }
}
