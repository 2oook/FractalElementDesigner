//---------------------------------------------------------------------------

#ifndef SchemeH
#define SchemeH

#include "RCStructure.h"
#include "TargetFunction.h"
#include "parser\mpParser.h"
//#pragma comment(lib, "muparser/muparser.lib")

using namespace std;

string ConvertFloatToStr(float number);

class CScheme
{
protected:
  const unsigned int m_KPQuantity;

public:
  CScheme(unsigned int KPQuantity) : m_KPQuantity(KPQuantity) {}
  unsigned int GetKPQuantity() {return m_KPQuantity;}
  int Setka(CRCStructureCalculateData *T, int C, double *R, CRCStructure *Structure);
  bool Approximate(CRCStructureCalculateData *T, int m, int n);
  void SetkaApproximated(CRCStructureCalculateData *T, int C, double *R);
  static CScheme* GetScheme(int index, double K, double L);
  //static void ShowScheme(int index, TImage *ImageSch);
  static inline double Magnitude(const complex<double>& Z) {return 20.0*log10(sqrt(norm(Z)));}
  static inline double Phase(const complex<double>& Z) {return 57.295779513082320876798*atan2(Z.imag(), Z.real());}//(Z._M_im, Z._M_re);}

  virtual int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure) = 0;
  virtual int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure) = 0;
  virtual string Params() {return "";}
  virtual bool isActiveScheme() {return false;}
  virtual void SetK(double K) {}
  virtual double GetK() {return 0.0;}
  virtual void SetL(double L) {}
  virtual double GetL() {return 0.0;}
};

class CActiveScheme: public CScheme
{
protected:
  double m_K, m_L;
public:
  CActiveScheme(unsigned int KPQuantity, double K, double L) : CScheme(KPQuantity) {m_K = K; m_L = L;}
  string Params() {return "K: "+ ConvertFloatToStr(m_K)+"; L: "+ ConvertFloatToStr(m_L);}
  bool isActiveScheme() {return true;}
  void SetK(double K) {m_K = K;}
  double GetK() {return m_K;}
  void SetL(double L) {if (L>=0.0) m_L = L;}
  double GetL() {return m_L;}
};

// —хема 1
class CPassive01: public CScheme
{
public:
  CPassive01() : CScheme(2) {}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = -T->m_MatY[i][1]/T->m_MatY[i][2];
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = T->m_MatY[i][2]/(T->m_MatY[i][0]*T->m_MatY[i][2]-T->m_MatY[i][1]*T->m_MatY[i][1]);
    return 0;
  }
};

// —хема 2
class CPassive02: public CScheme
{
public:
  CPassive02() : CScheme(2) {}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = (T->m_MatY[i][0]+T->m_MatY[i][1])/T->m_MatY[i][0];
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = T->m_MatY[i][0]/(T->m_MatY[i][0]*T->m_MatY[i][2]-T->m_MatY[i][1]*T->m_MatY[i][1]);
    return 0;
  }
};

// —хема 3
class CPassive03: public CScheme
{
public:
  CPassive03() : CScheme(2) {}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = (T->m_MatY[i][0]+T->m_MatY[i][1])/(T->m_MatY[i][0]+T->m_MatY[i][1]+T->m_MatY[i][1]+T->m_MatY[i][2]);
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = (T->m_MatY[i][0]+T->m_MatY[i][1]+T->m_MatY[i][1]+T->m_MatY[i][2])/(T->m_MatY[i][0]*T->m_MatY[i][2]-T->m_MatY[i][1]*T->m_MatY[i][1]);
    return 0;
  }
};

// —хема 4
class CARCFilterLowPass01: public CActiveScheme
{
public:
  complex<double> *m_Cond;
  CARCFilterLowPass01(double K, double L) : CActiveScheme(2, K, L)
  {
    m_Cond = new complex<double>[(m_KPQuantity*(m_KPQuantity+1))>>1];
  }
  ~CARCFilterLowPass01()
  {
    delete[] m_Cond;
  }

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    Structure->YParameters(0.0, m_Cond, NULL);
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = m_K*T->m_MatY[i][1]/(m_K*(T->m_MatY[i][1]+T->m_MatY[i][2])-T->m_MatY[i][2]-m_L/m_Cond[0]);
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    Structure->YParameters(0.0, m_Cond, NULL);
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = 1.0/(T->m_MatY[i][0] + (T->m_MatY[i][1]*T->m_MatY[i][1] - m_K*T->m_MatY[i][1]*(T->m_MatY[i][0]+T->m_MatY[i][1]))/(m_K*(T->m_MatY[i][1]+T->m_MatY[i][2])-T->m_MatY[i][2]-m_L/m_Cond[0]));
    return 0;
  }
};

