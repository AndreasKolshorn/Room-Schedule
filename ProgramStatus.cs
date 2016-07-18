using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for StudentInfoStatus
/// </summary>
public class ProgramStatus
{
    public int ProgramStatusID;
    public string ProgramStatusName;

    public ProgramStatus
        (int programStatusID,
        string programStatusName
        )
	    {
            this.ProgramStatusID= programStatusID;
            this.ProgramStatusName = programStatusName;
        }
}
