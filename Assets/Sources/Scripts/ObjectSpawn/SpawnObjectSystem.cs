using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Collider = Unity.Physics.Collider;
using Material = UnityEngine.Material;

//public class SpawnObjectSystem : SpawnObjectSystemBase<SpawnSettings> {}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(BuildPhysicsWorld))]
public abstract class SpawnObjectSystemBase<T> : SystemBase where T : SpawnSettings 
{
    public NativeList<BlobAssetReference<Collider>> CreatedColliders;

    private EntityQuery _objectsToSpawnQuery;

    protected float _friction;
    protected float _restitution;
    protected Material _material;

    static readonly Type k_DrawComponent = typeof(Unity.Physics.Authoring.DisplayBodyColliders)
        .GetNestedType("DrawComponent", BindingFlags.NonPublic);

    static readonly MethodInfo k_DrawComponent_BuildDebugDisplayMesh = k_DrawComponent
        .GetMethod("BuildDebugDisplayMesh", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(BlobAssetReference<Collider>) }, null);

    static readonly Type k_DisplayResult = k_DrawComponent.GetNestedType("DisplayResult");

    static readonly FieldInfo k_DisplayResultsMesh = k_DisplayResult.GetField("Mesh");
    static readonly PropertyInfo k_DisplayResultsTransform = k_DisplayResult.GetProperty("Transform");

    protected override void OnCreate()
    {
        CreatedColliders = new NativeList<BlobAssetReference<Collider>>(Allocator.Persistent);

        _objectsToSpawnQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(T)},
            None = new ComponentType[] {typeof(ObjectSpawnedTag)}
        });

        RequireForUpdate(GetEntityQuery(typeof(T)));
    }

    protected override void OnUpdate()
    {
        if (_objectsToSpawnQuery.CalculateEntityCount() == 0) { return; }

        using var entities = _objectsToSpawnQuery.ToEntityArray(Allocator.TempJob);
        foreach (var entity in entities)
        {
            T spawnSettings = EntityManager.GetComponentObject<T>(entity);
            
            _material = spawnSettings.Material;

            OnBeforeInstantiateObject(ref spawnSettings);

            Create(spawnSettings);

            EntityManager.AddComponentData(entity, new ObjectSpawnedTag());
        }
    }

    public abstract void Create(T spawnSettings);

    public virtual void OnBeforeInstantiateObject(ref T spawnSettings) {}


    protected override void OnDestroy()
    {
        foreach (var collider in CreatedColliders)
        {
            if (collider.IsCreated)
            {
                collider.Dispose();
            }
        }

        CreatedColliders.Dispose();
    }

    public Entity CreateDynamicBody(BlobAssetReference<Collider> collider, float3 position, quaternion orientation,
        float3 linearVelocity, float3 angularVelocity, float mass)
    {
        return CreateBody(collider, position, orientation, linearVelocity, angularVelocity, mass, true);
    }

    public Entity CreateStaticBody(BlobAssetReference<Collider> collider, float3 position, quaternion orientation)
    {
        return CreateBody(collider, position, orientation, float3.zero, float3.zero, 0.0f, false);
    }

    public Entity CreateBody(BlobAssetReference<Collider> collider,  float3 position, quaternion orientation,
        float3 linearVelocity, float3 angularVelocity, float mass, bool isDynamic)
    {
        ComponentType[] componentTypes = new ComponentType[isDynamic ? 9 : 6];

        componentTypes[0] = typeof(RenderMesh);
        componentTypes[1] = typeof(RenderBounds);
        componentTypes[2] = typeof(Translation);
        componentTypes[3] = typeof(Rotation);
        componentTypes[4] = typeof(LocalToWorld);
        componentTypes[5] = typeof(PhysicsCollider);
        
        if (isDynamic)
        {
            componentTypes[6] = typeof(PhysicsVelocity);
            componentTypes[7] = typeof(PhysicsMass);
            componentTypes[8] = typeof(PhysicsDamping);
        }

        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        Entity entity = entityManager.CreateEntity(componentTypes);

        entityManager.SetComponentData(entity, new Translation { Value = position });
        entityManager.SetComponentData(entity, new Rotation { Value = orientation });
        entityManager.SetComponentData(entity, new LocalToWorld());

        var colliderComponent = new PhysicsCollider {Value = collider};
        entityManager.SetComponentData(entity, colliderComponent);


        CreateRenderMeshForCollider(entityManager, entity, collider, _material);

        if (isDynamic)
        {
            entityManager.SetComponentData(entity, PhysicsMass.CreateDynamic(colliderComponent.MassProperties, mass));

            // Calculate the angular velocity in local space from rotation and world angular velocity
            float3 angularVelocityLocal = math.mul(math.inverse(colliderComponent.MassProperties.MassDistribution.Transform.rot), angularVelocity);

            entityManager.SetComponentData(entity, new PhysicsVelocity
            {
                Linear = linearVelocity,
                Angular = angularVelocityLocal
            });

            entityManager.SetComponentData(entity, new PhysicsDamping
            {
                Linear = 0.01f,
                Angular = 0.05f
            });
        }

        return entity;
    }

    public static void CreateRenderMeshForCollider(EntityManager entityManager, Entity entity, BlobAssetReference<Collider> collider, Material material)
    {
        var mesh = new Mesh {hideFlags = HideFlags.DontSave};
        var instances = new List<CombineInstance>(8);
        var numVertices = 0;

        foreach (var displayResult in (IEnumerable)k_DrawComponent_BuildDebugDisplayMesh.Invoke(null, new object[] { collider }))
        {
            var instance = new CombineInstance
            {
                mesh = k_DisplayResultsMesh.GetValue(displayResult) as Mesh,
                transform = (float4x4)k_DisplayResultsTransform.GetValue(displayResult)
            };

            instances.Add(instance);
            numVertices += mesh.vertexCount;
        }

        mesh.indexFormat = numVertices > UInt16.MaxValue ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
        mesh.CombineMeshes(instances.ToArray());
        mesh.RecalculateBounds();

        entityManager.SetSharedComponentData(entity, new RenderMesh
        {
            mesh = mesh,
            material = material,
            castShadows = ShadowCastingMode.On
        });

        entityManager.SetComponentData(entity, new RenderBounds { Value = mesh.bounds.ToAABB() });
    }

    public void Conversion()
    {
        var meshConversionSystem = World.GetOrCreateSystem<MeshRendererConversion>();
        
    }
}