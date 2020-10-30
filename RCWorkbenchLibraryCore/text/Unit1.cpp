//---------------------------------------------------------------------------

#include <vcl.h>
#include <process.h>
#pragma hdrstop

#include "Unit1.h"
#include "Unit2.h"
#include "Unit3.h"
#include "Unit4.h"
#include "Unit5.h"
#include "Unit6.h"
#include "Unit7.h"
#include "Unit8.h"
#include "Unit9.h"
#include "Unit10.h"
#include "Unit11.h"
#include "Unit12.h"
#include "proots.h"
#include "RC0Structure.h"
#include "RCG0Structure.h"
#include "RCNRStructure.h"
#include "RCGNRStructure.h"
#include "RCGNRAStructure.h"

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "CSPIN"
#pragma resource "*.dfm"
TForm1 *Form1;

bool bIsNotBatch = true;
bool bMouseInited = false;

CAnalyseParameters *pAnalyseParameters = NULL;
CRCStructure **pIslandStructures;
CScheme *pScheme = NULL;
CTargetFunction *pTarget = NULL;
TThreadGA *pTGA = NULL;
CGASettings *pGASettings;

double dDeviation = 100000.0;
double dLastDeviation = 100000.0;
double dMeanValue = 0.0;
double dParam1 = 0.1;
double dIslandDeviation = 0.1;

int nCharacteristic;
int nGACount = 0;  // Счётчик цикла ГА
int nMaxBatchGACount = 3000;
int nTime0, nTime_accumulated = 0;
int nPrecision = 6;
int nDigits = 4;
int napr = 0;
EnumRCElementType SelectedElementType = RCET_NONE;
unsigned int nIslandStructuresCount = 0;


#ifdef _DEBUG
int nThreads = 1;
#else
int nThreads = 4; // 4
#endif

//---------------------------------------------------------------------------
__fastcall TForm1::TForm1(TComponent* Owner)
        : TForm(Owner)
{
  DoubleBuffered = true;
  DecimalSeparator='.';
  SpeedButton6->AllowAllUp = true;
  SpeedButton6->Glyph->TransparentColor = clWhite;
  SpeedButton6->Glyph->LoadFromResourceName((int)HInstance, "PLAYICON");
  StructureImage = new Graphics::TBitmap();
}
//---------------------------------------------------------------------------

