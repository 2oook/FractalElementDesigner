﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FractalElementDesigner.MathModel;
using System.Threading.Tasks;
using FractalElementDesigner.StructureSchemeSynthesis;
using GalaSoft.MvvmLight.Threading;
using FractalElementDesigner.SchemeEditing;
using FractalElementDesigner.MathModel.Structure;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using FractalElementDesigner.FEEditing;
using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.FEEditing.Model.StructureElements;
using FractalElementDesigner.Navigating.Interfaces;
using FractalElementDesigner.Pages;
using FractalElementDesigner.Windows;
using FractalElementDesigner.RCWorkbenchLibrary;
using FractalElementDesigner.RCWorkbenchLibrary.Helpers;
using MathNet.Numerics.LinearAlgebra;
using FractalElementDesigner.ProjectTree;

namespace FractalElementDesigner.ViewModels
{
    /// <summary>
    /// ViewModel страницы анализа и синтеза
    /// </summary>
    class StructureDesigningPageViewModel : INotifyPropertyChanged, IPageViewModel
    {
        public StructureDesigningPageViewModel()
        {
            InitializeCommands();

            SchemeSynthesizer.OnDoWork += OnDoWork;
            SchemeSynthesizer.OnStateChange += OnProgressStateChange;
            StructureCreator.OnDoWork += OnDoWork;
            StructureCreator.OnStateChange += OnProgressStateChange;

            StructureSynthesizer.OnDoWork += OnDoWork;
            StructureSynthesizer.OnStateChange += OnProgressStateChange;

            IToolCreator toolCreator = new ToolCreator();

            EditorTools.Add("Numerate", toolCreator.CreateTool(ToolType.ContactNumerator));
            EditorTools.Add("Cut", toolCreator.CreateTool(ToolType.CutCellDisposer));
            //EditorTools.Add("RC", toolCreator.CreateTool(ToolType.RCCellDisposer));
            EditorTools.Add("R", toolCreator.CreateTool(ToolType.RCellDisposer));
            EditorTools.Add("Contact", toolCreator.CreateTool(ToolType.ContactCellDisposer));
            EditorTools.Add("Forbid", toolCreator.CreateTool(ToolType.ForbidContactDisposer));
            EditorTools.Add("Shunt", toolCreator.CreateTool(ToolType.ShuntCellDisposer));

            foreach (var tool in EditorTools.Values)
            {
                tool.PropertyChanged += (object sender, PropertyChangedEventArgs e) => 
                {
                    if (sender as Tool != null)
                    {
                        var _tool = (Tool)sender;

                        if (e.PropertyName == nameof(_tool.IsChecked))
                        {
                            if (_tool.IsChecked == true)
                            {
                                SelectedTool = _tool;
                            }
                        }
                    }
                };
            }

            HomePageVisibility = Visibility.Visible;      
        }

        public StructureDesigningPageViewModel(IDialogCoordinator dialogCoordinator) : this()
        {
            _dialogCoordinator = dialogCoordinator;
        }

        #region Глобальные переменные

        /// <summary>
        /// Объект для вывода диалогов
        /// </summary>
        private IDialogCoordinator _dialogCoordinator;

        /// <summary>
        /// Ссылка на страницу
        /// </summary>
        private StructureDesigningPage _Page;

        #endregion

        #region Свойства

        /// <summary>
        /// Словарь инструментов для редактирования структуры
        /// </summary>
        public Dictionary<string, IEditingTool> EditorTools { get; set; } = new Dictionary<string, IEditingTool>();

        private object selectedProjectTreeItem;
        /// <summary>
        /// Выбранный элемент дерева проекта
        /// </summary>
        public object SelectedProjectTreeItem
        {
            get { return selectedProjectTreeItem; }
            set 
            {
                InvalidateAllVisualViewers();
                selectedProjectTreeItem = value;
                SetChosenProjectTreeItemEnvironment(value);
 
                RaisePropertyChanged(nameof(SelectedProjectTreeItem));
            }
        }

        private IEditingTool selectedTool = null;
        /// <summary>
        /// Выбранный инструмент
        /// </summary>
        public IEditingTool SelectedTool 
        {
            get
            {
                if (selectedTool != null && selectedTool.IsChecked)
                {                 
                    return selectedTool;
                }
                else
                {
                    return null;    
                }

            }
            set 
            {
                selectedTool = value;
                RaisePropertyChanged(nameof(SelectedTool));
            }
        }

