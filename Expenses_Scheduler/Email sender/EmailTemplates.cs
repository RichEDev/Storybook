using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Specialized;

namespace Expenses_Scheduler
{
    public class EmailTemplates
    {
        
		public string PadIt(string textIn, int len){
			if(textIn.Length < len)
			{
				while(textIn.Length != len){
					textIn = string.Concat("0", textIn);
				}
			}
			return textIn;
		}
		
		public void SaveTemplate(int templateType, string templateName, string templateBody, string fileName){
			String strHeader = this.PadIt(templateType.ToString(), 4);
			
			strHeader = String.Concat(strHeader, this.PadIt(templateName.Length.ToString(), 10), templateName);
		
			strHeader = String.Concat(strHeader, this.PadIt(templateBody.Length.ToString(), 10), templateBody);
		
			if(System.IO.File.Exists(fileName) == true){
				System.IO.File.Delete(fileName);
			}

			StreamWriter x = new StreamWriter(fileName, true);
			x.Write(strHeader);
			x.Flush();
			x.Close();
			
		}

		
		public NameValueCollection ReadTemplate(string fileName){
			NameValueCollection output = new NameValueCollection();
			
			//Read in file
			StreamReader x = new StreamReader(fileName);
			string rawIn = x.ReadToEnd();
			x.Close();

			//Template Type
			int templateType = int.Parse(rawIn.Substring(0, 4));
			output.Add("templateType", templateType.ToString());

			//Template Title
			int titleOffset = int.Parse(rawIn.Substring(4, 10));
			string templateTitle = rawIn.Substring(14, titleOffset);
			output.Add("templateTitle", templateTitle);

			//Body
			string templateBody = rawIn.Substring(24 + titleOffset);
			output.Add("templateBody", templateBody);

			return output;
		}
    }
}
