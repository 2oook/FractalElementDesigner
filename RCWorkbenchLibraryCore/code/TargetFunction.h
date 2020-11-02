//---------------------------------------------------------------------------

#ifndef TargetFunctionH
#define TargetFunctionH

#include <complex>
#include "ap.h"
using namespace std;

class CRCStructureCalculateData
{
public:
  int m_length;
  double *m_w, *m_char[5];
  complex<double> **m_MatY, *m_T, *m_Tapprox;
  double*** y_result;
  ap::real_1d_array m_sol, NumResR, NumResI, DenResR, DenResI;

  CRCStructureCalculateData(int length)
  {
    m_length = length;
    m_char[0] = new double[m_length];// свободная серия для вывода графика (использования не обнаружено)
    m_char[1] = new double[m_length];//  для вывода по вертикальной оси серии АЧХ или ФЧХ
    m_char[2] = new double[m_length];// для вывода по вертикальной оси серии годографа
    m_char[3] = new double[m_length];// свободная серия для вывода графика
    m_char[4] = new double[m_length];// свободная серия для вывода графика
    m_w = new double[m_length];// частоты (с распределением линейным либо логарифмическим)
    m_T = new complex<double>[m_length];// массив комплексных чисел после уменьшения порядка до 1 (СКОРЕЕ ВСЕГО)
    m_Tapprox = new complex<double>[m_length];
    m_MatY = new complex<double>*[m_length];// левые треугольники разложенных в вектор матриц проводимости по частотам 

    y_result = new double** [m_length];

    for (int i=0; i<m_length; ++i)
    {   // 36 - это количество ячеек в треугольнике матрице + главная диагональ???? в матрице 8х8 // потому что существует только схема максимум для 8ми КП
      m_MatY[i] = new complex<double>[36]; // 8 контактных площадок 

      y_result[i] = new double* [36];

      for (size_t k = 0; k < 36; k++)
      {
          y_result[i][k] = new double[2];
      }

      m_char[0][i] = 0;
    }
  }

  ~CRCStructureCalculateData()
  {
    for(int i=0; i<m_length; delete[] m_MatY[i++]);
    delete[] m_MatY;
    delete[] m_w;
    delete[] m_T;
    delete[] m_Tapprox;
    delete[] m_char[0];
    delete[] m_char[1];
    delete[] m_char[2];
    delete[] m_char[3];
    delete[] m_char[4];
  }
};

class CTargetFunction : public CRCStructureCalculateData
{
public:
  double *m_low, *m_high;
  bool *m_en_low, *m_en_high;

  CTargetFunction(int length) : CRCStructureCalculateData(length)
  {
    m_low = new double[m_length];
    m_high = new double[m_length];
    m_en_low = new bool[m_length];
    m_en_high = new bool[m_length];

    for (int i=0; i<m_length; ++i)
    {
      m_char[1][i] = 0;
      m_char[2][i] = 0;
    }
  }

  ~CTargetFunction()
  {
    delete[] m_low;
    delete[] m_high;
    delete[] m_en_low;
    delete[] m_en_high;
  }

  double Deviation(int structure);
  double MeanValue(int structure);
  double DeviationArray(int structure, double *DevArray);
};


//---------------------------------------------------------------------------
#endif
