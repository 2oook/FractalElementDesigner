//---------------------------------------------------------------------------

#ifndef GASettingsH
#define GASettingsH

class CGASettings
{
friend class TThreadGA;

public:
  struct CAutoRegulation
  {
    bool Enabled;
    unsigned int Threshold;
    double Low, High, Step;
  };

protected:
  int m_nProbabilities[8];
  int m_nRestrictions[6];
  bool m_bDirectSynthesis;
  unsigned int m_nCancelDirectSynthesisThreshold;
  bool m_bStructureFit;
  bool m_bIslandModel;
  unsigned int m_nIslands;
  double m_dIslandDeviation;
  unsigned int m_nIslandMigrationThreshold;
  bool m_bDynamicGrid;
  double m_dNonImprovementThreshold;
  CAutoRegulation m_AutoRegulateK;
  CAutoRegulation m_AutoRegulateN;
  CAutoRegulation m_AutoRegulateWRC;

public:
  CGASettings();
  CGASettings(const CGASettings& S);
  ~CGASettings() {}

  void SetProbabilities(double dProbMutateCond, double dProbMutateDiel, double dProbKPResize, double dProbKPMove, double dProb1, double dProb2, double dProbCross, double dProbXCross);
  void SetProbMutateR1(double prob);
  void SetProbMutateR3(double prob);
  void SetProbCross(double prob);
  void SetRestrictions(int nMaxMutateX, int nMaxMutateY, int nMaxCrossX, int nMaxCrossY, int nMaxKPResize, int nMaxKPMove);
  void SetDirectSynthesis(bool bEnabled, unsigned int nCancelThreshold);
  void SetStructureFit(bool bEnabled);
  void SetIslandModel(bool bEnabled, unsigned int nIslands, double dIslandDeviation, unsigned int nIslandMigrationThreshold);
  void SetIslandModel(bool bEnabled);
  void SetDynamicGrid(bool bEnabled);
  void SetNonImprovementThreshold(double m_dNonImprovementThresholdPercents);
  void SetAutoRegulateK(bool nEnabled, unsigned int nThreshold, double dLow, double dHigh, double dStep);
  void SetAutoRegulateN(bool nEnabled, unsigned int nThreshold, double dLow, double dHigh, double dStep);
  void SetAutoRegulateWRC(bool nEnabled, unsigned int nThreshold, double dLow, double dHigh, double dStep);

};

//---------------------------------------------------------------------------
#endif
