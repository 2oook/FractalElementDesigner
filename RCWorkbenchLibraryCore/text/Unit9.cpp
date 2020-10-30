//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "Unit9.h"
#include "Unit1.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm9 *Form9;

extern CAnalyseParameters *pAnalyseParameters;
//---------------------------------------------------------------------------
__fastcall TForm9::TForm9(TComponent* Owner)
  : TForm(Owner)
{
}
//---------------------------------------------------------------------------

void __fastcall TForm9::Button1Click(TObject *Sender)
{
  SaveDialog1->Execute();
  if (SaveDialog1->FileName.Length())
    Edit1->Text = SaveDialog1->FileName;
}
//---------------------------------------------------------------------------
void __fastcall TForm9::Button2Click(TObject *Sender)
{
  int nPrecision = 7;
  int nDigits = 2;
  AnsiString strCond;

  AnsiString pathtofile = Edit1->Text;
  if (!ExtractFileDrive(pathtofile).Length())
    SetCurrentDirectory(ExtractFileDir(Application->ExeName).c_str()); // если указано только им€ файла, или относительный путь к файлу, а не полный путь, то измен€ем текущую директорию на директорию где лежит программа

  FILE *f = fopen(pathtofile.c_str(), "wt");
  if (f)
  {
    const char space = 9;
    const char CRLF = 10;

//    AnsiString str = "% [w,y11_real,y11_imag,y12_real,y12_imag,y22_real,y22_imag,scheme_real,scheme_imag]=textread('export.txt','%f %f %f %f %f %f %f %f %f','commentstyle','matlab');";
//    fwrite(str.c_str(), sizeof(char), str.Length(), f);
//    fwrite(&CRLF, sizeof(char), 1, f);

    AnsiString str = "% [w";
    fwrite(str.c_str(), sizeof(char), str.Length(), f);
    int values = 1;

    for (int y=0; y<nCheckBoxArraySize; y++)
      for (int x=y; x<nCheckBoxArraySize; x++)
        if (pCheckBoxArray[y][x]->Checked)
        {
          str = "," + pCheckBoxArray[y][x]->Caption + "_real," + pCheckBoxArray[y][x]->Caption + "_imag";
          fwrite(str.c_str(), sizeof(char), str.Length(), f);
          values++;
          values++;
        }

    if (CheckBox5->Checked)
    {
      str = ",conductance";
      fwrite(str.c_str(), sizeof(char), str.Length(), f);
      values++;
    }
    if (CheckBox4->Checked)
    {
      str = ",scheme_real,scheme_imag";
      fwrite(str.c_str(), sizeof(char), str.Length(), f);
      values++;
      values++;
    }
    if (CheckBox6->Checked)
    {
      str = ",magnitude";
      fwrite(str.c_str(), sizeof(char), str.Length(), f);
      values++;
    }
    if (CheckBox7->Checked)
    {
      str = ",phase";
      fwrite(str.c_str(), sizeof(char), str.Length(), f);
      values++;
    }
    str = "]=textread('" + Edit1->Text + "','";
    fwrite(str.c_str(), sizeof(char), str.Length(), f);

    str = "%f ";
    for (int i=0; i<values; i++)
    {
      if (i == values-1)
        str = "%f','commentstyle','matlab');";
      fwrite(str.c_str(), sizeof(char), str.Length(), f);
    }
    fwrite(&CRLF, sizeof(char), 1, f);


    double *pPhaseArray = new double[pAnalyseParameters->m_pT->m_length];

    str = "% „астота";
    fwrite(str.c_str(), sizeof(char), str.Length(), f);
    fwrite(&space, sizeof(char), 1, f);

    for (int y=0; y<nCheckBoxArraySize; y++)
      for (int x=y; x<nCheckBoxArraySize; x++)
        if (pCheckBoxArray[y][x]->Checked)
        {
          str = pCheckBoxArray[y][x]->Caption + ".real";
          fwrite(str.c_str(), sizeof(char), str.Length(), f);
          fwrite(&space, sizeof(char), 1, f);
          str = pCheckBoxArray[y][x]->Caption + ".imag";
          fwrite(str.c_str(), sizeof(char), str.Length(), f);
          fwrite(&space, sizeof(char), 1, f);
        }

    if (CheckBox5->Checked)
    {
      str = CheckBox5->Caption;
      fwrite(str.c_str(), sizeof(char), str.Length(), f);
      fwrite(&space, sizeof(char), 1, f);
      complex<double> *cond = new complex<double>[21];
      Form1->Structure5->YParameters(0.0, cond);
      strCond = FloatToStrF(cond[0].real(), ffExponent, nPrecision, nDigits);
      delete[] cond;
    }
    if (CheckBox4->Checked)
    {
      str = CheckBox4->Caption + ".real";
      fwrite(str.c_str(), sizeof(char), str.Length(), f);
      fwrite(&space, sizeof(char), 1, f);
      str = CheckBox4->Caption + ".imag";
      fwrite(str.c_str(), sizeof(char), str.Length(), f);
      fwrite(&space, sizeof(char), 1, f);
    }
    if (CheckBox6->Checked)
    {
      str = CheckBox6->Caption;
      fwrite(str.c_str(), sizeof(char), str.Length(), f);
      fwrite(&space, sizeof(char), 1, f);
    }
    if (CheckBox7->Checked)
    {
      str = CheckBox7->Caption;
      fwrite(str.c_str(), sizeof(char), str.Length(), f);
      fwrite(&space, sizeof(char), 1, f);

      // рассчитываем массив со значением фазы
      for (int i=0; i<pAnalyseParameters->m_pT->m_length; i++)
      {
        pPhaseArray[i] = CScheme::Phase(pAnalyseParameters->m_pT->m_T[i]);
        if (i>0)
        {
          // исправл€ем переход -180/180
          if (pPhaseArray[i] - pPhaseArray[i-1] < -350.0)
            pPhaseArray[i] += 360.0;
          else if (pPhaseArray[i] - pPhaseArray[i-1] > 350.0)
            pPhaseArray[i] -= 360.0;
        }
      }
    }
    fwrite(&CRLF, sizeof(char), 1, f);

    for (int i=0; i<pAnalyseParameters->m_pT->m_length; i++)
    {
      str = FloatToStrF(pAnalyseParameters->m_pT->m_w[i], ffExponent, nPrecision, nDigits);
      fwrite(str.c_str(), sizeof(char), str.Length(), f);
      fwrite(&space, sizeof(char), 1, f);

      for (int y=0; y<nCheckBoxArraySize; y++)
        for (int x=y; x<nCheckBoxArraySize; x++)
          if (pCheckBoxArray[y][x]->Checked)
          {
            int k = nCheckBoxArraySize*y - (y|1)*((y+1)>>1) + x;
            str = FloatToStrF(pAnalyseParameters->m_pT->m_MatY[i][k].real(), ffExponent, nPrecision, nDigits);
            fwrite(str.c_str(), sizeof(char), str.Length(), f);
            fwrite(&space, sizeof(char), 1, f);
            str = FloatToStrF(pAnalyseParameters->m_pT->m_MatY[i][k].imag(), ffExponent, nPrecision, nDigits);
            fwrite(str.c_str(), sizeof(char), str.Length(), f);
            fwrite(&space, sizeof(char), 1, f);
          }

      if (CheckBox5->Checked)
      {
        str = strCond;
        fwrite(str.c_str(), sizeof(char), str.Length(), f);
        fwrite(&space, sizeof(char), 1, f);
      }
      if (CheckBox4->Checked)
      {
        str = FloatToStrF(pAnalyseParameters->m_pT->m_T[i].real(), ffExponent, nPrecision, nDigits);
        fwrite(str.c_str(), sizeof(char), str.Length(), f);
        fwrite(&space, sizeof(char), 1, f);
        str = FloatToStrF(pAnalyseParameters->m_pT->m_T[i].imag(), ffExponent, nPrecision, nDigits);
        fwrite(str.c_str(), sizeof(char), str.Length(), f);
        fwrite(&space, sizeof(char), 1, f);
      }
      if (CheckBox6->Checked)
      {
        str = FloatToStrF(CScheme::Magnitude(pAnalyseParameters->m_pT->m_T[i]), ffExponent, nPrecision, nDigits);
        fwrite(str.c_str(), sizeof(char), str.Length(), f);
        fwrite(&space, sizeof(char), 1, f);
      }
      if (CheckBox7->Checked)
      {
        // значени€ берутс€ из ранее рассчитанного массива
        str = FloatToStrF(pPhaseArray[i], ffExponent, nPrecision, nDigits);
        fwrite(str.c_str(), sizeof(char), str.Length(), f);
        fwrite(&space, sizeof(char), 1, f);
      }

      fwrite(&CRLF, sizeof(char), 1, f);
    } // end for
    fclose(f);
    delete[] pPhaseArray;
  } // end (f)
  else
  {
    ShowMessage("Ќевозможно открыть указанный файл дл€ записи");
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm9::FormActivate(TObject *Sender)
{
  nCheckBoxArraySize = Form1->Structure5->GetKPQuantity();

  pCheckBoxArray = new TCheckBox**[nCheckBoxArraySize];
  for (int i=0; i<nCheckBoxArraySize; i++)
    pCheckBoxArray[i] = new TCheckBox*[nCheckBoxArraySize];

  for (int y=0; y<nCheckBoxArraySize; y++)
  {
    for (int x=y; x<nCheckBoxArraySize; x++)
    {
      pCheckBoxArray[y][x] = new TCheckBox(this);
      pCheckBoxArray[y][x]->Parent = GroupBox2;
      pCheckBoxArray[y][x]->Width = 35;
      pCheckBoxArray[y][x]->Height = 15;
      pCheckBoxArray[y][x]->Left = 15 + 42*x;
      pCheckBoxArray[y][x]->Top = 20 + 42*y;
      pCheckBoxArray[y][x]->Caption = "y" + IntToStr(y+1) + IntToStr(x+1);
    }
  }
}
//---------------------------------------------------------------------------


void __fastcall TForm9::FormClose(TObject *Sender, TCloseAction &Action)
{
  for (int i=0; i<nCheckBoxArraySize; i++)
    delete[] pCheckBoxArray[i];
  delete[] pCheckBoxArray;
}
//---------------------------------------------------------------------------

