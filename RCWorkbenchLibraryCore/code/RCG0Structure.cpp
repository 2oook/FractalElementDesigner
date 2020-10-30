//---------------------------------------------------------------------------

#pragma hdrstop
#include "RCG0Structure.h"

//---------------------------------------------------------------------------
#pragma package(smart_init)

void CRCG0Structure::InitMatR()
{
  double Z_x = m_R*2.0;
  double Z_y = m_R*2.0;

  double Yrx = 1.0/Z_x;
  double Yry = 1.0/Z_y;

  Mat_r[0][0]=Yrx+Yry;     Mat_r[0][1]=-Yrx;        Mat_r[0][2]=-Yry;        Mat_r[0][3]=0.0;
  Mat_r[1][0]=Mat_r[0][1]; Mat_r[1][1]=Mat_r[0][0]; Mat_r[1][2]=0.0;         Mat_r[1][3]=Mat_r[0][2];
  Mat_r[2][0]=Mat_r[0][2]; Mat_r[2][1]=0.0;         Mat_r[2][2]=Mat_r[0][0]; Mat_r[2][3]=Mat_r[0][1];
  Mat_r[3][0]=0.0;         Mat_r[3][1]=Mat_r[0][2]; Mat_r[3][2]=Mat_r[0][1]; Mat_r[3][3]=Mat_r[0][0];
}

void CRCG0Structure::Dorabotka()
{
  for (int i=0; i<m_x-1; i++)
  {
    for (int j=0; j<m_y-1; j++)
    {
      if (m_pMatrix[i][j]==0 && m_pMatrix[i+1][j+1]==0 && m_pMatrix[i+1][j]!=0 && m_pMatrix[i][j+1]!=0)
        m_pMatrix[i][j] = 3;
      else if (m_pMatrix[i][j]!=0 && m_pMatrix[i+1][j+1]!=0 && m_pMatrix[i+1][j]==0 && m_pMatrix[i][j+1]==0)
        m_pMatrix[i+1][j] = 3;
    }
  }

  for (int i=0; i<m_Perimeter; i++)
  {
    TMasKP *temp = &m_pMasKP[0][i];
    if (temp->KPType==KPNORMAL && m_pMatrix[temp->x][temp->y]==0)
      m_pMatrix[temp->x][temp->y] = 3;
  }
}


void CRCG0Structure::FillGlobalMatrix(complex<double> **Ym, double w)
{
  complex<double> Mat_rc[4][4];

  double Z_x = m_R*2.0;
  double Z_y = m_R*2.0;

  complex<double> p = complex<double>(0.0, w);
  complex<double> temp = 1.0/m_H + p*m_R*m_C;
  complex<double> Y = temp/((1.0+m_G*temp)*((double)(4*m_x*m_y))*m_R);
  //complex<double> temp = 1.0/m_H + p*Z_x*m_C;
  //complex<double> Y = temp/((1.0+m_G*temp)*((double)(4*m_x*m_y))*Z_x);

  complex<double> tetta_x = sqrt(Z_x*Y);
  complex<double> tetta_y = sqrt(Z_y*Y);

  double koeff_x = 1.0 / Z_x;
  double koeff_y = 1.0 / Z_y;

  complex<double> Yxaa =  koeff_x*tetta_x/tanh(tetta_x);
  complex<double> Yxab = -koeff_x*tetta_x/sinh(tetta_x);
  complex<double> Yyaa =  koeff_y*tetta_y/tanh(tetta_y);
  complex<double> Yyab = -koeff_y*tetta_y/sinh(tetta_y);

  Yyaa += Yxaa;
  Mat_rc[0][0]=Yyaa; Mat_rc[0][1]=Yxab; Mat_rc[0][2]=Yyab; Mat_rc[0][3]=0.0;
  Mat_rc[1][0]=Yxab; Mat_rc[1][1]=Yyaa; Mat_rc[1][2]=0.0;  Mat_rc[1][3]=Yyab;
  Mat_rc[2][0]=Yyab; Mat_rc[2][1]=0.0;  Mat_rc[2][2]=Yyaa; Mat_rc[2][3]=Yxab;
  Mat_rc[3][0]=0.0;  Mat_rc[3][1]=Yyab; Mat_rc[3][2]=Yxab; Mat_rc[3][3]=Yyaa;

  //int u0;
  for (int i=0; i<m_x; i++)
  {
    for (int j=0; j<m_y; j++)
    {
      switch(m_pMatrix[i][j])
      {
        case 1:
        {
          AddElement44ToMatrix(Mat_r);
        }
        break;
        case 3:
        {
          AddElement44ToMatrix(Mat_rc);
        }
        break;
      }
    }
  }

}

int CRCG0Structure::FixElement(int Element)
{
  static const int fixtable[] = {0, 1, 0, 3};
  return fixtable[Element&3];
}

