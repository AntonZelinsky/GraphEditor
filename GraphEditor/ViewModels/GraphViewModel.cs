using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GraphEditor.Models;

namespace GraphEditor.ViewModels
{
    public sealed class GraphViewModel
    {
        public readonly CommandBindingCollection CommandBindings;

        public GraphViewModel(GraphModel graphModel)
        {
            GetModel = graphModel;

            RegisterChangedEvent();
        }

        public GraphModel GetModel { get; }

        public void CreateGraph(List<Vertex> verticies, List<Edge> edges)
        {
            verticies.ForEach(v => AddVertex(v));
            edges.ForEach(e => AddEdge(e));
        }

        #region Change Model

        public delegate void Change(GraphViewModel model);

        public event Change ModelChanged;

        public string FileName
        {
            get { return GetModel.FileName; }
            private set
            {
                GetModel.FileName = value;
                ModelChanged?.Invoke(this);
            }
        }

        public bool Changed
        {
            get { return GetModel.Changed; }
            private set
            {
                GetModel.Changed = value;
                ModelChanged?.Invoke(this);
            }
        }

        private void RegisterChangedEvent()
        {
            AddedVertex += delegate
            {
                Changed = true;
                ModelChanged?.Invoke(this);
            };
            AddedEdge += delegate
            {
                Changed = true;
                ModelChanged?.Invoke(this);
            };
            RemovedElement += delegate
            {
                Changed = true;
                ModelChanged?.Invoke(this);
            };
            UpdateLabel += delegate
            {
                Changed = true;
                ModelChanged?.Invoke(this);
            };
        }

        #endregion ChangeModel

        #region Add element

        public delegate void AddedElement(IElement el);

        public event AddedElement AddedVertex;
        public event AddedElement AddedEdge;

        public bool AddVertex(Point p)
        {
            var v = new Vertex(p);
            return AddVertex(v);
        }

        private bool AddVertex(Vertex v)
        {
            if (GetModel.ContainsVerticies(v.Id))
                return false;

            GetModel.AddVertex(v);
            AddedVertex?.Invoke(v);
            return true;
        }

        /// <summary>
        ///     Insert a Edge into the graph
        /// </summary>
        /// <param name="fromId"></param>
        /// <param name="toId"></param>
        /// <returns>status</returns>
        public bool AddEdge(int fromId, int toId)
        {
            return AddEdge(new Edge(fromId, toId));
        }

        /// <summary>
        ///     Insert a Edge into the graph
        /// </summary>
        /// <param name="e">the Edge</param>
        /// <returns>status</returns>
        private bool AddEdge(Edge e)
        {
            if (GetModel.ContainsEdges(e.Id))
                return false;
            GetModel.AddEdge(e);

            GetModel.GetVertex(e.FromId).EdgesId.Add(e.Id);
            GetModel.GetVertex(e.ToId).EdgesId.Add(e.Id);

            AddedEdge?.Invoke(e);
            return true;
        }

        #endregion Add element

        #region Selected elements

        public readonly List<int> SelectedElements = new List<int>();

        public int SelectedElementsCount => SelectedElements.Count;

        public delegate void SelectElement(int id);

        public event SelectElement SelectedElement;
        public event SelectElement UnselectedElement;

        public void AddSelectedElement(int id, bool ctrl, bool multi)
        {
            if (SelectedElementsCount != 0 & !multi && !ctrl)
                UnselectElements();
            if (SelectedElements.Contains(id))
            {
                SelectedElements.Remove(id);
                UnselectedElement?.Invoke(id);
            }
            else
            {
                SelectedElements.Add(id);
                SelectedElement?.Invoke(id);
            }
            ModelChanged?.Invoke(this);
        }

        public void SelectAll()
        {
            SelectedElements.AddRange(GetModel.GetAllElements().Select(e => e.Id));
        }

        public void UnselectElements()
        {
            SelectedElements.ForEach(id => UnselectedElement?.Invoke(id));
            SelectedElements.Clear();
            ModelChanged?.Invoke(this);
        }

        public void ChangePosition(Point p)
        {
            SelectedElements.ForEach(v =>
            {
                if (!GetModel.ContainsVerticies(v))
                    return;
                var vc = GetModel.GetVertex(v);

                vc.PositionX = p.X;
                vc.PositionY = p.Y;
            });
            Changed = true;
        }

        #endregion

        #region Remove Element

        public event SelectElement RemovedElement;

        public void RemoveSelectedElements()
        {
            if (SelectedElementsCount < 0)
                return;
            SelectedElements.ForEach(RemoveElement);
            SelectedElements.Clear();
        }

        private void RemoveElement(int id)
        {
            RemovedElement?.Invoke(id);
            if (GetModel.VertexOfEdgesById.ContainsKey(id))
            {
                GetModel.VertexOfEdgesById[id].ForEach(RemoveElement);
            }

            GetModel.RemoveById(id);
        }

        private void RemoveAllElements()
        {
            GetModel.Verticies.Keys.ToList().ForEach(RemoveElement);
            GetModel.Edges.Keys.ToList().ForEach(RemoveElement);
            SelectedElements.Clear();
        }

        #endregion

        #region Label Control

        public delegate void LabelName(int id, string name);

        public event LabelName UpdateLabel;

        public void UpdeteElementLabel(int id, string name)
        {
            GetModel.GetElement(id).LabelName = name;
            UpdateLabel?.Invoke(id, name);
        }

        #endregion

        #region Commands

        public void LoadFile(GraphModelSerialization model)
        {
            model.Verticies.ForEach(v => AddVertex(v));
            model.Edges.ForEach(es => AddEdge(es));
            Changed = model.Changed;
            FileName = model.FileName;
        }

        public void SaveFile(GraphModelSerialization model)
        {
            Changed = model.Changed;
            FileName = model.FileName;
        }

        #endregion Commands        

        #region Algoritm command

        public delegate Color ChangeColorElement(int id, Color color);

        public event ChangeColorElement ChangedColor;

        public Color? ChangeColor(int idElement, Color color)
        {
            return ChangedColor?.Invoke(idElement, color);
        }

        public delegate void ResetColorElement();

        public event ResetColorElement ResetedColors;

        public void ResetColor()
        {
            ResetedColors?.Invoke();
        }

        #endregion
    }
}