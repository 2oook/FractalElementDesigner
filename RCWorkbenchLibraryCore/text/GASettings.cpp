//---------------------------------------------------------------------------

#include <stdlib.h>
#include <mem.h>
#pragma hdrstop

#include "GASettings.h"

//---------------------------------------------------------------------------

#pragma package(smart_init)

CGASettings::CGASettings()
{
  m_nProbabilities[0] = 1503238554; // вероятность мутации проводящего слоя
  m_nProbabilities[1] = 1503238554; // вероятность мутации диэлектрического слоя
  m_nProbabilities[2] = 322122547; // вероятность изменения ширины КП
  m_nProbabilities[3] = 322122547; // вероятность смещения КП
  m_nProbabilities[4] = 1503238554; // вероятность мутации нижнего проводящего слоя (было Вероятность переупорядочивания (Перед скрещиванием))
  m_nProbabilities[5] = 0; // Вероятность переупорядочивания (Перед мутацией)
  m_nProbabilities[6] = 1717986918; // Вероятность скрещивания одинаковых слоев
  m_nProbabilities[7] = 429496730; // Вероятность скрещивания разных слоев

  m_nRestrictions[0] = 4; // максимальная площадь мутации по X
  m_nRestrictions[1] = 4; // максимальная площадь мутации по Y
  m_nRestrictions[2] = 4; // максимальная площадь скрещивания по X
  m_nRestrictions[3] = 4; // максимальная площадь скрещивания по Y
  m_nRestrictions[4] = 2; // Максимальное изменение ширины КП за один цикл
  m_nRestrictions[5] = 2; // Максимальное смещение КП за один цикл

  m_bDirectSynthesis = false;
  m_nCancelDirectSynthesisThreshold = 50u;

  m_bStructureFit = false;

  m_bIslandModel = false;
  m_nIslands = 5u;
  m_dIslandDeviation = 0.2;
  m_nIslandMigrationThreshold = 10u;

  m_bDynamicGrid = false;

  m_dNonImprovementThreshold = 0.99;

  m_AutoRegulateK.Enabled = false;
  m_AutoRegulateK.Threshold = 25u;
  m_AutoRegulateK.Low = 0.9;
  m_AutoRegulateK.High = 1.1;
  m_AutoRegulateK.Step = 0.02;

  m_AutoRegulateN.Enabled = false;
  m_AutoRegulateN.Threshold = 25u;
  m_AutoRegulateN.Low = 0.1;
  m_AutoRegulateN.High = 2.0;
  m_AutoRegulateN.Step = 0.2;

  m_AutoRegulateWRC.Enabled = true;
  m_AutoRegulateWRC.Threshold = 40u;
  m_AutoRegulateWRC.Low = 0.5;
  m_AutoRegulateWRC.High = 2.0;
  m_AutoRegulateWRC.Step = 0.1;
}

CGASettings::CGASettings(const CGASettings& S)
{
  memcpy(this, &S, sizeof(S));
}

void CGASettings::SetProbabilities(double dProbMutateCond, double dProbMutateDiel, double dProbKPResize, double dProbKPMove, double dProb1, double dProb2, double dProbCross, double dProbXCross)
{
  m_nProbabilities[0] = dProbMutateCond*LRAND_MAX;
  m_nProbabilities[1] = dProbMutateDiel*LRAND_MAX;
  m_nProbabilities[2] = dProbKPResize*LRAND_MAX;
  m_nProbabilities[3] = dProbKPMove*LRAND_MAX;
  m_nProbabilities[4] = dProb1*LRAND_MAX;
  m_nProbabilities[5] = dProb2*LRAND_MAX;
  m_nProbabilities[6] = dProbCross*LRAND_MAX;
  m_nProbabilities[7] = dProbXCross*LRAND_MAX;
}

void CGASettings::SetProbMutateR1(double prob)
{
  m_nProbabilities[0] = prob*LRAND_MAX;
}

void CGASettings::SetProbMutateR3(double prob)
{
  m_nProbabilities[4] = prob*LRAND_MAX;
}

void CGASettings::SetProbCross(double prob)
{
  m_nProbabilities[6] = prob*LRAND_MAX;
}


void CGASettings::SetRestrictions(int nMaxMutateX, int nMaxMutateY, int nMaxCrossX, int nMaxCrossY, int nMaxKPResize, int nMaxKPMove)
{
  m_nRestrictions[0] = nMaxMutateX;
  m_nRestrictions[1] = nMaxMutateY;
  m_nRestrictions[2] = nMaxCrossX;
  m_nRestrictions[3] = nMaxCrossY;
  m_nRestrictions[4] = nMaxKPResize;
  m_nRestrictions[5] = nMaxKPMove;
}

void CGASettings::SetDirectSynthesis(bool bEnabled, unsigned int nCancelThreshold)
{
  m_bDirectSynthesis = bEnabled;
  m_nCancelDirectSynthesisThreshold = nCancelThreshold;
}

void CGASettings::SetStructureFit(bool bEnabled)
{
  m_bStructureFit = bEnabled;
}

void CGASettings::SetIslandModel(bool bEnabled, unsigned int nIslands, double dIslandDeviation, unsigned int nIslandMigrationThreshold)
{
  m_bIslandModel = bEnabled;
  m_nIslands = nIslands;
  m_dIslandDeviation = dIslandDeviation;
  m_nIslandMigrationThreshold = nIslandMigrationThreshold;
}

void CGASettings::SetDynamicGrid(bool bEnabled)
{
  m_bDynamicGrid = bEnabled;
}

void CGASettings::SetNonImprovementThreshold(double m_dNonImprovementThresholdPercents)
{
  m_dNonImprovementThreshold = 1.0 - m_dNonImprovementThresholdPercents/100.0;
}

void CGASettings::SetAutoRegulateK(bool nEnabled, unsigned int nThreshold, double dLow, double dHigh, double dStep)
{
  m_AutoRegulateK.Enabled = nEnabled;
  m_AutoRegulateK.Threshold = nThreshold;
  m_AutoRegulateK.Low = dLow;
  m_AutoRegulateK.High = dHigh+dStep;
  m_AutoRegulateK.Step = dStep;
}

void CGASettings::SetAutoRegulateN(bool nEnabled, unsigned int nThreshold, double dLow, double dHigh, double dStep)
{
  m_AutoRegulateN.Enabled = nEnabled;
  m_AutoRegulateN.Threshold = nThreshold;
  m_AutoRegulateN.Low = dLow;
  m_AutoRegulateN.High = dHigh+dStep;
  m_AutoRegulateN.Step = dStep;
}

void CGASettings::SetAutoRegulateWRC(bool nEnabled, unsigned int nThreshold, double dLow, double dHigh, double dStep)
{
  m_AutoRegulateWRC.Enabled = nEnabled;
  m_AutoRegulateWRC.Threshold = nThreshold;
  m_AutoRegulateWRC.Low = dLow;
  m_AutoRegulateWRC.High = dHigh+dStep;
  m_AutoRegulateWRC.Step = dStep;
}

