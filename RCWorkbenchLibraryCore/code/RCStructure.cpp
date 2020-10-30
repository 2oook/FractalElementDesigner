//---------------------------------------------------------------------------

//#include <classes.hpp>
#pragma hdrstop

#include "RCStructure.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)

class TPoint
{
public:

    TPoint()
    {
        this->x = 0;
        this->y = 0;
    }

    TPoint(int _x, int _y)
    {
        this->x = _x;
        this->y = _y;
    }

    int x;
    int y;

};

// конструктор создания структуры, параметры вводятся вручную
CRCStructure::CRCStructure(double R, double C, int x, int y, double Kf, EnumRCStructureType RCStructureType, int Layers) : m_RCStructureType(RCStructureType), m_Layers(Layers)
{
  m_R = R;
  m_C = C;
  m_x=x;
  m_y=y;
  int FillValue = 2 | ((1 << ((m_Layers << 1) - 1)) - 1); // (2 | ...) чтобы для RC0 структуры заполняющее значение было равно 3, а не 1

  m_pMatrix = new int*[m_x];
  m_pMatrixMask = new int*[m_x];
  for (int i=0; i<m_x; ++i)
  {
    m_pMatrix[i] = new int[m_y];
    m_pMatrixMask[i] = new int[m_y];
    for (int j=0; j<m_y; ++j)
    {
      m_pMatrix[i][j] = FillValue;
      m_pMatrixMask[i][j] = 0;
    }
  }
  m_Kf = (double)m_x/(double)m_y;
  m_Perimeter = (m_x+m_y)<<1;

  m_pKPQuantity = new int[m_Layers];
  m_pMasKP = new TMasKP*[m_Layers];
  for (int l=0; l<m_Layers; ++l)
  {
    m_pKPQuantity[l] = 0;
    m_pMasKP[l] = new TMasKP[m_Perimeter];
  }
  InitMasKP(m_pMasKP);

  m_pCircuitNodes = new int**[m_Layers];
  for (int l=0; l<m_Layers; ++l)
  {
    m_pCircuitNodes[l] = new int*[m_x+1];
    for (int i=0; i<=m_x; ++i)
    {
      m_pCircuitNodes[l][i] = new int[m_y+1];
    }
  }

  m_pRowSizes = new int[(m_x+1)*(m_y+1)*m_Layers];

}

// конструктор создания структуры, параметры берутся из уже созданной структуры
CRCStructure::CRCStructure(const CRCStructure& S)
{
  m_RCStructureType = S.m_RCStructureType;
  m_R = S.m_R;
  m_C = S.m_C;
  m_Layers = S.m_Layers;
  m_x = S.m_x;
  m_y = S.m_y;

  m_pMatrix = new int*[m_x];
  m_pMatrixMask = new int*[m_x];
  for (int i=0; i<m_x; ++i)
  {
    m_pMatrix[i] = new int[m_y];
    m_pMatrixMask[i] = new int[m_y];
    memcpy(m_pMatrix[i], S.m_pMatrix[i], m_y*sizeof(int));
    memcpy(m_pMatrixMask[i], S.m_pMatrixMask[i], m_y*sizeof(int));
  }
  m_Kf = S.m_Kf;
  m_Perimeter = S.m_Perimeter;

  m_pMasKP = new TMasKP*[m_Layers];
  m_pKPQuantity = new int[m_Layers];
  for (int l=0; l<m_Layers; ++l)
  {
    m_pKPQuantity[l] = S.m_pKPQuantity[l];
    m_pMasKP[l] = new TMasKP[m_Perimeter];
  }
  InitMasKP(m_pMasKP);

  for (int l=0; l<m_Layers; ++l)
    for (int i=0; i<m_Perimeter; ++i)
    {
      m_pMasKP[l][i].KPType = S.m_pMasKP[l][i].KPType;
      m_pMasKP[l][i].KPNumber = S.m_pMasKP[l][i].KPNumber;
    }

  m_pCircuitNodes = new int**[m_Layers];
  for (int l=0; l<m_Layers; ++l)
  {
    m_pCircuitNodes[l] = new int*[m_x+1];
    for (int i=0; i<=m_x; ++i)
    {
      m_pCircuitNodes[l][i] = new int[m_y+1];
    }
  }

  m_pRowSizes = new int[(m_x+1)*(m_y+1)*m_Layers];

}

// конструктор создания структуры, параметры читаются из файла
CRCStructure::CRCStructure(FILE *f, int Layers) : m_Layers(Layers)
{
  fseek(f, 0, SEEK_SET);
  fread(&m_RCStructureType, sizeof(EnumRCStructureType), 1, f);
  fread(&m_R, sizeof(double), 1, f);
  fread(&m_C, sizeof(double), 1, f);
  fread(&m_x, sizeof(int), 1, f);
  fread(&m_y, sizeof(int), 1, f);
  fread(&m_Kf, sizeof(double), 1, f);

  m_pMatrix = new int*[m_x];
  m_pMatrixMask = new int*[m_x];
  for (int i=0; i<m_x; ++i)
  {
    m_pMatrix[i] = new int[m_y];
    m_pMatrixMask[i] = new int[m_y];
    fread(m_pMatrix[i], sizeof(int), m_y, f);

    for (int j=0; j<m_y; ++j)
    {
      m_pMatrixMask[i][j] = 0;
    }
  }

  m_Perimeter = (m_x+m_y)<<1;
  m_pKPQuantity = new int[m_Layers];
  m_pMasKP = new TMasKP*[m_Layers];
  for (int l=0; l<m_Layers; ++l)
  {
    fread(&m_pKPQuantity[l], sizeof(int), 1, f);  // сначала в m_KPQuantity хранится сумма длин всех КП в слое, а потом кол-во КП в слое
    m_pMasKP[l] = new TMasKP[m_Perimeter];
  }
  InitMasKP(m_pMasKP);

  int L;
  for (int l=0; l<m_Layers; ++l)
  {
    for (int i=0; i<m_pKPQuantity[l]; ++i)
    {
      fread(&L, sizeof(int), 1, f);
      fread(&m_pMasKP[l][L].KPType, sizeof(int), 1, f);
      fread(&m_pMasKP[l][L].KPNumber, sizeof(int), 1, f);
    }

    // подсчитываем количество КП с типом 1
    TMasKP *temp = m_pMasKP[l];
    int nKP = 0;
    while (temp->KPType == KPNORMAL)
      temp = temp->next;
    TMasKP *KPBegin = temp;
    temp = temp->next;
    while (temp!=KPBegin)
    {
      while (temp!=KPBegin && temp->KPType != KPNORMAL)
        temp = temp->next;
      if (temp!=KPBegin)
        ++nKP;
      while (temp!=KPBegin && temp->KPType == KPNORMAL)
        temp = temp->next;
    }
    m_pKPQuantity[l] = nKP;
  }

  m_pCircuitNodes = new int**[m_Layers];
  for (int l=0; l<m_Layers; ++l)
  {
    m_pCircuitNodes[l] = new int*[m_x+1];
    for (int i=0; i<=m_x; ++i)
    {
      m_pCircuitNodes[l][i] = new int[m_y+1];
    }
  }

  m_pRowSizes = new int[(m_x+1)*(m_y+1)*m_Layers];

}

// деструктор структуры
CRCStructure::~CRCStructure()
{
  delete[] m_pKPQuantity;
  for(int i=0; i<m_Layers; delete[] m_pMasKP[i++]);
  delete[] m_pMasKP;

  for(int i=0; i<m_x; delete[] m_pMatrix[i++]);
  delete[] m_pMatrix;
  for(int i=0; i<m_x; delete[] m_pMatrixMask[i++]);
  delete[] m_pMatrixMask;

  for (int l=0; l<m_Layers; delete[] m_pCircuitNodes[l++])
    for (int i=0; i<=m_x; delete[] m_pCircuitNodes[l][i++]);
  delete[] m_pCircuitNodes;
  delete[] m_pRowSizes;
}

// инициализация массива контактных площадок
void CRCStructure::InitMasKP(TMasKP **MasKP)
{
  for (int l=0; l<m_Layers; ++l)
  {
    int i=0;
    int j=0;
    int k=1;

    MasKP[l][0].KPType = 0;
    MasKP[l][0].KPNumber = 0;
    MasKP[l][0].x = i;
    MasKP[l][0].y = j;
    MasKP[l][0].ux = i;
    MasKP[l][0].uy = j;
    MasKP[l][0].prev = &MasKP[l][m_Perimeter-1];
    MasKP[l][0].next = &MasKP[l][1];
    ++i;

    while (i<m_x)
    {
      MasKP[l][k].KPType = 0;
      MasKP[l][k].KPNumber = 0;
      MasKP[l][k].x = i;
      MasKP[l][k].y = j;
      MasKP[l][k].ux = i;
      MasKP[l][k].uy = j;
      MasKP[l][k].prev = &MasKP[l][k-1];
      MasKP[l][k].next = &MasKP[l][k+1];
      ++i;
      ++k;
    }

    --i;
    while (j<m_y)
    {
      MasKP[l][k].KPType = 0;
      MasKP[l][k].KPNumber = 0;
      MasKP[l][k].x = i;
      MasKP[l][k].y = j;
      MasKP[l][k].ux = m_x;
      MasKP[l][k].uy = j;
      MasKP[l][k].prev = &MasKP[l][k-1];
      MasKP[l][k].next = &MasKP[l][k+1];
      ++j;
      ++k;
    }

    --j;
    while (i>=0)
    {
      MasKP[l][k].KPType = 0;
      MasKP[l][k].KPNumber = 0;
      MasKP[l][k].x = i;
      MasKP[l][k].y = j;
      MasKP[l][k].ux = i+1;
      MasKP[l][k].uy = m_y;
      MasKP[l][k].prev = &MasKP[l][k-1];
      MasKP[l][k].next = &MasKP[l][k+1];
      --i;
      ++k;
    }

    ++i;
    while (j>0)
    {
      MasKP[l][k].KPType = 0;
      MasKP[l][k].KPNumber = 0;
      MasKP[l][k].x = i;
      MasKP[l][k].y = j;
      MasKP[l][k].ux = i;
      MasKP[l][k].uy = j+1;
      MasKP[l][k].prev = &MasKP[l][k-1];
      MasKP[l][k].next = &MasKP[l][k+1];
      --j;
      ++k;
    }

    MasKP[l][k].KPType = 0;
    MasKP[l][k].KPNumber = 0;
    MasKP[l][k].x = i;
    MasKP[l][k].y = j;
    MasKP[l][k].ux = i;
    MasKP[l][k].uy = j+1;
    MasKP[l][k].prev = &MasKP[l][k-1];
    MasKP[l][k].next = &MasKP[l][0];
  }
}

