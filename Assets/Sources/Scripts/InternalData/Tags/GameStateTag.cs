using System;
using Unity.Entities;

[Serializable]
public struct InitializeSystemsTag : IComponentData { }

[Serializable]
public struct SpawnInitialObjectsTag : IComponentData { }

[Serializable]
public struct CreateOperationsTag : IComponentData { }

[Serializable]
public struct GetPlayerActionsTag : IComponentData { }

[Serializable]
public struct FireCannonTag : IComponentData { }

[Serializable]
public struct CannonballHitOnIslandTag : IComponentData { }

[Serializable]
public struct UpdateObjectsPositionTag : IComponentData { }