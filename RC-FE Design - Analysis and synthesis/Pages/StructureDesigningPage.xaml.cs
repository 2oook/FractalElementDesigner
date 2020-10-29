﻿using FractalElementDesigner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FractalElementDesigner.Pages
{
    /// <summary>
    /// Interaction logic for AnalysisPage.xaml
    /// </summary>
    public partial class StructureDesigningPage : Page
    {
        public StructureDesigningPage()
        {
            InitializeComponent();

            Focus();
        }

        /// <summary>
        /// Обработчик нажатия клавиш клавиатуры
        /// </summary>
        /// <param name="e">Параметры события</param>
        /// <param name="sender">Объект отправитель</param>
        private void HandleKeyEvents(object sender, KeyEventArgs e)
        {
            var context = this.DataContext as StructureDesigningPageViewModel;
            if (context == null) return;

            bool isControl = Keyboard.Modifiers == ModifierKeys.Control;
            var key = e.Key;

            if (isControl == true)
            {
                switch (key)
                {
                    case Key.O: context.LoadProjectCommand.Execute(null); break;
                    case Key.S: context.SaveProjectCommand.Execute(null); break;
                    case Key.N: context.NewProjectCommand.Execute(null); break;
                }
            }            
        }

        /// <summary>
        /// Обработчик нажатия правой клавиши мыши для элемента дерева проекта
        /// </summary>
        /// <param name="sender">Объект отправитель</param>
        /// <param name="e">Параметры события</param>
        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        // Метод для поиска элемента дерева
        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }
    }
}
