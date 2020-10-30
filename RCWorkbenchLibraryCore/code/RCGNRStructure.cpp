//---------------------------------------------------------------------------

#pragma hdrstop
#include "RCGNRStructure.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)



// ячейка R (1) (3 уникальных значения)
void CRCGNRStructure::InitMatR()
{
  double Z_x = m_R*2.0;
  double Z_y = m_R*2.0;

  double Yrx = 1.0/Z_x;
  double Yry = 1.0/Z_y;

  Mat_r[0][0]=Yrx+Yry;     Mat_r[0][1]=-Yrx;        Mat_r[0][2]=-Yry;        Mat_r[0][3]=0.0;         Mat_r[0][4]=0.0; Mat_r[0][5]=0.0; Mat_r[0][6]=0.0; Mat_r[0][7]=0.0;
  Mat_r[1][0]=Mat_r[0][1]; Mat_r[1][1]=Mat_r[0][0]; Mat_r[1][2]=0.0;         Mat_r[1][3]=Mat_r[0][2]; Mat_r[1][4]=0.0; Mat_r[1][5]=0.0; Mat_r[1][6]=0.0; Mat_r[1][7]=0.0;
  Mat_r[2][0]=Mat_r[0][2]; Mat_r[2][1]=0.0;         Mat_r[2][2]=Mat_r[0][0]; Mat_r[2][3]=Mat_r[0][1]; Mat_r[2][4]=0.0; Mat_r[2][5]=0.0; Mat_r[2][6]=0.0; Mat_r[2][7]=0.0;
  Mat_r[3][0]=0.0;         Mat_r[3][1]=Mat_r[0][2]; Mat_r[3][2]=Mat_r[0][1]; Mat_r[3][3]=Mat_r[0][0]; Mat_r[3][4]=0.0; Mat_r[3][5]=0.0; Mat_r[3][6]=0.0; Mat_r[3][7]=0.0;
  Mat_r[4][0]=0.0;         Mat_r[4][1]=0.0;         Mat_r[4][2]=0.0;         Mat_r[4][3]=0.0;         Mat_r[4][4]=0.0; Mat_r[4][5]=0.0; Mat_r[4][6]=0.0; Mat_r[4][7]=0.0;
  Mat_r[5][0]=0.0;         Mat_r[5][1]=0.0;         Mat_r[5][2]=0.0;         Mat_r[5][3]=0.0;         Mat_r[5][4]=0.0; Mat_r[5][5]=0.0; Mat_r[5][6]=0.0; Mat_r[5][7]=0.0;
  Mat_r[6][0]=0.0;         Mat_r[6][1]=0.0;         Mat_r[6][2]=0.0;         Mat_r[6][3]=0.0;         Mat_r[6][4]=0.0; Mat_r[6][5]=0.0; Mat_r[6][6]=0.0; Mat_r[6][7]=0.0;
  Mat_r[7][0]=0.0;         Mat_r[7][1]=0.0;         Mat_r[7][2]=0.0;         Mat_r[7][3]=0.0;         Mat_r[7][4]=0.0; Mat_r[7][5]=0.0; Mat_r[7][6]=0.0; Mat_r[7][7]=0.0;
}

