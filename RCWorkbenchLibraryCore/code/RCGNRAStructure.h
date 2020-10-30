//---------------------------------------------------------------------------

#ifndef RCGNRAStructureH
#define RCGNRAStructureH

#include "RCStructure.h"

class CRCGNRAStructure : public CRCStructure
{
protected:
  double m_G;
  double m_H;
  double m_N;
  double m_P;
  double Mat_r[8][8]; double Mat_rK[8][8];
  double Mat_nr[8][8]; double Mat_nrK[8][8];
  void InitMatR();
  void InitMatNR();
  int FixElement(int Element);
  void FillGlobalMatrix(complex<double> **Ym, double w);

public:
  CRCGNRAStructure(double R, double C, int x, int y, double Kf, double G, double H, double N, double P) : CRCStructure(R, C, x, y, Kf, RCGNRA, 2)
  {
    m_G = G;
    m_H = H;
    m_N = N;
    m_P = P;
    InitMatR();
    InitMatNR();
  }

  CRCGNRAStructure(const CRCGNRAStructure& S) : CRCStructure(S)
  {
    m_G = S.m_G;
    m_H = S.m_H;
    m_N = S.m_N;
    m_P = S.m_P;
    InitMatR();
    InitMatNR();
  }

  CRCGNRAStructure(FILE *f) : CRCStructure(f, 2)
  {
    fread(&m_G, sizeof(double), 1, f);
    fread(&m_H, sizeof(double), 1, f);
    fread(&m_N, sizeof(double), 1, f);
    fread(&m_P, sizeof(double), 1, f);
    InitMatR();
    InitMatNR();
  }

  CRCStructure* clone() const
  {
    return new CRCGNRAStructure(*this);
  }

  void SaveStructureToFile(FILE *f)
  {
    CRCStructure::SaveStructureToFile(f);
    fwrite(&m_G, sizeof(double), 1, f);
    fwrite(&m_H, sizeof(double), 1, f);
    fwrite(&m_N, sizeof(double), 1, f);
    fwrite(&m_P, sizeof(double), 1, f);
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
  double GetP() {return m_P;}
  void SetP(double P) {if (P>0.0) m_P=P;}
};


class CRRCGNRAStructure : public CRCGNRAStructure, CPlanarStructure
{
public:
  CRRCGNRAStructure(double R, double C, int x, int y, double Kf, double G, double H, double N, double P, double Diag) : CRCGNRAStructure(R, C, x, y, Kf, G, H, N, P), CPlanarStructure(Diag)
  {
    m_RCStructureType = RRCGNRA;
    MakePlanar(this);
    KPPlanar(this);
  }

  CRRCGNRAStructure(const CRRCGNRAStructure& S) : CRCGNRAStructure(S), CPlanarStructure(S) {}
  CRRCGNRAStructure(FILE *f) : CRCGNRAStructure(f), CPlanarStructure(f) {}

  void SaveStructureToFile(FILE *f)
  {
    CRCGNRAStructure::SaveStructureToFile(f);
    CPlanarStructure::SaveStructureToFile(f);
  }

  CRCStructure* clone() const
  {
    return new CRRCGNRAStructure(*this);
  }

  void Dorabotka()
  {
    CRCGNRAStructure::Dorabotka();
    MakePlanar(this);
  }
};


//---------------------------------------------------------------------------
#endif
