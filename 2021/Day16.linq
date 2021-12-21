<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Base
//
// Links
//   https://adventofcode.com/2021/day/16
//
// Answer
//   Part1 := 906
//   Part2 := 819324480368
//
using System.Numerics;

void Main() {
    //var packetString = "D2FE28"; // literal
    //var packetString = "38006F45291200"; // operator
    //var packetString = "EE00D40C823060";
    //var packetString = "8A004A801A8002F478";
    //var packetString = "620080001611562C8802118E34";
    //var packetString = "C0015000016115A2E0802F182340";
    //var packetString = "A0016C880162017C3686B18A3D4780";

    //var packetString = "C200B40A82";
    //var packetString = "04005AC33890";
    //var packetString = "880086C3E88112";
    //var packetString = "CE00C43D881120";
    //var packetString = "D8005AC2A8F0";
    //var packetString = "";
    //var packetString = "";

    string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day16-input.txt");
    var packetString = File.ReadAllText(path);

    var packetBitString = "";
    foreach (var ch in packetString) {
        packetBitString += ByteLookup[ch];
    }
    packetBitString.Dump();
    var packetBitStringLength = packetBitString.Length;

    var position = 0;
    int versionSum = 0;
    BigInteger value = 0;

    do {
        value += ReadPacket();

        var (version, id) = ReadHeader(false);
        if (version == 0) break;

    } while (position < packetBitString.Length);

    $"versionSum = {versionSum}".Dump();
    $"value = {value}".Dump();

    return;

    // ==================================== //

    bool ReadBoolean(bool advance = true) {
        var savePosition = position;
        var result = ReadInt32(1) == 1 ? true : false;

        if (!advance) position = savePosition;
        return result;
    }

    int ReadInt32(int bitCount, bool advance = true) {

        var result = Convert.ToInt32(packetBitString[position..(position + bitCount)], 2);

        if (advance) position += bitCount;
        return result;
    }

    (int version, int id) ReadHeader(bool advance = true) {
        int savePosition = position;

        if (position + 6 >= packetBitString.Length) return (0, 0); // we are at the end

        var version = ReadInt32(3);
        var id = ReadInt32(3);

        if (!advance) position = savePosition;
        return (version, id);
    }

    BigInteger ReadPacket(bool advance = true) {
        BigInteger result = 0;
        var savePosition = position;
        var (version, id) = ReadHeader(false);

        if (id == 4) {
            result = ReadLiteralPacket();
        } else {
            result = ReadOperatorPacket();
        }

        if (!advance) position = savePosition;
        return result;
    }

    // id = 4
    BigInteger ReadLiteralPacket(bool advance = true) {
        var savePosition = position;
        var (version, id) = ReadHeader();

        BigInteger result = 0;
        bool groupBit = false;

        do {
            groupBit = ReadBoolean();
            result = (result << 4) + ReadInt32(4);
        } while (groupBit);

        $"version {version}, id {id}, value {result}".Dump();
        versionSum += version;

        if (!advance) position = savePosition;
        return result;
    }

    // id != 4
    BigInteger ReadOperatorPacket(bool advance = true) {
        var savePosition = position;
        var (version, id) = ReadHeader();

        BigInteger result = 0;
        List<BigInteger> subPacketValues = new();

        var lengthTypeId = ReadInt32(1);
        if (lengthTypeId == 0) {
            $"id {id}, lengthTypeId {lengthTypeId}".Dump();
            versionSum += version;

            var totalBitLength = ReadInt32(15);
            savePosition = position + totalBitLength;

            do {
                var data = ReadPacket();
                subPacketValues.Add(data);
            } while (position < savePosition);

        } else {
            $"id {id}, lengthTypeId {lengthTypeId}".Dump();
            versionSum += version;

            var numberOfSubPackets = ReadInt32(11);
            for (var x = 0; x < numberOfSubPackets; x++) {
                subPacketValues.Add(ReadPacket());
            }
        }

        switch (id) {
            case (int)PacketEnum.Sum:
                foreach (var value in subPacketValues)
                    result += value;
                break;
            case (int)PacketEnum.Product:
                result = 1;
                foreach (var item in subPacketValues) {
                    result = item * result;
                }
                break;
            case (int)PacketEnum.Minimum:
                result = subPacketValues.Min();
                break;
            case (int)PacketEnum.Maximum:
                result = subPacketValues.Max();
                break;
            case (int)PacketEnum.GreaterThan:
                result = (subPacketValues[0] > subPacketValues[1]) ? 1 : 0;
                break;
            case (int)PacketEnum.LessThan:
                result = (subPacketValues[0] < subPacketValues[1]) ? 1 : 0;
                break;
            case (int)PacketEnum.EqualTo:
                result = (subPacketValues[0] == subPacketValues[1]) ? 1 : 0;
                break;
        }

        if (!advance) position = savePosition;
        return result;
    }
}
enum PacketEnum {
    Sum = 0,
    Product = 1,
    Minimum = 2,
    Maximum = 3,
    Literal = 4,
    GreaterThan = 5,
    LessThan = 6,
    EqualTo = 7
}

#region ByteLookup
Dictionary<char, string> ByteLookup = new() {
    { '0', "0000" },
    { '1', "0001" },
    { '2', "0010" },
    { '3', "0011" },
    { '4', "0100" },
    { '5', "0101" },
    { '6', "0110" },
    { '7', "0111" },
    { '8', "1000" },
    { '9', "1001" },
    { 'A', "1010" },
    { 'B', "1011" },
    { 'C', "1100" },
    { 'D', "1101" },
    { 'E', "1110" },
    { 'F', "1111" }
};
#endregion