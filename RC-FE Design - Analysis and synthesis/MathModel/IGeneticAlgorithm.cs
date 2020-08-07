using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel
{
    /// <summary>
    /// Интерфейс генетического алгоритма
    /// </summary>
    interface IGeneticAlgorithm
    {
        void MutatePopulation();

        void InitiatePopulation(FElementScheme schemePrototype);
         
        void InitiateIndividual(FESchemeModel model);

        FElementScheme Cross(FElementScheme first, FElementScheme second);

        void CrossPopulation();

        void Fit(FESchemeModel model);

        void RatePopulation();

        void SelectPopulation();

        List<FElementScheme> GetPopulation();

        void Start();
    }
}
