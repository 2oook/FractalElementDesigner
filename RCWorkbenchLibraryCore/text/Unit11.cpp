//---------------------------------------------------------------------------

#include <vcl.h>
#include <filectrl.hpp>
#pragma hdrstop

#include "Unit11.h"

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm11 *Form11;
//---------------------------------------------------------------------------
__fastcall TForm11::TForm11(TComponent* Owner)
  : TForm(Owner)
{
  Edit3->Text = ExtractFileDir(Application->ExeName);
}
//---------------------------------------------------------------------------

void __fastcall TForm11::Button3Click(TObject *Sender)
{
  SaveDialog1->Execute();
  if (SaveDialog1->FileName.Length())
    Edit2->Text = SaveDialog1->FileName;  
}
//---------------------------------------------------------------------------

void __fastcall TForm11::Button4Click(TObject *Sender)
{
  const AnsiString capt = "Выберите папку для сохранения структур";
  const WideString root = "";
  AnsiString Dir;
  if (SelectDirectory(capt, root, Dir))
    Edit3->Text = Dir;
}
//---------------------------------------------------------------------------

void __fastcall TForm11::Button2Click(TObject *Sender)
{
  Close();
}
//---------------------------------------------------------------------------




