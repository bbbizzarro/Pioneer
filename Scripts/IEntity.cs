using Godot;
using System;
using Pioneer;

public delegate void PositionEventHandler(IEntity entity, Vector2 position);
public delegate void EntityEventHandler(IEntity entity);

public interface IEntity {
    void SubscribeToOnDestroyed(EntityEventHandler call);
}
