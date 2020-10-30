//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "Unit4.h"
#include "Unit1.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm4 *Form4;

//---------------------------------------------------------------------------
__fastcall TForm4::TForm4(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------

void __fastcall TForm4::Button1Click(TObject *Sender)
{
  if (MaskEdit1->Text.ToDouble()>1 || MaskEdit2->Text.ToDouble()>1 || MaskEdit3->Text.ToDouble()>1 || MaskEdit4->Text.ToDouble()>1 || MaskEdit5->Text.ToDouble()>1 || MaskEdit6->Text.ToDouble()>1 || MaskEdit11->Text.ToDouble()>1 || MaskEdit12->Text.ToDouble()>1)
  {
    ShowMessage("Не корректно заданы вероятности, повторите ввод");
    ModalResult=mrNone;
  }
  if (MaskEdit7->Text.ToInt()>Form1->Structure1->Width()-2 || MaskEdit8->Text.ToInt()>Form1->Structure1->Height()-2 || MaskEdit9->Text.ToInt()>Form1->Structure1->Width()-2 || MaskEdit10->Text.ToInt()>Form1->Structure1->Height()-2)
  {
    ShowMessage("Не корректно заданы площади мутации и скрещивания, повторите ввод");
    ModalResult=mrNone;
  }
}
//---------------------------------------------------------------------------


