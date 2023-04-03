using Projekat1.Command;
using Projekat1.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Point = System.Windows.Point;

namespace Projekat1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int countOfElement = 0;
        public List<Point> pocetnaMreza = new List<Point>();
        public List<PowerEntity> allElements = new List<PowerEntity>();
        public double newX, newY, minX, minY, maxX, maxY, distanceX, distanceY, realX, realY;
        public List<double> allX = new List<double>();
        public List<double> allY = new List<double>();
        public Dictionary<Point, PowerEntity> pairs = new Dictionary<Point, PowerEntity>();
        public List<LineEntity> lines = new List<LineEntity>();
        public Point startpoint, endpoint;

        //animacije
        public string currentMode;
        public Ellipse remEllipse1 = null;
        public Ellipse remEllipse2 = null;
        public Brush remBrush1 = null;
        public Brush remBrush2 = null;
        public List<UIElement> uIElements = new List<UIElement>();
        public Storyboard story = new Storyboard();
        public DoubleAnimation startAnimation1 = new DoubleAnimation();
        public DoubleAnimation startAnimation2 = new DoubleAnimation();
        public DoubleAnimation endAnimation1 = new DoubleAnimation();
        public DoubleAnimation endAnimation2 = new DoubleAnimation();

        //za poligon
        public bool isSelectedPoints;
        public List<Point> polygonPoints;

        //za undo redo
        public bool clearState;
        public readonly CommandStack commandStack = new CommandStack();

        public MainWindow()
        {
            InitializeComponent();
            ClearBtn.IsEnabled = false; 
            startAnimation1.From = 2;
            startAnimation1.To = 4;
            startAnimation1.AutoReverse = true;
            startAnimation1.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTargetProperty(startAnimation1, new PropertyPath(Ellipse.WidthProperty));

            startAnimation2.From = 2;
            startAnimation2.To = 4;
            startAnimation2.AutoReverse = true;
            startAnimation2.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTargetProperty(startAnimation2, new PropertyPath(Ellipse.HeightProperty));

            endAnimation1.From = 2;
            endAnimation1.To = 4;
            endAnimation1.AutoReverse = true;
            endAnimation1.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTargetProperty(endAnimation1, new PropertyPath(Ellipse.WidthProperty));

            endAnimation2.From = 2;
            endAnimation2.To = 4;
            endAnimation2.AutoReverse = true;
            endAnimation2.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTargetProperty(endAnimation2, new PropertyPath(Ellipse.HeightProperty));


        }
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
           
            Nodes.Visibility = Visibility.Visible;
            Switches.Visibility = Visibility.Visible;
            Substations.Visibility = Visibility.Visible;
            ElNodes.Visibility = Visibility.Visible;
            ElSwitches.Visibility = Visibility.Visible;
            ElSubstations.Visibility = Visibility.Visible;
            
            
            pocetnaMreza = Mreza();

            LoadElements();


            Scale();
           
            Write(); 
            
            LoadLines();

            foreach(LineEntity line in lines)
            {
                StartAndEndOfLine(line, out startpoint, out endpoint);
                WriteLines(startpoint, endpoint, line, canvas);
            }
            LoadButton.IsEnabled = false;

        }

        //crtanje modela
        private List<Point> Mreza()
        {

            Point p;
            List<Point> lista = new List<Point>();
            
            for (int i = 0; i < 900; i += 3)
            {
                for (int j = 0; j < 600; j += 2)
                {
                    p = new Point(i,j);
                    lista.Add(p);
                }

            }

            return lista;
        }
        private void LoadElements()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Geographic.xml");
            XmlNodeList nodeList;
            CultureInfo culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;

            //substations
            nodeList = doc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
          
            foreach ( XmlNode node in nodeList)
            {
                  SubstationEntity sub = new SubstationEntity();
                sub.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                sub.Name = node.SelectSingleNode("Name").InnerText;
                sub.X = double.Parse(node.SelectSingleNode("X").InnerText);
                sub.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
                sub.ToolTip = $"Substation: Id-{sub.Id} Name-{sub.Name}" ;
               
                allElements.Add(sub);

                ToLatLon(sub.X, sub.Y,34, out newX, out newY);

                allX.Add(newX);
                allY.Add(newY);
                
            }


            //switches
            nodeList = doc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            foreach (XmlNode node in nodeList)
            {
                SwitchEntity sw = new SwitchEntity();
                sw.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                sw.Name = node.SelectSingleNode("Name").InnerText;
                sw.X = double.Parse(node.SelectSingleNode("X").InnerText);
                sw.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
                sw.Status = node.SelectSingleNode("Status").InnerText;
                sw.ToolTip = $"Switch: ID-{sw.Id} Name-{sw.Name}";
               

                allElements.Add(sw);

                ToLatLon(sw.X, sw.Y, 34, out newX, out newY);

                allX.Add(newX);
                allY.Add(newY);
            }

            //nodes
            nodeList = doc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            foreach (XmlNode node in nodeList)
            {
                NodeEntity nod = new NodeEntity();
                nod.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                nod.Name = node.SelectSingleNode("Name").InnerText;
                nod.X = double.Parse(node.SelectSingleNode("X").InnerText);
                nod.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

                nod.ToolTip = $"Node: ID-{nod.Id} Name-{nod.Name}";
               

                allElements.Add(nod);

                ToLatLon(nod.X, nod.Y, 34, out newX, out newY);

                allX.Add(newX);
                allY.Add(newY);
            }

           
        }
        private void LoadLines()
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");
            XmlNodeList nodeList;
            CultureInfo culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            foreach (XmlNode node in nodeList)
            {
                LineEntity line = new LineEntity();
                line.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                line.Name = node.SelectSingleNode("Name").InnerText;
                if (node.SelectSingleNode("IsUnderground").InnerText.Equals("true"))
                {
                    line.IsUnderground = true;
                }
                else
                {
                    line.IsUnderground = false;
                }
                line.R = float.Parse(node.SelectSingleNode("R").InnerText);
                line.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
                line.LineType = node.SelectSingleNode("LineType").InnerText;
                line.ThermalConstantHeat = long.Parse(node.SelectSingleNode("ThermalConstantHeat").InnerText);
                line.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText);
                line.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText);

                lines.Add(line);
            }

        }
        private void Scale()
        {
            minX = allX.Min();
            maxX = allX.Max();
            minY = allY.Min();
            maxY = allY.Max();
            distanceX = (maxX - minX) * 100;
            distanceY = (maxY - minY) * 100;
           
        }
        private void Write()
        {
           
            foreach (var element in allElements)
            {
               
                ToLatLon(element.X, element.Y, 34, out newX, out newY);
                CalculateXY(newX, newY, out realX, out realY);

                Ellipse ellipse = new Ellipse();
                ellipse.Height = 2;
                ellipse.Width = 2;
                ellipse.Fill = element.Color;
                ellipse.ToolTip = element.ToolTip;

                ellipse.Name = "e" + element.Id.ToString();
                this.RegisterName(ellipse.Name, ellipse);
               

                Point tacka = pocetnaMreza.Find(t => t.X == realX && t.Y == realY);
                int brojac = 1;
                if (!pairs.ContainsKey(tacka))
                {
                    pairs.Add(tacka, element);
                }
                else
                {
                    bool flag = false;
                    while (true)
                    {
                        for (double findX = realX - brojac * 3; findX <= realX + brojac * 3; findX += 3)
                        {
                            if (findX < 0)
                                continue;
                            for (double findY = realY - brojac * 2; findY <= realY + brojac * 2; findY += 2)
                            {
                                if (findY < 0)
                                    continue;
                                tacka = pocetnaMreza.Find(t => t.X == findX && t.Y == findY);
                                if (!pairs.ContainsKey(tacka))
                                {
                                    pairs.Add(tacka, element);
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                        if (flag)
                            break;

                        brojac++;
                    }
                }

               
                Canvas.SetTop(ellipse, tacka.Y - 1); 
                Canvas.SetLeft(ellipse, tacka.X - 1);
                canvas.Children.Add(ellipse);
               
            }
        }
        private void StartAndEndOfLine(LineEntity line, out Point startPoint, out Point endPoint )
        {
            PowerEntity starter = allElements.Find(element => element.Id == line.FirstEnd);
            PowerEntity ender = allElements.Find(element => element.Id == line.SecondEnd);

            startPoint = new Point();
            endPoint = new Point();
            if(starter != null && ender != null)
            {
                startPoint = pairs.Where(element => element.Value == starter).First().Key;
                endPoint = pairs.Where(element => element.Value == ender).First().Key;
            }
        
        }
        private void WriteLines(Point startPoint, Point endPoint, LineEntity line, Canvas c)
        {
            GeometryGroup group = new GeometryGroup();
            LineGeometry horizontal = new LineGeometry();
            LineGeometry vertical = new LineGeometry();

            horizontal.StartPoint = startPoint;
            horizontal.EndPoint = new Point(endPoint.X, startPoint.Y);

            vertical.StartPoint = new Point(endPoint.X, startPoint.Y);
            vertical.EndPoint = endPoint;

            group.Children.Add(horizontal);
            group.Children.Add(vertical);

            Path realLine = new Path {
                Stroke = Brushes.Black,
                StrokeThickness = 0.5,
                Fill = Brushes.Black,
                Visibility = Visibility.Visible,
                Data = group,
                ToolTip = $"Line: ID-{line.Id} Name-{line.Name}"
                
            };

            realLine.MouseRightButtonDown+=Line_MouseRightButtonDown;
            c.Children.Add(realLine);

        }
        private void CalculateXY(double noviX, double noviY, out double praviX, out double praviY)
        {
            double odstojanjeX = 300 / distanceX;
            double odstojanjeY = 300 / distanceY;

            praviX = Math.Round((noviX - minX) * 100 * odstojanjeX) * 3;
            praviY = Math.Round((noviY - minY) * 100 * odstojanjeY) * 2;
            
        }
        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }

        //ovde sve animacije i akcije
        private void Line_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(remEllipse1 != null && remEllipse2 != null)
            {
                remEllipse1.Fill = remBrush1;
                remEllipse2.Fill = remBrush2;
            }

            string[] toolTip = ((Path)sender).ToolTip.ToString().Split(' ');
            string id = toolTip[1];
            LineEntity current = lines.Find(l => l.Id == long.Parse(id));

            //pamtim boje elipsi dok se ne klikne na drugu
            remBrush1 = ((Ellipse)canvas.FindName("e" + current.FirstEnd.ToString())).Fill;
            ((Ellipse)canvas.FindName("e" + current.FirstEnd.ToString())).Fill = Brushes.Red;
            remEllipse1 = ((Ellipse)canvas.FindName("e" + current.FirstEnd.ToString()));

           
            remBrush2 = ((Ellipse)canvas.FindName("e" + current.SecondEnd.ToString())).Fill;
            ((Ellipse)canvas.FindName("e" + current.SecondEnd.ToString())).Fill = Brushes.Red;
            remEllipse2 = ((Ellipse)canvas.FindName("e" + current.SecondEnd.ToString()));

           
            Storyboard.SetTargetName(startAnimation1, ((Ellipse)canvas.FindName("e" + current.FirstEnd.ToString())).Name);
            Storyboard.SetTargetName(startAnimation2, ((Ellipse)canvas.FindName("e" + current.FirstEnd.ToString())).Name);
            Storyboard.SetTargetName(endAnimation1, ((Ellipse)canvas.FindName("e" + current.SecondEnd.ToString())).Name);
            Storyboard.SetTargetName(endAnimation2, ((Ellipse)canvas.FindName("e" + current.SecondEnd.ToString())).Name);
            story.Begin((FrameworkElement)sender);
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
           
            foreach (var element in uIElements)
            {
                if (canvas.Children.Contains(element))
                {
                    RemoveElement(element);
                }

            }
            clearState = true;
           ClearBtn.IsEnabled = false;  
        }
        private void CreateElipse_Click(object sender, RoutedEventArgs e)
        {
          
            currentMode = "EllipseMode";

            
            Mouse.OverrideCursor = Cursors.Cross;

        }
        private void CreatePolygon_Click(object sender, RoutedEventArgs e)
        {
            currentMode = "PolygonMode";
            isSelectedPoints = true;
            polygonPoints = new List<Point>();
        }
        private void canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }
        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var ellipse = (Ellipse)sender;
            var ellipseWindow = new ChangeEllipseDialog();

            bool? result = ellipseWindow.ShowDialog();
            if(result.HasValue && result.Value)
            {
                ellipse.StrokeThickness = ellipseWindow.StrokeTh;
                ellipse.Stroke = ellipseWindow.StrokeColor;
                ellipse.Fill = ellipseWindow.Fill;
            }

        }
        private void Polygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           
            var polygon = (Polygon)sender;
            var PolygonWindow = new ChangeEllipseDialog();

            bool? result = PolygonWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                polygon.StrokeThickness = PolygonWindow.StrokeTh;
                polygon.Stroke = PolygonWindow.StrokeColor;
                polygon.Fill = PolygonWindow.Fill;
            }
        }
        private void Text_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var txtBlock = (TextBlock)sender;
            var TextWindow = new ChangeTextDialog();

            bool? result = TextWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                txtBlock.Foreground = TextWindow.TextColor;
                txtBlock.FontSize = TextWindow.textSize;
            }
        }
        private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ( e.RightButton == MouseButtonState.Pressed && isSelectedPoints == true)
            {
                Point clickedPoint = e.GetPosition((UIElement)sender);
                polygonPoints.Add(clickedPoint);
              

            }
            Point mousePosition = e.GetPosition(canvas);
            if (currentMode == "EllipseMode")
            {
                var ellipseWindow = new EllipseDialog();

                bool? result = ellipseWindow.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    double width = ellipseWindow.EllipseWidth;
                    double height = ellipseWindow.EllipseHeight;
                    double strokeTh = ellipseWindow.StrokeTh;

                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = width;
                    ellipse.Height = height;
                    ellipse.Stroke = ellipseWindow.EllipseStrokeColor;
                    ellipse.StrokeThickness = strokeTh;
                    ellipse.Fill = Brushes.Transparent;
                    ellipse.Name = $"Element{countOfElement}";
                    ellipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
                    ellipse.Opacity =ellipseWindow.opacity;
                    ellipse.Fill = ellipseWindow.Fill;

                    Canvas.SetLeft(ellipse, mousePosition.X - ellipse.Width / 2);
                    Canvas.SetTop(ellipse, mousePosition.Y - ellipse.Height / 2);

                    //tekst za elipsu
                    TextBlock textBlock = new TextBlock();

                    //racunanje width i height
                    double radiusX = width / 2;
                    double radiusY = height / 2;

                    double diagonal = Math.Sqrt(Math.Pow(radiusX,2) + Math.Pow(radiusY,2));
                    double widthTB = diagonal * radiusX / radiusY;
                    double heightTB =  diagonal * radiusY/ radiusX; 

                    textBlock.Width = widthTB;
                    textBlock.Height = heightTB;
                    textBlock.Text = ellipseWindow.Text;
                    textBlock.Foreground = ellipseWindow.TextColor;
                    textBlock.TextAlignment = TextAlignment.Center;
                    textBlock.Name = ellipse.Name + $"_text";

                    Canvas.SetLeft(textBlock, mousePosition.X - textBlock.Width / 2);
                    Canvas.SetTop(textBlock, mousePosition.Y - textBlock.Height / 2);

                    //canvas.Children.Add(ellipse);
                    AddElement(ellipse);
                    //canvas.Children.Add(textBlock);
                    AddElement(textBlock);
                    ClearBtn.IsEnabled = true;
                    uIElements.Add(ellipse);
                    uIElements.Add(textBlock);
                    currentMode = "";
                    Mouse.OverrideCursor = null;
                  


                }
                else
                {
                    currentMode = "";
                }
            }
            if(currentMode == "TextMode")
            {
                var TextWindow = new TextDialog();
                bool? result = TextWindow.ShowDialog();
                if(result.HasValue && result.Value)
                {
                   TextBlock textBlock = new TextBlock();
                    textBlock.FontSize = TextWindow.Size;
                    textBlock.Text = TextWindow.Text;
                    textBlock.Foreground = TextWindow.Color;
                    textBlock.TextAlignment = TextAlignment.Center;
                    
                    textBlock.MaxWidth = double.PositiveInfinity;
                    textBlock.Measure(new Size(double.PositiveInfinity,double.PositiveInfinity));
                    Size newSize = textBlock.DesiredSize;

                    textBlock.Height = newSize.Height;
                    textBlock.Width = newSize.Width;
                    textBlock.MouseLeftButtonDown += Text_MouseLeftButtonDown;
                   
                    Canvas.SetLeft(textBlock, mousePosition.X - textBlock.Width / 2);
                    Canvas.SetTop(textBlock, mousePosition.Y - textBlock.Height / 2);

                    //canvas.Children.Add(textBlock);
                    AddElement(textBlock);
                    uIElements.Add(textBlock);
                    ClearBtn.IsEnabled = true;
                    currentMode = "";
                    Mouse.OverrideCursor = null;

                }
                else
                {
                    currentMode = "";
                }


            }

        }
        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isSelectedPoints) { 
                 PolygonDialog attributesWindow = new PolygonDialog();
                bool? result = attributesWindow.ShowDialog();
                if(result.HasValue && result.Value)
                {

                    
                    Polygon polygon = new Polygon();
                    polygon.Points = new PointCollection(polygonPoints);
                    
                    polygon.StrokeThickness = attributesWindow.PolygonStrokeThickness;
                    polygon.Stroke = attributesWindow.PolygonStrokeColor;
                    polygon.Fill = attributesWindow.PolygonFill;
                    polygon.Opacity = attributesWindow.opacity;
                    polygon.Name = $"Element{countOfElement}";
                    polygon.MouseLeftButtonDown += Polygon_MouseLeftButtonDown;
                    TextBlock textBlock = new TextBlock();

                    double radiusX = polygon.ActualWidth / 2;
                    double radiusY = polygon.ActualHeight / 2;

                    double diagonal = Math.Sqrt(Math.Pow(radiusX, 2) + Math.Pow(radiusY, 2));
                    double widthTB = diagonal * radiusX / radiusY;
                    double heightTB = diagonal * radiusY / radiusX;

                    textBlock.Width = widthTB;
                    textBlock.Height = heightTB;
                    textBlock.Text = attributesWindow.Text;
                    textBlock.Foreground = attributesWindow.TextColor;
                    textBlock.TextAlignment = TextAlignment.Center;
                    textBlock.Name = polygon.Name + $"_text";


                    PointCollection points = polygon.Points;

                    //racunam x i y za tekst
                    double avgX = 0;
                    double avgY = 0;
                    foreach (Point point in points)
                    {
                        avgX += point.X;
                        avgY += point.Y;
                    }
                    avgX /= points.Count;
                    avgY /= points.Count;

                    
                    Point center = new Point(avgX, avgY);

                    Canvas.SetLeft(textBlock, center.X - polygon.ActualWidth / 2);
                    Canvas.SetTop(textBlock, center.Y - polygon.ActualHeight / 2);


                    //canvas.Children.Add(polygon);
                    //canvas.Children.Add(textBlock);
                    AddElement(polygon);
                    AddElement(textBlock);
                    ClearBtn.IsEnabled = true;
                    uIElements.Add(textBlock);
                    uIElements.Add(polygon);

                    isSelectedPoints = false;
                    polygonPoints.Clear();
                }
                else
                {
                    isSelectedPoints = false;
                    polygonPoints.Clear();
                }
               


            }
           
        }
        private void TextBtn_Click(object sender, RoutedEventArgs e)
        {

            currentMode = "TextMode";


        }
        private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (clearState)
            {
                foreach (var element in uIElements)
                {
                    canvas.Children.Add(element);

                }
                ClearBtn.IsEnabled = true;
                clearState = false;
            }
            else
            {
                commandStack.Undo();
            }
           
                

        }
        private void AddElement(UIElement element)
        {
            var command = new AddCommand(canvas, element);
            commandStack.Execute(command);
        }
        private void RemoveElement(UIElement element)
        {
            var command = new RemoveCommand(canvas, element);
            commandStack.Execute(command);
        }
        private void RedoBtn_Click(object sender, RoutedEventArgs e)
        {
           commandStack.Redo(); 
        }
    }
}
