using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess.logic.Pieces
{
    public class Queen : Piece
    {
        public Queen(int X, int Y, bool isWhite) : base(X, Y, isWhite) { }
        public override bool canMove(int X, int Y, Board board)
        {
            bool likeRook = this.X == X || this.Y == Y;
            bool likeBishop = Math.Abs(this.X - X) == Math.Abs(this.Y - Y);
            return likeBishop || likeRook;   //it`s easier to understand
        }
        public override string ToString()
        {
            return "Q";
        }
    }
}
