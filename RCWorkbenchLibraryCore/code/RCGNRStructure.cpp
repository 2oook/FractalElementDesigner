//---------------------------------------------------------------------------

#pragma hdrstop
#include "RCGNRStructure.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)



// ������ R (1) (3 ���������� ��������)
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

// ������ NR (4) (3 ���������� ��������)
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
    // RkCNRk
    complex<double> Mat_Rk_C_NRk[8][8]{0}; // Y - ��������� Rk_C_NRk - ������

    double Z_x_Rk_C_NRk = m_R * 2.0 * (1.0 + m_N);
    double Z_y_Rk_C_NRk = m_R * 2.0 * (1.0 + m_N);

    complex<double> p_Rk_C_NRk = complex<double>(0.0, w);
    complex<double> temp_Rk_C_NRk = 1.0 / m_H + p_Rk_C_NRk * m_R * m_C;
    complex<double> Y_Rk_C_NRk = temp_Rk_C_NRk / ((1.0 + m_G * temp_Rk_C_NRk) * ((double)(4 * m_x * m_y)) * m_R);

    complex<double> tetta_x_Rk_C_NRk = sqrt(Z_x_Rk_C_NRk * Y_Rk_C_NRk);
    complex<double> tetta_y_Rk_C_NRk = sqrt(Z_y_Rk_C_NRk * Y_Rk_C_NRk);

    double koeff_x_Rk_C_NRk = 1.0 / Z_x_Rk_C_NRk;
    double koeff_y_Rk_C_NRk = 1.0 / Z_y_Rk_C_NRk;

    complex<double> tx_Rk_C_NRk = tetta_x_Rk_C_NRk / tanh(tetta_x_Rk_C_NRk);
    complex<double> sx_Rk_C_NRk = tetta_x_Rk_C_NRk / sinh(tetta_x_Rk_C_NRk);
    complex<double> ty_Rk_C_NRk = tetta_y_Rk_C_NRk / tanh(tetta_y_Rk_C_NRk);
    complex<double> sy_Rk_C_NRk = tetta_y_Rk_C_NRk / sinh(tetta_y_Rk_C_NRk);

    tx_Rk_C_NRk *= koeff_x_Rk_C_NRk;
    sx_Rk_C_NRk *= koeff_x_Rk_C_NRk;
    ty_Rk_C_NRk *= koeff_y_Rk_C_NRk;
    sy_Rk_C_NRk *= koeff_y_Rk_C_NRk;

    double KxN_Rk_C_NRk = koeff_x_Rk_C_NRk * m_N;
    double KyN_Rk_C_NRk = koeff_y_Rk_C_NRk * m_N;
    double Kx_N_Rk_C_NRk = koeff_x_Rk_C_NRk / m_N;
    double Ky_N_Rk_C_NRk = koeff_y_Rk_C_NRk / m_N;

    Mat_Rk_C_NRk[0][0] = tx_Rk_C_NRk + ty_Rk_C_NRk + KxN_Rk_C_NRk + KyN_Rk_C_NRk;       Mat_Rk_C_NRk[0][1] = -sx_Rk_C_NRk - KxN_Rk_C_NRk;               Mat_Rk_C_NRk[0][2] = -sy_Rk_C_NRk - KyN_Rk_C_NRk;               Mat_Rk_C_NRk[0][3] = 0.0;                   Mat_Rk_C_NRk[0][4] = koeff_x_Rk_C_NRk + koeff_y_Rk_C_NRk - tx_Rk_C_NRk - ty_Rk_C_NRk;       Mat_Rk_C_NRk[0][5] = sx_Rk_C_NRk - koeff_x_Rk_C_NRk;            Mat_Rk_C_NRk[0][6] = sy_Rk_C_NRk - koeff_y_Rk_C_NRk;            Mat_Rk_C_NRk[0][7] = 0.0;
    Mat_Rk_C_NRk[1][0] = Mat_Rk_C_NRk[0][1];                                            Mat_Rk_C_NRk[1][1] = Mat_Rk_C_NRk[0][0];                        Mat_Rk_C_NRk[1][2] = 0.0;                                       Mat_Rk_C_NRk[1][3] = Mat_Rk_C_NRk[0][2];    Mat_Rk_C_NRk[1][4] = Mat_Rk_C_NRk[0][5];                                                    Mat_Rk_C_NRk[1][5] = Mat_Rk_C_NRk[0][4];                        Mat_Rk_C_NRk[1][6] = 0.0;                                       Mat_Rk_C_NRk[1][7] = Mat_Rk_C_NRk[0][6];
    Mat_Rk_C_NRk[2][0] = Mat_Rk_C_NRk[0][2];                                            Mat_Rk_C_NRk[2][1] = 0.0;                                       Mat_Rk_C_NRk[2][2] = Mat_Rk_C_NRk[0][0];                        Mat_Rk_C_NRk[2][3] = Mat_Rk_C_NRk[0][1];    Mat_Rk_C_NRk[2][4] = Mat_Rk_C_NRk[0][6];                                                    Mat_Rk_C_NRk[2][5] = 0.0;                                       Mat_Rk_C_NRk[2][6] = Mat_Rk_C_NRk[0][4];                        Mat_Rk_C_NRk[2][7] = Mat_Rk_C_NRk[0][5];
    Mat_Rk_C_NRk[3][0] = 0.0;                                                           Mat_Rk_C_NRk[3][1] = Mat_Rk_C_NRk[0][2];                        Mat_Rk_C_NRk[3][2] = Mat_Rk_C_NRk[0][1];                        Mat_Rk_C_NRk[3][3] = Mat_Rk_C_NRk[0][0];    Mat_Rk_C_NRk[3][4] = 0.0;                                                                   Mat_Rk_C_NRk[3][5] = Mat_Rk_C_NRk[0][6];                        Mat_Rk_C_NRk[3][6] = Mat_Rk_C_NRk[0][5];                        Mat_Rk_C_NRk[3][7] = Mat_Rk_C_NRk[0][4];
    Mat_Rk_C_NRk[4][0] = Mat_Rk_C_NRk[0][4];                                            Mat_Rk_C_NRk[4][1] = Mat_Rk_C_NRk[0][5];                        Mat_Rk_C_NRk[4][2] = Mat_Rk_C_NRk[0][6];                        Mat_Rk_C_NRk[4][3] = 0.0;                   Mat_Rk_C_NRk[4][4] = tx_Rk_C_NRk + ty_Rk_C_NRk + Kx_N_Rk_C_NRk + Ky_N_Rk_C_NRk;             Mat_Rk_C_NRk[4][5] = -sx_Rk_C_NRk - Kx_N_Rk_C_NRk;              Mat_Rk_C_NRk[4][6] = -sy_Rk_C_NRk - Ky_N_Rk_C_NRk;              Mat_Rk_C_NRk[4][7] = 0.0;
    Mat_Rk_C_NRk[5][0] = Mat_Rk_C_NRk[0][5];                                            Mat_Rk_C_NRk[5][1] = Mat_Rk_C_NRk[0][4];                        Mat_Rk_C_NRk[5][2] = 0.0;                                       Mat_Rk_C_NRk[5][3] = Mat_Rk_C_NRk[0][6];    Mat_Rk_C_NRk[5][4] = Mat_Rk_C_NRk[4][5];                                                    Mat_Rk_C_NRk[5][5] = Mat_Rk_C_NRk[4][4];                        Mat_Rk_C_NRk[5][6] = 0.0;                                       Mat_Rk_C_NRk[5][7] = Mat_Rk_C_NRk[4][6];
    Mat_Rk_C_NRk[6][0] = Mat_Rk_C_NRk[0][6];                                            Mat_Rk_C_NRk[6][1] = 0.0;                                       Mat_Rk_C_NRk[6][2] = Mat_Rk_C_NRk[0][4];                        Mat_Rk_C_NRk[6][3] = Mat_Rk_C_NRk[0][5];    Mat_Rk_C_NRk[6][4] = Mat_Rk_C_NRk[4][6];                                                    Mat_Rk_C_NRk[6][5] = 0.0;                                       Mat_Rk_C_NRk[6][6] = Mat_Rk_C_NRk[4][4];                        Mat_Rk_C_NRk[6][7] = Mat_Rk_C_NRk[4][5];
    Mat_Rk_C_NRk[7][0] = 0.0;                                                           Mat_Rk_C_NRk[7][1] = Mat_Rk_C_NRk[0][6];                        Mat_Rk_C_NRk[7][2] = Mat_Rk_C_NRk[0][5];                        Mat_Rk_C_NRk[7][3] = Mat_Rk_C_NRk[0][4];     Mat_Rk_C_NRk[7][4] = 0.0;                                                                  Mat_Rk_C_NRk[7][5] = Mat_Rk_C_NRk[4][6];                        Mat_Rk_C_NRk[7][6] = Mat_Rk_C_NRk[4][5];                        Mat_Rk_C_NRk[7][7] = Mat_Rk_C_NRk[4][4];

    // RkCNR
    complex<double> Mat_Rk_C_NR[8][8]{0}; // Y - ��������� Rk_C_NR - ������

    double Z_x_Rk_C_NR = m_R * 2.0 * (1.0 + m_N);
    double Z_y_Rk_C_NR = m_R * 2.0 * (1.0 + m_N);

    complex<double> p_Rk_C_NR = complex<double>(0.0, w);
    complex<double> temp_Rk_C_NR = 1.0 / m_H + p_Rk_C_NRk * m_R * m_C;
    complex<double> Y_Rk_C_NR = temp_Rk_C_NR / ((1.0 + m_G * temp_Rk_C_NR) * ((double)(4 * m_x * m_y)) * m_R);

    complex<double> tetta_x_Rk_C_NR = sqrt(Z_x_Rk_C_NR * Y_Rk_C_NR);
    complex<double> tetta_y_Rk_C_NR = sqrt(Z_y_Rk_C_NR * Y_Rk_C_NR);

    double koeff_x_Rk_C_NR = 1.0 / Z_x_Rk_C_NR;
    double koeff_y_Rk_C_NR = 1.0 / Z_y_Rk_C_NR;

    complex<double> tx_Rk_C_NR = tetta_x_Rk_C_NR / tanh(tetta_x_Rk_C_NR);
    complex<double> sx_Rk_C_NR = tetta_x_Rk_C_NR / sinh(tetta_x_Rk_C_NR);
    complex<double> ty_Rk_C_NR = tetta_y_Rk_C_NR / tanh(tetta_y_Rk_C_NR);
    complex<double> sy_Rk_C_NR = tetta_y_Rk_C_NR / sinh(tetta_y_Rk_C_NR);

    tx_Rk_C_NR *= koeff_x_Rk_C_NR;
    sx_Rk_C_NR *= koeff_x_Rk_C_NR;
    ty_Rk_C_NR *= koeff_y_Rk_C_NR;
    sy_Rk_C_NR *= koeff_y_Rk_C_NR;

    double KxN_Rk_C_NR = koeff_x_Rk_C_NR * m_N;
    double KyN_Rk_C_NR = koeff_y_Rk_C_NR * m_N;
    double Kx_N_Rk_C_NR = koeff_x_Rk_C_NR / m_N;
    double Ky_N_Rk_C_NR = koeff_y_Rk_C_NR / m_N;

    Mat_Rk_C_NR[0][0] = tx_Rk_C_NR + ty_Rk_C_NR + KxN_Rk_C_NR + KyN_Rk_C_NR;    Mat_Rk_C_NR[0][1] = -sx_Rk_C_NR - KxN_Rk_C_NR;              Mat_Rk_C_NR[0][2] = -sy_Rk_C_NR - KyN_Rk_C_NR;              Mat_Rk_C_NR[0][3] = 0.0;                   Mat_Rk_C_NR[0][4] = koeff_x_Rk_C_NR + koeff_y_Rk_C_NR - tx_Rk_C_NR - ty_Rk_C_NR;       Mat_Rk_C_NR[0][5] = sx_Rk_C_NR - koeff_x_Rk_C_NR;            Mat_Rk_C_NR[0][6] = sy_Rk_C_NR - koeff_y_Rk_C_NR;            Mat_Rk_C_NR[0][7] = 0.0;
    Mat_Rk_C_NR[1][0] = Mat_Rk_C_NR[0][1];                                      Mat_Rk_C_NR[1][1] = Mat_Rk_C_NR[0][0];                      Mat_Rk_C_NR[1][2] = 0.0;                                    Mat_Rk_C_NR[1][3] = Mat_Rk_C_NR[0][2];    Mat_Rk_C_NR[1][4] = Mat_Rk_C_NR[0][5];                                                    Mat_Rk_C_NR[1][5] = Mat_Rk_C_NR[0][4];                        Mat_Rk_C_NR[1][6] = 0.0;                                       Mat_Rk_C_NR[1][7] = Mat_Rk_C_NR[0][6];
    Mat_Rk_C_NR[2][0] = Mat_Rk_C_NR[0][2];                                      Mat_Rk_C_NR[2][1] = 0.0;                                    Mat_Rk_C_NR[2][2] = Mat_Rk_C_NR[0][0];                      Mat_Rk_C_NR[2][3] = Mat_Rk_C_NR[0][1];    Mat_Rk_C_NR[2][4] = Mat_Rk_C_NR[0][6];                                                    Mat_Rk_C_NR[2][5] = 0.0;                                       Mat_Rk_C_NR[2][6] = Mat_Rk_C_NR[0][4];                        Mat_Rk_C_NR[2][7] = Mat_Rk_C_NR[0][5];
    Mat_Rk_C_NR[3][0] = 0.0;                                                    Mat_Rk_C_NR[3][1] = Mat_Rk_C_NR[0][2];                      Mat_Rk_C_NR[3][2] = Mat_Rk_C_NR[0][1];                      Mat_Rk_C_NR[3][3] = Mat_Rk_C_NR[0][0];    Mat_Rk_C_NR[3][4] = 0.0;                                                                   Mat_Rk_C_NR[3][5] = Mat_Rk_C_NR[0][6];                        Mat_Rk_C_NR[3][6] = Mat_Rk_C_NR[0][5];                        Mat_Rk_C_NR[3][7] = Mat_Rk_C_NR[0][4];
    Mat_Rk_C_NR[4][0] = Mat_Rk_C_NR[0][4];                                      Mat_Rk_C_NR[4][1] = Mat_Rk_C_NR[0][5];                      Mat_Rk_C_NR[4][2] = Mat_Rk_C_NR[0][6];                      Mat_Rk_C_NR[4][3] = 0.0;                   Mat_Rk_C_NR[4][4] = tx_Rk_C_NR + ty_Rk_C_NR + Kx_N_Rk_C_NR + Ky_N_Rk_C_NR;             Mat_Rk_C_NR[4][5] = -sx_Rk_C_NR - Kx_N_Rk_C_NR;              Mat_Rk_C_NR[4][6] = -sy_Rk_C_NR - Ky_N_Rk_C_NR;              Mat_Rk_C_NR[4][7] = 0.0;
    Mat_Rk_C_NR[5][0] = Mat_Rk_C_NR[0][5];                                      Mat_Rk_C_NR[5][1] = Mat_Rk_C_NR[0][4];                      Mat_Rk_C_NR[5][2] = 0.0;                                    Mat_Rk_C_NR[5][3] = Mat_Rk_C_NR[0][6];    Mat_Rk_C_NR[5][4] = Mat_Rk_C_NR[4][5];                                                    Mat_Rk_C_NR[5][5] = Mat_Rk_C_NR[4][4];                        Mat_Rk_C_NR[5][6] = 0.0;                                       Mat_Rk_C_NR[5][7] = Mat_Rk_C_NR[4][6];
    Mat_Rk_C_NR[6][0] = Mat_Rk_C_NR[0][6];                                      Mat_Rk_C_NR[6][1] = 0.0;                                    Mat_Rk_C_NR[6][2] = Mat_Rk_C_NR[0][4];                      Mat_Rk_C_NR[6][3] = Mat_Rk_C_NR[0][5];    Mat_Rk_C_NR[6][4] = Mat_Rk_C_NR[4][6];                                                    Mat_Rk_C_NR[6][5] = 0.0;                                       Mat_Rk_C_NR[6][6] = Mat_Rk_C_NR[4][4];                        Mat_Rk_C_NR[6][7] = Mat_Rk_C_NR[4][5];
    Mat_Rk_C_NR[7][0] = 0.0;                                                    Mat_Rk_C_NR[7][1] = Mat_Rk_C_NR[0][6];                      Mat_Rk_C_NR[7][2] = Mat_Rk_C_NR[0][5];                      Mat_Rk_C_NR[7][3] = Mat_Rk_C_NR[0][4];     Mat_Rk_C_NR[7][4] = 0.0;                                                                  Mat_Rk_C_NR[7][5] = Mat_Rk_C_NR[4][6];                        Mat_Rk_C_NR[7][6] = Mat_Rk_C_NR[4][5];                        Mat_Rk_C_NR[7][7] = Mat_Rk_C_NR[4][4];

    // RCNRk
    complex<double> Mat_R_C_NRk[8][8]{0}; // Y - ��������� R_C_NRk - ������

    double Z_x_R_C_NRk = m_R * 2.0 * (1.0 + m_N);
    double Z_y_R_C_NRk = m_R * 2.0 * (1.0 + m_N);

    complex<double> p_R_C_NRk = complex<double>(0.0, w);
    complex<double> temp_R_C_NRk = 1.0 / m_H + p_R_C_NRk * m_R * m_C;
    complex<double> Y_R_C_NRk = temp_R_C_NRk / ((1.0 + m_G * temp_R_C_NRk) * ((double)(4 * m_x * m_y)) * m_R);

    complex<double> tetta_x_R_C_NRk = sqrt(Z_x_R_C_NRk * Y_R_C_NRk);
    complex<double> tetta_y_R_C_NRk = sqrt(Z_y_R_C_NRk * Y_R_C_NRk);

    double koeff_x_R_C_NRk = 1.0 / Z_x_R_C_NRk;
    double koeff_y_R_C_NRk = 1.0 / Z_y_R_C_NRk;

    complex<double> tx_R_C_NRk = tetta_x_R_C_NRk / tanh(tetta_x_R_C_NRk);
    complex<double> sx_R_C_NRk = tetta_x_R_C_NRk / sinh(tetta_x_R_C_NRk);
    complex<double> ty_R_C_NRk = tetta_y_R_C_NRk / tanh(tetta_y_R_C_NRk);
    complex<double> sy_R_C_NRk = tetta_y_R_C_NRk / sinh(tetta_y_R_C_NRk);

    tx_R_C_NRk *= koeff_x_R_C_NRk;
    sx_R_C_NRk *= koeff_x_R_C_NRk;
    ty_R_C_NRk *= koeff_y_R_C_NRk;
    sy_R_C_NRk *= koeff_y_R_C_NRk;

    double KxN_R_C_NRk = koeff_x_R_C_NRk * m_N;
    double KyN_R_C_NRk = koeff_y_R_C_NRk * m_N;
    double Kx_N_R_C_NRk = koeff_x_R_C_NRk / m_N;
    double Ky_N_R_C_NRk = koeff_y_R_C_NRk / m_N;

    Mat_R_C_NRk[0][0] = tx_R_C_NRk + ty_R_C_NRk + KxN_R_C_NRk + KyN_R_C_NRk;        Mat_R_C_NRk[0][1] = -sx_R_C_NRk - KxN_R_C_NRk;              Mat_R_C_NRk[0][2] = -sy_R_C_NRk - KyN_R_C_NRk;              Mat_R_C_NRk[0][3] = 0.0;                Mat_R_C_NRk[0][4] = koeff_x_R_C_NRk + koeff_y_R_C_NRk - tx_R_C_NRk - ty_R_C_NRk;    Mat_R_C_NRk[0][5] = sx_R_C_NRk - koeff_x_R_C_NRk;           Mat_R_C_NRk[0][6] = sy_R_C_NRk - koeff_y_R_C_NRk;           Mat_R_C_NRk[0][7] = 0.0;
    Mat_R_C_NRk[1][0] = Mat_R_C_NRk[0][1];                                          Mat_R_C_NRk[1][1] = Mat_R_C_NRk[0][0];                      Mat_R_C_NRk[1][2] = 0.0;                                    Mat_R_C_NRk[1][3] = Mat_R_C_NRk[0][2];  Mat_R_C_NRk[1][4] = Mat_R_C_NRk[0][5];                                              Mat_R_C_NRk[1][5] = Mat_R_C_NRk[0][4];                      Mat_R_C_NRk[1][6] = 0.0;                                    Mat_R_C_NRk[1][7] = Mat_R_C_NRk[0][6];
    Mat_R_C_NRk[2][0] = Mat_R_C_NRk[0][2];                                          Mat_R_C_NRk[2][1] = 0.0;                                    Mat_R_C_NRk[2][2] = Mat_R_C_NRk[0][0];                      Mat_R_C_NRk[2][3] = Mat_R_C_NRk[0][1];  Mat_R_C_NRk[2][4] = Mat_R_C_NRk[0][6];                                              Mat_R_C_NRk[2][5] = 0.0;                                    Mat_R_C_NRk[2][6] = Mat_R_C_NRk[0][4];                      Mat_R_C_NRk[2][7] = Mat_R_C_NRk[0][5];
    Mat_R_C_NRk[3][0] = 0.0;                                                        Mat_R_C_NRk[3][1] = Mat_R_C_NRk[0][2];                      Mat_R_C_NRk[3][2] = Mat_R_C_NRk[0][1];                      Mat_R_C_NRk[3][3] = Mat_R_C_NRk[0][0];  Mat_R_C_NRk[3][4] = 0.0;                                                            Mat_R_C_NRk[3][5] = Mat_R_C_NRk[0][6];                      Mat_R_C_NRk[3][6] = Mat_R_C_NRk[0][5];                      Mat_R_C_NRk[3][7] = Mat_R_C_NRk[0][4];
    Mat_R_C_NRk[4][0] = Mat_R_C_NRk[0][4];                                          Mat_R_C_NRk[4][1] = Mat_R_C_NRk[0][5];                      Mat_R_C_NRk[4][2] = Mat_R_C_NRk[0][6];                      Mat_R_C_NRk[4][3] = 0.0;                Mat_R_C_NRk[4][4] = tx_R_C_NRk + ty_R_C_NRk + Kx_N_R_C_NRk + Ky_N_R_C_NRk;          Mat_R_C_NRk[4][5] = -sx_R_C_NRk - Kx_N_R_C_NRk;             Mat_R_C_NRk[4][6] = -sy_R_C_NRk - Ky_N_R_C_NRk;             Mat_R_C_NRk[4][7] = 0.0;
    Mat_R_C_NRk[5][0] = Mat_R_C_NRk[0][5];                                          Mat_R_C_NRk[5][1] = Mat_R_C_NRk[0][4];                      Mat_R_C_NRk[5][2] = 0.0;                                    Mat_R_C_NRk[5][3] = Mat_R_C_NRk[0][6];  Mat_R_C_NRk[5][4] = Mat_R_C_NRk[4][5];                                              Mat_R_C_NRk[5][5] = Mat_R_C_NRk[4][4];                      Mat_R_C_NRk[5][6] = 0.0;                                    Mat_R_C_NRk[5][7] = Mat_R_C_NRk[4][6];
    Mat_R_C_NRk[6][0] = Mat_R_C_NRk[0][6];                                          Mat_R_C_NRk[6][1] = 0.0;                                    Mat_R_C_NRk[6][2] = Mat_R_C_NRk[0][4];                      Mat_R_C_NRk[6][3] = Mat_R_C_NRk[0][5];  Mat_R_C_NRk[6][4] = Mat_R_C_NRk[4][6];                                              Mat_R_C_NRk[6][5] = 0.0;                                    Mat_R_C_NRk[6][6] = Mat_R_C_NRk[4][4];                      Mat_R_C_NRk[6][7] = Mat_R_C_NRk[4][5];
    Mat_R_C_NRk[7][0] = 0.0;

    // RCNR
    complex<double> Mat_rcnr[8][8]; // Y - ��������� RCNR - ������

    //double Z_x = m_R*2.0*(1.0+m_N)*(double)m_y/((double)m_x);
    //double Z_y = m_R*2.0*(1.0+m_N)*(double)m_x/((double)m_y);
    double Z_x = m_R*2.0*(1.0+m_N);
    double Z_y = m_R*2.0*(1.0+m_N);

    complex<double> p = complex<double>(0.0, w);
    complex<double> temp = 1.0/m_H + p*m_R*m_C;
    complex<double> Y = temp/((1.0+m_G*temp)*((double)(4*m_x*m_y))*m_R);
    //complex<double> temp = 1.0/m_H + p*Z_x*m_C;
    //complex<double> Y = temp/((1.0+m_G*temp)*((double)(4*m_x*m_y))*Z_x); // �� ���� ��� �����������, �.�. G � H ����������� � R, � �� � R*2*(N+1)

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


    // ��� 2x2 �� ������� ���� 0dB
    //Mat_rcnr[0][0]=tx+ty+KxN+KyN;  Mat_rcnr[0][1]=-sx-KxN;        Mat_rcnr[0][2]=-sy-KyN;        Mat_rcnr[0][3]=0.0;            Mat_rcnr[0][4]=sx+sy-koeff_x-koeff_y; Mat_rcnr[0][5]=koeff_x-tx;     Mat_rcnr[0][6]=koeff_y-ty;     Mat_rcnr[0][7]=0.0;

    // ��� 2x2 ������� ���� 0dB
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
            case 1:// Rv
            {
                auto Element = Mat_r;

                int u1 = m_pCircuitNodes[0][i][j];
                int u2 = m_pCircuitNodes[0][i + 1][j];
                int u3 = m_pCircuitNodes[0][i][j + 1];
                int u4 = m_pCircuitNodes[0][i + 1][j + 1];
                int u5 = m_pCircuitNodes[1][i][j];
                int u6 = m_pCircuitNodes[1][i + 1][j];
                int u7 = m_pCircuitNodes[1][i][j + 1];
                int u8 = m_pCircuitNodes[1][i + 1][j + 1];

                /* ������� */
                if (u1 < 0 || u2 < 0 || u3 < 0 || u4 < 0 || u5 < 0 || u6 < 0 || u7 < 0 || u8 < 0)
                {
                    int t = 25;
                }
                /* ������� */

                AddToMatrix(Ym, u1, u1, Element[0][0], false);
                AddToMatrix(Ym, u1, u2, Element[0][1], true);
                AddToMatrix(Ym, u1, u3, Element[0][2], true);
                AddToMatrix(Ym, u1, u4, Element[0][3], true);
                AddToMatrix(Ym, u1, u5, Element[0][4], true);
                AddToMatrix(Ym, u1, u6, Element[0][5], true);
                AddToMatrix(Ym, u1, u7, Element[0][6], true);
                AddToMatrix(Ym, u1, u8, Element[0][7], true);

                AddToMatrix(Ym, u2, u2, Element[1][1], false);
                AddToMatrix(Ym, u2, u3, Element[1][2], true);
                AddToMatrix(Ym, u2, u4, Element[1][3], true);
                AddToMatrix(Ym, u2, u5, Element[1][4], true);
                AddToMatrix(Ym, u2, u6, Element[1][5], true);
                AddToMatrix(Ym, u2, u7, Element[1][6], true);
                AddToMatrix(Ym, u2, u8, Element[1][7], true);

                AddToMatrix(Ym, u3, u3, Element[2][2], false);
                AddToMatrix(Ym, u3, u4, Element[2][3], true);
                AddToMatrix(Ym, u3, u5, Element[2][4], true);
                AddToMatrix(Ym, u3, u6, Element[2][5], true);
                AddToMatrix(Ym, u3, u7, Element[2][6], true);
                AddToMatrix(Ym, u3, u8, Element[2][7], true);

                AddToMatrix(Ym, u4, u4, Element[3][3], false);
                AddToMatrix(Ym, u4, u5, Element[3][4], true);
                AddToMatrix(Ym, u4, u6, Element[3][5], true);
                AddToMatrix(Ym, u4, u7, Element[3][6], true);
                AddToMatrix(Ym, u4, u8, Element[3][7], true);

                AddToMatrix(Ym, u5, u5, Element[4][4], false);
                AddToMatrix(Ym, u5, u6, Element[4][5], true);
                AddToMatrix(Ym, u5, u7, Element[4][6], true);
                AddToMatrix(Ym, u5, u8, Element[4][7], true);

                AddToMatrix(Ym, u6, u6, Element[5][5], false);
                AddToMatrix(Ym, u6, u7, Element[5][6], true);
                AddToMatrix(Ym, u6, u8, Element[5][7], true);

                AddToMatrix(Ym, u7, u7, Element[6][6], false);
                AddToMatrix(Ym, u7, u8, Element[6][7], true);

                AddToMatrix(Ym, u8, u8, Element[7][7], false);
            }
            break;
            case 4:// Rn
            {
                auto Element = Mat_nr;

                int u1 = m_pCircuitNodes[0][i][j];
                int u2 = m_pCircuitNodes[0][i + 1][j];
                int u3 = m_pCircuitNodes[0][i][j + 1];
                int u4 = m_pCircuitNodes[0][i + 1][j + 1];
                int u5 = m_pCircuitNodes[1][i][j];
                int u6 = m_pCircuitNodes[1][i + 1][j];
                int u7 = m_pCircuitNodes[1][i][j + 1];
                int u8 = m_pCircuitNodes[1][i + 1][j + 1];

                /* ������� */
                if (u1 < 0 || u2 < 0 || u3 < 0 || u4 < 0 || u5 < 0 || u6 < 0 || u7 < 0 || u8 < 0 )
                {
                    int t = 25;
                }
                /* ������� */

                AddToMatrix(Ym, u1, u1, Element[0][0], false);
                AddToMatrix(Ym, u1, u2, Element[0][1], true);
                AddToMatrix(Ym, u1, u3, Element[0][2], true);
                AddToMatrix(Ym, u1, u4, Element[0][3], true);
                AddToMatrix(Ym, u1, u5, Element[0][4], true);
                AddToMatrix(Ym, u1, u6, Element[0][5], true);
                AddToMatrix(Ym, u1, u7, Element[0][6], true);
                AddToMatrix(Ym, u1, u8, Element[0][7], true);

                AddToMatrix(Ym, u2, u2, Element[1][1], false);
                AddToMatrix(Ym, u2, u3, Element[1][2], true);
                AddToMatrix(Ym, u2, u4, Element[1][3], true);
                AddToMatrix(Ym, u2, u5, Element[1][4], true);
                AddToMatrix(Ym, u2, u6, Element[1][5], true);
                AddToMatrix(Ym, u2, u7, Element[1][6], true);
                AddToMatrix(Ym, u2, u8, Element[1][7], true);

                AddToMatrix(Ym, u3, u3, Element[2][2], false);
                AddToMatrix(Ym, u3, u4, Element[2][3], true);
                AddToMatrix(Ym, u3, u5, Element[2][4], true);
                AddToMatrix(Ym, u3, u6, Element[2][5], true);
                AddToMatrix(Ym, u3, u7, Element[2][6], true);
                AddToMatrix(Ym, u3, u8, Element[2][7], true);

                AddToMatrix(Ym, u4, u4, Element[3][3], false);
                AddToMatrix(Ym, u4, u5, Element[3][4], true);
                AddToMatrix(Ym, u4, u6, Element[3][5], true);
                AddToMatrix(Ym, u4, u7, Element[3][6], true);
                AddToMatrix(Ym, u4, u8, Element[3][7], true);

                AddToMatrix(Ym, u5, u5, Element[4][4], false);
                AddToMatrix(Ym, u5, u6, Element[4][5], true);
                AddToMatrix(Ym, u5, u7, Element[4][6], true);
                AddToMatrix(Ym, u5, u8, Element[4][7], true);

                AddToMatrix(Ym, u6, u6, Element[5][5], false);
                AddToMatrix(Ym, u6, u7, Element[5][6], true);
                AddToMatrix(Ym, u6, u8, Element[5][7], true);

                AddToMatrix(Ym, u7, u7, Element[6][6], false);
                AddToMatrix(Ym, u7, u8, Element[6][7], true);

                AddToMatrix(Ym, u8, u8, Element[7][7], false);
            }
            break;
            case 7:// R C NR
            {
                // � ���������� ������� Ym �������� ������ ������� ����������� ������������ �� ���������

                auto Element = Mat_rcnr;

                int u1 = m_pCircuitNodes[0][i][j];
                int u2 = m_pCircuitNodes[0][i + 1][j];
                int u3 = m_pCircuitNodes[0][i][j + 1];
                int u4 = m_pCircuitNodes[0][i + 1][j + 1];
                int u5 = m_pCircuitNodes[1][i][j];
                int u6 = m_pCircuitNodes[1][i + 1][j];
                int u7 = m_pCircuitNodes[1][i][j + 1];
                int u8 = m_pCircuitNodes[1][i + 1][j + 1];

                /* ������� */
                if (u1 < 0 || u2 < 0 || u3 < 0 || u4 < 0 || u5 < 0 || u6 < 0 || u7 < 0 || u8 < 0)
                {
                    int t = 25;
                }
                /* ������� */

                AddToMatrix(Ym, u1, u1, Element[0][0], false);
                AddToMatrix(Ym, u1, u2, Element[0][1], true);
                AddToMatrix(Ym, u1, u3, Element[0][2], true);
                AddToMatrix(Ym, u1, u4, Element[0][3], true);
                AddToMatrix(Ym, u1, u5, Element[0][4], true);
                AddToMatrix(Ym, u1, u6, Element[0][5], true);
                AddToMatrix(Ym, u1, u7, Element[0][6], true);
                AddToMatrix(Ym, u1, u8, Element[0][7], true);

                AddToMatrix(Ym, u2, u2, Element[1][1], false);
                AddToMatrix(Ym, u2, u3, Element[1][2], true);
                AddToMatrix(Ym, u2, u4, Element[1][3], true);
                AddToMatrix(Ym, u2, u5, Element[1][4], true);
                AddToMatrix(Ym, u2, u6, Element[1][5], true);
                AddToMatrix(Ym, u2, u7, Element[1][6], true);
                AddToMatrix(Ym, u2, u8, Element[1][7], true);

                AddToMatrix(Ym, u3, u3, Element[2][2], false);
                AddToMatrix(Ym, u3, u4, Element[2][3], true);
                AddToMatrix(Ym, u3, u5, Element[2][4], true);
                AddToMatrix(Ym, u3, u6, Element[2][5], true);
                AddToMatrix(Ym, u3, u7, Element[2][6], true);
                AddToMatrix(Ym, u3, u8, Element[2][7], true);

                AddToMatrix(Ym, u4, u4, Element[3][3], false);
                AddToMatrix(Ym, u4, u5, Element[3][4], true);
                AddToMatrix(Ym, u4, u6, Element[3][5], true);
                AddToMatrix(Ym, u4, u7, Element[3][6], true);
                AddToMatrix(Ym, u4, u8, Element[3][7], true);

                AddToMatrix(Ym, u5, u5, Element[4][4], false);
                AddToMatrix(Ym, u5, u6, Element[4][5], true);
                AddToMatrix(Ym, u5, u7, Element[4][6], true);
                AddToMatrix(Ym, u5, u8, Element[4][7], true);

                AddToMatrix(Ym, u6, u6, Element[5][5], false);
                AddToMatrix(Ym, u6, u7, Element[5][6], true);
                AddToMatrix(Ym, u6, u8, Element[5][7], true);

                AddToMatrix(Ym, u7, u7, Element[6][6], false);
                AddToMatrix(Ym, u7, u8, Element[6][7], true);

                AddToMatrix(Ym, u8, u8, Element[7][7], false);
            }
            break;
            case 8:// Rk C NR
            {
                auto Element = Mat_Rk_C_NR;

                int u1 = m_pCircuitNodes[0][i][j];
                int u2 = m_pCircuitNodes[0][i + 1][j];
                int u3 = m_pCircuitNodes[0][i][j + 1];
                int u4 = m_pCircuitNodes[0][i + 1][j + 1];
                int u5 = m_pCircuitNodes[1][i][j];
                int u6 = m_pCircuitNodes[1][i + 1][j];
                int u7 = m_pCircuitNodes[1][i][j + 1];
                int u8 = m_pCircuitNodes[1][i + 1][j + 1];

                /* ������� */
                if (u1 < 0 || u2 < 0 || u3 < 0 || u4 < 0 || u5 < 0 || u6 < 0 || u7 < 0 || u8 < 0)
                {
                    int t = 25;
                }
                /* ������� */

                AddToMatrix(Ym, u1, u1, Element[0][0], false);
                AddToMatrix(Ym, u1, u2, Element[0][1], true);
                AddToMatrix(Ym, u1, u3, Element[0][2], true);
                AddToMatrix(Ym, u1, u4, Element[0][3], true);
                AddToMatrix(Ym, u1, u5, Element[0][4], true);
                AddToMatrix(Ym, u1, u6, Element[0][5], true);
                AddToMatrix(Ym, u1, u7, Element[0][6], true);
                AddToMatrix(Ym, u1, u8, Element[0][7], true);

                AddToMatrix(Ym, u2, u2, Element[1][1], false);
                AddToMatrix(Ym, u2, u3, Element[1][2], true);
                AddToMatrix(Ym, u2, u4, Element[1][3], true);
                AddToMatrix(Ym, u2, u5, Element[1][4], true);
                AddToMatrix(Ym, u2, u6, Element[1][5], true);
                AddToMatrix(Ym, u2, u7, Element[1][6], true);
                AddToMatrix(Ym, u2, u8, Element[1][7], true);

                AddToMatrix(Ym, u3, u3, Element[2][2], false);
                AddToMatrix(Ym, u3, u4, Element[2][3], true);
                AddToMatrix(Ym, u3, u5, Element[2][4], true);
                AddToMatrix(Ym, u3, u6, Element[2][5], true);
                AddToMatrix(Ym, u3, u7, Element[2][6], true);
                AddToMatrix(Ym, u3, u8, Element[2][7], true);

                AddToMatrix(Ym, u4, u4, Element[3][3], false);
                AddToMatrix(Ym, u4, u5, Element[3][4], true);
                AddToMatrix(Ym, u4, u6, Element[3][5], true);
                AddToMatrix(Ym, u4, u7, Element[3][6], true);
                AddToMatrix(Ym, u4, u8, Element[3][7], true);

                AddToMatrix(Ym, u5, u5, Element[4][4], false);
                AddToMatrix(Ym, u5, u6, Element[4][5], true);
                AddToMatrix(Ym, u5, u7, Element[4][6], true);
                AddToMatrix(Ym, u5, u8, Element[4][7], true);

                AddToMatrix(Ym, u6, u6, Element[5][5], false);
                AddToMatrix(Ym, u6, u7, Element[5][6], true);
                AddToMatrix(Ym, u6, u8, Element[5][7], true);

                AddToMatrix(Ym, u7, u7, Element[6][6], false);
                AddToMatrix(Ym, u7, u8, Element[6][7], true);

                AddToMatrix(Ym, u8, u8, Element[7][7], false);
            }
            break;
            case 12:// R C NRk
            {
                auto Element = Mat_R_C_NRk;

                int u1 = m_pCircuitNodes[0][i][j];
                int u2 = m_pCircuitNodes[0][i + 1][j];
                int u3 = m_pCircuitNodes[0][i][j + 1];
                int u4 = m_pCircuitNodes[0][i + 1][j + 1];
                int u5 = m_pCircuitNodes[1][i][j];
                int u6 = m_pCircuitNodes[1][i + 1][j];
                int u7 = m_pCircuitNodes[1][i][j + 1];
                int u8 = m_pCircuitNodes[1][i + 1][j + 1];

                /* ������� */
                if (u1 < 0 || u2 < 0 || u3 < 0 || u4 < 0 || u5 < 0 || u6 < 0 || u7 < 0 || u8 < 0)
                {
                    int t = 25;
                }
                /* ������� */

                AddToMatrix(Ym, u1, u1, Element[0][0], false);
                AddToMatrix(Ym, u1, u2, Element[0][1], true);
                AddToMatrix(Ym, u1, u3, Element[0][2], true);
                AddToMatrix(Ym, u1, u4, Element[0][3], true);
                AddToMatrix(Ym, u1, u5, Element[0][4], true);
                AddToMatrix(Ym, u1, u6, Element[0][5], true);
                AddToMatrix(Ym, u1, u7, Element[0][6], true);
                AddToMatrix(Ym, u1, u8, Element[0][7], true);

                AddToMatrix(Ym, u2, u2, Element[1][1], false);
                AddToMatrix(Ym, u2, u3, Element[1][2], true);
                AddToMatrix(Ym, u2, u4, Element[1][3], true);
                AddToMatrix(Ym, u2, u5, Element[1][4], true);
                AddToMatrix(Ym, u2, u6, Element[1][5], true);
                AddToMatrix(Ym, u2, u7, Element[1][6], true);
                AddToMatrix(Ym, u2, u8, Element[1][7], true);

                AddToMatrix(Ym, u3, u3, Element[2][2], false);
                AddToMatrix(Ym, u3, u4, Element[2][3], true);
                AddToMatrix(Ym, u3, u5, Element[2][4], true);
                AddToMatrix(Ym, u3, u6, Element[2][5], true);
                AddToMatrix(Ym, u3, u7, Element[2][6], true);
                AddToMatrix(Ym, u3, u8, Element[2][7], true);

                AddToMatrix(Ym, u4, u4, Element[3][3], false);
                AddToMatrix(Ym, u4, u5, Element[3][4], true);
                AddToMatrix(Ym, u4, u6, Element[3][5], true);
                AddToMatrix(Ym, u4, u7, Element[3][6], true);
                AddToMatrix(Ym, u4, u8, Element[3][7], true);

                AddToMatrix(Ym, u5, u5, Element[4][4], false);
                AddToMatrix(Ym, u5, u6, Element[4][5], true);
                AddToMatrix(Ym, u5, u7, Element[4][6], true);
                AddToMatrix(Ym, u5, u8, Element[4][7], true);

                AddToMatrix(Ym, u6, u6, Element[5][5], false);
                AddToMatrix(Ym, u6, u7, Element[5][6], true);
                AddToMatrix(Ym, u6, u8, Element[5][7], true);

                AddToMatrix(Ym, u7, u7, Element[6][6], false);
                AddToMatrix(Ym, u7, u8, Element[6][7], true);

                AddToMatrix(Ym, u8, u8, Element[7][7], false);
            }
            break;
            case 14:// Rk C NRk
            {
                auto Element = Mat_Rk_C_NRk;

                int u1 = m_pCircuitNodes[0][i][j];
                int u2 = m_pCircuitNodes[0][i + 1][j];
                int u3 = m_pCircuitNodes[0][i][j + 1];
                int u4 = m_pCircuitNodes[0][i + 1][j + 1];
                int u5 = m_pCircuitNodes[1][i][j];
                int u6 = m_pCircuitNodes[1][i + 1][j];
                int u7 = m_pCircuitNodes[1][i][j + 1];
                int u8 = m_pCircuitNodes[1][i + 1][j + 1];

                /* ������� */
                if (u1 < 0 || u2 < 0 || u3 < 0 || u4 < 0 || u5 < 0 || u6 < 0 || u7 < 0 || u8 < 0)
                {
                    int t = 25;
                }
                /* ������� */

                AddToMatrix(Ym, u1, u1, Element[0][0], false);
                AddToMatrix(Ym, u1, u2, Element[0][1], true);
                AddToMatrix(Ym, u1, u3, Element[0][2], true);
                AddToMatrix(Ym, u1, u4, Element[0][3], true);
                AddToMatrix(Ym, u1, u5, Element[0][4], true);
                AddToMatrix(Ym, u1, u6, Element[0][5], true);
                AddToMatrix(Ym, u1, u7, Element[0][6], true);
                AddToMatrix(Ym, u1, u8, Element[0][7], true);

                AddToMatrix(Ym, u2, u2, Element[1][1], false);
                AddToMatrix(Ym, u2, u3, Element[1][2], true);
                AddToMatrix(Ym, u2, u4, Element[1][3], true);
                AddToMatrix(Ym, u2, u5, Element[1][4], true);
                AddToMatrix(Ym, u2, u6, Element[1][5], true);
                AddToMatrix(Ym, u2, u7, Element[1][6], true);
                AddToMatrix(Ym, u2, u8, Element[1][7], true);

                AddToMatrix(Ym, u3, u3, Element[2][2], false);
                AddToMatrix(Ym, u3, u4, Element[2][3], true);
                AddToMatrix(Ym, u3, u5, Element[2][4], true);
                AddToMatrix(Ym, u3, u6, Element[2][5], true);
                AddToMatrix(Ym, u3, u7, Element[2][6], true);
                AddToMatrix(Ym, u3, u8, Element[2][7], true);

                AddToMatrix(Ym, u4, u4, Element[3][3], false);
                AddToMatrix(Ym, u4, u5, Element[3][4], true);
                AddToMatrix(Ym, u4, u6, Element[3][5], true);
                AddToMatrix(Ym, u4, u7, Element[3][6], true);
                AddToMatrix(Ym, u4, u8, Element[3][7], true);

                AddToMatrix(Ym, u5, u5, Element[4][4], false);
                AddToMatrix(Ym, u5, u6, Element[4][5], true);
                AddToMatrix(Ym, u5, u7, Element[4][6], true);
                AddToMatrix(Ym, u5, u8, Element[4][7], true);

                AddToMatrix(Ym, u6, u6, Element[5][5], false);
                AddToMatrix(Ym, u6, u7, Element[5][6], true);
                AddToMatrix(Ym, u6, u8, Element[5][7], true);

                AddToMatrix(Ym, u7, u7, Element[6][6], false);
                AddToMatrix(Ym, u7, u8, Element[6][7], true);

                AddToMatrix(Ym, u8, u8, Element[7][7], false);
            }
            break;
            default:
            {
                throw new exception();
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
  static const int fixtable[] = {0, 1, 0, 1, 4, 7, 4, 7}; // 0 ����� � ���� ����� // 1 ����� ����� // 4 ����� ������ // 7 ������ ��������� 
  return fixtable[Element&7];
}

