using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ProgReqSection
/// </summary>
public class ProgReqSection
{
        private int parentIndex; public int ParentIndex { get { return parentIndex; }} // set { parentIndex = value; } }
        private int bufferRow; public int BufferRow { get { return bufferRow; } }
        private int b100; public int B100 { get { return b100; } }
        private int courseTypeID; public int CourseTypeID { get { return courseTypeID; } }
        private string courseTypeName; public string CourseTypeName { get { return courseTypeName; } }
        private int classRoleTypeID; public int ClassRoleTypeID { get { return classRoleTypeID; } }
        private string classRoleTypeName; public string ClassRoleTypeName { get { return classRoleTypeName; } }
        private int enrollmentTypeID; public int EnrollmentTypeID { get { return enrollmentTypeID; } }
        private string enrollmentTypeName; public string EnrollmentTypeName { get { return enrollmentTypeName; } }
        private int shiftSlotTypeID; public int ShiftSlotTypeID { get { return shiftSlotTypeID; } }
        private string shiftSlotTypeName; public string ShiftSlotTypeName { get { return shiftSlotTypeName; } }

        private int standardColCnt; public int StandardColCnt { get { return standardColCnt; } set { standardColCnt = value; } }

        private List<ProgReqRow> progReqRow = new List<ProgReqRow>();
        public List<ProgReqRow> Row { get { return this.progReqRow; } }

        private ProgReqTotalRow subTotalInSection = new ProgReqTotalRow();
        public ProgReqTotalRow SubTotalInSection { get { return this.subTotalInSection; } }

        public ProgReqSection() { }

	    public ProgReqSection(
            int parentIndex,
            int bufferRow,
            int b100,
            int courseTypeID,
            string courseTypeName,
            int classRoleTypeID,
            string classRoleTypeName,
            int enrollmentTypeID,
            string enrollmentTypeName,
            int shiftSlotTypeID,
            string shiftSlotTypeName
            )
	    {
            this.parentIndex = parentIndex;
            this.bufferRow = bufferRow;
            this.b100=b100;
            this.courseTypeID=courseTypeID;
            this.courseTypeName=courseTypeName;
            this.classRoleTypeID=classRoleTypeID;
            this.classRoleTypeName=classRoleTypeName;
            this.enrollmentTypeID=enrollmentTypeID;
            this.enrollmentTypeName=enrollmentTypeName;
            this.shiftSlotTypeID=shiftSlotTypeID;
            this.shiftSlotTypeName=shiftSlotTypeName;
	    }
}

