using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
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
    }
}