// сохранение структуры в файл
void CRCStructure::SaveStructureToFile(FILE *f)
{
  fwrite(&m_RCStructureType, sizeof(EnumRCStructureType), 1, f);
  fwrite(&m_R, sizeof(double), 1, f);
  fwrite(&m_C, sizeof(double), 1, f);
  fwrite(&m_x, sizeof(int), 1, f);
  fwrite(&m_y, sizeof(int), 1, f);
  fwrite(&m_Kf, sizeof(double), 1, f);
  for (int i=0; i<m_x; ++i)
    fwrite(m_pMatrix[i], sizeof(int), m_y, f);

  int L;
  for (int l=0; l<m_Layers; ++l)
  {
    L=0;
    for (int i=0; i<m_Perimeter; ++i)
      if (m_pMasKP[l][i].KPType) // записываем в файл все типы КП
        ++L;
    fwrite(&L, sizeof(int), 1, f);
  }

  for (int l=0; l<m_Layers; ++l)
  {
    for (int i=0; i<m_Perimeter; ++i)
    {
      if (m_pMasKP[l][i].KPType)
      {
        fwrite(&i, sizeof(int), 1, f);
        fwrite(&m_pMasKP[l][i].KPType, sizeof(int), 1, f);
        fwrite(&m_pMasKP[l][i].KPNumber, sizeof(int), 1, f);
      }
    }
  }

}

// копирование структуры
void CRCStructure::CopyStructure(CRCStructure *S)
{
  for (int i=0; i<m_x; ++i)
  {
    memcpy(m_pMatrix[i], S->m_pMatrix[i], m_y*sizeof(int));
    memcpy(m_pMatrixMask[i], S->m_pMatrixMask[i], m_y*sizeof(int));
  }

  for (int l=0; l<m_Layers; ++l)
  {
    m_pKPQuantity[l] = S->m_pKPQuantity[l];
    for (int i=0; i<m_Perimeter; ++i)
    {
      m_pMasKP[l][i].KPType = S->m_pMasKP[l][i].KPType;
      m_pMasKP[l][i].KPNumber = S->m_pMasKP[l][i].KPNumber;
    }
  }

}

/*
void CRCStructure::Swap(CRCStructure *S)
{
  void *temp;

  temp = this->m_pKPQuantity;
  this->m_pKPQuantity = S->m_pKPQuantity;
  S->m_pKPQuantity = (int*)temp;

  temp = this->m_pMasKP;
  this->m_pMasKP = S->m_pMasKP;
  S->m_pMasKP = (TMasKP**)temp;

  temp = this->m_pMatrix;
  this->m_pMatrix = S->m_pMatrix;
  S->m_pMatrix = (int**)temp;
}
*/

// установить контактные площадки вертикально
void CRCStructure::KPV()
{
  int i;
  for (int l=0; l<m_Layers; ++l)
  {
    i=0;
    while (i<m_x)
    {
      m_pMasKP[l][i].KPType = 0;
      m_pMasKP[l][i].KPNumber = 0;
      ++i;
    }
    while (i<m_x+m_y)
    {
      m_pMasKP[l][i].KPType = 1;
      m_pMasKP[l][i].KPNumber = l*2+2;
      ++i;
    }
    while (i<m_Perimeter-m_y)
    {
      m_pMasKP[l][i].KPType = 0;
      m_pMasKP[l][i].KPNumber = 0;
      ++i;
    }
    while (i<m_Perimeter)
    {
      m_pMasKP[l][i].KPType = 1;
      m_pMasKP[l][i].KPNumber = l*2+1;
      ++i;
    }

    m_pKPQuantity[l] = 2;
  }
}

// установить контактные площадки горизонтально
void CRCStructure::KPH()
{
  int i;
  for (int l=0; l<m_Layers; ++l)
  {
    i=0;
    while (i<m_x)
    {
      m_pMasKP[l][i].KPType = 1;
      m_pMasKP[l][i].KPNumber = l*2+2;
      ++i;
    }
    while (i<m_x+m_y)
    {
      m_pMasKP[l][i].KPType = 0;
      m_pMasKP[l][i].KPNumber = 0;
      ++i;
    }
    while (i<m_Perimeter-m_y)
    {
      m_pMasKP[l][i].KPType = 1;
      m_pMasKP[l][i].KPNumber = l*2+1;
      ++i;
    }
    while (i<m_Perimeter)
    {
      m_pMasKP[l][i].KPType = 0;
      m_pMasKP[l][i].KPNumber = 0;
      ++i;
    }

    m_pKPQuantity[l] = 2;
  }
}

// редактирование структуры через графический редактор
bool CRCStructure::SetElementType(int Layer, int x, int y, EnumRCElementType ElementType)
{
  bool bChanged = true;

  TMasKP *thisKP;
  switch (ElementType)
  {
    // нумерация КП
    case RCET_NONE:
    {
      if (y>=0 && y<m_y)// если КП вертикальное
      {
        if (x<0)// с левой стороны
        {
          if (m_pMasKP[Layer][m_Perimeter-1-y].KPType==1 && !m_pMasKP[Layer][m_Perimeter-1-y].KPNumber) // нумеровать можно только КП с типом 1
            DoNumerate(Layer, m_Perimeter-1-y, GetKPQuantity());
        }
        else if (x>=m_x) // с правой стороны
        {
          if (m_pMasKP[Layer][m_x+y].KPType==1 && !m_pMasKP[Layer][m_x+y].KPNumber)
            DoNumerate(Layer, m_x+y, GetKPQuantity());
        }
      }
      // если КП горизонтальное
      if (x>=0 && x<m_x)
      {
        if (y<0)// сверху
        {
          if (m_pMasKP[Layer][x].KPType==1 && !m_pMasKP[Layer][x].KPNumber)
            DoNumerate(Layer, x, GetKPQuantity());
        }
        else if (y>=m_y)// снизу
        {
          if (m_pMasKP[Layer][m_Perimeter-1-m_y-x].KPType==1 && !m_pMasKP[Layer][m_Perimeter-1-m_y-x].KPNumber)
            DoNumerate(Layer, m_Perimeter-1-m_y-x, GetKPQuantity());
        }
      }
    }
    break;

    // если не выбран инструмент
    case RCET_EMPTY: case RCET_R: case RCET_RC:
    {
      if (x>=0 && x<m_x && y>=0 && y<m_y)
      {
        int temp = m_pMatrix[x][y];
        m_pMatrix[x][y] &= ~(3<<(Layer<<1));
        m_pMatrix[x][y] |= (ElementType<<(Layer<<1));
        m_pMatrix[x][y] = FixElement(m_pMatrix[x][y]);
        bChanged = (temp != m_pMatrix[x][y]);
      }
    }
    break;

    case RCET_KPNORMAL: // KPNORMAL нельзя ставить на пустую ячейку и рядом с шунтом
    {
      if (y>=0 && y<m_y)
      {
        if (x<0)
        {
          thisKP = &m_pMasKP[Layer][m_Perimeter-1-y];
          if (thisKP->KPType == KPNORMAL || m_pMatrix[0][y]==0)
            thisKP->KPType = KPNONE;
          else if (thisKP->KPType == KPNONE && thisKP->prev->KPType != KPSHUNT && thisKP->next->KPType != KPSHUNT)
            thisKP->KPType = KPNORMAL;
        }
        else if (x>=m_x)
        {
          thisKP = &m_pMasKP[Layer][m_x+y];
          if (thisKP->KPType == KPNORMAL || m_pMatrix[m_x-1][y]==0)
            thisKP->KPType = KPNONE;
          else if (thisKP->KPType == KPNONE && thisKP->prev->KPType != KPSHUNT && thisKP->next->KPType != KPSHUNT)
            thisKP->KPType = KPNORMAL;
        }
      }

      if (x>=0 && x<m_x)
      {
        if (y<0)
        {
          thisKP = &m_pMasKP[Layer][x];
          if (thisKP->KPType == KPNORMAL || m_pMatrix[x][0]==0)
            thisKP->KPType = KPNONE;
          else if (thisKP->KPType == KPNONE && thisKP->prev->KPType != KPSHUNT && thisKP->next->KPType != KPSHUNT)
            thisKP->KPType = KPNORMAL;
        }
        else if (y>=m_y)
        {
          thisKP = &m_pMasKP[Layer][m_Perimeter-1-m_y-x];
          if (thisKP->KPType == KPNORMAL || m_pMatrix[x][m_y-1]==0)
            thisKP->KPType = KPNONE;
          else if (thisKP->KPType == KPNONE && thisKP->prev->KPType != KPSHUNT && thisKP->next->KPType != KPSHUNT)
            thisKP->KPType = KPNORMAL;
        }
      }
    }
    break;

    case RCET_KPRESTRICT: // KPRESTRICT можно ставить в любое место
    {
      if (y>=0 && y<m_y)
      {
        if (x<0)
        {
          thisKP = &m_pMasKP[Layer][m_Perimeter-1-y];
          if (thisKP->KPType == KPRESTRICT)
            thisKP->KPType = KPNONE;
          else if (thisKP->KPType == KPNONE)
            thisKP->KPType = KPRESTRICT;
        }
        else if (x>=m_x)
        {
          thisKP = &m_pMasKP[Layer][m_x+y];
          if (thisKP->KPType == KPRESTRICT)
            thisKP->KPType = KPNONE;
          else if (thisKP->KPType == KPNONE)
            thisKP->KPType = KPRESTRICT;
        }
      }

      if (x>=0 && x<m_x)
      {
        if (y<0)
        {
          thisKP = &m_pMasKP[Layer][x];
          if (thisKP->KPType == KPRESTRICT)
            thisKP->KPType = KPNONE;
          else if (thisKP->KPType == KPNONE)
            thisKP->KPType = KPRESTRICT;
        }
        else if (y>=m_y)
        {
          thisKP = &m_pMasKP[Layer][m_Perimeter-1-m_y-x];
          if (thisKP->KPType == KPRESTRICT)
            thisKP->KPType = KPNONE;
          else if (thisKP->KPType == KPNONE)
            thisKP->KPType = KPRESTRICT;
        }
      }
    }
    break;

    case RCET_KPSHUNT: // KPSHUNT можно ставить в любое место, но нельзя рядом с KPNORMAL
    {
      if (y>=0 && y<m_y)
      {
        if (x<0)
        {
          thisKP = &m_pMasKP[Layer][m_Perimeter-1-y];
          if (thisKP->KPType == KPSHUNT)
            thisKP->KPType = KPNONE;
          else if (thisKP->KPType == KPNONE && thisKP->prev->KPType != KPNORMAL && thisKP->next->KPType != KPNORMAL)
            thisKP->KPType = KPSHUNT;
        }
        else if (x>=m_x)
        {
          thisKP = &m_pMasKP[Layer][m_x+y];
          if (thisKP->KPType == KPSHUNT)
            thisKP->KPType = KPNONE;
          else if (thisKP->KPType == KPNONE && thisKP->prev->KPType != KPNORMAL && thisKP->next->KPType != KPNORMAL)
            thisKP->KPType = KPSHUNT;
        }
      }

      if (x>=0 && x<m_x)
      {
        if (y<0)
        {
          thisKP = &m_pMasKP[Layer][x];
          if (thisKP->KPType == KPSHUNT)
            thisKP->KPType = KPNONE;
          else if (thisKP->KPType == KPNONE && thisKP->prev->KPType != KPNORMAL && thisKP->next->KPType != KPNORMAL)
            thisKP->KPType = KPSHUNT;
        }
        else if (y>=m_y)
        {
          thisKP = &m_pMasKP[Layer][m_Perimeter-1-m_y-x];
          if (thisKP->KPType == KPSHUNT)
            thisKP->KPType = KPNONE;
          else if (thisKP->KPType == KPNONE && thisKP->prev->KPType != KPNORMAL && thisKP->next->KPType != KPNORMAL)
            thisKP->KPType = KPSHUNT;
        }
      }
    }
    break;

    case RCET_MASKEMPTY: case RCET_MASKR: case RCET_MASKRC:
    {
      if (x>=0 && x<m_x && y>=0 && y<m_y)
      {
        int temp = m_pMatrixMask[x][y];
        m_pMatrixMask[x][y] &= ~(3<<(Layer<<1));
        m_pMatrixMask[x][y] |= ((ElementType-RCET_MASKEMPTY)<<(Layer<<1));
        bChanged = (temp != m_pMatrixMask[x][y]);
      }
    }
    break;
    
  }
  return bChanged;// возвращаемое значение не отражает действительности!
}

