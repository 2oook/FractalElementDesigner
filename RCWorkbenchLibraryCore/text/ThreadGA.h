//---------------------------------------------------------------------------

#ifndef ThreadGAH
#define ThreadGAH
//---------------------------------------------------------------------------

#include "Scheme.h"
#include "GASettings.h"

//---------------------------------------------------------------------------

class TThreadGA : public TThread
{
private:
  double m_dMaxDeviation;
  unsigned int m_nNonImprovements;
  CRCStructure **m_ppInitialStructure;

protected:
  double st[5];
  CRCStructure *SS[5];
  CGASettings *m_pGASettings;

  TPoint *PreferableMutatePlaces;

  void __fastcall Execute();
  void __fastcall Update();
  void __fastcall Update2();
  int __fastcall GetPreferableMutatePlacesFor(int Index);
  double __fastcall CalculateStructure(int Index);

public:
  __fastcall TThreadGA(CRCStructure **ppInitialStructure, double dMaxDeviation, CGASettings *pSettings);
  __fastcall ~TThreadGA();

};




//---------------------------------------------------------------------------
#endif
