<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day03
//
// Links
//   https://adventofcode.com/2021/day/3
//
// Answer
//   Part1 := gammaRate = 1565, EpsilonRate = 2530 -> PowerRate = 3959450
//   Part2 := 7440311
//
using static System.Math;

(UInt64 number, int bitCount) ConvertFromBinaryString(string binaryString) {
    int bitCount = 0;
    UInt64 number = 0;

    foreach (var digit in binaryString) {
        bitCount++;
        number = digit == '1' ? (number << 1) | 1 : (number << 1);
    }
    return (number, bitCount);
}

async Task Main() {

    // sample data
    //var dataInfo = sample.Split("\n").Select(str => ConvertFromBinaryString(str.Trim()));

    // data
    string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day03-input.txt");
    var dataInfo = (await File.ReadAllLinesAsync(path)).Select(str => ConvertFromBinaryString(str)).ToArray();

    var data = dataInfo.Select(x => x.number).ToArray();
    var bitCount = dataInfo.Max(x => x.bitCount);

    //Part1(data, bitCount);
    Part2(data, bitCount);
}

(int[] oneBitCount, int[] zeroBitCount) GetBitCounts(UInt64[] data, int bitCount) {
    var oneBitCount = new int[bitCount + 1];
    var zeroBitCount = new int[bitCount + 1];

    foreach (var input in data) {
        for (var bit = 0; bit < bitCount; bit++) {
            var position = (UInt64)Math.Pow(2, bit);
            if ((input & position) > 0) oneBitCount[bit]++; else zeroBitCount[bit]++;
        }
    }
    return (oneBitCount, zeroBitCount);
}

void Part1(UInt64[] data, int bitCount) {

    var (oneBitCount, zeroBitCount) = GetBitCounts(data, bitCount);

    UInt64 gammaRate = 0;
    UInt64 epsilonRate = 0;

    for (var bit = 0; bit < bitCount; bit++) {
        var position = (UInt64)Math.Pow(2, bit);

        if (oneBitCount[bit] > zeroBitCount[bit]) // more 1
            gammaRate |= position;
        else if (oneBitCount[bit] > 0 && oneBitCount[bit] < zeroBitCount[bit])// more 0
            epsilonRate |= position;
    }
    var powerRate = gammaRate * epsilonRate;
    $"GammaRate = {gammaRate}, EpsilonRate = {epsilonRate} => PowerRate {powerRate}".Dump("Part1");
}

void Part2(UInt64[] data, int bitCount) {
    var oxygenGeneratorRating = new List<UInt64>(data);
    for (var bit = bitCount - 1; bit >= 0; bit--) {
        var position = (UInt64)Math.Pow(2, bit);
        var (oneBitOxygenCount, zeroBitOxygenCount) = GetBitCounts(oxygenGeneratorRating.ToArray(), bitCount);
        var removeItems = new List<UInt64>();

        if (oneBitOxygenCount[bit] == zeroBitOxygenCount[bit]) {
            removeItems.AddRange(oxygenGeneratorRating.Where(value => (value & position) == 0).Select(v => v));
        } else if (oneBitOxygenCount[bit] > 0 && oneBitOxygenCount[bit] > zeroBitOxygenCount[bit]) {
            removeItems.AddRange(oxygenGeneratorRating.Where(value => (value & position) == 0).Select(v => v));
        } else if (zeroBitOxygenCount[bit] > 0 && zeroBitOxygenCount[bit] > oneBitOxygenCount[bit]) {
            removeItems.AddRange(oxygenGeneratorRating.Where(value => (value & position) > 0).Select(v => v));
        }

        foreach (var removeItem in removeItems) {
            oxygenGeneratorRating.Remove(removeItem);
        }

        if (oxygenGeneratorRating.Count == 1) break;
    }

    var co2ScrubberRating = new List<UInt64>(data);
    for (var bit = bitCount - 1; bit >= 0; bit--) {
        var position = (UInt64)Math.Pow(2, bit);
        var (oneBitCo2Count, zeroBitCo2Count) = GetBitCounts(co2ScrubberRating.ToArray(), bitCount);
        var removeItems = new List<UInt64>();

        if (oneBitCo2Count[bit] == zeroBitCo2Count[bit]) {
            removeItems.AddRange(co2ScrubberRating.Where(value => (value & position) > 0).Select(v => v));
        } else if (oneBitCo2Count[bit] > 0 && oneBitCo2Count[bit] > zeroBitCo2Count[bit]) {
            removeItems.AddRange(co2ScrubberRating.Where(value => (value & position) > 0).Select(v => v));
        } else if (zeroBitCo2Count[bit] > 0 && zeroBitCo2Count[bit] > oneBitCo2Count[bit]) {
            removeItems.AddRange(co2ScrubberRating.Where(value => (value & position) == 0).Select(v => v));
        }

        foreach (var removeItem in removeItems) {
            co2ScrubberRating.Remove(removeItem);
        }

        if (co2ScrubberRating.Count == 1) break;
    }
    
    var lifeSupportRating = oxygenGeneratorRating[0] * co2ScrubberRating[0];

    lifeSupportRating.Dump("Part2");
}

#region Sample
string sample =
@"00100
11110
10110
10111
10101
01111
00111
11100
10000
11001
00010
01010";
#endregion