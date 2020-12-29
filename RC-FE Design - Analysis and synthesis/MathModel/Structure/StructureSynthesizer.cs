using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.StructureSchemeSynthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel.Structure
{
    /// <summary>
    /// Класс реализующий синтез конструкции элемента
    /// </summary>
    class StructureSynthesizer
    {
        /// <summary>
        /// Событие выполнения части работы
        /// </summary>
        public static event Action<double> OnDoWork;

        /// <summary>
        /// Событие изменения статуса выполнения процесса
        /// </summary>
        public static event Action<string> OnStateChange;

        // Метод для синтезирования схемы включения элемента
        public static List<RCStructure> Synthesize(StructureSchemeSynthesisParameters synthesisParameters, RCStructure structure)
        {
            OnStateChange("Выполнение синтеза");

            var ga = new StructureGeneticAlgorithm(4, synthesisParameters, structure);

            StructureGeneticAlgorithm.OnDoWork += GeneticAlgorithm_OnDoWork;

            ga.Start();

            OnStateChange("");

            return ga.GetPopulation();
        }

        private static void GeneticAlgorithm_OnDoWork(double obj)
        {
            OnDoWork(obj);
        }
    }
}
