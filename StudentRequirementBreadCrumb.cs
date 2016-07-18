using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for DoctorScheduleBreadCrumb
/// </summary>
public class StudentRequirementBreadCrumb 
{
    const string sOrangePoint = "<font color = orange>" + " > " + "</font>";
    private int userEvent;
    private string crumbBase="";           //     
    private string crumbBasePlus = "";
    private string selectedDoctorName = ""; // ddl selection
    private string errorMessage = "";
  
    public int UserEvent { get { return userEvent; } set { userEvent = value; } }
    public string Base { get; private set; }
    public string BasePlus { get; set; }
 
    // BreadCrum events 
    public enum eventID
    {
        ddl_DoctorList_SelectedIndexChanged,
        errorMessage,
        panelAdd,
        panelEdit,
        PanelSelect,
        defaultEvent
    }

    // BreadCrumb event accessors.
    public int zDLL_DoctorList_SelectedIndexChanged { get { return (int)eventID.ddl_DoctorList_SelectedIndexChanged; } }

    public int zErrorMessage { get { return (int)eventID.errorMessage; } }
    public int zPanelAdd { get { return (int)eventID.panelAdd; } }
    public int zPanelEdit { get { return (int)eventID.panelEdit; } }

    public int zPanelSelect { get { return (int)eventID.PanelSelect; } }

    public int zDefaultEvent { get { return (int)eventID.defaultEvent; } }

    public StudentRequirementBreadCrumb(string programAreaName) 
    {
        Base = "Student Requirement : Edit :" + programAreaName;

        selectedDoctorName = "";
        userEvent = (int)eventID.defaultEvent;
	}

    public void SetDoctorSelected(DoctorEditCore doctorEditCore, int userEvent)
    {
        selectedDoctorName = " " + doctorEditCore.LastName + ", " + doctorEditCore.FirstName;
        this.userEvent = userEvent;
    }

    public void SetUserEvent(int userEventID)
    {
        this.userEvent = userEventID;
    }

    
    public void SetErrorMessage(string errorMessage)
    {
        this.errorMessage = errorMessage;
        this.userEvent = (int) eventID.errorMessage;
    }

    public string GetMessage()
    {
        string msg = "";
        string doctorName = (selectedDoctorName.Length > 0 ? selectedDoctorName : "no_selectedDoctorName");
        string errorMsg = (errorMessage.Length > 0 ? errorMessage : "no_errorMessage");

        switch (UserEvent)
        {
            case (int)eventID.ddl_DoctorList_SelectedIndexChanged:
                msg = Base + doctorName; 
                break;
            case (int) eventID.PanelSelect:
                msg = Base ; 
                break;
            case (int) eventID.panelAdd:
                msg = Base + "<font color = orange><B>" + " EDIT " + "</B></font>";
                
                break;

            case (int) eventID.panelEdit:
//                msg = Base + selectedSlotStudentName + " Edit ";
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