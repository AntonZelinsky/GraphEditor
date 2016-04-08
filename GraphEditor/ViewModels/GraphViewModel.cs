using System;
using System.Linq;   
using System.Windows;        
using System.ComponentModel;
using System.Collections.Generic;  
using System.Runtime.CompilerServices;         
using GraphEditor.Models;       

namespace GraphEditor.ViewModels
{
    public sealed class GraphViewModel : INotifyPropertyChanged
    {
        #region Properties          

        private readonly GraphModel _graphModel;

        public delegate void AddedElement(IElement el);
        public event AddedElement AddedVertex;
        public event AddedElement AddedEdge;
      
        #endregion

        public GraphViewModel(GraphModel graphModel)
        {
            _graphModel = graphModel;                                                       
        }

        public void CreateGraph(List<Vertex> verticies, List<Edge> edges)
        {                                                   
            verticies.ForEach(v => AddVertex(v));
            edges.ForEach(e => AddEdge(e));                                                     
        }

        public bool AddVertex(Point p)
        {                      
            var v = new Vertex(p);
            return AddVertex(v);
        }

        public bool AddVertex(Vertex v)
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
        public bool AddEdge(Edge e)
        {
            if(_graphModel.ContainsEdges(e.Id))
                return false; 
            _graphModel.AddEdge(e);

            _graphModel.GetVertex(e.FromId).EdgesId.Add(e.Id);
            _graphModel.GetVertex(e.ToId).EdgesId.Add(e.Id);
                                                          
            AddedEdge?.Invoke(e);
            return true;
        }

        #region Selected elements
                 
        private List<int> SelectedElements = new List<int>();

        public int SelectedElementsCount => SelectedElements.Count;

        public delegate void SelectElement(int id);
        public event SelectElement SelectedElement; 
        public event SelectElement UnselectedElement;   

        public void AddSelectedElement(int id, bool ctrl, bool multi)
        {
            if(SelectedElementsCount != 0 & !multi && !ctrl) 
                UnselectElements();
            if(SelectedElements.Contains(id))
            {
                SelectedElements.Remove(id);
                UnselectedElement?.Invoke(id);
            }
            else
            {
                SelectedElements.Add(id);
                SelectedElement?.Invoke(id);
            }        
        }
                  
        public void SelectAll()
        {
            SelectedElements.AddRange(_graphModel.GetAllElements().Select(e => e.Id)); 
        }

        public void UnselectElements()
        {                     
            SelectedElements.ForEach(id =>
            {                                   
                UnselectedElement?.Invoke(id);
            });
            SelectedElements.Clear();  
        }


        public delegate void ChangePositionVertex(int id, Point p);
        public event ChangePositionVertex ChangedPositionVertex;
        public void ChangePosition(Point p)
        {
            SelectedElements.ForEach(v =>
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

        #region IElement Control 

  

        #region Remove IElement

        public SelectElement RemovedElement;

        public void RemoveElements()
        {
            if(SelectedElementsCount < 0)
                return;
            SelectedElements.ForEach(RemoveElement);
            SelectedElements.Clear();
        }

        public void RemoveElement(int id)
        {
            RemovedElement?.Invoke(id);
            if (_graphModel.VertexOfEdgesById.ContainsKey(id))
            {       
                _graphModel.VertexOfEdgesById[id].ForEach(RemoveElement);   
            }


            _graphModel.RemoveById(id);    
        }

        #endregion

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
         
        public event PropertyChangedEventHandler PropertyChanged;
                                           
        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
