using System.Collections.Generic;
using UnityEngine;

public abstract class ECSWorld
{
    private List<ECSSystem> _systems = new List<ECSSystem>();

    private float _time = 0;

    public ECSWorld() {
        SystemAdd();
        Init();
    }

    public void Init()
    {
        _time = Time.time;
        foreach (var system in _systems)
        {
            system.Init();
        }
    }

    public abstract void SystemAdd();

    public void Update()
    {
        float dt = Time.time - _time;
        foreach (var system in _systems)
        {
            system.Execute(dt);
        }
        _time += dt;
        //ConsoleUtils.Log("º‰∏Ù ±º‰",dt);
    }

    public void DrawGrizmos()
    {
        foreach (var system in _systems)
        {
            system.DrawGizmos();
        }
    }

    public void Add(ECSSystem system)
    {
        _systems.Add(system);
    }

    public void Remove(ECSSystem system)
    {
        _systems.Remove(system);
    }

    public void Clear()
    {
        _systems.Clear();
    }
}