void __fastcall TForm1::OnChartAfterDraw(TObject *Sender)
{
  bMouseInited = false;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::FormDestroy(TObject *Sender)
{
  // посылаем команду остановить потоки
  SetEvent(hStopCompletionPort);
  // ждем максимум 10 секунд пока потоки завершатся
  WaitForMultipleObjects(nThreads, phThreads, true, 10000);
  for (int i=0; i<nThreads; ++i)
    CloseHandle(phThreads[i]);
  delete[] phThreads;
  CloseHandle(hCompletionPort);
  while (nIslandStructuresCount)
    delete pIslandStructures[--nIslandStructuresCount];
  delete[] pIslandStructures;

  // т.к. потоки могут работать еще некоторое время, то структуры удаляем последними
  delete StructureImage;
  StructureImage = NULL;
  delete pGASettings;
  delete pTarget;
  delete pScheme;
  delete pAnalyseParameters;
  delete Structure1;
  delete Structure5;

  CloseHandle(hStopCompletionPort);
}
//---------------------------------------------------------------------------

// рисование структуры
void TForm1::ShowStructure(CRCStructure *Structure, int Layer, TImage *Target)
{
  Graphics::TBitmap *Source = StructureImage;
  int nImageSize = Source->Width;
  PatBlt(Source->Canvas->Handle, 0, 0, nImageSize, nImageSize, WHITENESS);
  Source->Canvas->Pen->Color=clBlack;
  Source->Canvas->Brush->Color=clWhite;
  Source->Canvas->Rectangle(0, 0, nImageSize, nImageSize);
  int x = Structure->Width();
  int y = Structure->Height();
  int Perimeter = Structure->Perimeter();
  int nElementSizeX = MyMin(Source->Width/(x+2), Source->Height/((double)x/Structure->GetKf()+2.0));
  int nElementSizeY = nElementSizeX*(double)x/((double)y*Structure->GetKf());
  int nElementSize_2 = nElementSizeX >> 1;
  int nImageLeft = (Source->Width  - nElementSizeX*x) >> 1;
  int nImageTop  = (Source->Height - nElementSizeY*y) >> 1;
  int nLeft = nImageLeft;
  int nTop;
  int nLeftKP = nImageLeft - nElementSize_2;
  int nTopKP  = nImageTop  - nElementSize_2;
  int nKPNumber, nKPType;

  // рисуем ячейки
  for (int i=0; i<x; ++i)
  {
    nTop = nImageTop;
    for (int j=0; j<y; ++j)
    {
      if (Structure->GetElementTypeMask(i,j,Layer))
        Source->Canvas->Brush->Style = bsDiagCross;
      else
        Source->Canvas->Brush->Style = bsSolid;

      Source->Canvas->Brush->Color = GetElementColor(Structure->GetElementType(i,j,Layer));
      PatBlt(Source->Canvas->Handle, nLeft, nTop, nElementSizeX, nElementSizeY, PATCOPY);
      #ifdef _DEBUG
      Source->Canvas->TextOutA(nLeft+2, nTop+2, Structure->GetElementRawValue(i,j));
      #endif
      nTop += nElementSizeY;
    }
    nLeft += nElementSizeX;
  }
  Source->Canvas->Brush->Style = bsSolid; // чтобы контактные площадки рисовались нормально после рисованиея маски на ячейках

  int i=0;

  // рисуем контактные площадки сверху
  nLeft = nImageLeft;

  while (i<x)
  {
    Source->Canvas->Brush->Color = GetKPColor(Structure->GetKPType(Layer, i));
    PatBlt(Source->Canvas->Handle, nLeft, nTopKP, nElementSizeX, nElementSize_2, PATCOPY);
    nKPNumber = Structure->GetKPNumber(Layer, i);
    if (nKPNumber)
      Source->Canvas->TextOut(nLeft+nElementSize_2-3,nTopKP+(nElementSize_2>>1)-5,IntToStr(nKPNumber));

    ++i;
    nLeft += nElementSizeX;
  }
   
  // рисуем уголок в правом-верхнем углу
  nKPType = Structure->GetKPType(Layer, x);
  if (nKPType != KPNONE && Structure->GetKPType(Layer, x-1) == nKPType)
  {
    Source->Canvas->Brush->Color = GetKPColor(nKPType);
    PatBlt(Source->Canvas->Handle, nLeft, nTopKP, nElementSize_2, nElementSize_2, PATCOPY);
  }

  // рисуем контактные площадки справа
  nTop = nImageTop;
  while (i<x+y)
  {
    Source->Canvas->Brush->Color = GetKPColor(Structure->GetKPType(Layer, i));
    PatBlt(Source->Canvas->Handle, nLeft, nTop, nElementSize_2, nElementSizeY, PATCOPY);
    nKPNumber = Structure->GetKPNumber(Layer, i);
    if (nKPNumber)
      Source->Canvas->TextOut(nLeft+(nElementSize_2>>1)-3,nTop+(nElementSizeY>>1)-5,IntToStr(nKPNumber));

    ++i;
    nTop += nElementSizeY;
  }

  // рисуем уголок в правом-нижнем углу
  nKPType = Structure->GetKPType(Layer, x+y);
  if (nKPType != KPNONE && Structure->GetKPType(Layer, x+y-1) == nKPType)
  {
    Source->Canvas->Brush->Color = GetKPColor(nKPType);
    PatBlt(Source->Canvas->Handle, nLeft, nTop, nElementSize_2, nElementSize_2, PATCOPY);
  }

  // рисуем контактные площадки снизу
  nLeft -= nElementSizeX;
  while (i<x+y+x)
  {
    Source->Canvas->Brush->Color = GetKPColor(Structure->GetKPType(Layer, i));
    PatBlt(Source->Canvas->Handle, nLeft, nTop, nElementSizeX, nElementSize_2, PATCOPY);
    nKPNumber = Structure->GetKPNumber(Layer, i);
    if (nKPNumber)
      Source->Canvas->TextOut(nLeft+nElementSize_2-3,nTop+(nElementSize_2>>1)-5,IntToStr(nKPNumber));

    ++i;
    nLeft -= nElementSizeX;
  }

  // рисуем уголок в левом-нижнем углу
  nKPType = Structure->GetKPType(Layer, Perimeter-y);
  if (nKPType != KPNONE && Structure->GetKPType(Layer, Perimeter-y-1) == nKPType)
  {
    Source->Canvas->Brush->Color = GetKPColor(nKPType);
    PatBlt(Source->Canvas->Handle, nLeftKP, nTop, nElementSize_2, nElementSize_2, PATCOPY);
  }

  // рисуем контактные площадки слева
  nTop -= nElementSizeY;
  while (i<Perimeter)
  {
    Source->Canvas->Brush->Color = GetKPColor(Structure->GetKPType(Layer, i));
    PatBlt(Source->Canvas->Handle, nLeftKP, nTop, nElementSize_2, nElementSizeY, PATCOPY);
    nKPNumber = Structure->GetKPNumber(Layer, i);
    if (nKPNumber)
      Source->Canvas->TextOut(nLeftKP+(nElementSize_2>>1)-3,nTop+(nElementSizeY>>1)-5,IntToStr(nKPNumber));

    ++i;
    nTop -= nElementSizeY;
  }

  // рисуем уголок в левом-верхнем углу
  nKPType = Structure->GetKPType(Layer, 0);
  if (nKPType != KPNONE && Structure->GetKPType(Layer, Perimeter-1) == nKPType)
  {
    Source->Canvas->Brush->Color = GetKPColor(nKPType);
    PatBlt(Source->Canvas->Handle, nLeftKP, nTopKP, nElementSize_2, nElementSize_2, PATCOPY);
  }

  // рисуем сетку
  nLeft = nImageLeft+nElementSizeX*x+nElementSize_2;
  for (int i=0; i<=y; ++i)
  {
    nTop += nElementSizeY;
    Source->Canvas->MoveTo(nLeftKP, nTop);
    Source->Canvas->LineTo(nLeft, nTop);
  }

  nTop += nElementSize_2;
  nLeft -= nElementSize_2;
  for (int j=0; j<=x; ++j)
  {
    Source->Canvas->MoveTo(nLeft, nTopKP);
    Source->Canvas->LineTo(nLeft, nTop);
    nLeft -= nElementSizeX;
  }

  nLeft = nImageLeft+nElementSizeX*x+nElementSize_2;
  Source->Canvas->MoveTo(nLeftKP, nTopKP);
  Source->Canvas->LineTo(nLeft, nTopKP);
  Source->Canvas->LineTo(nLeft, nTop);
  Source->Canvas->LineTo(nLeftKP, nTop);
  Source->Canvas->LineTo(nLeftKP, nTopKP);

  BitBlt(Target->Canvas->Handle, 0, 0, nImageSize, nImageSize, Source->Canvas->Handle, 0, 0, SRCCOPY);
  Target->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TForm1::FMNewClick(TObject *Sender)
{
  Form2->ShowModal();
  if (Form2->ModalResult==mrOk)
  {
    if (MMAnalysis->Checked)
    {
      StatusBar1->Panels->Items[0]->Text = ""; // цикл ГА
      StatusBar1->Panels->Items[1]->Text = ""; // отклонение
      StatusBar1->Panels->Items[2]->Text = ""; // время
      StatusBar1->Panels->Items[3]->Text = ""; // островная популяция
      Series4->Clear();
      delete Structure5;
      TabControl3->Tabs->Clear();

      if (Form2->RadioButton1->Checked)
      {
        Structure5 = new CRC0Structure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble());
        TabControl3->Tabs->Add("R-C-слой");
      }
      else if (Form2->RadioButton2->Checked)
      {
        //NEW
        Structure5 = new CRCG0Structure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble(), Form2->Edit5->Text.ToDouble(), Form2->Edit6->Text.ToDouble());
        TabControl3->Tabs->Add("R-CG-слой");
      }
      else if (Form2->RadioButton3->Checked)
      {
        Structure5 = new CRCNRStructure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble(), Form2->Edit4->Text.ToDouble());
        TabControl3->Tabs->Add("R-C-слой");
        TabControl3->Tabs->Add("NR-слой");
      }
      else if (Form2->RadioButton4->Checked)
      {
        Structure5 = new CRCGNRStructure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble(), Form2->Edit5->Text.ToDouble(), Form2->Edit6->Text.ToDouble(), Form2->Edit4->Text.ToDouble());
        TabControl3->Tabs->Add("R-CG-слой");
        TabControl3->Tabs->Add("NR-слой");
      }
      else if (Form2->RadioButton5->Checked)
      {
        Structure5 = new CRCGNRAStructure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble(), Form2->Edit5->Text.ToDouble(), Form2->Edit6->Text.ToDouble(), Form2->Edit4->Text.ToDouble(), Form2->Edit7->Text.ToDouble());
        TabControl3->Tabs->Add("R-CG-слой");
        TabControl3->Tabs->Add("NR-слой");
      }
      else if (Form2->RadioButton6->Checked)
      {
        Structure5 = new CRRCNRStructure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble(), Form2->Edit4->Text.ToDouble(), Form2->Edit8->Text.ToDouble());
        TabControl3->Tabs->Add("R-C-слой");
        TabControl3->Tabs->Add("NR-слой");
      }
      else if (Form2->RadioButton7->Checked)
      {
        Structure5 = new CRRCGNRStructure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble(), Form2->Edit5->Text.ToDouble(), Form2->Edit6->Text.ToDouble(), Form2->Edit4->Text.ToDouble(), Form2->Edit8->Text.ToDouble());
        TabControl3->Tabs->Add("R-CG-слой");
        TabControl3->Tabs->Add("NR-слой");
      }
      else if (Form2->RadioButton8->Checked)
      {
        Structure5 = new CRRCGNRAStructure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble(), Form2->Edit5->Text.ToDouble(), Form2->Edit6->Text.ToDouble(), Form2->Edit4->Text.ToDouble(), Form2->Edit7->Text.ToDouble(), Form2->Edit8->Text.ToDouble());
        TabControl3->Tabs->Add("R-CG-слой");
        TabControl3->Tabs->Add("NR-слой");
      }
      else
      {
        ShowMessage("Неизвестный тип структуры");
      }

      TabControl1->Tabs->Clear();
      TabControl1->Tabs->Add(ComboBox1->Text+" - "+ComboBox2->Text);
      TabControl1->Tabs->Add("Годограф");
      //UpdateParametersList();

      ShowStructure(Structure5, TabControl3->TabIndex, Image2);
    }
    else
    {
      nGACount = 0;
      nTime_accumulated = 0;
      StatusBar1->Panels->Items[0]->Text = " Цикл ГА: 0";
      dDeviation = 100000.0;
      StatusBar1->Panels->Items[1]->Text = " Отклонение: 100000";
      StatusBar1->Panels->Items[2]->Text = ""; // время
      StatusBar1->Panels->Items[3]->Text = ""; // островная популяция

      Series2->Clear();
      Series3->Clear();

      if (pTarget)
      {
        memset(pTarget->m_char[1], 0, sizeof(double)*pTarget->m_length);
        memset(pTarget->m_char[2], 0, sizeof(double)*pTarget->m_length);
      }

      delete Structure1;

      TabControl2->Tabs->Clear();

      if (Form2->RadioButton1->Checked)
      {
        Structure1 = new CRC0Structure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble());
        TabControl2->Tabs->Add("R-C-слой");
      }
      else if (Form2->RadioButton2->Checked)
      {
        Structure1 = new CRCG0Structure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble(), Form2->Edit5->Text.ToDouble(), Form2->Edit6->Text.ToDouble());
        TabControl2->Tabs->Add("R-CG-слой");
      }
      else if (Form2->RadioButton3->Checked)
      {
        Structure1 = new CRCNRStructure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble(), Form2->Edit4->Text.ToDouble());
        TabControl2->Tabs->Add("R-C-слой");
        TabControl2->Tabs->Add("NR-слой");
      }
      else if (Form2->RadioButton4->Checked)
      {
        Structure1 = new CRCGNRStructure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble(), Form2->Edit5->Text.ToDouble(), Form2->Edit6->Text.ToDouble(), Form2->Edit4->Text.ToDouble());
        TabControl2->Tabs->Add("R-CG-слой");
        TabControl2->Tabs->Add("NR-слой");
      }
      else if (Form2->RadioButton5->Checked)
      {
        Structure1 = new CRCGNRAStructure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble(), Form2->Edit5->Text.ToDouble(), Form2->Edit6->Text.ToDouble(), Form2->Edit4->Text.ToDouble(), Form2->Edit7->Text.ToDouble());
        TabControl2->Tabs->Add("R-CG-слой");
        TabControl2->Tabs->Add("NR-слой");
      }
      else if (Form2->RadioButton6->Checked)
      {
        Structure1 = new CRRCNRStructure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble(), Form2->Edit4->Text.ToDouble(), Form2->Edit8->Text.ToDouble());
        TabControl2->Tabs->Add("R-C-слой");
        TabControl2->Tabs->Add("NR-слой");
      }
      else if (Form2->RadioButton7->Checked)
      {
        Structure1 = new CRRCGNRStructure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble(), Form2->Edit5->Text.ToDouble(), Form2->Edit6->Text.ToDouble(), Form2->Edit4->Text.ToDouble(), Form2->Edit8->Text.ToDouble());
        TabControl2->Tabs->Add("R-CG-слой");
        TabControl2->Tabs->Add("NR-слой");
      }
      else if (Form2->RadioButton8->Checked)
      {
        Structure1 = new CRRCGNRAStructure(Form2->Edit9->Text.ToDouble(), Form2->Edit10->Text.ToDouble(), Form2->Edit1->Text.ToInt(), Form2->Edit2->Text.ToInt(), Form2->Edit3->Text.ToDouble(), Form2->Edit5->Text.ToDouble(), Form2->Edit6->Text.ToDouble(), Form2->Edit4->Text.ToDouble(), Form2->Edit7->Text.ToDouble(), Form2->Edit8->Text.ToDouble());
        TabControl2->Tabs->Add("R-CG-слой");
        TabControl2->Tabs->Add("NR-слой");
      }
      else
      {
        ShowMessage("Неизвестный тип структуры");
      }

      ShowStructure(Structure1, TabControl2->TabIndex, Image1);
    }
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::FormResize(TObject *Sender)
{
  if (StructureImage)
  {
    int nImageSizeX;
    int nImageSizeY;
    if (MMAnalysis->Checked)
    {
      nImageSizeX = ClientWidth - Panel1->Width;
      nImageSizeY = ClientHeight - StatusBar1->Height;

      TabControl1->Left = 0;
      TabControl1->Top  = 0;
      TabControl1->Width =nImageSizeX;
      TabControl1->Height=nImageSizeY;
      Chart2->Width =nImageSizeX;
      Chart2->Height=nImageSizeY-21;
      TabControl3->Left = 0;
      TabControl3->Top  = 0;
      TabControl3->Width =nImageSizeX;
      TabControl3->Height=nImageSizeY;
      Image2->Left  =0;
      Image2->Top   =0;
      Image2->Width =nImageSizeX;
      Image2->Height=nImageSizeY-21;

      StructureImage->Width =nImageSizeX;
      StructureImage->Height=nImageSizeY-21;

      SpeedButton1->Top=0;
      SpeedButton2->Top=0;
      SpeedButton3->Top=0;
      SpeedButton4->Top=0;
      SpeedButton5->Top=0;
      SpeedButton7->Top=0;
      SpeedButton8->Top=0;
      SpeedButton1->Left = nImageSizeX + 9;
      SpeedButton2->Left = SpeedButton1->Left + SpeedButton1->Width;
      SpeedButton3->Left = SpeedButton2->Left + SpeedButton2->Width;
      SpeedButton4->Left = SpeedButton3->Left + SpeedButton3->Width;
      SpeedButton5->Left = SpeedButton4->Left + SpeedButton4->Width;
      SpeedButton7->Left = SpeedButton5->Left + SpeedButton5->Width;
      SpeedButton8->Left = SpeedButton7->Left + SpeedButton7->Width;
      Panel1->Top = SpeedButton1->Height;
      Panel1->Left = nImageSizeX;

      if (Structure5)
        ShowStructure(Structure5, TabControl3->TabIndex, Image2);
    }
    else
    {
      int nImageSizeX = MyMin(ClientWidth>>1, ClientHeight-100);
      nImageSizeY = nImageSizeX;

      Image1->Left  =0;
      Image1->Top   =0;
      Image1->Width =nImageSizeX;
      Image1->Height=nImageSizeY;
      TabControl2->Left =0;
      TabControl2->Top  =0;
      TabControl2->Width =nImageSizeX;
      TabControl2->Height=nImageSizeY+20;
      Chart1->Left  =nImageSizeX;
      Chart1->Top   =0;
      Chart1->Width =nImageSizeX;
      Chart1->Height=nImageSizeY;

      StructureImage->Width =nImageSizeX;
      StructureImage->Height=nImageSizeY;

      SpeedButton1->Top = nImageSizeY+20;
      SpeedButton2->Top = SpeedButton1->Top;
      SpeedButton3->Top = SpeedButton1->Top;
      SpeedButton4->Top = SpeedButton1->Top;
      SpeedButton5->Top = SpeedButton1->Top;
      SpeedButton7->Top = SpeedButton1->Top;
      SpeedButton8->Top = SpeedButton1->Top;
      SpeedButton1->Left = 0;
      SpeedButton2->Left = SpeedButton1->Left + SpeedButton1->Width;
      SpeedButton3->Left = SpeedButton2->Left + SpeedButton2->Width;
      SpeedButton4->Left = SpeedButton3->Left + SpeedButton3->Width;
      SpeedButton5->Left = SpeedButton4->Left + SpeedButton4->Width;
      SpeedButton7->Left = SpeedButton5->Left + SpeedButton5->Width;
      SpeedButton8->Left = SpeedButton7->Left + SpeedButton7->Width;
      SpeedButton6->Top = ClientHeight-65;
      SpeedButton6->Left = ClientWidth-115;

      ShowStructure(Structure1, TabControl2->TabIndex, Image1);
    }
  }
}
//---------------------------------------------------------------------------

