using UnityEngine;
using UnityEngine.Animations;

public class Healthbar : MonoBehaviour
{
    private float originalY;
    private RotationConstraint rotationConstraint;
    private GameObject stationaryObject;

    private void Awake()
    {
        rotationConstraint = transform.GetComponent<RotationConstraint>();
    }

    private void Start()
    {
        stationaryObject = GameManager.Instance.gameObject;
        if (stationaryObject == null)
        {
            stationaryObject = GameObject.Find("/StationaryObject");
            if (stationaryObject == null)
                stationaryObject = new GameObject("StationaryObject");
        }
        originalY = transform.localPosition.y;
        var constraintSource = new ConstraintSource();
        constraintSource.sourceTransform = stationaryObject.transform;
        constraintSource.weight = 1;
        rotationConstraint.AddSource(constraintSource);
    }

    private void Update()
    {
        transform.position = new Vector3(
            transform.parent.position.x,
            transform.parent.position.y + originalY,
            transform.parent.position.z);
    }
}
