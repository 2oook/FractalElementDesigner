using RC_FE_Design___Analysis_and_synthesis.IO.ProjectSaveModel;
using RC_FE_Design___Analysis_and_synthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RC_FE_Design___Analysis_and_synthesis.FEEditor;
using System.Collections.ObjectModel;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model.Cells;

namespace RC_FE_Design___Analysis_and_synthesis.IO
{
    public class ProjectConverter
    {
        public static SavingProject Convert(Project project) 
        {
            var structures = new ObservableCollection<SavingRCStructureBase>();

            foreach (var structure in project.Structures)
            {
                var structureLayers = new ObservableCollection<SavingLayer>();

                foreach (var layer in structure.StructureLayers)
                {
                    var structureCells = new ObservableCollection<ObservableCollection<SavingStructureCellBase>>();

                    foreach (var row in layer.StructureCells)
                    {
                        var _row = new ObservableCollection<SavingStructureCellBase>();

                        foreach (var col in row)
                        {
                            _row.Add(new SavingStructureCellBase() { CellType = col.CellType });
                        }

                        structureCells.Add(_row);
                    }

                    structureLayers.Add(new SavingLayer()
                    {
                        Name = layer.Name,
                        CellsType = layer.CellsType,
                        StructureCells = structureCells
                    });
                }

                structures.Add(new SavingRCStructureBase()
                {
                    Name = structure.Name,
                    StructureLayers = structureLayers
                });
            }

            var savingProject = new SavingProject()
            {
                Name = project.Name,
                Structures = structures
            };

            return savingProject;
        }

        public static Project ConvertBack(SavingProject savingProject) 
        {
            var structures = new ObservableCollection<RCStructureBase>();

            foreach (var structure in savingProject.Structures)
            {
                var structureLayers = new ObservableCollection<Layer>();

                foreach (var layer in structure.StructureLayers)
                {
                    var structureCells = new ObservableCollection<ObservableCollection<StructureCellBase>>();

                    foreach (var row in layer.StructureCells)
                    {
                        var _row = new ObservableCollection<StructureCellBase>();

                        foreach (var col in row)
                        {
                            _row.Add(new StructureCellBase() { CellType = col.CellType });
                        }

                        structureCells.Add(_row);
                    }

                    structureLayers.Add(new Layer()
                    {
                        Name = layer.Name,
                        CellsType = layer.CellsType,
                        StructureCells = structureCells
                    });
                }

                structures.Add(new RCStructureBase()
                {
                    Name = structure.Name,
                    StructureLayers = structureLayers
                });
            }

            var project = new Project()
            {
                Name = savingProject.Name,
                Structures = structures
            };

            return project;
        }
    }
}
