//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "Unit2.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm2 *Form2;
//---------------------------------------------------------------------------
__fastcall TForm2::TForm2(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------

void __fastcall TForm2::OnEditKeyPress(TObject *Sender, char &Key)
{
  if ((Key >= '0') && (Key <= '9')) {} // если цифра
  else if (Key == 8) {}
  else if ((Key == 'E' || Key == 'e') && Sender!=Edit1 && Sender!=Edit2) {}
  else if ((Key == '.' || Key == ',') && Sender!=Edit1 && Sender!=Edit2 && (((TEdit*)Sender)->Text.Pos(DecimalSeparator)==0 || ((TEdit*)Sender)->SelText.Pos(DecimalSeparator)!=0))
    Key = DecimalSeparator;
  else Key = 0; // не цифра
}

void __fastcall TForm2::Button2Click(TObject *Sender)
{
  int x = Edit1->Text.ToInt();
  int y = Edit2->Text.ToInt();
  if (x<2 || x>200 || y<2 || y>200)
  {
    ShowMessage("Количество ячеек должно быть от 2 до 200");
    ModalResult = mrNone;
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm2::StructureTypeRadioButtonClick(TObject *Sender)
{
  bool bN = (RadioButton3->Checked || RadioButton4->Checked || RadioButton5->Checked || RadioButton6->Checked || RadioButton7->Checked || RadioButton8->Checked);
  bool bG = (RadioButton2->Checked || RadioButton4->Checked || RadioButton5->Checked || RadioButton7->Checked || RadioButton8->Checked);
  bool bA = RadioButton5->Checked || RadioButton8->Checked;
  bool bDiag = (RadioButton6->Checked || RadioButton7->Checked || RadioButton8->Checked);

  Label5->Visible = bG; Edit5->Visible = bG;
  Label6->Visible = bG; Edit6->Visible = bG;
  Label4->Visible = bN; Edit4->Visible = bN;
  Label7->Visible = bA; Edit7->Visible = bA;
  Label8->Visible = bDiag; Edit8->Visible = bDiag;

}
//---------------------------------------------------------------------------

void __fastcall TForm2::FormActivate(TObject *Sender)
{
  StructureTypeRadioButtonClick(this);
}
//---------------------------------------------------------------------------





