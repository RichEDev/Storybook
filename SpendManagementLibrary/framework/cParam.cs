using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
	public class cParam
	{
		private string sParamName;
		private string sParamValue;
		private bool bIsEditable;

		public string ParameterName
		{
			get { return sParamName; }
		}

		public string ParameterValue
		{
			get { return sParamValue; }
			set { sParamValue = value; }
		}

		public bool IsEditable
		{
			get { return bIsEditable; }
		}

		public cParam(string param_name, string param_value, bool iseditable)
		{
			sParamName = param_name;
			sParamValue = param_value;
			bIsEditable = iseditable;
		}
	}
}
