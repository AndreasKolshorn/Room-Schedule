using System;
using System.Collections.Generic;
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;


/// <summary>
/// Summary description for StudentList
/// </summary>
public class StudentListItem
{
    // Basic properties. 
    private int studentUID = -1;             // StudentUId assigned to this object by constructor.
    private string lastname = "Unknown";
    private string firstname = "Unknown";

    public int StudentUID { get { return studentUID; } }
    public string Lastname { get { return lastname; } }
    public string Firstname { get { return firstname; } }


	public StudentListItem(int studentUID, string lastname, string firstname)
	{
		this.studentUID=studentUID;
        this.lastname = lastname;
        this.firstname = firstname;
	}

}