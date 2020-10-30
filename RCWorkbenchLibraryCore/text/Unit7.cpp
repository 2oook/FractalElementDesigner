//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "Unit7.h"
#include "Unit3.h"
#include "Unit1.h"

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm7 *Form7;
extern int nPrecision, nDigits;
//---------------------------------------------------------------------------
__fastcall TForm7::TForm7(TComponent* Owner)
        : TForm(Owner)
{
}
//---------------------------------------------------------------------------

void HorizontalTicks(TCanvas *can, int x, int y, AnsiString s)
{
  can->MoveTo(x, y-4);
  can->LineTo(x, y+5);
  can->TextOutA(x - can->TextWidth(s)/2, y + can->TextHeight(s)/2, s);
}

void VerticalTicks(TCanvas *can, int x, int y, AnsiString s)
{
  can->MoveTo(x-4, y);
  can->LineTo(x+5, y);
  can->TextOutA(x - can->TextWidth(s)-7, y - can->TextHeight(s)/2, s);
}

void MyRectangle(TCanvas *can, int x1, int y1, int x2, int y2, bool open)
{
//  TColor b = can->Brush->Color;
//  TColor p = can->Pen->Color;
  can->Pen->Color = clBlack;
  can->Brush->Color = clBtnFace;
  can->Rectangle(x1,y1,x2,y2);
  if (open)
  {
    can->Pen->Color = clWhite;
    can->MoveTo(x1, y1); can->LineTo(x2, y1);
  }
//  can->Brush->Color = b;
//  can->Pen->Color = p;
}

