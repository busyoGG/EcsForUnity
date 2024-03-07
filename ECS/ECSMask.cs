using System.Collections.Generic;
using UnityEngine;

public class ECSMask
{
    private List<int> _mask;

    private int _size;

    public ECSMask() {
        _size = Mathf.CeilToInt(ECSManager.Ins().GetTotalCompNum() / 31f);
        _mask = new List<int>(_size);
        int data = 0;
        _mask.Add(data);
    }

    public void Set(int id)
    {
        _mask[id / 31] |= (1 << (id % 31));
    }

    public void Remove(int id)
    {
        _mask[id / 31] &= ~(1 << (id % 31));
    }

    public bool Has(int id)
    {
        return (_mask[id / 31] & (1 << (id % 31))) > 0;
    }

    //和其他mask比较

    public bool Or(ECSMask other)
    {
        for (int i = 0; i < _size; ++i)
        {
            if ((_mask[i] & other._mask[i]) > 0)
            {
                return true;
            }
        }
        return false;
    }

    public bool And(ECSMask other)
    {
        for (int i = 0; i < _size; ++i)
        {
            if ((_mask[i] & other._mask[i]) != _mask[i])
            {
                return false;
            }
        }
        return true;
    }

    public void Clear()
    {
        for(int i = 0; i < _size; ++i)
        {
            _mask[i] = 0;
        }
    }
}
