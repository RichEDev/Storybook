using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
	#region cRechargeSetting
	/// <summary>
	/// cRechargeSetting class
	/// </summary>
    [Serializable]
	public class cRechargeSetting
	{
		private string sReferenceAs;
		public string ReferenceAs
		{
			get { return sReferenceAs; }
			set { sReferenceAs = value; }
		}
		private string sStaffRepAs;
		public string StaffRepAs
		{
			get { return sStaffRepAs; }
			set { sStaffRepAs = value; }
		}
		private int nRechargePeriod;
		public int RechargePeriod
		{
			get { return nRechargePeriod; }
			set { nRechargePeriod = value; }
		}
		private int nFinYearCommence;
		public int FinYearCommence
		{
			get { return nFinYearCommence; }
			set { nFinYearCommence = value; }
		}
		private int nCP_Delete_Action;
		public int CP_Delete_Action
		{
			get { return nCP_Delete_Action; }
			set { nCP_Delete_Action = value; }
		}

		public cRechargeSetting(string refAs, string staffRepAs, int rechargePeriod, int financialYearCommence, int cpDeleteAction)
		{
			sReferenceAs = refAs;
			sStaffRepAs = staffRepAs;
			nRechargePeriod = rechargePeriod;
			nFinYearCommence = financialYearCommence;
			nCP_Delete_Action = cpDeleteAction;
		}
	}
	#endregion
}
