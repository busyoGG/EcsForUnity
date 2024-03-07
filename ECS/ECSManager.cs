


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using Unity.VisualScripting;

public class ECSManager : Singleton<ECSManager>
{
    private int _compId = 1;

    private int _entityId = 0;

    private int _total = 0;

    private Dictionary<Type, int> _comps = new Dictionary<Type, int>();

    private Dictionary<ECSMatcher, Group> _groups = new Dictionary<ECSMatcher, Group>();
    /// <summary>
    /// 组件变化监听字典
    /// </summary>
    private Dictionary<int, List<Action<Entity>>> _onCompChangeAction = new Dictionary<int, List<Action<Entity>>>();
    /// <summary>
    /// 实体对象池
    /// </summary>
    private Stack<Entity> _entityPool = new Stack<Entity>();

    /// <summary>
    /// 实体对象
    /// </summary>
    private Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();

    private Dictionary<int, Stack<Comp>> _compPool = new Dictionary<int, Stack<Comp>>();

    public void Init()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(ECSManager));
        Type[] types = assembly.GetTypes();
        foreach (Type type in types)
        {
            //获取自定义特性的过程中自动执行构造函数
            var attr = type.GetCustomAttribute<CompRegister>();
            if (attr != null)
            {
                _total++;
            }
        }
    }

    public void SetTotalCompNum(int num)
    {
        _total = num;
    }

    public int GetTotalCompNum()
    {
        return _total;
    }

    public int GetCompId()
    {
        return _compId++;
    }

    public int GetCompId(Type comp)
    {
        return _comps[comp];
    }

    public Dictionary<Type, int> GetAllCompTypes()
    {
        return _comps;
    }

    /// <summary>
    /// 注册组件
    /// </summary>
    /// <param name="comp"></param>
    /// <param name="compId"></param>
    public void CompRegister(Type comp, int compId)
    {
        _comps.Add(comp, compId);
        _compPool.Add(compId, new Stack<Comp>());
        ConsoleUtils.Log("注册类:", compId, comp);
    }

    /// <summary>
    /// 创建组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="comp"></param>
    /// <returns></returns>
    public Comp CreateComp(Type comp)
    {
        if (!_comps.ContainsKey(comp))
        {
            ConsoleUtils.Warn("未找到对应组件:", comp);
            return null;
        }
        else
        {
            int compId = _comps[comp];
            Comp instance;
            Stack<Comp> queue;
            _compPool.TryGetValue(compId, out queue);
            if (queue == null || queue.Count == 0)
            {
                instance = ClassFactory.CreateClass<Comp>(comp);
                instance.compId = compId;
            }
            else
            {
                instance = queue.Pop();
            }

            return instance;
        }
    }

    /// <summary>
    /// 回收组件
    /// </summary>
    /// <param name="comp"></param>
    public void RemoveComp(Comp comp)
    {
        Stack<Comp> queue;
        _compPool.TryGetValue(comp.compId, out queue);
        if (queue == null)
        {
            queue = new Stack<Comp>();
            _compPool.Add(comp.compId, queue);
        }

        comp.Reset();
        queue.Push(comp);
    }

    public Group CreateGroup(ECSMatcher matcher)
    {
        Group res;
        _groups.TryGetValue(matcher, out res);
        if (res == null)
        {
            res = new Group(matcher);
            _groups.Add(matcher, res);
            List<int> compIds = matcher.GetCompIds();
            foreach (int compId in compIds)
            {
                List<Action<Entity>> actionList;
                _onCompChangeAction.TryGetValue(compId, out actionList);
                if (actionList == null)
                {
                    actionList = new List<Action<Entity>>();
                    _onCompChangeAction.Add(compId, actionList);
                }
                actionList.Add(res.OnCompChange);
            }
        }
        return res;
    }

    public Entity CreateEntity()
    {
        if (_entityPool.Count == 0)
        {
            Entity entity = new Entity();
            entity.id = _entityId++;
            _entities.Add(entity.id,entity);
            return entity;
        }
        else
        {
            Entity entity = _entityPool.Pop();
            _entities.Add(entity.id, entity);
            return _entityPool.Pop();
        }
    }

    public Entity GetEntity(int id)
    {
        Entity entity;
        _entities.TryGetValue(id, out entity);
        return entity;
    }

    public void RemoveEntity(Entity entity)
    {
        entity.Clear();
        _entityPool.Push(entity);
        _entities.Remove(entity.id);
    }

    public void CompChangeBroadcast(Entity entity, int compId)
    {
        List<Action<Entity>> actionList;
        _onCompChangeAction.TryGetValue(compId, out actionList);
        if (actionList != null)
        {
            foreach (Action<Entity> action in actionList)
            {
                action.Invoke(entity);
            }
        }
    }

    public ECSMatcher AnyOf(params Type[] comps)
    {
        return new ECSMatcher().AnyOf(comps);
    }

    public ECSMatcher AllOf(params Type[] comps)
    {
        return new ECSMatcher().AllOf(comps);
    }

    public ECSMatcher ExcludeOf(params Type[] comps)
    {
        return new ECSMatcher().ExcludeOf(comps);
    }

    public ECSMatcher OnlyOf(params Type[] comps)
    {
        return new ECSMatcher().OnlyOf(comps);
    }
}