// —хема 5
class CPassive04: public CScheme
{
public:
  double m_L;
  complex<double> *m_Cond;
  CPassive04(double L) : CScheme(2)
  {
    m_L = L;
    m_Cond = new complex<double>[(m_KPQuantity*(m_KPQuantity+1))>>1];
  }
  ~CPassive04()
  {
    delete[] m_Cond;
  }
  string Params() {return "L: "+ConvertFloatToStr(m_L);}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    Structure->YParameters(0.0, m_Cond, NULL);
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = (T->m_MatY[i][1]+T->m_MatY[i][2])/(T->m_MatY[i][2]+m_L/m_Cond[0]);
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    Structure->YParameters(0.0, m_Cond, NULL);
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = (T->m_MatY[i][2]+m_L/m_Cond[0])/((T->m_MatY[i][0]+T->m_MatY[i][1]+T->m_MatY[i][1]+T->m_MatY[i][2])*m_L/m_Cond[0]+T->m_MatY[i][0]*T->m_MatY[i][2]-T->m_MatY[i][1]*T->m_MatY[i][1]);
    return 0;
  }
};

// —хема 6
class CPassive05: public CScheme
{
public:
  CPassive05() : CScheme(4) {}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = -T->m_MatY[i][1]/T->m_MatY[i][4];
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = T->m_MatY[i][4]/(T->m_MatY[i][0]*T->m_MatY[i][4]-T->m_MatY[i][1]*T->m_MatY[i][1]);
    return 0;
  }
};

// —хема 7
class CPassive06: public CScheme
{
public:
  CPassive06() : CScheme(4) {}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
    {
      //complex<double> y11 = T->m_MatY[i][0] - T->m_MatY[i][2]*T->m_MatY[i][2]/T->m_MatY[i][7];
      complex<double> y12 = T->m_MatY[i][1] - T->m_MatY[i][2]*T->m_MatY[i][5]/T->m_MatY[i][7];
      complex<double> y22 = T->m_MatY[i][4] - T->m_MatY[i][5]*T->m_MatY[i][5]/T->m_MatY[i][7];

      T->m_T[i] = -y12/y22;
    }
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
    {
      if (fabs(T->m_MatY[i][1].real()) < 1e-37 || fabs(T->m_MatY[i][8].real()) < 1e-37)
        return -1;

      complex<double> y11 = T->m_MatY[i][0] - T->m_MatY[i][2]*T->m_MatY[i][2]/T->m_MatY[i][7];
      complex<double> y12 = T->m_MatY[i][1] - T->m_MatY[i][2]*T->m_MatY[i][5]/T->m_MatY[i][7];
      complex<double> y22 = T->m_MatY[i][4] - T->m_MatY[i][5]*T->m_MatY[i][5]/T->m_MatY[i][7];
      T->m_T[i] = y22/(y11*y22 - y12*y12);

    }
    return 0;
  }
};

// —хема 8
class CPassive07: public CScheme
{
public:
  CPassive07() : CScheme(4) {}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
    {
      complex<double> y11 = T->m_MatY[i][0] - T->m_MatY[i][3]*T->m_MatY[i][3]/T->m_MatY[i][9];
      complex<double> y12 = T->m_MatY[i][1] - T->m_MatY[i][3]*T->m_MatY[i][6]/T->m_MatY[i][9];
      complex<double> y22 = T->m_MatY[i][4] - T->m_MatY[i][6]*T->m_MatY[i][6]/T->m_MatY[i][9];

      T->m_T[i] = -y12/y22;
    }
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
    {
      complex<double> y11 = T->m_MatY[i][0] - T->m_MatY[i][3]*T->m_MatY[i][3]/T->m_MatY[i][9];
      complex<double> y12 = T->m_MatY[i][1] - T->m_MatY[i][3]*T->m_MatY[i][6]/T->m_MatY[i][9];
      complex<double> y22 = T->m_MatY[i][4] - T->m_MatY[i][6]*T->m_MatY[i][6]/T->m_MatY[i][9];

      T->m_T[i] = y22/(y11*y22 - y12*y12);
    }
    return 0;
  }
};

