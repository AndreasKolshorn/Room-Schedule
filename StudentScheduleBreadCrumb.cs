using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for StudentScheduleBreadCrumb
/// </summary>
public class StudentScheduleBreadCrumb 
{
    const string sOrangePoint = "<font color = orange>" + " > " + "</font>";
    private int userEvent;
    private string crumbBase="";           //     
    private string crumbBasePlus = "";
    private string selectedStudentName = ""; // ddl selection
    private string selectedCourseName = "";  // tv selection 
    private string selectedCell = "";        // cell that one is editing or adding to.
    private string errorMessage = "";
    private string selectedRoom = "";
    private string selectedTime = "";
    private string selectedSlotStudentName = "";

    private string slotSelectedName = "";

    public int UserEvent { get { return userEvent; } set { userEvent = value; } }
    public string Base { get; private set; }
    public string BasePlus { get; set; }
    public string StudentSelected { get; set; }
    public string SlotSelected { get; set; }
    //public const int tt;

    public struct cellCoordinate
    {
        string dayTime;
        string roomName;
    }

    // BreadCrum events 
    public enum eventID
    {
        ddl_ClinicStudents_SelectedIndexChanged,
        tv_ProgramShiftsNodeChanged,
        errorMessage,
        panelAdd,
        panelEdit,
        PanelSelect,
        defaultEvent
    }

    // BreadCrumb event accessors.
    public int zDLL_ClinicStudents_SelectedIndexChanged { get { return (int) eventID.ddl_ClinicStudents_SelectedIndexChanged; } }
    public int zTV_ProgramShiftsNodeChanged { get { return (int) eventID.tv_ProgramShiftsNodeChanged; } }
    public int zErrorMessage { get { return (int)eventID.errorMessage; } }
    public int zPanelAdd { get { return (int)eventID.panelAdd; } }
    public int zPanelEdit { get { return (int)eventID.panelEdit; } }
    public int zPanelSelect { get { return (int)eventID.PanelSelect; } }
    public int zDefaultEvent { get { return (int)eventID.defaultEvent; } }

	public StudentScheduleBreadCrumb(string programAreaName) 
    {
        Base = "Student : Schedule : " + programAreaName ;

        selectedStudentName = slotSelectedName = "";
        userEvent = (int)eventID.defaultEvent;
	}

    public void SetSelectedCell (string room, string daytime, int userEvent)
    {
        this.selectedRoom = room;
        this.selectedTime = daytime;
        this.userEvent = userEvent;
    }

    public void SetSelectedSlot(ShiftSlot ss)
    {
        this.selectedSlotStudentName =  ss.StudentName;
    }


    public void SetStudentSelected(StudentBasic student, int userEvent)
    {
        selectedStudentName =   student.Lastname+ " " +student.Firstname;
        selectedCourseName = "";
        this.userEvent = userEvent;
    }


    public void SetUserEvent(int userEventID)
    {
        this.userEvent = userEventID;
    }

    public void SetCourseSelected(string course, int userEvent)
    {
        selectedCourseName = StripHtml(course, false);
        this.userEvent = userEvent;
    }
    
    public void SetErrorMessage(string errorMessage)
    {
        this.errorMessage = errorMessage;
        this.userEvent = (int) eventID.errorMessage;
    }

    public string GetMessage()
    {
        string msg = "";
        string studentName = (selectedStudentName.Length > 0 ? " : " + selectedStudentName : ""); //  "no_selectedStudentName");
        string courseName = (selectedCourseName.Length > 0 ? " : " + selectedCourseName : ""); // "no_selectedCourseName");
        string errorMsg = (errorMessage.Length > 0 ? errorMessage : ""); //"no_errorMessage");

        switch (UserEvent)
        {
            case (int) eventID.ddl_ClinicStudents_SelectedIndexChanged:
                msg =Base + studentName; 
                break;
            case (int) eventID.PanelSelect:
                msg = Base   +  studentName + courseName;; 
                break;
            case (int) eventID.tv_ProgramShiftsNodeChanged:
                msg = Base + studentName  + courseName;
                break; 
            case (int) eventID.panelAdd:
                msg = Base + " : <font color = orange><B>" + " A d d  " + "</B></font> : "
                    + studentName + " " + courseName + " "
                    + selectedCell;
                break;

            case (int) eventID.panelEdit:
                msg = Base + " : <font color = orange><B>" + " E d i t " + "</B></font> : "  + selectedSlotStudentName + courseName + " ";
                break;
            case (int) eventID.errorMessage:
                msg = errorMsg;
                break;
            case (int) eventID.defaultEvent:
                msg = Base;
                break;
            default:
                msg = "Really Undefined event";
                break;
        }

        return msg;

    }

    private string StripHtml(string html, bool allowHarmlessTags)
    {
        if (html == null || html == string.Empty)
            return string.Empty;
        if (allowHarmlessTags)
            return System.Text.RegularExpressions.Regex.Replace(html, "", string.Empty);

        return System.Text.RegularExpressions.Regex.Replace(html, "<[^>]*>", string.Empty);
    }


}