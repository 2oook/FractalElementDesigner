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


// ��������� �������� ������
MainClass* mainClassInstance = NULL;

extern void __stdcall InitiateLibrary()
{
    mainClassInstance = new MainClass();
};

extern void __stdcall CreateCAnalyseParameters(int Scheme, int Characteristic, double K, double L, bool Log, int Length)
{
    mainClassInstance->pAnalyseParameters = new CAnalyseParameters(Scheme, Characteristic, K, L, Log, Length);
};

extern void __stdcall SetFrequencyRange(double Wmin, double Wmax, int n)
{
    double dw;

    // ������������� ������
    if (/*���� ������� ����� ��������� �������������*/true)// ������ �������
        dw = (Wmax - Wmin) / (n - 1);
    else if (/*���� ������� ����� ���������������� �������������*/false)
        dw = (log10(Wmax) - log10(Wmin)) / (n - 1);

    for (int i = 0; i < n; ++i)
    {
        if (/*���� ������� ����� ��������� �������������*/true)
            mainClassInstance->pAnalyseParameters->m_pT->m_w[i] = Wmin + dw * i;
        else if (/*���� ������� ����� ���������������� �������������*/false)
            mainClassInstance->pAnalyseParameters->m_pT->m_w[i] = pow(10.0, log10(Wmin) + dw * i);
    }
};

extern void __stdcall CreateRCGNRStructure(double R, double C, int x, int y, double Kf, double G, double H, double N)
{
    // ������� ���������
    mainClassInstance->Structure5 = new CRCGNRStructure(R, C, x, y, Kf, G, H, N);
};

extern void __stdcall SetElementTypeToStructureCell(int Layer, int x, int y, int ElementType)
{
    // ���������� ��� ������
    mainClassInstance->Structure5->SetElementType(Layer, x, y, (EnumRCElementType)ElementType);
};

extern void __stdcall SetElementTypeDirectlyToStructureCell(int Layer, int x, int y, int ElementType)
{
    // ���������� ��� ������ ��������
    mainClassInstance->Structure5->SetElementTypeDirectly(Layer, x, y, (EnumRCElementType)ElementType);
};

extern void __cdecl CalculateYParameters(double result[][SECOND_DIMENSION_FOR_8_CONTACTS], int first_dimention_size, int second_dimention_size)
{
    mainClassInstance->pAnalyseParameters->m_pT->y_result = result;

    mainClassInstance->ExitCode = mainClassInstance->CalculateYParameters(mainClassInstance->Structure5, mainClassInstance->pAnalyseParameters->m_pT);
};

extern int __stdcall GetCPQuantity()
{
    return mainClassInstance->Structure5->GetKPQuantity();
};

extern void __stdcall ClearCPNumbers()
{
    mainClassInstance->Structure5->ClearKPNumbers();
};

extern void __stdcall GetFrequences(double* frequences)
{
    for (size_t i = 0; i < mainClassInstance->pAnalyseParameters->m_pT->m_length; i++)
    {
        frequences[i] = mainClassInstance->pAnalyseParameters->m_pT->m_w[i];
    }
};

extern void __stdcall GetNodesNumeration(int* numeration)
{  
    mainClassInstance->Structure5->ElementsToNodes();

    int counter = 0;

    for (size_t i = 0; i < mainClassInstance->Structure5->m_Layers; i++)
    {
        for (size_t j = 0; j < mainClassInstance->Structure5->m_x + 1; j++)
        {
            for (size_t k = 0; k < mainClassInstance->Structure5->m_y + 1; k++)
            {
                numeration[counter] = mainClassInstance->Structure5->m_pCircuitNodes[i][j][k];    
                counter++;
            }                    
        }
    }
};

extern int __stdcall GetNodesQuantity() 
{
    return mainClassInstance->Structure5->m_Nodes;
};