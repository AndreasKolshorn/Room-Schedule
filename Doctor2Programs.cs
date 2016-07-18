using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Doctor2Programs
/// </summary>
public class Doctor2Programs
{
    int doctor2ProgramsID; public int Doctor2ProgramsID { get { return doctor2ProgramsID; } }
    int doctorID; public int DoctorID { get { return doctorID; } }
    int programsID; public int ProgramsID { get { return programsID; } }
    string programsCode; public string ProgramsCode { get { return programsCode; } }
    string description; public string Description { get { return description; } }

	public Doctor2Programs()
	{
	}

    public Doctor2Programs(int doctor2ProgramsID, int doctorID, int programsID, string programsCode, string description)
    {
        this.doctor2ProgramsID = doctor2ProgramsID;
        this.doctorID = doctorID;
        this.programsID = programsID;
        this.programsCode = programsCode;
        this.description = description;
    }



}