void __fastcall TForm7::DrawGraph(int index)
{
try
{
  int nGraphLeftPadding = 45;
  int nGraphRightPadding = 15;
  int nGraphTopPadding = 15;
  int nGraphBottomPadding = 30;
  double SpaceUp = 0.1;    //  10% отступ сверху
  double SpaceDown = 0.1;  //  10% отступ сниху
  int nGraphWidth = Image1->Width - nGraphLeftPadding - nGraphRightPadding;
  int nGraphHeight = Image1->Height - nGraphTopPadding - nGraphBottomPadding;
  TCanvas *can = Image1->Canvas;
  can->Pen->Color = clBlack;
  can->Brush->Color = clWhite;
  can->Rectangle(0,0,Image1->Width,Image1->Height);
  double Wmin = Edit1->Text.ToDouble();
  double Wmax = Edit2->Text.ToDouble();
  if (RadioButton2->Checked)
  {
    Wmin = log10(Wmin);
    Wmax = log10(Wmax);
  }
  double PPF = nGraphWidth/(Wmax-Wmin);

  switch (index)
  {
    case 0:
    {
      double Wpass = Edit4->Text.ToDouble();
      double Wstop = Edit5->Text.ToDouble();
      if (RadioButton2->Checked)
      {
        Wpass = log10(Wpass);
        Wstop = log10(Wstop);
      }
      double Apass1 = Edit6->Text.ToDouble();
      double Apass2 = Edit7->Text.ToDouble();
      double Astop = Edit8->Text.ToDouble();
      double Amin = Apass1 - (Astop-Apass1)*SpaceUp;
      double Amax = Astop + (Astop-Apass1)*SpaceDown;
      double PPA = nGraphHeight/(Amax-Amin);

      // прямоугольники
      MyRectangle(can, nGraphLeftPadding                 , nGraphTopPadding                  , nGraphLeftPadding+(Wpass-Wmin)*PPF, nGraphTopPadding+(Apass1-Amin)*PPA  , true);
      MyRectangle(can, nGraphLeftPadding                 , nGraphTopPadding+(Apass2-Amin)*PPA, nGraphLeftPadding+(Wpass-Wmin)*PPF, Image1->Height-nGraphBottomPadding+1, false);
      MyRectangle(can, nGraphLeftPadding+(Wstop-Wmin)*PPF, nGraphTopPadding                  , Image1->Width-nGraphRightPadding  , nGraphTopPadding+(Astop-Amin)*PPA   , true);

      // подписи и штришки
      can->Pen->Color = clBlack;
      can->Brush->Color = clWhite;
      VerticalTicks(can, nGraphLeftPadding, nGraphTopPadding+(Apass1-Amin)*PPA-1, "Apass1");
      VerticalTicks(can, nGraphLeftPadding, nGraphTopPadding+(Apass2-Amin)*PPA,   "Apass2");
      VerticalTicks(can, nGraphLeftPadding, nGraphTopPadding+(Astop -Amin)*PPA-1, "Astop");
      HorizontalTicks(can, nGraphLeftPadding                   , Image1->Height-nGraphBottomPadding, "wmin");
      HorizontalTicks(can, nGraphLeftPadding+(Wpass-Wmin)*PPF-1, Image1->Height-nGraphBottomPadding, "wpass");
      HorizontalTicks(can, nGraphLeftPadding+(Wstop-Wmin)*PPF  , Image1->Height-nGraphBottomPadding, "wstop");
      HorizontalTicks(can, Image1->Width-nGraphRightPadding-1  , Image1->Height-nGraphBottomPadding, "wmax");

    }
    break;

    case 1:
    {
      double Wstop = Edit9->Text.ToDouble();
      double Wpass = Edit10->Text.ToDouble();
      if (RadioButton2->Checked)
      {
        Wstop = log10(Wstop);
        Wpass = log10(Wpass);
      }
      double Astop = Edit11->Text.ToDouble();
      double Apass1 = Edit12->Text.ToDouble();
      double Apass2 = Edit13->Text.ToDouble();
      double Amin = Apass1 - (Astop-Apass1)*SpaceUp;
      double Amax = Astop + (Astop-Apass1)*SpaceDown;
      double PPA = nGraphHeight/(Amax-Amin);

      // прямоугольники
      MyRectangle(can, nGraphLeftPadding                 , nGraphTopPadding                  , nGraphLeftPadding+(Wstop-Wmin)*PPF, nGraphTopPadding+(Astop-Amin)*PPA   , true);
      MyRectangle(can, nGraphLeftPadding+(Wpass-Wmin)*PPF, nGraphTopPadding                  , Image1->Width-nGraphRightPadding  , nGraphTopPadding+(Apass1-Amin)*PPA  , true);
      MyRectangle(can, nGraphLeftPadding+(Wpass-Wmin)*PPF, nGraphTopPadding+(Apass2-Amin)*PPA, Image1->Width-nGraphRightPadding  , Image1->Height-nGraphBottomPadding+1, false);

      // подписи и штришки
      can->Pen->Color = clBlack;
      can->Brush->Color = clWhite;
      VerticalTicks(can, nGraphLeftPadding, nGraphTopPadding+(Apass1-Amin)*PPA-1, "Apass1");
      VerticalTicks(can, nGraphLeftPadding, nGraphTopPadding+(Apass2-Amin)*PPA,   "Apass2");
      VerticalTicks(can, nGraphLeftPadding, nGraphTopPadding+(Astop -Amin)*PPA-1, "Astop");
      HorizontalTicks(can, nGraphLeftPadding                   , Image1->Height-nGraphBottomPadding, "wmin");
      HorizontalTicks(can, nGraphLeftPadding+(Wpass-Wmin)*PPF  , Image1->Height-nGraphBottomPadding, "wpass");
      HorizontalTicks(can, nGraphLeftPadding+(Wstop-Wmin)*PPF-1, Image1->Height-nGraphBottomPadding, "wstop");
      HorizontalTicks(can, Image1->Width-nGraphRightPadding-1  , Image1->Height-nGraphBottomPadding, "wmax");

    }
    break;

    case 2:
    {
      double Wstop1 = Edit14->Text.ToDouble();
      double Wpass1 = Edit15->Text.ToDouble();
      double Wpass2 = Edit16->Text.ToDouble();
      double Wstop2 = Edit17->Text.ToDouble();

      if (RadioButton2->Checked)
      {
        Wstop1 = log10(Wstop1);
        Wpass1 = log10(Wpass1);
        Wpass2 = log10(Wpass2);
        Wstop2 = log10(Wstop2);
      }
      double Astop1 = Edit18->Text.ToDouble();
      double Apass1 = Edit19->Text.ToDouble();
      double Apass2 = Edit20->Text.ToDouble();
      double Astop2 = Edit21->Text.ToDouble();
      double Amax,Amin;
      if (Astop2>Astop1)
      {
        Amax = Astop2 + (Astop2-Apass1)*SpaceDown;
        Amin = Apass1 - (Astop2-Apass1)*SpaceUp;
      }
      else
      {
        Amax = Astop1 + (Astop1-Apass1)*SpaceDown;
        Amin = Apass1 - (Astop1-Apass1)*SpaceUp;
      }
      double PPA = nGraphHeight/(Amax-Amin);

      // прямоугольники
      MyRectangle(can, nGraphLeftPadding                  , nGraphTopPadding                  , nGraphLeftPadding+(Wstop1-Wmin)*PPF, nGraphTopPadding+(Astop1-Amin)*PPA  , true);
      MyRectangle(can, nGraphLeftPadding+(Wpass1-Wmin)*PPF, nGraphTopPadding                  , nGraphLeftPadding+(Wpass2-Wmin)*PPF, nGraphTopPadding+(Apass1-Amin)*PPA  , true);
      MyRectangle(can, nGraphLeftPadding+(Wpass1-Wmin)*PPF, nGraphTopPadding+(Apass2-Amin)*PPA, nGraphLeftPadding+(Wpass2-Wmin)*PPF, Image1->Height-nGraphBottomPadding+1, false);
      MyRectangle(can, nGraphLeftPadding+(Wstop2-Wmin)*PPF, nGraphTopPadding                  , Image1->Width-nGraphRightPadding   , nGraphTopPadding+(Astop2-Amin)*PPA  , true);

      // подписи и штришки
      can->Pen->Color = clBlack;
      can->Brush->Color = clWhite;
      VerticalTicks(can, nGraphLeftPadding, nGraphTopPadding+(Apass1-Amin)*PPA-1, "Apass1");
      VerticalTicks(can, nGraphLeftPadding, nGraphTopPadding+(Apass2-Amin)*PPA,   "Apass2");
      VerticalTicks(can, nGraphLeftPadding, nGraphTopPadding+(Astop1-Amin)*PPA-1, "Astop1");
      VerticalTicks(can, nGraphLeftPadding, nGraphTopPadding+(Astop2-Amin)*PPA-1, "Astop2");
      HorizontalTicks(can, nGraphLeftPadding                    , Image1->Height-nGraphBottomPadding, "wmin");
      HorizontalTicks(can, nGraphLeftPadding+(Wpass1-Wmin)*PPF  , Image1->Height-nGraphBottomPadding, "wpass1");
      HorizontalTicks(can, nGraphLeftPadding+(Wpass2-Wmin)*PPF-1, Image1->Height-nGraphBottomPadding, "wpass2");
      HorizontalTicks(can, nGraphLeftPadding+(Wstop1-Wmin)*PPF-1, Image1->Height-nGraphBottomPadding, "wstop1");
      HorizontalTicks(can, nGraphLeftPadding+(Wstop2-Wmin)*PPF  , Image1->Height-nGraphBottomPadding, "wstop2");
      HorizontalTicks(can, Image1->Width-nGraphRightPadding-1   , Image1->Height-nGraphBottomPadding, "wmax");

    }
    break;

    case 3:
    {
      double Wpass1 = Edit22->Text.ToDouble();
      double Wstop1 = Edit23->Text.ToDouble();
      double Wstop2 = Edit24->Text.ToDouble();
      double Wpass2 = Edit25->Text.ToDouble();
      if (RadioButton2->Checked)
      {
        Wpass1 = log10(Wpass1);
        Wstop1 = log10(Wstop1);
        Wstop2 = log10(Wstop2);
        Wpass2 = log10(Wpass2);
      }
      double Apass1 = Edit26->Text.ToDouble();
      double Apass2 = Edit27->Text.ToDouble();
      double Astop  = Edit28->Text.ToDouble();
      double Apass3 = Edit29->Text.ToDouble();
      double Apass4 = Edit30->Text.ToDouble();
      double Amax,Amin;
      if (Apass1<Apass3)
      {
        Amax = Astop + (Astop-Apass1)*SpaceDown;
        Amin = Apass1 - (Astop-Apass1)*SpaceUp;
      }
      else
      {
        Amax = Astop + (Astop-Apass3)*SpaceDown;
        Amin = Apass3 - (Astop-Apass3)*SpaceUp;
      }
      double PPA = nGraphHeight/(Amax-Amin);

      // прямоугольники
      MyRectangle(can, nGraphLeftPadding                  , nGraphTopPadding                  , nGraphLeftPadding+(Wpass1-Wmin)*PPF, nGraphTopPadding+(Apass1-Amin)*PPA  , true);
      MyRectangle(can, nGraphLeftPadding                  , nGraphTopPadding+(Apass2-Amin)*PPA, nGraphLeftPadding+(Wpass1-Wmin)*PPF, Image1->Height-nGraphBottomPadding+1, false);
      MyRectangle(can, nGraphLeftPadding+(Wstop1-Wmin)*PPF, nGraphTopPadding                  , nGraphLeftPadding+(Wstop2-Wmin)*PPF, nGraphTopPadding+(Astop-Amin)*PPA   , true);
      MyRectangle(can, nGraphLeftPadding+(Wpass2-Wmin)*PPF, nGraphTopPadding                  , Image1->Width-nGraphRightPadding   , nGraphTopPadding+(Apass3-Amin)*PPA  , true);
      MyRectangle(can, nGraphLeftPadding+(Wpass2-Wmin)*PPF, nGraphTopPadding+(Apass4-Amin)*PPA, Image1->Width-nGraphRightPadding   , Image1->Height-nGraphBottomPadding+1, false);

      // подписи и штришки
      can->Pen->Color = clBlack;
      can->Brush->Color = clWhite;
      VerticalTicks(can, nGraphLeftPadding, nGraphTopPadding+(Apass1-Amin)*PPA-1, "Apass1");
      VerticalTicks(can, nGraphLeftPadding, nGraphTopPadding+(Apass2-Amin)*PPA,   "Apass2");
      VerticalTicks(can, nGraphLeftPadding, nGraphTopPadding+(Astop -Amin)*PPA-1, "Astop");
      VerticalTicks(can, nGraphLeftPadding, nGraphTopPadding+(Apass3-Amin)*PPA,   "Apass3");
      VerticalTicks(can, nGraphLeftPadding, nGraphTopPadding+(Apass4-Amin)*PPA,   "Apass4");
      HorizontalTicks(can, nGraphLeftPadding                    , Image1->Height-nGraphBottomPadding, "wmin");
      HorizontalTicks(can, nGraphLeftPadding+(Wpass1-Wmin)*PPF-1, Image1->Height-nGraphBottomPadding, "wpass1");
      HorizontalTicks(can, nGraphLeftPadding+(Wstop1-Wmin)*PPF  , Image1->Height-nGraphBottomPadding, "wstop1");
      HorizontalTicks(can, nGraphLeftPadding+(Wstop2-Wmin)*PPF-1, Image1->Height-nGraphBottomPadding, "wstop2");
      HorizontalTicks(can, nGraphLeftPadding+(Wpass2-Wmin)*PPF  , Image1->Height-nGraphBottomPadding, "wpass2");
      HorizontalTicks(can, Image1->Width-nGraphRightPadding-1   , Image1->Height-nGraphBottomPadding, "wmax");

    }
    break;

  }

  // оси координат
  can->Pen->Color = clBlack;
  can->MoveTo(nGraphLeftPadding, 0);
  can->LineTo(nGraphLeftPadding, Image1->Height-nGraphBottomPadding);
  can->LineTo(Image1->Width, Image1->Height-nGraphBottomPadding);

  // стрелки осей координат
  can->MoveTo(nGraphLeftPadding-3, 10); can->LineTo(nGraphLeftPadding, 0); can->LineTo(nGraphLeftPadding+3, 11);
  can->MoveTo(Image1->Width-10, Image1->Height-nGraphBottomPadding-3); can->LineTo(Image1->Width, Image1->Height-nGraphBottomPadding); can->LineTo(Image1->Width-11, Image1->Height-nGraphBottomPadding+3);
}
catch (EZeroDivide &e)
{
}
}
//---------------------------------------------------------------------------

