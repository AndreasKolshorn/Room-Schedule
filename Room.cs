using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Room
/// </summary>
public class Room
{
    public int ColNum { get; set; }
    public int RoomID { get; set; }
    public string RoomName { get; set; }
    public string RoomColumn { get; set; } 
        
	public Room()	{	}

    public Room(int colNum, int roomID, string roomName, string roomColumn)
    {
        this.ColNum = colNum;
        this.RoomID = roomID;
        this.RoomName = roomName;
        this.RoomColumn = roomColumn;
    }


}