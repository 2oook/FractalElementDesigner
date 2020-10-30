//---------------------------------------------------------------------------
//#include <vcl.h>
#pragma hdrstop
#include "Scheme.h"
#include "mmsvdsolveunit.h"

#pragma package(smart_init)

string ConvertFloatToStr(float number) {
    ostringstream buff;
    buff << number;
    return buff.str();
}

/// <summary>
/// Метод для ????
/// </summary>
/// <param name="T">Объект CRCStructureCalculateData</param>
/// <param name="C">Тип характеристики</param>
/// <param name="R">Серия (вертикальная ось графика)</param>
/// <param name="Structure">Структура</param>
/// <returns></returns>
int CScheme::Setka(CRCStructureCalculateData *T, int C, double *R, CRCStructure *Structure)
{
  int ExitCode;
  switch (C)
  {
    case 0:
    {
      ExitCode = CharacteristicT(T, Structure);// T->m_T заполнен нулями до вызова
      if (!ExitCode)
      {
        for (int i=0; i<T->m_length; ++i)
        {
          if (fabs(T->m_T[i].real()) > 1e-37)
            R[i] = Magnitude(T->m_T[i]);
          else
            return -1;
        }
      }
    }
    break;

    case 1:
    {
      ExitCode = CharacteristicT(T, Structure);
      if (!ExitCode)
      {
        int adjustment = 0;
        for (int i=0; i<T->m_length; ++i)
        {
          double phase = Phase(T->m_T[i]);
          if (i>0)
          {
            double delta = phase + adjustment - R[i-1];
            if (delta > 340)
              adjustment -= 360;
            else if (delta < -340)
              adjustment += 360;
          }

          R[i] = phase + adjustment;
        }
      }
    }
    break;

    case 2:
    {
      ExitCode = CharacteristicZ(T, Structure);
      if (!ExitCode)
      {
        for (int i=0; i<T->m_length; ++i)
        {
          if (fabs(T->m_T[i].real()) > 1e-37)
            R[i] = Magnitude(T->m_T[i]);
          else
            return -1;
        }
      }
    }
    break;

    case 3:
    {
      ExitCode = CharacteristicZ(T, Structure); // ФЧХ входного импеданса
      if (!ExitCode)
      {
        int adjustment = 0;
        for (int i=0; i<T->m_length; ++i)
        {
          double phase = Phase(T->m_T[i]);
          if (i>0)
          {
            double delta = phase + adjustment - R[i-1];
            if (delta > 340)
              adjustment -= 360;
            else if (delta < -340)
              adjustment += 360;
          }

          R[i] = phase + adjustment;
        }
      }
    }
    break;

    default:
    {
      for (int i=0; i<T->m_length; ++i)
      {
        R[i] = 1.0;
      }
      ExitCode = -1;
    }
  }
  return ExitCode;
}

bool CScheme::Approximate(CRCStructureCalculateData *T, int m, int n)
{
  ap::real_2d_array X;
  X.setbounds(1,T->m_length,1,m+n+2);
  T->m_sol.setbounds(1,m+n+1);
  double temp;

  for (int i=0; i<T->m_length; ++i)
  {
    X(i+1,1) = 1.0;
    X(i+1,2) = T->m_w[i];
    for (int j=2; j<=m; ++j)
    {
      temp = exp(log(T->m_w[i])*j);
      if (j&2)
        temp = -temp;
      X(i+1,j+1) = temp;
    }
    for (int j=1; j<=n; ++j)
    {
      if (j&1)
        temp = -exp(log(T->m_w[i])*j)*(T->m_T[i].real() - T->m_T[i].imag());
      else
        temp = -exp(log(T->m_w[i])*j)*(T->m_T[i].real() + T->m_T[i].imag());
      if (j&2)
        temp = -temp;
      X(i+1,j+m+1) = temp;
    }
    X(i+1,m+n+2) = T->m_T[i].real() + T->m_T[i].imag();
  }

  bool res = svdsolve(X,T->m_length,m+n+1,T->m_sol);

  complex<double> apr_num,apr_den;

  for (int i=0; i<T->m_length; ++i)
  {
    apr_num = T->m_sol(m+1);
    for (int j=m; j>0; j--)
    {
      apr_num *= complex<double>(0.0,T->m_w[i]);
      apr_num += T->m_sol(j);
    }
    apr_den = 0.0;
    for (int j=m+n+1; j>m+1; j--)
    {
      apr_den += T->m_sol(j);
      apr_den *= complex<double>(0.0,T->m_w[i]);
    }
    apr_den += 1.0;
    T->m_Tapprox[i] = apr_num/apr_den;
  }

  return res;
}

void CScheme::SetkaApproximated(CRCStructureCalculateData *T, int C, double *R)
{
  switch (C)
  {
    case 0: case 2:
    {
      for (int i=0; i<T->m_length; ++i)
      {
        R[i] = Magnitude(T->m_Tapprox[i]);
      }
    }
    break;

    case 1: case 3:
    {
      for (int i=0; i<T->m_length; ++i)
      {
        R[i] = Phase(T->m_Tapprox[i]);
        if (i>0)
        {
          if (R[i] - R[i-1] < -350.0)
            R[i] += 360.0;
          else if (R[i] - R[i-1] > 350.0)
            R[i] -= 360.0;
        }
      }
    }
    break;

    default:
    {
      for (int i=0; i<T->m_length; ++i)
      {
        R[i] = 1.0;
      }
    }
    break;
  }
}

