//---------------------------------------------------------------------------

#ifndef RCStructureH
#define RCStructureH

# define M_PI           3.14159265358979323846  /* pi */

#include <stdio.h> // нужно дл€ FILE
#include <complex>

#define NODENULL (-1)
#define NODESHUNT (-2)

#define KPNONE (0)
#define KPNORMAL (1)
#define KPRESTRICT (2)
#define KPSHUNT (3)

#define AddElement44ToMatrix(Element) {          \
  int u1 = m_pCircuitNodes[0][i  ][j  ];         \
  int u2 = m_pCircuitNodes[0][i+1][j  ];         \
  int u3 = m_pCircuitNodes[0][i  ][j+1];         \
  int u4 = m_pCircuitNodes[0][i+1][j+1];         \
                                                 \
  AddToMatrix(Ym, u1, u1, Element[0][0], false); \
  AddToMatrix(Ym, u1, u2, Element[0][1], true);  \
  AddToMatrix(Ym, u1, u3, Element[0][2], true);  \
  AddToMatrix(Ym, u1, u4, Element[0][3], true);  \
                                                 \
  AddToMatrix(Ym, u2, u2, Element[1][1], false); \
  AddToMatrix(Ym, u2, u3, Element[1][2], true);  \
  AddToMatrix(Ym, u2, u4, Element[1][3], true);  \
                                                 \
  AddToMatrix(Ym, u3, u3, Element[2][2], false); \
  AddToMatrix(Ym, u3, u4, Element[2][3], true);  \
                                                 \
  AddToMatrix(Ym, u4, u4, Element[3][3], false); \
}
// 0 - первый слой 1 - второй слой (нижний)
// две матрицы 2х2 друг под другом (всего 8 €чеек)
#define AddElement88ToMatrix(Element) {          \
  int u1 = m_pCircuitNodes[0][i  ][j  ];         \
  int u2 = m_pCircuitNodes[0][i+1][j  ];         \
  int u3 = m_pCircuitNodes[0][i  ][j+1];         \
  int u4 = m_pCircuitNodes[0][i+1][j+1];         \
  int u5 = m_pCircuitNodes[1][i  ][j  ];         \
  int u6 = m_pCircuitNodes[1][i+1][j  ];         \
  int u7 = m_pCircuitNodes[1][i  ][j+1];         \
  int u8 = m_pCircuitNodes[1][i+1][j+1];         \
                                                 \
  AddToMatrix(Ym, u1, u1, Element[0][0], false); \
  AddToMatrix(Ym, u1, u2, Element[0][1], true);  \
  AddToMatrix(Ym, u1, u3, Element[0][2], true);  \
  AddToMatrix(Ym, u1, u4, Element[0][3], true);  \
  AddToMatrix(Ym, u1, u5, Element[0][4], true);  \
  AddToMatrix(Ym, u1, u6, Element[0][5], true);  \
  AddToMatrix(Ym, u1, u7, Element[0][6], true);  \
  AddToMatrix(Ym, u1, u8, Element[0][7], true);  \
                                                 \
  AddToMatrix(Ym, u2, u2, Element[1][1], false); \
  AddToMatrix(Ym, u2, u3, Element[1][2], true);  \
  AddToMatrix(Ym, u2, u4, Element[1][3], true);  \
  AddToMatrix(Ym, u2, u5, Element[1][4], true);  \
  AddToMatrix(Ym, u2, u6, Element[1][5], true);  \
  AddToMatrix(Ym, u2, u7, Element[1][6], true);  \
  AddToMatrix(Ym, u2, u8, Element[1][7], true);  \
                                                 \
  AddToMatrix(Ym, u3, u3, Element[2][2], false); \
  AddToMatrix(Ym, u3, u4, Element[2][3], true);  \
  AddToMatrix(Ym, u3, u5, Element[2][4], true);  \
  AddToMatrix(Ym, u3, u6, Element[2][5], true);  \
  AddToMatrix(Ym, u3, u7, Element[2][6], true);  \
  AddToMatrix(Ym, u3, u8, Element[2][7], true);  \
                                                 \
  AddToMatrix(Ym, u4, u4, Element[3][3], false); \
  AddToMatrix(Ym, u4, u5, Element[3][4], true);  \
  AddToMatrix(Ym, u4, u6, Element[3][5], true);  \
  AddToMatrix(Ym, u4, u7, Element[3][6], true);  \
  AddToMatrix(Ym, u4, u8, Element[3][7], true);  \
                                                 \
  AddToMatrix(Ym, u5, u5, Element[4][4], false); \
  AddToMatrix(Ym, u5, u6, Element[4][5], true);  \
  AddToMatrix(Ym, u5, u7, Element[4][6], true);  \
  AddToMatrix(Ym, u5, u8, Element[4][7], true);  \
                                                 \
  AddToMatrix(Ym, u6, u6, Element[5][5], false); \
  AddToMatrix(Ym, u6, u7, Element[5][6], true);  \
  AddToMatrix(Ym, u6, u8, Element[5][7], true);  \
                                                 \
  AddToMatrix(Ym, u7, u7, Element[6][6], false); \
  AddToMatrix(Ym, u7, u8, Element[6][7], true);  \
                                                 \
  AddToMatrix(Ym, u8, u8, Element[7][7], false); \
}

