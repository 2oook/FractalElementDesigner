#pragma once

#include "TargetFunction.h"
#include "Scheme.h"
class CAnalyseParameters
{
public:
    int m_Scheme, m_Characteristic;
    double m_K, m_L;
    bool m_Log;
    CRCStructureCalculateData* m_pT;
    CScheme* m_pScheme;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="Scheme">Номер схемы</param>
    /// <param name="Characteristic">Тип характеристики</param>
    /// <param name="K">К</param>
    /// <param name="L">L</param>
    /// <param name="Log">Логарифмическая ли шкала</param>
    /// <param name="Length">Число точек</param>
    CAnalyseParameters(int Scheme, int Characteristic, double K, double L, bool Log, int Length)
    {
        m_Scheme = Scheme;
        m_Characteristic = Characteristic;
        m_K = K;
        m_L = L;
        m_Log = Log;
        m_pT = new CRCStructureCalculateData(Length);
        m_pScheme = CScheme::GetScheme(Scheme, K, L);
    }

    CAnalyseParameters(bool Log, int Length)
    {
        m_Scheme = 0;
        m_Characteristic = 0;
        m_K = 0;
        m_L = 0;
        m_Log = Log;
        m_pT = new CRCStructureCalculateData(Length);
        m_pScheme = NULL;
    }

    ~CAnalyseParameters()
    {
        delete m_pT;
        delete m_pScheme;
    }

};

