//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "ThreadGA.h"
#include "Unit1.h"
#include "Unit6.h"

extern TForm1 *Form1;
extern TForm6 *Form6;
extern CTargetFunction *pTarget;
extern CScheme *pScheme;
extern int nGACount, nCharacteristic, nMaxBatchGACount;
extern double dDeviation, dLastDeviation;
extern double dMeanValue;
extern bool bIsNotBatch;
extern int nTime0, nTime_accumulated;
extern int napr;
extern double dParam1;

extern CRCStructure **pIslandStructures;
extern unsigned int nIslandStructuresCount;

#pragma package(smart_init)
//---------------------------------------------------------------------------

//   Important: Methods and properties of objects in VCL can only be
//   used in a method called using Synchronize, for example:
//
//      Synchronize(UpdateCaption);
//
//   where UpdateCaption could look like:
//
//      void __fastcall TThreadGA::UpdateCaption()
//      {
//        Form1->Caption = "Updated in a thread";
//      }
//---------------------------------------------------------------------------

__fastcall TThreadGA::TThreadGA(CRCStructure **ppInitialStructure, double dMaxDeviation, CGASettings *pSettings) : TThread(false)
{
  m_ppInitialStructure = ppInitialStructure;
  SS[1] = *ppInitialStructure;
  SS[2] = (*ppInitialStructure)->clone();
  SS[3] = (*ppInitialStructure)->clone();
  SS[4] = (*ppInitialStructure)->clone();
  m_dMaxDeviation = dMaxDeviation;

  m_pGASettings = new CGASettings(*pSettings);

  m_nNonImprovements = 0;
  FreeOnTerminate = true;
}
//---------------------------------------------------------------------------

__fastcall TThreadGA::~TThreadGA()
{
  *m_ppInitialStructure = SS[1];
  delete SS[2];
  delete SS[3];
  delete SS[4];
  delete m_pGASettings;
}
//---------------------------------------------------------------------------

