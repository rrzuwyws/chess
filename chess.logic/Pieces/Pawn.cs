using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess.logic.Pieces
{
    public class Pawn : Piece
    {
        public bool jump { get; private set; }
        //bool firstMove = true;
        public Pawn(int X, int Y, bool isWhite) : base(X, Y, isWhite) { jump = false; }
        public override bool canMove(int X, int Y, Board board)
        {
            return ((Y - this.Y) == (this.isWhite ? 1 : -1) && Math.Abs(this.X - X) <= 1)
                   || ((Y - this.Y) == (this.isWhite ? 2 : -2) && this.X == X && this.lastMove == -1);
        }
        public override int Y
        {
            get
            {
                return base.Y;
            }
            internal protected set
            {
                jump = Math.Abs(this.Y - value) == 2;
                base.Y = value;
            }
        }
        public override string ToString()
        {
            return "P";
        }
    }
}
