using System;
using Unity.Entities;

[Serializable]
public struct LoadMenuTag : IComponentData { }

[Serializable]
public struct LoadGameTag : IComponentData { }

[Serializable]
public struct InitializeGameSystemsTag : IComponentData { }

[Serializable]
public struct InGameTag : IComponentData { }

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
public struct CannonFiredTag : IComponentData { }

[Serializable]
public struct CannonballHitOnIslandTag : IComponentData { }

[Serializable]
public struct CannonballMisshitTag : IComponentData { }

[Serializable]
public struct UpdateUiTag : IComponentData { }

[Serializable]
public struct UpdateLabelTag : IComponentData { }

[Serializable]
public struct UpdateObjectsPositionTag : IComponentData { }

[Serializable]
public struct ReloadMenuTag : IComponentData { }

[Serializable]
public struct ReloadGameTag : IComponentData { }

/////////////
[Serializable]
public struct DeactivateSystemsTag : IComponentData { }