void __fastcall TThreadGA::Execute()
{
  nTime0 = GetTickCount();
  int nMutateSizeTemp;
  int nMutateSizeX = m_pGASettings->m_nRestrictions[0];
  int nMutateSizeY = m_pGASettings->m_nRestrictions[1];
  int nOriginalSizeX = SS[1]->Width();
  int nOriginalSizeY = SS[1]->Height();

  if (m_pGASettings->m_bDynamicGrid)
  {
    SS[1]->Resize(nOriginalSizeX/2, nOriginalSizeY/2);
  }

  srand((unsigned)time(NULL));

  if (m_pGASettings->m_bIslandModel && nIslandStructuresCount < m_pGASettings->m_nIslands)
  {
    int nOriginalGACount = nGACount;
    double dOriginalDeviation = dDeviation;

    m_pGASettings->m_bIslandModel = false;

    bool bIsNotBatch_Prev = bIsNotBatch;
    bIsNotBatch = false;
    CRCStructure *pTempStructure = SS[1]->clone();
    pTempStructure->Resize(pTempStructure->Width()/2, pTempStructure->Height()/2);

    while (nIslandStructuresCount < m_pGASettings->m_nIslands)
    {
      nGACount = 0;
      dDeviation = 100000.0;
      CRCStructure *pIslandStructure = pTempStructure->clone();

      TThreadGA *pInslandTGA = new TThreadGA(&pIslandStructure, m_pGASettings->m_dIslandDeviation, m_pGASettings);
      WaitForSingleObject((HANDLE)(pInslandTGA->Handle), INFINITE);
      //if (WaitForSingleObject((HANDLE)(pInslandTGA->Handle), INFINITE) != WAIT_OBJECT_0)
      //  ShowMessage("Ошибка ожидания потока");
      //Application->ProcessMessages();

      pIslandStructures[nIslandStructuresCount++] = pIslandStructure;
    }
    bIsNotBatch = bIsNotBatch_Prev;
    m_pGASettings->m_bIslandModel = true;

    delete pTempStructure;
    nGACount = nOriginalGACount;
    dDeviation = dOriginalDeviation;
  }

  st[1] = CalculateStructure(1);
  st[2] = CalculateStructure(2);
  dDeviation = st[1];

  while (!Terminated && dDeviation > m_dMaxDeviation)
  {

    // динамическое изменение максимальной площади мутации
    double m1 = pTarget->MeanValue(1);
    double p1 = pow(dDeviation/m1, 0.25);

    if (nMutateSizeX > 1)
    {
      nMutateSizeTemp = p1*SS[1]->Width();
      if (nMutateSizeTemp == 0)
        nMutateSizeX = 1;
      else if (nMutateSizeTemp < nMutateSizeX)
        nMutateSizeX = nMutateSizeTemp;
    }

    if (nMutateSizeY > 1)
    {
      nMutateSizeTemp = p1*SS[1]->Height();
      if (nMutateSizeTemp == 0)
        nMutateSizeY = 1;
      else if (nMutateSizeTemp < nMutateSizeY)
        nMutateSizeY = nMutateSizeTemp;
    }

    // динамическое изменение сетки
    if (m_pGASettings->m_bDynamicGrid)
    {
      if (dDeviation < 0.1 || m_nNonImprovements>200)
      {
        SS[1]->Resize(nOriginalSizeX, nOriginalSizeY);
        SS[2]->Resize(nOriginalSizeX, nOriginalSizeY);
        SS[3]->Resize(nOriginalSizeX, nOriginalSizeY);
        SS[4]->Resize(nOriginalSizeX, nOriginalSizeY);
      }
      else if (dDeviation < 2.0 || m_nNonImprovements>100)
      {
        SS[1]->Resize(nOriginalSizeX*0.71, nOriginalSizeY*0.71);
        SS[2]->Resize(nOriginalSizeX*0.71, nOriginalSizeY*0.71);
        SS[3]->Resize(nOriginalSizeX*0.71, nOriginalSizeY*0.71);
        SS[4]->Resize(nOriginalSizeX*0.71, nOriginalSizeY*0.71);
      }
      else
      {
        SS[1]->Resize(nOriginalSizeX >> 1, nOriginalSizeY >> 1);
        SS[2]->Resize(nOriginalSizeX >> 1, nOriginalSizeY >> 1);
        SS[3]->Resize(nOriginalSizeX >> 1, nOriginalSizeY >> 1);
        SS[4]->Resize(nOriginalSizeX >> 1, nOriginalSizeY >> 1);
      }
    }

    SS[3]->CopyStructure(SS[1]);
    SS[4]->CopyStructure(SS[2]);

    SS[3]->ResizeKP(m_pGASettings->m_nProbabilities[2], m_pGASettings->m_nRestrictions[4], m_pGASettings->m_bStructureFit);
    SS[3]->MoveKP(m_pGASettings->m_nProbabilities[3],   m_pGASettings->m_nRestrictions[5], m_pGASettings->m_bStructureFit);
    SS[4]->ResizeKP(m_pGASettings->m_nProbabilities[2], m_pGASettings->m_nRestrictions[4], m_pGASettings->m_bStructureFit);
    SS[4]->MoveKP(m_pGASettings->m_nProbabilities[3],   m_pGASettings->m_nRestrictions[5], m_pGASettings->m_bStructureFit);
// если выполнили скрещивание одинаковых слоев, то скрещивание разных слоев выполнять не нужно
// если скрещивание одинаковых слоев не выполнилось, то по выходу из функции выполняем скрещивание разных слоев
    if (SS[3]->Cross(SS[4], m_pGASettings->m_nProbabilities[6], m_pGASettings->m_nRestrictions[2], m_pGASettings->m_nRestrictions[3], false))
      SS[3]->Cross(SS[4], m_pGASettings->m_nProbabilities[7],   m_pGASettings->m_nRestrictions[2], m_pGASettings->m_nRestrictions[2], true);


    int nMutateX, nMutateY, nMutateWidth, nMutateHeight, nPlaces, nPlacesTemp;
    PreferableMutatePlaces = NULL;
    if (m_pGASettings->m_bDirectSynthesis && ((nPlaces=GetPreferableMutatePlacesFor(3)) > 0)) // направленный выбор места мутации
    {
      nPlacesTemp = _lrand()%nPlaces;
      nMutateX = PreferableMutatePlaces[nPlacesTemp].x;
      nMutateY = PreferableMutatePlaces[nPlacesTemp].y;
      nMutateWidth  = _lrand()%nMutateSizeX;
      nMutateHeight = _lrand()%nMutateSizeY;
      nMutateX -= nMutateWidth >>1;
      nMutateY -= nMutateHeight>>1;
      if (nMutateX<0) nMutateX=0;
      if (nMutateY<0) nMutateY=0;
      SS[3]->Mutate(m_pGASettings->m_nProbabilities[0], m_pGASettings->m_nProbabilities[1], m_pGASettings->m_bStructureFit, nMutateX, nMutateY, nMutateWidth, nMutateHeight, m_pGASettings->m_nProbabilities[4]);

      nPlacesTemp = _lrand()%nPlaces;
      nMutateX = PreferableMutatePlaces[nPlacesTemp].x;
      nMutateY = PreferableMutatePlaces[nPlacesTemp].y;
      nMutateWidth  = _lrand()%nMutateSizeX;
      nMutateHeight = _lrand()%nMutateSizeY;
      nMutateX -= nMutateWidth >>1;
      nMutateY -= nMutateHeight>>1;
      if (nMutateX<0) nMutateX=0;
      if (nMutateY<0) nMutateY=0;
      SS[4]->Mutate(m_pGASettings->m_nProbabilities[0], m_pGASettings->m_nProbabilities[1], m_pGASettings->m_bStructureFit, nMutateX, nMutateY, nMutateWidth, nMutateHeight, m_pGASettings->m_nProbabilities[4]);
    }
    else // случайный выбор места мутации
    {
      // применять мутцию пока структура не изменится
      do
      {
        nMutateWidth  = _lrand()%nMutateSizeX;
        nMutateHeight = _lrand()%nMutateSizeY;
        nMutateX = _lrand()%(SS[3]->Width()-nMutateWidth);
        nMutateY = _lrand()%(SS[3]->Height()-nMutateHeight);
        SS[3]->Mutate(m_pGASettings->m_nProbabilities[0], m_pGASettings->m_nProbabilities[1], m_pGASettings->m_bStructureFit, nMutateX, nMutateY, nMutateWidth, nMutateHeight, m_pGASettings->m_nProbabilities[4]);
      }
      while (SS[1]->Equal(SS[3]));

      do
      {
        nMutateWidth  = _lrand()%nMutateSizeX;
        nMutateHeight = _lrand()%nMutateSizeY;
        nMutateX = _lrand()%(SS[4]->Width()-nMutateWidth);
        nMutateY = _lrand()%(SS[4]->Height()-nMutateHeight);
        SS[4]->Mutate(m_pGASettings->m_nProbabilities[0], m_pGASettings->m_nProbabilities[1], m_pGASettings->m_bStructureFit, nMutateX, nMutateY, nMutateWidth, nMutateHeight, m_pGASettings->m_nProbabilities[4]);
      }
      while (SS[2]->Equal(SS[4]));
    }

    delete[] PreferableMutatePlaces;

    st[3] = CalculateStructure(3);
    st[4] = CalculateStructure(4);

    double *R[5] = {NULL, pTarget->m_char[1], pTarget->m_char[2], pTarget->m_char[3], pTarget->m_char[4]};
    CRCStructure *S[5] = {NULL, SS[1], SS[2], SS[3], SS[4]};

    int min_index = 1;
    for (int i=2; i<5; ++i)
      if (st[i] < st[min_index])
        min_index = i;
    if (min_index != 1)
    {
      S[0] = S[1];
      S[1] = S[min_index];
      S[min_index] = S[0];
      R[0] = R[1];
      R[1] = R[min_index];
      R[min_index] = R[0];
      st[0] = st[1];
      st[1] = st[min_index];
      st[min_index] = st[0];
      SS[1]->CopyStructure(S[1]);
      memcpy(pTarget->m_char[1], R[1], pTarget->m_length*sizeof(double));

      *m_ppInitialStructure = SS[1];
    }
    min_index = 2;
    for (int i=3; i<5; ++i)
      if (st[i] < st[min_index])
        min_index = i;
    if (min_index != 2)
    {
      S[0] = S[2];
      S[2] = S[min_index];
      S[min_index] = S[0];
      R[0] = R[2];
      R[2] = R[min_index];
      R[min_index] = R[0];
      SS[2]->CopyStructure(S[2]);
      memcpy(pTarget->m_char[2], R[2], pTarget->m_length*sizeof(double));
    }

    dLastDeviation = dDeviation;
    dDeviation = st[1];

    if (dDeviation/dLastDeviation > m_pGASettings->m_dNonImprovementThreshold)
      ++m_nNonImprovements;
    else
      m_nNonImprovements=0;

    if (m_pGASettings->m_bDirectSynthesis && m_nNonImprovements > m_pGASettings->m_nCancelDirectSynthesisThreshold)
      m_pGASettings->m_bDirectSynthesis = false;

    if (m_pGASettings->m_AutoRegulateK.Enabled && pScheme->isActiveScheme() && m_nNonImprovements>m_pGASettings->m_AutoRegulateK.Threshold)
    {
      double best_K = pScheme->GetK();
      for (double k_temp=m_pGASettings->m_AutoRegulateK.Low; k_temp<m_pGASettings->m_AutoRegulateK.High; k_temp+=m_pGASettings->m_AutoRegulateK.Step)
      {
        pScheme->SetK(k_temp);

        pScheme->Setka(pTarget, nCharacteristic, pTarget->m_char[0], SS[1]);
        st[0] = pTarget->Deviation(0);

        if (st[0] < dDeviation)
        {
          dDeviation = st[0];
          best_K = k_temp;
          memcpy(pTarget->m_char[1], pTarget->m_char[0], pTarget->m_length*sizeof(double));
        }
      }
      m_nNonImprovements=0;
      pScheme->SetK(best_K);
    }

    if (m_pGASettings->m_AutoRegulateN.Enabled && m_nNonImprovements>m_pGASettings->m_AutoRegulateN.Threshold)
    {
      double best_N = SS[1]->GetN();
      for (double n_temp=m_pGASettings->m_AutoRegulateN.Low; n_temp<m_pGASettings->m_AutoRegulateN.High; n_temp+=m_pGASettings->m_AutoRegulateN.Step)
      {
        SS[1]->SetN(n_temp);
        if (Form1->CalculateYParameters(SS[1], pTarget) == -1)
          st[0] = 100000.0;
        else
        {
          pScheme->Setka(pTarget, nCharacteristic, pTarget->m_char[0], SS[1]);
          st[0] = pTarget->Deviation(0);
        }

        if (st[0] < dDeviation)
        {
          dDeviation = st[0];
          best_N = n_temp;
          memcpy(pTarget->m_char[1], pTarget->m_char[0], pTarget->m_length*sizeof(double));
        }
      }
      m_nNonImprovements=0;
      SS[1]->SetN(best_N);
      SS[2]->SetN(best_N);
      SS[3]->SetN(best_N);
      SS[4]->SetN(best_N);
    }

    if (m_pGASettings->m_AutoRegulateWRC.Enabled && m_nNonImprovements>m_pGASettings->m_AutoRegulateWRC.Threshold && pScheme != NULL)
    {
      double best_koeff = 1.0;
      double *temp_w = new double[pTarget->m_length];
      memcpy(temp_w, pTarget->m_w, pTarget->m_length*sizeof(double));

      for (double wrc_temp=m_pGASettings->m_AutoRegulateWRC.Low; wrc_temp<m_pGASettings->m_AutoRegulateWRC.High; wrc_temp+=m_pGASettings->m_AutoRegulateWRC.Step)
      {
        for (int i=0; i<pTarget->m_length; ++i)
          pTarget->m_w[i] *= wrc_temp;

        if (Form1->CalculateYParameters(SS[1], pTarget) == -1)
          st[0] = 100000.0;
        else
        {
          pScheme->Setka(pTarget, nCharacteristic, pTarget->m_char[0], SS[1]);
          st[0] = pTarget->Deviation(0);
        }

        if (st[0] < dDeviation)
        {
          dDeviation = st[0];
          best_koeff = wrc_temp;
          memcpy(pTarget->m_char[1], pTarget->m_char[0], pTarget->m_length*sizeof(double));
        }

        memcpy(pTarget->m_w, temp_w, pTarget->m_length*sizeof(double));
      }

      delete[] temp_w;
      m_nNonImprovements=0;
      for (int j=0; j<pTarget->m_length; ++j)
        pTarget->m_w[j] *= best_koeff;
    }

    if (m_pGASettings->m_bIslandModel && nIslandStructuresCount>0 && m_nNonImprovements>m_pGASettings->m_nIslandMigrationThreshold)
    {
      CRCStructure *tempStructure = pIslandStructures[--nIslandStructuresCount];
      tempStructure->Resize(SS[2]->Width(),SS[2]->Height());
      delete SS[2];          //SS[2]->CopyStructure(tempStructure);
      SS[2] = tempStructure; //delete tempStructure;
      st[2] = CalculateStructure(2);
      m_nNonImprovements=0;
    }

    ++nGACount;

    if (bIsNotBatch)
      Synchronize(Update);
    else if (nGACount >= nMaxBatchGACount)
      Terminate();
  }

  SS[1]->Resize(nOriginalSizeX, nOriginalSizeY);
  SS[2]->Resize(nOriginalSizeX, nOriginalSizeY);
  SS[3]->Resize(nOriginalSizeX, nOriginalSizeY);
  SS[4]->Resize(nOriginalSizeX, nOriginalSizeY);
}
//---------------------------------------------------------------------------

