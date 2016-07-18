using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for StudentInfoCert
/// </summary>
public class StudentInfoCert
{
    public bool FromDB;
    public int BufferRow;
    public int StudentCertTypeID;
    public string StudentCertTypeName;
    public DateTime AniversaryDate;
    public DateTime ExpireDate;
    public bool Waived;

	public StudentInfoCert
        (bool fromDB,
        int bufferRow,
        int studentCertTypeID,
        string studentCertTypeName,
        DateTime aniversaryDate,
        DateTime expireDate,
        bool waived)
	{
        this.FromDB = fromDB;
        this.BufferRow = bufferRow;
        this.StudentCertTypeID=studentCertTypeID;
        this.StudentCertTypeName=studentCertTypeName;
        this.AniversaryDate=aniversaryDate;
        this.ExpireDate=expireDate;
        this.Waived = waived;
	}
}