        private ObservableCollection<Project> _projects = new ObservableCollection<Project>();
        /// <summary>
        /// Список проектов
        /// </summary>
        public ObservableCollection<Project> Projects
        {
            get { return _projects; }
            set 
            { 
                _projects = value;
                RaisePropertyChanged(nameof(Projects));
            }
        }

        private double progressBarValue;
        /// <summary>
        /// Значение строки прогресса
        /// </summary>
        public double ProgressBarValue
        {
            get { return progressBarValue; }
            set 
            { 
                progressBarValue = value;
                RaisePropertyChanged(nameof(ProgressBarValue));
            }
        }

        private string progressState = string.Empty;
        /// <summary>
        /// Статус прогресса
        /// </summary>
        public string ProgressState
        {
            get { return progressState; }
            set
            {
                progressState = value;
                RaisePropertyChanged(nameof(ProgressState));
            }
        }

        private Visibility structureEeditorVisibility = Visibility.Hidden;
        /// <summary>
        /// Видимость редактора структуры
        /// </summary>
        public Visibility StructureEditorVisibility
        {
            get { return structureEeditorVisibility; }
            set
            {
                structureEeditorVisibility = value;
                RaisePropertyChanged(nameof(StructureEditorVisibility));
            }
        }

        private Visibility schemeEditorVisibility = Visibility.Hidden;
        /// <summary>
        /// Видимость редактора схем
        /// </summary>
        public Visibility SchemeEditorVisibility
        {
            get { return schemeEditorVisibility; }
            set
            {
                schemeEditorVisibility = value;
                RaisePropertyChanged(nameof(SchemeEditorVisibility));
            }
        }

        private Visibility homePageVisibility = Visibility.Hidden;
        /// <summary>
        /// Видимость домашней страницы
        /// </summary>
        public Visibility HomePageVisibility
        {
            get { return homePageVisibility; }
            set
            {
                homePageVisibility = value;
                RaisePropertyChanged(nameof(HomePageVisibility));
            }
        }

        private Visibility plotVisibility = Visibility.Hidden;
        /// <summary>
        /// Видимость графика
        /// </summary>
        public Visibility PlotVisibility
        {
            get{ return plotVisibility; }
            set
            {
                plotVisibility = value;
                RaisePropertyChanged(nameof(PlotVisibility));
            }
        }

        private bool isProjectTreeEnabled = true;
        /// <summary>
        /// Разрешено ли манипулирование с деревом проекта 
        /// </summary>
        public bool IsProjectTreeEnabled
        {
            get { return isProjectTreeEnabled; }
            set 
            { 
                isProjectTreeEnabled = value;
                RaisePropertyChanged(nameof(IsProjectTreeEnabled));
            }
        }

        #endregion

        #region Команды

        private ICommand testCommand;
        public ICommand TestCommand
        {
            get { return testCommand; }
            set
            {
                testCommand = value;
                RaisePropertyChanged(nameof(TestCommand));
            }
        }

        private ICommand recalculateStructureCommand;
        /// <summary>
        /// Команда для пересчёта структуры
        /// </summary>
        public ICommand RecalculateStructureCommand
        {
            get { return recalculateStructureCommand; }
            set
            {
                recalculateStructureCommand = value;
                RaisePropertyChanged(nameof(RecalculateStructureCommand));
            }
        }

        private ICommand newProjectCommand;
        /// <summary>
        /// Команда для создания нового проекта
        /// </summary>
        public ICommand NewProjectCommand
        {
            get { return newProjectCommand; }
            set
            {
                newProjectCommand = value;
                RaisePropertyChanged(nameof(NewProjectCommand));
            }
        }

        private ICommand cellApplyToolCommand;
        /// <summary>
        /// Команда для применения инструмента к ячейке элемента
        /// </summary>
        public ICommand CellApplyToolCommand
        {
            get { return cellApplyToolCommand; }
            set
            {
                cellApplyToolCommand = value;
                RaisePropertyChanged(nameof(CellApplyToolCommand));
            }
        }  

        private ICommand loadProjectCommand;
        /// <summary>
        /// Команда для загрузки проекта
        /// </summary>
        public ICommand LoadProjectCommand
        {
            get { return loadProjectCommand; }
            set
            {
                loadProjectCommand = value;
                RaisePropertyChanged(nameof(LoadProjectCommand));
            }
        }

