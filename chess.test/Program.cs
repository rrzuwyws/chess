using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using chess.logic;
using chess.logic.Pieces;

namespace chess.test
{
    class Program
    {
        static void Main(string[] args)
        {
            printAll();
            Board b = Board.Instance;
            //b.Move(b[1, 0], 2, 2);      //doesn`t work
            b[1, 0].TryMove(2, 2, b);     //works
            b[3, 6].TryMove(3, 4, b);
            b[2, 2].TryMove(3, 4, b);
            b[3, 7].TryMove(3, 4, b); 
            Console.WriteLine();/*todo moveCounter*/
            printAll();
            Console.WriteLine();
            //b.canselMove();
            //b.canselMove();
            //b.canselMove();
            printAll();
            Console.WriteLine();
            printAll();
            Console.WriteLine();
            Console.ReadKey();
            
        }
        static void printAll()
        {
            Board b = Board.Instance;
            int i = 0;
            foreach (var v in b.All)
            {
                if (i++ % (Board.WIDTH + 1) == 0)
                    Console.WriteLine();
                if (v != null)
                    Console.Write((v.GetType().Name.ToString().Substring(0, 2)
                        + "(" + v.X + "," + v.Y + ")"
                        + (v.isWhite ? "W" : "B")).PadLeft(9));
                else
                    Console.Write(" ".PadRight(9, '_'));
            }
        }
    }
}
