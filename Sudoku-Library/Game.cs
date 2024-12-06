using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;

namespace Sudoku_Library;

public class Game : IEnumerable<(byte, bool, bool)> 
{
    private Board board;
    private bool[,] mask;
    public int Size => board.Size;

    public Game(int size, float fillPercent)
    {
        // Create solved board
        board = new Board(size);
        board = board.Solve()!;
        
        // Clear cells from board
        var rand = new Random();
        int numClear = (int) Math.Round(board.Size * board.Size * (1 - fillPercent));
        while (numClear > 0)
        {
            int row = rand.Next(0, board.Size);
            int col = rand.Next(0, board.Size);
            if (board.Get(row, col).Value != 0)
            {
                board.Set(row, col, 0);
                numClear--;
            } 
        }
        
        // Create mask
        mask = new bool[board.Size, board.Size];
        for (int i = 0; i < board.Size; i++)
        {
            for (int j = 0; j < board.Size; j++)
            {
                if (board.Get(i, j).Value == 0)
                    mask[i, j] = true;
            }
        }
    }

    public void Set(int row, int col, byte value)
    {
        if (!IsFixed(row, col))
            board.Set(row, col, value);
    }

    public Cell Get(int row, int col)
    {
        return board.Get(row, col);
    }

    public bool IsFixed(int row, int col)
    {
        if (row < 0 || row >= board.Size || col < 0 || col >= board.Size)
            return true;
        return mask[row, col];
    }

    public void Display()
    {
        for (int row = 0; row < board.Size; row++)
        {
            Console.Write("| ");
            for (int col = 0; col < board.Size; col++)
            {
                if (col > 0) Console.Write("  ");
                byte value = board.Get(row, col).Value;
                Console.Write(value == 0 ? "." : value.ToString());
            }
            Console.WriteLine(" |");
        }
    }

    public IEnumerator<(byte, bool, bool)> GetEnumerator()
    {
        for (int row = 0; row < board.Size; row++)
        {
            for (int col = 0; col < board.Size; col++)
            {
                Cell cell = board.Get(row, col);
                yield return (cell.Value, cell.IsValid, mask[row, col]);

            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return board.GetEnumerator();
    }
}