void __fastcall TForm7::OnEditKeyPress(TObject *Sender, char &Key)
{
  if ((Key >= '0') && (Key <= '9')) {} // если цифра
  else if (Key == 8) {}
  else if (Key=='-' && Sender!=Edit1 && Sender!=Edit2 && Sender!=Edit3 && Sender!=Edit4 && Sender!=Edit5 && Sender!=Edit9 && Sender!=Edit10 && Sender!=Edit14 && Sender!=Edit15 && Sender!=Edit16 && Sender!=Edit17 && Sender!=Edit22 && Sender!=Edit23 && Sender!=Edit24 && Sender!=Edit25 && ((TEdit*)Sender)->SelStart==0 && (((TEdit*)Sender)->Text.Pos(Key)==0 || ((TEdit*)Sender)->SelText.Pos(Key)!=0)) {} //минус можно поставить только в начало и только если минуса еще нет
  else if ((Key=='.' || Key==',') && Sender!=Edit3 && (((TEdit*)Sender)->Text.Pos(DecimalSeparator)==0 || ((TEdit*)Sender)->SelText.Pos(DecimalSeparator)!=0)) // запятую и точку заменяем на разделитель целой и дробной части (настроено что это точка), разделитель можно поставить только если в строке он ниразу не встречается
    Key = DecimalSeparator;
  else Key = 0; // не цифра
}

