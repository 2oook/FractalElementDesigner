using RC_FE_Design___Analysis_and_synthesis.SchemeEditor;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model; 

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Editor
{
    public class Parser : ISchemeParser
    {
        public TreeSolution Parse(string model, ISchemeCreator creator, ParseOptions options)
        {
            if (model == null || creator == null || options == null)
                return null;

            double offsetX = options.OffsetX;
            double offsetY = options.OffsetY;
            bool appendIds = options.AppendIds;
            bool updateIds = options.UpdateIds;
            bool select = options.Select;
            bool createElements = options.CreateElements;
            string name = null;
            string root = null;
            var counter = new IdCounter();
            var total = new IdCounter();
            var elements = new List<object>();
            Child child = null;
            var dict = new Dictionary<string, Child>();
            TreeSolution solution = null;
            TreeProjects projects = null;
            TreeProject project = null;
            TreeSchemes diagrams = null;
            TreeScheme diagram = null;

            var lines = model.Split(Environment.NewLine.ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var args = GetArgs(line);

                int length = args.Length;
                if (length < 2)
                    continue;

                root = args[0];
                name = args[1];

                // root element
                if (StringHelper.Compare(root, Constants.PrefixRoot))
                {
                    // Solution
                    if (StringHelper.StartsWith(name, Constants.TagHeaderSolution) &&
                        (length == 2 || length == 3 || length == 4))
                    {
                        int id = int.Parse(name.Split(Constants.TagNameSeparator)[1]);
                        counter.Set(Math.Max(counter.Count, id + 1));

                        total.Next();

                        projects = new TreeProjects();
                        solution = new TreeSolution(name, projects);
                    }

                    // Project
                    else if (StringHelper.StartsWith(name, Constants.TagHeaderProject) &&
                        length == 2)
                    {
                        int id = int.Parse(name.Split(Constants.TagNameSeparator)[1]);
                        counter.Set(Math.Max(counter.Count, id + 1));

                        total.Next();

                        if (projects != null)
                        {
                            diagrams = new TreeSchemes();
                            project = new TreeProject(name, diagrams);
                            projects.Push(project);
                        }
                    }

                    // Diagram
                    else if (StringHelper.StartsWith(name, Constants.TagHeaderDiagram) &&
                        length == 13)
                    {
                        int id = int.Parse(name.Split(Constants.TagNameSeparator)[1]);
                        counter.Set(Math.Max(counter.Count, id + 1));

                        total.Next();

                        if (diagrams != null)
                        {
                            diagram = new TreeScheme();
                            diagrams.Push(diagram);
                            diagram.Push(line);
                        }

                        if (createElements == true)
                        {
                            var prop = new SchemeProperties();

                            prop.PageWidth = int.Parse(args[2]);
                            prop.PageHeight = int.Parse(args[3]);
                            prop.GridOriginX = int.Parse(args[4]);
                            prop.GridOriginY = int.Parse(args[5]);
                            prop.GridWidth = int.Parse(args[6]);
                            prop.GridHeight = int.Parse(args[7]);
                            prop.GridSize = int.Parse(args[8]);
                            prop.SnapX = double.Parse(args[9]);
                            prop.SnapY = double.Parse(args[10]);
                            prop.SnapOffsetX = double.Parse(args[11]);
                            prop.SnapOffsetY = double.Parse(args[12]);

                            creator.CreateDiagram(prop);

                            options.Properties = prop;
                        }
                    }

                    // Pin
                    else if (StringHelper.StartsWith(name, Constants.TagElementPin) &&
                        length == 4)
                    {
                        if (diagram != null)
                            diagram.Push(line);

                        total.Next();

                        if (createElements == true)
                        {
                            double x = double.Parse(args[2]);
                            double y = double.Parse(args[3]);
                            int id = int.Parse(name.Split(Constants.TagNameSeparator)[1]);

                            counter.Set(Math.Max(counter.Count, id + 1));

                            var element = creator.CreateElement(Constants.TagElementPin,
                                new object[] { id },
                                x + offsetX, y + offsetY, false);
                            elements.Add(element);

                            child = new Child(element, new List<Pin>());

                            if (dict.ContainsKey(name) == false)
                                dict.Add(name, child);
                            else
                                System.Diagnostics.Debug.Print("Dictionary already contains name key: {0}", name);
                        }
                    }

                    // AndGate
                    else if (StringHelper.StartsWith(name, Constants.TagElementAndGate) &&
                        length == 4)
                    {
                        if (diagram != null)
                            diagram.Push(line);

                        total.Next();

                        if (createElements == true)
                        {
                            double x = double.Parse(args[2]);
                            double y = double.Parse(args[3]);
                            int id = int.Parse(name.Split(Constants.TagNameSeparator)[1]);

                            counter.Set(Math.Max(counter.Count, id + 1));

                            var element = creator.CreateElement(Constants.TagElementAndGate,
                                new object[] { id },
                                x + offsetX, y + offsetY, false);
                            elements.Add(element);

                            child = new Child(element, new List<Pin>());

                            if (dict.ContainsKey(name) == false)
                                dict.Add(name, child);
                            else
                                System.Diagnostics.Debug.Print("Dictionary already contains name key: {0}", name);
                        }
                    }

                    // FEElement
                    else if (StringHelper.StartsWith(name, Constants.TagElementFElement) &&
                        length == 4)
                    {
                        if (diagram != null)
                            diagram.Push(line);

                        total.Next();

                        if (createElements == true)
                        {
                            double x = double.Parse(args[2]);
                            double y = double.Parse(args[3]);
                            int id = int.Parse(name.Split(Constants.TagNameSeparator)[1]);

                            counter.Set(Math.Max(counter.Count, id + 1));

                            var element = creator.CreateElement(Constants.TagElementFElement,
                                new object[] { id },
                                x + offsetX, y + offsetY, false);
                            elements.Add(element);

                            child = new Child(element, new List<Pin>());

                            if (dict.ContainsKey(name) == false)
                                dict.Add(name, child);
                            else
                                System.Diagnostics.Debug.Print("Dictionary already contains name key: {0}", name);
                        }
                    }


                    // Wire
                    else if (StringHelper.StartsWith(name, Constants.TagElementWire) &&
                        (length == 6 || length == 8 || length == 10))
                    {
                        if (diagram != null)
                            diagram.Push(line);

                        total.Next();

                        if (createElements == true)
                        {
                            double x1 = double.Parse(args[2]);
                            double y1 = double.Parse(args[3]);
                            double x2 = double.Parse(args[4]);
                            double y2 = double.Parse(args[5]);
                            bool startVisible = (length == 8 || length == 10) ? bool.Parse(args[6]) : false;
                            bool endVisible = (length == 8 || length == 10) ? bool.Parse(args[7]) : false;
                            bool startIsIO = (length == 10) ? bool.Parse(args[8]) : false;
                            bool endIsIO = (length == 10) ? bool.Parse(args[9]) : false;
                            int id = int.Parse(name.Split(Constants.TagNameSeparator)[1]);

                            counter.Set(Math.Max(counter.Count, id + 1));

                            var element = creator.CreateElement(Constants.TagElementWire,
                                new object[] 
                                {
                                    x1 + offsetX, y1 + offsetY,
                                    x2 + offsetX, y2 + offsetY,
                                    startVisible, endVisible,
                                    startIsIO, endIsIO,
                                    id
                                },
                                0.0, 0.0, false);
                            elements.Add(element);

                            child = new Child(element, new List<Pin>());

                            if (dict.ContainsKey(name) == false)
                                dict.Add(name, child);
                            else
                                System.Diagnostics.Debug.Print("Dictionary already contains name key: {0}", name);
                        }
                    }
                }

                // child element
                else if (StringHelper.Compare(root, Constants.PrefixChild))
                {
                    if (StringHelper.StartsWith(name, Constants.TagElementWire) &&
                        length == 3)
                    {
                        if (diagram != null)
                            diagram.Push(line);

                        if (createElements == true && child != null)
                        {
                            var pins = child.Pins;

                            pins.Add(new Pin(name, args[2]));
                        }
                    }
                }
            }

            if (createElements == true)
            {
                creator.UpdateConnections(dict);

                if (appendIds == true)
                    creator.AppendIds(elements);

                if (updateIds == true)
                    creator.UpdateCounter(options.Counter, counter);

                creator.InsertElements(elements, select, offsetX, offsetY);
            }

            return solution;
        }

        private static string[] GetArgs(string line)
        {
            return line.Split(new char[] { Constants.ArgumentSeparator, '\t', ' ' },
                StringSplitOptions.RemoveEmptyEntries);
        }
    } 
}