// —хема 9
class CARCFilterLowPass02: public CActiveScheme
{
public:
  complex<double> *m_Cond;
  CARCFilterLowPass02(double K, double L) : CActiveScheme(3, K, L)
  {
    m_Cond = new complex<double>[(m_KPQuantity*(m_KPQuantity+1))>>1];
  }
  ~CARCFilterLowPass02()
  {
    delete[] m_Cond;
  }

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    Structure->YParameters(0.0, m_Cond, NULL);
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = -m_K*T->m_MatY[i][1]/(m_K*T->m_MatY[i][4]+T->m_MatY[i][3]+m_L/m_Cond[0]);
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    Structure->YParameters(0.0, m_Cond, NULL);
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = 1.0/(T->m_MatY[i][0] - (m_K*T->m_MatY[i][1]*T->m_MatY[i][2]+T->m_MatY[i][1]*T->m_MatY[i][1])/(m_K*T->m_MatY[i][4]+T->m_MatY[i][3]+m_L/m_Cond[0]));
    return 0;
  }
};

// —хема 10
class CARCFilterLowPass03: public CActiveScheme
{
public:
  complex<double> *m_Cond;
  CARCFilterLowPass03(double K, double L) : CActiveScheme(4, K, L)
  {
    m_Cond = new complex<double>[(m_KPQuantity*(m_KPQuantity+1))>>1];
  }
  ~CARCFilterLowPass03()
  {
    delete[] m_Cond;
  }

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    Structure->YParameters(0.0, m_Cond, NULL);
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = -m_K*T->m_MatY[i][1]/(m_K*T->m_MatY[i][5]+T->m_MatY[i][4]+m_L/m_Cond[0]);
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    Structure->YParameters(0.0, m_Cond, NULL);
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = 1.0/(T->m_MatY[i][0] - (m_K*T->m_MatY[i][1]*T->m_MatY[i][2]+T->m_MatY[i][1]*T->m_MatY[i][1])/(m_K*T->m_MatY[i][5]+T->m_MatY[i][4]+m_L/m_Cond[0]));
    return 0;
  }
};


// —хема 11
class CARCFilterHighPass01: public CActiveScheme
{
public:
  complex<double> *m_Cond;
  CARCFilterHighPass01(double K, double L) : CActiveScheme(4, K, L)
  {
    m_Cond = new complex<double>[(m_KPQuantity*(m_KPQuantity+1))>>1];
  }
  ~CARCFilterHighPass01()
  {
    delete[] m_Cond;
  }

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    Structure->YParameters(0.0, m_Cond, NULL);
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = -m_K*T->m_MatY[i][8]/(m_K*(T->m_MatY[i][3]+T->m_MatY[i][6])+T->m_MatY[i][9]+m_L/m_Cond[0]);
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    Structure->YParameters(0.0, m_Cond, NULL);
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = 1.0/(T->m_MatY[i][7] - (m_K*T->m_MatY[i][8]*(T->m_MatY[i][2]+T->m_MatY[i][5])+T->m_MatY[i][8]*T->m_MatY[i][8])/(m_K*(T->m_MatY[i][3]+T->m_MatY[i][6])+T->m_MatY[i][9]+m_L/m_Cond[0]));
    return 0;
  }
};

// —хема 12
class CARCFilterBandStop01: public CActiveScheme
{
public:
  complex<double> *m_Cond;
  CARCFilterBandStop01(double K, double L) : CActiveScheme(4, K, L)
  {
    m_Cond = new complex<double>[(m_KPQuantity*(m_KPQuantity+1))>>1];
  }
  ~CARCFilterBandStop01()
  {
    delete[] m_Cond;
  }

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    Structure->YParameters(0.0, m_Cond, NULL);
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = -m_K*(T->m_MatY[i][2]+T->m_MatY[i][5])/(m_K*T->m_MatY[i][8]+T->m_MatY[i][7]+m_L/m_Cond[0]);
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    complex<double> y1323;
    Structure->YParameters(0.0, m_Cond, NULL);
    for (int i=0; i<T->m_length; ++i)
    {
      y1323 = T->m_MatY[i][2] + T->m_MatY[i][5];
      T->m_T[i] = 1.0/(T->m_MatY[i][0]+T->m_MatY[i][1] - (m_K*y1323*(T->m_MatY[i][3]+T->m_MatY[i][6])+y1323*y1323)/(m_K*T->m_MatY[i][8]+T->m_MatY[i][7]+m_L/m_Cond[0]));
    }
    return 0;
  }
};

// —хема 13
class CPassive08: public CScheme
{
public:
  CPassive08() : CScheme(4) {}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = 1.0;
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    complex<double> y11,y12,y13,y22,y23,y33;
    for (int i=0; i<T->m_length; ++i)
    {
      // заполнение матрицы схемы
      y11 = T->m_MatY[i][9];
      y12 = T->m_MatY[i][8];
      y13 = T->m_MatY[i][6];
      y22 = T->m_MatY[i][4]+T->m_MatY[i][7];
      y23 = T->m_MatY[i][5]*2.0;
      y33 = y22;

      // понижение пор€дка
      y11 -= y13*y13/y33;
      y12 -= y13*y23/y33;
      y22 -= y23*y23/y33;

      // понижение пор€дка и получение T
      T->m_T[i] = y22/(y11*y22-y12*y12);
    }
    return 0;
  }
};

