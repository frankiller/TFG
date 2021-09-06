using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrajectoryPredictionNoECS : ScriptableObject
{
    private int maxIterations;

    private Scene _currentScene;
    private Scene _predictionScene;

    private PhysicsScene _currentPhysicsScene;
    private PhysicsScene _predictionPhysicsScene;

    private GameObject _dummyGameObject;

    private void Awake()
    {
        Physics.autoSimulation = false;

        _currentScene = SceneManager.GetActiveScene();
        _currentPhysicsScene = _currentScene.GetPhysicsScene();

        var parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        _predictionScene = SceneManager.CreateScene("Prediction", parameters);
        _predictionPhysicsScene = _predictionScene.GetPhysicsScene();

        //var inputVariablesQuery = World.DefaultGameObjectInjectionWorld.EntityManager;
        //var inputEntity = inputVariablesQuery.GetSingletonEntity<InputVariables>();
        //var inputVariables = GetComponent<InputVariables>(inputEntity);
        maxIterations = 75;
    }

    public void Simulate()
    {
        if (GameManager.IsGameOver()) { return; }

        if (_currentPhysicsScene.IsValid())
        {
            _currentPhysicsScene.Simulate(Time.fixedDeltaTime);
        }
    }

    public float3 Predict(GameObject predictable, float3 currentPosition, Vector3 force)
    {
        if (_currentPhysicsScene.IsValid() && _predictionPhysicsScene.IsValid())
        {
            if (_dummyGameObject == null)
            {
                _dummyGameObject = Instantiate(predictable);
                SceneManager.MoveGameObjectToScene(_dummyGameObject, _predictionScene);
            }

            _dummyGameObject.transform.position = currentPosition;
            _dummyGameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

            var iterationPosition = _dummyGameObject.transform.position;

            for (int i = 0; i < maxIterations; i++)
            {
                _predictionPhysicsScene.Simulate(Time.fixedDeltaTime);

                iterationPosition = _dummyGameObject.transform.position;
            }

            Destroy(_dummyGameObject);

            return iterationPosition;
        }

        return currentPosition;
    }
}
