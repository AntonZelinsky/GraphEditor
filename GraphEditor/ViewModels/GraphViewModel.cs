using System.Linq;   
using System.Windows;  
using GraphEditor.Helper;
using GraphEditor.Models; 
using System.Windows.Input;       
using System.Collections.Generic;
using System.Windows.Media;

namespace GraphEditor.ViewModels
{
    public sealed class GraphViewModel
    {
        private GraphModel _graphModel;

        public readonly CommandBindingCollection CommandBindings;

        public GraphViewModel(GraphModel graphModel)
        {
            _graphModel = graphModel;      

            RegisterChangedEvent();
        }

        public void CreateGraph(List<Vertex> verticies, List<Edge> edges)
        {
            verticies.ForEach(v => AddVertex(v));
            edges.ForEach(e => AddEdge(e));
        }

        #region Change Model

        public delegate void Change();

        public event Change ModelChanged;

        public string FileName
        {
            get { return _graphModel.FileName; }
            private set
            {
                _graphModel.FileName = value;
                ModelChanged?.Invoke();
            }
        }

        public bool Changed
        {
            get { return _graphModel.Changed; }
            private set
            {
                _graphModel.Changed = value;
                ModelChanged?.Invoke();
            }
        }                                  

        private void RegisterChangedEvent()
        {
            AddedVertex += delegate { Changed = true; };
            AddedEdge += delegate { Changed = true; };
            ChangedPositionVertex += delegate { Changed = true; };
            RemovedElement += delegate { Changed = true; };
            UpdateLabel += delegate { Changed = true; };
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
            if (_graphModel.ContainsVerticies(v.Id))
                return false;

            _graphModel.AddVertex(v);
            AddedVertex?.Invoke(v);  
            return true;
        } 

        /// <summary>
        /// Insert a Edge into the graph
        /// </summary>
        /// <param name="fromId"></param>
        /// <param name="toId"></param>
        /// <returns>status</returns>
        public bool AddEdge(int fromId, int toId)
        {
            return AddEdge(new Edge(fromId, toId));
        }

        /// <summary>
        /// Insert a Edge into the graph
        /// </summary>
        /// <param name="e">the Edge</param>
        /// <returns>status</returns>
        private bool AddEdge(Edge e)
        {
            if(_graphModel.ContainsEdges(e.Id))
                return false; 
            _graphModel.AddEdge(e);

            _graphModel.GetVertex(e.FromId).EdgesId.Add(e.Id);
            _graphModel.GetVertex(e.ToId).EdgesId.Add(e.Id);
                                                          
            AddedEdge?.Invoke(e);
            return true;
        }

        #endregion Add element

        #region Selected elements

        private readonly List<int> _selectedElements = new List<int>();

        public int SelectedElementsCount => _selectedElements.Count;

        public delegate void SelectElement(int id);
        public event SelectElement SelectedElement; 
        public event SelectElement UnselectedElement;   

        public void AddSelectedElement(int id, bool ctrl, bool multi)
        {
            if(SelectedElementsCount != 0 & !multi && !ctrl) 
                UnselectElements();
            if(_selectedElements.Contains(id))
            {
                _selectedElements.Remove(id);
                UnselectedElement?.Invoke(id);
            }
            else
            {
                _selectedElements.Add(id);
                SelectedElement?.Invoke(id);
            }        
        }
                  
        public void SelectAll()
        {
            _selectedElements.AddRange(_graphModel.GetAllElements().Select(e => e.Id)); 
        }

        public void UnselectElements()
        {                     
            _selectedElements.ForEach(id =>
            {                                   
                UnselectedElement?.Invoke(id);
            });
            _selectedElements.Clear();  
        }


        public delegate void ChangePositionVertex(int id, Point p);
        public event ChangePositionVertex ChangedPositionVertex;
        public void ChangePosition(Point p)
        {
            _selectedElements.ForEach(v =>
            {
                if (!_graphModel.ContainsVerticies(v)) 
                    return;
                var vc = _graphModel.GetVertex(v);
                var vector = vc.Position - p;
                if(vector.X == 0 && vector.Y == 0)
                    return;

                vc.PositionX =  p.X;
                vc.PositionY = p.Y;
                ChangedPositionVertex?.Invoke(v, p);
            });
        }

        #endregion
       
        #region Remove Element

        public event SelectElement RemovedElement;

        public void RemoveSelectedElements()
        {
            if(SelectedElementsCount < 0)
                return;
            _selectedElements.ForEach(RemoveElement);
            _selectedElements.Clear();
        }

        private void RemoveElement(int id)
        {
            RemovedElement?.Invoke(id);
            if (_graphModel.VertexOfEdgesById.ContainsKey(id))
            {       
                _graphModel.VertexOfEdgesById[id].ForEach(RemoveElement);   
            }     

            _graphModel.RemoveById(id);    
        }

        private void RemoveAllElements()
        {
            _graphModel.Verticies.Keys.ToList().ForEach(RemoveElement);
            _graphModel.Edges.Keys.ToList().ForEach(RemoveElement);
            _selectedElements.Clear();
        }

        #endregion

        #region Label Control


        public delegate void LabelName(int id, string name); 
        public event LabelName UpdateLabel;
                               
        public void UpdeteElementLabel(int id, string name)
        {
            _graphModel.GetElement(id).LabelName = name;
            UpdateLabel?.Invoke(id, name);
        }

        #endregion

        #region Commands

        public void NewFile()
        {
            RemoveAllElements();
            _graphModel = new GraphModel();
        }

        public void LoadFile(GraphModelSerialization model)
        {
            NewFile();
            model.Verticies.ForEach(v => AddVertex(v));
            model.Edges.ForEach(es => AddEdge(es));
            Changed = model.Changed;
            FileName = model.FileName;
        }

        public GraphModel GetModel()
        {
            return _graphModel;
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
