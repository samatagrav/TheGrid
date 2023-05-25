using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Grid.PathFinding;
// ReSharper disable All
#pragma warning disable SYSLIB0006

namespace Grid
{
    public class GridController
    {
        private MainWindow mainWindow;
        public GridController(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        private const int _offSet = 25;
        private const string Separator = "\n";
        private const int _sleepTimeInMiliseconds = 50;
        private bool _sleep = false;
        private DrawingVisual _gridLinesVisual = new DrawingVisual();
        private DrawingGroup _drawingGroup = new DrawingGroup();
        private readonly Color _red = Color.FromRgb(255, 0, 0);
        private readonly Color _black = Color.FromRgb(0, 0, 0);
        private readonly Color _brown = Color.FromRgb(165, 75, 0);
        private readonly Color _greenStart = Color.FromRgb(50,200,50);
        private readonly Color _greenEnd = Color.FromRgb(10, 100, 10);
        private readonly Color _white = Color.FromRgb(255, 255, 255);
        private readonly Color _blue = Color.FromRgb(25, 25, 125);
        private readonly Color _yellow = Color.FromRgb(255,255,0);
        private readonly Color _orange = Color.FromRgb(255,185,25);
        private readonly Color _nectaring = Color.FromRgb(255, 100, 15);
        private Thread pathFinding;

        private Nullable<Point> _lastPoint;
        private int _baseGridHash;
        private NodeHandler _nodeHandler;
        private int _width;
        private int _height;
        private int _offset;
        private int _rows;
        private int _columns;
        private int visited = 0;
        public void InnitGrid()
        {

            _width = (int)mainWindow.GridImage.Width;
            _height = (int)mainWindow.GridImage.Height;
            _offset = 25;
            _rows = _height / _offset;
            _columns = _width / _offset;
            _nodeHandler= new NodeHandler(_rows,_columns, _offset);
            DrawGrid(_rows, _columns);
            PopulateFields(_rows,_columns);
        }

        public void WallClick(Point position)
        {
            Drawing result = _drawingGroup.Children.FirstOrDefault(x => x.Bounds.Contains(position) && !x.Bounds.GetHashCode().Equals(_baseGridHash));
            if (result != null && _lastPoint.HasValue && result.Bounds.Contains(_lastPoint.Value))
            {
                _lastPoint = position;
                return;
            }
            else if (result != null && _lastPoint.HasValue)
            {
                _lastPoint = null;
            }
            else 
            {
                _lastPoint = position;
            }

            if (result == null)
            {
                var node = _nodeHandler.GetNode(position);
                node.SetWall();
                ColourSquare(node.TopLeft, _brown, _brown);
            }
            else
            {
                var node = _nodeHandler.GetNode(position);
                node.SetTile();
                _drawingGroup.Children.Remove(result);
                mainWindow.GridImage.Source = new DrawingImage(_drawingGroup);
            }
        }
        public void Serialize(String name)
        {
            List<string> result =  _nodeHandler.Serialize();
            File.WriteAllLines(name + ".json", result);
        }

        public void LeftClick(Point position)
        {
            Node node = _nodeHandler.GetNode(position);
            Drawing result = _drawingGroup.Children.FirstOrDefault(x => x.Bounds.Contains(position) && !x.Bounds.GetHashCode().Equals(_baseGridHash));
            if (result == null)
            {
                node.SetWall();
                ColourSquare(node.TopLeft, _brown, _brown); 
            }
            else
            {
                node.SetTile();
                _drawingGroup.Children.Remove(result);
                mainWindow.GridImage.Source = new DrawingImage(_drawingGroup);
            }

        }

        public void SetStart(Point position)
        {
            Node node = _nodeHandler.GetNode(position);
            _nodeHandler.Start =node;
            ColourSquare(node.TopLeft, _greenStart, _greenStart);
        }

        public void SetEnd(Point position)
        {
            Node node = _nodeHandler.GetNode(position);
            _nodeHandler.End = _nodeHandler.GetNode(position);
            ColourSquare(node.TopLeft, _greenEnd, _greenEnd);
        }

        private void DrawGrid(int rows, int columns)
        {
            DrawingContext dct = _gridLinesVisual.RenderOpen();
            SolidColorBrush scb = new SolidColorBrush(_black);
            Pen lightPen = new Pen(scb, 0.5), darkPen = new Pen(scb, 1);
            int width = (int)mainWindow.GridImage.Width;
            int height = (int)mainWindow.GridImage.Height;
            int yOffset = 25,
                xOffset = 25,
                rowssHeight = height / yOffset,
                columnss = width / xOffset;
            Point point0 = new Point(0, 0.5);
            Point point1 = new Point(width, 0.5);

            for (int i = 0; i <= rows; i++)
            {
                dct.DrawLine(darkPen, point0, point1);
                point0.Offset(0, yOffset);
                point1.Offset(0, yOffset);
            }
            point0 = new Point(0, 0.5);
            point1 = new Point(0, height);

            for (int i = 0; i <= columns; i++)
            {
                dct.DrawLine(darkPen, point0, point1);
                point0.Offset(xOffset, 0);
                point1.Offset(xOffset, 0);
            }

            dct.Close();
            _baseGridHash = _gridLinesVisual.Drawing.Bounds.GetHashCode();
            AddAndRender(_gridLinesVisual.Drawing);
        }
        private void ColourSquare(Point clickPoint, Color border, Color filling)
        {
            int startX = Normalise(clickPoint.X);
            int startY = Normalise(clickPoint.Y);
            DrawingContext dct = _gridLinesVisual.RenderOpen();
            Rect rect = new Rect(new Point(startX, startY), new Point(startX + _offSet, startY + _offSet));
            SolidColorBrush brush = new SolidColorBrush(filling);
            SolidColorBrush penBrush = new SolidColorBrush(border);
            dct.DrawRectangle(brush, new Pen(penBrush, 1), rect);
            dct.Close();
            AddAndRender(_gridLinesVisual.Drawing);
        }
        private void AddAndRender(Drawing drawing)
        {
            _drawingGroup.Children.Add(drawing);
            mainWindow.GridImage.Source = new DrawingImage(_drawingGroup);
        }

        private int Normalise(double number)
        {
            return (int)number;
        }

        private void PopulateFields(int rows, int columns)
        {
            for (int i = 0; i < rows; i++) 
            {
                for (int j = 0; j < columns; j++)
                {
                    _nodeHandler.CreateNode(j,i,new Point(j*_offset, i*_offset));
                }
            }
        }

        public void Deserialize(string content)
        {
            string[] lines = content.Split(Separator);
            _nodeHandler.Flush();
            int length = lines.Length - 1;
            for (int i = 0; i < length; i++)
            {
                Node node = JsonSerializer.Deserialize<Node>(lines[i]);
                int x = (int)node.TopLeft.X / _offSet;
                int y = (int)node.TopLeft.Y / _offSet;
                _nodeHandler.CreateNode(x, y, node);
                if (node.FieldType == FieldTypeValue.Wall)
                {
                    LeftClick(new Point(node.TopLeft.X + 1, node.TopLeft.Y + 1));
                }
            }

        }

        public void PathFind(int algorithmIndex)
        {
            var start = _nodeHandler.Start;
            var end = _nodeHandler.End;
            if ( start!= null && end != null)
            {
                visited = 0;
                BasePathFinding a;
                _sleep = (bool)mainWindow.cbSlowMode.IsChecked;
                if (algorithmIndex == 0 )
                {
                    a = new AStar(start, end, null, _nodeHandler);
                } else if(algorithmIndex == 1)
                {
                    a = new AStar(start, end, AStar.EuclideanDistance, _nodeHandler);
                }
                else if (algorithmIndex == 2)
                {
                    a = new Dijkstra(start, end, _nodeHandler);
                } else
                {
                    a = new Dijkstra(start, end, _nodeHandler,true);
                }

                a.CurrentMark = ColorCurrent;
                a.VisitedMark = ColorVisited;
                a.QueueMark = ColorQueu;
                pathFinding = new Thread(e =>
                {
                    
                    try
                    {
                        Stopwatch watch = new System.Diagnostics.Stopwatch();
                        System.GC.Collect();
                        watch.Restart();
                        List<Node> result = a.FindPath();
                        watch.Stop();
                        long elasped = watch.ElapsedMilliseconds;
                        if (result == null)
                        {
                            mainWindow.Dispatcher.Invoke(() => { mainWindow.lShortestPath.Content = 0.ToString(); });
                            return;
                        }

                        List<Node> drawingResult = result.GetRange(1, result.Count - 2);
                        drawingResult.ForEach(e =>
                        {
                            mainWindow.Dispatcher.Invoke(() => { ColourSquare(e.TopLeft, _black, _blue); });
                        });
                        mainWindow.Dispatcher.Invoke(() =>
                        {
                            mainWindow.lShortestPath.Content = (result.Count - 1).ToString();
                        });
                        mainWindow.Dispatcher.Invoke(() => { mainWindow.lAlgTime.Content = elasped.ToString(); });
                    }
                    catch (ThreadAbortException exception)
                    {
                        
                    }
                });
                pathFinding.IsBackground = true;
                pathFinding.Start();
            }

        }

        

        public void ColorVisited(Node node)
        {
            if(DontColor(node))
                return;
            mainWindow.Dispatcher.Invoke(() =>
            {
                    ColourSquare(node.TopLeft, _black, _yellow);
            });
            Sleep();
        }

        public void ColorCurrent(Node node)
        {
            if (DontColor(node))
                return;
            mainWindow.Dispatcher.Invoke(() =>
            {
                ColourSquare(node.TopLeft, _black, _red);
                mainWindow.lVisitedNodes.Content = (visited++).ToString();

            });
            Sleep();
        }

        public void ColorQueu(Node node)
        {
            if (DontColor(node))
                return;
            mainWindow.Dispatcher.Invoke(() =>
            {
                ColourSquare(node.TopLeft, _black, _orange);
            });
            Sleep();
        }



        private bool DontColor(Node node)
        {
            return _nodeHandler.Start.Equals(node) || _nodeHandler.End.Equals(node);
        }

        public void Clear(bool clearWalls,bool clearStartEnd)
        {
            List<Node> nodeList = _nodeHandler.GetNodes().SelectMany(x => x).ToList();
            if (!clearStartEnd)
            {
                nodeList = nodeList.Where(x => !x.Equals(_nodeHandler.Start) && !x.Equals(_nodeHandler.End)).ToList();
            }

            IEnumerable<Drawing> toRemove;
            if (clearWalls)
            {
                nodeList.ForEach(x => x.SetTile());
                toRemove = _drawingGroup.Children.Where(x => !x.Bounds.GetHashCode().Equals(_baseGridHash));
            }
            else
            {
               toRemove = _drawingGroup.Children.Where(x => 
                   !x.Bounds.GetHashCode().Equals(_baseGridHash) && 
                   nodeList.Any(n => 
                       n.IsPassable() && 
                       x.Bounds.Contains(PointPlus1(n.TopLeft))));
            }

            toRemove = toRemove.ToList();
            foreach (var drawing in toRemove)
            {
                _drawingGroup.Children.Remove(drawing);
            }

            if (clearStartEnd)
            {
                _nodeHandler.Start = null;
                _nodeHandler.End = null;
            }

            visited = 0;
            mainWindow.lVisitedNodes.Content = "";
            mainWindow.lShortestPath.Content = "";
            mainWindow.lAlgTime.Content = "";
        }

        private void Sleep()
        {
            if (_sleep)
            {
                Thread.Sleep(_sleepTimeInMiliseconds);
            }
        }

        private static Point PointPlus1(Point point)
        {
            return new Point(point.X + 1, point.Y + 1);
        }

        public void Stop()
        {
            pathFinding.Abort();
        }
    }
}