double __fastcall TThreadGA::CalculateStructure(int Index)
{
  int ExitCode = 0;
  SS[Index]->Dorabotka();

  if (pScheme == NULL || SS[Index]->GetKPQuantity() == pScheme->GetKPQuantity())
  {
    ExitCode = Form1->CalculateYParameters(SS[Index], pTarget);
  }

  if (ExitCode != -1)
  {
    if (pScheme == NULL)
    {
      mup::ParserX parser;
      parser.SetExpr(Form6->Edit3->Text.c_str());

      int i = 0;
      double *pLastValue = NULL;

      parser.DefineFun(new FunGetY(pTarget->m_MatY, &i, &SS[Index]));
      parser.DefineFun(new FunMag());
      parser.DefineFun(new FunPhase(&pLastValue));
      parser.DefineFun(new FunCond(&SS[Index]));

      for (; i < pTarget->m_length; ++i)
      {
        mup::IValue *val = parser.Eval().AsIValue();
        if (val->IsNonComplexScalar())
        {
          pTarget->m_char[Index][i] = val->GetFloat();
          pLastValue = &pTarget->m_char[Index][i];
        }
        else
        {
          throw new std::runtime_error("formula must evaluate to non-complex value");
        }
      }
    }
    else
    {
      if (pScheme->Setka(pTarget, nCharacteristic, pTarget->m_char[Index], SS[Index]))
      {
        return 100000.0;
      }
    }

    return pTarget->Deviation(Index);
  }
  
  return 100000.0;
}


