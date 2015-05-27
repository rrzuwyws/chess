using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace chess.logic
{
    using Pieces;

    public class Board
    {
        private bool searchingMovies = false;
        private King BlackKing;
        private King WhiteKing;

        public PieceType nextPawn = PieceType.Queen;

        public delegate void CasleDone(bool white, bool toRight);
        public event CasleDone CastleEvent = (i, j) => { };

        public delegate void Check(bool forWhite);
        public event Check CheckEvent = i => { };
        
        public delegate void CheckMate(bool forWhite);
        public event CheckMate CheckMateEvent = i => { };

        public delegate void Pate(bool forWhite);
        public event Pate PateEvent = i => { };

        public delegate void SelectTransformation(bool forWhite);
        public event SelectTransformation TransformationEvent = i => { };

        private bool checkingMate = false;
        public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King };
        public struct MoveStruct
        {
            public int X_from;
            public int X_to;
            public int Y_from;
            public int Y_to;
            public int counter;
            public Piece beat;
            public PieceType moved;
            public bool firstMove;
        }
        private List<MoveStruct> _moves = new List<MoveStruct>();
        public MoveStruct LastMove
        {
            get
            {
                return _moves[_moves.Count - 1];
            }
        }
        public int MoveCounter { get; private set; }
        public const int HEIGHT = 7;
        public const int WIDTH = 7;
        /** abcdefgh
         * 8        8 7
         * 7        7 6
         * 6        6 5
         * 5        5 4
         * 4        4 3
         * 3        3 2
         * 2        2 1
         * 1        1 0
         *  abcdefgh
         */
        private Piece[,] plist = new Piece[8, 8];  //[num,letter]
        private List<Piece> captured = new List<Piece>();
        private static Board _chessBoard = null;
        public bool isWhiteMove { get; protected set; } 
        public static Board getInstance()
        {
            if (_chessBoard == null)
                _chessBoard = new Board();
            return _chessBoard;
        }
        public static Board Instance
        {
            get
            {
                return Board.getInstance();
            }
        }
        private void chessInit()
        {
            //pawns
            for (int i = 0; i < 16; ++i)
                plist[i % 8, (i / 8) * 5 + 1] = new Pawn(i % 8, (i / 8) * 5 + 1, i / 8 == 0);
            //rooks
            plist[0, 0] = new Rook(0, 0, true);
            plist[7, 0] = new Rook(7, 0, true);
            plist[0, 7] = new Rook(0, 7, false);
            plist[7, 7] = new Rook(7, 7, false);
            //knigtsh
            plist[1, 0] = new Knight(1, 0, true);
            plist[6, 0] = new Knight(6, 0, true);
            plist[1, 7] = new Knight(1, 7, false);
            plist[6, 7] = new Knight(6, 7, false);
            //bishpso
            plist[2, 0] = new Bishop(2, 0, true);
            plist[5, 0] = new Bishop(5, 0, true);
            plist[2, 7] = new Bishop(2, 7, false);
            plist[5, 7] = new Bishop(5, 7, false);
            //king  s
            plist[4, 0] = WhiteKing = new King(4, 0, true);
            plist[4, 7] = BlackKing = new King(4, 7, false);
            //quees n          
            plist[3, 0] = new Queen(3, 0, true);
            plist[3, 7] = new Queen(3, 7, false);
        }
        private Board() {
            //init all
            isWhiteMove = true;
            MoveCounter = 0;
            chessInit();
        }
        public bool Move(Piece F, int X, int Y)
        {
            
            if (F.isWhite ^ isWhiteMove)
                return false;
            if (F.GetType() == typeof(Knight))             //runs through obstacles
                return MoveOrBite(F, X, Y);
            if (F.GetType() == typeof(Pawn))    
            {
                if (X != F.X)
                {
                    if (plist[X, Y + (isWhiteMove ? -1 : 1)] != null &&
                        plist[X, Y + (isWhiteMove ? -1 : 1)].GetType() == typeof(Pawn) &&
                        plist[X, Y + (isWhiteMove ? -1 : 1)].isWhite ^ isWhiteMove &&
                        (plist[X, Y + (isWhiteMove ? -1 : 1)] as Pawn).jump &&
                        plist[X, Y + (isWhiteMove ? -1 : 1)].lastMove == this.MoveCounter &&
                        _chessBoard[X, Y] == null)                          //En passant
                    {
                        plist[X, Y] = plist[X, Y + (isWhiteMove ? -1 : 1)];
                        plist[X, Y + (isWhiteMove ? -1 : 1)] = null;
                        return MoveOrBite(F, X, Y);
                    }
                    else if (_chessBoard[X, Y] != null)
                        return MoveOrBite(F, X, Y);
                    return false;
                }
                if (((plist[X, (Y + F.Y) / 2] == null && Math.Abs(Y - F.Y) == 2) &&
                     plist[X, Y] == null) || (plist[X, Y] == null && Math.Abs(Y - F.Y) == 1))
                {
                    return MoveOrBite(F, X, Y);
                }
                return false;
            }
            if (F.GetType() == typeof(King) && Math.Abs(X - F.X) > 1) //castle
            {
                if (X - F.X > 0)
                {
                    if (F.lastMove == Piece.NOT_MOVED &&
                        plist[7, F.Y]!=null &&
                        plist[7, F.Y].lastMove == Piece.NOT_MOVED &&
                        !canBeCaptured(6, F.Y, !F.isWhite) &&
                        !canBeCaptured(5, F.Y, !F.isWhite) &&
                        !canBeCaptured(4, F.Y, !F.isWhite) &&
                        noObstacles(F.X, 6, F.Y, F.Y))
                    {
                        plist[7, F.Y].TryMove(5, F.Y, this);
                        MoveStruct mv = new MoveStruct();
                        plist[X, Y] = F;
                        plist[F.X, F.Y] = null;
                        mv.X_from = F.X;
                        mv.Y_from = F.Y;
                        mv.X_to = X;
                        mv.Y_to = Y;
                        mv.firstMove = true;
                        mv.counter = this.MoveCounter;
                        mv.moved = PieceType.King;
                        _moves.Add(mv);
                        CastleEvent(F.isWhite, true);
                        
                    }
                }
                else
                {
                    if (F.lastMove == Piece.NOT_MOVED && 
                        plist[0, F.Y]!= null &&
                        plist[0, F.Y].lastMove == Piece.NOT_MOVED &&
                        !canBeCaptured(1, F.Y, !F.isWhite) &&
                        !canBeCaptured(2, F.Y, !F.isWhite) &&
                        !canBeCaptured(3, F.Y, !F.isWhite) &&
                        !canBeCaptured(4, F.Y, !F.isWhite) &&
                        noObstacles(F.X, 1, F.Y, F.Y))
                    {
                        plist[0, F.Y].TryMove(3, F.Y, this);
                        MoveStruct mv = new MoveStruct();
                        plist[X, Y] = F;
                        plist[F.X, F.Y] = null;
                        mv.X_from = F.X;
                        mv.Y_from = F.Y;
                        mv.X_to = X;
                        mv.Y_to = Y;
                        mv.firstMove = true;
                        mv.counter = this.MoveCounter;
                        mv.moved = PieceType.King;
                        _moves.Add(mv);
                        CastleEvent(F.isWhite, false);
                    }
                }
                return false;
                //throw new NotImplementedException();
            }
            if (!noObstacles(F.X, X, F.Y, Y)) return false;
            return MoveOrBite(F, X, Y);
            throw new NotImplementedException();
        }
        private bool noObstacles(int Xfrom, int Xto, int Yfrom, int Yto)
        {
            int Xincrement = Math.Sign(Xto - Xfrom);
            int Yincrement = Math.Sign(Yto - Yfrom);
            if (plist[Xto, Yto] != null && plist[Xto, Yto].isWhite == plist[Xfrom, Yfrom].isWhite)
                return false;
            //if (Yto != Yfrom) Yto -= Yincrement;
            //if (Xto != Xfrom) Xto -= Xincrement;
            for (int i = Xfrom + Xincrement, j = Yfrom + Yincrement;
                i != Xto || j != Yto;
                i += Xincrement, j += Yincrement)
            {
                if (plist[i, j] != null)
                    return false;
            }
            return true;
        }
        private bool MoveOrBite(Piece F, int X, int Y) //true if ok
        {
            MoveStruct mv = new MoveStruct();
            if (plist[X, Y] != null)
            {
                captured.Add(plist[X, Y]);
                mv.beat = plist[X, Y];
                plist[X, Y] = null;
            }
            plist[X, Y] = F;
            plist[F.X, F.Y] = null;
            ++MoveCounter;
            mv.X_from = F.X;
            mv.Y_from = F.Y;
            mv.X_to = X;
            mv.Y_to = Y;
            mv.firstMove = F.lastMove == Piece.NOT_MOVED;
            plist[X, Y].X = X;
            plist[X, Y].Y = Y;
            plist[X, Y].lastMove = MoveCounter;
            mv.counter = this.MoveCounter;
            mv.moved = (PieceType)Enum.Parse(typeof(PieceType), F.GetType().Name.ToString());
            _moves.Add(mv);
            isWhiteMove ^= true;
            int[] KingXY = getKing(!isWhiteMove);
            if (canBeCaptured(KingXY[0], KingXY[1], isWhiteMove))
            {
                canselMove();
                return false;
            }

            if (F.GetType() == typeof(Pawn) && (Y == 0 || Y == 7)
                && !checkingMate && !searchingMovies)
            {
                TransformationEvent(F.isWhite);
                switch (nextPawn)
                {
                    case PieceType.Bishop:
                        plist[X, Y] = new Bishop(X, Y, F.isWhite);
                        break;
                    case PieceType.Knight:
                        plist[X, Y] = new Knight(X, Y, F.isWhite);
                        break;
                    case PieceType.Rook:
                        plist[X, Y] = new Rook(X, Y, F.isWhite);
                        break;
                    case PieceType.Queen:
                        plist[X, Y] = new Queen(X, Y, F.isWhite);
                        break;
                    default:
                        plist[F.X, F.Y] = new Queen(F.X, F.Y, F.isWhite);
                        break;
                }
            }

            if (!checkingMate)
            {
                checkingMate = true;
                KingXY = getKing(isWhiteMove);
                //int x_old = F.X;
                //int y_old = F.Y;
                //F.Move(X, Y);
                if(checkCheckMate(isWhiteMove))
                    if (canBeCaptured(KingXY[0], KingXY[1], !isWhiteMove))
                    {
                        CheckMateEvent(isWhiteMove);
                    }
                    else
                    {
                        PateEvent(isWhiteMove);
                    }
                //F.Move(x_old, y_old);
                checkingMate = false;
            }
            return true;
        }
        private int[] getKing(bool white)
        {
            /*foreach(Piece p in plist)
            {
                if (p != null && p.GetType() == typeof(King) && p.isWhite == white)
                    return p as King;
            }*/
            //for (int i = 0; i <= WIDTH; ++i) 
            //    for (int j = 0; j <= HEIGHT; ++j) 
            //        if (plist[i, j] != null && plist[i, j].GetType() == typeof(King) && 
            //            plist[i, j].isWhite == white) 
            //            return new int[] { i, j };
            //return null;
            if (white)
                return new[] { WhiteKing.X, WhiteKing.Y };
            else
                return new[] { BlackKing.X, BlackKing.Y };
        }
        public Piece this[int X, int Y]
        {
            get
            {
                return plist[X, Y];
            }
        }
        public Piece[,] All
        {
            get
            {
                return plist;
            }
        }
        public IList<Piece> Captured
        {
            get
            {
                return captured.AsReadOnly();
            }
        }
        public IList<MoveStruct> AllMoves
        {
            get
            {
                return _moves.AsReadOnly();
            }
        }
        private bool canselMove() //temporary public
        {
            MoveStruct last = _moves[_moves.Count - 1];
            _moves.Remove(last);
            plist[last.X_to, last.Y_to].X = last.X_from;
            plist[last.X_to, last.Y_to].Y = last.Y_from;
            plist[last.X_from, last.Y_from] = plist[last.X_to, last.Y_to].lastMove == Piece.NOT_MOVED ? 
                new Pawn(last.X_from, last.Y_from, plist[last.X_to, last.Y_to].isWhite) :
                plist[last.X_to, last.Y_to];
            plist[last.X_to, last.Y_to] = last.beat;
            --MoveCounter;
            plist[last.X_from, last.Y_from].lastMove = last.firstMove ? Piece.NOT_MOVED : MoveCounter;
            isWhiteMove ^= true;
            return true;
        }
        private bool canMove(Piece F, int X, int Y)
        {
            if (F.GetType() == typeof(King))
            {
                return Math.Abs(F.X - X) <= 1 && Math.Abs(F.Y - Y) <= 1;
            }
            if (F.canMove(X, Y, this))// && Move(F, X, Y))
            {
                //canselMove();
                return true;
            }
            return false;
        }
        private bool canBeCaptured(int X, int Y, bool byWhite)
        {

            foreach (Piece p in plist)
            {

                if (p != null && byWhite == p.isWhite && canMove(p, X, Y))
                {
                    if(p.GetType() == typeof(Knight))
                        return true;
                    if(p.GetType() == typeof(Pawn) && p.X == X)
                        continue;
                    if(noObstacles(p.X, X, p.Y, Y))
                        return true;
                }
            }
            return false;
        }
        private Piece getFirstOnVector(int X, int Y, int Xiterator, int Yiterator)
        {
            for (int i = X + Xiterator, j = Y + Yiterator;
                i <= WIDTH && i >= 0 && j <= HEIGHT && j >= 0;
                i += Xiterator, j += Yiterator)
                if (plist[i, j] != null)
                    return plist[i, j];

            Piece p = null;
            return p;
        }

        /// <summary>
        /// returns all moves in format X*100+Y
        /// </summary>
        /// <param name="P">for piece</param>
        /// <returns>List of moves in format X*100+Y</returns>
        public List<int> allMoves(Piece P)
        {
            searchingMovies = true;
            List<int> l = new List<int>();
            if (P.GetType() == typeof(Pawn))
            {
                if (plist[P.X, P.Y + (P.isWhite ? 1 : -1)] == null)
                {
                    l.Add(P.X * 100 + P.Y + (P.isWhite ? 1 : -1));
                    if (P.lastMove == Piece.NOT_MOVED)
                    {
                        if (plist[P.X, P.Y + (P.isWhite ? 2 : -2)] == null)
                            l.Add(P.X * 100 + P.Y + (P.isWhite ? 2 : -2));
                    }
                }
                if (P.X > 0 && ((plist[P.X - 1, P.Y + (P.isWhite ? 1 : -1)] != null
                    && plist[P.X - 1, P.Y + (P.isWhite ? 1 : -1)].isWhite ^ P.isWhite)
                    || (plist[P.X - 1, P.Y] != null
                    &&  plist[P.X - 1, P.Y].isWhite ^ P.isWhite
                    &&  plist[P.X - 1, P.Y].GetType() == typeof(Pawn)
                    &&  plist[P.X - 1, P.Y].lastMove == this.MoveCounter)))
                    l.Add((P.X - 1) * 100 + P.Y + (P.isWhite ? 1 : -1));
                if (P.X < 7 && ((plist[P.X + 1, P.Y + (P.isWhite ? 1 : -1)] != null
                    && plist[P.X + 1, P.Y + (P.isWhite ? 1 : -1)].isWhite ^ P.isWhite)
                    || (plist[P.X + 1, P.Y] != null
                    &&  plist[P.X + 1, P.Y].isWhite ^ P.isWhite
                    &&  plist[P.X + 1, P.Y].GetType() == typeof(Pawn)
                    &&  plist[P.X + 1, P.Y].lastMove == this.MoveCounter)))
                    l.Add((P.X + 1) * 100 + P.Y + (P.isWhite ? 1 : -1));
                searchingMovies = false;
                return l;
            }
            if (P.GetType() == typeof(Knight))
            {
                for (int i = (P.X > 2 ? P.X - 2 : 0); i <= (P.X <= WIDTH - 2 ? P.X + 2 : WIDTH); ++i)
                {
                    for (int j = (P.Y > 2 ? P.Y - 2 : 0); j <= (P.Y <= HEIGHT - 2 ? P.Y + 2 : HEIGHT); ++j)
                    {
                        if (P.canMove(i, j, this) && (plist[i,j] == null || plist[i,j].isWhite ^ P.isWhite))
                            l.Add(i * 100 + j);
                    }
                }
                searchingMovies = false;
                return l;
            }

            if (P.GetType() == typeof(King))
            {
                for (int i = P.X > 0 ? P.X - 1 : 0; i <= (P.X <= WIDTH - 1 ? P.X + 1 : WIDTH - 1); ++i)
                {
                    for (int j = P.Y > 0 ? P.Y - 1 : 0; j <= (P.Y <= HEIGHT - 1 ? P.Y + 1 : HEIGHT - 1); ++j)
                    {
                        if (P.canMove(i, j, this) && (plist[i, j] == null || plist[i, j].isWhite ^ P.isWhite))
                        { l.Add(i * 100 + j); }
                    }
                }
                if (P.lastMove == Piece.NOT_MOVED)
                {
                    if (plist[7, P.Y] != null &&
                        plist[7, P.Y].lastMove == Piece.NOT_MOVED &&
                        !canBeCaptured(6, P.Y, !P.isWhite) &&
                        !canBeCaptured(5, P.Y, !P.isWhite) &&
                        !canBeCaptured(4, P.Y, !P.isWhite) &&
                        noObstacles(P.X, 6, P.Y, P.Y))
                        l.Add(600 + P.Y); //castle to right
                    if (plist[0, P.Y]!=null &&
                        plist[0, P.Y].lastMove == Piece.NOT_MOVED &&
                        !canBeCaptured(1, P.Y, !P.isWhite) &&
                        !canBeCaptured(2, P.Y, !P.isWhite) &&
                        !canBeCaptured(3, P.Y, !P.isWhite) &&
                        !canBeCaptured(4, P.Y, !P.isWhite) &&
                        noObstacles(P.X, 2, P.Y, P.Y))
                        l.Add(200 + P.Y); //castle to left
                }
                searchingMovies = false;
                return l;
            }

            for (int i = 0; i <= WIDTH; ++i)
            {
                for (int j = 0; j <= HEIGHT; ++j)
                {
                    if (P.canMove(i, j, this))
                    {
                        if (noObstacles(P.X, i, P.Y, j) &&
                            (plist[i, j] == null || plist[i, j].isWhite ^ P.isWhite))
                            l.Add(i * 100 + j);
                    }
                }
            }
            searchingMovies = false;
            return l;
        }
        private bool checkCheckMate(bool forWhiteKing)
        {
            int[] KingXY = getKing(forWhiteKing);
            foreach (Piece p in plist)
            {
                if(p!=null && p.isWhite == forWhiteKing)
                {
                    for (int i = 0; i <= WIDTH; ++i)
                        for (int j = 0; j <= HEIGHT; ++j)
                            if (p.TryMove(i, j, this))
                            {
                                //p.Move(_moves[_moves.Count - 1].X_from, _moves[_moves.Count - 1].Y_from);
                                canselMove();
                                return false;
                            }
                }
            }
            return true;
        }
        public IList<int> allMovesWithCheck(Piece P, List<int> moves = null)
        {
            if (moves == null)
                moves = allMoves(P);
            searchingMovies = true;
            int[] temp = new int[moves.Count];
            moves.CopyTo(temp);
            if (P.GetType() == typeof(King))
            {
                foreach (int i in temp)
                {
                    if (Math.Abs(i / 100 - P.X) <= 1 && Math.Abs(i % 100 - P.Y) <= 1)
                    {
                        if (!P.TryMove(i / 100, i % 100, this))
                        {
                            moves.Remove(i);
                        }
                        else
                        {
                            MoveStruct last = _moves[_moves.Count - 1];
                            P.Move(last.X_from, last.Y_from);
                            canselMove();
                        }
                    }
                }
            }
            else
                foreach (int i in temp)
                {
                    if (!P.TryMove(i / 100, i % 100, this))
                    {
                        moves.Remove(i);
                    }
                    else
                    {
                        MoveStruct last = _moves[_moves.Count - 1];
                        P.Move(last.X_from, last.Y_from);
                        canselMove();
                    }
                }
            searchingMovies = false;
            return moves.AsReadOnly();
        }

    }
}
