using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ProgramReqProgram
/// </summary>
public class ProgReqProgram
{
    private int bufferRow; private int BufferRow { get { return bufferRow; } }
    private int b10000; public int B10000 { get { return b10000; } }

    private int programReqColumnSetID;  public int ProgramReqColumnSetID { get { return programReqColumnSetID;}}
    private string programReqNote;      public string ProgramReqNote { get { return programReqNote;}}
//    private int programReqOrder;        public int ProgramReqOrder { get { return  programReqOrder;}}
    private int programsID;             public int ProgramsID { get { return programsID;}}
    private string programDescription;  public string ProgramDescription { get { return programDescription;}}
    private string programsCode;        public string ProgramsCode { get { return programsCode; } }
    private bool calculateColumnTotal; public bool CalculateColumnTotal { get { return calculateColumnTotal; } }
    private string expectedGradTerm; public string ExpectedGradTerm { get { return expectedGradTerm; } }

    private List<ProgReqSection> progReqSection = new List<ProgReqSection>();
    public List<ProgReqSection> Section { get { return this.progReqSection; } }

    private ProgReqTotalRow totalRequired = new ProgReqTotalRow();
    public ProgReqTotalRow TotalRequired { get { return this.totalRequired; } }

    // Program level subtotal containers
    private ProgReqTotalRow subTotalDeficit = new ProgReqTotalRow();
    public ProgReqTotalRow SubTotalDeficit { get { return this.subTotalDeficit; } }
  
    private ProgReqTotalRow subTotalTodate = new ProgReqTotalRow();
    public ProgReqTotalRow SubTotalTodate { get { return this.subTotalTodate; } }

 
	public ProgReqProgram(
        int bufferRow,
        int b10000,
        int programReqColumnSetID,
        string programReqNote,
         bool calculateColumnTotal,   
        //        int programReqOrder,
        int programsID,
        string programDescription,
        string programsCode,
        string expectedGradTerm
        )
	{
        this.bufferRow = bufferRow;
        this.b10000 = b10000;
        this.programReqColumnSetID = programReqColumnSetID;
        this.programReqNote = programReqNote;
        this.calculateColumnTotal = calculateColumnTotal;
  //      this.programReqOrder=programReqOrder;
        this.programsID=programsID;
        this.programDescription=programDescription;
        this.programsCode=programsCode;
        this.expectedGradTerm = expectedGradTerm;
    }
}

