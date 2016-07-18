using System;
using System.Collections.Generic;
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

/// <summary>
/// Summary description for TermCalendarItem
/// </summary>
public class TermCalendarItem
{
    // Standard error properties
    public string errorMessage = "";   // Recent message from SQL displayed in orange UI warning bar. 
    public int errorCode = 0;          // Messages recent error code (OK=0, ASPX=-1, SQL=-10)

    // Basic properties
    public int TermCalendarID;
    public string Term;
    public string TermText;

	public TermCalendarItem()
	{
	}

    public TermCalendarItem(int termCalendarID, string term, string termText)
    {
        this.TermCalendarID = termCalendarID;
        this.Term = term;
        this.TermText = termText;
    }


    public bool SQL_GetTermsAfter(List<TermCalendarItem> termCalendarItem, int afterTermCalendarID)
    {
        string connStr = ConfigurationManager.ConnectionStrings["ClinicScheduleConnString"].ToString();
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();

        try
        {
            connection.Open();
            command.Connection = connection;

            // Build and execute command string
            command.CommandText = "CSSP_GetTermsAfter";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@TermCalendarId", SqlDbType.Int).Value = afterTermCalendarID;
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                TermCalendarID = Convert.ToInt32(reader["termcalendarid"]);
                Term = reader["term"] as string;
                TermText = reader["textterm"] as string;

                termCalendarItem.Add(new TermCalendarItem(TermCalendarID, Term, TermText));
            }
        }

        catch (Exception ex)
        {
            errorMessage = ex.Message.ToString();
            termCalendarItem.Add(new TermCalendarItem(-1, "-1", "Exception"));
            return false;
        }

        finally
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
            command.Dispose();
        }

        return true;
    }




}