unsigned int __stdcall TForm1::WorkerThread(LPVOID CompletionPortID)
{
  unsigned long numOfBytes;
  unsigned long key;
  OVERLAPPED *poverlapped;
  CJob *pJob;

  while (WaitForSingleObject(hStopCompletionPort, 0) == WAIT_TIMEOUT)
  {
    if (GetQueuedCompletionStatus((HANDLE)CompletionPortID, &numOfBytes, &key, &poverlapped, 1000))
    {
      pJob = (CJob*)poverlapped;
      if ((WaitForSingleObject(pJob->isBadStructureEvent, 0) == WAIT_TIMEOUT) && (pJob->Structure->YParameters(pJob->Data->m_w[key], pJob->Data->m_MatY[key]) == -1))
        SetEvent(pJob->isBadStructureEvent);
      SetEvent(pJob->isDoneEvents[key]);
    }
  }
  return 0;
}

void __fastcall TForm1::FormCreate(TObject *Sender)
{
  pGASettings = new CGASettings();

  // подставляем в заголовок формы номер версии программы
  TResourceStream *pResource = new TResourceStream((int)HInstance, "#1", RT_VERSION);
  char *pVersionInfo = new char[pResource->Size];
  memcpy(pVersionInfo, pResource->Memory, pResource->Size);
  char *pBuff;
  unsigned int size;
  AnsiString FileVersion = "\\StringFileInfo\\041904E3\\FileVersion";
  VerQueryValue(pVersionInfo, FileVersion.c_str(), (void**)&pBuff, &size);
  FileVersion = AnsiString(pBuff);
  Caption = Caption + "  (версия " + FileVersion + ")";
  delete[] pVersionInfo;
  delete pResource;

  // создаем начальные структуры
  Structure1 = new CRC0Structure(1.0, 1.0, 10, 10, 1.0);
  TabControl2->Tabs->Add("R-C-слой");

  // обновляем размеры формы
  FormResize(this);

  // определяем количество процессоров
  SYSTEM_INFO si;
  GetSystemInfo(&si);
  int nProcessors = si.dwNumberOfProcessors;
  if (nThreads > nProcessors)
    nThreads = nProcessors;

  // создаем порт завершения
  hCompletionPort = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, nThreads);

  // используюя этот эвент, потоки поймут, что надо завершиться
  hStopCompletionPort = CreateEvent(NULL, true, false, NULL);

  // создаем потоки-обработчики заданий
  phThreads = new HANDLE[nThreads];
  for (int i=0; i<nThreads; ++i)
  {
    phThreads[i] = (HANDLE)_beginthreadex(NULL, 0, TForm1::WorkerThread, hCompletionPort, 0, NULL);
    SetThreadPriority(phThreads[i], THREAD_PRIORITY_LOWEST);
    if (nThreads == nProcessors)
      SetThreadIdealProcessor(phThreads[i], i);
  }

  pIslandStructures = new CRCStructure*[100];
}
//---------------------------------------------------------------------------

void __fastcall TForm1::FormActivate(TObject *Sender)
{
  ChangeProgramMode(MMAnalysis);
  //ChangeProgramMode(MMSynthesis);
  //FMNewClick(this);
  FMOpenClick(this);
}
//---------------------------------------------------------------------------

void TForm1::ImageClick(CRCStructure *Structure, int Layer, TImage *Target, int X, int Y)
{
  Graphics::TBitmap *Source = StructureImage;
  int i, j;
  int x = Structure->Width();
  int y = Structure->Height();
  int nElementSizeX = MyMin(Source->Width/(x+2), Source->Height/((double)x/Structure->GetKf()+2.0));
  int nElementSizeY = nElementSizeX*(double)x/(double)y/Structure->GetKf();
  int nImageLeft = (Source->Width  - nElementSizeX*x) >> 1;
  int nImageTop  = (Source->Height - nElementSizeY*y) >> 1;
  if (X<nImageLeft)
    i = -1;
  else
    i = (X-nImageLeft)/nElementSizeX;
  if (Y<nImageTop)
    j = -1;
  else
    j = (Y-nImageTop)/nElementSizeY;

  bool bChanged;
  if (!MMAnalysis->Checked && SMEditMask->Checked)
    bChanged = Structure->SetElementType(Layer, i, j, SelectedElementType + RCET_MASKEMPTY);
  else
    bChanged = Structure->SetElementType(Layer, i, j, SelectedElementType);

  if (SelectedElementType == RCET_KPNORMAL)
    Structure->ClearKPNumbers();
  //Structure1->AutonumerateKP();
  if (bChanged)
    ShowStructure(Structure, Layer, Target);
}
//---------------------------------------------------------------------------


