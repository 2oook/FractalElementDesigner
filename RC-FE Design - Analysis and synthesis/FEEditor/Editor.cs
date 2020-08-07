using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditor
{
    /// <summary>
    /// Редактор
    /// </summary>
    [Serializable]
    public class Editor
    {
        #region Properties

        /// <summary>
        /// Контекст редактора
        /// </summary>
        public Context Context { get; set; }

        #endregion
    }
}
