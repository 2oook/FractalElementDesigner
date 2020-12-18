using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel.Structure
{
    /// <summary>
    /// Перечисление типа сегмента структуры
    /// </summary>
    public enum StructureSegmentTypeEnum
    {
        EMPTY, // вырез с обоих слоях
        R_C_NR, // нормальная структура
        Rv, // вырез снизу
        Rn, // вырез сверху
        Rk_C_NRk, // ячейка с контактом сверху и снизу
        R_C_NRk, // ячейка с контактом снизу
        Rk_C_NR // ячейка с контактом сверху
    }
}
