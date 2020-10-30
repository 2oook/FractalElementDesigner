//---------------------------------------------------------------------------

#ifndef Unit1H
#define Unit1H
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <Menus.hpp>
#include <ExtCtrls.hpp>
#include <ComCtrls.hpp>
#include <Buttons.hpp>
#include <ArrowCha.hpp>
#include <Chart.hpp>
#include <Series.hpp>
#include <TeEngine.hpp>
#include <TeeProcs.hpp>
#include "ThreadGA.h"
#include <Dialogs.hpp>
#include "CSPIN.h"

//---------------------------------------------------------------------------
class TForm1 : public TForm
{
__published:	// IDE-managed Components
  TMainMenu *MainMenu1;
  TSpeedButton *SpeedButton1;
  TSpeedButton *SpeedButton2;
  TSpeedButton *SpeedButton4;
  TSpeedButton *SpeedButton5;
  TImage *Image1;
  TMenuItem *FileMenu;
  TMenuItem *FMNew;
  TMenuItem *FMOpen;
  TMenuItem *FMSave;
  TMenuItem *SynthesisMenu;
        TMenuItem *SMProbabilities;
        TMenuItem *SMOptions;
  TChart *Chart1;
  TArrowSeries *Series1;
  TFastLineSeries *Series2;
  TFastLineSeries *Series3;
  TSpeedButton *SpeedButton6;
  TStatusBar *StatusBar1;
  TMenuItem *SMTargetFunction;
  TOpenDialog *OpenDialog1;
  TSaveDialog *SaveDialog1;
  TMenuItem *SMScheme;
  TMenuItem *MMSynthesis;
  TMenuItem *ModeMenu;
  TMenuItem *MMAnalysis;
  TMenuItem *AnalysisMenu;
  TMenuItem *AMStructure;
  TMenuItem *AMGraph;
  TImage *Image2;
  TChart *Chart2;
  TGroupBox *GroupBox2;
  TLabel *Label1;
  TLabel *Label2;
  TLabel *Label3;
  TEdit *Edit1;
  TEdit *Edit2;
  TEdit *Edit3;
  TRadioGroup *RadioGroup1;
  TRadioButton *RadioButton1;
  TRadioButton *RadioButton2;
  TTabControl *TabControl1;  // график характеристики в режиме анализа
  TFastLineSeries *Series4;
  TFastLineSeries *Series5;
  TGroupBox *GroupBox1;
  TImage *Image3;
  TLabel *Label4;
  TLabel *Label5;
  TLabel *Label6;
  TLabel *Label7;
  TComboBox *ComboBox1;
  TEdit *Edit4;
  TEdit *Edit5;
  TComboBox *ComboBox2;
  TMenuItem *AMLoad;
  TLabel *Label8;
  TMenuItem *FMBatchSynth;
  TTabControl *TabControl2;  // рисунок структуры в режиме синтеза
  TTabControl *TabControl3;  // рисунок структуры в режиме анализа
  TBitBtn *BitBtn1;
  TMenuItem *AMExport;
  TMenuItem *AMKPV;
  TMenuItem *AMKPH;
  TMenuItem *MMFit;
  TCSpinEdit *CSpinEdit1;
  TCSpinEdit *CSpinEdit2;
  TPanel *Panel1;
  TGroupBox *GroupBox3;
  TLabel *Label9;
  TLabel *Label10;
  TCheckBox *CheckBox1;
  TPointSeries *Series6;
  TPointSeries *Series7;
  TLabel *Label11;
  TLabel *Label12;
  TEdit *Edit6;
  TEdit *Edit7;
  TEdit *Edit8;
  TGroupBox *GroupBox4;
  TLabel *Label13;
  TPointSeries *Series8;
  TPointSeries *Series9;
  TArrowSeries *Series10;
  TCheckBox *CheckBox2;
  TCheckBox *CheckBox3;
  TCheckBox *CheckBox4;
  TMenuItem *FMSeparator1;
  TMenuItem *SMResizeStructure;
  TSpeedButton *SpeedButton7;
  TSpeedButton *SpeedButton8;
    TMenuItem *StructureMenu;
    TMenuItem *SMChangeKf;
    TMenuItem *SMChangeG;
    TMenuItem *SMChangeN;
  TMenuItem *SMSeparator2;
  TMenuItem *SMChangeH;
  TMenuItem *SMChangeP;
        TMenuItem *SMChangeR;
        TMenuItem *SMChangeC;
        TSpeedButton *SpeedButton3;
  TMenuItem *SMSeparator1;
  TMenuItem *SMEditMask;
        TEdit *Edit9;