        private ICommand saveProjectCommand;
        /// <summary>
        /// Команда для сохранения проекта
        /// </summary>
        public ICommand SaveProjectCommand
        {
            get { return saveProjectCommand; }
            set
            {
                saveProjectCommand = value;
                RaisePropertyChanged(nameof(SaveProjectCommand));
            }
        }

        private ICommand synthesisCommand;
        /// <summary>
        /// Команда для синтеза
        /// </summary>
        public ICommand SynthesisCommand
        {
            get { return synthesisCommand; }
            set
            {
                synthesisCommand = value;
                RaisePropertyChanged(nameof(SynthesisCommand));
            }
        }

        private ICommand сreateStructureCommand;
        /// <summary>
        /// Команда для создания конструкции элемента
        /// </summary>
        public ICommand CreateStructureCommand
        {
            get { return сreateStructureCommand; }
            set
            {
                сreateStructureCommand = value;
                RaisePropertyChanged(nameof(CreateStructureCommand));
            }
        }

        private ICommand choiceOfSchemeCommand;
        /// <summary>
        /// Команда для выбора схемы
        /// </summary>
        public ICommand ChoiceOfSchemeCommand
        {
            get { return choiceOfSchemeCommand; }
            set
            {
                choiceOfSchemeCommand = value;
                RaisePropertyChanged(nameof(ChoiceOfSchemeCommand));
            }
        }

        /// <summary>
        /// Команда для перемещения на главную страницу 
        /// </summary>
        public ICommand GoToMainPageCommand { get; set; }

        #endregion

        #region Обработчики событий

        /// <summary>
        /// Обработчик выполнения части работы
        /// </summary>
        /// <param name="value">Процент выполнения</param>
        private void OnDoWork(double value)
        {
            ProgressBarValue = value;
        }

