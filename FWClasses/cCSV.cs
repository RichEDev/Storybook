using System;
using System.Data;
using System.Text;
using System.IO;
using System.Web;

namespace csvParser
{
	/// <summary>
	/// Summary description for cCSV.
	/// </summary>
	public class cCSV
	{
		private string delim = ",";
		private string tagAt = "\"";
		private bool hasHRow = false;

		public bool hasHeaderRow
		{
			get { return hasHRow; }
			set { hasHRow = value; }
		}

		public string DelimiterChar
		{
			get { return delim; }
			set { delim = value; }
		}

		public string TagChar
		{
			get { return tagAt; }
			set { tagAt = value; }
		}

		DataSet dsResult;
		int columnId;

		public cCSV()
		{

		}

		public DataSet CSVToDataset(string fileName)
		{
			int standardCount = -1;
			columnId = 0;
			dsResult = new DataSet();

			DataTable t = new DataTable();
			dsResult.Tables.Add(t);
			t.Dispose();
			t = null;
			System.IO.StreamReader freader = File.OpenText(fileName);
			StringBuilder strStack = new StringBuilder();
			
			bool inLiteral = false;
			bool isHeaderRow = hasHRow;

			string line;

			while ((line = freader.ReadLine()) != null)
			{
				DataRow r = dsResult.Tables[0].NewRow();
				columnId = 0;
				addCol("", ref r);

				for(int x = 0; x < line.Length; x++)
				{
					string currentChr = line.Substring(x, 1);
					if(currentChr == tagAt)
					{
						if(inLiteral == false)
						{
							inLiteral = true;
						}
						else
						{
							inLiteral = false;						
						}
					}
					else if(currentChr == delim)
					{
						if(inLiteral == false)
						{
							if(isHeaderRow == true)
							{
								addCol(strStack.ToString(),ref r, strStack.ToString());
								strStack.Remove(0, strStack.Length);
							}
							else
							{
								addCol(strStack.ToString(), ref r);
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

                    if (x == line.Length - 1)
                    {
                        if (inLiteral == true)
                        {
                            x = -1;

                            while ((line = freader.ReadLine()) != null)
                            {
                                strStack.Append("\r\n");
                                if(line.Length > 0)
                                    break;
                            }
                        }
                    }
				}
				
				if(isHeaderRow == true)
				{
					addCol(strStack.ToString(), ref r,strStack.ToString());
                    isHeaderRow = false;
				}
				else
				{
					addCol(strStack.ToString(), ref r);
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
						setCol("0", ref r, 0);
					}
					else if(rRow > standardCount)
					{
						setCol("1", ref r, 0);
					}
					else if (rRow < standardCount)
					{
						setCol("2", ref r, 0);
					}
				}

				strStack.Remove(0, strStack.Length);
				dsResult.Tables[0].Rows.Add(r);
			}

			freader.DiscardBufferedData();
			freader.Close();
			
			return dsResult;
		}

		private void addCol(string sVal, ref DataRow r)
		{
			if(dsResult.Tables[0].Columns.Count <= columnId)
			{
				DataColumn col = new DataColumn();
				dsResult.Tables[0].Columns.Add(col);
				col.Dispose();
				col = null;
			}
			r[columnId] = sVal;
			columnId++;
		}

		private void addCol(string sVal, ref DataRow r,string ColumnName)
		{
			if(dsResult.Tables[0].Columns.Count <= columnId)
			{
				DataColumn col = new DataColumn(ColumnName);
				dsResult.Tables[0].Columns.Add(col);
				col.Dispose();
				col = null;
			}
			r[columnId] = sVal;
			columnId++;
		}

		private void setCol(string sVal, ref DataRow r, int colCount)
		{
			r[colCount] = sVal;
		}
		
		public void WriteCSVTable(DataTable dtin, bool headers, string filename)
		{
			string datOut = DataTableToCSV(dtin, headers);
			FileStream fs = new FileStream(filename, FileMode.Create);

			StreamWriter sw = new StreamWriter(fs);

			sw.Write(datOut.ToCharArray());

			sw.Flush();

			sw.Close();

			fs.Close();
		
		}
		
		public string DataTableToCSV(DataTable dsin, bool headers)
		{
			StringBuilder dataOut = new StringBuilder();

			if(headers == true){
				for(int i = 0; i < dsin.Columns.Count; i++)
				{
					dataOut.Append(dsin.Columns[i].ColumnName);		
					if(i < (dsin.Columns.Count - 1))
					{
						dataOut.Append(delim);
					}
				}
				dataOut.Append("\r\n");
			}

			foreach(DataRow dRow in dsin.Rows)
			{
				for(int i = 0; i < dsin.Columns.Count; i++)
				{
					string tmpOut = dRow[i].ToString();

					if(tmpOut.IndexOf(delim) >= 0)
					{
						dataOut.Append(tagAt + tmpOut + tagAt);
					}
					else
					{
						dataOut.Append(tmpOut);
					}

					if(i < (dsin.Columns.Count - 1))
					{
						dataOut.Append(delim);
					}
				}

				dataOut.Append("\r\n");
			}
			
			return dataOut.ToString();
		}

		
	}
}
