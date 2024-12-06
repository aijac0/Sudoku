using System.Collections;

namespace Sudoku_Library;

public enum State
{
	Solved,
	Unsolved,
	Invalid,
}

public class Cell
{
	public byte Value { get; internal set; }
	internal int Collisions { get; set; }
	public bool IsValid => Value != 0 && Collisions == 0;
	public bool IsFixed { get; internal set; }

	public Cell()
	{
		Value = 0;
		Collisions = 0;
		IsFixed = false;
	}
	
	public Cell(Cell oth)
	{
		Value = oth.Value;
		Collisions = oth.Collisions;
		IsFixed = oth.IsFixed;
	}
}


public class Board : IEnumerable<Cell>
{
	public Cell[,] Cells { get; private set; }
	public int Subsize { get; private set; }
    public int Size => Subsize * Subsize;
    
	// Constructor to initialize the board with specified size
	public Board(int size) {
		Subsize = (int) Math.Round(Math.Sqrt((float) size));
		Cells = new Cell[size, size];
		
		// Initialize board to all zeros (empty)
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				Cells[i, j] = new Cell();
			}
		}
	}

	public State GetState()
	{
		bool solved = true;
		foreach (Cell cell in this)
		{
			if (cell.Collisions > 0) return State.Invalid;
			if (cell.Value == 0) solved = false;
		}
		return solved ? State.Solved : State.Unsolved;
	}
	
	// Solve the board
	public Board? Solve()
	{
		// Check state of current board
		State state = GetState();
		if (state == State.Solved) return this;
		if (state == State.Invalid) return null;

		// Get the next cell to set
		int row = 0, col = 0;
		while (row < Size)
		{
			Cell cell = Cells[row, col];
			if (cell.Value == 0 && !cell.IsFixed) break;
			if (++col == Size)
			{
				row++;
				col = 0;
			}
		}
		if (row == Size) return null;
		
		// Find solution after setting the next cell
		var rand = new Random();
		var nums = Enumerable.Range(1, Size);
		foreach (byte num in nums.OrderBy(x => rand.Next()))
		{
			Board next = (Board) Clone();
			next.Set(row, col, num);
			Board? solution = next.Solve();
			if (solution != null) return solution;
		}
		return null;
	}
	
	// Creates a deep copy of the current board
	public object Clone() {
		var board = new Board(Size);
		for (int i = 0; i < Size; i++) {
			for (int j = 0; j < Size; j++)
			{
				board.Cells[i, j] = new Cell(Cells[i, j]);
			}
		}
		return board;
	}
	
	// Set value at specified cell
	public void Set(int i, int j, byte value)
	{
		if (i < 0 || i >= Size || j < 0 || j >= Size) return;
		Cell prev = Cells[i, j];
		if (prev.Value == value) return;
		if (prev.IsFixed) return;
		int collisions = 0;
		
		// Update cell collisions
		int b = ((i / Subsize) * Subsize) + (j / Subsize);
		foreach (var cell in GetRow(i).Concat(GetCol(j)).Concat(GetBlk(b)))
		{
			if (cell.Value != 0)
			{
				if (cell.Value == prev.Value)
				{
					cell.Collisions--;
				}

				if (cell.Value == value && value != 0)
				{
					cell.Collisions++;
					collisions++;
				}	
			}
		}
		
		// Update board
		Cells[i, j].Value = value;
		Cells[i, j].Collisions = collisions;
	}

	// Get specified cell in the board
	public Cell Get(int i, int j)
	{
		if (i < 0 || i >= Size || j < 0 || j >= Size) 
			return new Cell();
		
		return Cells[i, j];
	}

	// Get a specified row in the board
	public IEnumerable<Cell> GetRow(int r) {
		if (r < 0 || r >= Size)
			yield break;
		
		for (int j = 0; j < Size; j++) {
			yield return Cells[r, j];
		}
	}

	// Get a specified column in the board
	public IEnumerable<Cell> GetCol(int c)
	{
		if (c < 0 || c >= Size)
			yield break;
		
		for (int i = 0; i < Size; i++)
		{
			yield return Cells[i, c];
		}
	}

	// Get a specified block in the board
	public IEnumerable<Cell> GetBlk(int b) {
		if (b < 0 || b >= Size)
			yield break;
		
		for (int r = 0; r < Subsize; r++) {
			int i = ((b / Subsize) * Subsize) + r;
			for (int c = 0; c < Subsize; c++) {
				int j = ((b % Subsize) * Subsize) + c;
				yield return Cells[i,j];
			}
		}
	}
	
	// Get each cell in the board
	public IEnumerator<Cell> GetEnumerator()
	{
		for (int i = 0; i < Size; i++)
		{
			for (int j = 0; j < Size; j++)
			{
				yield return Cells[i, j];
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public static Board Create(int size, float fillPercent)
	{
		// Create solved board
		Board board = new Board(size);
		board = board.Solve()!;
        
		// Clear cells from board
		var rand = new Random();
		int numClear = (int) Math.Round(board.Size * board.Size * (1 - fillPercent));
		while (numClear > 0)
		{
			int row = rand.Next(0, board.Size);
			int col = rand.Next(0, board.Size);
			if (board.Get(row, col) is var cell && cell.Value != 0)
			{
				board.Set(row, col, 0);
				numClear--;
			} 
		}
		
		// Set all non-zero cells to fixed
		foreach (var cell in board)
		{
			if (cell.Value != 0)
			{
				cell.IsFixed = true;
			}
		}
		
		return board;
	}
}