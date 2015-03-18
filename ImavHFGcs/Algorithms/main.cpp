#include "stdafx.h"
#include "qrcode.h"

using namespace System;
using namespace System::IO;
using namespace System::Xml;
using namespace System::Collections;
using namespace HighFlyers::Algorithms;

int main()
{
	XmlDocument^ doc = gcnew XmlDocument();
	try
	{
		doc->Load("data.xml");
	}
	catch(Exception^ ex)
	{
		Console::WriteLine("couldn't load data.xml");
		return -1;
	}
	XmlNode^ root = doc->DocumentElement;
	IEnumerator^ it = root->GetEnumerator();
	XmlNode^ node;
	int total = 0;
	int passed = 0;
	while(it->MoveNext())
	{
		node = dynamic_cast<XmlNode^>(it->Current);
		
		for ( int i = 0; i < node->ChildNodes->Count-1; i++ )
		{
		  XmlNode^ inner = (node->ChildNodes[ i ]);
         //Console::WriteLine( inner->InnerText );
		  String^ name = inner->InnerText;
		  String^ dots = "..";
		  String^ cat = "test_data_quadro";
		 // name = "A.bmp";
		  name = name->Replace("\"", "");
		  String^ path = Path::Combine(dots,cat,name);
		  array<wchar_t>^ inv = Path::GetInvalidFileNameChars();
		  array<Byte>^ arr = File::ReadAllBytes(path);
		  QRCode^ alg;
		  pin_ptr<Byte> ptr = &arr[0];
		  String^ ans = nullptr;
		  ans = alg->RecognizeQrCode(ptr, 768, 576);
		  if (ans == "QRCODE Missed")
		  {
				Console::WriteLine("Passed on " + path);
				passed++;
		  }
		  else 
		  {
			  Console::WriteLine("Failed on " + path + Environment::NewLine + "Decoded was: " + ans);
		  }
		  total++;
		}
		/*String^ name = (String^)node->ChildNodes[0]->InnerText;
		Console::WriteLine(node->OuterXml);*/
	}
	if (total != passed)
	{
		Console::ForegroundColor = ConsoleColor::Red;
		Console::WriteLine("Not all test passed!");
		Console::WriteLine("{0}/{1}", passed, total);
	}
	else
	{
		Console::ForegroundColor = ConsoleColor::Green;
		Console::WriteLine("All test passed!");
	}
	Console::ReadLine();
	return 0;
}