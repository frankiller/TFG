using System;
using Unity.Entities;

[Serializable]
public enum GameState
{
    LoadMenuScene,
    InitializeSystemsTag,
    SpawnInitialObjects,
    CreateOperations,
    FinishedCreateOperations,
    FinishedSpawnTargets,
    GetPlayerActions,
    FireCannon,
    CannonballHitOnIsland
}

[Serializable]
public struct GameStateData : IComponentData
{
    public GameState Value;
}

[Serializable]
public struct InitializeSystemsTag : IComponentData { }

[Serializable]
public struct SpawnInitialObjectsTag : IComponentData { }

[Serializable]
public struct CreateOperationsTag : IComponentData { }

[Serializable]
public struct FinishedCreateOperationsTag : IComponentData { }

[Serializable]
public struct FinishedSpawnTargetsTag : IComponentData { }

[Serializable]
public struct GetPlayerActionsTag : IComponentData { }

[Serializable]
public struct FireCannonTag : IComponentData { }

[Serializable]
public struct CannonballHitOnIslandTag : IComponentData { }

[Serializable]
public struct UpdateObjectsPositionTag : IComponentData { }

[Serializable]
public struct DeactivateSystemsTag : IComponentData { }

[Serializable]
public struct LoadMenuSceneTag : IComponentData { }

[Serializable]
public struct LoadGameSceneTag : IComponentData { }

[Serializable]
public struct ReloadMenuSceneTag : IComponentData { }