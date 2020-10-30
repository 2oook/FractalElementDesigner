//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "Unit8.h"
#include "Unit3.h"
#include "Unit1.h"

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm8 *Form8;
extern int nPrecision, nDigits;
//---------------------------------------------------------------------------
__fastcall TForm8::TForm8(TComponent* Owner)
        : TForm(Owner)
{
}
//---------------------------------------------------------------------------

void __fastcall TForm8::OnEditKeyPress(TObject *Sender, char &Key)
{
  if ((Key >= '0') && (Key <= '9')) {} // если цифра
  else if (Key == 8) {}
  else if (Key=='-' && Sender!=Edit1 && Sender!=Edit2 && Sender!=Edit3 && ((TEdit*)Sender)->SelStart==0 && (((TEdit*)Sender)->Text.Pos(Key)==0 || ((TEdit*)Sender)->SelText.Pos(Key)!=0)) {} //минус можно поставить только в начало и только если минуса еще нет
  else if ((Key=='.' || Key==',') && Sender!=Edit3 && (((TEdit*)Sender)->Text.Pos(DecimalSeparator)==0 || ((TEdit*)Sender)->SelText.Pos(DecimalSeparator)!=0)) // зап€тую и точку замен€ем на разделитель целой и дробной части (настроено что это точка), разделитель можно поставить только если в строке он ниразу не встречаетс€
    Key = DecimalSeparator;
  else Key = 0; // не цифра
}
//---------------------------------------------------------------------------

void __fastcall TForm8::Button1Click(TObject *Sender)
{
  if (ValidateForm())
  {
    double dw, w, VMin, VMax, dVMin, dVMax;
    double Wmin = Edit1->Text.ToDouble();
    double Wmax = Edit2->Text.ToDouble();
    double VBeginMin = Edit4->Text.ToDouble();
    double VBeginMax = Edit5->Text.ToDouble();
    double VEndMin   = Edit6->Text.ToDouble();
    double VEndMax   = Edit7->Text.ToDouble();
    int n = Edit3->Text.ToInt();
    for (int i=Form3->StringGrid1->RowCount-2; i>0; i--)
      Form3->StringGrid1->Rows[i]->Clear();
    Form3->StringGrid1->RowCount = n+1;
    if (RadioButton1->Checked)
      dw = (Wmax-Wmin)/(n-1);
    else
      dw = (log10(Wmax/Wmin))/(n-1);

    dVMin = (VEndMin-VBeginMin)/(n-1);
    dVMax = (VEndMax-VBeginMax)/(n-1);

    for (int i=0; i<n; i++)
    {
      if (RadioButton1->Checked)
        w = Wmin+dw*i;
      else
        w = Wmin*pow(10.0, dw*i);

      VMin = VBeginMin+dVMin*i;
      VMax = VBeginMax+dVMax*i;

      Form3->StringGrid1->Cells[0][i+1]=FloatToStrF(w, ffFixed, nPrecision, nDigits);
      Form3->StringGrid1->Cells[1][i+1]=FloatToStrF(VMin, ffFixed, nPrecision, nDigits);
      Form3->StringGrid1->Cells[2][i+1]=FloatToStrF(VMax, ffFixed, nPrecision, nDigits);
    }

    Form1->Chart1->TopAxis->Logarithmic    = RadioButton2->Checked;
    Form1->Chart1->BottomAxis->Logarithmic = RadioButton2->Checked;
  }
}
//---------------------------------------------------------------------------

bool __fastcall TForm8::ValidateForm()
{
  if (Edit1->Text.Length()>0 && Edit2->Text.Length()>0 && Edit3->Text.Length()>0 && Edit4->Text.Length()>0 && Edit5->Text.Length()>0)
  {
    if (Edit1->Text.ToDouble()<=0.0)
    {
      ShowMessage("ћинимальна€ частота должна быть > 0");
      return false;
    }
    if (Edit2->Text.ToDouble()<=0.0)
    {
      ShowMessage("ћаксимальна€ частота должна быть > 0");
      return false;
    }
    if (Edit1->Text.ToDouble() >= Edit2->Text.ToDouble())
    {
      ShowMessage("ћаксимальна€ частота должна быть больше минимальной");
      return false;
    }
    if (Edit3->Text.ToIntDef(0)<2)
    {
      ShowMessage("ƒолжно быть минимум 2 точки");
      return false;
    }
    if (Edit4->Text.ToDouble() >= Edit5->Text.ToDouble())
    {
      ShowMessage("ћаксимальное значение должно быть больше минимального");
      return false;
    }
  }
  else
  {
    ShowMessage("≈сть незаполненные пол€");
    return false;
  }

  ModalResult = mrOk;
  return true;
}

