using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ProgReqTotalRow
/// </summary>
public class ProgReqTotalRow
{
    private string rowHeader; public string RowHeader { get { return rowHeader; } set { rowHeader = value; } }
    private string rowNote; public string RowNote { get { return rowNote; } set { rowNote = value; } }

    private List<ProgReqItem> progReqItem = new List<ProgReqItem>();
    public List<ProgReqItem> Item { get { return this.progReqItem; } }

    public ProgReqTotalRow() { }

    public ProgReqTotalRow(
        string rowHeader,
        string rowNote
        )
	{
        this.rowHeader = rowHeader;
        this.rowNote= rowNote;
    }
}