// сброс номеров контактных площадок
void CRCStructure::ClearKPNumbers()
{
  for (int l=0; l<m_Layers; ++l)
  {
    m_pKPQuantity[l] = 0;
    for (int i=0; i<m_Perimeter; ++i)
      m_pMasKP[l][i].KPNumber = 0;
  }
}

// нумерует КП на которую кликнули, а также КП слева и справа рекурсивно
void CRCStructure::DoNumerate(int Layer, int L, int KPNumber)
{
  ++KPNumber;
  ++m_pKPQuantity[Layer];
  m_pMasKP[Layer][L].KPNumber = KPNumber;
  TMasKP *begin = &m_pMasKP[Layer][L];

  TMasKP *temp = begin->prev;
  while (temp->KPType==1 && temp!=begin)// нумерация предшествующих кликнутой КП
  {
    temp->KPNumber = KPNumber;
    temp = temp->prev;
  }

  temp = begin->next;
  while (temp->KPType==1 && temp!=begin)// нумерация последующих кликнутой КП
  {
    temp->KPNumber = KPNumber;
    temp = temp->next;
  }
}

// подсчитывает длину контактных площадок и шунтов, это нужно для определения итогового числа узлов
int CRCStructure::GetKPSummaryLength()
{
  int Length=0;
  for (int l=0; l<m_Layers; ++l)
    for (int i=0; i<m_Perimeter; ++i)
      if (m_pMasKP[l][i].KPType==KPNORMAL || m_pMasKP[l][i].KPType==KPSHUNT)
        ++Length;
  return Length;
}

// изменение размеров КП
void CRCStructure::ResizeKP(int prob, int max_value, bool isfit)
{
  TMasKP *KP_begin, *KP_end, *KP_current;
  int value, v1, v2;

  for (int i=0; i<m_Layers; ++i)
  {
    KP_current = m_pMasKP[i];
    while (KP_current->KPType == KPNORMAL) // пока предполагаем, что во время синтеза шунты не будут перемещаться
      KP_current = KP_current->next;

    for (int kp=m_pKPQuantity[i]; kp>0; --kp)
    {
      while (KP_current->KPType != KPNORMAL)  // пока предполагаем, что во время синтеза шунты не будут перемещаться
        KP_current = KP_current->next;
      KP_begin = KP_current;

      while (KP_current->KPType == KPNORMAL)
        KP_current = KP_current->next;
      KP_end = KP_current->prev;

      if (prob > rand())
      {
        value = rand()%max_value;
        ++value;
        v1 = value >> 1;
        v2 = value & 1;

        // увеличение ширины КП с вероятностью 0.5,
        if (rand()&1)
        {
          KP_begin = KPBeginMove(KP_begin, -v1, isfit);
          KP_end   = KPEndMove(KP_end, v1, isfit);
          // если число не четное, то с вероятностью 0.5 увеличиваем либо слева либо справа
          if (rand()&1)
            KPBeginMove(KP_begin, -v2, isfit);
          else
            KP_end = KPEndMove(KP_end, v2, isfit);
        }
        else // иначе уменьшение
        {
          KP_begin = KPBeginMove(KP_begin, v1, isfit);
          KP_end   = KPEndMove(KP_end, -v1, isfit);
          if (rand()&1)
            KPBeginMove(KP_begin, v2, isfit);
          else
            KP_end = KPEndMove(KP_end, -v2, isfit);
        }
      }

      KP_current = KP_end->next;
    }
  }

}

// перемещение КП
void CRCStructure::MoveKP(int prob, int max_value, bool isfit)
{
  TMasKP *KP_begin, *KP_end, *KP_current, *temp2;
  int v1, v2;

  for (int i=0; i<m_Layers; ++i)
  {
    KP_current = m_pMasKP[i];
    while (KP_current->KPType==1)
      KP_current = KP_current->next;

    for (int kp=m_pKPQuantity[i]; kp>0; --kp)
    {
      while (KP_current->KPType!=1)
        KP_current = KP_current->next;
      KP_begin = KP_current;

      while (KP_current->KPType==1)
        KP_current = KP_current->next;
      KP_end = KP_current->prev;

      if (prob > rand())
      {
        v1 = rand()%max_value;
        ++v1;
        v2 = 0;

        // с вероятностью 0.5 сдвигаем вправо
        if (rand()&1)
        {
          temp2 = KP_end;
          while (temp2->next->KPType!=2 && temp2->next->next->KPType!=1 && temp2->next->next->KPType!=3 && v2<v1) // сдвигать можно или до рестрикции, или до КП с отступом в 1, или до шунта с отступом в 1
          {
            ++v2;
            temp2 = temp2->next;
          }
          KP_end = KPEndMove(KP_end, v2, isfit);
          KPBeginMove(KP_begin, v2, isfit);
        }
        else // или влево
        {
          temp2 = KP_begin;
          while (temp2->prev->KPType!=2 && temp2->prev->prev->KPType!=1 && temp2->prev->prev->KPType!=3 && v2<v1)
          {
            ++v2;
            temp2 = temp2->prev;
          }
          KPBeginMove(KP_begin, -v2, isfit);
          KPEndMove(KP_end, -v2, isfit);
        }
      }

      KP_current = KP_end->next;
    }
  }
}

// внутренняя функция, нужна для изменения размера и положения КП
TMasKP* CRCStructure::KPBeginMove(TMasKP *KP_begin, int value, bool isfit)
{
  if (value>=0)
  {
    // TODO: заменять отрезанную часть КП на шунт при режиме подгонки
    for (int i=0; i<value; ++i)
    {
      if (KP_begin->next->KPType == KPNORMAL || KP_begin->next->KPType == KPSHUNT)
      {
        KP_begin->KPType = KPNONE;
        KP_begin->KPNumber = 0;
        KP_begin = KP_begin->next;
      }
      else
      {
        break;
      }
    }
  }
  else
  {
    if (!isfit) // если режим подгонки, то нельзя расширять КП
    {
      for (int i=0; i>value; --i)
      {
        if (KP_begin->prev->KPType != KPRESTRICT && KP_begin->prev->prev->KPType != KPNORMAL && KP_begin->prev->prev->KPType != KPSHUNT)
        {
          KP_begin->prev->KPType = KP_begin->KPType;
          KP_begin->prev->KPNumber = KP_begin->KPNumber;
          KP_begin = KP_begin->prev;
        }
        else
        {
          break;
        }
      }
    }
  }
  return KP_begin;
}

