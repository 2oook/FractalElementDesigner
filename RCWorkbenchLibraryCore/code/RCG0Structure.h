//---------------------------------------------------------------------------

#ifndef RCG0StructureH
#define RCG0StructureH

#include "RCStructure.h"

class CRCG0Structure : public CRCStructure
{
protected:
  double m_G;
  double m_H;
  void InitMatR();
  int FixElement(int Element);
  void FillGlobalMatrix(complex<double> **Ym, double w);

public:
  CRCG0Structure(double R, double C, int x, int y, double Kf, double G, double H) : CRCStructure(R, C, x, y, Kf, RCG0, 1)
  {
    m_G = G;
    m_H = H;
    InitMatR();
  }

  CRCG0Structure(const CRCG0Structure& S) : CRCStructure(S)
  {
    m_G = S.m_G;
    m_H = S.m_H;
    InitMatR();
  }

  CRCG0Structure(FILE *f) : CRCStructure(f, 1)
  {
    fread(&m_G, sizeof(double), 1, f);
    fread(&m_H, sizeof(double), 1, f);
    InitMatR();
  }

  CRCStructure* clone() const
  {
    return new CRCG0Structure(*this);
  }

  void SaveStructureToFile(FILE *f)
  {
    CRCStructure::SaveStructureToFile(f);
    fwrite(&m_G, sizeof(double), 1, f);
    fwrite(&m_H, sizeof(double), 1, f);
  }

  void Resize(int new_x, int new_y)
  {
    CRCStructure::Resize(new_x, new_y);
    InitMatR();
  }
  
  void Dorabotka();
  double GetG() {return m_G;}
  void SetG(double G) {if (G>0.0) m_G=G;}
  double GetH() {return m_H;}
  void SetH(double H) {if (H>0.0) m_H=H;}

};

//---------------------------------------------------------------------------
#endif

