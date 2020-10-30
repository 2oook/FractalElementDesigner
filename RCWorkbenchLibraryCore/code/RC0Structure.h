//---------------------------------------------------------------------------

#ifndef RC0StructureH
#define RC0StructureH

#include "RCStructure.h"

class CRC0Structure : public CRCStructure
{
protected:
  void InitMatR();
  int FixElement(int Element);
  void FillGlobalMatrix(complex<double> **Ym, double w);

public:
  CRC0Structure(double R, double C, int x, int y, double Kf) : CRCStructure(R, C, x, y, Kf, RC0, 1)
  {
    InitMatR();
  }

  CRC0Structure(const CRC0Structure& S) : CRCStructure(S)
  {
    InitMatR();
  }

  CRC0Structure(FILE *f) : CRCStructure(f, 1)
  {
    InitMatR();
  }

  CRCStructure* clone() const
  {
    return new CRC0Structure(*this);
  }

  void Resize(int new_x, int new_y)
  {
    CRCStructure::Resize(new_x, new_y);
    InitMatR();
  }
  
  void Dorabotka();
};

//---------------------------------------------------------------------------
#endif

