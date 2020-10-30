//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "Unit6.h"
#include "Scheme.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm6 *Form6;
//---------------------------------------------------------------------------
__fastcall TForm6::TForm6(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------

void __fastcall TForm6::Button1Click(TObject *Sender)
{
  try
  {
    if (Edit1->Text.ToDouble() < 0.0 || Edit2->Text.ToDouble() < 0.0)
    {
      ShowMessage("Неверные значения");
      ModalResult = mrNone;
    }
  }
  catch (EConvertError &e)
  {
    ShowMessage("Неверные значения");
    ModalResult = mrNone;
  }
}
//---------------------------------------------------------------------------

void UpdateControls(TComboBox *CB, TLabel *LCharacteristic, TComboBox *CBCharacteristic, TImage *ImageSch, TLabel *LK, TEdit *EK, TLabel *LL, TEdit *EL, TLabel *LKP, TEdit *EFormula)
{
  int index = CB->ItemIndex;
  switch (index)
  {
    case  0: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "2";} break;
    case  1: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "2";} break;
    case  2: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "2";} break;
    case  3: {LK->Visible= true; EK->Visible= true; LL->Visible= true; EL->Visible= true; LKP->Caption = "2";} break;
    case  4: {LK->Visible=false; EK->Visible=false; LL->Visible= true; EL->Visible= true; LKP->Caption = "2";} break;
    case  5: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "4";} break;
    case  6: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "4";} break;
    case  7: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "4";} break;
    case  8: {LK->Visible= true; EK->Visible= true; LL->Visible= true; EL->Visible= true; LKP->Caption = "3";} break;
    case  9: {LK->Visible= true; EK->Visible= true; LL->Visible= true; EL->Visible= true; LKP->Caption = "4";} break;
    case 10: {LK->Visible= true; EK->Visible= true; LL->Visible= true; EL->Visible= true; LKP->Caption = "4";} break;
    case 11: {LK->Visible= true; EK->Visible= true; LL->Visible= true; EL->Visible= true; LKP->Caption = "4";} break;
    case 12: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "4";} break;
    case 13: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "4";} break;
    case 14: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "4";} break;
    case 15: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "4";} break;
    case 16: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "4";} break;
    case 17: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "4";} break;
    case 18: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "4";} break;
    case 19: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "6";} break;
    case 20: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "8";} break;
    case 21: {LK->Visible=false; EK->Visible=false; LL->Visible=false; EL->Visible=false; LKP->Caption = "*";} break;
  }
  EFormula->Visible = (index == 21);
  CBCharacteristic->Visible = (index != 21);
  if (LCharacteristic != NULL)
  {
    LCharacteristic->Visible = CBCharacteristic->Visible;
  }
  CScheme::ShowScheme(index, ImageSch);
}
//---------------------------------------------------------------------------

void __fastcall TForm6::ComboBox1Change(TObject *Sender)
{
  UpdateControls(ComboBox1, Label1, ComboBox2, Image1, Label2, Edit1, Label3, Edit2, Label5, Edit3);
}
//---------------------------------------------------------------------------

void __fastcall TForm6::FormActivate(TObject *Sender)
{
  CScheme::ShowScheme(ComboBox1->ItemIndex, Image1);
}
//---------------------------------------------------------------------------

void __fastcall TForm6::OnEditKeyPress(TObject *Sender, char &Key)
{
  if ((Key >= '0') && (Key <= '9')) {} // если цифра
  else if (Key == 8) {}
  else if ((Key == '.' || Key == ',') && (((TEdit*)Sender)->Text.Pos(DecimalSeparator)==0 || ((TEdit*)Sender)->SelText.Pos(DecimalSeparator)!=0))
    Key = DecimalSeparator;
  else Key = 0; // не цифра
}