// ячейка NR (4) (3 уникальных значения)
void CRCGNRStructure::InitMatNR()
{
  double NZ_x = m_R*m_N*2.0;
  double NZ_y = m_R*m_N*2.0;

  double Ynrx = 1.0/NZ_x;
  double Ynry = 1.0/NZ_y;

  Mat_nr[0][0]=0.0; Mat_nr[0][1]=0.0; Mat_nr[0][2]=0.0; Mat_nr[0][3]=0.0; Mat_nr[0][4]=0.0;          Mat_nr[0][5]=0.0;          Mat_nr[0][6]=0.0;          Mat_nr[0][7]=0.0;
  Mat_nr[1][0]=0.0; Mat_nr[1][1]=0.0; Mat_nr[1][2]=0.0; Mat_nr[1][3]=0.0; Mat_nr[1][4]=0.0;          Mat_nr[1][5]=0.0;          Mat_nr[1][6]=0.0;          Mat_nr[1][7]=0.0;
  Mat_nr[2][0]=0.0; Mat_nr[2][1]=0.0; Mat_nr[2][2]=0.0; Mat_nr[2][3]=0.0; Mat_nr[2][4]=0.0;          Mat_nr[2][5]=0.0;          Mat_nr[2][6]=0.0;          Mat_nr[2][7]=0.0;
  Mat_nr[3][0]=0.0; Mat_nr[3][1]=0.0; Mat_nr[3][2]=0.0; Mat_nr[3][3]=0.0; Mat_nr[3][4]=0.0;          Mat_nr[3][5]=0.0;          Mat_nr[3][6]=0.0;          Mat_nr[3][7]=0.0;
  Mat_nr[4][0]=0.0; Mat_nr[4][1]=0.0; Mat_nr[4][2]=0.0; Mat_nr[4][3]=0.0; Mat_nr[4][4]=Ynrx+Ynry;    Mat_nr[4][5]=-Ynrx;        Mat_nr[4][6]=-Ynry;        Mat_nr[4][7]=0.0;
  Mat_nr[5][0]=0.0; Mat_nr[5][1]=0.0; Mat_nr[5][2]=0.0; Mat_nr[5][3]=0.0; Mat_nr[5][4]=Mat_nr[4][5]; Mat_nr[5][5]=Mat_nr[4][4]; Mat_nr[5][6]=0.0;          Mat_nr[5][7]=Mat_nr[4][6];
  Mat_nr[6][0]=0.0; Mat_nr[6][1]=0.0; Mat_nr[6][2]=0.0; Mat_nr[6][3]=0.0; Mat_nr[6][4]=Mat_nr[4][6]; Mat_nr[6][5]=0.0;          Mat_nr[6][6]=Mat_nr[4][4]; Mat_nr[6][7]=Mat_nr[4][5];
  Mat_nr[7][0]=0.0; Mat_nr[7][1]=0.0; Mat_nr[7][2]=0.0; Mat_nr[7][3]=0.0; Mat_nr[7][4]=0.0;          Mat_nr[7][5]=Mat_nr[4][6]; Mat_nr[7][6]=Mat_nr[4][5]; Mat_nr[7][7]=Mat_nr[4][4];
}

