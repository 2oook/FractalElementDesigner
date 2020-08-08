using FractalElementDesigner.MathModel;
using FractalElementDesigner.SchemeEditing.Core;
using FractalElementDesigner.SchemeEditing.Editor;
using FractalElementDesigner.SchemeEditing.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.SchemeEditing
{
    class SchemeVisualizator
    {
        public static void InsertVisual(FElementScheme scheme, SchemeControl structureEditorControl)
        {
            scheme.Editor.Add(scheme.Editor.Context.CurrentCanvas, Constants.TagElementFElement, new PointEx(100, 200));

            //// вставить слои
            //foreach (var layer in structure.StructureLayers)
            //{
            //    var editor = new Editor() { Context = new Context() };
            //    editor.Context.CurrentCanvas = structureEditorControl.CreateFECanvas();

            //    layer.Editor = editor;

            //    Insert.StructureLayer(editor.Context.CurrentCanvas as FECanvas, layer, layer.CellsType);
            //}
        }
    }
}
