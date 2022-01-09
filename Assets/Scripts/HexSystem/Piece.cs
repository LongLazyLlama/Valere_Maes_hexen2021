using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexSystem
{
    public class PieceEventArgs<TPosition> : EventArgs
        where TPosition : IPosition
    {
        public TPosition Position { get; }

        public PieceEventArgs(TPosition position)
        {
            Position = position;
        }
    }

    public class Piece<TPosition>
         where TPosition : IPosition
    {
        public event EventHandler<PieceEventArgs<TPosition>> Placed;
        public event EventHandler<PieceEventArgs<TPosition>> Taken;
        public event EventHandler<PieceEventArgs<TPosition>> Moved;

        //internal => its only used in this assembly.
        internal bool HasMoved { get; set; }
        public int PlayerID { get; set; }

        //When a piece does something, trigger the eventargs.
        internal void MoveTo(TPosition position)
        {
            OnMoved(new PieceEventArgs<TPosition>(position));
        }
        internal void TakeFrom(TPosition position)
        {
            OnTaken(new PieceEventArgs<TPosition>(position));
        }
        internal void PlaceAt(TPosition position)
        {
            OnPlaced(new PieceEventArgs<TPosition>(position));
        }

        protected virtual void OnPlaced(PieceEventArgs<TPosition> eventArgs)
        {
            var handler = Placed;
            handler?.Invoke(this, eventArgs);
        }
        protected virtual void OnMoved(PieceEventArgs<TPosition> eventArgs)
        {
            var handler = Moved;
            handler?.Invoke(this, eventArgs);
        }
        protected virtual void OnTaken(PieceEventArgs<TPosition> eventArgs)
        {
            var handler = Taken;
            handler?.Invoke(this, eventArgs);
        }
    }
}
