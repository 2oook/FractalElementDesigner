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
extern "C" RCWORKBENCH_LIBRARY_API  void __stdcall CalculateYParameters(complex<double>** result, int resultSize);
