using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> _data = new List<T>();
    public int Count => _data.Count;
    public void Push(T item)
    {
        _data.Add(item);
        int ci = _data.Count - 1; 
        while (ci > 0)
        {
            int pi = (ci - 1) / 2;
            // 親ノードと比較して、親ノードの方が小さい場合は終了
            if (_data[ci].CompareTo(_data[pi]) >= 0) break; 
            T tmp = _data[ci]; _data[ci] = _data[pi]; _data[pi] = tmp;
            ci = pi;
        }
    }
    public T Pop() 
    {
        if(_data.Count ==0) 
            throw new InvalidOperationException("PriorityQueue is empty");
        T ret = _data[0];
        int last = _data.Count - 1;
        _data[0] = _data[last];
        _data.RemoveAt(last);
        int i = 0;
        while (true) 
        {
            int l = 2 * i + 1;
            int r = l + 1;
            if (l >= _data.Count) break;
            // 左右の子ノードのうち、より小さい方を選ぶ
            int smallest = (r < _data.Count && _data[r].CompareTo(_data[l]) < 0) ? r : l;
            // 親ノードと比較して、親ノードの方が小さい場合は終了
            if (_data[i].CompareTo(_data[smallest]) <= 0) break;
            // 
            T tmp = _data[i];
            _data[i] = _data[smallest]; 
            _data[smallest] = tmp;
            i = smallest;
        }
        return ret;
    }

    public T Peek()
    {
        if(_data.Count ==0) 
            throw new InvalidOperationException("PriorityQueue is empty");
        return _data[0];
    }

    public void Clear()
    {
        _data.Clear();
    }
}