// внутренняя функция, нужна для изменения размера и положения КП
TMasKP* CRCStructure::KPEndMove(TMasKP *KP_end, int value, bool isfit)
{
  if (value>=0)
  {
    if (!isfit) // если режим подгонки, то нельзя расширять КП
    {
      for (int i=0; i<value; ++i)
      {
        if (KP_end->next->KPType != KPRESTRICT && KP_end->next->next->KPType != KPNORMAL && KP_end->next->next->KPType != KPSHUNT)
        {
          KP_end->next->KPType = KP_end->KPType;
          KP_end->next->KPNumber = KP_end->KPNumber;
          KP_end = KP_end->next;
        }
        else
        {
          break;
        }
      }
    }
  }
  else
  {
    // TODO: заменять отрезанную часть КП на шунт при режиме подгонки
    for (int i=0; i>value; --i)
    {
      if (KP_end->prev->KPType == KPNORMAL || KP_end->prev->KPType == KPSHUNT)
      {
        KP_end->KPType = KPNONE;
        KP_end->KPNumber = 0;
        KP_end = KP_end->prev;
      }
      else
      {
        break;
      }
    }
  }
  return KP_end;
}

// оператор кроссинговера
bool CRCStructure::Cross(CRCStructure *S, int prob, int MaxWidth, int MaxHeight, bool ExchangeLayers)
{
  // TODO: доделать обмен разными слоями
  if (prob > rand())
  {
    int temp;
    int width = rand()%MaxWidth;
    int height = rand()%MaxHeight;
    int x = rand()%(m_x-width);
    int y = rand()%(m_y-height);
    for (int i=x; i<=x+width; ++i)
    {
      for (int j=y; j<=y+height; ++j)
      {
        if (ExchangeLayers)
        {
        }
        else
        {
          if (!(m_pMatrixMask[i][j] | S->m_pMatrixMask[i][j]))
          {
            temp = m_pMatrix[i][j];
            m_pMatrix[i][j] = S->m_pMatrix[i][j];
            S->m_pMatrix[i][j] = temp;
          }
        }
      }
    }
    return false; // если выполнили скрещивание одинаковых слоев, то скрещивание разных слоев выполнять не нужно
  }
  return true; // если скрещивание одинаковых слоев не выполнилось, то по выходу из функции выполняем скрещивание разных слоев
}

// вычисление значения  invert = 1.0/complex_value;
complex<double> invert(const complex<double>& __z)
{
  double denom = __z.real() * __z.real() + __z.imag() * __z.imag();//__z._M_re*__z._M_re + __z._M_im*__z._M_im;
  return complex<double>(__z.real() / denom, -__z.imag() / denom);//(__z._M_re/denom, -__z._M_im/denom);
}

// Понижение порядка матрицы до количества КП
void CRCStructure::Poradok(complex<double> **Ym)
{
  complex<double> Y1, Y2;
  complex<double> *YL, *src, *dst;

  int kp = GetKPQuantity();
  for (int i=m_Nodes-1; i>=kp; --i)
  {
    if (Ym[i][0] != 0.0)
    {
      Y1 = invert(Ym[i][0]); // 1/D

      for (int l=0; l<i; ++l)
      {
        int row = i-l; // цикл по рядам
        YL = Ym[l];
        if (row < m_pRowSizes[l] && YL[row] != 0.0)
        {
          Y2 = YL[row]*Y1; // C/D
          dst = YL;
          for (int m=l; m<i; ++m, ++dst)
          {
            int column = i-m; // цикл по столбцам
            //if (column < m_pRowSizes[m] && *(src=&Ym[m][column]) != 0.0)
            if (column < m_pRowSizes[m] && *(src=Ym[m]+column) != 0.0)
            {

                   //Ym[l][m-l] -= Ym[m][column]*Y2;
                // ассемблерная вставка, которая вычисляет A -= B*C/D
                // dst = A
                // src = B
                // Y2 = C/D

                __asm
                {
                  mov esi, src
                  lea ecx, Y2
                  mov edi, dst

                  fld qword ptr [esi]
                  fld qword ptr [esi]
                  fld qword ptr [ecx]
                  fmul ST(2),ST
                  fld qword ptr [ecx+8]
                  fmul ST(2),ST
                  fld qword ptr [esi+8]
                  fmul ST(2),ST
                  fmul
                  fsubp ST(3),ST
                  fadd
                  fxch
                  fld qword ptr [edi]
                  fsubrp ST(1),ST
                  fstp qword ptr [edi]
                  fld qword ptr [edi+8]
                  fsubrp ST(1),ST
                  fstp qword ptr [edi+8]

                }

            }
          }
        }
      }
    }
  }
}

// проверяем пронумерованы ли контактные площадки
bool CRCStructure::CheckKP()
{
  for (int l=0; l<m_Layers; ++l)
    for (int i=0; i<m_Perimeter; ++i)
      if (m_pMasKP[l][i].KPType == KPNORMAL && m_pMasKP[l][i].KPNumber==0)
        return false;
  return true;
}

// внутренняя функция, возвращает значение, в зависимости есть ли материал в заданном слое
inline int HaveLayer(int Element, int Layer)
{
  return Element & (1 << (Layer << 1));
}

