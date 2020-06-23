﻿using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Editor
{
    public class Context
    {
        #region Properties

        public ISchemeCreator DiagramCreator { get; set; }
        public IClipboard Clipboard { get; set; }

        #endregion

        #region Fields

        public string ClipboardText = null;

        public ITree CurrentTree = null;
        public ICanvas CurrentCanvas = null;

        public ILine CurrentLine = null;
        public IElement CurrentRoot = null;

        public PointEx RightClick;

        public bool EnableInsertLast = false;
        public string LastInsert = Constants.TagElementFElement;

        public double DefaultGridSize = 30;

        public bool EnableSnap = true;
        public bool SnapOnRelease = false;

        public bool MoveAllSelected = false;

        public bool SkipContextMenu = false;
        public bool SkipLeftClick = false;

        public PointEx PanStart;
        public double PreviousScrollOffsetX = -1.0;
        public double PreviousScrollOffsetY = -1.0;

        public double ZoomInFactor = 0.1;
        public double ZoomOutFactor = 0.1;

        public double ZoomLogBase = 1.9;
        public double ZoomExpFactor = 1.3;

        public PointEx ZoomPoint;

        public double ReversePanDirection = -1.0; // reverse: 1.0, normal: -1.0

        public PointEx SelectionOrigin;

        public LinkedList<IElement> SelectedThumbList = null;
        public LinkedListNode<IElement> CurrentThumbNode = null;

        #endregion

        #region Hooks

        public Action UpdateProperties { get; set; }
        public Action<SchemeProperties> SetProperties { get; set; }

        public Func<ITreeItem> CreateSolution { get; set; }
        public Func<ITreeItem> CreateProject { get; set; }
        public Func<ITreeItem> CreateDiagram { get; set; }

        public Func<bool> IsControlPressed { get; set; }

        #endregion
    }
}