CScheme* CScheme::GetScheme(int index, double K, double L)
{
  CScheme *pSch;
  switch (index)
  {
    case  0: pSch = new CPassive01(); break;
    case  1: pSch = new CPassive02(); break;
    case  2: pSch = new CPassive03(); break;
    case  3: pSch = new CARCFilterLowPass01(K, L); break;
    case  4: pSch = new CPassive04(L); break;
    case  5: pSch = new CPassive05(); break;
    case  6: pSch = new CPassive06(); break;
    case  7: pSch = new CPassive07(); break;
    case  8: pSch = new CARCFilterLowPass02(K, L); break;
    case  9: pSch = new CARCFilterLowPass03(K, L); break;
    case 10: pSch = new CARCFilterHighPass01(K, L); break;
    case 11: pSch = new CARCFilterBandStop01(K, L); break;
    case 12: pSch = new CPassive08(); break;
    case 13: pSch = new CPassive09(); break;
    case 14: pSch = new CPassive10(); break;
    case 15: pSch = new CPassive11(); break;
    case 16: pSch = new CPassive12(); break;
    case 17: pSch = new CPassive13(); break;
    case 18: pSch = new CPassive14(); break;
    case 19: pSch = new CPassive15(); break;
    case 20: pSch = new CPassive16(); break;
    case 21: pSch = NULL; break; // формула
    default: pSch = NULL;
  }
  return pSch;
}

