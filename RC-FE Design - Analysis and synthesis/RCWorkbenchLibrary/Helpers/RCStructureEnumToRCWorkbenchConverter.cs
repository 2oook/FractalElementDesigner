using FractalElementDesigner.FEEditing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.RCWorkbenchLibrary.Helpers
{
    /// <summary>
    /// Конвертер типа структуры
    /// </summary>
    class RCStructureEnumToRCWorkbenchConverter
    {
        public static int Convert(string structureType)
        {
            switch (structureType)
            {
                case RCStructureTypeConstants.R_C_0:
                    return 0x20304352;
                case RCStructureTypeConstants.R_CG_0:
                    return 0x30474352;
                case RCStructureTypeConstants.R_C_NR:
                    return 0x524E4352;
                case RCStructureTypeConstants.R_CG_NR:
                    return 0x4E474352;
                case RCStructureTypeConstants.R_CG_NR_plus_CP:
                    return 0x41474352;
                case RCStructureTypeConstants.R_plus_R_C_NR:
                    return 0x4E435252;
                case RCStructureTypeConstants.R_plus_R_CG_NR:
                    return 0x47435252;
                case RCStructureTypeConstants.R_plus_R_CG_NR_plus_CP:
                    return 0x48435252;
                default:
                    return -1;
            }
        }

        public static string ConvertBack(int structureTypeFromRCW)
        {
            switch (structureTypeFromRCW)
            {
                case 0x20304352:
                    return RCStructureTypeConstants.R_C_0;
                case 0x30474352:
                    return RCStructureTypeConstants.R_CG_0;
                case 0x524E4352:
                    return RCStructureTypeConstants.R_C_NR;
                case 0x4E474352:
                    return RCStructureTypeConstants.R_CG_NR;
                case 0x41474352:
                    return RCStructureTypeConstants.R_CG_NR_plus_CP;
                case 0x4E435252:
                    return RCStructureTypeConstants.R_plus_R_C_NR;
                case 0x47435252:
                    return RCStructureTypeConstants.R_plus_R_CG_NR;
                case 0x48435252:
                    return RCStructureTypeConstants.R_plus_R_CG_NR_plus_CP;
                default:
                    return "";
            }
        }
    }
}