        /// <summary>
        /// Обработчик изменения статуса выполнения процесса
        /// </summary>
        /// <param name="state">Статус выполнения</param>
        private void OnProgressStateChange(string state)
        {
            ProgressState = state;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Метод для установки видимости в "скрыто" всем элементам
        /// </summary>
        private void InvalidateAllVisualViewers() 
        {
            HomePageVisibility = Visibility.Hidden;
            SchemeEditorVisibility = Visibility.Hidden;
            StructureEditorVisibility = Visibility.Hidden;
            PlotVisibility = Visibility.Hidden;
        }

        /// <summary>
        /// Метод для установки окружения в зависимости от установленного итема в дереве проекта
        /// </summary>
        /// <param name="value"></param>
        private void SetChosenProjectTreeItemEnvironment(object value) 
        {
            try
            {
                if (value is Project project)
                {

                }
                else if (value is FElementScheme scheme)
                {
                    SchemeEditorVisibility = Visibility.Visible;
                    _Page.schemeEditorControl.SchemeControl.Editor = scheme.Editor;
                }
                else if (value is PRPlot plot)
                {
                    PlotVisibility = Visibility.Visible;
                    _Page.plotControl.DataContext = plot;
                }
                else if (value is StructureInProjectTree structureInProject)
                {
                    StructureEditorVisibility = Visibility.Visible;
                    _Page.structureEditorControl.structurePlot.DataContext = null;

                    var _plot = structureInProject.Items.Where(x => x is PRPlot).Single() as PRPlot;

                    if (_plot != null && _plot.Points != null)
                    {
                        _Page.structureEditorControl.structurePlot.DataContext = _plot.Clone();
                    }
                }
                else if (value is RCStructureBase)
                {
                    StructureEditorVisibility = Visibility.Visible;

                    _Page.structureEditorControl.structurePlot.DataContext = null;

                    // выбрать StructureInProjectTree который содержит value
                    var _structureInProject = Projects.Where(x => x.Items.Where(y => y is StructureInProjectTree).Count() > 0)
                        .SelectMany(x => x.Items.Where(y => y is StructureInProjectTree))
                        .Single(x => (x as StructureInProjectTree).Items.Contains(value)) as StructureInProjectTree;

                    var _plot = _structureInProject.Items.Where(x => x is PRPlot).Single() as PRPlot;

                    if (_plot != null && _plot.Points != null)
                    {
                        _Page.structureEditorControl.structurePlot.DataContext = _plot.Clone();
                    }
                }
                else if (value is Layer layer)
                {
                    StructureEditorVisibility = Visibility.Visible;
                    _Page.structureEditorControl.FEControl.Editor = layer.Editor;

                    _Page.structureEditorControl.structurePlot.DataContext = null;

                    // выбрать StructureInProjectTree который содержит layer.ParentStructure
                    var _structureInProject = Projects.Where(x => x.Items.Where(y => y is StructureInProjectTree).Count() > 0)
                        .SelectMany(x => x.Items.Where(y => y is StructureInProjectTree))
                        .Single(x => (x as StructureInProjectTree).Items.Contains(layer.ParentStructure)) as StructureInProjectTree;

                    var _plot = _structureInProject.Items.Where(x => x is PRPlot).Single() as PRPlot;

                    if (_plot != null && _plot.Points != null)
                    {
                        _Page.structureEditorControl.structurePlot.DataContext = _plot.Clone();
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "", "Ошибка" + Environment.NewLine + ex.Message);
            } 
        }

        /// <summary>
        /// Метод для инициализации команд
        /// </summary>
        private void InitializeCommands()
        {
            NewProjectCommand = new RelayCommand(CreateNewProject);
            ChoiceOfSchemeCommand = new RelayCommand(ChoiceOfScheme, IsChoiceOfSchemePossible);

            SynthesisCommand = new RelayCommand(SynthesizeScheme, IsSchemeSynthesisPossible);
            CreateStructureCommand = new RelayCommand(CreateStructure, IsStructureCreatingPossible);
            RecalculateStructureCommand = new RelayCommand(RecalculateStructure, IsStructureRecalculatingPossible);

            CellApplyToolCommand = new RelayCommand<Cell>(ApplyToolForElementCell);
        }

        /// <summary>
        /// Метод для применения инструмента к ячейке элемента
        /// </summary>
        /// <param name="cell">Объект ячейки</param>
        private void ApplyToolForElementCell(Cell cell) 
        {
            cell.ApplyTool(SelectedTool);
        }

        /// <summary>
        /// Метод для выбора схемы
        /// </summary>
        private void ChoiceOfScheme() 
        {
            if (SelectedProjectTreeItem is FElementScheme scheme)
            {
                var project = Projects.SingleOrDefault(x => x.Items.Contains(scheme));

                if (project.Items.Where(x => x is FElementScheme).Count() == 1)
                    return;

                if (project == null)
                    return;

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    project.Items.Clear();
                    project.Items.Add(scheme);
                });
            }
        }

        // Метод для создания конструкции
        private void CreateStructure()
        {
            if (SelectedProjectTreeItem is FElementScheme scheme)
            {
                // создать окно
                var window = new NewStructureWindow();
                // создать vm для окна создания новой структуры
                var newStructureWindowViewModel = new NewStructureWindowViewModel(window, scheme.SynthesisParameters);
                // вывести окно ввода параметров структуры
                window.DataContext = newStructureWindowViewModel;
                var dialogResult = window.ShowDialog();

                // если не было подтверждения выйти
                if (dialogResult.HasValue == false)
                {
                    return;
                }
                else
                {
                    if (dialogResult.Value == false)
                    {
                        return;
                    }
                }

                var project = Projects.SingleOrDefault(x => x.Items.Contains(scheme));

                CreateStructureAsync(project, scheme, newStructureWindowViewModel.CurrentStructure, true);
            }
        }

