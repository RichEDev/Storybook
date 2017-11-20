using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
	[Serializable()]
	public abstract class cParamsBase
	{
		protected SortedList<string, cParam> ParamList;
		protected cFWSettings cFWS;
		protected UserInfo cUInfo;
		protected int curLocation;
		protected int nAccountId;
		protected string connectionstring;

		public cParamsBase(int accountid, int locationid)
		{
			nAccountId = accountid;
			curLocation = locationid;
		}

		#region properties
		public cParamsBase getBase
		{
			get
			{
				return this;
			}
		}
		public int Count
		{
			get { return ParamList.Count; }
		}
		#endregion

		public cParam GetParamByName(string param_name)
		{
			cParam retParam;
			if (ParamList.ContainsKey(param_name))
			{
				retParam = (cParam)ParamList[param_name];
			}
			else
			{
				retParam = new cParam("<Unknown>", "<Not Found>", false);
			}

			return retParam;
		}

		public ArrayList GetParamList()
		{
			ArrayList retList = new ArrayList();

			foreach (KeyValuePair<string, cParam> item in ParamList)
			{
				cParam param = (cParam)item.Value;
				retList.Add(param);
			}

			return retList;
		}

		public bool ParamExists(string param_name)
		{
			return ParamList.ContainsKey(param_name);
		}

		public SortedList GetParamSortedList()
		{
			SortedList retList = new SortedList();

			foreach (KeyValuePair<string, cParam> item in ParamList)
			{
				cParam parm = item.Value;
				retList.Add(parm.ParameterName, parm.ParameterValue);
			}

			return retList;
		}

		public void SetParameterValue(string param_name, string param_value)
		{
			try
			{
				cFWDBConnection db = new cFWDBConnection();
				db.DBOpen(cFWS, false);

				db.SetFieldValue("Value", param_value, "S", true);

				if (ParamList.ContainsKey(param_name))
				{
					cParam param = ParamList[param_name];
					param.ParameterValue = param_value;

					db.FWDb("A", "fwparams", "Param", param_name, "Location Id", curLocation, "", "", "", "", "", "", "", "");
				}
				else
				{
					cParam param = new cParam(param_name, param_value, true);
					ParamList.Add(param_name, param);

					// add the parameter as it doesn't exist
					db.SetFieldValue("Param", param_name, "S", false);
					db.SetFieldValue("Location Id", curLocation, "N", false);
					db.SetFieldValue("Editable", 1, "N", false);
					db.FWDb("W", "fwparams", "", "", "", "", "", "", "", "", "", "", "", "");
				}
				db.DBClose();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("cParam:SetParameterValue:" + ex.Message);
			}
		}
	}
}