using namespace std;

struct TMasKP
{
  int KPType, KPNumber, x, y, ux, uy;
  TMasKP *prev, *next;
};

struct T3DPoint
{
  int z,x,y;
  T3DPoint() {}
  T3DPoint(int Z, int X, int Y)
  {
    z = Z;
    x = X;
    y = Y;
  }
};

enum EnumRCStructureType
{
  RCAbstract = 0,
  RC0    = 0x20304352,
  RCG0   = 0x30474352,
  RCNR   = 0x524E4352,
  RCGNR  = 0x4E474352,
  RCGNRA = 0x41474352,
  RRCNR  = 0x4E435252,
  RRCGNR = 0x47435252,
  RRCGNRA= 0x48435252,
};

enum EnumRCElementType
{
  RCET_NONE = -1,// инструмент "стрелка" либо не выбранный инструмент // при отмене инструмента тип инструмента не сбрасываетс€
  RCET_EMPTY = 0,
  RCET_R = 1,
  RCET_RC = 3,
  RCET_KPNORMAL = 8,
  RCET_KPRESTRICT = 16,
  RCET_KPSHUNT = 24,
  RCET_MASKEMPTY = 256,
  RCET_MASKR = 257,
  RCET_MASKRC = 259,
};

class CRCStructure
{
  friend class CPlanarStructure;

protected:
  double m_R;
  double m_C;
  EnumRCStructureType m_RCStructureType; // тип структуры
  int m_Layers;     // количество слоев
  int m_x;          // количество €чеек по горизонтали
  int m_y;          // количество €чеек по вертикали
  int m_Perimeter;  // (x+y)*2
  double m_Kf;      // коэффициент формы
  int *m_pKPQuantity; // количество пронумерованных  ѕ
  TMasKP **m_pMasKP; // массив данных о  ѕ
  int **m_pMatrix;  // матрица элементов
  int **m_pMatrixMask; // маска матрицы элементов
  double Mat_r[4][4];
  int m_Nodes;
  int ***m_pCircuitNodes;//3-х мерна€ матрица узлов
  int *m_pRowSizes;

  void InitMasKP(TMasKP **MasKP);
  TMasKP* KPBeginMove(TMasKP *KP_begin, int value, bool isfit);
  TMasKP* KPEndMove(TMasKP *KP_end, int value, bool isfit);
  TMasKP* Wave(int ***pWave, TMasKP *kp);
  void DoNumerate(int Layer, int L, int KPNumber);

