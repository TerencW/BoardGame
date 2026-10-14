namespace BoardGame.Model
{
    public class Cell
    {
        public int Row { get; }
        public int Column { get; }
        
        protected Cell(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
