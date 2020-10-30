//---------------------------------------------------------------------------

#pragma hdrstop
#include "TargetFunction.h"

//---------------------------------------------------------------------------

#pragma package(smart_init)


double CTargetFunction::Deviation(int structure)
{
  double otkl;
  double summ = 0.0;
  for(int i=0; i<m_length; ++i) // перебор всех точек
  {
    if (m_en_high[i] && (otkl = m_char[structure][i] - m_high[i]) > 0)
    {
      summ += otkl*otkl;
    }
    else if (m_en_low[i] && (otkl = m_low[i] - m_char[structure][i]) > 0)
    {
      summ += otkl*otkl;
    }
  }
  return summ/m_length;
}

double CTargetFunction::MeanValue(int structure)
{
  double summ = 0.0;
  for(int i=0; i<m_length; ++i) // перебор всех точек
  {
    summ += fabs(m_char[structure][i]);
  }
  return summ/m_length;
}

double CTargetFunction::DeviationArray(int structure, double *DevArray)
{
  double otkl;
  double summ = 0.0;

  for(int i=0; i<m_length; ++i) // перебор всех точек
  {
    if (m_en_high[i] && (otkl = m_char[structure][i] - m_high[i]) > 0)
    {
      DevArray[i] = otkl*otkl;
      summ += DevArray[i];
    }
    else if (m_en_low[i] && (otkl = m_low[i] - m_char[structure][i]) > 0)
    {
      DevArray[i] = otkl*otkl;
      summ += DevArray[i];
    }
  }
  for(int i=0; i<m_length; ++i)
    DevArray[i] /= summ;
  return summ/m_length;
}