// —хема 14
class CPassive09: public CScheme
{
public:
  CPassive09() : CScheme(4) {}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = 1.0;
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    complex<double> Y11,Y12,Y13,Y22,Y23,Y33;
    for (int i=0; i<T->m_length; ++i)
    {
      // заполнение матрицы схемы
      Y11 = T->m_MatY[i][0];
      Y12 = T->m_MatY[i][1];
      Y13 = T->m_MatY[i][3];
      Y22 = T->m_MatY[i][4];
      Y23 = T->m_MatY[i][6];
      Y33 = T->m_MatY[i][9];

      // понижение пор€дка
      Y11 -= Y13*Y13/Y33;
      Y12 -= Y13*Y23/Y33;
      Y22 -= Y23*Y23/Y33;

      // понижение пор€дка и получение T
      T->m_T[i] = Y22/(Y11*Y22-Y12*Y12);
    }
    return 0;
  }
};

// —хема 15
class CPassive10: public CScheme
{
public:
  CPassive10() : CScheme(4) {}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = 1.0;
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    complex<double> Y11,Y12,Y13,Y22,Y23,Y33;
    for (int i=0; i<T->m_length; ++i)
    {
      // заполнение матрицы схемы
      Y11 = T->m_MatY[i][0];
      Y12 = T->m_MatY[i][1];
      Y13 = T->m_MatY[i][2];
      Y22 = T->m_MatY[i][4];
      Y23 = T->m_MatY[i][5];
      Y33 = T->m_MatY[i][7];

      // понижение пор€дка
      Y11 -= Y13*Y13/Y33;
      Y12 -= Y13*Y23/Y33;
      Y22 -= Y23*Y23/Y33;

      // понижение пор€дка и получение T
      T->m_T[i] = Y22/(Y11*Y22-Y12*Y12);
    }
    return 0;
  }
};

// —хема 16
class CPassive11: public CScheme
{
public:
  CPassive11() : CScheme(4) {}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = 1.0;
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
    {
      // понижение пор€дка и получение T
      T->m_T[i] = T->m_MatY[i][7]/(T->m_MatY[i][0]*T->m_MatY[i][7]-T->m_MatY[i][2]*T->m_MatY[i][2]);
    }
    return 0;
  }
};

// —хема 17
class CPassive12: public CScheme
{
public:
  CPassive12() : CScheme(4) {}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = 1.0;
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
    {
      T->m_T[i] = 1.0/T->m_MatY[i][0];
    }
    return 0;
  }
};

// —хема 18
class CPassive13: public CScheme
{
public:
  CPassive13() : CScheme(4) {}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = 1.0;
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
    {
      // понижение пор€дка и получение T
      T->m_T[i] = T->m_MatY[i][4]/(T->m_MatY[i][0]*T->m_MatY[i][4]-T->m_MatY[i][1]*T->m_MatY[i][1]);
    }
    return 0;
  }
};

// —хема 19
class CPassive14: public CScheme
{
public:
  CPassive14() : CScheme(4) {}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = 1.0;
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    complex<double> Y11,Y12,Y22;
    for (int i=0; i<T->m_length; ++i)
    {
      // заполнение матрицы схемы
      Y11 = T->m_MatY[i][0];
      Y12 = T->m_MatY[i][1]+T->m_MatY[i][3];
      Y22 = T->m_MatY[i][4]+T->m_MatY[i][9];// ошибка? нужно добавить T->m_MatY[i][6] ???

      // понижение пор€дка и получение T
      T->m_T[i] = Y22/(Y11*Y22-Y12*Y12);
    }
    return 0;
  }
};

// —хема 20
class CPassive15: public CScheme
{
public:
  CPassive15() : CScheme(6) {}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = T->m_MatY[i][0];
    return 0;
  }
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = T->m_MatY[i][0];
    return 0;
  }
};

// —хема 21
class CPassive16: public CScheme
{
public:
  CPassive16() : CScheme(8) {}

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
  {
    for (int i=0; i<T->m_length; ++i)
      T->m_T[i] = 1.0;
    return 0;
  }

  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure);
};

// shema formula
class CPassive17: public CScheme
{
  mup::ParserX p;
  
public:
  CPassive17(const string& formula) : CScheme(0)
  {
    ParseFormula(formula);
  }

  int CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure);
  int CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure);
  void ParseFormula(const string& formula);
};

//---------------------------------------------------------------------------
#endif

