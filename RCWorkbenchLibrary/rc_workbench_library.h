#pragma once

#ifdef RCWORKBENCH_LIBRARY_EXPORTS
#define RCWORKBENCH_LIBRARY_API __declspec(dllexport)
#else
#define RCWORKBENCH_LIBRARY_API __declspec(dllimport)
#endif

extern "C" RCWORKBENCH_LIBRARY_API  void __stdcall InitiateLibrary();
extern "C" RCWORKBENCH_LIBRARY_API  void __stdcall CreateCAnalyseParameters(int Scheme, int Characteristic, double K, double L, bool Log, int Length);
extern "C" RCWORKBENCH_LIBRARY_API  void __stdcall SetFrequencyRange(double Wmin, double Wmax, int n);
extern "C" RCWORKBENCH_LIBRARY_API  void __stdcall CreateRCGNRStructure(double R, double C, int x, int y, double Kf, double G, double H, double N);
extern "C" RCWORKBENCH_LIBRARY_API  void __stdcall SetElementTypeToStructureCell(int Layer, int x, int y, int ElementType);
extern "C" RCWORKBENCH_LIBRARY_API  void __stdcall SetElementTypeDirectlyToStructureCell(int Layer, int x, int y, int ElementType);
extern "C" RCWORKBENCH_LIBRARY_API  void __cdecl CalculateYParameters(double result[][SECOND_DIMENSION_FOR_8_CONTACTS], int first_dimention_size, int second_dimention_size);
extern "C" RCWORKBENCH_LIBRARY_API  int __stdcall GetCPQuantity();
extern "C" RCWORKBENCH_LIBRARY_API  void __stdcall ClearCPNumbers();
extern "C" RCWORKBENCH_LIBRARY_API  void __stdcall GetFrequences(double* frequences);
extern "C" RCWORKBENCH_LIBRARY_API  void __stdcall GetNodesNumeration(int* numeration);
extern "C" RCWORKBENCH_LIBRARY_API  void __stdcall CreateRCGNRStructureStraight(double R, double C, int x, int y, double Kf, double G, double H, double N);
extern "C" RCWORKBENCH_LIBRARY_API  void __stdcall DeleteStructureStraight();
extern "C" RCWORKBENCH_LIBRARY_API  int __stdcall GetNodesQuantity();
