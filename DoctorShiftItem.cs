using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DoctorShiftItem
/// </summary>
public class DoctorShiftItem
{
    private Int32 scheduleID;
    private string dayName;
    private string shiftName;
    private string roomName;

    public Int32 ScheduleID { get { return scheduleID; } }
    public string DayName { get { return dayName; } }

    public string ShiftName { get { return shiftName; } }
    public string RoomName { get { return roomName; } }

	public DoctorShiftItem()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public DoctorShiftItem(Int32 scheduleID, string dayName, string shiftName, string roomName)
    {
        this.scheduleID = scheduleID;
        this.dayName = dayName;
        this.shiftName = shiftName;
        this.roomName = roomName;
    }

}