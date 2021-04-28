using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrajectoryPredictionNoECS : MonoBehaviour
{
    public int maxIterations;

    private Scene _currentScene;
    private Scene _predictionScene;

    private PhysicsScene _currentPhysicsScene;
    private PhysicsScene _predictionPhysicsScene;

    private LineRenderer _lineRenderer;
    private GameObject _dummyGameObject;

    private void Start()
    {
        Physics.autoSimulation = false;

        _currentScene = SceneManager.GetActiveScene();
        _currentPhysicsScene = _currentScene.GetPhysicsScene();

        var parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        _predictionScene = SceneManager.CreateScene("Prediction", parameters);
        _predictionPhysicsScene = _predictionScene.GetPhysicsScene();

        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        if (_currentPhysicsScene.IsValid())
        {
            _currentPhysicsScene.Simulate(Time.fixedDeltaTime);
        }
    }

    public Vector3 Predict(GameObject predictable, Vector3 currentPosition, Vector3 force)
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

            _lineRenderer.positionCount = maxIterations;

            var iterationPosition = _dummyGameObject.transform.position;

            for (int i = 0; i < maxIterations; i++)
            {
                _predictionPhysicsScene.Simulate(Time.fixedDeltaTime);

                iterationPosition = _dummyGameObject.transform.position;
                _lineRenderer.SetPosition(i, iterationPosition);
                
            }

            Destroy(_dummyGameObject);

            return iterationPosition;
        }

        return currentPosition;
    }
}
