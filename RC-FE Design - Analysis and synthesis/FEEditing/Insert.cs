using FractalElementDesigner.FEEditing.Controls;
using FractalElementDesigner.FEEditing.Core;
using FractalElementDesigner.FEEditing.Elements;
using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.FEEditing.Model.StructureElements;
using FractalElementDesigner.MathModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FractalElementDesigner.FEEditing
{
    /// <summary>
    /// Класс для вставки элементов в область редактирования
    /// </summary>
    static class Insert
    {
        // Метод для вставки слоя структуры в элемент Canvas
        public static void StructureLayer(FECanvas canvas, RCStructureBase structure, Layer layer)
        {
            double contactCellHeight = 30;
            double contactCellWidth = 30;

            double mainCellHeight = 60;
            double mainCellWidth = 60;

            double structureWidth = 0;
            double structureHeight = 0;

            var cells_array = structure.Segments;

            var _grid = new Grid();

            for (int i = 0; i < cells_array.Count; i++)
            {
                structureHeight += mainCellHeight;
                _grid.RowDefinitions.Add(new RowDefinition());
            }

            for (int j = 0; j < cells_array[0].Count; j++)
            {
                structureWidth += mainCellWidth;
                _grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < cells_array.Count; i++)
            {
                var row = cells_array[i];

                double _height = mainCellHeight;

                for (int j = 0; j < row.Count; j++)
                {
                    double _width = mainCellWidth;

                    // первая строка
                    if (i == 0)
                    {
                        _height = contactCellHeight;
                    }
                    // последняя строка
                    if (i == cells_array.Count - 1)
                    {
                        _height = contactCellHeight;
                    }
                    // первая колонка
                    if (j == 0)
                    {
                        _width = contactCellWidth;
                    }
                    // последняя колонка
                    if (j == row.Count - 1)
                    {
                        _width = contactCellWidth;
                    }

                    // создать контрол ячейки
                    var cell = new CellControl(_height, _width);
                    // связать отображение с объектом структуры
                    cell.DataContext = cells_array[i][j].CellsInLayer[layer];

                    Grid.SetRow(cell, i);
                    Grid.SetColumn(cell, j);
                    _grid.Children.Add(cell);
                }
            }

            FitCanvasToStructure(structureWidth, structureHeight, canvas);

            PlaceCanvasInCenter(_grid, canvas);
        }

        // Метод для вставки слоя структуры в элемент Canvas
        public static void ExistingStructureLayer(FECanvas canvas, RCStructureBase structure, Layer layer)
        {
            double _BorderCellHeight = 30;
            double _BorderCellWidth = 30;

            double _CommonCellHeight = 60;
            double _CommonCellWidth = 60;

            double structureWidth = 0;
            double structureHeight = 0;

            var _grid = new Grid();

            var cells_array = structure.Segments;

            for (int i = 0; i < cells_array.Count; i++)
            {
                _grid.RowDefinitions.Add(new RowDefinition());

                double height = _CommonCellHeight;

                var row = cells_array[i];

                for (int j = 0; j < row.Count; j++)
                {
                    double width = _CommonCellWidth;

                    // первая строка
                    if (i == 0)
                    {
                        structureWidth += width;
                        _grid.ColumnDefinitions.Add(new ColumnDefinition());
                        height = _BorderCellHeight;
                    }
                    // последняя строка
                    if (i == cells_array.Count - 1)
                    {
                        height = _BorderCellHeight;
                    }
                    // первая колонка
                    if (j == 0)
                    {
                        width = _BorderCellWidth;
                    }
                    // последняя колонка
                    if (j == row.Count - 1)
                    {
                        width = _BorderCellWidth;
                    }

                    // создать контрол ячейки
                    var cell = new CellControl(height, width);
                    // связать отображение с объектом структуры
                    cell.DataContext = cells_array[i][j].CellsInLayer[layer];

                    Grid.SetRow(cell, i);
                    Grid.SetColumn(cell, j);

                    _grid.Children.Add(cell);
                }

                structureHeight += height;
            }

            FitCanvasToStructure(structureWidth, structureHeight, canvas);

            PlaceCanvasInCenter(_grid, canvas);
        }

        // Метод для позиционирования элемента canvas в центре экрана
        private static void PlaceCanvasInCenter(Grid _grid, Canvas canvas) 
        {
            // создать дополнительную сетку для размещения структуры в центре области редактирования
            var grid = new Grid();

            var row1 = new RowDefinition();
            row1.Height = new GridLength(1, GridUnitType.Star);
            var row2 = new RowDefinition();
            row2.Height = new GridLength(1, GridUnitType.Auto);
            var row3 = new RowDefinition();
            row3.Height = new GridLength(1, GridUnitType.Star);

            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);

            var col1 = new ColumnDefinition();
            col1.Width = new GridLength(1, GridUnitType.Star);
            var col2 = new ColumnDefinition();
            col2.Width = new GridLength(1, GridUnitType.Auto);
            var col3 = new ColumnDefinition();
            col3.Width = new GridLength(1, GridUnitType.Star);

            grid.ColumnDefinitions.Add(col1);
            grid.ColumnDefinitions.Add(col2);
            grid.ColumnDefinitions.Add(col3);

            Grid.SetRow(_grid, 1);
            Grid.SetColumn(_grid, 1);

            var widthBindind = new Binding();
            widthBindind.Source = canvas;
            widthBindind.Path = new PropertyPath("ActualWidth");
            widthBindind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            grid.SetBinding(Grid.WidthProperty, widthBindind);

            var heightBindind = new Binding();
            heightBindind.Source = canvas;
            heightBindind.Path = new PropertyPath("ActualHeight");
            heightBindind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            grid.SetBinding(Grid.HeightProperty, heightBindind);

            grid.Children.Add(_grid);

            canvas.Children.Add(grid);
        }

        // метод для подгонки размеров элемента Canvas для размещения структуры
        private static void FitCanvasToStructure(double structureWidth, double structureHeight, FECanvas canvas) 
        {
            // если необходимая ширина больше фактической
            if (structureWidth > canvas.Width)
            {             
                canvas.Width = structureWidth + 10;             
                var k = structureWidth / canvas.Width;
                canvas.Height = canvas.Height * k + 10;
            }
            else
            {
                canvas.Width = canvas.Width;
            }

            // если необходимая высота больше фактической
            if (structureHeight > canvas.Height)
            {
                canvas.Height = structureHeight + 10;
                // вычислить коэффициент пропорции высоты
                var k = structureHeight / canvas.Height;
                canvas.Width = canvas.Width * k + 10;
            }
            else
            {
                canvas.Height = canvas.Height;
            }
        }
    }
}