// преобразует конечные элементы в матрицу узлов (теперь граф узлов)
void CRCStructure::ElementsToNodes()
{
  for (int l=0; l<m_Layers; ++l)
    for (int i=0; i<=m_x; ++i)
      memset(m_pCircuitNodes[l][i], 0xFF, (m_y+1)*sizeof(int)); // заполняем память значением FF, все значения в массиве будут равны -1

  unsigned int KolUz = (m_x+m_y) << 3; // максимальный размер фронта волны равен четырем периметрам (предполагаем, что этого количества хватит)
  T3DPoint *pNewFront = new T3DPoint[KolUz];
  T3DPoint *pOldFront = new T3DPoint[KolUz];
  T3DPoint *pTempFront;
  int u = GetKPQuantity(); // номер следующего узла
  int NewCount = 0;
  int OldCount;

  for (int l=0; l<m_Layers; ++l)
  {
    bool bCicled = false;

    // находим место свободное от КП, это будет началом просмотра всех КП
    TMasKP *KPBegin = m_pMasKP[l];
    TMasKP *temp = m_pMasKP[l];
    do
    {
      temp = temp->next;
    }
    while (temp != KPBegin && temp->KPType != KPNONE);

    if (temp == KPBegin && temp != KPNONE)
      bCicled = true; // попалась зацикленная КП или Шунт

    KPBegin = temp; // нашли начало просмотра КП

    if (bCicled)
    {
      do
      {
        temp = temp->next;
        if (temp->KPType == KPNORMAL)
        {
          m_pCircuitNodes[l][temp->ux][temp->uy] = temp->KPNumber-1; // присвоили узлу номер
          pNewFront[NewCount++] = T3DPoint(l, temp->ux, temp->uy); // добавили во фронт волны
        }
        else if (temp->KPType == KPSHUNT)
        {
          m_pCircuitNodes[l][temp->ux][temp->uy] = NODESHUNT; // пометили, что узел принадлежит шунту
        }
      }
      while (temp != KPBegin);
    }
    else
    {
      do
      {
        temp = temp->next;

        while (temp->KPType == KPNORMAL)
        {
          m_pCircuitNodes[l][temp->ux][temp->uy] = temp->KPNumber-1; // присвоили узлу номер
          pNewFront[NewCount++] = T3DPoint(l, temp->ux, temp->uy); // добавили во фронт волны
          temp = temp->next;
          if (temp->KPType != KPNORMAL) // если конец КП, нумеруем узел который принадлежит концу КП
          {
            m_pCircuitNodes[l][temp->ux][temp->uy] = temp->prev->KPNumber-1;
            pNewFront[NewCount++] = T3DPoint(l, temp->ux, temp->uy);
          }
        }

        while (temp->KPType == KPSHUNT)
        {
          m_pCircuitNodes[l][temp->ux][temp->uy] = NODESHUNT; // присвоили узлу номер
          temp = temp->next;
          if (temp->KPType != KPSHUNT) // если конец шунта, нумеруем узел который принадлежит концу шунта
          {
            m_pCircuitNodes[l][temp->ux][temp->uy] = NODESHUNT;
          }
        }
      }
      while (temp != KPBegin);
    }
  }

  memset(m_pRowSizes, 0, (m_x+1)*(m_y+1)*m_Layers*sizeof(int));

  // выше по коду в pNewFront занесли фронт начала распространения волны
  // ниже по коду происходит распространение "волны" нумерации от всех контактных площадок одновременно
  do
  {
    pTempFront = pOldFront;
    pOldFront = pNewFront;
    pNewFront = pTempFront;
    OldCount = NewCount;
    NewCount = 0;

    for (int e=0; e<OldCount; ++e)
    //while(OldCount--)
    {
      int curZ = pOldFront[e].z;
      int curX = pOldFront[e].x;
      int curY = pOldFront[e].y;
      //int curZ = pOldFront[OldCount].z;
      //int curX = pOldFront[OldCount].x;
      //int curY = pOldFront[OldCount].y;
      int newZ, newX, newY;
      int tempX, tempY;
      int *pNewNode;
      int u0 = m_pCircuitNodes[curZ][curX][curY];
      int utemp = u0;

      // над
      if (curZ > 0)
      {
        newZ=curZ-1;
        pNewNode = &m_pCircuitNodes[newZ][curX][curY];
        if (*pNewNode==NODENULL)
        {
          tempX = curX-1;
          tempY = curY-1;
          if ((curX<m_x && curY<m_y && HaveLayer(m_pMatrix[curX][curY],newZ)) || (curX<m_x && tempY>=0 && HaveLayer(m_pMatrix[curX][tempY],newZ)) || (tempX>=0 && curY<m_y && HaveLayer(m_pMatrix[tempX][curY],newZ)) || (tempX>=0 && tempY>=0 && HaveLayer(m_pMatrix[tempX][tempY],newZ)))
          {
            pNewFront[NewCount++] = T3DPoint(newZ, curX, curY);
            *pNewNode = u++;
          }
        }
        else if (*pNewNode==NODESHUNT)
        {
          tempX = curX-1;
          tempY = curY-1;
          if ((curX<m_x && curY<m_y && HaveLayer(m_pMatrix[curX][curY],newZ)) || (curX<m_x && tempY>=0 && HaveLayer(m_pMatrix[curX][tempY],newZ)) || (tempX>=0 && curY<m_y && HaveLayer(m_pMatrix[tempX][curY],newZ)) || (tempX>=0 && tempY>=0 && HaveLayer(m_pMatrix[tempX][tempY],newZ)))
          {
            pNewFront[NewCount++] = T3DPoint(newZ, curX, curY);
            NewCount = RecursiveNumering(pNewFront, NewCount, u++);
          }
        }
        if (*pNewNode > utemp) utemp = *pNewNode;

        // над-слева
        if (curX > 0)
        {
          newX=curX-1;
          pNewNode = &m_pCircuitNodes[newZ][newX][curY];
          if (*pNewNode==NODENULL)
          {
            tempY = curY-1;
            if ((curY<m_y && HaveLayer(m_pMatrix[newX][curY],newZ)) || (tempY>=0 && HaveLayer(m_pMatrix[newX][tempY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, newX, curY);
              *pNewNode = u++;
            }
          }
          else if (*pNewNode==NODESHUNT)
          {
            tempY = curY-1;
            if ((curY<m_y && HaveLayer(m_pMatrix[newX][curY],newZ)) || (tempY>=0 && HaveLayer(m_pMatrix[newX][tempY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, newX, curY);
              NewCount = RecursiveNumering(pNewFront, NewCount, u++);
            }
          }
          if (*pNewNode > utemp) utemp = *pNewNode;
        }

        // над-справа
        if (curX < m_x)
        {
          newX=curX+1;
          pNewNode = &m_pCircuitNodes[newZ][newX][curY];
          if (*pNewNode==NODENULL)
          {
            tempY = curY-1;
            if ((curY<m_y && HaveLayer(m_pMatrix[curX][curY],newZ)) || (tempY>=0 && HaveLayer(m_pMatrix[curX][tempY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, newX, curY);
              *pNewNode = u++;
            }
          }
          else if (*pNewNode==NODESHUNT)
          {
            tempY = curY-1;
            if ((curY<m_y && HaveLayer(m_pMatrix[curX][curY],newZ)) || (tempY>=0 && HaveLayer(m_pMatrix[curX][tempY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, newX, curY);
              NewCount = RecursiveNumering(pNewFront, NewCount, u++);
            }
          }
          if (*pNewNode > utemp) utemp = *pNewNode;
        }

        // над-сверху
        if (curY > 0)
        {
          newY=curY-1;
          pNewNode = &m_pCircuitNodes[newZ][curX][newY];
          if (*pNewNode==NODENULL)
          {
            tempX = curX-1;
            if ((curX<m_x && HaveLayer(m_pMatrix[curX][newY],newZ)) || (tempX>=0 && HaveLayer(m_pMatrix[tempX][newY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, curX, newY);
              *pNewNode = u++;
            }
          }
          else if (*pNewNode==NODESHUNT)
          {
            tempX = curX-1;
            if ((curX<m_x && HaveLayer(m_pMatrix[curX][newY],newZ)) || (tempX>=0 && HaveLayer(m_pMatrix[tempX][newY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, curX, newY);
              NewCount = RecursiveNumering(pNewFront, NewCount, u++);
            }
          }
          if (*pNewNode > utemp) utemp = *pNewNode;
        }

        // над-снизу
        if (curY < m_y)
        {
          newY=curY+1;
          pNewNode = &m_pCircuitNodes[newZ][curX][newY];
          if (*pNewNode==NODENULL)
          {
            tempX = curX-1;
            if ((curX<m_x && HaveLayer(m_pMatrix[curX][curY],newZ)) || (tempX>=0 && HaveLayer(m_pMatrix[tempX][curY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, curX, newY);
              *pNewNode = u++;
            }
          }
          else if (*pNewNode==NODESHUNT)
          {
            tempX = curX-1;
            if ((curX<m_x && HaveLayer(m_pMatrix[curX][curY],newZ)) || (tempX>=0 && HaveLayer(m_pMatrix[tempX][curY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, curX, newY);
              NewCount = RecursiveNumering(pNewFront, NewCount, u++);
            }
          }
          if (*pNewNode > utemp) utemp = *pNewNode;
        }

      }


      // под
      if (curZ+1 < m_Layers)
      {
        newZ=curZ+1;
        pNewNode = &m_pCircuitNodes[newZ][curX][curY];
        if (*pNewNode==NODENULL)
        {
          tempX = curX-1;
          tempY = curY-1;
          if ((curX<m_x && curY<m_y && HaveLayer(m_pMatrix[curX][curY],newZ)) || (curX<m_x && tempY>=0 && HaveLayer(m_pMatrix[curX][tempY],newZ)) || (tempX>=0 && curY<m_y && HaveLayer(m_pMatrix[tempX][curY],newZ)) || (tempX>=0 && tempY>=0 && HaveLayer(m_pMatrix[tempX][tempY],newZ)))
          {
            pNewFront[NewCount++] = T3DPoint(newZ, curX, curY);
            *pNewNode = u++;
          }
        }
        else if (*pNewNode==NODESHUNT)
        {
          tempX = curX-1;
          tempY = curY-1;
          if ((curX<m_x && curY<m_y && HaveLayer(m_pMatrix[curX][curY],newZ)) || (curX<m_x && tempY>=0 && HaveLayer(m_pMatrix[curX][tempY],newZ)) || (tempX>=0 && curY<m_y && HaveLayer(m_pMatrix[tempX][curY],newZ)) || (tempX>=0 && tempY>=0 && HaveLayer(m_pMatrix[tempX][tempY],newZ)))
          {
            pNewFront[NewCount++] = T3DPoint(newZ, curX, curY);
            NewCount = RecursiveNumering(pNewFront, NewCount, u++);
          }
        }
        if (*pNewNode > utemp) utemp = *pNewNode;

        // под-слева
        if (curX > 0)
        {
          newX=curX-1;
          pNewNode = &m_pCircuitNodes[newZ][newX][curY];
          if (*pNewNode==NODENULL)
          {
            tempY = curY-1;
            if ((curY<m_y && HaveLayer(m_pMatrix[newX][curY],newZ)) || (tempY>=0 && HaveLayer(m_pMatrix[newX][tempY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, newX, curY);
              *pNewNode = u++;
            }
          }
          else if (*pNewNode==NODESHUNT)
          {
            tempY = curY-1;
            if ((curY<m_y && HaveLayer(m_pMatrix[newX][curY],newZ)) || (tempY>=0 && HaveLayer(m_pMatrix[newX][tempY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, newX, curY);
              NewCount = RecursiveNumering(pNewFront, NewCount, u++);
            }
          }
          if (*pNewNode > utemp) utemp = *pNewNode;
        }

        // под-справа
        if (curX < m_x)
        {
          newX=curX+1;
          pNewNode = &m_pCircuitNodes[newZ][newX][curY];
          if (*pNewNode==NODENULL)
          {
            tempY = curY-1;
            if ((curY<m_y && HaveLayer(m_pMatrix[curX][curY],newZ)) || (tempY>=0 && HaveLayer(m_pMatrix[curX][tempY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, newX, curY);
              *pNewNode = u++;
            }
          }
          else if (*pNewNode==NODESHUNT)
          {
            tempY = curY-1;
            if ((curY<m_y && HaveLayer(m_pMatrix[curX][curY],newZ)) || (tempY>=0 && HaveLayer(m_pMatrix[curX][tempY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, newX, curY);
              NewCount = RecursiveNumering(pNewFront, NewCount, u++);
            }
          }
          if (*pNewNode > utemp) utemp = *pNewNode;
        }

        // под-сверху
        if (curY > 0)
        {
          newY=curY-1;
          pNewNode = &m_pCircuitNodes[newZ][curX][newY];
          if (*pNewNode==NODENULL)
          {
            tempX = curX-1;
            if ((curX<m_x && HaveLayer(m_pMatrix[curX][newY],newZ)) || (tempX>=0 && HaveLayer(m_pMatrix[tempX][newY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, curX, newY);
              *pNewNode = u++;
            }
          }
          else if (*pNewNode==NODESHUNT)
          {
            tempX = curX-1;
            if ((curX<m_x && HaveLayer(m_pMatrix[curX][newY],newZ)) || (tempX>=0 && HaveLayer(m_pMatrix[tempX][newY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, curX, newY);
              NewCount = RecursiveNumering(pNewFront, NewCount, u++);
            }
          }
          if (*pNewNode > utemp) utemp = *pNewNode;
        }

        // под-снизу
        if (curY < m_y)
        {
          newY=curY+1;
          pNewNode = &m_pCircuitNodes[newZ][curX][newY];
          if (*pNewNode==NODENULL)
          {
            tempX = curX-1;
            if ((curX<m_x && HaveLayer(m_pMatrix[curX][curY],newZ)) || (tempX>=0 && HaveLayer(m_pMatrix[tempX][curY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, curX, newY);
              *pNewNode = u++;
            }
          }
          else if (*pNewNode==NODESHUNT)
          {
            tempX = curX-1;
            if ((curX<m_x && HaveLayer(m_pMatrix[curX][curY],newZ)) || (tempX>=0 && HaveLayer(m_pMatrix[tempX][curY],newZ)))
            {
              pNewFront[NewCount++] = T3DPoint(newZ, curX, newY);
              NewCount = RecursiveNumering(pNewFront, NewCount, u++);
            }
          }
          if (*pNewNode > utemp) utemp = *pNewNode;
        }

      }


      // слева
      if (curX > 0)
      {
        newX=curX-1;
        pNewNode = &m_pCircuitNodes[curZ][newX][curY];
        if (*pNewNode==NODENULL)
        {
          tempY = curY-1;
          if ((curY<m_y && HaveLayer(m_pMatrix[newX][curY],curZ)) || (tempY>=0 && HaveLayer(m_pMatrix[newX][tempY],curZ)))
          {
            pNewFront[NewCount++] = T3DPoint(curZ, newX, curY);
            *pNewNode = u++;
          }
        }
        else if (*pNewNode==NODESHUNT)
        {
          tempY = curY-1;
          if ((curY<m_y && HaveLayer(m_pMatrix[newX][curY],curZ)) || (tempY>=0 && HaveLayer(m_pMatrix[newX][tempY],curZ)))
          {
            pNewFront[NewCount++] = T3DPoint(curZ, newX, curY);
            NewCount = RecursiveNumering(pNewFront, NewCount, u++);
          }
        }
        if (*pNewNode > utemp) utemp = *pNewNode;
      }

      // справа
      if (curX < m_x)
      {
        newX=curX+1;
        pNewNode = &m_pCircuitNodes[curZ][newX][curY];
        if (*pNewNode==NODENULL)
        {
          tempY = curY-1;
          if ((curY<m_y && HaveLayer(m_pMatrix[curX][curY],curZ)) || (tempY>=0 && HaveLayer(m_pMatrix[curX][tempY],curZ)))
          {
            pNewFront[NewCount++] = T3DPoint(curZ, newX, curY);
            *pNewNode = u++;
          }
        }
        else if (*pNewNode==NODESHUNT)
        {
          tempY = curY-1;
          if ((curY<m_y && HaveLayer(m_pMatrix[curX][curY],curZ)) || (tempY>=0 && HaveLayer(m_pMatrix[curX][tempY],curZ)))
          {
            pNewFront[NewCount++] = T3DPoint(curZ, newX, curY);
            NewCount = RecursiveNumering(pNewFront, NewCount, u++);
          }
        }
        if (*pNewNode > utemp) utemp = *pNewNode;
      }

      // сверху
      if (curY > 0)
      {
        newY=curY-1;
        pNewNode = &m_pCircuitNodes[curZ][curX][newY];
        if (*pNewNode==NODENULL)
        {
          tempX = curX-1;
          if ((curX<m_x && HaveLayer(m_pMatrix[curX][newY],curZ)) || (tempX>=0 && HaveLayer(m_pMatrix[tempX][newY],curZ)))
          {
            pNewFront[NewCount++] = T3DPoint(curZ, curX, newY);
            *pNewNode = u++;
          }
        }
        else if (*pNewNode==NODESHUNT)
        {
          tempX = curX-1;
          if ((curX<m_x && HaveLayer(m_pMatrix[curX][newY],curZ)) || (tempX>=0 && HaveLayer(m_pMatrix[tempX][newY],curZ)))
          {
            pNewFront[NewCount++] = T3DPoint(curZ, curX, newY);
            NewCount = RecursiveNumering(pNewFront, NewCount, u++);
          }
        }
        if (*pNewNode > utemp) utemp = *pNewNode;
      }

      // снизу
      if (curY < m_y)
      {
        newY=curY+1;
        pNewNode = &m_pCircuitNodes[curZ][curX][newY];
        if (*pNewNode==NODENULL)
        {
          tempX = curX-1;
          if ((curX<m_x && HaveLayer(m_pMatrix[curX][curY],curZ)) || (tempX>=0 && HaveLayer(m_pMatrix[tempX][curY],curZ)))
          {
            pNewFront[NewCount++] = T3DPoint(curZ, curX, newY);
            *pNewNode = u++;
          }
        }
        else if (*pNewNode==NODESHUNT)
        {
          tempX = curX-1;
          if ((curX<m_x && HaveLayer(m_pMatrix[curX][curY],curZ)) || (tempX>=0 && HaveLayer(m_pMatrix[tempX][curY],curZ)))
          {
            pNewFront[NewCount++] = T3DPoint(curZ, curX, newY);
            NewCount = RecursiveNumering(pNewFront, NewCount, u++);
          }
        }
        if (*pNewNode > utemp) utemp = *pNewNode;
      }

      if (m_pRowSizes[u0] < utemp - u0 + 1)
        m_pRowSizes[u0] = utemp - u0 + 1;
    }

  }
  while (NewCount);

  delete[] pOldFront;
  delete[] pNewFront;

  m_Nodes = u;
}

// нумерация узлов, объединенных шунтом
int CRCStructure::RecursiveNumering(T3DPoint *pBegin, int BeginNum, int u)
{
  unsigned int KolUz = (m_x+m_y) << 2; // длина шунта не может быть больше периметра, поэтому двух периметров должно хватить
  T3DPoint *pOldFront = new T3DPoint[KolUz];
  T3DPoint *pNewFront = new T3DPoint[KolUz];
  T3DPoint *pTempFront;

  int NewCount = 1;
  int OldCount;

  pNewFront[0] = pBegin[BeginNum-1];

  do
  {
    pTempFront = pOldFront;
    pOldFront = pNewFront;
    pNewFront = pTempFront;
    OldCount = NewCount;
    NewCount = 0;

    while (OldCount--)
    {
      int curZ = pOldFront[OldCount].z;
      int curX = pOldFront[OldCount].x;
      int curY = pOldFront[OldCount].y;
      int newZ, newX, newY;
      int tempX, tempY;
      int *pNewNode;

      // над
      if (curZ > 0)
      {
        newZ=curZ-1;
        pNewNode = &m_pCircuitNodes[newZ][curX][curY];
        if (*pNewNode==NODESHUNT)
        {
          pBegin[BeginNum++] = pNewFront[NewCount++] = T3DPoint(newZ, curX, curY);
          *pNewNode = u;
        }

        // над-слева
        if (curX > 0)
        {
          newX=curX-1;
          pNewNode = &m_pCircuitNodes[newZ][newX][curY];
          if (*pNewNode==NODESHUNT)
          {
            pBegin[BeginNum++] = pNewFront[NewCount++] = T3DPoint(newZ, newX, curY);
            *pNewNode = u;
          }
        }
  
        // над-справа
        if (curX < m_x)
        {
          newX=curX+1;
          pNewNode = &m_pCircuitNodes[newZ][newX][curY];
          if (*pNewNode==NODESHUNT)
          {
            pBegin[BeginNum++] = pNewFront[NewCount++] = T3DPoint(newZ, newX, curY);
            *pNewNode = u;
          }
        }
  
        // над-сверху
        if (curY > 0)
        {
          newY=curY-1;
          pNewNode = &m_pCircuitNodes[newZ][curX][newY];
          if (*pNewNode==NODESHUNT)
          {
            pBegin[BeginNum++] = pNewFront[NewCount++] = T3DPoint(newZ, curX, newY);
            *pNewNode = u;
          }
        }
  
        // над-снизу
        if (curY < m_y)
        {
          newY=curY+1;
          pNewNode = &m_pCircuitNodes[newZ][curX][newY];
          if (*pNewNode==NODESHUNT)
          {
            pBegin[BeginNum++] = pNewFront[NewCount++] = T3DPoint(newZ, curX, newY);
            *pNewNode = u;
          }
        }

      }


      // под
      if (curZ+1 < m_Layers)
      {
        newZ=curZ+1;
        pNewNode = &m_pCircuitNodes[newZ][curX][curY];
        if (*pNewNode==NODESHUNT)
        {
          pBegin[BeginNum++] = pNewFront[NewCount++] = T3DPoint(newZ, curX, curY);
          *pNewNode = u;
        }

        // под-слева
        if (curX > 0)
        {
          newX=curX-1;
          pNewNode = &m_pCircuitNodes[newZ][newX][curY];
          if (*pNewNode==NODESHUNT)
          {
            pBegin[BeginNum++] = pNewFront[NewCount++] = T3DPoint(newZ, newX, curY);
            *pNewNode = u;
          }
        }

        // под-справа
        if (curX < m_x)
        {
          newX=curX+1;
          pNewNode = &m_pCircuitNodes[newZ][newX][curY];
          if (*pNewNode==NODESHUNT)
          {
            pBegin[BeginNum++] = pNewFront[NewCount++] = T3DPoint(newZ, newX, curY);
            *pNewNode = u;
          }
        }
  
        // под-сверху
        if (curY > 0)
        {
          newY=curY-1;
          pNewNode = &m_pCircuitNodes[newZ][curX][newY];
          if (*pNewNode==NODESHUNT)
          {
            pBegin[BeginNum++] = pNewFront[NewCount++] = T3DPoint(newZ, curX, newY);
            *pNewNode = u;
          }
        }
  
        // под-снизу
        if (curY < m_y)
        {
          newY=curY+1;
          pNewNode = &m_pCircuitNodes[newZ][curX][newY];
          if (*pNewNode==NODESHUNT)
          {
            pBegin[BeginNum++] = pNewFront[NewCount++] = T3DPoint(newZ, curX, newY);
            *pNewNode = u;
          }
        }

      }


      // слева
      if (curX > 0)
      {
        newX=curX-1;
        pNewNode = &m_pCircuitNodes[curZ][newX][curY];
        if (*pNewNode==NODESHUNT)
        {
          pBegin[BeginNum++] = pNewFront[NewCount++] = T3DPoint(curZ, newX, curY);
          *pNewNode = u;
        }
      }

      // справа
      if (curX < m_x)
      {
        newX=curX+1;
        pNewNode = &m_pCircuitNodes[curZ][newX][curY];
        if (*pNewNode==NODESHUNT)
        {
          pBegin[BeginNum++] = pNewFront[NewCount++] = T3DPoint(curZ, newX, curY);
          *pNewNode = u;
        }
      }

      // сверху
      if (curY > 0)
      {
        newY=curY-1;
        pNewNode = &m_pCircuitNodes[curZ][curX][newY];
        if (*pNewNode==NODESHUNT)
        {
          pBegin[BeginNum++] = pNewFront[NewCount++] = T3DPoint(curZ, curX, newY);
          *pNewNode = u;
        }
      }

      // снизу
      if (curY < m_y)
      {
        newY=curY+1;
        pNewNode = &m_pCircuitNodes[curZ][curX][newY];
        if (*pNewNode==NODESHUNT)
        {
          pBegin[BeginNum++] = pNewFront[NewCount++] = T3DPoint(curZ, curX, newY);
          *pNewNode = u;
        }
      }


    }

  }
  while (NewCount);

  delete[] pOldFront;
  delete[] pNewFront;

  return BeginNum;
}

// прибавить значение проводимости узла к глобальной матрице проводимости
void CRCStructure::AddToMatrix(complex<double> **Ym, int u0, int u1, const complex<double>& value, bool f)
{
  if (u0>=0 && u1>=0 && value != 0.0)
  {
    if (u1 > u0)
    {
      Ym[u0][u1-u0] += value;
    }
    else
    {
      Ym[u1][u0-u1] += value;
      if (f && u1==u0)  // когда номера узлов u1 и u0 одинаковы, но эти узлы не относятся к собственному узлу,
      {
        Ym[u1][0] += value; // то проводимость между этими узлами надо прибавлять дважды
        //Ym[u1][0] += complex<double>(-1e10, -1e10);
      }
    }
  }
}

// вычислить Y-параметры для частоты w // Dest -> m_MatY
int CRCStructure::YParameters(double w, complex<double> *Dest) 
{
  //LARGE_INTEGER freq, count0, count1, count2, count3, count4, count5;
  //QueryPerformanceCounter(&count0);

  //QueryPerformanceCounter(&count1);

/*
  int summ = 0;
  for (int i=0; i<m_Nodes; ++i)
    summ += m_pRowSizes[i];
*/

  char **Ym_notaligned = new char*[m_Nodes];
  complex<double> **Ym = new complex<double>*[m_Nodes]; // выравненный массив
  const size_t AlignSize = 16UL;
  for (int i=0; i<m_Nodes; ++i)
  {
    Ym_notaligned[i] = (char*)calloc(m_pRowSizes[i] + AlignSize/sizeof(complex<double>), sizeof(complex<double>)); // добавим 16 байт, выравнивать нужно именно на 16 байт, а не на sizeof(complex<double>)
    Ym[i] = (complex<double>*)((size_t)Ym_notaligned[i] + AlignSize - (size_t)Ym_notaligned[i]%AlignSize); // выравниваем на 16 байт
    //Ym[i] = (complex<double>*)calloc(m_pRowSizes[i], sizeof(complex<double>));
  }
  //QueryPerformanceCounter(&count2);

  if (w < 1e-37)
    w = 1e-37;
  FillGlobalMatrix(Ym, w*2.0*M_PI); //w*2.0*M_PI

  //QueryPerformanceCounter(&count3);
  Poradok(Ym);
  //QueryPerformanceCounter(&count4);

  int ExitCode = 0;
  int k=0;
  int kkk = GetKPQuantity();

  for (int kp1=0; kp1<kkk; ++kp1)
  {
    if (fabs(Ym[kp1][0].real()) < 1e-37)
      ExitCode = -1;
    for (int kp2=0; kp2<kkk-kp1; ++kp2)
      Dest[k++]=Ym[kp1][kp2];
  }

  for (int i=0; i<m_Nodes; ++i)
    free(Ym_notaligned[i]);
  //free(Ym[i]); //free(Ym_notaligned[i]);
  delete[] Ym_notaligned;
  delete[] Ym;
  //QueryPerformanceCounter(&count5);
  return ExitCode;
}

// определяет длину пути между контактными площадками и конечным элементом. Нужно для "направленного синтеза" (который в реальности не эффеткивен)
void CRCStructure::WaveCast(int ***pWave, int *pShortWay, int *pLongWay)
{
  for (int i=1; i<3; ++i)
    for (int j=0; j<m_x; ++j)
      memset(pWave[i][j], 0, sizeof(int)*m_y);

  TMasKP *kp = m_pMasKP[0];
  while (kp->KPType==1)
    kp = kp->next;
  TMasKP *begin = kp;
  while (kp->KPType!=1)
    kp = kp->next;

  while (kp != begin)
  {
    if (kp->KPType==1 && kp->KPNumber)
      kp = Wave(pWave, kp);
    kp = kp->next;
  }

  *pShortWay = 0x7FFFFFFF;
  *pLongWay = 0;
  for (int i=0; i<m_x; ++i)
    for (int j=0; j<m_y; ++j)
    {
      pWave[0][i][j] = pWave[1][i][j] + pWave[2][i][j];
      if (pWave[0][i][j] && pWave[0][i][j] < *pShortWay)
        *pShortWay = pWave[0][i][j];
      if (pWave[0][i][j] > *pLongWay)
        *pLongWay = pWave[0][i][j];
    }
}

// внутренняя функция
TMasKP* CRCStructure::Wave(int ***pWave, TMasKP *kp)
{
  TPoint *pOldFront = new TPoint[m_x*m_y];
  TPoint *pNewFront = new TPoint[m_x*m_y];
  TPoint *pTempFront;

  int NewCount = 0;
  int OldCount;
  int T = 1;
  TMasKP *begin = kp;
  int KPNumber = kp->KPNumber;
  while (kp->KPType==1)
  {
    pWave[KPNumber][kp->x][kp->y] = T;
    kp = kp->next;
  }
  ++T;
  kp = begin;
  while (kp->KPType==1)
  {
    if (kp->y>0                    && !pWave[KPNumber][kp->x][kp->y-1] && m_pMatrix[kp->x][kp->y-1])
    {
      pNewFront[NewCount++]            = TPoint(kp->x, kp->y-1);
      pWave[KPNumber][kp->x][kp->y-1] = T;
    }
    if (kp->y>0     && kp->x<m_x-1 && !pWave[KPNumber][kp->x+1][kp->y-1] && m_pMatrix[kp->x+1][kp->y-1])
    {
      pNewFront[NewCount++]            = TPoint(kp->x+1, kp->y-1);
      pWave[KPNumber][kp->x+1][kp->y-1] = T;
    }
    if (               kp->x<m_x-1 && !pWave[KPNumber][kp->x+1][kp->y] && m_pMatrix[kp->x+1][kp->y])
    {
      pNewFront[NewCount++]            = TPoint(kp->x+1, kp->y);
      pWave[KPNumber][kp->x+1][kp->y] = T;
    }
    if (kp->y<m_y-1 && kp->x<m_x-1 && !pWave[KPNumber][kp->x+1][kp->y+1] && m_pMatrix[kp->x+1][kp->y+1])
    {
      pNewFront[NewCount++]            = TPoint(kp->x+1, kp->y+1);
      pWave[KPNumber][kp->x+1][kp->y+1] = T;
    }
    if (kp->y<m_y-1                && !pWave[KPNumber][kp->x][kp->y+1] && m_pMatrix[kp->x][kp->y+1])
    {
      pNewFront[NewCount++]            = TPoint(kp->x, kp->y+1);
      pWave[KPNumber][kp->x][kp->y+1] = T;
    }
    if (kp->y<m_y-1 && kp->x>0     && !pWave[KPNumber][kp->x-1][kp->y+1] && m_pMatrix[kp->x-1][kp->y+1])
    {
      pNewFront[NewCount++]            = TPoint(kp->x-1, kp->y+1);
      pWave[KPNumber][kp->x-1][kp->y+1] = T;
    }
    if (               kp->x>0     && !pWave[KPNumber][kp->x-1][kp->y] && m_pMatrix[kp->x-1][kp->y])
    {
      pNewFront[NewCount++]            = TPoint(kp->x-1, kp->y);
      pWave[KPNumber][kp->x-1][kp->y] = T;
    }
    if (kp->y>0     && kp->x>0     && !pWave[KPNumber][kp->x-1][kp->y-1] && m_pMatrix[kp->x-1][kp->y-1])
    {
      pNewFront[NewCount++]            = TPoint(kp->x-1, kp->y-1);
      pWave[KPNumber][kp->x-1][kp->y-1] = T;
    }
    kp = kp->next;
  }

  do
  {
    ++T;
    pTempFront = pOldFront;
    pOldFront = pNewFront;
    pNewFront = pTempFront;
    OldCount = NewCount;
    NewCount = 0;

    while (OldCount--)
    {
      // распространение волны вверх
      if (pOldFront[OldCount].y>0                                    && !pWave[KPNumber][pOldFront[OldCount].x][pOldFront[OldCount].y-1] && m_pMatrix[pOldFront[OldCount].x][pOldFront[OldCount].y-1])
      {
        pNewFront[NewCount++] = TPoint(pOldFront[OldCount].x, pOldFront[OldCount].y-1);
        pWave[KPNumber][pOldFront[OldCount].x][pOldFront[OldCount].y-1] = T;
      }
      // вправо-вверх
      if (pOldFront[OldCount].y>0     && pOldFront[OldCount].x<m_x-1 && !pWave[KPNumber][pOldFront[OldCount].x+1][pOldFront[OldCount].y-1] && m_pMatrix[pOldFront[OldCount].x+1][pOldFront[OldCount].y-1])
      {
        pNewFront[NewCount++] = TPoint(pOldFront[OldCount].x+1, pOldFront[OldCount].y-1);
        pWave[KPNumber][pOldFront[OldCount].x+1][pOldFront[OldCount].y-1] = T;
      }
      // вправо
      if (                               pOldFront[OldCount].x<m_x-1 && !pWave[KPNumber][pOldFront[OldCount].x+1][pOldFront[OldCount].y] && m_pMatrix[pOldFront[OldCount].x+1][pOldFront[OldCount].y])
      {
        pNewFront[NewCount++] = TPoint(pOldFront[OldCount].x+1, pOldFront[OldCount].y);
        pWave[KPNumber][pOldFront[OldCount].x+1][pOldFront[OldCount].y] = T;
      }
      // вправо-вниз
      if (pOldFront[OldCount].y<m_y-1 && pOldFront[OldCount].x<m_x-1 && !pWave[KPNumber][pOldFront[OldCount].x+1][pOldFront[OldCount].y+1] && m_pMatrix[pOldFront[OldCount].x+1][pOldFront[OldCount].y+1])
      {
        pNewFront[NewCount++] = TPoint(pOldFront[OldCount].x+1, pOldFront[OldCount].y+1);
        pWave[KPNumber][pOldFront[OldCount].x+1][pOldFront[OldCount].y+1] = T;
      }
      // вниз
      if (pOldFront[OldCount].y<m_y-1                                && !pWave[KPNumber][pOldFront[OldCount].x][pOldFront[OldCount].y+1] && m_pMatrix[pOldFront[OldCount].x][pOldFront[OldCount].y+1])
      {
        pNewFront[NewCount++] = TPoint(pOldFront[OldCount].x, pOldFront[OldCount].y+1);
        pWave[KPNumber][pOldFront[OldCount].x][pOldFront[OldCount].y+1] = T;
      }
      // влево-вниз
      if (pOldFront[OldCount].y<m_y-1 && pOldFront[OldCount].x>0     && !pWave[KPNumber][pOldFront[OldCount].x-1][pOldFront[OldCount].y+1] && m_pMatrix[pOldFront[OldCount].x-1][pOldFront[OldCount].y+1])
      {
        pNewFront[NewCount++] = TPoint(pOldFront[OldCount].x-1, pOldFront[OldCount].y+1);
        pWave[KPNumber][pOldFront[OldCount].x-1][pOldFront[OldCount].y+1] = T;
      }
      // влево
      if (                               pOldFront[OldCount].x>0     && !pWave[KPNumber][pOldFront[OldCount].x-1][pOldFront[OldCount].y] && m_pMatrix[pOldFront[OldCount].x-1][pOldFront[OldCount].y])
      {
        pNewFront[NewCount++] = TPoint(pOldFront[OldCount].x-1, pOldFront[OldCount].y);
        pWave[KPNumber][pOldFront[OldCount].x-1][pOldFront[OldCount].y] = T;
      }
      // влево-вверх
      if (pOldFront[OldCount].y>0     && pOldFront[OldCount].x>0     && !pWave[KPNumber][pOldFront[OldCount].x-1][pOldFront[OldCount].y-1] && m_pMatrix[pOldFront[OldCount].x-1][pOldFront[OldCount].y-1])
      {
        pNewFront[NewCount++] = TPoint(pOldFront[OldCount].x-1, pOldFront[OldCount].y-1);
        pWave[KPNumber][pOldFront[OldCount].x-1][pOldFront[OldCount].y-1] = T;
      }
    }
  }
  while (NewCount);
  delete[] pNewFront;
  delete[] pOldFront;
  return kp->prev;
}

// возвращает число контактных площадок
unsigned int CRCStructure::GetKPQuantity()
{
  unsigned int k = 0;
  for (int l=0; l<m_Layers; ++l)
    k += m_pKPQuantity[l];
  return k;
}

// проверяет одинаковы ли структуры
bool CRCStructure::Equal(CRCStructure *S)
{

  for (int i=0; i<m_x; ++i)
    for (int j=0; j<m_y; ++j)
       if (m_pMatrix[i][j] != S->m_pMatrix[i][j])
        return false;

/*
  for (int i=0; i<m_x; ++i)
    if (memcmp(m_pMatrix[i], S->m_pMatrix[i], sizeof(int)*j))
      return false;
*/
  for (int l=0; l<m_Layers; ++l)
    for (int i=0; i<m_Perimeter; ++i)
      if ((m_pMasKP[l][i].KPType) != (S->m_pMasKP[l][i].KPType))
        return false;

  return true;
}

// оператор мутации
void CRCStructure::Mutate(int prob_res, int prob_cond, bool isfit, int x, int y, int width, int height, int prob1)
{
  int MutateVector = 0;
  int MutateBit = 1;

  if (isfit)
  {
    MutateVector = ~MutateVector;
    for (int l=0; l<m_Layers; ++l)
    {
      if (prob_res > rand())
        MutateVector &= ~MutateBit;
      MutateBit <<= 1;
      if (prob_cond > rand())
        MutateVector &= ~MutateBit;
      MutateBit <<= 1;
    }

    for (int i=x; i<=x+width && i<m_x; ++i)
    {
      for (int j=y; j<=y+height && j<m_y; ++j)
      {
        m_pMatrix[i][j] = FixElement(m_pMatrix[i][j] & (MutateVector | m_pMatrixMask[i][j]));
      }
    }
  }
  else
  {
    for (int l=0; l<m_Layers; ++l)
    {
      /*
      if (prob_res > rand())
        MutateVector |= MutateBit;
      MutateBit <<= 1;
      if (prob_cond > rand())
        MutateVector |= MutateBit;
      MutateBit <<= 1;
      */

      if (prob_res > rand())
        MutateVector |= 1;
      if (prob1 > rand())
        MutateVector |= 4;
    }

    for (int i=x; i<=x+width && i<m_x; ++i)
    {
      for (int j=y; j<=y+height && j<m_y; ++j)
      {
        m_pMatrix[i][j] = FixElement(m_pMatrix[i][j] ^ (MutateVector & ~m_pMatrixMask[i][j]));
      }
    }
  }
}

// изменение размера структуры. Билинейная интерполяция
void CRCStructure::Resize(int new_x, int new_y)
{
  if ((new_x==m_x && new_y==m_y) || new_x < 2 || new_y < 2) return; // Размер структуры должен быть не меньше 2x2

  int **new_Matrix = new int*[new_x];

  // цикл по новой структуре
  for (int i=0; i<new_x; ++i)
  {
    new_Matrix[i] = new int[new_y];
    for (int j=0; j<new_y; ++j)
    {
      int temp = 0;
      double sx = (double)(i*(m_x-1))/(double)(new_x-1);
      double sy = (double)(j*(m_y-1))/(double)(new_y-1);
      int fsx = floor(sx); // round down
      int fsy = floor(sy);
      int csx = ceil(sx); // round up
      int csy = ceil(sy);
      sx -= fsx;
      sy -= fsy;

      if (csx == m_x) --csx;
      if (csy == m_y) --csy;
      if (csx<m_x && csy<m_y)
      {
        for (int l=(m_Layers<<1)-1; l>=0; --l)
        {
          double val = ((m_pMatrix[fsx][fsy] >> l) & 1)*(1.0-sx)*(1.0-sy)
                     + ((m_pMatrix[fsx][csy] >> l) & 1)*(1.0-sx)*(sy)
                     + ((m_pMatrix[csx][fsy] >> l) & 1)*(sx)*(1.0-sy)
                     + ((m_pMatrix[csx][csy] >> l) & 1)*(sx)*(sy);
          if (val>=0.5)
            temp |= (1 << l);
        }
      }
      new_Matrix[i][j] = FixElement(temp);
    }
  }

  for(int i=0; i<m_x; delete[] m_pMatrix[i++]);
  delete[] m_pMatrix;
  m_pMatrix = new_Matrix;

  for (int l=0; l<m_Layers; delete[] m_pCircuitNodes[l++])
    for (int i=0; i<=m_x; delete[] m_pCircuitNodes[l][i++]);
  delete[] m_pCircuitNodes;
  delete[] m_pRowSizes;

  m_pCircuitNodes = new int**[m_Layers];
  for (int l=0; l<m_Layers; ++l)
  {
    m_pCircuitNodes[l] = new int*[new_x+1];
    for (int i=0; i<=new_x; ++i)
    {
      m_pCircuitNodes[l][i] = new int[new_y+1];
    }
  }
  m_pRowSizes = new int[(new_x+1)*(new_y+1)*m_Layers];

  // для правильной работы функции InitMasKP нужно установить новые m_x, m_y, m_Perimeter
  int old_x = m_x;
  int old_y = m_y;
  m_x = new_x;
  m_y = new_y;
  m_Kf = (double)new_x/(double)new_y;
  m_Perimeter = (new_x+new_y)<<1;

  TMasKP **new_MasKP = new TMasKP*[m_Layers];
  for (int l=0; l<m_Layers; ++l)
  {
    new_MasKP[l] = new TMasKP[m_Perimeter];
  }
  InitMasKP(new_MasKP);


  for (int l=0; l<m_Layers; ++l)
  {
    KPResizer(new_x, old_x, &new_MasKP[l][0], &m_pMasKP[l][0]);
    KPResizer(new_y, old_y, &new_MasKP[l][new_x], &m_pMasKP[l][old_x]);
    KPResizer(new_x, old_x, &new_MasKP[l][new_x+new_y], &m_pMasKP[l][old_x+old_y]);
    KPResizer(new_y, old_y, &new_MasKP[l][new_x+new_y+new_x], &m_pMasKP[l][old_x+old_y+old_x]);
    delete[] m_pMasKP[l];
  }

  delete[] m_pMasKP;
  m_pMasKP = new_MasKP;

  Dorabotka();
}

// внутренняя функция. Нужна для изменения размера структуры
void CRCStructure::KPResizer(int new_size, int old_size, TMasKP *new_KP, TMasKP *old_KP)
{
  TMasKP *new_pointer, *old_pointer;
  if (new_size > old_size)
  {
    for (int i=0; i<new_size; ++i)
    {
      double sp = (double)(i*(old_size-1))/(double)(new_size-1);
      double fsp = floor(sp);
      int k;
      if (sp - fsp >= 0.5)
        k = ceil(sp);
      else
        k = fsp;

      new_pointer = new_KP + i;
      old_pointer = old_KP + k;

      if (!new_pointer->KPType)
      {
        new_pointer->KPType = old_pointer->KPType;
        new_pointer->KPNumber = old_pointer->KPNumber;
      }
    }
  }
  else // (old_size > new_size)
  {
    for (int i=0; i<old_size; ++i)
    {
      double sp = (double)(i*(new_size-1))/(double)(old_size-1);
      double fsp = floor(sp);
      int k;
      if (sp - fsp >= 0.5)
        k = ceil(sp);
      else
        k = fsp;

      new_pointer = new_KP + k;
      old_pointer = old_KP + i;

      if (!new_pointer->KPType)
      {
        new_pointer->KPType = old_pointer->KPType;
        new_pointer->KPNumber = old_pointer->KPNumber;
      }
    }
  }
}


