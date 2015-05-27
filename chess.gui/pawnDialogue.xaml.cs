using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


using chess.logic;

namespace chess.gui
{
    /// <summary>
    /// Логика взаимодействия для pawnDialogue.xaml
    /// </summary>
    public partial class pawnDialogue : Window
    {
        bool buttonPressed = false;
        public pawnDialogue(bool White)
        {
            InitializeComponent();
            string[] pieces = { "bishop", "knight", "rook", "queen" };
            int i = 0;
            foreach (string s in pieces)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(@"resourses\"
                    + (White ? "white_" : "black_")
                    + s +".png", UriKind.Relative);
                bi.DecodePixelHeight = 50;
                bi.EndInit();

                Image im = new Image();
                im.Source = bi;
                Grid.SetColumn(im, i);
                Grid.SetRow(im, 1);
                myGrid.Children.Add(im);
                ++i;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MessageBox.Show(sender.ToString());
            if (!buttonPressed)
                e.Cancel = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Board.Instance.nextPawn = 
                (Board.PieceType)Enum.Parse(typeof(Board.PieceType), (sender as Button).Name);
            buttonPressed = true;
            this.Close();
        }
    }
}
