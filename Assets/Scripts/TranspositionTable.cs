using System.Collections.Generic;

public class TranspositionTable{
    public static Dictionary<int, int[]> Table => _table;
    private static readonly Dictionary<int, int[]> _table = new();

    public static bool Contains(int hashKey){
        return _table.ContainsKey(hashKey);
    }

    public static bool Add(int hashKey, int value, int depth){
        if (_table.ContainsKey(hashKey)){
            if (_table[hashKey][1] < depth){
                _table[hashKey][0] = value;
                _table[hashKey][1] = depth;
            }
            else{
                return false;
            }
        }
        else{
            _table.Add(hashKey, new []{value, depth});
        }

        return true;
    }

    public static bool Cutoff(int hashKey, int depth){
        return _table.ContainsKey(hashKey) && _table[hashKey][1] >= depth;
    }
}