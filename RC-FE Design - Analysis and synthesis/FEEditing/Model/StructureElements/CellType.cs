using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditing.Model.StructureElements
{
    /// <summary>
    /// Перечисление тип ячейки
    /// </summary>
    [Flags]
    public enum CellType
    {
        None = 0,// ячейка без назначения
        PlaceForContact = 1,// место для расположения КП или шунта
        Contact = 3,// КП
        Forbid = 7,// запрет КП
        Shunt = 15,// шунт 
        Cut = 2,// вырез (емкость)
        RC = 6,// верхняя резистивная площадка
        R = 14,// нижняя резистивная площадка
        Rk = 8,// верхняя резистивная площадка с контактом
        NRk = 10// нижняя резистивная площадка с контактом
    }
}
