using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.FEEditing.Model.StructureElements;
using FractalElementDesigner.MathModel;
using FractalElementDesigner.MathModel.Structure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditing.Model
{
    /// <summary>
    /// Класс констант названий слоёв структуры
    /// </summary>
    static class RCStructureLayerTypeConstants
    {
        public const string R_C = "R-C";
        public const string R_CG = "R-CG";
        public const string NR = "NR";

        public const int R_C_number = 1;
        public const int R_CG_number = 0;
        public const int NR_number = 1;
    }

    /// <summary>
    /// Класс структуры
    /// </summary>
    [Serializable]
    class RCStructure : RCStructureBase, IRCStructurePrototype
    {
        /// <summary>
        /// Статический конструктор класса структуры
        /// </summary>
        static RCStructure()
        {
            // определить шаблоны доступных для создания структур
            AllTemplateStructures = new Dictionary<string, RCStructureTemplate>();

            var commonProperties = new Dictionary<string, StructureProperty>()
            {
                {
                    "R0",
                    new StructureProperty()
                    {
                        Name = "Удельное сопротивление ro",
                        Value = 1.0
                    }
                },
                {
                    "C",
                    new StructureProperty()
                    {
                        Name = "Полная емкость структуры C",
                        Value = 1.0
                    }
                },
                {
                    "HorizontalCellsCount",
                    new StructureProperty()
                    {
                        Name = "Количество ячеек по горизонтали",
                        Value = 10
                    }
                },
                {
                    "VerticalCellsCount",
                    new StructureProperty()
                    {
                        Name = "Количество ячеек по вертикали",
                        Value = 10
                    }
                }
            };

            var G_H_Properties = new Dictionary<string, StructureProperty>()
            {
                {
                    "G",
                    new StructureProperty()
                    {
                        Name = "Коэффициент последовательного сопротивления G",
                        Value = 0.001
                    }
                },
                {
                    "H",
                    new StructureProperty()
                    {
                        Name = "Коэффициент параллельного сопротивления H",
                        Value = 100.0
                    }
                }
            };

            var N_Property = new Dictionary<string, StructureProperty>()
            {
                {
                    "N",
                    new StructureProperty()
                    {
                        Name = "Коэффициент N",
                        Value = 0.218
                    }
                }
            };

            var P_Property = new Dictionary<string, StructureProperty>()
            {
                {
                    "P",
                    new StructureProperty()
                    {
                        Name = "Коэффициент сопротивления КП P",
                        Value = 0.01
                    }
                }
            };

            var Proportions_Property = new Dictionary<string, StructureProperty>()
            {
                {
                    "Proportions",
                    new StructureProperty()
                    {
                        Name = "Пропорции частей",
                        Value = 0.5
                    }
                }
            };

            var R_C_0 = new RCStructureTemplate()
            {
                Name = RCStructureTypeConstants.R_C_0,
                StructureProperties = commonProperties,
                StructureLayers = new ObservableCollection<Layer>() { 
                    new Layer() { Name = RCStructureLayerTypeConstants.R_C, Number = RCStructureLayerTypeConstants.R_C_number } }
            };

            AllTemplateStructures.Add(R_C_0.Name, R_C_0);

            var R_CG_0 = new RCStructureTemplate()
            {
                Name = RCStructureTypeConstants.R_CG_0,
                StructureProperties = commonProperties.Concat(N_Property).ToDictionary(x => x.Key, x => x.Value),
                StructureLayers = new ObservableCollection<Layer>() { 
                    new Layer() { Name = RCStructureLayerTypeConstants.R_CG, Number = RCStructureLayerTypeConstants.R_CG_number } }
            };

            AllTemplateStructures.Add(R_CG_0.Name, R_CG_0);

            var R_C_NR = new RCStructureTemplate()
            {
                Name = RCStructureTypeConstants.R_C_NR,
                StructureProperties = commonProperties.Concat(N_Property).ToDictionary(x => x.Key, x => x.Value),
                StructureLayers = new ObservableCollection<Layer>() { 
                    new Layer() { Name = RCStructureLayerTypeConstants.R_C, Number = RCStructureLayerTypeConstants.R_C_number },
                    new Layer() { Name = RCStructureLayerTypeConstants.NR, Number = RCStructureLayerTypeConstants.NR_number } }
            };

            AllTemplateStructures.Add(R_C_NR.Name, R_C_NR);

            var R_CG_NR = new RCStructureTemplate()
            {
                Name = RCStructureTypeConstants.R_CG_NR,
                StructureProperties = commonProperties.Concat(G_H_Properties).Concat(N_Property).ToDictionary(x => x.Key, x => x.Value),
                StructureLayers = new ObservableCollection<Layer>() { 
                    new Layer() { Name = RCStructureLayerTypeConstants.R_CG, Number = RCStructureLayerTypeConstants.R_CG_number}, 
                    new Layer() { Name = RCStructureLayerTypeConstants.NR, Number = RCStructureLayerTypeConstants.NR_number } }
            };

            AllTemplateStructures.Add(R_CG_NR.Name, R_CG_NR);

            var R_CG_NR_plus_CP = new RCStructureTemplate()
            {
                Name = RCStructureTypeConstants.R_CG_NR_plus_CP,
                StructureProperties = commonProperties.Concat(G_H_Properties).Concat(N_Property).Concat(P_Property).ToDictionary(x => x.Key, x => x.Value),
                StructureLayers = new ObservableCollection<Layer>() { 
                    new Layer() { Name = RCStructureLayerTypeConstants.R_CG, Number = RCStructureLayerTypeConstants.R_CG_number }, 
                    new Layer() { Name = RCStructureLayerTypeConstants.NR, Number = RCStructureLayerTypeConstants.NR_number } }
            };

            AllTemplateStructures.Add(R_CG_NR_plus_CP.Name, R_CG_NR_plus_CP);

            var R_plus_R_C_NR = new RCStructureTemplate()
            {
                Name = RCStructureTypeConstants.R_plus_R_C_NR,
                StructureProperties = commonProperties.Concat(N_Property).Concat(Proportions_Property).ToDictionary(x => x.Key, x => x.Value),
                StructureLayers = new ObservableCollection<Layer>() { 
                    new Layer() { Name = RCStructureLayerTypeConstants.R_C, Number = RCStructureLayerTypeConstants.R_C_number }, 
                    new Layer() { Name = RCStructureLayerTypeConstants.NR, Number = RCStructureLayerTypeConstants.NR_number } }
            };

            AllTemplateStructures.Add(R_plus_R_C_NR.Name, R_plus_R_C_NR);

            var R_plus_R_CG_NR = new RCStructureTemplate()
            {
                Name = RCStructureTypeConstants.R_plus_R_CG_NR,
                StructureProperties = commonProperties.Concat(G_H_Properties).Concat(N_Property).Concat(Proportions_Property).ToDictionary(x => x.Key, x => x.Value),
                StructureLayers = new ObservableCollection<Layer>() { 
                    new Layer() { Name = RCStructureLayerTypeConstants.R_CG, Number = RCStructureLayerTypeConstants.R_CG_number }, 
                    new Layer() { Name = RCStructureLayerTypeConstants.NR, Number = RCStructureLayerTypeConstants.NR_number } }
            };

            AllTemplateStructures.Add(R_plus_R_CG_NR.Name, R_plus_R_CG_NR);

            var R_plus_R_CG_NR_plus_CP = new RCStructureTemplate()
            {
                Name = RCStructureTypeConstants.R_plus_R_CG_NR_plus_CP,
                StructureProperties = commonProperties.Concat(G_H_Properties).Concat(N_Property).Concat(P_Property).Concat(Proportions_Property).ToDictionary(x => x.Key, x => x.Value),
                StructureLayers = new ObservableCollection<Layer>() { 
                    new Layer() { Name = RCStructureLayerTypeConstants.R_CG, Number = RCStructureLayerTypeConstants.R_CG_number }, 
                    new Layer() { Name = RCStructureLayerTypeConstants.NR, Number = RCStructureLayerTypeConstants.NR_number } }
            };

            AllTemplateStructures.Add(R_plus_R_CG_NR_plus_CP.Name, R_plus_R_CG_NR_plus_CP);
        }

        /// <summary>
        /// Конструктор экземпляра структуры
        /// </summary>
        /// <param name="structureName">Название структуры</param>
        public RCStructure(string structureName)
        {
            Name = structureName;

            var template = AllTemplateStructures.SingleOrDefault(x => x.Key == structureName).Value;

            if (template == null)
            {
                throw new Exception("Не удается найти подходящего шаблона, невозможно создать объект структуры");
            }

            InitializeInstanceProperties(template);
        }

        /// <summary>
        /// Словарь шаблонов структур
        /// </summary>
        public static Dictionary<string, RCStructureTemplate> AllTemplateStructures;

        /// <summary>
        /// Словарь свойств структуры
        /// </summary>
        [field: NonSerialized]
        public Dictionary<string, StructureProperty> StructureProperties { get; set; } = new Dictionary<string, StructureProperty>();

        /// <summary>
        /// Информация об особи
        /// </summary>
        public StateOfIndividual StateInGA { get; set; } = new StateOfIndividual();

        /// <summary>
        /// Метод для инициализации свойств структуры в соответствии с шаблоном
        /// </summary>
        /// <param name="template">Шаблон структуры</param>
        private void InitializeInstanceProperties(RCStructureTemplate template) 
        {
            foreach (var property in template.StructureProperties)
            {
                var temp = new StructureProperty()
                {
                    Name = property.Value.Name,
                    Value = property.Value.Value
                };

                StructureProperties.Add(property.Key, temp);
            }
        }

        // Метод для клонирования модели схемы
        public IRCStructurePrototype DeepClone()
        {
            RCStructure structure;

            var formatter = new BinaryFormatter();

            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                structure = formatter.Deserialize(stream) as RCStructure;

                if (structure == null)
                {
                    throw new Exception("Ошибка сериализации объекта конструкции!");
                }
            }

            return structure;
        }
    }

    /// <summary>
    /// Класс свойства структуры
    /// </summary>
    public class StructureProperty
    {
        /// <summary>
        /// Название свойства
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Значение свойства
        /// </summary>
        public double Value { get; set; }
    }
}
