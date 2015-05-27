using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess.logic.Pieces
{
    public class King : Piece
    {
        public King(int X, int Y, bool isWhite) : base(X, Y, isWhite) { }
        public override bool canMove(int X, int Y, Board board)
        {
            bool castle = this.lastMove == Piece.NOT_MOVED && Y == this.Y && Math.Abs(this.X - X) == 2;
            return Math.Abs(this.X - X) <= 1 && Math.Abs(this.Y - Y) <= 1 || castle;
        }
        public override string ToString()
        {
            return "K";
        }
    }
}
