#include "pch.h"
#include <cmath>

#include "main_class.h"
#include "rc_workbench_library.h"

#include "..\rcworkbenchlibrarycore\code\Scheme.h"
#include "..\rcworkbenchlibrarycore\code\TargetFunction.h"
#include "..\rcworkbenchlibrarycore\code\CAnalyseParameters.h"
#include "..\rcworkbenchlibrarycore\code\RCStructure.h"
#include "..\rcworkbenchlibrarycore\code\RC0Structure.h"
#include "..\rcworkbenchlibrarycore\code\RCG0Structure.h"
#include "..\rcworkbenchlibrarycore\code\RCGNRAStructure.h"
#include "..\rcworkbenchlibrarycore\code\RCGNRStructure.h"
#include "..\rcworkbenchlibrarycore\code\RCNRStructure.h"


// экземпляр главного класса
MainClass* mainClassInstance = NULL;

extern void __stdcall InitiateLibrary()
{
    mainClassInstance = new MainClass();
};

extern void __stdcall CreateCAnalyseParameters(int Scheme, int Characteristic, double K, double L, bool Log, int Length)
{
    // симулировать pAnalyseParameters // СХЕМА №3
    mainClassInstance->pAnalyseParameters = new CAnalyseParameters(Scheme, Characteristic, K, L, Log, Length);
};

extern void __stdcall SetFrequencyRange(double Wmin, double Wmax, int n)
{
    double dw;

    // инициализация частот
    if (/*если включен режим линейного распределения*/true)// всегда включен
        dw = (Wmax - Wmin) / (n - 1);
    else if (/*если включен режим логарифмического распределения*/false)
        dw = (log10(Wmax) - log10(Wmin)) / (n - 1);

    for (int i = 0; i < n; ++i)
    {
        if (/*если включен режим линейного распределения*/true)
            mainClassInstance->pAnalyseParameters->m_pT->m_w[i] = Wmin + dw * i;
        else if (/*если включен режим логарифмического распределения*/false)
            mainClassInstance->pAnalyseParameters->m_pT->m_w[i] = pow(10.0, log10(Wmin) + dw * i);
    }
};

extern void __stdcall CreateRCGNRStructure(double R, double C, int x, int y, double Kf, double G, double H, double N)
{
    // создать структуру
    mainClassInstance->Structure5 = new CRCGNRStructure(R, C, x, y, Kf, G, H, N);
};

extern void __stdcall SetElementTypeToStructureCell(int Layer, int x, int y, int ElementType)
{
    // установить тип ячейки
    mainClassInstance->Structure5->SetElementType(Layer, x, y, (EnumRCElementType)ElementType);
};

extern void __cdecl CalculateYParameters(double result[][SECOND_DIMENSION_FOR_8_CONTACTS], int first_dimention_size, int second_dimention_size)
{
    mainClassInstance->pAnalyseParameters->m_pT = new CRCStructureCalculateData(first_dimention_size, result);

    mainClassInstance->ExitCode = mainClassInstance->CalculateYParameters(mainClassInstance->Structure5, mainClassInstance->pAnalyseParameters->m_pT);
};