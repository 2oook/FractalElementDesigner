using FractalElementDesigner.MathModel.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditing.Model.StructureElements
{
    /// <summary>
    /// Ячейка
    /// </summary>
    class Cell : INotifyPropertyChanged
    {
        /// <summary>
        /// Выводы ячейки
        /// </summary>
        public List<Pin> Pins = new List<Pin>();

        /// <summary>
        /// Сегмент содержащий ячейку
        /// </summary>
        public SegmentOfTheStructure MainCell { get; set; }

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
                    case ToolType.ContactCellDisposer:
                        if (cell.CellType == CellType.Contact)
                        {
                            cell.CellType = CellType.PlaceForContact;
                        }
                        else if ((cell.CellType & CellType.PlaceForContact) == CellType.PlaceForContact)
                        {
                            cell.CellType = CellType.Contact;
                        }
                        break;
                    case ToolType.ForbidContactDisposer:
                        if (cell.CellType == CellType.Forbid)
                        {
                            cell.CellType = CellType.PlaceForContact;
                        }
                        else if ((cell.CellType & CellType.PlaceForContact) == CellType.PlaceForContact)
                        {
                            cell.CellType = CellType.Forbid;
                        }
                        break;
                    case ToolType.ShuntCellDisposer:
                        if (cell.CellType == CellType.Shunt)
                        {
                            cell.CellType = CellType.PlaceForContact;
                        }
                        else if ((cell.CellType & CellType.PlaceForContact) == CellType.PlaceForContact)
                        {
                            cell.CellType = CellType.Shunt;
                        }
                        break;
                    case ToolType.CutCellDisposer:
                        cell.CellType = CellType.Cut;
                        break;
                    case ToolType.RCCellDisposer:
                        if (cell.CellType == CellType.RC)
                        {
                            cell.CellType = CellType.Cut;
                        }
                        else
                        {
                            cell.CellType = CellType.RC;
                        }
                        break;
                    case ToolType.RCellDisposer:
                        if (cell.CellType == CellType.R)
                        {
                            cell.CellType = CellType.Cut;
                        }
                        else
                        {
                            cell.CellType = CellType.R;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        // метод для проверки возможности применения инструмента к ячейке
        private static bool CheckToolApplyPossibility(IEditingTool tool, Cell cell)
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
                    if (!CheckContactCellPlacing(cell, CellType.Contact) && (cell.CellType & CellType.PlaceForContact) == CellType.PlaceForContact) result = false;
                    break;
                case ToolType.ForbidContactDisposer:
                    if ((cell.CellType & CellType.PlaceForContact) != CellType.PlaceForContact) result = false;
                    break;
                case ToolType.RCCellDisposer:
                    if ((cell.CellType & CellType.PlaceForContact) == CellType.PlaceForContact) result = false;
                    break;
                case ToolType.RCellDisposer:
                    if ((cell.CellType & CellType.PlaceForContact) == CellType.PlaceForContact) result = false;
                    break;
                case ToolType.ShuntCellDisposer:
                    if (!CheckContactCellPlacing(cell, CellType.Shunt) && (cell.CellType & CellType.PlaceForContact) == CellType.PlaceForContact) result = false;
                    break;
                default:
                    break;
            }

            bool CheckContactCellPlacing(Cell _cell, CellType needed_cell_type) 
            {
                var _layer = cell.MainCell.CellsInLayer.Single(x => x.Value == _cell).Key;

                // верхняя строка 
                if (_cell.MainCell.Position.x == 0)
                {
                    var left_cell_type = _layer.Cells[_cell.MainCell.Position.x][_cell.MainCell.Position.y - 1].CellType;
                    var right_cell_type = _layer.Cells[_cell.MainCell.Position.x][_cell.MainCell.Position.y + 1].CellType;
                    // ячейка слева снизу
                    var down_left_cell_type = _layer.Cells[_cell.MainCell.Position.x + 1][_cell.MainCell.Position.y - 1].CellType;
                    // ячейка справа снизу
                    var down_right_cell_type = _layer.Cells[_cell.MainCell.Position.x + 1][_cell.MainCell.Position.y + 1].CellType;

                    // ячейка слева
                    if ((left_cell_type == CellType.Shunt & left_cell_type != needed_cell_type) || (left_cell_type == CellType.Contact & left_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                    // ячейка справа
                    else if ((right_cell_type == CellType.Shunt & right_cell_type != needed_cell_type) || (right_cell_type == CellType.Contact & right_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                    else if ((down_left_cell_type == CellType.Shunt & down_left_cell_type != needed_cell_type) || (down_left_cell_type == CellType.Contact & down_left_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                    else if ((down_right_cell_type == CellType.Shunt & down_right_cell_type != needed_cell_type) || (down_right_cell_type == CellType.Contact & down_right_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                }
                // нижняя
                else if (_cell.MainCell.Position.x == _layer.Cells.First().Count - 1)
                {
                    var left_cell_type = _layer.Cells[_cell.MainCell.Position.x][_cell.MainCell.Position.y - 1].CellType;
                    var right_cell_type = _layer.Cells[_cell.MainCell.Position.x][_cell.MainCell.Position.y + 1].CellType;
                    // ячейка слева сверху
                    var up_left_cell_type = _layer.Cells[_cell.MainCell.Position.x - 1][_cell.MainCell.Position.y - 1].CellType;
                    // ячейка справа сверху
                    var up_right_cell_type = _layer.Cells[_cell.MainCell.Position.x - 1][_cell.MainCell.Position.y + 1].CellType;

                    // ячейка слева
                    if ((left_cell_type == CellType.Shunt & left_cell_type != needed_cell_type) || (left_cell_type == CellType.Contact & left_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                    // ячейка справа
                    else if ((right_cell_type == CellType.Shunt & right_cell_type != needed_cell_type) || (right_cell_type == CellType.Contact & right_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                    else if ((up_left_cell_type == CellType.Shunt & up_left_cell_type != needed_cell_type) || (up_left_cell_type == CellType.Contact & up_left_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                    else if ((up_right_cell_type == CellType.Shunt & up_right_cell_type != needed_cell_type) || (up_right_cell_type == CellType.Contact & up_right_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                }
                // левый столбец
                else if (_cell.MainCell.Position.y == 0)
                {
                    var up_cell_type = _layer.Cells[_cell.MainCell.Position.x - 1][_cell.MainCell.Position.y].CellType;
                    var down_cell_type = _layer.Cells[_cell.MainCell.Position.x + 1][_cell.MainCell.Position.y].CellType;
                    // ячейка справа снизу
                    var down_right_cell_type = _layer.Cells[_cell.MainCell.Position.x + 1][_cell.MainCell.Position.y + 1].CellType;
                    // ячейка справа сверху
                    var up_right_cell_type = _layer.Cells[_cell.MainCell.Position.x - 1][_cell.MainCell.Position.y + 1].CellType;

                    // ячейка сверху
                    if ((up_cell_type == CellType.Shunt & up_cell_type != needed_cell_type) || (up_cell_type == CellType.Contact & up_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                    // ячейка снизу
                    else if ((down_cell_type == CellType.Shunt & down_cell_type != needed_cell_type) || (down_cell_type == CellType.Contact & down_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                    else if ((up_right_cell_type == CellType.Shunt & up_right_cell_type != needed_cell_type) || (up_right_cell_type == CellType.Contact & up_right_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                    else if ((down_right_cell_type == CellType.Shunt & down_right_cell_type != needed_cell_type) || (down_right_cell_type == CellType.Contact & down_right_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                }
                // правый
                else if (_cell.MainCell.Position.y == _layer.Cells.Count - 1)
                {
                    var up_cell_type = _layer.Cells[_cell.MainCell.Position.x - 1][_cell.MainCell.Position.y].CellType;
                    var down_cell_type = _layer.Cells[_cell.MainCell.Position.x + 1][_cell.MainCell.Position.y].CellType;
                    // ячейка слева снизу
                    var down_left_cell_type = _layer.Cells[_cell.MainCell.Position.x + 1][_cell.MainCell.Position.y - 1].CellType;
                    // ячейка слева сверху
                    var up_left_cell_type = _layer.Cells[_cell.MainCell.Position.x - 1][_cell.MainCell.Position.y - 1].CellType;

                    // ячейка сверху
                    if ((up_cell_type == CellType.Shunt & up_cell_type != needed_cell_type) || (up_cell_type == CellType.Contact & up_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                    // ячейка снизу
                    else if ((down_cell_type == CellType.Shunt & down_cell_type != needed_cell_type) || (down_cell_type == CellType.Contact & down_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                    else if ((up_left_cell_type == CellType.Shunt & up_left_cell_type != needed_cell_type) || (up_left_cell_type == CellType.Contact & up_left_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                    else if ((down_left_cell_type == CellType.Shunt & down_left_cell_type != needed_cell_type) || (down_left_cell_type == CellType.Contact & down_left_cell_type != needed_cell_type))
                    {
                        return false;
                    }
                }

                return true;
            }

            return result;
        }

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        [field: NonSerialized]
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
