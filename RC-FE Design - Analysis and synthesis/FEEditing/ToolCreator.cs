using FractalElementDesigner.FEEditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditing
{
    class ToolCreator : IToolCreator
    {
        public IEditingTool CreateTool(ToolType toolType) 
        {
            switch (toolType)
            {
                case ToolType.ContactNumerator:
                    return new Tool()
                    {
                        Name = "Нумерация КП",
                        ImageURI = "pack://application:,,,/Resources/button0.png",
                        Type = ToolType.ContactNumerator
                    };
                case ToolType.CutCellDisposer:
                    return new Tool()
                    {
                        Name = "Разрез",
                        ImageURI = "pack://application:,,,/Resources/button1.png",
                        Type = ToolType.CutCellDisposer
                    };
                case ToolType.ContactCellDisposer:
                    return new Tool()
                    {
                        Name = "Контактная площадка",
                        ImageURI = "pack://application:,,,/Resources/button4.png",
                        Type = ToolType.ContactCellDisposer
                    };
                case ToolType.ForbidContactDisposer:
                    return new Tool()
                    {
                        Name = "Запрет КП",
                        ImageURI = "pack://application:,,,/Resources/button5.png",
                        Type = ToolType.ForbidContactDisposer
                    };
                case ToolType.RCCellDisposer:
                    return new Tool()
                    {
                        Name = "RC-ячейка",
                        ImageURI = "pack://application:,,,/Resources/button2.png",
                        Type = ToolType.RCCellDisposer
                    };
                case ToolType.RCellDisposer:
                    return new Tool()
                    {
                        Name = "R-ячейка",
                        ImageURI = "pack://application:,,,/Resources/button3.png",
                        Type = ToolType.RCellDisposer
                    };
                case ToolType.ShuntCellDisposer:
                    return new Tool()
                    {
                        Name = "Шунт",
                        ImageURI = "pack://application:,,,/Resources/button6.png",
                        Type = ToolType.ShuntCellDisposer
                    };
                default:
                    return null;
            }
        }
    }
}
