using System;
using System.Collections.Generic;
using System.Linq;
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
        [DllImport(@"RCWorkbenchLibrary", ExactSpelling = false, EntryPoint = "TestMeth", CallingConvention = CallingConvention.StdCall)]
        public static extern int TestMeth();
    }
}
