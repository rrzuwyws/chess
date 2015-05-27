using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess.logic.Pieces
{
    public abstract class Piece
    {
        internal const int NOT_MOVED = -1;
        /// <summary>
        /// num game move, last moved on;
        /// -1 == not moved (NOT_MOVED)
        /// </summary>
        public int lastMove { get; internal protected set; }
        public virtual int X { get; internal protected set; } //{ return _x; } protected set { _x = value; } } //0..7
        public virtual int Y { get; internal protected set; } //0..7
        public bool isWhite { get; protected set; } //true for white
        //protected int _x, _y;
        public bool TryMove(int X, int Y, Board board)
        {
            if (!canMove(X, Y, board) || 
                X < 0 || X > Board.HEIGHT ||
                Y < 0 || Y > Board.WIDTH)
                return false;
            return board.Move(this, X, Y);
            
                //lastMove = board.MoveCounter;
                //this.X = X;
                //this.Y = Y;
        }
        public Piece(int X, int Y, bool isWhite)
        {
            this.X = X;
            this.Y = Y;
            this.isWhite = isWhite;
            this.lastMove = NOT_MOVED;
        }

        internal void Move(int newX, int newY)
        {
            X = newX;
            Y = newY;
        }

        public abstract override string ToString();
        abstract public bool canMove(int X, int Y, Board board);
    }
}