//void CScheme::ShowScheme(int index, TImage *ImageSch)
//{
//  PatBlt(ImageSch->Canvas->Handle, 0, 0, ImageSch->Width, ImageSch->Height, WHITENESS);
//  ImageSch->Canvas->Pen->Color=clBlack;
//  ImageSch->Canvas->Brush->Color=clWhite;
//  ImageSch->Canvas->Pen->Width=1;
//  ImageSch->Canvas->Rectangle(0, 0, ImageSch->Width, ImageSch->Height);
//
//  switch (index)
//  {
//    case 0:
//    {
//      ImageSch->Canvas->MoveTo(10,30); ImageSch->Canvas->LineTo(30,30);
//      ImageSch->Canvas->Rectangle(30,20,70,40);
//      ImageSch->Canvas->MoveTo(70,30); ImageSch->Canvas->LineTo(90,30);
//      ImageSch->Canvas->MoveTo(30,45); ImageSch->Canvas->LineTo(70,45);
//      ImageSch->Canvas->MoveTo(50,45); ImageSch->Canvas->LineTo(50,90);
//      ImageSch->Canvas->MoveTo(10,90); ImageSch->Canvas->LineTo(90,90);
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(10,90); ImageSch->Canvas->LineTo(10,90);
//      ImageSch->Canvas->MoveTo(50,90); ImageSch->Canvas->LineTo(50,90);
//      ImageSch->Canvas->MoveTo(90,90); ImageSch->Canvas->LineTo(90,90);
//      ImageSch->Canvas->MoveTo(10,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->MoveTo(90,30); ImageSch->Canvas->LineTo(90,30);
//      ImageSch->Canvas->TextOutA(17, 15, "1");
//      ImageSch->Canvas->TextOutA(77, 15, "2");
//    }
//    break;
//    case 1:
//    {
//      ImageSch->Canvas->MoveTo(10,50); ImageSch->Canvas->LineTo(35,50);
//      ImageSch->Canvas->MoveTo(35,30); ImageSch->Canvas->LineTo(35,70);
//      ImageSch->Canvas->Rectangle(40,30,60,70);
//      ImageSch->Canvas->MoveTo(50,30); ImageSch->Canvas->LineTo(50,10);
//      ImageSch->Canvas->MoveTo(50,10); ImageSch->Canvas->LineTo(90,10);
//      ImageSch->Canvas->MoveTo(50,70); ImageSch->Canvas->LineTo(50,90);
//      ImageSch->Canvas->MoveTo(10,90); ImageSch->Canvas->LineTo(90,90);
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(10,90); ImageSch->Canvas->LineTo(10,90);
//      ImageSch->Canvas->MoveTo(50,90); ImageSch->Canvas->LineTo(50,90);
//      ImageSch->Canvas->MoveTo(90,90); ImageSch->Canvas->LineTo(90,90);
//      ImageSch->Canvas->MoveTo(10,50); ImageSch->Canvas->LineTo(10,50);
//      ImageSch->Canvas->MoveTo(90,10); ImageSch->Canvas->LineTo(90,10);
//      ImageSch->Canvas->TextOutA(55, 15, "1");
//      ImageSch->Canvas->TextOutA(55, 71, "2");
//    }
//    break;
//    case 2:
//    {
//      ImageSch->Canvas->MoveTo(90,50); ImageSch->Canvas->LineTo(65,50);
//      ImageSch->Canvas->MoveTo(65,30); ImageSch->Canvas->LineTo(65,70);
//      ImageSch->Canvas->Rectangle(40,30,60,70);
//      ImageSch->Canvas->MoveTo(50,30); ImageSch->Canvas->LineTo(50,10);
//      ImageSch->Canvas->MoveTo(50,10); ImageSch->Canvas->LineTo(10,10);
//      ImageSch->Canvas->MoveTo(50,70); ImageSch->Canvas->LineTo(50,90);
//      ImageSch->Canvas->MoveTo(10,90); ImageSch->Canvas->LineTo(90,90);
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(10,90); ImageSch->Canvas->LineTo(10,90);
//      ImageSch->Canvas->MoveTo(50,90); ImageSch->Canvas->LineTo(50,90);
//      ImageSch->Canvas->MoveTo(90,90); ImageSch->Canvas->LineTo(90,90);
//      ImageSch->Canvas->MoveTo(10,10); ImageSch->Canvas->LineTo(10,10);
//      ImageSch->Canvas->MoveTo(90,50); ImageSch->Canvas->LineTo(90,50);
//      ImageSch->Canvas->TextOutA(40, 15, "1");
//      ImageSch->Canvas->TextOutA(40, 72, "2");
//    }
//    break;
//    case 3:
//    {
//      ImageSch->Canvas->MoveTo(55,55); ImageSch->Canvas->LineTo(55,25);
//      ImageSch->Canvas->LineTo(75,40);
//      ImageSch->Canvas->LineTo(55,55);
//      ImageSch->Canvas->Rectangle(20,35,40,45);
//      ImageSch->Canvas->MoveTo(40,40); ImageSch->Canvas->LineTo(55,40);
//      ImageSch->Canvas->MoveTo(20,30); ImageSch->Canvas->LineTo(40,30);
//      ImageSch->Canvas->MoveTo( 5,40); ImageSch->Canvas->LineTo(20,40);
//      ImageSch->Canvas->MoveTo(75,40); ImageSch->Canvas->LineTo(95,40);
//      ImageSch->Canvas->MoveTo( 5,75); ImageSch->Canvas->LineTo(95,75);
//      ImageSch->Canvas->MoveTo(65,48); ImageSch->Canvas->LineTo(65,83);
//      ImageSch->Canvas->MoveTo(60,83); ImageSch->Canvas->LineTo(70,83);
//      ImageSch->Canvas->MoveTo(30,30); ImageSch->Canvas->LineTo(30,15);
//      ImageSch->Canvas->LineTo(80,15);
//      ImageSch->Canvas->LineTo(80,40);
//      ImageSch->Canvas->MoveTo(45,40);
//      ImageSch->Canvas->LineTo(45,75);
//      ImageSch->Canvas->Rectangle(40,68,50,48);
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(45,40); ImageSch->Canvas->LineTo(45,40);
//      ImageSch->Canvas->MoveTo(45,75); ImageSch->Canvas->LineTo(45,75);
//      ImageSch->Canvas->MoveTo(80,40); ImageSch->Canvas->LineTo(80,40);
//      ImageSch->Canvas->MoveTo(65,75); ImageSch->Canvas->LineTo(65,75);
//      ImageSch->Canvas->MoveTo( 5,75); ImageSch->Canvas->LineTo( 5,75);
//      ImageSch->Canvas->MoveTo(95,75); ImageSch->Canvas->LineTo(95,75);
//      ImageSch->Canvas->MoveTo( 5,40); ImageSch->Canvas->LineTo( 5,40);
//      ImageSch->Canvas->MoveTo(95,40); ImageSch->Canvas->LineTo(95,40);
//      ImageSch->Canvas->TextOutA(10, 25, "1");
//      ImageSch->Canvas->TextOutA(42, 25, "2");
//    }
//    break;
//    case 4:
//    {
//      ImageSch->Canvas->MoveTo(35,30); ImageSch->Canvas->LineTo(35,70);
//      ImageSch->Canvas->Rectangle(30,40,40,60);
//      ImageSch->Canvas->MoveTo(65,30); ImageSch->Canvas->LineTo(65,70);
//      ImageSch->Canvas->Rectangle(60,40,70,60);
//      ImageSch->Canvas->MoveTo(35,30); ImageSch->Canvas->LineTo(90,30);
//      ImageSch->Canvas->MoveTo(25,40); ImageSch->Canvas->LineTo(25,60);
//      ImageSch->Canvas->MoveTo(10,50); ImageSch->Canvas->LineTo(25,50);
//      ImageSch->Canvas->MoveTo(10,70); ImageSch->Canvas->LineTo(90,70);
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(10,70); ImageSch->Canvas->LineTo(10,70);
//      ImageSch->Canvas->MoveTo(10,50); ImageSch->Canvas->LineTo(10,50);
//      ImageSch->Canvas->MoveTo(35,70); ImageSch->Canvas->LineTo(35,70);
//      ImageSch->Canvas->MoveTo(65,30); ImageSch->Canvas->LineTo(65,30);
//      ImageSch->Canvas->MoveTo(65,70); ImageSch->Canvas->LineTo(65,70);
//      ImageSch->Canvas->MoveTo(90,30); ImageSch->Canvas->LineTo(90,30);
//      ImageSch->Canvas->MoveTo(90,70); ImageSch->Canvas->LineTo(90,70);
//      ImageSch->Canvas->TextOutA(43, 56, "1");
//      ImageSch->Canvas->TextOutA(43, 31, "2");
//    }
//    break;
//    case 5:
//    {
//      ImageSch->Canvas->MoveTo(10,30); ImageSch->Canvas->LineTo(90,30);
//      ImageSch->Canvas->Rectangle(30,20,70,40);
//      ImageSch->Canvas->MoveTo(20,80); ImageSch->Canvas->LineTo(20,60);
//      ImageSch->Canvas->LineTo(80,60);
//      ImageSch->Canvas->LineTo(80,80);
//      ImageSch->Canvas->Rectangle(30,50,70,70);
//      ImageSch->Canvas->MoveTo(10,80); ImageSch->Canvas->LineTo(90,80);
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(10,80); ImageSch->Canvas->LineTo(10,80);
//      ImageSch->Canvas->MoveTo(20,80); ImageSch->Canvas->LineTo(20,80);
//      ImageSch->Canvas->MoveTo(80,80); ImageSch->Canvas->LineTo(80,80);
//      ImageSch->Canvas->MoveTo(90,80); ImageSch->Canvas->LineTo(90,80);
//      ImageSch->Canvas->MoveTo(10,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->MoveTo(90,30); ImageSch->Canvas->LineTo(90,30);
//      ImageSch->Canvas->TextOutA(17, 15, "1");
//      ImageSch->Canvas->TextOutA(77, 15, "2");
//      ImageSch->Canvas->TextOutA(17, 45, "3");
//      ImageSch->Canvas->TextOutA(77, 45, "4");
//    }
//    break;
//    case 6:
//    {
//      ImageSch->Canvas->MoveTo(10,30); ImageSch->Canvas->LineTo(90,30);
//      ImageSch->Canvas->Rectangle(30,20,70,40);
//      ImageSch->Canvas->MoveTo(80,80); ImageSch->Canvas->LineTo(80,60);
//      ImageSch->Canvas->MoveTo(80,60); ImageSch->Canvas->LineTo(20,60);
//      ImageSch->Canvas->Rectangle(30,50,70,70);
//      ImageSch->Canvas->MoveTo(10,80); ImageSch->Canvas->LineTo(90,80);
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(10,80); ImageSch->Canvas->LineTo(10,80);
//      ImageSch->Canvas->MoveTo(80,80); ImageSch->Canvas->LineTo(80,80);
//      ImageSch->Canvas->MoveTo(90,80); ImageSch->Canvas->LineTo(90,80);
//      ImageSch->Canvas->MoveTo(10,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->MoveTo(90,30); ImageSch->Canvas->LineTo(90,30);
//      ImageSch->Canvas->TextOutA(17, 15, "1");
//      ImageSch->Canvas->TextOutA(77, 15, "2");
//      ImageSch->Canvas->TextOutA(17, 45, "3");
//      ImageSch->Canvas->TextOutA(77, 45, "4");
//    }
//    break;
//    case 7:
//    {
//      ImageSch->Canvas->MoveTo(10,30); ImageSch->Canvas->LineTo(90,30);
//      ImageSch->Canvas->Rectangle(30,20,70,40);
//      ImageSch->Canvas->MoveTo(20,80); ImageSch->Canvas->LineTo(20,60);
//      ImageSch->Canvas->MoveTo(20,60); ImageSch->Canvas->LineTo(80,60);
//      ImageSch->Canvas->Rectangle(30,50,70,70);
//      ImageSch->Canvas->MoveTo(10,80); ImageSch->Canvas->LineTo(90,80);
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(10,80); ImageSch->Canvas->LineTo(10,80);
//      ImageSch->Canvas->MoveTo(20,80); ImageSch->Canvas->LineTo(20,80);
//      ImageSch->Canvas->MoveTo(90,80); ImageSch->Canvas->LineTo(90,80);
//      ImageSch->Canvas->MoveTo(10,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->MoveTo(90,30); ImageSch->Canvas->LineTo(90,30);
//      ImageSch->Canvas->TextOutA(17, 15, "1");
//      ImageSch->Canvas->TextOutA(77, 15, "2");
//      ImageSch->Canvas->TextOutA(17, 45, "3");
//      ImageSch->Canvas->TextOutA(77, 45, "4");
//    }
//    break;
//    case 8:
//    {
//      // ОУ
//      ImageSch->Canvas->MoveTo(55,55); ImageSch->Canvas->LineTo(55,25);
//      ImageSch->Canvas->LineTo(75,40);
//      ImageSch->Canvas->LineTo(55,55);
//      ImageSch->Canvas->MoveTo(40,40); ImageSch->Canvas->LineTo(55,40);
//      ImageSch->Canvas->MoveTo(75,40); ImageSch->Canvas->LineTo(95,40);
//
//      // RC-ЭРП
//      ImageSch->Canvas->Rectangle(20,20,40,30);
//      ImageSch->Canvas->Rectangle(20,35,40,45);
//      ImageSch->Canvas->MoveTo( 5,40); ImageSch->Canvas->LineTo(20,40);
//
//      // Обратная связь ОУ
//      ImageSch->Canvas->MoveTo(40,25); ImageSch->Canvas->LineTo(45,25);
//      ImageSch->Canvas->MoveTo(45,25); ImageSch->Canvas->LineTo(45,10);
//      ImageSch->Canvas->MoveTo(45,10); ImageSch->Canvas->LineTo(80,10);
//      ImageSch->Canvas->MoveTo(80,10); ImageSch->Canvas->LineTo(80,40);
//      ImageSch->Canvas->MoveTo(45,40); ImageSch->Canvas->LineTo(45,75);
//
//      ImageSch->Canvas->MoveTo( 5,75); ImageSch->Canvas->LineTo(95,75);
//      ImageSch->Canvas->MoveTo(65,48); ImageSch->Canvas->LineTo(65,83);
//      ImageSch->Canvas->MoveTo(60,83); ImageSch->Canvas->LineTo(70,83);
//
//      ImageSch->Canvas->Rectangle(40,68,50,48);
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(45,40); ImageSch->Canvas->LineTo(45,40);
//      ImageSch->Canvas->MoveTo(45,75); ImageSch->Canvas->LineTo(45,75);
//      ImageSch->Canvas->MoveTo(80,40); ImageSch->Canvas->LineTo(80,40);
//      ImageSch->Canvas->MoveTo(65,75); ImageSch->Canvas->LineTo(65,75);
//      ImageSch->Canvas->MoveTo( 5,75); ImageSch->Canvas->LineTo( 5,75);
//      ImageSch->Canvas->MoveTo(95,75); ImageSch->Canvas->LineTo(95,75);
//      ImageSch->Canvas->MoveTo( 5,40); ImageSch->Canvas->LineTo( 5,40);
//      ImageSch->Canvas->MoveTo(95,40); ImageSch->Canvas->LineTo(95,40);
//      ImageSch->Canvas->TextOutA(10, 25, "1");
//      ImageSch->Canvas->TextOutA(42, 26, "2");
//      ImageSch->Canvas->TextOutA(35, 7, "3");
//    }
//    break;
//    case 9:
//    {
//      // ОУ
//      ImageSch->Canvas->MoveTo(65,55); ImageSch->Canvas->LineTo(65,25);
//      ImageSch->Canvas->LineTo(85,40);
//      ImageSch->Canvas->LineTo(65,55);
//      // RC-ЭРП
//      ImageSch->Canvas->Rectangle(15,35,35,45);
//      ImageSch->Canvas->MoveTo(15,30); ImageSch->Canvas->LineTo(23,30);
//      ImageSch->Canvas->MoveTo(27,30); ImageSch->Canvas->LineTo(35,30);
//      // линия справа от RC-ЭРП
//      ImageSch->Canvas->MoveTo(35,40); ImageSch->Canvas->LineTo(65,40);
//      // линия слева от RC-ЭРП
//      ImageSch->Canvas->MoveTo( 5,40); ImageSch->Canvas->LineTo(15,40);
//      // линия справа от ОУ
//      ImageSch->Canvas->MoveTo(85,40); ImageSch->Canvas->LineTo(100,40);
//      // нижняя линия
//      ImageSch->Canvas->MoveTo( 5,75); ImageSch->Canvas->LineTo(100,75);
//      // земля ОУ
//      ImageSch->Canvas->MoveTo(75,48); ImageSch->Canvas->LineTo(75,83);
//      ImageSch->Canvas->MoveTo(70,83); ImageSch->Canvas->LineTo(80,83);
//      // обратная связь ОУ
//      ImageSch->Canvas->MoveTo(90,40); ImageSch->Canvas->LineTo(90,10);
//      ImageSch->Canvas->LineTo(19,10);
//      ImageSch->Canvas->LineTo(19,30);
//
//      // резистор L
//      ImageSch->Canvas->MoveTo(55,40); ImageSch->Canvas->LineTo(55,68);
//      ImageSch->Canvas->MoveTo(55,68); ImageSch->Canvas->LineTo(55,75);
//      ImageSch->Canvas->Rectangle(50,68,60,48);
//      // земля RC-ЭРП
//      ImageSch->Canvas->MoveTo(30,30); ImageSch->Canvas->LineTo(30,20);
//      ImageSch->Canvas->LineTo(43,20);
//      ImageSch->Canvas->LineTo(43,75);
//
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(55,40); ImageSch->Canvas->LineTo(55,40);
//      ImageSch->Canvas->MoveTo(55,75); ImageSch->Canvas->LineTo(55,75);
//      ImageSch->Canvas->MoveTo(90,40); ImageSch->Canvas->LineTo(90,40);
//      ImageSch->Canvas->MoveTo(75,75); ImageSch->Canvas->LineTo(75,75);
//      ImageSch->Canvas->MoveTo( 5,75); ImageSch->Canvas->LineTo( 5,75);
//      ImageSch->Canvas->MoveTo(100,75); ImageSch->Canvas->LineTo(100,75);
//      ImageSch->Canvas->MoveTo( 5,40); ImageSch->Canvas->LineTo( 5,40);
//      ImageSch->Canvas->MoveTo(100,40); ImageSch->Canvas->LineTo(100,40);
//      ImageSch->Canvas->MoveTo(43,75); ImageSch->Canvas->LineTo(43,75);
//      ImageSch->Canvas->TextOutA(8, 45, "1");
//      ImageSch->Canvas->TextOutA(35, 45, "2");
//      ImageSch->Canvas->TextOutA(8, 26, "3");
//      ImageSch->Canvas->TextOutA(35, 26, "4");
//    }
//    break;
//    case 10:
//    {
//      // ОУ
//      ImageSch->Canvas->MoveTo(65,55); ImageSch->Canvas->LineTo(65,25);
//      ImageSch->Canvas->LineTo(85,40);
//      ImageSch->Canvas->LineTo(65,55);
//      // RC-ЭРП
//      ImageSch->Canvas->Rectangle(15,15,35,25);
//      ImageSch->Canvas->MoveTo(15,30); ImageSch->Canvas->LineTo(23,30);
//      ImageSch->Canvas->MoveTo(27,30); ImageSch->Canvas->LineTo(35,30);
//      // линия справа от RC-ЭРП
//      ImageSch->Canvas->MoveTo(30,30); ImageSch->Canvas->LineTo(30,40);
//      ImageSch->Canvas->LineTo(65,40);
//      // линия слева от RC-ЭРП
//      ImageSch->Canvas->MoveTo( 5,40); ImageSch->Canvas->LineTo(19,40);
//      ImageSch->Canvas->LineTo(19,30);
//      // линия справа от ОУ
//      ImageSch->Canvas->MoveTo(85,40); ImageSch->Canvas->LineTo(100,40);
//      // нижняя линия
//      ImageSch->Canvas->MoveTo( 5,75); ImageSch->Canvas->LineTo(100,75);
//      // земля ОУ
//      ImageSch->Canvas->MoveTo(75,48); ImageSch->Canvas->LineTo(75,83);
//      ImageSch->Canvas->MoveTo(70,83); ImageSch->Canvas->LineTo(80,83);
//      // обратная связь ОУ
//      ImageSch->Canvas->MoveTo(90,40); ImageSch->Canvas->LineTo(90,10);
//      ImageSch->Canvas->LineTo(10,10);
//      ImageSch->Canvas->LineTo(10,20);
//      ImageSch->Canvas->LineTo(15,20);
//      ImageSch->Canvas->MoveTo(35,20); ImageSch->Canvas->LineTo(40,20);
//      ImageSch->Canvas->LineTo(40,10);
//
//      // резистор L
//      ImageSch->Canvas->MoveTo(55,40); ImageSch->Canvas->LineTo(55,68);
//      ImageSch->Canvas->MoveTo(55,68); ImageSch->Canvas->LineTo(55,75);
//      ImageSch->Canvas->Rectangle(50,68,60,48);
//
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(55,40); ImageSch->Canvas->LineTo(55,40);
//      ImageSch->Canvas->MoveTo(55,75); ImageSch->Canvas->LineTo(55,75);
//      ImageSch->Canvas->MoveTo(90,40); ImageSch->Canvas->LineTo(90,40);
//      ImageSch->Canvas->MoveTo(75,75); ImageSch->Canvas->LineTo(75,75);
//      ImageSch->Canvas->MoveTo( 5,75); ImageSch->Canvas->LineTo( 5,75);
//      ImageSch->Canvas->MoveTo(100,75); ImageSch->Canvas->LineTo(100,75);
//      ImageSch->Canvas->MoveTo( 5,40); ImageSch->Canvas->LineTo( 5,40);
//      ImageSch->Canvas->MoveTo(100,40); ImageSch->Canvas->LineTo(100,40);
//      ImageSch->Canvas->MoveTo(40,10); ImageSch->Canvas->LineTo(40,10);
//      ImageSch->Canvas->TextOutA(3, 13, "1");
//      ImageSch->Canvas->TextOutA(42, 13, "2");
//      ImageSch->Canvas->TextOutA(8, 26, "3");
//      ImageSch->Canvas->TextOutA(35, 26, "4");
//    }
//    break;
//    case 11:
//    {
//      // ОУ
//      ImageSch->Canvas->MoveTo(65,55); ImageSch->Canvas->LineTo(65,25);
//      ImageSch->Canvas->LineTo(85,40);
//      ImageSch->Canvas->LineTo(65,55);
//      // RC-ЭРП
//      ImageSch->Canvas->Rectangle(25,24,35,44);
//      ImageSch->Canvas->MoveTo(40,24); ImageSch->Canvas->LineTo(40,32);
//      ImageSch->Canvas->MoveTo(40,36); ImageSch->Canvas->LineTo(40,44);
//      // линия справа от RC-ЭРП
//      ImageSch->Canvas->MoveTo(40,40); ImageSch->Canvas->LineTo(65,40);
//      // линия слева от RC-ЭРП
//      ImageSch->Canvas->MoveTo( 5,34); ImageSch->Canvas->LineTo(15,34);
//      ImageSch->Canvas->LineTo(15,15);
//      ImageSch->Canvas->LineTo(30,15);
//      ImageSch->Canvas->LineTo(30,25);
//      ImageSch->Canvas->MoveTo(15,15); ImageSch->Canvas->LineTo(15,42);
//      ImageSch->Canvas->LineTo(15,52);
//      ImageSch->Canvas->LineTo(30,52);
//      ImageSch->Canvas->LineTo(30,42);
//      // линия справа от ОУ
//      ImageSch->Canvas->MoveTo(85,40); ImageSch->Canvas->LineTo(100,40);
//      // нижняя линия
//      ImageSch->Canvas->MoveTo( 5,75); ImageSch->Canvas->LineTo(100,75);
//      // земля ОУ
//      ImageSch->Canvas->MoveTo(75,48); ImageSch->Canvas->LineTo(75,83);
//      ImageSch->Canvas->MoveTo(70,83); ImageSch->Canvas->LineTo(80,83);
//      // обратная связь ОУ
//      ImageSch->Canvas->MoveTo(90,40); ImageSch->Canvas->LineTo(90,10);
//      ImageSch->Canvas->LineTo(50,10);
//      ImageSch->Canvas->LineTo(50,28);
//      ImageSch->Canvas->LineTo(40,28);
//
//      // резистор L
//      ImageSch->Canvas->MoveTo(55,40); ImageSch->Canvas->LineTo(55,68);
//      ImageSch->Canvas->MoveTo(55,68); ImageSch->Canvas->LineTo(55,75);
//      ImageSch->Canvas->Rectangle(50,68,60,48);
//
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(55,40); ImageSch->Canvas->LineTo(55,40);
//      ImageSch->Canvas->MoveTo(55,75); ImageSch->Canvas->LineTo(55,75);
//      ImageSch->Canvas->MoveTo(90,40); ImageSch->Canvas->LineTo(90,40);
//      ImageSch->Canvas->MoveTo(75,75); ImageSch->Canvas->LineTo(75,75);
//      ImageSch->Canvas->MoveTo( 5,75); ImageSch->Canvas->LineTo( 5,75);
//      ImageSch->Canvas->MoveTo(100,75); ImageSch->Canvas->LineTo(100,75);
//      ImageSch->Canvas->MoveTo( 5,34); ImageSch->Canvas->LineTo( 5,34);
//      ImageSch->Canvas->MoveTo(100,40); ImageSch->Canvas->LineTo(100,40);
//      ImageSch->Canvas->MoveTo(15,34); ImageSch->Canvas->LineTo(15,34);
//      ImageSch->Canvas->TextOutA(25, 2, "2");
//      ImageSch->Canvas->TextOutA(25, 53, "1");
//      ImageSch->Canvas->TextOutA(40, 10, "4");
//      ImageSch->Canvas->TextOutA(40, 45, "3");
//    }
//    break;
//    case 12:
//    {
//      ImageSch->Canvas->TextOutA(12, 28, "1");
//      ImageSch->Canvas->TextOutA(41, 28, "2");
//      ImageSch->Canvas->TextOutA(12, 56, "3");
//      ImageSch->Canvas->TextOutA(41, 56, "4");
//      ImageSch->Canvas->TextOutA(57, 28, "1");
//      ImageSch->Canvas->TextOutA(86, 28, "2");
//      ImageSch->Canvas->TextOutA(57, 56, "3");
//      ImageSch->Canvas->TextOutA(86, 56, "4");
//      // RC-ЭРП 1
//      ImageSch->Canvas->Rectangle(20,35,40,45);
//      ImageSch->Canvas->Rectangle(20,50,40,60);
//      ImageSch->Canvas->MoveTo(40,40); ImageSch->Canvas->LineTo(52,40);
//      ImageSch->Canvas->LineTo(52,55);
//      ImageSch->Canvas->LineTo(65,55);
//      ImageSch->Canvas->MoveTo(20,40); ImageSch->Canvas->LineTo(12,40);
//      ImageSch->Canvas->LineTo(12,48);
//      ImageSch->Canvas->LineTo(16,48);
//      ImageSch->Canvas->LineTo(8,48);
//      ImageSch->Canvas->MoveTo(20,55); ImageSch->Canvas->LineTo(5,55);
//      ImageSch->Canvas->LineTo(5,15);
//      ImageSch->Canvas->LineTo(100,15);
//      ImageSch->Canvas->LineTo(100,40);
//      ImageSch->Canvas->LineTo(84,40);
//      ImageSch->Canvas->MoveTo(84,55); ImageSch->Canvas->LineTo(95,55);
//      ImageSch->Canvas->LineTo(95,90);
//      ImageSch->Canvas->MoveTo(65,40); ImageSch->Canvas->LineTo(57,40);
//      ImageSch->Canvas->LineTo(57,48);
//      ImageSch->Canvas->LineTo(61,48);
//      ImageSch->Canvas->LineTo(53,48);
//      ImageSch->Canvas->MoveTo(40,55); ImageSch->Canvas->LineTo(47,55);
//      ImageSch->Canvas->LineTo(47,63);
//      ImageSch->Canvas->LineTo(51,63);
//      ImageSch->Canvas->LineTo(43,63);
//
//      // RC-ЭРП 2
//      ImageSch->Canvas->Rectangle(65,35,85,45);
//      ImageSch->Canvas->Rectangle(65,50,85,60);
//
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(95,90); ImageSch->Canvas->LineTo(95,90);
//    }
//    break;
//    case 13:
//    {
//      ImageSch->Canvas->Rectangle(30,20,70,40);
//      ImageSch->Canvas->Rectangle(30,50,70,70);
//      ImageSch->Canvas->MoveTo(30,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->MoveTo(70,30); ImageSch->Canvas->LineTo(80,30);
//      ImageSch->Canvas->MoveTo(70,60); ImageSch->Canvas->LineTo(80,60);
//      ImageSch->Canvas->MoveTo(30,60); ImageSch->Canvas->LineTo(20,60);
//      ImageSch->Canvas->LineTo(20,80);
//      ImageSch->Canvas->LineTo(25,80);
//      ImageSch->Canvas->LineTo(15,80);
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(10,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->TextOutA(17, 15, "1");
//      ImageSch->Canvas->TextOutA(77, 15, "2");
//      ImageSch->Canvas->TextOutA(17, 45, "3");
//      ImageSch->Canvas->TextOutA(77, 45, "4");
//    }
//    break;
//    case 14:
//    {
//      ImageSch->Canvas->Rectangle(30,20,70,40);
//      ImageSch->Canvas->Rectangle(30,50,70,70);
//      ImageSch->Canvas->MoveTo(30,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->MoveTo(70,30); ImageSch->Canvas->LineTo(80,30);
//      ImageSch->Canvas->MoveTo(30,60); ImageSch->Canvas->LineTo(20,60);
//      ImageSch->Canvas->MoveTo(70,60); ImageSch->Canvas->LineTo(80,60);
//      ImageSch->Canvas->LineTo(80,80);
//      ImageSch->Canvas->LineTo(85,80);
//      ImageSch->Canvas->LineTo(75,80);
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(10,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->TextOutA(17, 15, "1");
//      ImageSch->Canvas->TextOutA(77, 15, "2");
//      ImageSch->Canvas->TextOutA(17, 45, "3");
//      ImageSch->Canvas->TextOutA(77, 45, "4");
//    }
//    break;
//    case 15:
//    {
//      ImageSch->Canvas->Rectangle(30,20,70,40);
//      ImageSch->Canvas->Rectangle(30,50,70,70);
//      ImageSch->Canvas->MoveTo(30,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->MoveTo(70,30); ImageSch->Canvas->LineTo(90,30);
//      ImageSch->Canvas->LineTo(90,50);
//      ImageSch->Canvas->LineTo(95,50);
//      ImageSch->Canvas->LineTo(85,50);
//      ImageSch->Canvas->MoveTo(30,60); ImageSch->Canvas->LineTo(20,60);
//      ImageSch->Canvas->MoveTo(70,60); ImageSch->Canvas->LineTo(80,60);
//      ImageSch->Canvas->LineTo(80,80);
//      ImageSch->Canvas->LineTo(85,80);
//      ImageSch->Canvas->LineTo(75,80);
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(10,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->TextOutA(17, 15, "1");
//      ImageSch->Canvas->TextOutA(77, 15, "2");
//      ImageSch->Canvas->TextOutA(17, 45, "3");
//      ImageSch->Canvas->TextOutA(77, 45, "4");
//    }
//    break;
//    case 16:
//    {
//      ImageSch->Canvas->Rectangle(30,20,70,40);
//      ImageSch->Canvas->Rectangle(30,50,70,70);
//      ImageSch->Canvas->MoveTo(30,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->MoveTo(70,30); ImageSch->Canvas->LineTo(90,30);
//      ImageSch->Canvas->LineTo(90,50);
//      ImageSch->Canvas->LineTo(95,50);
//      ImageSch->Canvas->LineTo(85,50);
//      ImageSch->Canvas->MoveTo(30,60); ImageSch->Canvas->LineTo(20,60);
//      ImageSch->Canvas->LineTo(20,80);
//      ImageSch->Canvas->LineTo(25,80);
//      ImageSch->Canvas->LineTo(15,80);
//      ImageSch->Canvas->MoveTo(70,60); ImageSch->Canvas->LineTo(80,60);
//      ImageSch->Canvas->LineTo(80,80);
//      ImageSch->Canvas->LineTo(85,80);
//      ImageSch->Canvas->LineTo(75,80);
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(10,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->TextOutA(17, 15, "1");
//      ImageSch->Canvas->TextOutA(77, 15, "2");
//      ImageSch->Canvas->TextOutA(17, 45, "3");
//      ImageSch->Canvas->TextOutA(77, 45, "4");
//    }
//    break;
//    case 17:
//    {
//      ImageSch->Canvas->Rectangle(30,20,70,40);
//      ImageSch->Canvas->Rectangle(30,50,70,70);
//      ImageSch->Canvas->MoveTo(30,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->MoveTo(70,30); ImageSch->Canvas->LineTo(80,30);
//      ImageSch->Canvas->MoveTo(30,60); ImageSch->Canvas->LineTo(20,60);
//      ImageSch->Canvas->LineTo(20,80);
//      ImageSch->Canvas->LineTo(25,80);
//      ImageSch->Canvas->LineTo(15,80);
//      ImageSch->Canvas->MoveTo(70,60); ImageSch->Canvas->LineTo(80,60);
//      ImageSch->Canvas->LineTo(80,80);
//      ImageSch->Canvas->LineTo(85,80);
//      ImageSch->Canvas->LineTo(75,80);
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(10,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->TextOutA(17, 15, "1");
//      ImageSch->Canvas->TextOutA(77, 15, "2");
//      ImageSch->Canvas->TextOutA(17, 45, "3");
//      ImageSch->Canvas->TextOutA(77, 45, "4");
//    }
//    break;
//    case 18:
//    {
//      ImageSch->Canvas->Rectangle(30,20,70,40);
//      ImageSch->Canvas->Rectangle(30,50,70,70);
//      ImageSch->Canvas->MoveTo(30,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->MoveTo(70,30); ImageSch->Canvas->LineTo(90,30);
//      ImageSch->Canvas->LineTo(90,60);
//      ImageSch->Canvas->LineTo(69,60);
//      ImageSch->Canvas->MoveTo(30,60); ImageSch->Canvas->LineTo(20,60);
//      ImageSch->Canvas->LineTo(20,80);
//      ImageSch->Canvas->LineTo(25,80);
//      ImageSch->Canvas->LineTo(15,80);
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(10,30); ImageSch->Canvas->LineTo(10,30);
//      ImageSch->Canvas->TextOutA(17, 15, "1");
//      ImageSch->Canvas->TextOutA(77, 15, "2");
//      ImageSch->Canvas->TextOutA(17, 45, "3");
//      ImageSch->Canvas->TextOutA(77, 45, "4");
//    }
//    break;
//    case 20:
//    {
//      ImageSch->Canvas->TextOutA(12, 28, "1");
//      ImageSch->Canvas->TextOutA(41, 28, "2");
//      ImageSch->Canvas->TextOutA(12, 56, "3");
//      ImageSch->Canvas->TextOutA(41, 56, "4");
//      ImageSch->Canvas->TextOutA(57, 28, "5");
//      ImageSch->Canvas->TextOutA(86, 28, "6");
//      ImageSch->Canvas->TextOutA(57, 56, "7");
//      ImageSch->Canvas->TextOutA(86, 56, "8");
//      // RC-ЭРП 1
//      ImageSch->Canvas->Rectangle(20,35,40,45);
//      ImageSch->Canvas->Rectangle(20,50,40,60);
//      ImageSch->Canvas->MoveTo(40,40); ImageSch->Canvas->LineTo(52,40);
//      ImageSch->Canvas->LineTo(52,55);
//      ImageSch->Canvas->LineTo(65,55);
//      ImageSch->Canvas->MoveTo(20,40); ImageSch->Canvas->LineTo(12,40);
//      ImageSch->Canvas->LineTo(12,48);
//      ImageSch->Canvas->LineTo(16,48);
//      ImageSch->Canvas->LineTo(8,48);
//      ImageSch->Canvas->MoveTo(20,55); ImageSch->Canvas->LineTo(5,55);
//      ImageSch->Canvas->LineTo(5,15);
//      ImageSch->Canvas->LineTo(100,15);
//      ImageSch->Canvas->LineTo(100,40);
//      ImageSch->Canvas->LineTo(84,40);
//      ImageSch->Canvas->MoveTo(84,55); ImageSch->Canvas->LineTo(95,55);
//      ImageSch->Canvas->LineTo(95,90);
//      ImageSch->Canvas->MoveTo(65,40); ImageSch->Canvas->LineTo(57,40);
//      ImageSch->Canvas->LineTo(57,48);
//      ImageSch->Canvas->LineTo(61,48);
//      ImageSch->Canvas->LineTo(53,48);
//      ImageSch->Canvas->MoveTo(40,55); ImageSch->Canvas->LineTo(47,55);
//      ImageSch->Canvas->LineTo(47,63);
//      ImageSch->Canvas->LineTo(51,63);
//      ImageSch->Canvas->LineTo(43,63);
//
//      // RC-ЭРП 2
//      ImageSch->Canvas->Rectangle(65,35,85,45);
//      ImageSch->Canvas->Rectangle(65,50,85,60);
//
//      ImageSch->Canvas->Pen->Width=4;
//      ImageSch->Canvas->MoveTo(95,90); ImageSch->Canvas->LineTo(95,90);
//    }
//    break;
//
//    default:
//    {
//      //ImageSch->Canvas->Font->Style = ImageSch->Canvas->Font->Style << fsBold;
//      ImageSch->Canvas->TextOutA(30, 44, "no image");
//    }
//    break;
//  }
//
//}

int CPassive16::CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
{
  complex<double> y11,y12,y13,y22,y23,y33;
  for (int i=0; i<T->m_length; ++i)
  {
    // заполнение матрицы схемы
    y11 = T->m_MatY[i][35];
    y12 = T->m_MatY[i][34];
    y13 = T->m_MatY[i][32];
    y22 = T->m_MatY[i][ 8] + T->m_MatY[i][33];
    y23 = T->m_MatY[i][ 9] + T->m_MatY[i][31];
    y33 = T->m_MatY[i][30] + T->m_MatY[i][15];

    // понижение порядка
    y11 -= y13*y13/y33;
    y12 -= y13*y23/y33;
    y22 -= y23*y23/y33;

    // понижение порядка и получение T
    T->m_T[i] = y22/(y11*y22-y12*y12);
  }
  return 0;
}

int CPassive17::CharacteristicT(CRCStructureCalculateData *T, CRCStructure *Structure)
{
  for (int i=0; i<T->m_length; ++i)
  {
    T->m_T[i] = p.Eval().GetComplex();
  }
  return 0;
}

int CPassive17::CharacteristicZ(CRCStructureCalculateData *T, CRCStructure *Structure)
{
  for (int i=0; i<T->m_length; ++i)
  {
    T->m_T[i] = p.Eval().GetComplex();
  }
  return 0;
}

void CPassive17::ParseFormula(const string& formula)
{

}
