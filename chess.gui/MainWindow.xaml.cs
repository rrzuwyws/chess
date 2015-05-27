using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using chess.logic;
using chess.logic.Pieces;


namespace chess.gui
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool transformation = false;
        const double GRADIENT_DEPTH = .4;
        UIElement ui = null;
        DependencyObject parent = null;
        Board boardInstance = Board.Instance;
        readonly Color WHITE;
        readonly Color BLACK;
        string STRING = "";
        void paintGrid(Color black, Color white, int size)
        {
            BGrid.Width = size;
            BGrid.Height = size;
            //BGrid.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            
            for (int i = 0; i < 64; ++i)
            {
                Grid g = new Grid();
                g.Width = size / 8;
                g.Height = size / 8;
                g.Background = new SolidColorBrush((i + i / 8) % 2 == 1 ? black : white);
                g.MouseUp += SquareClick;

                BitmapImage bi = new BitmapImage();
                int[] point = GridNumToXY(i, true);
                if (boardInstance[point[0], point[1]] != null)
                {
                    bi.BeginInit();
                    bi.UriSource = new Uri(@"resourses\" +
                        (boardInstance[point[0], point[1]].isWhite ? "white_" : "black_")
                        + boardInstance[point[0], point[1]].GetType().Name.ToString().ToLower()
                        + ".png" , UriKind.Relative);
                    bi.DecodePixelHeight = (int)(g.Height * 0.9);
                    bi.EndInit();

                    Image im = new Image();
                    im.Source = bi;
                    im.MouseUp += FigureClick;

                    g.Children.Add(im);
                }
                BGrid.Children.Add(g);
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            WHITE = Colors.DarkRed;
            BLACK = Colors.LightGoldenrodYellow;
            paintGrid(BLACK, WHITE, 320);
            boardInstance.CastleEvent += CastleDone;
            boardInstance.CheckMateEvent +=boardInstance_CheckMateEvent;
            boardInstance.PateEvent += boardInstance_PateEvent;
            boardInstance.TransformationEvent += boardInstance_TransformationEvent;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            //new pawnDialogue(false).ShowDialog();
            //MessageBox.Show("hehe " + boardInstance.nextPawn);
            /*
            Image e = new Image();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(@"http://upload.wikimedia.org/wikipedia/commons/8/81/Chess_bdt60.png");
            bi.DecodePixelHeight = 80;
            bi.EndInit();
            e.Source = bi;
            e.Height = e.Width = 80d;
            //e.Fill = new SolidColorBrush(Colors.Red);
            e.MouseUp += _Click;
            (BGrid.Children[0] as Grid).Children.Add(e);*/
        }

        void CurrentDomain_ProcessExit(object sender, EventArgs args)
        {
            //MessageBox.Show(System.IO.Directory.Exists("log").ToString());

            writeData();
        }
        void writeData()
        {
            if (!System.IO.Directory.Exists("log"))
            {
                System.IO.Directory.CreateDirectory("log");
            }
            var c = System.IO.Directory.EnumerateFiles("log", "*.txt");
            int max = 0;
            foreach (string s in c)
            {
                string d = s.Substring("log\\".Length, s.Length - 4);
                int temp = int.Parse(d.Substring(0, d.Length - 4));
                max = temp > max ? temp : max;
            }
            ++max;
            System.IO.StreamWriter sr = new System.IO.StreamWriter("log\\" + max + ".txt");
            sr.Write(STRING);
            sr.Close();
            MessageBox.Show("All data in file log\\" + max + ".txt");
        }
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show("MyHandler caught : " + e.Message);
            MessageBox.Show("Runtime terminating: " + args.IsTerminating);
            STRING += "\n\n     Exception " + e.Message + " " + args.IsTerminating;
            writeData();
        }

        void boardInstance_TransformationEvent(bool forWhite)
        {
            new pawnDialogue(forWhite).ShowDialog();
            transformation = true;
        }
        void Transformation()
        {
            int X = boardInstance.AllMoves[boardInstance.AllMoves.Count - 1].X_to;
            int Y = boardInstance.AllMoves[boardInstance.AllMoves.Count - 1].Y_to;
            int i = XYToGridNum(X, Y);
            (BGrid.Children[i] as Grid).Children.Clear();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(@"E:\study\10sem\chess\chess\chess.gui\resourses\" +
                (boardInstance[X, Y].isWhite ? "white_" : "black_")
                + boardInstance[X, Y].GetType().Name.ToString().ToLower()
                + ".png");
            bi.DecodePixelHeight = (int)(BGrid.Height / 8 * 0.9);
            bi.EndInit();

            Image im = new Image();
            im.Source = bi;
            im.MouseUp += FigureClick;
            (BGrid.Children[i] as Grid).Children.Add(im);
            transformation = false;
        }
        void boardInstance_PateEvent(bool forWhite)
        {
            MessageBox.Show("Draw. " + (forWhite?"White":"black")+ " king is pated");
        }

        private void boardInstance_CheckMateEvent(bool forWhite)
        {
            MessageBox.Show("ChM4" + (forWhite ? "W" : "B"));
        }
        int[] GridNumToXY(int i, bool forWhite = true)
        {
            if (forWhite)
                return new int[] { i % 8, (7 - i / 8) };
            else
                return new int[] { 7 - i % 8, i / 8 };
        }
        int XYToGridNum(int X, int Y, bool forWhite = true)
        {
            return (7 - Y) * 8 + X;
        }
        int XYToGridNum(int[] X, bool forWhite = true)
        {
            return XYToGridNum(X[0], X[1], forWhite);
        }
        private void SquareClick(object sender, RoutedEventArgs e)
        {
            //int i = BGrid.Children.IndexOf(sender as Grid);
            //int X = GridNumToXY(i)[0];
            //int Y = GridNumToXY(i)[1];
            //int orig = (7 - Y) * 8 + X;
            //MessageBox.Show("(" + X + "," + Y + "):" + orig + " ,when i = " + i);


            if (sender as Grid == parent as Grid) //sender != null  ;always
            {
                int[] xy = GridNumToXY(BGrid.Children.IndexOf(parent as Grid));
                showPossibleMoves(boardInstance.allMovesWithCheck(boardInstance[xy[0], xy[1]]));
                showSelf(XYToGridNum(xy));
                return;
            }
            //MessageBox.Show("Here");
            if (parent != null && ui != null)
            {
                int[] from = GridNumToXY(BGrid.Children.IndexOf(parent as Grid));
                int[] to = GridNumToXY(BGrid.Children.IndexOf(sender as Grid));
                IList<int> l = boardInstance.allMovesWithCheck(boardInstance[from[0], from[1]]);
                if (boardInstance[from[0], from[1]].TryMove(to[0], to[1], boardInstance))
                {
                    (parent as Grid).Children.Clear();
                    if (boardInstance.AllMoves[boardInstance.AllMoves.Count - 1].beat != null &&
                       boardInstance.AllMoves[boardInstance.AllMoves.Count - 1].beat.GetType() == typeof(Pawn) &&
                       boardInstance.AllMoves[boardInstance.AllMoves.Count - 1].beat.Y != to[1])
                        (BGrid.Children
                                [XYToGridNum(to[0], to[1] + (boardInstance[to[0], to[1]].isWhite ? -1 : 1))]
                                as Grid).Children.Clear();
                    else
                        (sender as Grid).Children.Clear();
                    if (transformation)
                        Transformation();
                    else
                        (sender as Grid).Children.Add(ui);
                    MoveLog.Content += getStringMove(boardInstance.LastMove);
                }
                ui = null;
                parent = null;
                restoreColors(l);
                clearSelf(XYToGridNum(from));
            }
            else
            {
                return;
            }
        }
        private void showPossibleMoves(IList<int> moves)
        {
            foreach (int i in moves)
            {
                RadialGradientBrush b = new RadialGradientBrush();
                b.GradientOrigin = new Point(.5, .5);
                b.Center = new Point(.5, .5);
                b.RadiusX = .5;
                b.RadiusY = .5;
                b.GradientStops.Add(
                        new GradientStop(((i / 100 + i % 100) % 2 == 0 ? BLACK : WHITE), .0));
                b.GradientStops.Add(
                        new GradientStop(((i / 100 + i % 100) % 2 == 0 ? BLACK : WHITE), GRADIENT_DEPTH));
                if ((BGrid.Children[XYToGridNum(i / 100, i % 100)] as Grid).Children.Count == 0)
                {
                    b.GradientStops.Add(
                        new GradientStop(Color.FromArgb(100, 0, 255, 0),1.0));
                }
                else
                {
                    b.GradientStops.Add(
                        new GradientStop(Color.FromArgb(100, 255, 0, 0), 1.0));
                }
                (BGrid.Children[XYToGridNum(i / 100, i % 100)] as Grid).Background = b;
            }
        }
        private void restoreColors(IList<int> moves)
        {
            foreach (int i in moves)
            {
                (BGrid.Children[XYToGridNum(i / 100, i % 100)] as Grid).Background = 
                    new SolidColorBrush((i / 100 + i % 100) % 2 == 0 ? BLACK : WHITE);
            }
        }
        private void FigureClick(object sender, RoutedEventArgs e)
        {
            if (ui == null && parent == null)
            {

                ui = sender as Image;
                parent = (ui as Image).Parent;
                int[] xy = GridNumToXY(BGrid.Children.IndexOf(parent as Grid));
                if (boardInstance[xy[0], xy[1]].isWhite != boardInstance.isWhiteMove)
                {
                    ui = null;
                    parent = null;
                }
            }

            //int[] xy = GridNumToXY(((parent as Grid).Parent as Grid).Children.IndexOf(parent as Grid));
        }
        private void CastleDone(bool white, bool toRight)
        {
            int x = toRight ? 7 : 0;
            int kingX = 4;
            int y = white ? 0 : 7;
            int xincr = toRight ? -1 : 1;
            UIElement temp = (BGrid.Children[XYToGridNum(x, y)] as Grid).Children[0];
            (BGrid.Children[XYToGridNum(x, y)] as Grid).Children.Clear();
            (BGrid.Children[XYToGridNum(kingX - xincr, y)] as Grid)
                .Children.Add(temp);   //rook
            temp = (BGrid.Children[XYToGridNum(kingX, y)] as Grid).Children[0];
            (BGrid.Children[XYToGridNum(kingX, y)] as Grid).Children.Clear();
            (BGrid.Children[XYToGridNum(kingX - xincr * 2, y)] as Grid)
                .Children.Add(temp);  //king
            ui = null;
            parent = null;
        }
        private void showSelf(int gridNum)
        {
            RadialGradientBrush b = new RadialGradientBrush();
            b.GradientOrigin = new Point(.5, .5);
            b.Center = new Point(.5, .5);
            b.RadiusX = .5;
            b.RadiusY = .5;
            b.GradientStops.Add(
                    new GradientStop(((gridNum + gridNum / 8) % 2 == 0 ? BLACK : WHITE), .0));
            b.GradientStops.Add(
                    new GradientStop(((gridNum + gridNum / 8) % 2 == 0 ? BLACK : WHITE), GRADIENT_DEPTH));
            b.GradientStops.Add(
                    new GradientStop(Color.FromArgb(100, 100, 255, 255), 1.0));
            (BGrid.Children[gridNum] as Grid).Background = b;
        }
        private void clearSelf(int gridNum)
        {
            (BGrid.Children[gridNum] as Grid).Background =
                    new SolidColorBrush((gridNum + gridNum / 8) % 2 == 1 ? BLACK : WHITE);
        }

        private string getStringMove(Board.MoveStruct move)
        {
            string s = "";
            if (boardInstance[move.X_to, move.Y_to].isWhite)
                s += "\n" + (move.counter / 2 + 1) + ": ";
            if (boardInstance[move.X_to, move.Y_to].GetType() == typeof(King))
            {
                if (move.X_to - move.X_from > 1)
                {
                    s += "0-0 ";
                    return s;
                }
                else if (move.X_to - move.Y_from < -1)
                {
                    s += "0-0-0 ";
                    return s;
                }
            }
            s += boardInstance[move.X_to, move.Y_to].ToString() + "_" +
                numtoletter(move.X_from.ToString()) + (move.Y_from + 1).ToString() +
                numtoletter(move.X_to.ToString()) + (move.Y_to + 1).ToString();
            if (move.beat != null)
                s += "+";
            s += "   ";
            STRING += s;
            return s;
        }
        private string numtoletter(string X)
        {
            switch (X)
            {
                case "0": return "a";
                case "1": return "b";
                case "2": return "c";
                case "3": return "d";
                case "4": return "e";
                case "5": return "f";
                case "6": return "g";
                case "7": return "h";
                default: return "";
            }
        }
    }
}
