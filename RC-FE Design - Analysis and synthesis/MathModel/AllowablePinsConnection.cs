﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel
{
    /// <summary>
    /// Класс представляет допустимое соединение и заземление выводов двух БКЭ
    /// </summary>
    class AllowablePinsConnection
    {
        /// <summary>
        /// Матрица соединений
        /// </summary>
        public int[,] ConnectionMatrix { get; set; }

        /// <summary>
        /// Допустимые варианты заземления выводов
        /// </summary>
        public Dictionary<int, int[]> PEVector { get; set; }
    }
}
