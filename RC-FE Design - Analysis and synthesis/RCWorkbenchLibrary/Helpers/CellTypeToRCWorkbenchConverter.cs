﻿using FractalElementDesigner.FEEditing.Model.StructureElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.RCWorkbenchLibrary.Helpers
{
    /// <summary>
    /// Конвертер типа ячейки для интеграции с RCWorkbench
    /// </summary>
    class CellTypeToRCWorkbenchConverter
    {
        public static int Convert(CellType cellType) 
        {
            switch (cellType)
            {
                case CellType.None:
                    return -1;
                case CellType.PlaceForContact:
                    throw new NotImplementedException();
                case CellType.Contact:
                    return 8;
                case CellType.Cut:
                    return 0;
                case CellType.Forbid:
                    return 16;
                case CellType.RC:
                    return 3;
                case CellType.R:
                    return 1;
                case CellType.Rk:
                    return 104;
                case CellType.NRk:
                    return 160;
                case CellType.Shunt:
                    return 24;
                default:
                    throw new Exception();
            }
        }

        public static CellType ConvertBack(int cellTypeFromRCW)
        {
            switch (cellTypeFromRCW)
            {
                case -1:
                    return CellType.None;
                case 0:
                    return CellType.Cut;
                case 1:
                    return CellType.R;
                case 3:
                    return CellType.RC;
                case 8:
                    return CellType.Contact;
                case 104:
                    return CellType.Rk;
                case 160:
                    return CellType.NRk;
                case 16:
                    return CellType.Forbid;
                case 24:
                    return CellType.Shunt;
                case 256:
                    throw new NotImplementedException();
                case 257:
                    throw new NotImplementedException();
                case 259:
                    throw new NotImplementedException();
                default:
                    throw new Exception();
            }
        }
    }
}
