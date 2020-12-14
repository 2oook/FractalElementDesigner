using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.RCWorkbenchLibrary
{
    /// <summary>
    /// Точка входа в библиотеку RCWorkbenchLibrary
    /// </summary>
    public class RCWorkbenchLibraryEntry
    {
        [DllImport(@"RCWorkbenchLibrary", ExactSpelling = false, EntryPoint = "InitiateLibrary", CallingConvention = CallingConvention.StdCall)]
        public static extern void InitiateLibrary();

        [DllImport(@"RCWorkbenchLibrary", ExactSpelling = false, EntryPoint = "CreateCAnalyseParameters", CallingConvention = CallingConvention.StdCall)]
        public static extern void CreateCAnalyseParameters(int Scheme, int Characteristic, double K, double L, bool Log, int Length);

        [DllImport(@"RCWorkbenchLibrary", ExactSpelling = false, EntryPoint = "SetFrequencyRange", CallingConvention = CallingConvention.StdCall)]
        public static extern void SetFrequencyRange(double Wmin, double Wmax, int n);

        [DllImport(@"RCWorkbenchLibrary", ExactSpelling = false, EntryPoint = "CreateRCGNRStructure", CallingConvention = CallingConvention.StdCall)]
        public static extern void CreateRCGNRStructure(double R, double C, int x, int y, double Kf, double G, double H, double N);

        [DllImport(@"RCWorkbenchLibrary", ExactSpelling = false, EntryPoint = "SetElementTypeToStructureCell", CallingConvention = CallingConvention.StdCall)]
        public static extern void SetElementTypeToStructureCell(int Layer, int x, int y, int ElementType);

        [DllImport(@"RCWorkbenchLibrary", ExactSpelling = false, EntryPoint = "SetElementTypeDirectlyToStructureCell", CallingConvention = CallingConvention.StdCall)]
        public static extern void SetElementTypeDirectlyToStructureCell(int Layer, int x, int y, int ElementType);// Используется только для добавленных структур Rk и NRk

        [DllImport(@"RCWorkbenchLibrary", ExactSpelling = false, EntryPoint = "CalculateYParameters", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CalculateYParameters([In, Out] double[,] result, int first_dimention_size, int second_dimention_size);

        [DllImport(@"RCWorkbenchLibrary", ExactSpelling = false, EntryPoint = "GetCPQuantity", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetCPQuantity();

        [DllImport(@"RCWorkbenchLibrary", ExactSpelling = false, EntryPoint = "ClearCPNumbers", CallingConvention = CallingConvention.StdCall)]
        public static extern void ClearCPNumbers();

        [DllImport(@"RCWorkbenchLibrary", ExactSpelling = false, EntryPoint = "GetFrequences", CallingConvention = CallingConvention.StdCall)]
        public static extern void GetFrequences([In, Out] double[] frequences);
    }
}
