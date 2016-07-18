using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DoctorStatus
/// </summary>
public class DoctorStatus
{
    private int doctorStatusID; // tblDoctorStatus
    private int doctorID;       // tblDoctorStatus
    private int statusAsOf_TermCalendarID; // tblDoctorStatus
    private string statusAsOf_TermText; // tblDoctorStatus

    private bool active = false;
    private int doctorTypeID;   // tblDoctorType
    private string  doctorTypeName;   // tblDoctorType
    private string doctorDegree;

    private List<CredentialType> credentialType_List = new List<CredentialType>();
    public List<CredentialType> CredentialType_List { get {return credentialType_List;}}


    public int DoctorStatusID { get { return doctorStatusID; } }
    public int DoctorID { get { return doctorID; } }

    public int StatusAsOf_TermCalendarID { get { return statusAsOf_TermCalendarID; } }
    public string StatusAsOf_TermText { get { return statusAsOf_TermText; } }

    public bool Active { get { return active; } }
    public int DoctorTypeID { get { return doctorTypeID; } }
    public string DoctorTypeName { get { return doctorTypeName; } }
    public string DoctorDegree { get { return doctorDegree; } }

        
	public DoctorStatus()
	{
	}

    public DoctorStatus(
        int doctorStatusID,
        int doctorID,
        int statusAsOf_TermCalendarID,
        string statusAsOf_TermText,

        bool active,
        int doctorTypeID,
        string doctorTypeName,
        string doctorDegree
        )
    {
        this.doctorStatusID = doctorStatusID;
        this.doctorID = doctorID;
        this.statusAsOf_TermCalendarID = statusAsOf_TermCalendarID;
        this.statusAsOf_TermText = statusAsOf_TermText;
        this.active = active;

        this.doctorTypeID = doctorTypeID;
        this.doctorTypeName = doctorTypeName;
        this.doctorDegree = doctorDegree;
    }
}