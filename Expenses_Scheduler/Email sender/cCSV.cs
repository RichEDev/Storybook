using System;
using System.Data;
using System.Text;
using System.IO;
using System.Web;

namespace Expenses_Scheduler
{
	/// <summary>
	/// Summary description for cCSV.
	/// </summary>
	public class cCSV
	{
		private string _delim = ",";
		private string _tagAt = "\"";
		private bool _hasHRow;

		public bool HasHeaderRow
		{
			get { return _hasHRow; }
			set { _hasHRow = value; }
		}

		public string DelimiterChar
		{
			get { return _delim; }
			set { _delim = value; }
		}

		public string TagChar
		{
			get { return _tagAt; }
			set { _tagAt = value; }
		}

		DataSet _dsResult;
		int _columnId;

		public cCSV()
		{

		}

		public DataSet CSVToDataset(string fileName)
		{
			int standardCount = -1;
			_columnId = 0;
			_dsResult = new DataSet();

			DataTable t = new DataTable();
			_dsResult.Tables.Add(t);
			t.Dispose();
			t = null;
			StreamReader freader = File.OpenText(fileName);
			StringBuilder strStack = new StringBuilder();
			
			bool inLiteral = false;
			bool isHeaderRow = _hasHRow;

			string line;

			while ((line = freader.ReadLine()) != null)
			{
				DataRow r = _dsResult.Tables[0].NewRow();
				_columnId = 0;
				AddCol("", ref r);

				for(int x = 0; x < line.Length; x++)
				{
					string currentChr = line.Substring(x, 1);
					if(currentChr == _tagAt)
					{
						inLiteral = inLiteral == false;
					}
					else if(currentChr == _delim)
					{
						if(inLiteral == false)
						{
							if(isHeaderRow)
							{
								AddCol(strStack.ToString(),ref r, strStack.ToString());
								strStack.Remove(0, strStack.Length);
							}
							else
							{
								AddCol(strStack.ToString(), ref r);
								strStack.Remove(0, strStack.Length);
							}
						}
						else
						{
							strStack.Append(currentChr);
						}
					}
					else
					{
						strStack.Append(currentChr);
					}

				    if(x != line.Length - 1) continue;

				    if(!inLiteral) continue;

				    x = -1;
				    line = freader.ReadLine();
				    strStack.Append("\r\n");
				}
				
				if(isHeaderRow)
				{
					AddCol(strStack.ToString(), ref r,strStack.ToString());
				}
				else
				{
					AddCol(strStack.ToString(), ref r);
				}

				if(standardCount == -1)
				{
					standardCount = r.ItemArray.GetLength(0);
				}
				else
				{
					int rRow = r.ItemArray.GetLength(0);
					if(rRow == standardCount)
					{
						SetCol("0", ref r, 0);
					}
					else if(rRow > standardCount)
					{
						SetCol("1", ref r, 0);
					}
					else if (rRow < standardCount)
					{
						SetCol("2", ref r, 0);
					}
				}

				strStack.Remove(0, strStack.Length);
				_dsResult.Tables[0].Rows.Add(r);
			}

			freader.DiscardBufferedData();
			freader.Close();
			
			return _dsResult;
		}

		private void AddCol(string sVal, ref DataRow r)
		{
			if(_dsResult.Tables[0].Columns.Count <= _columnId)
			{
				DataColumn col = new DataColumn();
				_dsResult.Tables[0].Columns.Add(col);
				col.Dispose();
				col = null;
			}
			r[_columnId] = sVal;
			_columnId++;
		}

		private void AddCol(string sVal, ref DataRow r,string columnName)
		{
			if(_dsResult.Tables[0].Columns.Count <= _columnId)
			{
				DataColumn col = new DataColumn(columnName);
				_dsResult.Tables[0].Columns.Add(col);
				col.Dispose();
				col = null;
			}
			r[_columnId] = sVal;
			_columnId++;
		}

		private static void SetCol(string sVal, ref DataRow r, int colCount)
		{
			r[colCount] = sVal;
		}
		
		public void WriteCsvTable(DataTable dtin, bool headers, string filename)
		{
			string datOut = DataTableToCsv(dtin, headers);
			FileStream fs = new FileStream(filename, FileMode.Create);

			StreamWriter sw = new StreamWriter(fs);

			sw.Write(datOut.ToCharArray());

			sw.Flush();

			sw.Close();

			fs.Close();
		}
		
		public string DataTableToCsv(DataTable dsin, bool headers)
		{
			StringBuilder dataOut = new StringBuilder();

			if(headers){
				for(int i = 0; i < dsin.Columns.Count; i++)
				{
					dataOut.Append(dsin.Columns[i].ColumnName);		
					if(i < (dsin.Columns.Count - 1))
					{
						dataOut.Append(_delim);
					}
				}
				dataOut.Append("\r\n");
			}

			foreach(DataRow dRow in dsin.Rows)
			{
				for(int i = 0; i < dsin.Columns.Count; i++)
				{
					string tmpOut = dRow[i].ToString();

					if(tmpOut.IndexOf(_delim) >= 0)
					{
						dataOut.Append(_tagAt + tmpOut + _tagAt);
					}
					else
					{
						dataOut.Append(tmpOut);
					}

					if(i < (dsin.Columns.Count - 1))
					{
						dataOut.Append(_delim);
					}
				}

				dataOut.Append("\r\n");
			}
			
			return dataOut.ToString();
		}
	}
}