void __fastcall TThreadGA::Update()
{
  Form1->StatusBar1->Panels->Items[0]->Text = " Цикл ГА: " + IntToStr(nGACount);
  Form1->StatusBar1->Panels->Items[1]->Text = " Отклонение: " + FloatToStr(dDeviation);
  if (m_pGASettings->m_bIslandModel)
    Form1->StatusBar1->Panels->Items[3]->Text = " Структур: " + IntToStr(nIslandStructuresCount);
  else
    Form1->StatusBar1->Panels->Items[3]->Text = "";

  unsigned int t = (GetTickCount()-nTime0+nTime_accumulated) / 1000;
  unsigned int hour = t/3600;
  t %= 3600;
  AnsiString sumVrem;
  sumVrem.printf(" Общее время: \%2.2u:\%2.2u:\%2.2u",hour,t/60,t%60);
  Form1->StatusBar1->Panels->Items[2]->Text = sumVrem;

  Form1->ShowStructure(SS[1], Form1->TabControl2->TabIndex, Form1->Image1);
  Form1->Graph();
}

int __fastcall TThreadGA::GetPreferableMutatePlacesFor(int Index)
{
  double *DevArray = new double[pTarget->m_length];
  int places = 0;
  // в DevArray хранятся отклонения на каждой частоте, пронормированные от 0.0 до 1.0
  // сумма всех значений DevArray равна 1.0
  if (pTarget->DeviationArray(1, DevArray)>dParam1) // отклонения берутся от первой структуры, т.к. для третей они еще не рассчитаны
  {
    ++napr;

    // создаем три матрицы
    // в массив с индексом 1 заносятся расстояния от всех ячеек до 1-й КП
    // в массив с индексом 2 заносятся расстояния от всех ячеек до 2-й КП
    // в массив с индексом 0 заносится сумма массивов 1-2
    // минимальные значения в массиве с индексом 0 указывают, что эти ячейки располагаются на минмальном растоянии от обоих КП
    // т.е. на кратчайшем путь между двумя КП
    // а максимальные значения, значит эти ячейки максимально отдалены от КП
    int ***pWave = new int**[3];
    for (int i=0; i<3; ++i)
    {
      pWave[i] = new int*[SS[Index]->Width()];
      for (int j=0; j<SS[Index]->Width(); ++j)
      {
        pWave[i][j] = new int[SS[Index]->Height()];
        memset(pWave[i][j], 0, sizeof(int)*SS[Index]->Height());
      }
    }

    int ShortWay; // длина кратчайшего пути между КП
    int LongWay; // длина самого длиного пути между КП
    SS[Index]->WaveCast(pWave, &ShortWay, &LongWay);

    int ProbLength = LongWay-ShortWay+1; // разница между длинами путей дает нам количество интервалов
    double *Prob = new double[ProbLength+1]; // добавим один лишний элемент, чтобы (int)floor(x)-ShortWay+1 не выходило за границы массива при x == LongWay
    memset(Prob, 0, sizeof(double)*ProbLength);

    /*
    double dev=0.0;
    int di=0;
    for (int i=0; i<pTarget->m_length; ++i)
      if (DevArray[i]>dev)
      {
        di = i;
        dev = DevArray[i];
      }
    int PreferWay = LongWay - (double)((LongWay-ShortWay)*di)/(double)(pTarget->m_length-1);
    */

    double x;
    for (int i=0; i<pTarget->m_length; ++i) // перебираем частоты
    {
      // значение x преобразует интервал частот на интервал путей, причем переворачивает его -
      // начало интервала частот станет станет концом интервала путей,
      // а конец интервала частот станет началом интервала путей.
      // минимальная частота будет соответствовать максимальной длине пути
      // максимальная частота будет соответствовать кратчайшему пути
      x = (double)LongWay - (double)((LongWay-ShortWay)*i)/(double)(pTarget->m_length-1);

      // массив Prob - это гистограмма вероятностей выбора места мутации. сумма всех значений равна 1.0
      // т.к. X не попадает точно в индекс, то прибавляется значение индексу слева и справа в соответствующих долях, смотря куда ближе X - к левому индексу или к правому
      // функция floor отбрасывает дробную часть положительного вещественного числа x
      Prob[(int)floor(x)-ShortWay]   += DevArray[i]*(1.0-x+floor(x));
      Prob[(int)floor(x)-ShortWay+1] += DevArray[i]*(x-floor(x));
    }

    // гистограмма преобразуется в гистограмму с накапливающейся суммой
    for (int i=0; i<ProbLength-1; ++i)
      Prob[i+1] += Prob[i];

    // создаем новый массив для гистограммы. И нормируем гистограмму от 0 до LRAND_MAX.
    int *Prob2 = new int[ProbLength];
    for (int i=0; i<ProbLength; ++i)
      Prob2[i] = Prob[i]*LRAND_MAX;
    delete[] Prob;

    // выбор места мутации производится случайно, но там где отклонение больше шанс мутации выше
    int Rand = _lrand();

    int k=0;
    while ((Prob2[k]<Rand) && (k<ProbLength))
      ++k;
    int PreferWay = ShortWay+k;
    delete[] Prob2;

    PreferableMutatePlaces = new TPoint[SS[Index]->Width()*SS[Index]->Height()];
    for (int i=0; i<SS[Index]->Width(); ++i)
      for (int j=0; j<SS[Index]->Height(); ++j)
        if (pWave[0][i][j]==PreferWay)
          PreferableMutatePlaces[places++] = TPoint(i,j);

    for (int i=0; i<3; ++i)
    {
      for (int j=0; j<SS[Index]->Width(); ++j)
        delete[] pWave[i][j];
      delete[] pWave[i];
    }
    delete[] pWave;
  }
  delete[] DevArray;
  return places;
}

