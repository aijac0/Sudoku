namespace Sudoku_Web.Services;
using Sudoku_Library;

public class BoardService
{
    private Board _board;
    private State _state;
    
    private const int SizeDefault = 9;
    private const float FillPercentDefault = 0.5f;
    
    public event Action BoardChanged;
    public event Action BoardReset;

    public BoardService()
    {
        ResetBoard(SizeDefault, FillPercentDefault);
    }
    
    public Board GetBoard()
    {
        return _board;
    }
    
    public void ResetBoard(int size, float fillPercent)
    {
        fillPercent = Math.Max(Math.Min(fillPercent, 1.0f), 0.0f);
        _board = Board.Create(size, fillPercent);
        OnBoardReset();
    }

    public void OnBoardReset()
    {
        BoardReset?.Invoke();
    }
    
    public void OnBoardChanged()
    {
        BoardChanged?.Invoke();
    }
    
}