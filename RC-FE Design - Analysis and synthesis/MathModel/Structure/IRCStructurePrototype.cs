using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel.Structure
{
    /// <summary>
    /// Интерфейс прототипа конструкции
    /// </summary>
    interface IRCStructurePrototype
    {
        IRCStructurePrototype DeepClone();
    }
}
