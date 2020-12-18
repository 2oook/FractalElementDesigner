using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel.Structure
{
    /// <summary>
    /// Помощник взаимодействия с библиотекой RCWorkbench 
    /// </summary>
    static class RCWorkbenchIntercommunicationHelper
    {
        // Метод для восстановления массива нумерации узлов структуры
        public static int[,,] UnflatNumerationArray(int layerCount, int horizontalRange, int verticalRange, int[] nodesNumerationFlat) 
        {
            var numeration = new int[layerCount, horizontalRange, verticalRange];

            // восстановить массив нумерации узлов
            int counter = 0;

            for (int i = 0; i < layerCount; i++)
            {
                for (int j = 0; j < horizontalRange; j++)
                {
                    for (int k = 0; k < verticalRange; k++)
                    {
                        numeration[i, j, k] = nodesNumerationFlat[counter];
                        counter++;
                    }
                }
            }

            return numeration;
        }
    }
}
