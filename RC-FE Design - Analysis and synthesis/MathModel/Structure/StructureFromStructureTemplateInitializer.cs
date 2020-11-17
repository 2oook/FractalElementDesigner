using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.StructureSchemeSynthesis;
using FractalElementDesigner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel.Structure
{
    /// <summary>
    /// Класс инициализатора структуры из шаблона структуры
    /// </summary>
    class StructureFromStructureTemplateInitializer
    {
        public static void Initialize(RCStructureTemplate structureTemplate, RCStructure structure, Dictionary<string, StructurePropertyForValidation> structureProperties, 
            StructureSchemeSynthesisParameters parameters) 
        {
            // добавить слои в соответствии с шаблоном
            foreach (var layer in structureTemplate.StructureLayers)
            {
                structure.StructureLayers.Add(new Layer() { Name = layer.Name, Number = layer.Number, ParentStructure = structure });
            }

            // скопировать значения из словаря для валидации в словарь свойств структуры
            foreach (var property in structure.StructureProperties.Values)
            {
                property.Value = double.Parse(structureProperties[property.Name].Value);
            }

            structure.SynthesisParameters = parameters;
        }
    }
}
