#pragma once

#ifdef RCWORKBENCH_LIBRARY_EXPORTS
#define RCWORKBENCH_LIBRARY_API __declspec(dllexport)
#else
#define RCWORKBENCH_LIBRARY_API __declspec(dllimport)
#endif

extern "C" RCWORKBENCH_LIBRARY_API  int __stdcall TestMeth();