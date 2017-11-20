// xml.h
using namespace System;
using namespace System::Xml;
using namespace System::Xml::Serialization;
using namespace System::IO;

namespace FWCommon
{
	public __gc class xml
	{
		public :
		//Save object using an XML reader
		void SaveObj(String *sPath, Type *obType, Object *dataToSave){
			XmlSerializer *XSS = new XmlSerializer(obType);
			StreamWriter *SWriter = new StreamWriter(sPath, false);		
			XSS->Serialize(SWriter, dataToSave);
			SWriter->Close();
		};

		//Read object using an XML reader
		Object* ReadObject(String *sPath, Type *obType){
			XmlSerializer *XSS = new XmlSerializer(obType);
			FileStream *sReader = new FileStream(sPath, FileMode::Open);
			Object* toReturn = XSS->Deserialize(sReader);
			sReader->Close();
			return toReturn;
		};

	};
}
