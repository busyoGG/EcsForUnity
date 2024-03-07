
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class Entity
{
    public int id { get; set; }

    public string name { get; set; }

    public ECSMask mask { get; set; }

    private Dictionary<Type, Comp> _compsInEntity = new Dictionary<Type, Comp>();

    private Dictionary<Type, Comp> _compsRemoved = new Dictionary<Type, Comp>();

    public Entity() { 
        mask = new ECSMask();
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="comp"></param>
    /// <returns></returns>
    public T Add<T>() where T : Comp
    {
        //Comp comp;
        Type comp = typeof(T);
        Comp instance;
        int compId = ECSManager.Ins().GetCompId(comp);
        if (mask.Has(compId))
        {
            instance = _compsInEntity[comp];
            return instance as T;
        }

        if (_compsRemoved.ContainsKey(comp))
        {
            instance = _compsRemoved[comp];
            _compsRemoved.Remove(comp);
        }
        else
        {
            instance = ECSManager.Ins().CreateComp(comp);
        }


        if (instance == null)
        {
            ConsoleUtils.Warn("组件未注册");
        }
        else
        {
            _compsInEntity.Add(comp, instance);

            mask.Set(compId);

            ECSManager.Ins().CompChangeBroadcast(this, compId);
        }

        return instance as T;
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="comp"></param>
    /// <returns></returns>
    public void Add(Type comp)
    {
        //Comp comp;
        Comp instance;
        int compId = ECSManager.Ins().GetCompId(comp);
        if (mask.Has(compId))
        {
            return;
        }

        if (_compsRemoved.ContainsKey(comp))
        {
            instance = _compsRemoved[comp];
            _compsRemoved.Remove(comp);
        }
        else
        {
            instance = ECSManager.Ins().CreateComp(comp);
        }


        if (instance == null)
        {
            ConsoleUtils.Warn("组件未注册");
        }
        else
        {
            _compsInEntity.Add(comp, instance);

            mask.Set(compId);

            ECSManager.Ins().CompChangeBroadcast(this, compId);
        }
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    /// <param name="compIds"></param>
    public void AddComps(params Type[] comps)
    {
        foreach (Type comp in comps)
        {
            Add(comp);
        }
    }

    /// <summary>
    /// 获取组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="compId"></param>
    /// <returns></returns>
    public T Get<T>() where T : Comp
    {
        Comp instance;
        _compsInEntity.TryGetValue(typeof(T), out instance);
        return instance as T;
    }

    /// <summary>
    /// 是否有组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool Has<T>()
    {
        Type comp = typeof(T);
        int compId = ECSManager.Ins().GetCompId(comp);
        return mask.Has(compId);
    }

    /// <summary>
    /// 移除组件
    /// </summary>
    /// <param name="compId"></param>
    /// <param name="recycle"></param>
    public void Remove<T>(bool recycle = true)
    {
        Type comp = typeof(T);
        int compId = ECSManager.Ins().GetCompId(comp);
        if (mask.Has(compId))
        {
            Comp instance = _compsInEntity[comp];

            if (recycle)
            {
                ECSManager.Ins().RemoveComp(instance);
            }
            else
            {
                _compsRemoved.Add(comp, instance);
            }

            _compsInEntity.Remove(comp);
            mask.Remove(compId);
            ECSManager.Ins().CompChangeBroadcast(this, compId);
        }
        else
        {
            ConsoleUtils.Warn("组件不存在");
        }
    }

    /// <summary>
    /// 移除组件
    /// </summary>
    /// <param name="compId"></param>
    /// <param name="recycle"></param>
    public void Remove(Type comp, bool recycle = true)
    {
        int compId = ECSManager.Ins().GetCompId(comp);
        if (mask.Has(compId))
        {
            Comp instance = _compsInEntity[comp];

            if (recycle)
            {
                ECSManager.Ins().RemoveComp(instance);
            }
            else
            {
                _compsRemoved.Add(comp, instance);
            }

            _compsInEntity.Remove(comp);
            mask.Remove(compId);
            ECSManager.Ins().CompChangeBroadcast(this, compId);
        }
        else
        {
            ConsoleUtils.Warn("组件不存在");
        }
    }

    public void RemoveComps(bool recycle, params Type[] comps)
    {
        foreach (Type comp in comps)
        {
            Remove(comp, recycle);
        }
    }

    public void Clear()
    {
        foreach(var data in _compsInEntity.ToList())
        {
            Remove(data.Key);
        }

        foreach(var data in _compsRemoved.ToList())
        {
            ECSManager.Ins().RemoveComp(data.Value);
        }

        _compsInEntity.Clear();
        _compsRemoved.Clear();
        mask.Clear();
    }
}