void CRCGNRStructure::FillGlobalMatrix(complex<double> **Ym, double w)
{
  complex<double> Mat_rcnr[8][8]; // Y - параметры RCNR - ячейки

  //double Z_x = m_R*2.0*(1.0+m_N)*(double)m_y/((double)m_x);
  //double Z_y = m_R*2.0*(1.0+m_N)*(double)m_x/((double)m_y);
  double Z_x = m_R*2.0*(1.0+m_N);
  double Z_y = m_R*2.0*(1.0+m_N);

  complex<double> p = complex<double>(0.0, w);
  complex<double> temp = 1.0/m_H + p*m_R*m_C;
  complex<double> Y = temp/((1.0+m_G*temp)*((double)(4*m_x*m_y))*m_R);
  //complex<double> temp = 1.0/m_H + p*Z_x*m_C;
  //complex<double> Y = temp/((1.0+m_G*temp)*((double)(4*m_x*m_y))*Z_x); // по идее это неправильно, т.к. G и H нормируются к R, а не к R*2*(N+1)

  complex<double> tetta_x = sqrt(Z_x*Y);
  complex<double> tetta_y = sqrt(Z_y*Y);

  double koeff_x = 1.0 / Z_x;
  double koeff_y = 1.0 / Z_y;

  complex<double> tx = tetta_x/tanh(tetta_x);
  complex<double> sx = tetta_x/sinh(tetta_x);
  complex<double> ty = tetta_y/tanh(tetta_y);
  complex<double> sy = tetta_y/sinh(tetta_y);

  tx *= koeff_x;
  sx *= koeff_x;
  ty *= koeff_y;
  sy *= koeff_y;

  double KxN = koeff_x*m_N;
  double KyN = koeff_y*m_N;
  double Kx_N = koeff_x/m_N;
  double Ky_N = koeff_y/m_N;


  // при 2x2 не вылазит выше 0dB
  //Mat_rcnr[0][0]=tx+ty+KxN+KyN;  Mat_rcnr[0][1]=-sx-KxN;        Mat_rcnr[0][2]=-sy-KyN;        Mat_rcnr[0][3]=0.0;            Mat_rcnr[0][4]=sx+sy-koeff_x-koeff_y; Mat_rcnr[0][5]=koeff_x-tx;     Mat_rcnr[0][6]=koeff_y-ty;     Mat_rcnr[0][7]=0.0;

  // при 2x2 вылазит выше 0dB
  Mat_rcnr[0][0]=tx+ty+KxN+KyN;  Mat_rcnr[0][1]=-sx-KxN;        Mat_rcnr[0][2]=-sy-KyN;        Mat_rcnr[0][3]=0.0;            Mat_rcnr[0][4]=koeff_x+koeff_y-tx-ty; Mat_rcnr[0][5]=sx-koeff_x;     Mat_rcnr[0][6]=sy-koeff_y;     Mat_rcnr[0][7]=0.0;
  Mat_rcnr[1][0]=Mat_rcnr[0][1]; Mat_rcnr[1][1]=Mat_rcnr[0][0]; Mat_rcnr[1][2]=0.0;            Mat_rcnr[1][3]=Mat_rcnr[0][2]; Mat_rcnr[1][4]=Mat_rcnr[0][5];        Mat_rcnr[1][5]=Mat_rcnr[0][4]; Mat_rcnr[1][6]=0.0;            Mat_rcnr[1][7]=Mat_rcnr[0][6];
  Mat_rcnr[2][0]=Mat_rcnr[0][2]; Mat_rcnr[2][1]=0.0;            Mat_rcnr[2][2]=Mat_rcnr[0][0]; Mat_rcnr[2][3]=Mat_rcnr[0][1]; Mat_rcnr[2][4]=Mat_rcnr[0][6];        Mat_rcnr[2][5]=0.0;            Mat_rcnr[2][6]=Mat_rcnr[0][4]; Mat_rcnr[2][7]=Mat_rcnr[0][5];
  Mat_rcnr[3][0]=0.0;            Mat_rcnr[3][1]=Mat_rcnr[0][2]; Mat_rcnr[3][2]=Mat_rcnr[0][1]; Mat_rcnr[3][3]=Mat_rcnr[0][0]; Mat_rcnr[3][4]=0.0;                   Mat_rcnr[3][5]=Mat_rcnr[0][6]; Mat_rcnr[3][6]=Mat_rcnr[0][5]; Mat_rcnr[3][7]=Mat_rcnr[0][4];
  Mat_rcnr[4][0]=Mat_rcnr[0][4]; Mat_rcnr[4][1]=Mat_rcnr[0][5]; Mat_rcnr[4][2]=Mat_rcnr[0][6]; Mat_rcnr[4][3]=0.0;            Mat_rcnr[4][4]=tx+ty+Kx_N+Ky_N;       Mat_rcnr[4][5]=-sx-Kx_N;       Mat_rcnr[4][6]=-sy-Ky_N;       Mat_rcnr[4][7]=0.0;
  Mat_rcnr[5][0]=Mat_rcnr[0][5]; Mat_rcnr[5][1]=Mat_rcnr[0][4]; Mat_rcnr[5][2]=0.0;            Mat_rcnr[5][3]=Mat_rcnr[0][6]; Mat_rcnr[5][4]=Mat_rcnr[4][5];        Mat_rcnr[5][5]=Mat_rcnr[4][4]; Mat_rcnr[5][6]=0.0;            Mat_rcnr[5][7]=Mat_rcnr[4][6];
  Mat_rcnr[6][0]=Mat_rcnr[0][6]; Mat_rcnr[6][1]=0.0;            Mat_rcnr[6][2]=Mat_rcnr[0][4]; Mat_rcnr[6][3]=Mat_rcnr[0][5]; Mat_rcnr[6][4]=Mat_rcnr[4][6];        Mat_rcnr[6][5]=0.0;            Mat_rcnr[6][6]=Mat_rcnr[4][4]; Mat_rcnr[6][7]=Mat_rcnr[4][5];
  Mat_rcnr[7][0]=0.0;            Mat_rcnr[7][1]=Mat_rcnr[0][6]; Mat_rcnr[7][2]=Mat_rcnr[0][5]; Mat_rcnr[7][3]=Mat_rcnr[0][4]; Mat_rcnr[7][4]=0.0;                   Mat_rcnr[7][5]=Mat_rcnr[4][6]; Mat_rcnr[7][6]=Mat_rcnr[4][5]; Mat_rcnr[7][7]=Mat_rcnr[4][4];


  for (int i=0; i<m_x; i++)
  {
    for (int j=0; j<m_y; j++)
    {
      switch(m_pMatrix[i][j])
      {
        case 1:
        {
          AddElement88ToMatrix(Mat_r);
        }
        break;
        case 4:
        {
          AddElement88ToMatrix(Mat_nr);
        }
        break;
        case 7:
        {
          // в глобальную матрицу Ym попадает только верхний треугольник относительно гл диагонали
          AddElement88ToMatrix(Mat_rcnr);
        }
        break;
      }
    }
  }

}

