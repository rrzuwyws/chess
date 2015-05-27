using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess.logic.Pieces
{
    public class Rook: Piece
    {
        public Rook(int X, int Y, bool isWhite) : base(X, Y, isWhite) { }
        public override bool canMove(int X, int Y, Board board)
        {
            return this.X == X || this.Y == Y;
        }
        public override string ToString()
        {
            return "R";
        }
    }
}
