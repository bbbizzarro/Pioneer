using Godot;
using System;
using System.Collections.Generic;

public class PartyGroup : Node2D {
	List<PartyMember> partyMembers = new List<PartyMember>();

	public void Initialize(WorldMap worldMap) {
		// Get references to party sprites
		foreach (Node node in GetChildren()) {
			if (node.IsInGroup("PartyMember")) {
				partyMembers.Add((PartyMember)node);
			}
		}
		// Add party member to world scene
		for (int i = 0; i < partyMembers.Count; ++i)  {
			RemoveChild(partyMembers[i]);
			worldMap.AddChild(partyMembers[i]);
			float angle = i * (2f * Mathf.Pi )/ partyMembers.Count;
		}
	}

	public void SetIdleAnim() {
	}

	public void SetWalkAnim() {
	}

	public void Set(int position, string textureName) {
	}
	
	public void Unset(int position) {
	}
}