void __fastcall TForm7::ComboBox2Change(TObject *Sender)
{
  switch (ComboBox2->ItemIndex)
  {
    case 0: Panel1->Visible =  true; Panel2->Visible = false; Panel3->Visible = false; Panel4->Visible = false; break;
    case 1: Panel1->Visible = false; Panel2->Visible =  true; Panel3->Visible = false; Panel4->Visible = false; break;
    case 2: Panel1->Visible = false; Panel2->Visible = false; Panel3->Visible =  true; Panel4->Visible = false; break;
    case 3: Panel1->Visible = false; Panel2->Visible = false; Panel3->Visible = false; Panel4->Visible =  true; break;
  }
  DrawGraph(ComboBox2->ItemIndex);
}
//---------------------------------------------------------------------------

void __fastcall TForm7::Button1Click(TObject *Sender)
{
  if (ValidateForm(ComboBox2->ItemIndex))
  {
    double dw,w;
    double Wmin = Edit1->Text.ToDouble();
    double Wmax = Edit2->Text.ToDouble();
    int n = Edit3->Text.ToInt();
    for (int i=Form3->StringGrid1->RowCount-2; i>0; i--)
      Form3->StringGrid1->Rows[i]->Clear();
    Form3->StringGrid1->RowCount = n+1;

    if (ComboBox2->ItemIndex & 2)
      n -= 4;
    else
      n -= 2;

    if (RadioButton1->Checked)
      dw = (Wmax-Wmin)/(n-1);
    else
      dw = (log10(Wmax/Wmin))/(n-1);

    switch (ComboBox2->ItemIndex)
    {
      case 0:
      {
        for (int i=0; i<n; i++)
        {
          if (RadioButton1->Checked)
            w = Wmin+dw*i;
          else
            w = Wmin*pow(10.0, dw*i);

          Form3->StringGrid1->Cells[0][i+1]=FloatToStrF(w, ffFixed, nPrecision, nDigits);

          if (w <= Edit4->Text.ToDouble())
          {
            Form3->StringGrid1->Cells[2][i+1]=FloatToStrF(-Edit6->Text.ToDouble(), ffFixed, nPrecision, nDigits);
            Form3->StringGrid1->Cells[1][i+1]=FloatToStrF(-Edit7->Text.ToDouble(), ffFixed, nPrecision, nDigits);
          }
          else if (w >= Edit5->Text.ToDouble())
          {
            Form3->StringGrid1->Cells[2][i+1]=FloatToStrF(-Edit8->Text.ToDouble(), ffFixed, nPrecision, nDigits);
            Form3->StringGrid1->Cells[1][i+1]="";
          }
          else
          {
            Form3->StringGrid1->Cells[2][i+1]="";
            Form3->StringGrid1->Cells[1][i+1]="";
          }
        }

        Form3->StringGrid1->Cells[0][n+1]=FloatToStrF( Edit4->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[2][n+1]=FloatToStrF(-Edit6->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[1][n+1]=FloatToStrF(-Edit7->Text.ToDouble(), ffFixed, nPrecision, nDigits);

        Form3->StringGrid1->Cells[0][n+2]=FloatToStrF( Edit5->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[2][n+2]=FloatToStrF(-Edit8->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[1][n+2]="";
      }
      break;

      case 1:
      {
        for (int i=0; i<n; i++)
        {
          if (RadioButton1->Checked)
            w = Wmin+dw*i;
          else
            w = Wmin*pow(10.0, dw*i);

          Form3->StringGrid1->Cells[0][i+1]=FloatToStrF(w, ffFixed, nPrecision, nDigits);

          if (w <= Edit9->Text.ToDouble())
          {
            Form3->StringGrid1->Cells[2][i+1]=FloatToStrF(-Edit11->Text.ToDouble(), ffFixed, nPrecision, nDigits);
            Form3->StringGrid1->Cells[1][i+1]="";
          }
          else if (w >= Edit10->Text.ToDouble())
          {
            Form3->StringGrid1->Cells[2][i+1]=FloatToStrF(-Edit12->Text.ToDouble(), ffFixed, nPrecision, nDigits);
            Form3->StringGrid1->Cells[1][i+1]=FloatToStrF(-Edit13->Text.ToDouble(), ffFixed, nPrecision, nDigits);
          }
          else
          {
            Form3->StringGrid1->Cells[2][i+1]="";
            Form3->StringGrid1->Cells[1][i+1]="";
          }
        }

        Form3->StringGrid1->Cells[0][n+1]=FloatToStrF( Edit9->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[2][n+1]=FloatToStrF(-Edit11->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[1][n+1]="";

        Form3->StringGrid1->Cells[0][n+2]=FloatToStrF( Edit10->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[2][n+2]=FloatToStrF(-Edit12->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[1][n+2]=FloatToStrF(-Edit13->Text.ToDouble(), ffFixed, nPrecision, nDigits);
      }
      break;

      case 2:
      {
        for (int i=0; i<n; i++)
        {
          if (RadioButton1->Checked)
            w = Wmin+dw*i;
          else
            w = Wmin*pow(10.0, dw*i);

          Form3->StringGrid1->Cells[0][i+1]=FloatToStrF(w, ffFixed, nPrecision, nDigits);

          if (w <= Edit14->Text.ToDouble())
          {
            Form3->StringGrid1->Cells[2][i+1]=FloatToStrF(-Edit18->Text.ToDouble(), ffFixed, nPrecision, nDigits);
            Form3->StringGrid1->Cells[1][i+1]="";
          }
          else if (w >= Edit15->Text.ToDouble() && w <= Edit16->Text.ToDouble())
          {
            Form3->StringGrid1->Cells[2][i+1]=FloatToStrF(-Edit19->Text.ToDouble(), ffFixed, nPrecision, nDigits);
            Form3->StringGrid1->Cells[1][i+1]=FloatToStrF(-Edit20->Text.ToDouble(), ffFixed, nPrecision, nDigits);
          }
          else if (w >= Edit17->Text.ToDouble())
          {
            Form3->StringGrid1->Cells[2][i+1]=FloatToStrF(-Edit21->Text.ToDouble(), ffFixed, nPrecision, nDigits);
            Form3->StringGrid1->Cells[1][i+1]="";
          }
          else
          {
            Form3->StringGrid1->Cells[2][i+1]="";
            Form3->StringGrid1->Cells[1][i+1]="";
          }
        }

        Form3->StringGrid1->Cells[0][n+1]=FloatToStrF( Edit14->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[2][n+1]=FloatToStrF(-Edit18->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[1][n+1]="";

        Form3->StringGrid1->Cells[0][n+2]=FloatToStrF( Edit15->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[2][n+2]=FloatToStrF(-Edit19->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[1][n+2]=FloatToStrF(-Edit20->Text.ToDouble(), ffFixed, nPrecision, nDigits);

        Form3->StringGrid1->Cells[0][n+3]=FloatToStrF( Edit16->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[2][n+3]=FloatToStrF(-Edit19->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[1][n+3]=FloatToStrF(-Edit20->Text.ToDouble(), ffFixed, nPrecision, nDigits);

        Form3->StringGrid1->Cells[0][n+4]=FloatToStrF( Edit17->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[2][n+4]=FloatToStrF(-Edit21->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[1][n+4]="";
      }
      break;

      case 3:
      {
        for (int i=0; i<n; i++)
        {
          if (RadioButton1->Checked)
            w = Wmin+dw*i;
          else
            w = Wmin*pow(10.0, dw*i);

          Form3->StringGrid1->Cells[0][i+1]=FloatToStrF(w, ffFixed, nPrecision, nDigits);

          if (w <= Edit22->Text.ToDouble())
          {
            Form3->StringGrid1->Cells[2][i+1]=FloatToStrF(-Edit26->Text.ToDouble(), ffFixed, nPrecision, nDigits);
            Form3->StringGrid1->Cells[1][i+1]=FloatToStrF(-Edit27->Text.ToDouble(), ffFixed, nPrecision, nDigits);
          }
          else if (w >= Edit23->Text.ToDouble() && w <= Edit24->Text.ToDouble())
          {
            Form3->StringGrid1->Cells[2][i+1]=FloatToStrF(-Edit28->Text.ToDouble(), ffFixed, nPrecision, nDigits);
            Form3->StringGrid1->Cells[1][i+1]="";
          }
          else if (w >= Edit25->Text.ToDouble())
          {
            Form3->StringGrid1->Cells[2][i+1]=FloatToStrF(-Edit29->Text.ToDouble(), ffFixed, nPrecision, nDigits);
            Form3->StringGrid1->Cells[1][i+1]=FloatToStrF(-Edit30->Text.ToDouble(), ffFixed, nPrecision, nDigits);
          }
          else
          {
            Form3->StringGrid1->Cells[2][i+1]="";
            Form3->StringGrid1->Cells[1][i+1]="";
          }
        }

        Form3->StringGrid1->Cells[0][n+1]=FloatToStrF( Edit22->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[2][n+1]=FloatToStrF(-Edit26->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[1][n+1]=FloatToStrF(-Edit27->Text.ToDouble(), ffFixed, nPrecision, nDigits);

        Form3->StringGrid1->Cells[0][n+2]=FloatToStrF( Edit23->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[2][n+2]=FloatToStrF(-Edit28->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[1][n+2]="";

        Form3->StringGrid1->Cells[0][n+3]=FloatToStrF( Edit24->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[2][n+3]=FloatToStrF(-Edit28->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[1][n+3]="";

        Form3->StringGrid1->Cells[0][n+4]=FloatToStrF( Edit25->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[2][n+4]=FloatToStrF(-Edit29->Text.ToDouble(), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[1][n+4]=FloatToStrF(-Edit30->Text.ToDouble(), ffFixed, nPrecision, nDigits);
      }
      break;
    }

    Form1->Chart1->TopAxis->Logarithmic    = RadioButton2->Checked;
    Form1->Chart1->BottomAxis->Logarithmic = RadioButton2->Checked;
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm7::FormCreate(TObject *Sender)
{
  DrawGraph(ComboBox2->ItemIndex);
}
//---------------------------------------------------------------------------

bool __fastcall TForm7::ValidateForm(int index)
{
  int n = Edit3->Text.ToIntDef(0);
  if (index&2 && n<6)
  {
    Label31->Caption = "Должно быть минимум 6 точек";
    return false;
  }
  else if (n<4)
  {
    Label31->Caption = "Должно быть минимум 4 точки";
    return false;
  }
  if (Edit1->Text.ToDouble() <= 0.0)
  {
    Label31->Caption = "Минимальная частота должна быть > 0";
    return false;
  }
  if (Edit2->Text.ToDouble() <= 0.0)
  {
    Label31->Caption = "Максимальная частота должна быть > 0";
    return false;
  }

  switch (index)
  {
    case 0:
    {
      if (Edit1->Text.Length()>0 && Edit2->Text.Length()>0 && Edit4->Text.Length()>0 && Edit5->Text.Length()>0 && Edit6->Text.Length()>0 && Edit7->Text.Length()>0 && Edit8->Text.Length()>0)
      {
        if (Edit1->Text.ToDouble() >= Edit4->Text.ToDouble() || Edit4->Text.ToDouble() >= Edit5->Text.ToDouble() || Edit5->Text.ToDouble() >= Edit2->Text.ToDouble())
        {
          Label31->Caption = "Не выполняется условие: Wmin < wpass < wstop < Wmax";
          return false;
        }
        if (Edit6->Text.ToDouble() > Edit7->Text.ToDouble())
        {
          Label31->Caption = "Не выполняется условие: Apass2 >= Apass1";
          return false;
        }
      }
      else
      {
        Label31->Caption = "Есть незаполненные поля";
        return false;
      }
    }
    break;

    case 1:
    {
      if (Edit1->Text.Length()>0 && Edit2->Text.Length()>0 && Edit9->Text.Length()>0 && Edit10->Text.Length()>0 && Edit11->Text.Length()>0 && Edit12->Text.Length()>0 && Edit13->Text.Length()>0)
      {
        if (Edit1->Text.ToDouble() >= Edit9->Text.ToDouble() || Edit9->Text.ToDouble() >= Edit10->Text.ToDouble() || Edit10->Text.ToDouble() >= Edit2->Text.ToDouble())
        {
          Label31->Caption = "Не выполняется условие: Wmin < wstop < wpass < Wmax";
          return false;
        }
        if (Edit12->Text.ToDouble() > Edit13->Text.ToDouble())
        {
          Label31->Caption = "Не выполняется условие: Apass2 >= Apass1";
          return false;
        }
      }
      else
      {
        ShowMessage("Есть незаполненные поля");
        return false;
      }
    }
    break;

    case 2:
    {
      if (Edit1->Text.Length()>0 && Edit2->Text.Length()>0 && Edit14->Text.Length()>0 && Edit15->Text.Length()>0 && Edit16->Text.Length()>0 && Edit17->Text.Length()>0 && Edit18->Text.Length()>0 && Edit19->Text.Length()>0 && Edit20->Text.Length()>0 && Edit21->Text.Length()>0)
      {
        if (Edit1->Text.ToDouble() >= Edit14->Text.ToDouble() || Edit14->Text.ToDouble() >= Edit15->Text.ToDouble() || Edit15->Text.ToDouble() >= Edit16->Text.ToDouble() || Edit16->Text.ToDouble() >= Edit17->Text.ToDouble() || Edit17->Text.ToDouble() >= Edit2->Text.ToDouble())
        {
          Label31->Caption = "Не выполняется условие: Wmin < wstop < wpass1 < wpass2 < wstop2 < Wmax";
          return false;
        }
        if (Edit19->Text.ToDouble() > Edit20->Text.ToDouble())
        {
          Label31->Caption = "Не выполняется условие: Apass2 >= Apass1";
          return false;
        }
      }
      else
      {
        Label31->Caption = "Есть незаполненные поля";
        return false;
      }
    }
    break;

    case 3:
    {
      if (Edit1->Text.Length()>0 && Edit2->Text.Length()>0 && Edit22->Text.Length()>0 && Edit23->Text.Length()>0 && Edit24->Text.Length()>0 && Edit25->Text.Length()>0 && Edit26->Text.Length()>0 && Edit27->Text.Length()>0 && Edit28->Text.Length()>0 && Edit29->Text.Length()>0 && Edit30->Text.Length()>0)
      {
        if (Edit1->Text.ToDouble() >= Edit22->Text.ToDouble() || Edit22->Text.ToDouble() >= Edit23->Text.ToDouble() || Edit23->Text.ToDouble() >= Edit24->Text.ToDouble() || Edit24->Text.ToDouble() >= Edit25->Text.ToDouble() || Edit25->Text.ToDouble() >= Edit2->Text.ToDouble())
        {
          Label31->Caption = "Не выполняется условие: Wmin < wpass1 < wstop1 < wstop2 < wpass2 < Wmax";
          return false;
        }
        if (Edit26->Text.ToDouble() > Edit27->Text.ToDouble())
        {
          Label31->Caption = "Не выполняется условие: Apass2 >= Apass1";
          return false;
        }
        if (Edit29->Text.ToDouble() > Edit30->Text.ToDouble())
        {
          Label31->Caption = "Не выполняется условие: Apass4 >= Apass3";
          return false;
        }
      }
      else
      {
        Label31->Caption = "Есть незаполненные поля";
        return false;
      }
    }
    break;
  }
  return true;
}
//---------------------------------------------------------------------------

void __fastcall TForm7::RadioButton1Click(TObject *Sender)
{
  DrawGraph(ComboBox2->ItemIndex);
}
//---------------------------------------------------------------------------

void __fastcall TForm7::RadioButton2Click(TObject *Sender)
{
  DrawGraph(ComboBox2->ItemIndex);
}
//---------------------------------------------------------------------------

void __fastcall TForm7::OnEditChange(TObject *Sender)
{
  if (ValidateForm(ComboBox2->ItemIndex))
  {
    Button1->Enabled = true;
    Label31->Visible = false;
    DrawGraph(ComboBox2->ItemIndex);
  }
  else
  {
    Button1->Enabled = false;
    Label31->Visible = true;
  }
}
//---------------------------------------------------------------------------

