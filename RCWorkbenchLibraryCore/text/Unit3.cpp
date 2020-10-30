//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "Unit1.h"
#include "Unit3.h"
#include "Unit7.h"
#include "Unit8.h"

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm3 *Form3;
extern CTargetFunction *pTarget;
extern int nPrecision, nDigits;

//---------------------------------------------------------------------------
__fastcall TForm3::TForm3(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------

// нажатие на кнопку "СОХРАНЕНИЕ РЕЗУЛЬТАТОВ"--------------------------------
void __fastcall TForm3::Button2Click(TObject *Sender)
{
  int nTargetSize = StringGrid1->RowCount-2;

  delete pTarget;
  pTarget = NULL;
  if (nTargetSize)
    pTarget = new CTargetFunction(nTargetSize);

  for (int i=0; i<nTargetSize; i++)
  {
    pTarget->m_w[i] = StrToFloat(StringGrid1->Cells[0][i+1]);

    if (StringGrid1->Cells[1][i+1].Length() > 1)
    {
      pTarget->m_low[i] = StrToFloat(StringGrid1->Cells[1][i+1]);
      pTarget->m_en_low[i] = True;
    }
    else
      pTarget->m_en_low[i] = False;

    if (StringGrid1->Cells[2][i+1].Length() > 1)
    {
      pTarget->m_high[i] = StrToFloat(StringGrid1->Cells[2][i+1]);
      pTarget->m_en_high[i] = True;
    }
    else
      pTarget->m_en_high[i] = False;
  }
  Series1->Clear();
  Form1->Series1->Clear();
  for (int i=0; i<nTargetSize-1; i++)
  {
    if (pTarget->m_en_high[i+1] && pTarget->m_en_high[i])
    {
      Series1->AddArrow(pTarget->m_w[i], pTarget->m_high[i], pTarget->m_w[i+1], pTarget->m_high[i+1]);
      Form1->Series1->AddArrow(pTarget->m_w[i], pTarget->m_high[i], pTarget->m_w[i+1], pTarget->m_high[i+1]);
    }
    if (pTarget->m_en_low[i+1] && pTarget->m_en_low[i])
    {
      Series1->AddArrow(pTarget->m_w[i], pTarget->m_low[i], pTarget->m_w[i+1], pTarget->m_low[i+1]);
      Form1->Series1->AddArrow(pTarget->m_w[i], pTarget->m_low[i], pTarget->m_w[i+1], pTarget->m_low[i+1]);
    }
  }
  Form1->RecalculateSynthesisGraph();
}
//---------------------------------------------------------------------------

// нажатие на кнопку "ВЫХОД ИЗ РЕДАКТОРА"------------------------------------
void __fastcall TForm3::Button3Click(TObject *Sender)
{
  Close();
}
//---------------------------------------------------------------------------

//Вспомогательная функция сортировки
static int __fastcall CompareUp(void * Item1, void * Item2)
{
  if (((TMyData *)Item1)->S.ToDouble() < ((TMyData *)Item2)->S.ToDouble())
    return -1;
  else if (((TMyData *)Item1)->S.ToDouble() > ((TMyData *)Item2)->S.ToDouble())
    return 1;
  else
    return 0;
}
//---------------------------------------------------------------------------

//Сортировка строк в StringGrid
static void __fastcall Sort(TStringGrid *SG)
{
  int i,j,ARow;
  TList *SortList=new TList; //Список для сортировки
  TList *TempList=new TList; //Список для установки строк на новое место
  //Заполним список содержимым колонки ACol
  for(ARow=SG->FixedRows; ARow < SG->RowCount; ARow++)
  {
    TMyData *md=new TMyData;
    md->ARow=ARow;
    md->S=SG->Cells[0][ARow];
    SortList->Add(md);
    //Заполним TempList
    TempList->Add((void *)ARow); //Запомним старую расстановку строк
  }
  SortList->Sort(CompareUp); //Отсортируем по возрастающей

  for(i=0; i < SortList->Count; i++)
  {
    TMyData *md=(TMyData *)(SortList->Items[i]);
    //Возьмём номер строки в начальной расстановке
    ARow=md->ARow;
    //Найдём в каком месте сейчас находится бывшая строка ARow
    for(j=0; j < TempList->Count; j++)
    {
      if(ARow==(int)TempList->Items[j])break;
    }
    //Сейчас строка находится на месте j+FixedRows
    //Надо её переставить на место i+FixedRows
    AnsiString Sold=SG->Rows[j+SG->FixedRows]->Text; //Прежнее положение строки
    AnsiString Snew=SG->Rows[i+SG->FixedRows]->Text; //Новое положение строки
    SG->Rows[j+SG->FixedRows]->Text=Snew;
    SG->Rows[i+SG->FixedRows]->Text=Sold;
    //Делаем параллельную перестановку в TempList
    int jold=(int)(TempList->Items[j]);
    int inew=(int)(TempList->Items[i]);
    TempList->Items[j]=(void *)inew;
    TempList->Items[i]=(void *)jold;
  }
  //Удалим списки 
  delete TempList; 
  for(i=0; i < SortList->Count; i++) 
  { 
    delete (TMyData *)(SortList->Items[i]); 
  } 
  delete SortList;
}
//---------------------------------------------------------------------------

void __fastcall TForm3::Button4Click(TObject *Sender)
{
  if (!Edit1->Text.IsEmpty() && Edit1->Text.ToDouble()>0.0)
  {
    if (StringGrid1->RowCount>2)
    {
      for (int i=1; i<StringGrid1->RowCount-1; i++)
      {
        if (fabs(StringGrid1->Cells[0][i].ToDouble()-Edit1->Text.ToDouble())<0.001)
        {
          StringGrid1->Cells[0][i]=FloatToStrF(Edit1->Text.ToDouble(), ffFixed, nPrecision, nDigits);
          if (Edit2->Text.IsEmpty())
            StringGrid1->Cells[2][i]="";
          else
            StringGrid1->Cells[2][i]=FloatToStrF(Edit2->Text.ToDouble(), ffFixed, nPrecision, nDigits);

          if (Edit3->Text.IsEmpty())
            StringGrid1->Cells[1][i]="";
          else
            StringGrid1->Cells[1][i]=FloatToStrF(Edit3->Text.ToDouble(), ffFixed, nPrecision, nDigits);

          return;
        }
      }
    }
    StringGrid1->Cells[0][StringGrid1->RowCount-1]=FloatToStrF(Edit1->Text.ToDouble(), ffFixed, nPrecision, nDigits);
    if (!Edit2->Text.IsEmpty())
      StringGrid1->Cells[2][StringGrid1->RowCount-1]=FloatToStrF(Edit2->Text.ToDouble(), ffFixed, nPrecision, nDigits);
    if (!Edit3->Text.IsEmpty())
      StringGrid1->Cells[1][StringGrid1->RowCount-1]=FloatToStrF(Edit3->Text.ToDouble(), ffFixed, nPrecision, nDigits);
    Sort(StringGrid1);
    StringGrid1->RowCount++;
  }
}
//---------------------------------------------------------------------------

// нажатие на кнопку "Очистить"-----
void __fastcall TForm3::Button5Click(TObject *Sender)
{
  Form1->SMTargetFunction->Checked = False;
  for (int i=StringGrid1->RowCount-2; i>0; i--)
    StringGrid1->Rows[i]->Clear();
  StringGrid1->RowCount=2;
  Series1->Clear();
}
//---------------------------------------------------------------------------

// нажатие на кнопку "ФИЛЬТР"-----
void __fastcall TForm3::Button1Click(TObject *Sender)
{
  Form7->ShowModal();
  if (Form7->ModalResult==mrOk)
  {
    Sort(StringGrid1);
    StringGrid1->RowCount++;
    Button2Click(Button2);
    Close();
  }
}
//---------------------------------------------------------------------------

// нажатие на кнопку "ОКНО"-----
void __fastcall TForm3::Button6Click(TObject *Sender)
{
  Form8->ShowModal();
  if (Form8->ModalResult==mrOk)
  {
    Sort(StringGrid1);
    StringGrid1->RowCount++;
    Button2Click(Button2);
    Close();
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm3::FormCreate(TObject *Sender)
{
  StringGrid1->Cells[0][0]=" w";
  StringGrid1->Cells[1][0]=" снизу";
  StringGrid1->Cells[2][0]=" сверху";
}
//---------------------------------------------------------------------------


