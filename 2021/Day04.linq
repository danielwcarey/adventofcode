<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day04
//
// Links
//   https://adventofcode.com/2021/day/4
//
// Answer
//   Part1 := 49686
//   Part2 := 26878
//
async Task Main() {

	// sample data
	//var data = await ParseAsync(new StringReader(sample));// sample.Split("\n").Select(str => int.Parse(str)).ToArray();

	// data
	string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day04-input.txt");
	var data = await ParseAsync(new StringReader(await File.ReadAllTextAsync(path)));

	//Part1(data);
	Part2(data);
}

void Part1(Game game) {
	do {
		var drawnNumber = game.Draw();
		if (drawnNumber == null) break;

		var winners = game.GetWinners();
		if (winners.Count > 1) {
			"More than one board".Dump("Part1");
			break;
		} else if (winners.Count == 1) {
			var score = drawnNumber * winners[0].SumOfUnMarked;
			score.Dump("Part1");
			break;
		}
	} while (true);
}

void Part2(Game game) {
	var state = 1;
	Board? lastToWinBoard = null;
	do {
		var drawnNumber = game.Draw();
		if (drawnNumber == null) break;

		switch (state) {
			case 1: {// looking for last board to win
					var losers = game.GetLosers();
					if (losers.Count == 1) {
						lastToWinBoard = losers[0];
						state = 2;
					}
				}
				break;
			case 2:  // looking for last number to win
				if (lastToWinBoard != null && lastToWinBoard.IsWinner()) {
					var score = drawnNumber * lastToWinBoard.SumOfUnMarked;
					score.Dump("Part2");
					state = 0;
				}
				break;

		}
	} while (state != 0);
}

async Task<Game> ParseAsync(TextReader streamReader) {
	Game result = new();
	Board? board = null;
	int boardNumber = 1;

	int state = 1;
	while (state != 0) {
		var line = await streamReader.ReadLineAsync();
		if (line == null) { state = 0; break; }

		switch (state) {

			case 1: { // Read numbers
					var numbers = line?.Split(",");
					if (numbers != null) {
						foreach (var number in numbers.Select(n => int.Parse(n))) {
							result.NumberSequence.Enqueue(number);
						}
					}
					state = 2;
					break;
				}
			case 2: { // Skip blank lines					
					if (!string.IsNullOrWhiteSpace(line)) {
						board = new Board { Name = boardNumber.ToString() };
						boardNumber++;
						state = 3;
						goto case 3;
					}
					break;
				}
			case 3: { // Add Card rows/Create new Card
					if (string.IsNullOrWhiteSpace(line)) {
						if (board != null) result.Boards.Add(board);
						board = null;
						state = 2; ;
						break;
					}
					if (board != null) board.Rows.Add(readCardLines(line));
					break;
				}
			default:
				state = 0;
				break;
		}
	}
	if (board != null) result.Boards.Add(board);
	return result;

	List<Number> readCardLines(string line) {
		var normalLine = Regex.Replace(line.Trim(), @"\s\s*", ";");
		return normalLine.Split(";")
			.Select(x => new Number { Value = int.Parse(x.Trim()) })
			.ToList();
	}
}

class Number {
	public int Value { get; set; }
	public bool IsMarked { get; set; }
}

partial class Board {
	public string Name { get; set; } = "";
	public List<List<Number>> Rows { get; set; } = new();
}

partial class Game {
	public Queue<int> NumberSequence { get; set; } = new();
	public List<Board> Boards { get; set; } = new();
}

partial class Game {
	public int? Draw() {
		if (NumberSequence.Count == 0) return null;
		var result = NumberSequence.Dequeue();

		foreach (var board in Boards) {
			foreach (var row in board.Rows) {
				foreach (var number in row) {
					if (number.Value == result)
						number.IsMarked = true;
				}
			}
		}
		return result;
	}

	public List<Board> GetWinners() {
		List<Board> result = new();
		foreach (var board in Boards) {
			if (board.IsWinner())
				result.Add(board);
		}
		return result;
	}

	public List<Board> GetLosers() {
		List<Board> result = new();
		foreach (var board in Boards) {
			if (!board.IsWinner())
				result.Add(board);
		}
		return result;
	}
}

partial class Board {
	public int ColumnCount => Rows[0].Count;

	public bool IsWinner() {
		foreach (var row in Rows) {
			if (row.All(n => n.IsMarked)) return true;
		}
		for (var column = 0; column < ColumnCount; column++) {
			if (GetColumn(column).All(n => n.IsMarked)) return true;
		}
		return false;
	}

	public List<Number> GetColumn(int column) {
		var result = new List<Number>();
		foreach (var row in Rows) {
			result.Add(row[column]);
		}
		return result;
	}

	public int SumOfUnMarked => Rows.SelectMany(r => r.Where(n => !n.IsMarked)).Sum(n => n.Value);

}

#region Sample
string sample =
@"7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1

22 13 17 11  0
 8  2 23  4 24
21  9 14 16  7
 6 10  3 18  5
 1 12 20 15 19

 3 15  0  2 22
 9 18 13 17  5
19  8  7 25 23
20 11 10 24  4
14 21 16 12  6

14 21 17 24  4
10 16 15  9 19
18  8 23 26 20
22 11 13  6  5
 2  0 12  3  7
";
#endregion