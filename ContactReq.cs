using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Contact requirements (req) generally include things like contact counts and hours. In use
/// a program has such a spec and like other program requirements varies on students start into 
/// the program.
/// Other instances include a students cummulative total which is later differenced from prog req
/// to indicate what they still need.
///
/// </summary>
public class ContactReq
{
    private decimal mainHours;
    private decimal subHours;
    private int primaryContacts;
    private int totalContacts;
    private int fPI;

    public decimal MainHours { get { return mainHours; } set { mainHours=value;} }
    public decimal SubHours { get { return subHours; }  set { subHours=value;}  }
    public int PrimaryContacts { get { return primaryContacts; }  set { primaryContacts=value;}  }
    public int TotalContacts { get { return totalContacts; }  set { totalContacts=value;}  }
    public int FPI { get { return fPI; } set { fPI = value; } }


	public ContactReq() 
	{
        totalContacts=0;    //DWR: Why this?
	}
}