void __fastcall TForm1::SpeedButton1Click(TObject *Sender)
{
  SpeedButton6Click(SpeedButton6);
  SelectedElementType = RCET_NONE;
  if (MMAnalysis->Checked)
  {
    TabControl3->Visible = True;
    TabControl1->Visible = False;
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SpeedButton2Click(TObject *Sender)
{
  SpeedButton6Click(SpeedButton6);
  SelectedElementType = RCET_EMPTY;
  if (MMAnalysis->Checked)
  {
    TabControl3->Visible = True;
    TabControl1->Visible = False;
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SpeedButton3Click(TObject *Sender)
{
  SpeedButton6Click(SpeedButton6);
  SelectedElementType = RCET_RC;
  if (MMAnalysis->Checked)
  {
    TabControl3->Visible = True;
    TabControl1->Visible = False;
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SpeedButton4Click(TObject *Sender)
{
  SpeedButton6Click(SpeedButton6);
  SelectedElementType = RCET_R;
  if (MMAnalysis->Checked)
  {
    TabControl3->Visible = True;
    TabControl1->Visible = False;
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SpeedButton5Click(TObject *Sender)
{
  SpeedButton6Click(SpeedButton6);
  SelectedElementType = RCET_KPNORMAL;
  if (MMAnalysis->Checked)
  {
    TabControl3->Visible = True;
    TabControl1->Visible = False;
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SpeedButton7Click(TObject *Sender)
{
  SpeedButton6Click(SpeedButton6);
  SelectedElementType = RCET_KPRESTRICT;
  if (MMAnalysis->Checked)
  {
    TabControl3->Visible = True;
    TabControl1->Visible = False;
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SpeedButton8Click(TObject *Sender)
{
  SpeedButton6Click(SpeedButton6);
  SelectedElementType = RCET_KPSHUNT;
  if (MMAnalysis->Checked)
  {
    TabControl3->Visible = True;
    TabControl1->Visible = False;
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SpeedButton6Click(TObject *Sender)
{
  SelectedElementType = RCET_NONE;
  if (SpeedButton6->Down)
  {
    if (pTarget)
    {
      if (true || pScheme)
      {
        if (Structure1->CheckKP())
        {
          if (pScheme == NULL || Structure1->GetKPQuantity()==pScheme->GetKPQuantity())
          {
            SpeedButton6->Glyph->LoadFromResourceName((int)HInstance, "STOPICON");
            SpeedButton6->Caption = "Стоп";
            SynthesisMenu->Enabled = false;
            StructureMenu->Enabled = false;

            //GenerateIslandPopulation();
            pTGA = new TThreadGA(&Structure1, 0.0, pGASettings);
            pTGA->OnTerminate = OnTGATerminate;
          }
          else
          {
            SpeedButton6->Down = false;
            ShowMessage("Количество КП структуры не равно количеству КП схемы");
          }
        }
        else
        {
          SpeedButton6->Down = false;
          ShowMessage("Есть непронумерованные контактные площадки");
        }
      }
      else
      {
        SpeedButton6->Down = false;
        ShowMessage("Не задана схема включения");
      }
    }
    else
    {
      SpeedButton6->Down = false;
      ShowMessage("Не задана целевая функция");
    }
  }
  else
  {
    if (pTGA)
    {
      pTGA->Terminate();
      pTGA = NULL;
    }
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SMProbabilitiesClick(TObject *Sender)
{
  Form4->ShowModal();
  if (Form4->ModalResult==mrOk)
  {
    pGASettings->SetProbabilities(Form4->MaskEdit1->Text.ToDouble(), Form4->MaskEdit2->Text.ToDouble(), Form4->MaskEdit3->Text.ToDouble(), Form4->MaskEdit4->Text.ToDouble(), Form4->MaskEdit5->Text.ToDouble(), Form4->MaskEdit6->Text.ToDouble(), Form4->MaskEdit11->Text.ToDouble(), Form4->MaskEdit12->Text.ToDouble());
    pGASettings->SetRestrictions(Form4->MaskEdit7->Text.ToInt(), Form4->MaskEdit8->Text.ToInt(), Form4->MaskEdit9->Text.ToInt(), Form4->MaskEdit10->Text.ToInt(), Form4->MaskEdit13->Text.ToInt(), Form4->MaskEdit14->Text.ToInt());
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SMOptionsClick(TObject *Sender)
{
  Form5->ShowModal();
  if (Form5->ModalResult==mrOk)
  {
    pGASettings->SetAutoRegulateK(Form5->CheckBox1->Checked, Form5->Edit5->Text.ToInt(), Form5->Edit2->Text.ToDouble(), Form5->Edit3->Text.ToDouble(), Form5->Edit4->Text.ToDouble());
    pGASettings->SetAutoRegulateN(Form5->CheckBox4->Checked, Form5->Edit14->Text.ToInt(), Form5->Edit10->Text.ToDouble(), Form5->Edit11->Text.ToDouble(), Form5->Edit12->Text.ToDouble());
    pGASettings->SetAutoRegulateWRC(Form5->CheckBox2->Checked, Form5->Edit9->Text.ToInt(), Form5->Edit6->Text.ToDouble(), Form5->Edit7->Text.ToDouble(), Form5->Edit8->Text.ToDouble());
    pGASettings->SetDirectSynthesis(Form5->CheckBox3->Checked, Form5->Edit13->Text.ToInt());
    pGASettings->SetNonImprovementThreshold(Form5->Edit1->Text.ToDouble());
    pGASettings->SetIslandModel(Form5->CheckBox5->Checked, Form5->Edit15->Text.ToInt(), Form5->Edit16->Text.ToDouble(), Form5->Edit17->Text.ToInt());
    pGASettings->SetDynamicGrid(Form5->CheckBox6->Checked);
  }
}
//---------------------------------------------------------------------------

void TForm1::Graph()
{
  Series1->Clear();
  Series2->Clear();
  Series3->Clear();
  Chart1->Title->Text->Clear();

  if (pTarget)
  {
    int i;
    for (i=0; i<pTarget->m_length-1; ++i)
    {
      if (pTarget->m_en_high[i+1] && pTarget->m_en_high[i])
        Series1->AddArrow(pTarget->m_w[i], pTarget->m_high[i], pTarget->m_w[i+1], pTarget->m_high[i+1]);
      if (pTarget->m_en_low[i+1] && pTarget->m_en_low[i])
        Series1->AddArrow(pTarget->m_w[i], pTarget->m_low[i], pTarget->m_w[i+1], pTarget->m_low[i+1]);
      Series2->AddXY(pTarget->m_w[i], pTarget->m_char[1][i]);
      Series3->AddXY(pTarget->m_w[i], pTarget->m_char[2][i]);
    }
    Series2->AddXY(pTarget->m_w[i], pTarget->m_char[1][i]);
    Series3->AddXY(pTarget->m_w[i], pTarget->m_char[2][i]);

    Chart1->Title->Text->Add("Kf: "+FloatToStr(Structure1->GetKf())+";  G: "+FloatToStr(Structure1->GetG())+";  H: "+FloatToStr(Structure1->GetH())+";  N: "+FloatToStr(Structure1->GetN()));
    if (pScheme != NULL)
    {
      Chart1->Title->Text->Add(pScheme->Params());
    }
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SMTargetFunctionClick(TObject *Sender)
{
  Form3->ShowModal();
}
//---------------------------------------------------------------------------


void __fastcall TForm1::FMOpenClick(TObject *Sender)
{
  if (OpenDialog1->Execute())
  {
    FILE *f = fopen(OpenDialog1->FileName.c_str(),"rb");
    if (f)
    {
      EnumRCStructureType RCType;
      fread(&RCType, sizeof(EnumRCStructureType), 1, f);

      if (MMAnalysis->Checked)
      {
        delete Structure5;

        TabControl3->Tabs->Clear();

        switch (RCType)
        {
          case RC0:
          {
            Structure5 = new CRC0Structure(f);
            TabControl3->Tabs->Add("R-C-слой");
          }
          break;
          case RCG0:
          {
            Structure5 = new CRCG0Structure(f);
            TabControl3->Tabs->Add("R-CG-слой");
          }
          break;
          case RCNR:
          {
            Structure5 = new CRCNRStructure(f);
            TabControl3->Tabs->Add("R-C-слой");
            TabControl3->Tabs->Add("NR-слой");
          }
          break;
          case RCGNR:
          {
            Structure5 = new CRCGNRStructure(f);
            TabControl3->Tabs->Add("R-CG-слой");
            TabControl3->Tabs->Add("NR-слой");
          }
          break;
          case RCGNRA:
          {
            Structure5 = new CRCGNRAStructure(f);
            TabControl3->Tabs->Add("R-CG-слой");
            TabControl3->Tabs->Add("NR-слой");
          }
          break;
          case RRCNR:
          {
            Structure5 = new CRRCNRStructure(f);
            TabControl3->Tabs->Add("R-C-слой");
            TabControl3->Tabs->Add("NR-слой");
          }
          break;
          case RRCGNR:
          {
            Structure5 = new CRRCGNRStructure(f);
            TabControl3->Tabs->Add("R-CG-слой");
            TabControl3->Tabs->Add("NR-слой");
          }
          break;
          case RRCGNRA:
          {
            Structure5 = new CRRCGNRAStructure(f);
            TabControl3->Tabs->Add("R-CG-слой");
            TabControl3->Tabs->Add("NR-слой");
          }
          break;
          default:
          {
            ShowMessage("Неизвестный тип структуры");
          }
        }

        // загружаем схему
        bool bLog;
        fread(&bLog, sizeof(bool), 1, f);
        Form1->RadioButton1->Checked = !bLog;
        Form1->RadioButton2->Checked = bLog;
        Chart1->BottomAxis->Logarithmic = bLog;
        int length;
        fread(&length, sizeof(int), 1, f);
        Form1->ComboBox1->ItemIndex = length;
        fread(&length, sizeof(int), 1, f);
        Form1->ComboBox2->ItemIndex = length;

        char buf[50];
        fread(&length, sizeof(int), 1, f);
        fread(buf, 1, length, f);
        Form1->Edit4->Text = buf;
        fread(&length, sizeof(int), 1, f);
        fread(buf, 1, length, f);
        Form1->Edit5->Text = buf;

        TabControl1->Tabs->Clear();
        TabControl1->Tabs->Add(Form1->ComboBox1->Text + " - " + Form1->ComboBox2->Text);
        TabControl1->Tabs->Add("Годограф");

        // загружаем целевую функцию
        int nTargetFormat, n;
        fread(&nTargetFormat, sizeof(int), 1, f);

        if (nTargetFormat==0)
        {
          fread(&n, sizeof(int), 1, f);
          fread(&length, sizeof(int), 1, f);
          fread(&buf, 1, length, f);
          *strchr(buf, 13) = 0;
          Form1->Edit1->Text = buf;

          for (int i=2; i<n; ++i)
          {
            fread(&length, sizeof(int), 1, f);
            fread(&buf, 1, length, f);
          }
          
          fread(&length, sizeof(int), 1, f);
          fread(&buf, 1, length, f);
          *strchr(buf, 13) = 0;
          Form1->Edit2->Text = buf;

        }
        else if (nTargetFormat==1)
        {
          ReadFrequencyRange(f, Form1->Edit1, Form1->Edit2, Form1->Edit3, Form1->RadioButton1, Form1->RadioButton2);
        }

        ComboBox1Change(this);
        //UpdateParametersList();
        //RecalculateAnalysisGraph();

        FormResize(this);
        ShowStructure(Structure5, TabControl3->TabIndex, Image2);
      } // end MMAnalysis->Checked
      else
      {
        nGACount = 0;
        nTime_accumulated = 0;
        StatusBar1->Panels->Items[0]->Text = " Цикл ГА: 0";
        dDeviation = 100000.0;
        StatusBar1->Panels->Items[1]->Text = " Отклонение: 100000";
        StatusBar1->Panels->Items[2]->Text = ""; // время
        StatusBar1->Panels->Items[3]->Text = ""; // островная популяция

        Series2->Clear();
        Series3->Clear();

        if (pTarget)
        {
          memset(pTarget->m_char[1], 0, sizeof(double)*pTarget->m_length);
          memset(pTarget->m_char[2], 0, sizeof(double)*pTarget->m_length);
        }

        delete Structure1;

        TabControl2->Tabs->Clear();

        switch (RCType)
        {
          case RC0:
          {
            Structure1 = new CRC0Structure(f);
            TabControl2->Tabs->Add("R-C-слой");
          }
          break;
          case RCG0:
          {
            Structure1 = new CRCG0Structure(f);
            TabControl2->Tabs->Add("R-CG-слой");
          }
          break;
          case RCNR:
          {
            Structure1 = new CRCNRStructure(f);
            TabControl2->Tabs->Add("R-C-слой");
            TabControl2->Tabs->Add("NR-слой");
          }
          break;
          case RCGNR:
          {
            Structure1 = new CRCGNRStructure(f);
            TabControl2->Tabs->Add("R-CG-слой");
            TabControl2->Tabs->Add("NR-слой");
          }
          break;
          case RCGNRA:
          {
            Structure1 = new CRCGNRAStructure(f);
            TabControl2->Tabs->Add("R-CG-слой");
            TabControl2->Tabs->Add("NR-слой");
          }
          break;
          case RRCNR:
          {
            Structure1 = new CRRCNRStructure(f);
            TabControl2->Tabs->Add("R-C-слой");
            TabControl2->Tabs->Add("NR-слой");
          }
          break;
          case RRCGNR:
          {
            Structure1 = new CRRCGNRStructure(f);
            TabControl2->Tabs->Add("R-CG-слой");
            TabControl2->Tabs->Add("NR-слой");
          }
          break;
          case RRCGNRA:
          {
            Structure1 = new CRRCGNRAStructure(f);
            TabControl2->Tabs->Add("R-CG-слой");
            TabControl2->Tabs->Add("NR-слой");
          }
          break;
          default:
          {
            ShowMessage("Неизвестный тип структуры");
          }
        }

        // загружаем схему
        bool bLog;
        fread(&bLog, sizeof(bool), 1, f);
        Chart1->BottomAxis->Logarithmic = bLog;
        int length;
        fread(&length, sizeof(int), 1, f);
        Form6->ComboBox1->ItemIndex = length;
        fread(&length, sizeof(int), 1, f);
        Form6->ComboBox2->ItemIndex = length;

        char buf[50];
        fread(&length, sizeof(int), 1, f);
        fread(buf, 1, length, f);
        Form6->Edit1->Text = buf;
        fread(&length, sizeof(int), 1, f);
        fread(buf, 1, length, f);
        Form6->Edit2->Text = buf;

        nCharacteristic = Form6->ComboBox2->ItemIndex;
        delete pScheme;
        pScheme = CScheme::GetScheme(Form6->ComboBox1->ItemIndex, Form6->Edit1->Text.ToDouble(), Form6->Edit2->Text.ToDouble());

        LoadTargetFunctionForSynthesis(f);

        FormResize(this);
        ShowStructure(Structure1, TabControl2->TabIndex, Image1);
        RecalculateSynthesisGraph();
      } // end else
      fclose(f);
    }
    else
      ShowMessage("Невозможно открыть указанный файл для чтения");
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::FMSaveClick(TObject *Sender)
{
  FILE *f;
  if (SaveDialog1->Execute())
  {
    f = fopen(SaveDialog1->FileName.c_str(),"r");
    if (f)
    {
      fclose(f);
      int answer = MessageBox(SaveDialog1->Handle,"Такой файл уже существует. Заменить?","Подтверждение",MB_YESNOCANCEL|MB_ICONWARNING);
      if (answer==IDYES)
        SaveFile(SaveDialog1->FileName.c_str());
      else if (answer==IDNO)
        FMSaveClick(this);
    }
    else
    {
      SaveFile(SaveDialog1->FileName.c_str());
    }
  }
}
//---------------------------------------------------------------------------

void TForm1::WriteEditControl(FILE *f, TEdit *E)
{
  char *buf = strdup(E->Text.c_str());
  int length = strlen(buf)+1;
  fwrite(&length, sizeof(int), 1, f);
  fwrite(buf, length, 1, f);
  free(buf);
}

void TForm1::ReadEditControl(FILE *f, TEdit *E)
{
  int length;
  char buf[50];
  fread(&length, sizeof(int), 1, f);
  fread(buf, 1, length, f);
  E->Text = buf;
}

void TForm1::WriteFrequencyRange(FILE *f, TEdit *Emin, TEdit *Emax, TEdit *ENum, TRadioButton *ELinear, TRadioButton *ELog)
{
  WriteEditControl(f, Emin);
  WriteEditControl(f, Emax);
  WriteEditControl(f, ENum);
  bool bLinear = ELinear->Checked;
  fwrite(&bLinear, sizeof(bool), 1, f);
  bool bLog = ELog->Checked;
  fwrite(&bLog, sizeof(bool), 1, f);
}

void TForm1::ReadFrequencyRange(FILE *f, TEdit *Emin, TEdit *Emax, TEdit *ENum, TRadioButton *ELinear, TRadioButton *ELog)
{
  ReadEditControl(f, Emin);
  ReadEditControl(f, Emax);
  ReadEditControl(f, ENum);
  bool bLinear, bLog;
  fread(&bLinear, sizeof(bool), 1, f);
  ELinear->Checked = bLinear;
  fread(&bLog, sizeof(bool), 1, f);
  ELog->Checked = bLog;
}

void TForm1::SaveFile(char *FileName)
{
  FILE *f = fopen(FileName, "wb");
  if (f)
  {
    if (MMAnalysis->Checked)
    {
      Structure5->SaveStructureToFile(f);
      SaveScheme(f, Form1->RadioButton2->Checked, Form1->ComboBox1->ItemIndex, Form1->ComboBox2->ItemIndex, Form1->Edit4->Text.c_str(), Form1->Edit5->Text.c_str());
      SaveTargetFunction(f, 1);
    }
    else
    {
      Structure1->SaveStructureToFile(f);
      SaveScheme(f, Chart1->BottomAxis->Logarithmic, Form6->ComboBox1->ItemIndex, Form6->ComboBox2->ItemIndex, Form6->Edit1->Text.c_str(), Form6->Edit2->Text.c_str());
      SaveTargetFunction(f, 0);
    }
    fclose(f);
  }
  else
  {
    ShowMessage("Невозможно открыть указанный файл для записи");
  }
}
//---------------------------------------------------------------------------

void TForm1::SaveScheme(FILE *f, bool bLog, int nSch, int nChar, char *pK, char *pL)
{
  fwrite(&bLog, sizeof(bool), 1, f);
  fwrite(&nSch, sizeof(int), 1, f);
  fwrite(&nChar, sizeof(int), 1, f);

  int len = strlen(pK)+1;
  fwrite(&len, sizeof(int), 1, f);
  fwrite(pK, 1, len, f);

  len = strlen(pL)+1;
  fwrite(&len, sizeof(int), 1, f);
  fwrite(pL, 1, len, f);
}
//---------------------------------------------------------------------------

void TForm1::SaveTargetFunction(FILE *f, int nTargetFormat)
{
  int n, length;
  char *buf;
  fwrite(&nTargetFormat, sizeof(int), 1, f);
  if (nTargetFormat==0)
  {
    // сохранение в режиме синтеза
    // сохраняем Form3->StringGrid1 в файл - это целевая функция для синтеза
    // а также сохраняем все текстовые поля в надстройках Фильтр и Окно
    // при загрузке в режиме анализа берем первую и последнюю частоту
    // при загрузке в режиме синтеза загружаем как есть

    n = Form3->StringGrid1->RowCount-2; // количество точек
    fwrite(&n, sizeof(int), 1, f);

    for (int i=1; i<=n; ++i)
    {
      buf = strdup(Form3->StringGrid1->Rows[i]->GetText());
      length = strlen(buf)+1;
      fwrite(&length, sizeof(int), 1, f);
      fwrite(buf, length, 1, f);
      free(buf);
    }

    WriteFrequencyRange(f, Form7->Edit1, Form7->Edit2, Form7->Edit3, Form7->RadioButton1, Form7->RadioButton2);
    n = Form7->ComboBox2->ItemIndex;
    fwrite(&n, sizeof(int), 1, f);
    WriteEditControl(f, Form7->Edit4);
    WriteEditControl(f, Form7->Edit5);
    WriteEditControl(f, Form7->Edit6);
    WriteEditControl(f, Form7->Edit7);
    WriteEditControl(f, Form7->Edit8);
    WriteEditControl(f, Form7->Edit9);
    WriteEditControl(f, Form7->Edit10);
    WriteEditControl(f, Form7->Edit11);
    WriteEditControl(f, Form7->Edit12);
    WriteEditControl(f, Form7->Edit13);
    WriteEditControl(f, Form7->Edit14);
    WriteEditControl(f, Form7->Edit15);
    WriteEditControl(f, Form7->Edit16);
    WriteEditControl(f, Form7->Edit17);
    WriteEditControl(f, Form7->Edit18);
    WriteEditControl(f, Form7->Edit19);
    WriteEditControl(f, Form7->Edit20);
    WriteEditControl(f, Form7->Edit21);
    WriteEditControl(f, Form7->Edit22);
    WriteEditControl(f, Form7->Edit23);
    WriteEditControl(f, Form7->Edit24);
    WriteEditControl(f, Form7->Edit25);
    WriteEditControl(f, Form7->Edit26);
    WriteEditControl(f, Form7->Edit27);
    WriteEditControl(f, Form7->Edit28);
    WriteEditControl(f, Form7->Edit29);
    WriteEditControl(f, Form7->Edit30);

    WriteFrequencyRange(f, Form8->Edit1, Form8->Edit2, Form8->Edit3, Form8->RadioButton1, Form8->RadioButton2);
    WriteEditControl(f, Form8->Edit4);
    WriteEditControl(f, Form8->Edit5);
    WriteEditControl(f, Form8->Edit6);
    WriteEditControl(f, Form8->Edit7);
  }
  else
  {
    // сохранение в режиме анализа
    // сохраням количество точек, минимальную и максимальную частоту в файл
    // при загрузке в режиме анализа загружаем как есть
    // при загрузке в режиме синтеза преобразуем к целевой функции без ограничений
    WriteFrequencyRange(f, Form1->Edit1, Form1->Edit2, Form1->Edit3, Form1->RadioButton1, Form1->RadioButton2);
  }
}
//---------------------------------------------------------------------------

void TForm1::LoadTargetFunctionForSynthesis(FILE *f)
{
  int nTargetFormat, n, length;
  char buf[50];
  fread(&nTargetFormat, sizeof(int), 1, f);
  if (nTargetFormat==0)
  {
    fread(&n, sizeof(int), 1, f);
    Form3->StringGrid1->RowCount = n+2;

    for (int i=1; i<=n; ++i)
    {
      fread(&length, sizeof(int), 1, f);
      fread(&buf, 1, length, f);
      Form3->StringGrid1->Rows[i]->SetText(buf);
    }

    ReadFrequencyRange(f, Form7->Edit1, Form7->Edit2, Form7->Edit3, Form7->RadioButton1, Form7->RadioButton2);
    fread(&n, sizeof(int), 1, f);
    Form7->ComboBox2->ItemIndex = n;
    ReadEditControl(f, Form7->Edit4);
    ReadEditControl(f, Form7->Edit5);
    ReadEditControl(f, Form7->Edit6);
    ReadEditControl(f, Form7->Edit7);
    ReadEditControl(f, Form7->Edit8);
    ReadEditControl(f, Form7->Edit9);
    ReadEditControl(f, Form7->Edit10);
    ReadEditControl(f, Form7->Edit11);
    ReadEditControl(f, Form7->Edit12);
    ReadEditControl(f, Form7->Edit13);
    ReadEditControl(f, Form7->Edit14);
    ReadEditControl(f, Form7->Edit15);
    ReadEditControl(f, Form7->Edit16);
    ReadEditControl(f, Form7->Edit17);
    ReadEditControl(f, Form7->Edit18);
    ReadEditControl(f, Form7->Edit19);
    ReadEditControl(f, Form7->Edit20);
    ReadEditControl(f, Form7->Edit21);
    ReadEditControl(f, Form7->Edit22);
    ReadEditControl(f, Form7->Edit23);
    ReadEditControl(f, Form7->Edit24);
    ReadEditControl(f, Form7->Edit25);
    ReadEditControl(f, Form7->Edit26);
    ReadEditControl(f, Form7->Edit27);
    ReadEditControl(f, Form7->Edit28);
    ReadEditControl(f, Form7->Edit29);
    ReadEditControl(f, Form7->Edit30);

    ReadFrequencyRange(f, Form8->Edit1, Form8->Edit2, Form8->Edit3, Form8->RadioButton1, Form8->RadioButton2);
    ReadEditControl(f, Form8->Edit4);
    ReadEditControl(f, Form8->Edit5);
    ReadEditControl(f, Form8->Edit6);
    ReadEditControl(f, Form8->Edit7);

  }
  else if (nTargetFormat==1)
  {
    double dw, Wmin, Wmax;
    bool bLinear, bLog;

    fread(&length, sizeof(int), 1, f);
    fread(&buf, 1, length, f);
    Wmin = StrToFloat(buf);
    fread(&length, sizeof(int), 1, f);
    fread(&buf, 1, length, f);
    Wmax = StrToFloat(buf);
    fread(&length, sizeof(int), 1, f);
    fread(&buf, 1, length, f);
    n = StrToInt(buf);
    Form3->StringGrid1->RowCount = n+2;
    fread(&bLinear, sizeof(bool), 1, f);
    fread(&bLog, sizeof(bool), 1, f);

    if (bLinear)
      dw = (Wmax-Wmin)/(n-1);
    else if (bLog)
      dw = (log10(Wmax)-log10(Wmin))/(n-1);

    for (int i=0; i<n; ++i)
    {
      if (bLinear)
      {
        Form3->StringGrid1->Cells[0][i+1] = FloatToStrF(Wmin+dw*i, ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[1][i+1] = " ";
        Form3->StringGrid1->Cells[2][i+1] = " ";
      }
      else if (bLog)
      {
        Form3->StringGrid1->Cells[0][i+1] = FloatToStrF( pow(10.0, log10(Wmin)+dw*i), ffFixed, nPrecision, nDigits);
        Form3->StringGrid1->Cells[1][i+1] = " ";
        Form3->StringGrid1->Cells[2][i+1] = " ";
      }
    }
  }
  Form3->Button2Click(this);
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SMSchemeClick(TObject *Sender)
{
  Form6->ShowModal();
  if (Form6->ModalResult==mrOk)
  {
    nCharacteristic = Form6->ComboBox2->ItemIndex;
    delete pScheme;
    pScheme = CScheme::GetScheme(Form6->ComboBox1->ItemIndex, Form6->Edit1->Text.ToDouble(), Form6->Edit2->Text.ToDouble());
    RecalculateSynthesisGraph();
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::ChangeProgramMode(TObject *Sender)
{
  TMenuItem *Mode = (TMenuItem*)Sender;
  MMSynthesis->Checked = (Mode==MMSynthesis);
  MMAnalysis->Checked = (Mode==MMAnalysis);
  MMFit->Checked = (Mode==MMFit);
  SynthesisMenu->Enabled = (Mode!=MMAnalysis);
  AnalysisMenu->Enabled = (Mode==MMAnalysis);
  TabControl1->Visible = (Mode==MMAnalysis);
  TabControl2->Visible = (Mode!=MMAnalysis);
  TabControl3->Visible = (Mode==MMAnalysis);
  Chart1->Visible = (Mode!=MMAnalysis);
  SpeedButton6->Visible = (Mode!=MMAnalysis);
  Panel1->Visible = (Mode==MMAnalysis);
  if (Mode==MMAnalysis)
  {
    StatusBar1->Panels->Items[0]->Text="";
    StatusBar1->Panels->Items[1]->Text="";
    StatusBar1->Panels->Items[2]->Text="";
    StatusBar1->Panels->Items[3]->Text="";
    ComboBox1Change(this);
  }
  FormResize(this);
}
//---------------------------------------------------------------------------

void __fastcall TForm1::ChangeStructureParameter(TObject *Sender)
{
  TMenuItem *pParameter = (TMenuItem*)Sender;
  Form10->Caption = pParameter->Caption;
  Form10->Edit1->OnKeyPress = TForm1::OnEditKeyPress;
  double dParameter = 0.0;

  CRCStructure *Structure;
  if (MMAnalysis->Checked)
    Structure = Structure5;
  else
    Structure = Structure1;

       if (pParameter == SMChangeR)  {dParameter = Structure->GetR() ;}
  else if (pParameter == SMChangeC)  {dParameter = Structure->GetC() ;}
  else if (pParameter == SMChangeKf) {dParameter = Structure->GetKf();}
  else if (pParameter == SMChangeG)  {dParameter = Structure->GetG() ;}
  else if (pParameter == SMChangeH)  {dParameter = Structure->GetH() ;}
  else if (pParameter == SMChangeN)  {dParameter = Structure->GetN() ;}
  else if (pParameter == SMChangeP)  {dParameter = Structure->GetP() ;}

  Form10->Edit1->Text = FloatToStr(dParameter);
  if (Form10->ShowModal() == mrOk)
  {
    try
    {
      dParameter = Form10->Edit1->Text.ToDouble();
           if (pParameter == SMChangeR)  {Structure->SetR (dParameter);}
      else if (pParameter == SMChangeC)  {Structure->SetC (dParameter);}
      else if (pParameter == SMChangeKf) {Structure->SetKf(dParameter);}
      else if (pParameter == SMChangeG)  {Structure->SetG (dParameter);}
      else if (pParameter == SMChangeH)  {Structure->SetH (dParameter);}
      else if (pParameter == SMChangeN)  {Structure->SetN (dParameter);}
      else if (pParameter == SMChangeP)  {Structure->SetP (dParameter);}
      else ShowMessage("Неизвестный параметр");
    }
    catch (...)
    {}
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SMEditMaskClick(TObject *Sender)
{
  SMEditMask->Checked = !SMEditMask->Checked;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AMStructureClick(TObject *Sender)
{
  TabControl3->Visible = true;
  TabControl1->Visible = false;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AMGraphClick(TObject *Sender)
{
  TabControl3->Visible = false;
  TabControl1->Visible = true;
  //if (TabControl1->TabIndex != -1)
  //  RecalculateGraphAnalysis(TabControl1->TabIndex);
}
//---------------------------------------------------------------------------

void __fastcall TForm1::FMBatchSynthClick(TObject *Sender)
{
  Form11->ShowModal();
  if (Form11->ModalResult == mrOk)
  {
    if (pTGA)
    {
      ShowMessage("Синтез уже запущен");
      return;
    }

    int CMAX = Form11->Edit1->Text.ToIntDef(0);
    if (CMAX <= 0)
    {
      ShowMessage("Введите количество структур");
      return;
    }

    nMaxBatchGACount = Form11->Edit4->Text.ToIntDef(0);
    if (nMaxBatchGACount <= 0)
    {
      ShowMessage("Введите максимальное число циклов");
      return;
    }

    AnsiString LogFile = Form11->Edit2->Text;
    if (!ExtractFileDrive(LogFile).Length())
      SetCurrentDirectory(ExtractFileDir(Application->ExeName).c_str()); // если указано только имя файла, или относительный путь к файлу, а не полный путь, то изменяем текущую директорию на директорию где лежит программа
    FILE *f = fopen(LogFile.c_str(), "at");
    if (f)
    {
      fclose(f);
    }
    else
    {
      ShowMessage("Невозможно создать файл лога");
      return;
    }

    CRCStructure *Structure6 = Structure1;

    SYSTEMTIME systime;
    AnsiString savename, str;
    AnsiString format = "RC %04d-%02d-%02d %02d_%02d_%02d.rcs";
    if (Form11->Edit3->Text.Length())
      format = Form11->Edit3->Text + "\\" + format;

    bIsNotBatch = false;

/*
for (double cross = 0.1; cross < 1.0; cross += 0.2)
{
for (double mutate1 = 0.1; mutate1 < 1.0; mutate1 += 0.2)
{
for (double mutate3 = 0.1; mutate3 < 1.0; mutate3 += 0.2)
{
    f = fopen(LogFile.c_str(), "at");
    str = AnsiString("cross=") + AnsiString(cross) + AnsiString("\t");
    fputs(str.c_str(), f);
    str = AnsiString("mutate1=") + AnsiString(mutate1) + AnsiString("\t");
    fputs(str.c_str(), f);
    str = AnsiString("mutate3=") + AnsiString(mutate3) + AnsiString("\n");
    fputs(str.c_str(), f);
    fclose(f);

    pGASettings->SetProbMutateR1(mutate1);
    pGASettings->SetProbMutateR3(mutate3);
    pGASettings->SetProbCross(cross);
*/

    int nTotalGACount = 0;
    int *pCountArray = new int[CMAX];
    AnsiString Title = Application->Title;
    int time1 = GetTickCount();
    for (int iter=0; iter<CMAX; ++iter)
    {
      nGACount = 0;
      dDeviation = 100000.0;

      Structure1 = Structure6->clone();

      //pTGA = new TThreadGA(0.0, bAllowDirectSynthesis, MMFit->Checked);
      pTGA = new TThreadGA(&Structure1, 0.0, pGASettings);
      Application->Title = IntToStr(iter)+"/"+IntToStr(CMAX) + " Ждите…";

      if (WaitForSingleObject((HANDLE)(pTGA->Handle), INFINITE) != WAIT_OBJECT_0)
        ShowMessage("Ошибка ожидания потока");
      Application->ProcessMessages();

      nTotalGACount += nGACount;
      pCountArray[iter] = nGACount;

      f = fopen(LogFile.c_str(), "at");
      str = AnsiString(nGACount) + AnsiString("\t");
      fputs(str.c_str(), f);
      fclose(f);

      if (Form11->CheckBox1->Checked)
      {
        GetLocalTime(&systime);
        savename.sprintf(format.c_str(), systime.wYear, systime.wMonth, systime.wDay, systime.wHour, systime.wMinute, systime.wSecond);
        SaveFile(savename.c_str());
      }
    }
    int time2 = GetTickCount()-time1;
    Application->Title = Title;

    int nTemp;
    f = fopen(LogFile.c_str(), "at");
    fputs("\n", f);
    for (int i=0; i<CMAX; ++i)
    {
      for (int j=i+1; j<CMAX; ++j)
      {
        if (pCountArray[j] < pCountArray[i])
        {
          nTemp = pCountArray[i];
          pCountArray[i] = pCountArray[j];
          pCountArray[j] = nTemp;
        }
      }
      str = AnsiString(pCountArray[i]) + AnsiString("\t");
      fputs(str.c_str(), f);
    }
    delete[] pCountArray;

    str = "\nВсего циклов: " + AnsiString(nTotalGACount);
    fputs(str.c_str(), f);
    str = "\nСреднее число циклов: " + AnsiString(nTotalGACount/CMAX);
    fputs(str.c_str(), f);

    str = "\nВремя (мс): " + IntToStr(time2) + "\n\n";
    fputs(str.c_str(), f);
    fclose(f);
/*
}
}
}
*/
    bIsNotBatch = true;
    delete Structure1;
    Structure1 = Structure6;
    pTGA = NULL;
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::TabControl1Change(TObject *Sender)
{
  //UpdateForm(TabControl1->TabIndex);
  //RecalculateGraphAnalysis(TabControl1->TabIndex);
  DrawAnalyseGraph();
}

void TForm1::RecalculateAnalysisGraph()
{
  int ExitCode = -1;

  if (Structure5->CheckKP())
  {
    if (pAnalyseParameters->m_pScheme == NULL || Structure5->GetKPQuantity() == pAnalyseParameters->m_pScheme->GetKPQuantity())
    {
      int t0 = GetTickCount();
      ExitCode = CalculateYParameters(Structure5, pAnalyseParameters->m_pT);
      StatusBar1->Panels->Items[2]->Text = " Время расчета: " + IntToStr(GetTickCount()-t0) + " мс";
    }
    else
    {
      ShowMessage("Количество контактных площадок структуры не совпадает с количеством КП схемы.");
    }
  }
  else
  {
    ShowMessage("Пронумеруйте контактные площадки.");
  }

  if (ExitCode != -1)
  {
    ap::real_1d_array Num,Den;
    int m = CSpinEdit1->Text.ToInt();
    int n = CSpinEdit2->Text.ToInt();
    if (CheckBox1->Checked || TabControl1->TabIndex==1)
    {
      if (pAnalyseParameters->m_pScheme == NULL)
      {
        ShowMessage("Годограф недоступен для формулы");
      }
      else
      {
        double Kmin = Edit6->Text.ToDouble();
        double Kmax = Edit7->Text.ToDouble();
        int Kn = Edit8->Text.ToInt();
        double Kdelta = (Kmax-Kmin)/(Kn-1);
        ZerosR.setbounds(0,Kn-1,1,m);
        ZerosI.setbounds(0,Kn-1,1,m);
        PolesR.setbounds(0,Kn-1,1,n);
        PolesI.setbounds(0,Kn-1,1,n);
        double lastK = pAnalyseParameters->m_pScheme->GetK();
        for (int ki=0; ki<Kn; ++ki)
        {
          pAnalyseParameters->m_pScheme->SetK(Kmin+Kdelta*ki);
          pAnalyseParameters->m_pScheme->CharacteristicT(pAnalyseParameters->m_pT, Structure5);
          pAnalyseParameters->m_pScheme->Approximate(pAnalyseParameters->m_pT, m, n);

          Num.setbounds(0,m);
          pAnalyseParameters->m_pT->NumResR.setbounds(1,m);
          pAnalyseParameters->m_pT->NumResI.setbounds(1,m);
          Den.setbounds(0,n);
          pAnalyseParameters->m_pT->DenResR.setbounds(1,n);
          pAnalyseParameters->m_pT->DenResI.setbounds(1,n);
          for (int i=0; i<=m; ++i)
            Num(i) = pAnalyseParameters->m_pT->m_sol(i+1);
          for (int i=1; i<=n; ++i)
            Den(i) = pAnalyseParameters->m_pT->m_sol(i+1+m);
          Den(0) = 1.0;
          eigenpolyroots(Num, m, pAnalyseParameters->m_pT->NumResR, pAnalyseParameters->m_pT->NumResI);
          eigenpolyroots(Den, n, pAnalyseParameters->m_pT->DenResR, pAnalyseParameters->m_pT->DenResI);
          for (int i=1; i<=m; ++i)
          {
            ZerosR(ki,i) = pAnalyseParameters->m_pT->NumResR(i);
            ZerosI(ki,i) = pAnalyseParameters->m_pT->NumResI(i);
          }
          for (int i=1; i<=n; ++i)
          {
            PolesR(ki,i) = pAnalyseParameters->m_pT->DenResR(i);
            PolesI(ki,i) = pAnalyseParameters->m_pT->DenResI(i);
          }
        }
        pAnalyseParameters->m_pScheme->SetK(lastK);
        pAnalyseParameters->m_pScheme->Setka(pAnalyseParameters->m_pT, pAnalyseParameters->m_Characteristic, pAnalyseParameters->m_pT->m_char[1], Structure5);
        Label11->Visible = !pAnalyseParameters->m_pScheme->Approximate(pAnalyseParameters->m_pT, m, n);
        pAnalyseParameters->m_pScheme->SetkaApproximated(pAnalyseParameters->m_pT, pAnalyseParameters->m_Characteristic, pAnalyseParameters->m_pT->m_char[2]);

        Num.setbounds(0,m);
        pAnalyseParameters->m_pT->NumResR.setbounds(1,m);
        pAnalyseParameters->m_pT->NumResI.setbounds(1,m);
        Den.setbounds(0,n);
        pAnalyseParameters->m_pT->DenResR.setbounds(1,n);
        pAnalyseParameters->m_pT->DenResI.setbounds(1,n);
        for (int i=0; i<=m; ++i)
          Num(i) = pAnalyseParameters->m_pT->m_sol(i+1);
        for (int i=1; i<=n; ++i)
          Den(i) = pAnalyseParameters->m_pT->m_sol(i+1+m);
        Den(0) = 1.0;
        eigenpolyroots(Num, m, pAnalyseParameters->m_pT->NumResR, pAnalyseParameters->m_pT->NumResI);
        eigenpolyroots(Den, n, pAnalyseParameters->m_pT->DenResR, pAnalyseParameters->m_pT->DenResI);
        bool bIsUnstable = false;
        for (int i=1; i<=n; ++i)
          if (pAnalyseParameters->m_pT->DenResR(i)>0.0)
            bIsUnstable = true;
        Label12->Visible = bIsUnstable;
      }
    }
    else
    {
      if (pAnalyseParameters->m_pScheme == NULL)
      {
        mup::ParserX parser;
        parser.SetExpr(Edit9->Text.c_str());

        int i = 0;
        double *pLastValue = NULL;
        parser.DefineFun(new FunGetY(pAnalyseParameters->m_pT->m_MatY, &i, &Structure5));
        parser.DefineFun(new FunMag());
        parser.DefineFun(new FunPhase(&pLastValue));
        parser.DefineFun(new FunCond(&Structure5));

        for (; i < pAnalyseParameters->m_pT->m_length; ++i)
        {
          mup::IValue *val = parser.Eval().AsIValue();
          if (val->IsNonComplexScalar())
          {
            pAnalyseParameters->m_pT->m_char[1][i] = val->GetFloat();
            pLastValue = &pAnalyseParameters->m_pT->m_char[1][i];
          }
          else
          {
            throw new std::runtime_error("formula must evaluate to non-complex value");
          }
        }
      }
      else
      {
        pAnalyseParameters->m_pScheme->Setka(pAnalyseParameters->m_pT, pAnalyseParameters->m_Characteristic, pAnalyseParameters->m_pT->m_char[1], Structure5);
      }
    }
    DrawAnalyseGraph();
  }
}
//---------------------------------------------------------------------------


void TForm1::RecalculateSynthesisGraph()
{
  if (pTarget && pScheme && Structure1->CheckKP() && (pScheme == NULL || Structure1->GetKPQuantity()==pScheme->GetKPQuantity()))
  {
    //int t0 = GetTickCount();
    int ExitCode = CalculateYParameters(Structure1, pTarget);
    //StatusBar1->Panels->Items[2]->Text = IntToStr(GetTickCount()-t0) + " мс";

    if (ExitCode == -1)
    {
      dDeviation = 100000.0;
    }
    else
    {
      if (pScheme == NULL)
      {
        mup::ParserX parser;
        parser.SetExpr(Form6->Edit3->Text.c_str());

        int i = 0;
        double *pLastValue = NULL;

        parser.DefineFun(new FunGetY(pTarget->m_MatY, &i, &Structure1));
        parser.DefineFun(new FunMag());
        parser.DefineFun(new FunPhase(&pLastValue));
        parser.DefineFun(new FunCond(&Structure1));

        for (; i < pTarget->m_length; ++i)
        {
          mup::IValue *val = parser.Eval().AsIValue();
          if (val->IsNonComplexScalar())
          {
            pTarget->m_char[1][i] = val->GetFloat();
            pLastValue = &pTarget->m_char[1][i];
          }
          else
          {
            throw new std::runtime_error("formula must evaluate to non-complex value");
          }
        }
      }
      else
      {
        pScheme->Setka(pTarget, nCharacteristic, pTarget->m_char[1], Structure1);
      }
      dDeviation = pTarget->Deviation(1);
    }
    StatusBar1->Panels->Items[1]->Text = " Отклонение: " + FloatToStr(dDeviation);
    Graph();
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AMLoadClick(TObject *Sender)
{
  delete Structure5;

  TabControl3->Tabs = TabControl2->Tabs;
  Structure5 = Structure1->clone();

  // загружаем схему
  if (pScheme)
  {
    ComboBox1->ItemIndex = Form6->ComboBox1->ItemIndex;
    ComboBox2->ItemIndex = Form6->ComboBox2->ItemIndex;
    Edit4->Text = pScheme->GetK();
    Edit5->Text = pScheme->GetL();
    ComboBox1Change(this);
  }

  // загружаем целевую функцию
  if (pTarget)
  {
    Edit1->Text = pTarget->m_w[0];
    Edit2->Text = pTarget->m_w[pTarget->m_length-1];
    Edit3->Text = pTarget->m_length;
    // линейное или логарифмическое распределение не загружаем
  }

  TabControl3->Visible = true;
  TabControl1->Visible = false;
  TabControl1->Tabs->Clear();
  TabControl1->Tabs->Add(ComboBox1->Text+" - "+ComboBox2->Text);
  TabControl1->Tabs->Add("Годограф");
  delete pAnalyseParameters;
  pAnalyseParameters = new CAnalyseParameters(ComboBox1->ItemIndex, ComboBox2->ItemIndex, Edit4->Text.ToDouble(), Edit5->Text.ToDouble(), RadioButton2->Checked, Edit3->Text.ToInt());

  ShowStructure(Structure5, TabControl3->TabIndex, Image2);
}
//---------------------------------------------------------------------------

void __fastcall TForm1::ComboBox1Change(TObject *Sender)
{
  UpdateControls(ComboBox1, NULL, ComboBox2, Image3, Label4, Edit4, Label5, Edit5, Label7, Edit9);
  //UpdateParametersList();
}
//---------------------------------------------------------------------------

void TForm1::UpdateParametersList()
{
  if (Structure5)
  {
    delete pAnalyseParameters;
    pAnalyseParameters = new CAnalyseParameters(ComboBox1->ItemIndex, ComboBox2->ItemIndex, Edit4->Text.ToDouble(), Edit5->Text.ToDouble(), RadioButton2->Checked, Edit3->Text.ToInt());
    //pAnalyseParameters = new CAnalyseParameters(RadioButton2->Checked, Edit3->Text.ToInt());

    double dw;
    double Wmin = Edit1->Text.ToDouble();
    double Wmax = Edit2->Text.ToDouble();
    int n = Edit3->Text.ToInt();
    if (RadioButton1->Checked)
      dw = (Wmax-Wmin)/(n-1);
    else if (RadioButton2->Checked)
      dw = (log10(Wmax)-log10(Wmin))/(n-1);

    for (int i=0; i<n; ++i)
    {
      if (RadioButton1->Checked)
        pAnalyseParameters->m_pT->m_w[i] = Wmin+dw*i;
      else if (RadioButton2->Checked)
        pAnalyseParameters->m_pT->m_w[i] = pow(10.0, log10(Wmin)+dw*i);
    }

    TabControl1->Tabs->Strings[0] = (ComboBox1->Text + " - " + ComboBox2->Text);
  }
}

bool TForm1::ValidateForm()
{
  if (Edit1->Text.Length() && Edit2->Text.Length() && Edit3->Text.Length() && Edit4->Text.Length() && Edit5->Text.Length() && Edit3->Text.ToInt()>1 && Edit1->Text.ToDouble()>0 && Edit2->Text.ToDouble()>0 && Edit5->Text.ToDouble()>=0)
  {
    Label8->Visible = false;
    return true;
  }
  else
  {
    Label8->Visible = true;
    return false;
  }
}
void __fastcall TForm1::ComboBox2Change(TObject *Sender)
{
  /*
  //UpdateParametersList();
  if (Structure5 && ValidateForm())
  {
    pAnalyseParameters->m_Characteristic = ComboBox2->ItemIndex;
    TabControl1->Tabs->Strings[0] = (ComboBox1->Text + " - " + ComboBox2->Text);
    pAnalyseParameters->m_pScheme->Setka(pAnalyseParameters->m_pT, pAnalyseParameters->m_Characteristic, pAnalyseParameters->m_pT->m_char[1], Structure5);
    pAnalyseParameters->m_pScheme->SetkaApproximated(pAnalyseParameters->m_pT, pAnalyseParameters->m_Characteristic, pAnalyseParameters->m_pT->m_char[2]);
    DrawAnalyseGraph();
  }
  */
}
//---------------------------------------------------------------------------

void __fastcall TForm1::OnEditKeyPress(TObject *Sender, char &Key)
{
  if ((Key >= '0') && (Key <= '9')) {} // если цифра
  else if (Key == 8) {}
  else if ((Key=='.' || Key == ',') && Sender!=Edit3 && Sender!=Edit8 && (((TEdit*)Sender)->Text.Pos(DecimalSeparator)==0 || ((TEdit*)Sender)->SelText.Pos(DecimalSeparator)!=0))
    Key = DecimalSeparator;
  else Key = 0; // не цифра
}
//---------------------------------------------------------------------------

void __fastcall TForm1::OnTGATerminate(TObject* Sender)
{
  pTGA = NULL;
  nTime_accumulated += GetTickCount()-nTime0;
  SpeedButton6->Glyph->LoadFromResourceName((int)HInstance, "PLAYICON");
  SpeedButton6->Caption = "Синтез";
  SpeedButton6->Down = false;
  SynthesisMenu->Enabled = true;
  StructureMenu->Enabled = true;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::OnChartMouseMove(TObject *Sender, TShiftState Shift, int X, int Y)
{
  static int nOldMouseX, nOldMouseY;
  TChart *pChart = (TChart*)Sender;
  TCanvas3D *pCanvas = pChart->Canvas;
  pCanvas->Pen->Color = clBlue;
  pCanvas->Pen->Mode = pmNotXor;
  pCanvas->Pen->Style = psDot;
  CRCStructureCalculateData *pT;
  if (pChart == Chart1)
    pT = pTarget;
  else if ((pChart == Chart2) && (TabControl1->TabIndex == 0) && pAnalyseParameters)
    pT = pAnalyseParameters->m_pT;
  else
    pT = NULL;

  if (pT && pChart->ChartWidth>0)
  {
    double gX,gY,kk;
    if (pChart->BottomAxis->Logarithmic)
      gX = pow(10.0, (X-pChart->LeftAxis->PosAxis)*log10(pT->m_w[pT->m_length-1]/pT->m_w[0])/pChart->ChartWidth + log10(pT->m_w[0]));
    else
      gX = (X-pChart->LeftAxis->PosAxis)*(pT->m_w[pT->m_length-1]-pT->m_w[0])/pChart->ChartWidth + pT->m_w[0];

    int index = 0;
    while (index < pT->m_length  &&  gX > pT->m_w[index])
      ++index;
    if (index == pT->m_length)
      --index;
    if (index>0)
    {
      kk = (pT->m_char[1][index]-pT->m_char[1][index-1])/(pT->m_w[index]-pT->m_w[index-1]);
      gY = kk*gX + pT->m_char[1][index] - kk*pT->m_w[index];
    }
    StatusBar1->Panels->Items[4]->Text = " X: "+FloatToStr(gX)+"  Y: "+FloatToStr(gY);

    if (bMouseInited)
    {
      pCanvas->MoveTo(nOldMouseX, 0);
      pCanvas->LineTo(nOldMouseX, pChart->Height);
    }
    pCanvas->MoveTo(X, 0);
    pCanvas->LineTo(X, pChart->Height);

    nOldMouseX = X;
    nOldMouseY = Y;
    bMouseInited = true;
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::OnChartClick(TObject *Sender)
{
  if (!(MMAnalysis->Checked && TabControl1->TabIndex==1))
  {
    TChart *pChart = (TChart*)Sender;
    pChart->TopAxis->Logarithmic = !pChart->TopAxis->Logarithmic;
    pChart->BottomAxis->Logarithmic = !pChart->BottomAxis->Logarithmic;
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::MMFitClick(TObject *Sender)
{
  MMSynthesis->Checked = false;
  MMAnalysis->Checked = false;
  MMFit->Checked = true;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::TabControl2Change(TObject *Sender)
{
  ShowStructure(Structure1, TabControl2->TabIndex, Image1);
  SpeedButton3->Enabled = (TabControl2->TabIndex != TabControl2->Tabs->Count-1);
}
//---------------------------------------------------------------------------

void __fastcall TForm1::TabControl3Change(TObject *Sender)
{
  ShowStructure(Structure5, TabControl3->TabIndex, Image2);
  SpeedButton3->Enabled = (TabControl3->TabIndex != TabControl3->Tabs->Count-1);
}
//---------------------------------------------------------------------------

TColor TForm1::GetElementColor(EnumRCElementType ElementType)
{
  int color;
  switch (ElementType)
  {
    case RCET_EMPTY: color=0x00FFFFFF; break;  // вырез
    case RCET_R: color=0x00008000; break;  // R
    case RCET_RC: color=0x00808080; break;  // R-C
    default: color=0x00FF00FF; break;
  }
  return (TColor)color;
}
//---------------------------------------------------------------------------

TColor TForm1::GetKPColor(int KPType)
{
  TColor color;
  switch (KPType)
  {
    case KPNONE: color = clWhite; break;
    case KPNORMAL: color = clRed; break;
    case KPRESTRICT: color = clYellow; break;
    case KPSHUNT: color = clSkyBlue; break;
    default: color = clBlack; break;
  }
  return color;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::BitBtn1Click(TObject *Sender)
{
  if (ValidateForm())
  {
    try
    {
      BitBtn1->Enabled = false;
      StructureMenu->Enabled = false;
      Edit9->Color = clWindow;

      UpdateParametersList();
      RecalculateAnalysisGraph();
      TabControl3->Visible = false;
      TabControl1->Visible = true;
    }
    catch (const std::out_of_range& ex)
    {
      Edit9->Color = clRed;
    }
    catch (const mup::ParserError& ex)
    {
      Edit9->Color = clRed;
    }

    StructureMenu->Enabled = true;
    BitBtn1->Enabled = true;

  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AMExportClick(TObject *Sender)
{
  if (Structure5 && Structure5->GetKPQuantity() > 0)
    Form9->ShowModal();
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AMKPVClick(TObject *Sender)
{
  Structure5->KPV();
  ShowStructure(Structure5, TabControl3->TabIndex, Image2);
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AMKPHClick(TObject *Sender)
{
  Structure5->KPH();
  ShowStructure(Structure5, TabControl3->TabIndex, Image2);
}
//---------------------------------------------------------------------------

void TForm1::DrawAnalyseGraph()
{
  int m = CSpinEdit1->Text.ToInt();
  int n = CSpinEdit2->Text.ToInt();
  Series4->Clear();
  Series5->Clear();
  Series6->Clear();
  Series7->Clear();
  Series8->Clear();
  Series9->Clear();
  Series10->Clear();

  if (TabControl1->TabIndex==0)
  {
    Chart2->UndoZoom();
    Chart2->TopAxis->Logarithmic = pAnalyseParameters->m_Log;
    Chart2->BottomAxis->Logarithmic = pAnalyseParameters->m_Log;

    for (int i=0; i<pAnalyseParameters->m_pT->m_length; ++i)
    {
      Series4->AddXY(pAnalyseParameters->m_pT->m_w[i], pAnalyseParameters->m_pT->m_char[1][i]);
    }

    if (CheckBox1->Checked)
    {
      for (int i=0; i<pAnalyseParameters->m_pT->m_length; ++i)
      {
        Series5->AddXY(pAnalyseParameters->m_pT->m_w[i], pAnalyseParameters->m_pT->m_char[2][i]);
      }
    }
  }
  else if (TabControl1->TabIndex==1)
  {
    Chart2->TopAxis->Logarithmic = false;
    Chart2->BottomAxis->Logarithmic = false;

    if (CheckBox2->Checked)
    {
      for (int i=1; i<=m; ++i)
        Series6->AddXY(pAnalyseParameters->m_pT->NumResR(i), pAnalyseParameters->m_pT->NumResI(i), FloatToStrF(pAnalyseParameters->m_pT->NumResR(i), ffFixed, 6, 3));
    }
    if (CheckBox3->Checked)
    {
      for (int i=1; i<=n; ++i)
        Series7->AddXY(pAnalyseParameters->m_pT->DenResR(i), pAnalyseParameters->m_pT->DenResI(i), FloatToStrF(pAnalyseParameters->m_pT->DenResR(i), ffFixed, 6, 3));
    }

    if (CheckBox2->Checked)
    {
      for (int i=1; i<=m; ++i)
      {
        for (int ki=0; ki<Edit8->Text.ToInt(); ++ki)
        {
          Series8->AddXY(ZerosR(ki,i), ZerosI(ki,i));
          if (ki!=0 && CheckBox4->Checked)
            Series10->AddArrow(ZerosR(ki-1,i), ZerosI(ki-1,i), ZerosR(ki,i), ZerosI(ki,i));
        }
        //Series8->AddNull();
      }
    }
    if (CheckBox3->Checked)
    {
      for (int i=1; i<=n; ++i)
      {
        for (int ki=0; ki<Edit8->Text.ToInt(); ++ki)
        {
          Series9->AddXY(PolesR(ki,i), PolesI(ki,i));
          if (ki!=0 && CheckBox4->Checked)
            Series10->AddArrow(PolesR(ki-1,i), PolesI(ki-1,i), PolesR(ki,i), PolesI(ki,i));
        }
        //Series9->AddNull();
      }
    }

  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SMResizeStructureClick(TObject *Sender)
{
  Form12->Edit1->OnKeyPress = TForm1::OnEditKeyPress;
  Form12->Edit2->OnKeyPress = TForm1::OnEditKeyPress;
  int x, y;

  if (MMAnalysis->Checked)
  {
    x = Structure5->Width();
    y = Structure5->Height();
  }
  else
  {
    x = Structure1->Width();
    y = Structure1->Height();
  }

  Form12->Edit1->Text = IntToStr(x);
  Form12->Edit2->Text = IntToStr(y);

  if (Form12->ShowModal() == mrOk)
  {
    try
    {
      x = Form12->Edit1->Text.ToInt();
      y = Form12->Edit2->Text.ToInt();

      if (MMAnalysis->Checked)
      {
        Structure5->Resize(x, y);
        ShowStructure(Structure5, TabControl3->TabIndex, Image2);
      }
      else
      {
        Structure1->Resize(x, y);
        if (pTarget)
        {
          memcpy(pTarget->m_char[2], pTarget->m_char[1], sizeof(double)*pTarget->m_length); //
          RecalculateSynthesisGraph();
          for (int i=0; i<pTarget->m_length; ++i)                                           //
            pTarget->m_char[2][i] = pTarget->m_char[1][i] - pTarget->m_char[2][i];          //
          Graph();                                                                          //
        }
        ShowStructure(Structure1, TabControl2->TabIndex, Image1);
      }
    }
    catch (...)
    {}
  }
}
//---------------------------------------------------------------------------

int TForm1::CalculateYParameters(CRCStructure *Structure, CRCStructureCalculateData *Data)
{
  HANDLE hBadStructureEvent = CreateEvent(NULL, true, false, NULL);
  HANDLE *hDoneEvents = new HANDLE[Data->m_length];
  CJob job;
  job.Structure = Structure;
  job.Data = Data;
  job.isBadStructureEvent = hBadStructureEvent;
  job.isDoneEvents = hDoneEvents;

  Structure->ElementsToNodes(); // перед расчетом Y-параметров проводим нумерацию узлов
  // посылаем задания
  for (int i=0; i<Data->m_length; ++i)
  {
    hDoneEvents[i] = CreateEvent(NULL, true, false, NULL);
    PostQueuedCompletionStatus(hCompletionPort, 0, i, (OVERLAPPED*)&job);
  }

  // ожидаем завершения расчета
  for (int i=0; i<Data->m_length; ++i)
  {
    if (WaitForSingleObject(hDoneEvents[i], INFINITE) != WAIT_OBJECT_0)
      ShowMessage("Error while waiting event");
    CloseHandle(hDoneEvents[i]);
  }
  delete[] hDoneEvents;

  if (WaitForSingleObject(hBadStructureEvent, 0) == WAIT_OBJECT_0)
  {
    CloseHandle(hBadStructureEvent);
    return -1;
  }
  
  CloseHandle(hBadStructureEvent);
  return 0;
}



void __fastcall TForm1::Image1MouseMove(TObject *Sender, TShiftState Shift,
      int X, int Y)
{
  if (!pTGA && Shift.Contains(ssLeft))
  {
    if (SelectedElementType == RCET_EMPTY || SelectedElementType == RCET_R || SelectedElementType == RCET_RC)
      ImageClick(Structure1, TabControl2->TabIndex, Image1, X, Y);
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Image1MouseUp(TObject *Sender, TMouseButton Button,
      TShiftState Shift, int X, int Y)
{
  if (!pTGA && Button == mbLeft)
  {
    RecalculateSynthesisGraph();
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Image1MouseDown(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y)
{
  if (!pTGA && Shift.Contains(ssLeft))
  {
    ImageClick(Structure1, TabControl2->TabIndex, Image1, X, Y);
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Image2MouseMove(TObject *Sender, TShiftState Shift,
      int X, int Y)
{
  if (Structure5 && Shift.Contains(ssLeft))
    if (SelectedElementType == RCET_EMPTY || SelectedElementType == RCET_R || SelectedElementType == RCET_RC)
      ImageClick(Structure5, TabControl3->TabIndex, Image2, X, Y);
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Image2MouseDown(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y)
{
  if (Structure5 && Shift.Contains(ssLeft))
    ImageClick(Structure5, TabControl3->TabIndex, Image2, X, Y);
}
//---------------------------------------------------------------------------



