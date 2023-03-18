using Godot;
using System;
using System.Collections.Generic;
using Pioneer;

public class World : YSort {
	List<IEntity> _entities = new List<IEntity>();
	HashSet<Vector2Int> _closedTiles = new HashSet<Vector2Int>();

	public override void _Ready() {
		CollectChildNodes();
		//InstanceScene("Market");
	}

	public void InstanceScene(string ID) {
		var scene = SceneDB.Instance.GetScene(ID);
		if (scene != null) {
			var node = scene.Instance();
			AddChild(node);
			TrackNode(node);
		}
	}

	private void CollectChildNodes() {
		foreach (Node node in GetChildren()) {
			if (node.IsInGroup("Entity")) {
				TrackNode(node);
			}
		}
	}

	private void HandleDestroyedEntity(IEntity entity) {
		_entities.Remove(entity);
	}

	private Vector2Int WorldToGrid(Vector2 pos) {
		return new Vector2Int(Mathf.RoundToInt(pos.x / Globals.PixelsPerUnit),
							  Mathf.RoundToInt(pos.y / Globals.PixelsPerUnit));
	}

	private void TrackNode(Node node) {
		try {
			var entity = (IEntity)node;
			_entities.Add(entity);
			entity.SubscribeToOnDestroyed(HandleDestroyedEntity);
		}
		catch {
			GD.PrintErr("A non entity node is marked as in entity group.");
		}
	}
}