  int RecursiveNumering(T3DPoint *pBegin, int BeginNum, int u);
  void Poradok(complex<double> **Ym);
  virtual void FillGlobalMatrix(complex<double> **Ym, double w) = 0;
  virtual int FixElement(int Element) = 0;
  void AddToMatrix(complex<double> **Ym, int u0, int u1, const complex<double>& value, bool f);
  void KPResizer(int new_size, int old_size, TMasKP *new_KP, TMasKP *old_KP);

public:
  CRCStructure(double R, double C, int x, int y, double Kf, EnumRCStructureType RCStructureType=RCAbstract, int Layers=0);
  CRCStructure() {}
  CRCStructure(const CRCStructure& S);
  CRCStructure(FILE *f, int Layers=0);
  virtual CRCStructure* clone() const = 0;
  virtual ~CRCStructure();

  inline int Width() {return m_x;}
  inline int Height() {return m_y;}
  inline double GetR() {return m_R;}
  inline void SetR(double R) {if (R>0.0) m_R=R;}
  inline double GetC() {return m_C;}
  inline void SetC(double C) {if (C>0.0) m_C=C;}
  inline double GetKf() {return m_Kf;}
  inline void SetKf(double Kf) {if (Kf>0.0) m_Kf=Kf;}
  inline bool RLayer() {return m_R;}
  inline int Perimeter() {return m_Perimeter;}
  inline int GetKPType(int Layer, int L) {return m_pMasKP[Layer][L].KPType;}
  inline int GetKPNumber(int Layer, int L) {return m_pMasKP[Layer][L].KPNumber;}
  inline EnumRCElementType GetElementType(int x, int y, int Layer) {return (EnumRCElementType)(m_pMatrix[x][y] >> (Layer<<1)&3);}
  inline int GetElementRawValue(int x, int y) {return m_pMatrix[x][y];}
  inline bool GetElementTypeMask(int x, int y, int Layer) {return m_pMatrixMask[x][y] >> (Layer<<1)&3;}

  void CopyStructure(CRCStructure *S);
  bool SetElementType(int Layer, int x, int y, EnumRCElementType ElementType);
  void ClearKPNumbers();
  unsigned int GetKPQuantity();
  int GetKPSummaryLength();
  void ResizeKP(int prob, int max_value, bool isfit);
  void MoveKP(int prob, int max_value, bool isfit);
  bool Cross(CRCStructure *S, int prob, int MaxWidth, int MaxHeight, bool ExchangeLayers);
  void Mutate(int prob_res, int prob_cond, bool isfit, int x, int y, int width, int height, int prob1);
  int YParameters(double w, complex<double> *Dest);
  void KPV();
  void KPH();
  void WaveCast(int ***pWave, int *pShortWay, int *pLongWay);
  bool CheckKP();
  bool Equal(CRCStructure *S);
  void ElementsToNodes();
  void Swap(CRCStructure *S);

  virtual void Resize(int new_x, int new_y);
  virtual double GetG() {return 0.0;}
  virtual void SetG(double G) {}
  virtual double GetH() {return 1e37;}
  virtual void SetH(double H) {}
  virtual double GetN() {return 0.0;}
  virtual void SetN(double N) {}
  virtual double GetP() {return 1e37;}
  virtual void SetP(double P) {}
  virtual void SaveStructureToFile(FILE *f);
  virtual void Dorabotka() = 0;
};


class CPlanarStructure
{
protected:
  double m_dDiag;

public:
  CPlanarStructure(double Diag)
  {
    m_dDiag = Diag;
  }

  CPlanarStructure(const CPlanarStructure& S)
  {
    m_dDiag = S.m_dDiag;
  }

  CPlanarStructure(FILE *f)
  {
    fread(&m_dDiag, sizeof(m_dDiag), 1, f);
  }

  void SaveStructureToFile(FILE *f)
  {
    fwrite(&m_dDiag, sizeof(m_dDiag), 1, f);
  }

  void MakePlanar(CRCStructure *Structure)
  {
    int diag = m_dDiag*Structure->m_y;
    for (int i=0; i<Structure->m_x; ++i)
      Structure->m_pMatrix[i][diag] = 4;
  }

