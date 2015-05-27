using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess.logic.Pieces
{
    public class Knight : Piece
    {
        public Knight(int X, int Y, bool isWhite) : base(X, Y, isWhite) { }
        public override bool canMove(int X, int Y, Board board)
        {
            return Math.Abs(this.X - X) + Math.Abs(this.Y - Y) == 3 &&
                   Math.Abs(Math.Abs(this.X - X) - Math.Abs(this.Y - Y)) == 1;
        }
        public override string ToString()
        {
            return "N";
        }
    }
}
