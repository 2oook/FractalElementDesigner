﻿using RC_FE_Design___Analysis_and_synthesis.StructureSchemeSynthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    /// <summary>
    /// Класс реализующий синтез схемы включения элемента
    /// </summary>
    public class SchemeSynthesizer
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
        public static FElementScheme Synthesize(StructureSchemeSynthesisParameters synthesisParameters) 
        {
            var scheme = new FElementScheme() { Name = "Схема включения" };

            OnStateChange("Выполнение синтеза");

            // для отладки
            // для отладки
            // для отладки
            for (int i = 0; i < 100; i++)
            {                
                Thread.Sleep(10);
                OnDoWork(i+1);
            }
            // для отладки
            // для отладки
            // для отладки

            OnStateChange("");

            return scheme;
        }
    }
}
