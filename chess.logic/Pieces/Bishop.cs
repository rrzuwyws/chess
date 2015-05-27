using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess.logic.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(int X, int Y, bool isWhite) : base(X, Y, isWhite) { }
        public override bool canMove(int X, int Y, Board board)
        {
            return Math.Abs(this.X - X) == Math.Abs(this.Y - Y);
        }
        public override string ToString()
        {
            return "B";
        }
    }
}
