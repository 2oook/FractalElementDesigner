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
        // Метод для мутации популяции
        void MutatePopulation();

        // Метод для инициализации популяции
        void InitiatePopulation(FElementScheme schemePrototype);

        // Метод для инициализации индивида
        void InitiateIndividual(FESchemeModel model);

        // Метод для скрещивания двух схем
        FElementScheme Cross(FElementScheme first, FElementScheme second);

        // Метод для скрещивания популяции
        void CrossPopulation();

        // Функция соответствия модели схемы заданным параметрам
        void Fit(FESchemeModel model);

        // Метод для оценки популяции
        void RatePopulation();

        // Метод для отбора популяции
        void SelectPopulation();

        // Метод для получения популяции
        List<FElementScheme> GetPopulation();

        // Метод для старта алгоритма
        void Start();
    }
}