  void __fastcall FormDestroy(TObject *Sender);
  void __fastcall FMNewClick(TObject *Sender);
  void __fastcall FormResize(TObject *Sender);
  void __fastcall FormCreate(TObject *Sender);
  void __fastcall ChangeProgramMode(TObject *Sender);
  void __fastcall ChangeStructureParameter(TObject *Sender);
  void __fastcall SpeedButton1Click(TObject *Sender);
  void __fastcall SpeedButton2Click(TObject *Sender);
  void __fastcall SpeedButton3Click(TObject *Sender);
  void __fastcall SpeedButton4Click(TObject *Sender);
  void __fastcall SpeedButton5Click(TObject *Sender);
  void __fastcall SMProbabilitiesClick(TObject *Sender);
  void __fastcall SMOptionsClick(TObject *Sender);
  void __fastcall SpeedButton6Click(TObject *Sender);
  void __fastcall SMTargetFunctionClick(TObject *Sender);
  void __fastcall FMOpenClick(TObject *Sender);
  void __fastcall FMSaveClick(TObject *Sender);
  void __fastcall SMSchemeClick(TObject *Sender);
  void __fastcall AMStructureClick(TObject *Sender);
  void __fastcall AMGraphClick(TObject *Sender);
  void __fastcall Image2MouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y);
  void __fastcall TabControl1Change(TObject *Sender);
  void __fastcall AMLoadClick(TObject *Sender);
  void __fastcall ComboBox1Change(TObject *Sender);
  void __fastcall ComboBox2Change(TObject *Sender);
  void __fastcall MMFitClick(TObject *Sender);
  void __fastcall TabControl2Change(TObject *Sender);
  void __fastcall TabControl3Change(TObject *Sender);
  void __fastcall BitBtn1Click(TObject *Sender);
  void __fastcall AMExportClick(TObject *Sender);
  void __fastcall OnEditKeyPress(TObject *Sender, char &Key);
  void __fastcall AMKPVClick(TObject *Sender);
  void __fastcall AMKPHClick(TObject *Sender);
  void __fastcall FMBatchSynthClick(TObject *Sender);
  void __fastcall OnChartMouseMove(TObject *Sender, TShiftState Shift, int X, int Y);
  void __fastcall OnChartAfterDraw(TObject *Sender);
  void __fastcall OnChartClick(TObject *Sender);
  void __fastcall SMResizeStructureClick(TObject *Sender);
  void __fastcall SpeedButton7Click(TObject *Sender);
  void __fastcall SpeedButton8Click(TObject *Sender);
  void __fastcall FormActivate(TObject *Sender);
  void __fastcall SMEditMaskClick(TObject *Sender);
  void __fastcall Image1MouseMove(TObject *Sender, TShiftState Shift,
          int X, int Y);
  void __fastcall Image1MouseUp(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
  void __fastcall Image1MouseDown(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
  void __fastcall Image2MouseMove(TObject *Sender, TShiftState Shift,
          int X, int Y);

private:	// User declarations
  Graphics::TBitmap *StructureImage;
  HANDLE hCompletionPort;
  static HANDLE hStopCompletionPort;
  HANDLE *phThreads;
  ap::real_2d_array ZerosR, ZerosI, PolesR, PolesI;

  bool ValidateForm();
  void DrawAnalyseGraph();
  void UpdateParametersList();
  void RecalculateAnalysisGraph();
  TColor GetElementColor(EnumRCElementType ElementType);
  TColor GetKPColor(int KPType);
  void ImageClick(CRCStructure *Structure, int Layer, TImage *Target, int X, int Y);
  void SaveScheme(FILE *f, bool bLog, int nSch, int nChar, char *pK, char *pL);
  void SaveTargetFunction(FILE *f, int nTargetFormat);
  void LoadTargetFunctionForSynthesis(FILE *f);
  void WriteEditControl(FILE *f, TEdit *E);
  void ReadEditControl(FILE *f, TEdit *E);
  void WriteFrequencyRange(FILE *f, TEdit *Emin, TEdit *Emax, TEdit *ENum, TRadioButton *ELinear, TRadioButton *ELog);
  void ReadFrequencyRange(FILE *f, TEdit *Emin, TEdit *Emax, TEdit *ENum, TRadioButton *ELinear, TRadioButton *ELog);
  inline int MyMin(int a, int b) {return a < b ? a : b;}
  void SaveFile(char *FileName);

public:		// User declarations
  CRCStructure *Structure1, *Structure5;

  static unsigned int __stdcall WorkerThread(LPVOID CompletionPortID);
  
  __fastcall TForm1(TComponent* Owner);
  void Graph();
  void ShowStructure(CRCStructure *Structure, int Layer, TImage *Target);
  void RecalculateSynthesisGraph();
  int CalculateYParameters(CRCStructure *Structure, CRCStructureCalculateData *Data);
  void __fastcall OnTGATerminate(TObject* Sender);
};

HANDLE TForm1::hStopCompletionPort = NULL;

// TabControl1 содержит Chart2. Chart2 - это график и годограф в режиме анализа
// TabControl2 содержит Image1. Image1 - это рисунок структуры в режиме синтеза
// TabControl3 содержит Image2. Image2 - это рисунок структуры в режиме анализа

class CAnalyseParameters
{
public:
  int m_Scheme, m_Characteristic;
  double m_K, m_L;
  bool m_Log;
  CRCStructureCalculateData *m_pT;
  CScheme *m_pScheme;

  CAnalyseParameters(int Scheme, int Characteristic, double K, double L, bool Log, int Length)
  {
    m_Scheme = Scheme;
    m_Characteristic = Characteristic;
    m_K = K;
    m_L = L;
    m_Log = Log;
    m_pT = new CRCStructureCalculateData(Length);
    m_pScheme = CScheme::GetScheme(Scheme, K, L);
  }

  CAnalyseParameters(bool Log, int Length)
  {
    m_Scheme = 0;
    m_Characteristic = 0;
    m_K = 0;
    m_L = 0;
    m_Log = Log;
    m_pT = new CRCStructureCalculateData(Length);
    m_pScheme = NULL;
  }

  ~CAnalyseParameters()
  {
    delete m_pT;
    delete m_pScheme;
  }

};

class FunGetY : public mup::ICallback
{
  complex<double> **m_MatY;
  int *m_pIndex;
  CRCStructure **m_ppStructure;

public:
  FunGetY(complex<double> **MatY, int *pIndex, CRCStructure **ppStructure) : mup::ICallback(mup::cmFUNC, "y", 2), m_MatY(MatY), m_pIndex(pIndex), m_ppStructure(ppStructure)
  {
  }

  virtual void Eval(mup::ptr_val_type &ret, const mup::ptr_val_type *a_pArg, int a_iArgc)
  {
    complex<double> *my = m_MatY[*m_pIndex];
    int KPQuantity = (*m_ppStructure)->GetKPQuantity();

    int row = a_pArg[0]->GetInteger() - 1; // zero-based row number
    int col = a_pArg[1]->GetInteger() - 1; // zero-based column number

    if (row < 0 || row >= KPQuantity || col < 0 || col >= KPQuantity)
    {
      throw new std::out_of_range("wrong arguments passed to y() function");
    }

    if (row > col)
    {
      std::swap(row, col);
    }

    int row_begin_index = row * (2 * KPQuantity - row - 1) / 2;

    *ret = my[row_begin_index + col];
  }

  virtual const mup::char_type* GetDesc() const
  {
    return "Get Y-parameter";
  }

  virtual mup::IToken* Clone() const
  {
    return new FunGetY(*this);
  }
}; // class FunGetY


class FunMag : public mup::ICallback
{
public:
  FunMag() : mup::ICallback(mup::cmFUNC, "mag", 1)
  {}

  virtual void Eval(mup::ptr_val_type &ret, const mup::ptr_val_type *a_pArg, int a_iArgc)
  {
    complex<double> v = a_pArg[0]->GetComplex();

    *ret = 20.0*log10(sqrt(norm(v)));
  }

  virtual const mup::char_type* GetDesc() const
  {
    return "Get magnitude";
  }

  virtual mup::IToken* Clone() const
  {
    return new FunMag(*this);
  }
}; // class FunMag


class FunPhase : public mup::ICallback
{
  double **m_ppLastValue;
  int m_adjustment;

public:
  FunPhase(double **ppLastValue) : mup::ICallback(mup::cmFUNC, "phase", 1), m_ppLastValue(ppLastValue), m_adjustment(0)
  {}

  virtual void Eval(mup::ptr_val_type &ret, const mup::ptr_val_type *a_pArg, int a_iArgc)
  {
    complex<double> v = a_pArg[0]->GetComplex();
    double phase = 57.295779513082320876798*atan2(v._M_im, v._M_re);

    if (*m_ppLastValue != NULL)
    {
      double delta = phase + m_adjustment - **m_ppLastValue;
      if (delta > 340)
        m_adjustment -= 360;
      else if (delta < -340)
        m_adjustment += 360;
    }

    *ret = phase + m_adjustment;
  }

  virtual const mup::char_type* GetDesc() const
  {
    return "Get phase";
  }

  virtual mup::IToken* Clone() const
  {
    return new FunPhase(*this);
  }
}; // class FunPhase


class FunCond : public mup::ICallback
{
  CRCStructure **m_ppStructure;
  bool m_calculated;
  int m_KPQuantity;
  complex<double> *m_Cond;

public:
  FunCond(CRCStructure **ppStructure) : mup::ICallback(mup::cmFUNC, "cond", 2), m_ppStructure(ppStructure), m_calculated(false)
  {
    m_Cond = NULL;
  }

  ~FunCond()
  {
    delete[] m_Cond;
  }

  virtual void Eval(mup::ptr_val_type &ret, const mup::ptr_val_type *a_pArg, int a_iArgc)
  {
    if (!m_calculated)
    {
      m_KPQuantity = (*m_ppStructure)->GetKPQuantity();
      m_Cond = new complex<double>[(m_KPQuantity*(m_KPQuantity+1))>>1];
      (*m_ppStructure)->YParameters(0.0, m_Cond);
      m_calculated = true;
    }

    int row = a_pArg[0]->GetInteger() - 1; // zero-based row number
    int col = a_pArg[1]->GetInteger() - 1; // zero-based column number

    if (row < 0 || row >= m_KPQuantity || col < 0 || col >= m_KPQuantity)
    {
      throw new std::out_of_range("wrong arguments passed to cond() function");
    }

    if (row > col)
    {
      std::swap(row, col);
    }

    int row_begin_index = row * (2 * m_KPQuantity - row - 1) / 2;

    *ret = m_Cond[row_begin_index + col];
  }

  virtual const mup::char_type* GetDesc() const
  {
    return "Get conductance at 0 frequency";
  }

  virtual mup::IToken* Clone() const
  {
    return new FunCond(*this);
  }
}; // class FunCond


struct CJob
{
  CRCStructure *Structure;
  CRCStructureCalculateData *Data;
  HANDLE isBadStructureEvent;
  HANDLE *isDoneEvents;
};


//---------------------------------------------------------------------------
extern PACKAGE TForm1 *Form1;
//---------------------------------------------------------------------------
#endif
