using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
// ReSharper disable All

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
        private DrawingVisual _gridLinesVisual = new DrawingVisual();
        private DrawingGroup _drawingGroup = new DrawingGroup();
        private readonly Color _red = Color.FromRgb(255, 0, 0);
        private readonly Color _black = Color.FromRgb(0, 0, 0);
        private readonly Color _brown = Color.FromRgb(165, 75, 0);
        private readonly Color _greenStart = Color.FromRgb(50,200,50);
        private readonly Color _greenEnd = Color.FromRgb(10, 100, 10);
        private readonly Color _white = Color.FromRgb(255, 255, 255);
        private readonly Color _pink = Color.FromRgb(255, 192, 203);

        private Nullable<Point> _lastPoint;
        private int _baseGridHash;
        private NodeHandler _nodeHandler;
        private int _width;
        private int _height;
        private int _offset;
        private int _rows;
        private int _columns;
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
                ColourSquare(node.TopLeft, _brown, _brown);
            }
            else
            {
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

            ColourSquare(position, _greenStart, _greenStart);
        }

        public void SetEnd(Point position)
        {
            ColourSquare(position, _greenEnd, _greenEnd);
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
            var brush = new SolidColorBrush(filling);
            var penBrush = new SolidColorBrush(border);
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
            for (int i = 0; i < _columns; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    Node n = JsonSerializer.Deserialize<Node>(lines[j*_rows+ i]);
                    _nodeHandler.CreateNode(j, i, n);
                    if (n.FieldType == FieldTypeValue.Wall)
                    {
                        LeftClick(new Point(j * _offset + 1, i * _offset + 1));
                    }
                }
            }
        }
    }
}