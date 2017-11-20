// EmailTemplates.h
using namespace System;
using namespace System::IO;
using namespace System::Collections;
using namespace System::Collections::Specialized;

namespace FWCommon
{
	public __gc class EmailTemplates
	{
		public :
		
		String *PadIt(String *textIn, int len){
			if(textIn->Length < len)
			{
				while(textIn->Length != len){
					textIn = String::Concat("0", textIn);
				}
			}
			return textIn;
		}
		
		void SaveTemplate(int templateType, String *templateName, String *templateBody, String *fileName){
			String *strHeader = this->PadIt(templateType.ToString(), 4);
			
			strHeader = String::Concat(strHeader, this->PadIt(templateName->Length.ToString(), 10), templateName);
		
			strHeader = String::Concat(strHeader, this->PadIt(templateBody->Length.ToString(), 10), templateBody);
		
			if(System::IO::File::Exists(fileName) == true){
				System::IO::File::Delete(fileName);
			}

			StreamWriter *x = new StreamWriter(fileName, true);
			x->Write(strHeader);
			x->Flush();
			x->Close();
			
		};

		
		NameValueCollection *ReadTemplate(String *fileName){
			NameValueCollection *output = new NameValueCollection();
			
			//Read in file
			StreamReader *x = new StreamReader(fileName);
			String *rawIn = x->ReadToEnd();
			x->Close();

			//Template Type
			Int32 templateType = Int32::Parse(rawIn->Substring(0, 4));
			output->Add("templateType", templateType.ToString());

			//Template Title
			Int32 titleOffset = Int32::Parse(rawIn->Substring(4, 10));
			String *templateTitle = rawIn->Substring(14, titleOffset);
			output->Add("templateTitle", templateTitle);

			//Body
			String *templateBody = rawIn->Substring(24 + titleOffset);
			output->Add("templateBody", templateBody);

			return output;
		};

	};
}