  void KPPlanar(CRCStructure *Structure)
  {
    for (int l=0; l<Structure->m_Layers; ++l)
    {
      for (int i=0; i<Structure->m_Perimeter; ++i)
      {
        Structure->m_pMasKP[l][i].KPType = KPNONE;
        Structure->m_pMasKP[l][i].KPNumber = 0;
      }
    }

    Structure->m_pMasKP[0][0].KPType = KPRESTRICT;
    Structure->m_pMasKP[0][0].KPNumber = 0;
    Structure->m_pMasKP[0][Structure->m_x-1].KPType = KPRESTRICT;
    Structure->m_pMasKP[0][Structure->m_x-1].KPNumber = 0;
    Structure->m_pMasKP[0][Structure->m_x+Structure->m_y].KPType = KPRESTRICT;
    Structure->m_pMasKP[0][Structure->m_x+Structure->m_y].KPNumber = 0;
    Structure->m_pMasKP[0][Structure->m_Perimeter-1-Structure->m_y].KPType = KPRESTRICT;
    Structure->m_pMasKP[0][Structure->m_Perimeter-1-Structure->m_y].KPNumber = 0;

    Structure->m_pMasKP[1][0].KPType = KPRESTRICT;
    Structure->m_pMasKP[1][0].KPNumber = 0;
    Structure->m_pMasKP[1][Structure->m_x-1].KPType = KPRESTRICT;
    Structure->m_pMasKP[1][Structure->m_x-1].KPNumber = 0;
    Structure->m_pMasKP[1][Structure->m_x+Structure->m_y].KPType = KPRESTRICT;
    Structure->m_pMasKP[1][Structure->m_x+Structure->m_y].KPNumber = 0;
    Structure->m_pMasKP[1][Structure->m_Perimeter-1-Structure->m_y].KPType = KPRESTRICT;
    Structure->m_pMasKP[1][Structure->m_Perimeter-1-Structure->m_y].KPNumber = 0;

    int diag = Structure->m_y*m_dDiag;
    for (int i=0; i<Structure->m_y; ++i)
    {
      Structure->m_pMasKP[0][Structure->m_x+i].KPType = KPNORMAL;
      Structure->m_pMasKP[1][Structure->m_x+i].KPType = KPNORMAL;
      if (i<diag)
        Structure->m_pMasKP[0][Structure->m_x+i].KPNumber = 2;
      else
        Structure->m_pMasKP[0][Structure->m_x+i].KPNumber = 4;
      Structure->m_pMasKP[1][Structure->m_x+i].KPNumber = 6;

      if (i == diag)
      {
        Structure->m_pMasKP[0][Structure->m_x+i].KPType = KPRESTRICT;
        Structure->m_pMasKP[0][Structure->m_x+i].KPNumber = 0;
      }
    }

    for (int i=0; i<Structure->m_y; ++i)
    {
      Structure->m_pMasKP[0][Structure->m_Perimeter-1-i].KPType = KPNORMAL;
      Structure->m_pMasKP[1][Structure->m_Perimeter-1-i].KPType = KPNORMAL;
      if (i<diag)
        Structure->m_pMasKP[0][Structure->m_Perimeter-1-i].KPNumber = 1;
      else
        Structure->m_pMasKP[0][Structure->m_Perimeter-1-i].KPNumber = 3;
      Structure->m_pMasKP[1][Structure->m_Perimeter-1-i].KPNumber = 5;

      if (i == diag)
      {
        Structure->m_pMasKP[0][Structure->m_Perimeter-1-i].KPType = KPRESTRICT;
        Structure->m_pMasKP[0][Structure->m_Perimeter-1-i].KPNumber = 0;
      }
    }

    Structure->m_pKPQuantity[0] = 4;
    Structure->m_pKPQuantity[1] = 2;

  }

};

//---------------------------------------------------------------------------
#endif

