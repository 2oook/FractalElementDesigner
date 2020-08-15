using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditing.Model.Cells
{
    /// <summary>
    /// Базовый класс ячейки структуры
    /// </summary>
    class StructureCellBase : INotifyPropertyChanged
    {
        private CellType cellType = CellType.None;
        /// <summary>
        /// Тип ячейки структуры
        /// </summary>
        public CellType CellType 
        { 
            get => cellType;
            set 
            {
                cellType = value;
                RaisePropertyChanged(nameof(CellType));
            }
        }

        /// <summary>
        /// Метод для применения инструмента
        /// </summary>
        public void ApplyTool(IEditingTool tool)
        {
            var cell = this;

            if (tool != null && tool.IsChecked == true)
            {
                if (!CheckToolApplyPossibility(tool, cell))
                {
                    return;
                }

                switch (tool.Type)
                {
                    case ToolType.None:
                        cell.CellType = CellType.None;
                        break;
                    case ToolType.ContactNumerator:

                        break;
                    case ToolType.CutCellDisposer:
                        cell.CellType = CellType.Cut;
                        break;
                    case ToolType.ContactCellDisposer:
                        cell.CellType = CellType.Contact;
                        break;
                    case ToolType.ForbidContactDisposer:
                        cell.CellType = CellType.Forbid;
                        break;
                    case ToolType.RCCellDisposer:
                        cell.CellType = CellType.RC;
                        break;
                    case ToolType.RCellDisposer:
                        cell.CellType = CellType.R;
                        break;
                    case ToolType.ShuntCellDisposer:
                        cell.CellType = CellType.Shunt;
                        break;
                    default:
                        break;
                }
            }
        }

        // метод для проверки возможности применения инструмента к ячейке
        private static bool CheckToolApplyPossibility(IEditingTool tool, StructureCellBase cell)
        {
            var result = true;

            switch (tool.Type)
            {
                case ToolType.None:
                    break;
                case ToolType.ContactNumerator:
                    if (cell.CellType != CellType.Contact) result = false;
                    break;
                case ToolType.CutCellDisposer:
                    if (cell.CellType != CellType.R && cell.CellType != CellType.RC) result = false;
                    break;
                case ToolType.ContactCellDisposer:
                    if (cell.CellType != CellType.PlaceForContact && cell.CellType != CellType.Forbid && cell.CellType != CellType.Shunt) result = false;
                    break;
                case ToolType.ForbidContactDisposer:
                    if (cell.CellType != CellType.PlaceForContact && cell.CellType != CellType.Contact && cell.CellType != CellType.Shunt) result = false;
                    break;
                case ToolType.RCCellDisposer:
                    if (cell.CellType != CellType.R && cell.CellType != CellType.Cut) result = false;
                    break;
                case ToolType.RCellDisposer:
                    if (cell.CellType != CellType.RC && cell.CellType != CellType.Cut) result = false;
                    break;
                case ToolType.ShuntCellDisposer:
                    if (cell.CellType != CellType.PlaceForContact && cell.CellType != CellType.Forbid && cell.CellType != CellType.Contact) result = false;
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Метод для поднятия события изменения свойства
        /// </summary>
        /// <param name="propName">Имя свойства</param>
        protected virtual void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
