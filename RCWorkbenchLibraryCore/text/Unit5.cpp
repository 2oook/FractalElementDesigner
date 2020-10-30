//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "Unit5.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm5 *Form5;

//---------------------------------------------------------------------------
__fastcall TForm5::TForm5(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------

void __fastcall TForm5::OnEditKeyPress(TObject *Sender, char &Key)
{
  if ((Key >= '0') && (Key <= '9')) {}
  else if (Key == 8) {}
  else if ((Key=='.' || Key==',') && Sender!=Edit5 && Sender!=Edit9 && (((TEdit*)Sender)->Text.Pos(DecimalSeparator)==0 || ((TEdit*)Sender)->SelText.Pos(DecimalSeparator)!=0))
    Key = DecimalSeparator;
  else Key = 0; // не цифра
}
//---------------------------------------------------------------------------

void __fastcall TForm5::CheckBox1Click(TObject *Sender)
{
  bool temp = CheckBox1->Checked;
  Edit2->Enabled = temp;
  Edit3->Enabled = temp;
  Edit4->Enabled = temp;
  Edit5->Enabled = temp;
  Label2->Enabled = temp;
  Label3->Enabled = temp;
  Label4->Enabled = temp;
  Label5->Enabled = temp;
}
//---------------------------------------------------------------------------
void __fastcall TForm5::CheckBox2Click(TObject *Sender)
{
  bool temp = CheckBox2->Checked;
  Edit6->Enabled = temp;
  Edit7->Enabled = temp;
  Edit8->Enabled = temp;
  Edit9->Enabled = temp;
  Label6->Enabled = temp;
  Label7->Enabled = temp;
  Label8->Enabled = temp;
  Label9->Enabled = temp;
}
//---------------------------------------------------------------------------

void __fastcall TForm5::CheckBox3Click(TObject *Sender)
{
  bool temp = CheckBox3->Checked;
  Edit13->Enabled = temp;
  Label13->Enabled = temp;
}
//---------------------------------------------------------------------------

void __fastcall TForm5::CheckBox4Click(TObject *Sender)
{
  bool temp = CheckBox4->Checked;
  Edit10->Enabled = temp;
  Edit11->Enabled = temp;
  Edit12->Enabled = temp;
  Edit14->Enabled = temp;
  Label10->Enabled = temp;
  Label11->Enabled = temp;
  Label12->Enabled = temp;
  Label14->Enabled = temp;
}
//---------------------------------------------------------------------------

void __fastcall TForm5::CheckBox5Click(TObject *Sender)
{
  bool temp = CheckBox5->Checked;
  Label15->Enabled = temp;
  Label16->Enabled = temp;
  Label17->Enabled = temp;
  Edit15->Enabled = temp;
  Edit16->Enabled = temp;
  Edit17->Enabled = temp;
}
//---------------------------------------------------------------------------

