using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.FEEditor.Model
{
    public class RCStructureBase : ViewModelBase
    {

        /// <summary>
        /// Словарь слоёв структуры
        /// </summary>
        public Dictionary<string, Layer> StructureLayers { get; set; } = new Dictionary<string, Layer>();
    }
}
