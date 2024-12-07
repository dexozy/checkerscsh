namespace CheckersGame
{
    public enum PieceColor
    {
        None,
        Red,
        Black
    }

    public enum PieceType
    {
        None,
        Man,
        King
    }

    public class Piece
    {
        public PieceColor Color { get; set; }
        public PieceType Type { get; set; }

        public Piece(PieceColor color, PieceType type)
        {
            Color = color;
            Type = type;
        }
    }
}
