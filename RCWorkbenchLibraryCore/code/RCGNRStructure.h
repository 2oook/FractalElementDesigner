//---------------------------------------------------------------------------

#ifndef RCGNRStructureH
#define RCGNRStructureH

#include "RCStructure.h"

class CRCGNRStructure : public CRCStructure
{
protected:
  double m_G;
  double m_H;
  double m_N;
  double Mat_r[8][8];
  double Mat_nr[8][8];
  void InitMatR();
  void InitMatNR();

  int FixElement(int Element);
  void FillGlobalMatrix(complex<double> **Ym, double w);

public:
  CRCGNRStructure(double R, double C, int x, int y, double Kf, double G, double H, double N) : CRCStructure(R, C, x, y, Kf, RCGNR, 2)
  {
    m_G = G;
    m_H = H;
    m_N = N;
    InitMatR();
    InitMatNR();
  }

  CRCGNRStructure(const CRCGNRStructure& S) : CRCStructure(S)
  {
    m_G = S.m_G;
    m_H = S.m_H;
    m_N = S.m_N;
    InitMatR();
    InitMatNR();
  }

  CRCGNRStructure(FILE *f) : CRCStructure(f, 2)
  {
    fread(&m_G, sizeof(double), 1, f);
    fread(&m_H, sizeof(double), 1, f);
    fread(&m_N, sizeof(double), 1, f);
    InitMatR();
    InitMatNR();
  }

  CRCStructure* clone() const
  {
    return new CRCGNRStructure(*this);
  }

  void SaveStructureToFile(FILE *f)
  {
    CRCStructure::SaveStructureToFile(f);
    fwrite(&m_G, sizeof(double), 1, f);
    fwrite(&m_H, sizeof(double), 1, f);
    fwrite(&m_N, sizeof(double), 1, f);
  }

  void Resize(int new_x, int new_y)
  {
    CRCStructure::Resize(new_x, new_y);
    InitMatR();
    InitMatNR();
  }

  void Dorabotka();
  
  double GetG() {return m_G;}
  void SetG(double G) {if (G>0.0) m_G=G;}
  double GetH() {return m_H;}
  void SetH(double H) {if (H>0.0) m_H=H;}
  double GetN() {return m_N;}
  void SetN(double N) {if (N>0.0) m_N=N;}
};


class CRRCGNRStructure : public CRCGNRStructure, CPlanarStructure
{
public:
  CRRCGNRStructure(double R, double C, int x, int y, double Kf, double G, double H, double N, double Diag) : CRCGNRStructure(R, C, x, y, Kf, G, H, N), CPlanarStructure(Diag)
  {
    m_RCStructureType = RRCGNR;
    MakePlanar(this);
    KPPlanar(this);
  }

  CRRCGNRStructure(const CRRCGNRStructure& S) : CRCGNRStructure(S), CPlanarStructure(S) {}
  CRRCGNRStructure(FILE *f) : CRCGNRStructure(f), CPlanarStructure(f) {}

  void SaveStructureToFile(FILE *f)
  {
    CRCGNRStructure::SaveStructureToFile(f);
    CPlanarStructure::SaveStructureToFile(f);
  }

  CRCStructure* clone() const
  {
    return new CRRCGNRStructure(*this);
  }

  void Dorabotka()
  {
    CRCGNRStructure::Dorabotka();
    MakePlanar(this);
  }
};


//---------------------------------------------------------------------------
#endif
