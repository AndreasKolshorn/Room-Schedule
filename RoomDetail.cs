using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for RoomDetail
/// </summary>
public class RoomDetail
{
    // Standard error properties
    private string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    private int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    // Basic properties
    private int rowNum = 0;   // alias for Day/RoomDetail. Ties object to grid cell.
    
    private int shiftID;             // StudentUId assigned to this object by constructor.
 
    private int dayTypeID;
    private int shiftTimeID;

    private string shiftName;
    private string dayName;
    
	public RoomDetail()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public void SetRoomDetail(int aa)
    {
    
    }


}