void CRCGNRStructure::Dorabotka()
{
  for (int i=0; i<m_x-1; i++)
  {
    for (int j=0; j<m_y-1; j++)
    {
      if ((m_pMatrix[i][j]&1)==0 && (m_pMatrix[i+1][j+1]&1)==0 && (m_pMatrix[i+1][j]&1)!=0 && (m_pMatrix[i][j+1]&1)!=0)
        m_pMatrix[i][j] = 7;
      else if ((m_pMatrix[i][j]&1)!=0 && (m_pMatrix[i+1][j+1]&1)!=0 && (m_pMatrix[i+1][j]&1)==0 && (m_pMatrix[i][j+1]&1)==0)
        m_pMatrix[i+1][j] = 7;
      if ((m_pMatrix[i][j]&4)==0 && (m_pMatrix[i+1][j+1]&4)==0 && (m_pMatrix[i+1][j]&4)!=0 && (m_pMatrix[i][j+1]&4)!=0)
        m_pMatrix[i][j] = 7;
      else if ((m_pMatrix[i][j]&4)!=0 && (m_pMatrix[i+1][j+1]&4)!=0 && (m_pMatrix[i+1][j]&4)==0 && (m_pMatrix[i][j+1]&4)==0)
        m_pMatrix[i+1][j] = 7;
    }
  }

  int *pElement = NULL;
  for (int i=0; i<m_Perimeter; i++)
  {
    if (m_pMasKP[0][i].KPType==1)
    {
      pElement = &m_pMatrix[m_pMasKP[0][i].x][m_pMasKP[0][i].y];
      *pElement |= 1;
    }
    if (m_pMasKP[1][i].KPType==1)
    {
      pElement = &m_pMatrix[m_pMasKP[1][i].x][m_pMasKP[1][i].y];
      *pElement |= 4;
    }
    if (pElement)
    {
      *pElement = FixElement(*pElement);
      pElement = NULL;
    }
  }

}


int CRCGNRStructure::FixElement(int Element)
{
  static const int fixtable[] = {0, 1, 0, 1, 4, 7, 4, 7};
  return fixtable[Element&7];
}