        /// <summary>
        /// Метод для пересчёта структуры
        /// </summary>
        private void RecalculateStructure() 
        {
            try
            {
                if (SelectedProjectTreeItem is StructureInProjectTree structureInProject) 
                {
                    // проверка возможности анализа структуры
                    // TODO

                    var structure = structureInProject.Items.Where(x => x is RCStructureBase).Single() as RCStructure;

                    if (structure == null)
                    {
                        _dialogCoordinator.ShowMessageAsync(this, "", "Конструкция не определена");
                        return;
                    }

                    if (structure.Scheme == null)
                    {
                        #region RC Workbench

                        // создать структуру на стороне библиотеки
                        //ByRCWorkbenchStructureCreator.CreateStructure(scheme, structure); // Переделать! 

                        // анализ структуры
                        int first_dimension = structure.SynthesisParameters.PointsCountAtFrequencyAxle;
                        int second_dimension = 36;

                        double[] frequences = new double[first_dimension];
                        RCWorkbenchLibraryEntry.GetFrequences(frequences);

                        double[,] y_parameters_double = new double[first_dimension, second_dimension];
                        RCWorkbenchLibraryEntry.CalculateYParameters(y_parameters_double, first_dimension, second_dimension);

                        int _outer_pins_count = RCWorkbenchLibraryEntry.GetCPQuantity();

                        var matrices = MatrixHelper.GetYParametersMatricesFromRCWorkbenchArray(y_parameters_double, _outer_pins_count);

                        var I = Matrix<float>.Build.DenseOfArray(new float[4, 4]);
                        // установить диагональ
                        I.SetDiagonal(Vector<float>.Build.Dense(4, 1));
                        var pe = new List<int>() { 1, 2, 3 };

                        structure.PhaseResponsePoints.Clear();

                        for (int i = 0; i < matrices.Count; i++)
                        {
                            var matrix = matrices[i];

                            var phase = SchemePhaseResponseCalculator.ConsiderOuterScheme(ref matrix, ref I, pe);

                            structure.PhaseResponsePoints.Add((frequences[i], phase));
                        }

                        var _plot = structureInProject.Items.Where(x => x is PRPlot).Single() as PRPlot;

                        _plot.InitializatePhaseResponsePlot(structure.PhaseResponsePoints);

                        _Page.plotControl.DataContext = _plot.Clone();
                        _Page.structureEditorControl.structurePlot.DataContext = _plot.Clone();

                        return;

                        #endregion
                    }

                    // создать структуру на стороне библиотеки
                    ByRCWorkbenchStructureCreator.CreateStructureStraightByScheme(structure.Scheme, structure);

                    // пронумеровать контактные площадки 
                    StructureCreator.NumerateContactPlatesByScheme(structure);

                    // определить типы сегментов структуры по типам ячеек в слоях
                    StructureCreator.ResolveSegmentsTypes(structure);

                    // число ячеек по горизонтали структуры
                    var horizontalStructureDimensionValue = structure.Segments.First().Count -2;
                    // число ячеек по вертикали структуры
                    var verticalStructureDimensionValue = structure.Segments.Count -2;

                    var layerCount = structure.StructureLayers.Count;
                    var horizontalRange = (horizontalStructureDimensionValue + 1);
                    var verticalRange = (verticalStructureDimensionValue + 1);
                    var arrayDimension = layerCount * horizontalRange * verticalRange;
                    var nodesNumerationFlat = new int[arrayDimension];

                    // проверка
                    int outer_pins_count = RCWorkbenchLibraryEntry.GetCPQuantity();

                    RCWorkbenchLibraryEntry.GetNodesNumeration(nodesNumerationFlat);

                    // получить количество узлов
                    var nodesCount = RCWorkbenchLibraryEntry.GetNodesQuantity();

                    RCWorkbenchLibraryEntry.DeleteStructureStraight();

                    // восстановить плоский массив нумерации узлов
                    var nodesNumeration = RCWorkbenchIntercommunicationHelper.UnflatNumerationArray(layerCount, horizontalRange, verticalRange, nodesNumerationFlat);

                    var calculator = new StructurePhaseResponseCalculator(structure, horizontalStructureDimensionValue, verticalStructureDimensionValue, nodesCount, nodesNumeration);

                    var points = PhaseResponseCalculatorForStructureForAllFrequencies.CalculatePhaseResponseInStructure(
                        structure.SynthesisParameters.MinFrequencyLn, structure.SynthesisParameters.MaxFrequencyLn, structure.SynthesisParameters.PointsCountAtFrequencyAxle, structure, calculator);

                    structure.PhaseResponsePoints = points;

                    var plot = structureInProject.Items.Where(x => x is PRPlot).Single() as PRPlot;

                    plot.InitializatePhaseResponsePlot(structure.PhaseResponsePoints);

                    _Page.plotControl.DataContext = plot.Clone();
                    _Page.structureEditorControl.structurePlot.DataContext = plot.Clone();
                }
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "", "Ошибка выполнения анализа" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// Метод определяющий возможность пересчёта конструкции схемы
        /// </summary>
        /// <returns>Разрешающий флаг</returns>
        private bool IsStructureRecalculatingPossible()
        {
            if (SelectedProjectTreeItem is StructureInProjectTree structureInProjectTree)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод определяющий возможность создания конструкции схемы
        /// </summary>
        /// <returns>Разрешающий флаг</returns>
        private bool IsStructureCreatingPossible() 
        {
            if (SelectedProjectTreeItem is FElementScheme scheme)
            {
                if (scheme.IsLocked)
                {
                    return false;
                }

                var project = Projects.SingleOrDefault(x => x.Items.Contains(scheme));

                if (project.Items.Where(x => x is FElementScheme).Count() > 1)
                {
                    return false;
                }
                else if (project.Items.Where(x => x is FElementScheme).Count() == 1)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Метод для определения возможности выбора схемы
        /// </summary>
        /// <returns>Разрешающий флаг</returns>
        private bool IsChoiceOfSchemePossible()
        {
            if (SelectedProjectTreeItem is FElementScheme scheme)
            {
                var project = Projects.SingleOrDefault(x => x.Items.Contains(scheme));

                if (project == null)
                    return false;

                if (project.Items.Where(x => x is FElementScheme).Count() == 1)
                    return false;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод определяющий возможность синтеза
        /// </summary>
        /// <returns>Разрешающий флаг</returns>
        private bool IsSchemeSynthesisPossible() 
        {
            if (SelectedProjectTreeItem is FElementScheme scheme)
            {
                if (scheme.IsLocked)
                {
                    return false;
                }

                var project = Projects.SingleOrDefault(x => x.Items.Contains(scheme));

                if (project.Items.Count == 1)
                {
                    return true;
                }              
            }

            if (SelectedProjectTreeItem is StructureInProjectTree structureInProject)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод для запуска синтеза схемы
        /// </summary>
        private void SynthesizeScheme() 
        {
            if (SelectedProjectTreeItem is FElementScheme scheme)
            {
                // создать окно
                var window = new StructureSchemeSynthesisParametersWindow();
                // создать vm для окна 
                var structureSchemeSynthesisParametersViewModel = new StructureSchemeSynthesisParametersWindowViewModel(window);
                // вывести окно ввода параметров 
                window.DataContext = structureSchemeSynthesisParametersViewModel;
                var dialogResult = window.ShowDialog();

                // если не было подтверждения выйти
                if (dialogResult.HasValue == false)
                {
                    return;
                }
                else
                {
                    if (dialogResult.Value == false)
                    {
                        return;
                    }
                }

                var project = Projects.SingleOrDefault(x => x.Items.Contains(scheme));

                project.Items.Clear();

                StartSchemeSynthesisAsync(project, scheme);
            }

            if (SelectedProjectTreeItem is StructureInProjectTree structureInProject)
            {
                var structure = structureInProject.Items.Where(x => x is RCStructureBase).Single() as RCStructure;

                StartStructureSynthesisAsync(structure.SynthesisParameters, structure);
            }
        }

        // Метод для создания нового проекта
        private void CreateNewProject()
        {
            try
            { 
                // создать окно
                var window = new StructureSchemeSynthesisParametersWindow();
                // создать vm для окна 
                var structureSchemeSynthesisParametersViewModel = new StructureSchemeSynthesisParametersWindowViewModel(window);
                // вывести окно ввода параметров 
                window.DataContext = structureSchemeSynthesisParametersViewModel;
                var dialogResult = window.ShowDialog();

                // если не было подтверждения выйти
                if (dialogResult.HasValue == false)
                {
                    return;
                }
                else
                {
                    if (dialogResult.Value == false)
                    {
                        return;
                    }
                }

                //////////////////////////////////////////////////
                if (HomePageVisibility == Visibility.Visible)
                {
                    HomePageVisibility = Visibility.Hidden;
                }

                // создать проект
                var project = new Project() { Name = "Проект №1" };

                Projects.Add(project);

                var schemePrototype = new FElementScheme(structureSchemeSynthesisParametersViewModel.StructureSchemeSynthesisParametersInstance) 
                { 
                    Name = "Схема", 
                    Elements = { new PRPlot() }
                };

                StartSchemeSynthesisAsync(project, schemePrototype);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // Метод для асинхронного создания конструкции элемента
        private async void CreateStructureAsync(Project currentProject, FElementScheme scheme, RCStructure structure, bool by_scheme) 
        {
            await Task.Run(() =>
            {
                try
                {
                    scheme.IsLocked = true;

                    // создать конструкцию элемента
                    if (by_scheme)
                    {
                        structure = StructureCreator.CreateStructureByScheme(scheme, structure);
                    }
                    else
                    {
                        structure = StructureCreator.Create(structure);
                    }

                    var structure_in_project = new StructureInProjectTree() { Name = structure.Name };
                    structure_in_project.Items.Add(structure);
                    structure_in_project.Items.Add(new PRPlot());

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        StructureCreator.InsertVisual(structure, _Page.structureEditorControl.FEControl);
                        currentProject.Items.Add(structure_in_project);
                    });

                    // тест
                    // тест
                    
                    // TODO выполнить по выбору пользователя // ПОМЕСТИТЬ В КОМАНДУ // ТЕСТОВЫЙ ВЫЗОВ
                    //StartStructureSynthesisAsync(structureSchemeSynthesisParametersViewModel.StructureSchemeSynthesisParametersInstance, currentProject, structure);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    _dialogCoordinator.ShowMessageAsync(this, "", "Ошибка создания конструкции" + Environment.NewLine + ex.Message);
                }
                finally
                {
                    scheme.IsLocked = false;
                }
            });
        }

        // Метод для запуска синтеза конструкции асинхронно
        private async void StartStructureSynthesisAsync(StructureSchemeSynthesisParameters structureSchemeSynthesisParametersInstance, RCStructure structure)
        {
            await Task.Run(() =>
            {
                try
                {
                    // синтезировать конструкцию
                    var structures = StructureSynthesizer.Synthesize(structureSchemeSynthesisParametersInstance, structure);

                    for (int i = 0; i < structures.Count; i++)
                    {
                        var _structure = structures[i];
                        _structure.Name = "Конструкция №" + (i + 1);

                        // преобразовать типы сегментов в типы ячеек

                        var structure_in_project = new StructureInProjectTree() { Name = _structure.Name };
                        structure_in_project.Items.Add(_structure);
                        structure_in_project.Items.Add(new PRPlot());

                        var project = Projects.SingleOrDefault(x => x.Items.Contains(structure.Scheme));

                        // Инициализировать график
                        var plot = structure_in_project.Items.Where(x => x is PRPlot).SingleOrDefault() as PRPlot;
                        plot.InitializatePhaseResponsePlot(_structure.PhaseResponsePoints);

                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            StructureCreator.InsertVisual(_structure, _Page.structureEditorControl.FEControl);
                            project.Items.Add(structure_in_project);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    _dialogCoordinator.ShowMessageAsync(this, "", "Ошибка синтеза" + Environment.NewLine + ex.Message);
                }
            });
        }


        // Метод для запуска синтеза схемы асинхронно
        private async void StartSchemeSynthesisAsync(Project currentProject, FElementScheme scheme) 
        {
            await Task.Run(() =>
            {
                try
                {
                    // синтезировать схему
                    var schemes = SchemeSynthesizer.Synthesize(scheme);

                    for (int i = 0; i < schemes.Count; i++)
                    {
                        var _scheme = schemes[i];
                        _scheme.Name = "Схема №" + (i+1);

                        // Инициализировать график
                        var plot = _scheme.Elements.Where(x => x is PRPlot).SingleOrDefault() as PRPlot;
                        plot.InitializatePhaseResponsePlot(_scheme.Model.PhaseResponsePoints);

                        // Создать отображение схемы из полученной модели
                        CreateSchemeInEditor(_scheme);

                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            currentProject.Items.Add(_scheme);
                        });               
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    _dialogCoordinator.ShowMessageAsync(this, "", "Ошибка синтеза" + Environment.NewLine + ex.Message);
                } 
            });
        }

        // Метод для отображения схемы 
        private void CreateSchemeInEditor(FElementScheme scheme) 
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => 
            {
                var canvas = _Page.schemeEditorControl.SchemeControl.CreateSchemeCanvas();

                if (canvas == null)
                    return;

                var editor = _Page.schemeEditorControl.InitializeNewEditor(canvas);
                scheme.Editor = editor;

                SchemeVisualizator.InsertSections(scheme);
            });
     
        }

        /// <summary>
        /// Метод для установки ссылки на страницу
        /// </summary>
        /// <param name="page"></param>
        public void SetPage(Page page)
        {
            _Page = (StructureDesigningPage)page;
        }

        #endregion

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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
