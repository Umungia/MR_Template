using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference _togglePlanesAction;

    private ARPlaneManager _planeManager;
    private bool _isVisible = true;
    private int _numPlanesAddedOccurred = 0;

    void Start()
    {
        Debug.Log("-> SceneController::Start()");
        _planeManager = GetComponent<ARPlaneManager>();
        if (_planeManager is null)
        {
            Debug.LogError("-> Can't find 'ARPlaneManager' :(  ");
        }

        _togglePlanesAction.action.performed += OnTogglePlanesAction;
        _planeManager.trackablesChanged.AddListener(OnPlanesChanged);
    }

    void Update()
    {

    }

    private void OnTogglePlanesAction(InputAction.CallbackContext obj)
    {
        _isVisible = !_isVisible;
        float fillAlpha = _isVisible ? 0.3f : 0f;
        float lineAlpha = _isVisible ? 1.0f : 0f;

        Debug.Log("-> OnTogglePlanesAction() - trackables.count: " + _planeManager.trackables.count);

        foreach (var plane in _planeManager.trackables)
        {
            SetPlaneAlpha(plane, fillAlpha, lineAlpha);
        }
    }

    private void SetPlaneAlpha(ARPlane plane, float fillAlpha, float lineAlpha)
    {
        var meshRenderer = plane.GetComponentInChildren<MeshRenderer>();
        var lineRenderer = plane.GetComponentInChildren<LineRenderer>();

        if (meshRenderer != null)
        {
            // En URP la propiedad suele ser _BaseColor en lugar de _Color
            Color color = meshRenderer.material.HasProperty("_BaseColor") ?
                          meshRenderer.material.GetColor("_BaseColor") :
                          meshRenderer.material.color;

            color.a = fillAlpha;

            if (meshRenderer.material.HasProperty("_BaseColor"))
                meshRenderer.material.SetColor("_BaseColor", color);
            else
                meshRenderer.material.color = color; // Fallback para Standard Render Pipeline
        }

        if (lineRenderer != null)
        {
            Color startColor = lineRenderer.startColor;
            Color endColor = lineRenderer.endColor;

            startColor.a = lineAlpha;
            endColor.a = lineAlpha;

            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;
        }
    }

    private void OnPlanesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
    {
        if (args.added.Count > 0)
        {
            _numPlanesAddedOccurred++;

            foreach (var plane in _planeManager.trackables)
            {
                PrintPlaneLabel(plane);
            }

            Debug.Log("-> Number of planes : " + _planeManager.trackables.count);
            Debug.Log("-> Num Planes Added Occurrer: " + _numPlanesAddedOccurred);
        }
    }

    private void PrintPlaneLabel(ARPlane plane)
    {
        string label = plane.classifications.ToString();
        string log = $"Plane ID: {plane.trackableId}, Label: {label}";
    }

    private void OnDestroy()
    {
        Debug.Log("-> SceneController::OnDestroy()");
        _togglePlanesAction.action.performed -= OnTogglePlanesAction;
        _planeManager.trackablesChanged.RemoveListener(OnPlanesChanged);
    }
}