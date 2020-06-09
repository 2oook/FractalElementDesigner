﻿using RC_FE_Design___Analysis_and_synthesis.FEEditor.Controls;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Core;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Elements;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RC_FE_Design___Analysis_and_synthesis.FEEditor
{
    public static class Insert
    {

        public static void StructureLayer(FECanvas canvas, PointEx point, RCStructure structure)
        {
            double _BorderCellHeight = 30;
            double _BorderCellWidth = 30;

            double _CommonCellHeight = 60;
            double _CommonCellWidth = 60;

            var _grid = new Grid();

            var rows = structure.StructureCells;

            for (int i = 0; i < rows.Count; i++)
            {
                _grid.RowDefinitions.Add(new RowDefinition());

                double height = _CommonCellHeight;

                var row = rows[i];

                for (int j = 0; j < row.Count; j++)
                {
                    double width = _CommonCellWidth;

                    if (i == 0)
                    {
                        _grid.ColumnDefinitions.Add(new ColumnDefinition());
                        height = _BorderCellHeight;  
                    }

                    if (j == 0)
                    {
                        width = _BorderCellWidth;
                    }

                    if (i == rows.Count -1)
                    {
                        height = _BorderCellHeight;
                    }

                    if (j == row.Count - 1)
                    {
                        width = _BorderCellWidth;
                    }

                    var cell = new CellControl(height, width);

                    Grid.SetRow(cell, i);
                    Grid.SetColumn(cell, j);

                    _grid.Children.Add(cell);               
                }
            }

            var grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            var widthBindind = new Binding();
            widthBindind.Source = canvas;
            widthBindind.Path = new PropertyPath("ActualWidth");
            widthBindind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //widthBindind.Mode = BindingMode.TwoWay;
            grid.SetBinding(Grid.WidthProperty, widthBindind);

            var heightBindind = new Binding();
            heightBindind.Source = canvas;
            heightBindind.Path = new PropertyPath("ActualHeight");
            heightBindind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //heightBindind.Mode = BindingMode.TwoWay;
            grid.SetBinding(Grid.HeightProperty, heightBindind);

            grid.Children.Add(_grid);

            canvas.Add(grid);
        }
    }
}
