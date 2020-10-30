//---------------------------------------------------------------------------

#ifndef RCNRStructureH
#define RCNRStructureH

#include "RCStructure.h"

class CRCNRStructure : public CRCStructure
{
protected:
  double m_N;
  double Mat_r[8][8];
  double Mat_nr[8][8];
  void InitMatR();
  void InitMatNR();
  int FixElement(int Element);
  void FillGlobalMatrix(complex<double> **Ym, double w);

public:
  CRCNRStructure() {}
  CRCNRStructure(double R, double C, int x, int y, double Kf, double N) : CRCStructure(R, C, x, y, Kf, RCNR, 2)
  {
    m_N = N;
    InitMatR();
    InitMatNR();
  }

  CRCNRStructure(const CRCNRStructure& S) : CRCStructure(S)
  {
    m_N = S.m_N;
    InitMatR();
    InitMatNR();
  }

  CRCNRStructure(FILE *f) : CRCStructure(f, 2)
  {
    fread(&m_N, sizeof(double), 1, f);
    InitMatR();
    InitMatNR();
  }

  CRCStructure* clone() const
  {
    return new CRCNRStructure(*this);
  }

  void SaveStructureToFile(FILE *f)
  {
    CRCStructure::SaveStructureToFile(f);
    fwrite(&m_N, sizeof(double), 1, f);
  }

  void Resize(int new_x, int new_y)
  {
    CRCStructure::Resize(new_x, new_y);
    InitMatR();
    InitMatNR();
  }

  void Dorabotka();
  
  double GetN() {return m_N;}
  void SetN(double N) {if (N>0.0) m_N=N;}
};


class CRRCNRStructure : public CRCNRStructure, CPlanarStructure
{
public:
  CRRCNRStructure(double R, double C, int x, int y, double Kf, double N, double Diag) : CRCNRStructure(R, C, x, y, Kf, N), CPlanarStructure(Diag)
  {
    m_RCStructureType = RRCNR;
    MakePlanar(this);
    KPPlanar(this);
  }

  CRRCNRStructure(const CRRCNRStructure& S) : CRCNRStructure(S), CPlanarStructure(S) {}
  CRRCNRStructure(FILE *f) : CRCNRStructure(f), CPlanarStructure(f) {}

  void SaveStructureToFile(FILE *f)
  {
    CRCNRStructure::SaveStructureToFile(f);
    CPlanarStructure::SaveStructureToFile(f);
  }

  CRCStructure* clone() const
  {
    return new CRRCNRStructure(*this);
  }

  void Dorabotka()
  {
    CRCNRStructure::Dorabotka();
    MakePlanar(this);
  }


};


//---------------------------------------------------------------------------
#endif
