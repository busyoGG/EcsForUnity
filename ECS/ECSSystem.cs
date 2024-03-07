using System.Collections.Generic;

public abstract class ECSSystem
{
    private Group _group;

    protected float _dt = 0;

    private List<Entity> _enters = new List<Entity>();

    private List<Entity> _removes = new List<Entity>();

    public ECSSystem()
    {
        _group = ECSManager.Ins().CreateGroup(Filter());
        _group.AddEntityListener(EntityEnter, EntityRemove);
    }

    public void Execute(float dt)
    {
        _dt = dt;

        List<Entity> entities = _group.GetEntities();

        OnEnter(_enters);
        OnUpdate(entities);
        OnRemove(_removes);

        _removes.Clear();
        _enters.Clear();
    }

    public void DrawGizmos()
    {
        List<Entity> entities = _group.GetEntities();
        OnDrawGizmos(entities);
    }

    private void EntityEnter(Entity entity)
    {
        _enters.Add(entity);
    }

    private void EntityRemove(Entity entity)
    {
        List<Entity> entities = _group.GetEntities();
        if (entities.Contains(entity))
        {
            _removes.Add(entity);
        }
    }

    public virtual void Init() { }
    public virtual void OnDestroy()
    {
        _group.RemoveEntityListener(EntityEnter, EntityRemove);
    }
    public virtual void OnEnter(List<Entity> entities) { }
    public virtual void OnRemove(List<Entity> entities) { }

    public virtual void OnDrawGizmos(List<Entity> entities) { }
    public abstract ECSMatcher Filter();
    public abstract void OnUpdate(List<Entity> entities);
}
