using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CannonMuzzle : MonoBehaviour
{
    public bool IsFireButtonPressed { get; set; }
    public bool IsShootAllowed { get; set; }

    [Header("Specs")]
    
    [SerializeField] private Transform _muzzleTransform;

    [SerializeField] private GameObject _cannonballPrefab;

    //Aquí iría la declaración de los efectos de sonido con su respectivo [Header("Effects")]

    private EntityManager _entityManager;
    private Entity _cannonballEntityPrefab;

    private BlobAssetStore _blobAssetStore;
    
    protected virtual void Start()
    {
        var currentWorld = World.DefaultGameObjectInjectionWorld;

        _entityManager = currentWorld.EntityManager;

        var settings = GameObjectConversionSettings.FromWorld(currentWorld, _blobAssetStore = new BlobAssetStore());
        _cannonballEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_cannonballPrefab, settings);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (GameManager.IsGameOver())
        {
            return;
        }

        //if (IsShootAllowed && IsFireButtonPressed)
        //{
        //    FireCannonball();
        //}

        if (IsFireButtonPressed)
        {
            FireCannonball();
        }
    }

    protected void OnDestroy()
    {
        _blobAssetStore.Dispose();
    }

    public virtual void FireCannonball()
    {
        var cannonBallEntity = _entityManager.Instantiate(_cannonballEntityPrefab);

        _entityManager.SetComponentData(cannonBallEntity, new Translation { Value = _muzzleTransform.position});
        _entityManager.SetComponentData(cannonBallEntity, new Rotation { Value = _muzzleTransform.rotation });

        var side1 = Vector3.right - _muzzleTransform.position;
        var side2 = Vector3.forward - _muzzleTransform.position;

        Debug.Log("side 1: " + side1 + "side 2: " + side2 + "Cross: " + Vector3.Cross(side1, side2));

        _entityManager.AddComponentData(cannonBallEntity, new CannonShootData
        {
            Direction = Camera.main.transform.forward,
            //Extraer Force a un authoring component
            Force = 50f
        });
    }
}
