using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Material lineMat;
    [SerializeField] private float lineWidth = 0.01f;
    private LineRenderer _lineRenderer;

    private LineRenderer LineRenderer
    {
        get
        {
            if (_lineRenderer == null)
            {
                _lineRenderer = gameObject.AddComponent<LineRenderer>();
                _lineRenderer.useWorldSpace = false;
                _lineRenderer.material = lineMat;
                _lineRenderer.startWidth = lineWidth;
                _lineRenderer.endWidth = lineWidth;
            }

            return _lineRenderer;
        }
    }
    private int _positionCount;

    private void Start()
    {
        // _lineRenderer = GetComponent<LineRenderer>();
        // ラインの座標指定を、このラインオブジェクトのローカル座標系を基準にするよう設定を変更
        // この状態でラインオブジェクトを移動・回転させると、描かれたラインもワールド空間に取り残されることなく、一緒に移動・回転
        // _lineRenderer.useWorldSpace = false;
        _positionCount = 0;
        // mainCamera = Camera.main;
    }

    public void Draw(Vector3 position)
    {
        var cameraTransform = mainCamera.transform;
        transform.position = cameraTransform.position + cameraTransform.forward * 10;
        transform.rotation = cameraTransform.rotation;
        
        
        var pos = transform.InverseTransformPoint(position);

        // さらにそれをローカル座標に直す。
        // pos = transform.InverseTransformPoint(pos);

        // 得られたローカル座標をラインレンダラーに追加する
        _positionCount++;
        LineRenderer.positionCount = _positionCount;
        LineRenderer.SetPosition(_positionCount - 1, pos);
    }

    public void Clear()
    {
        _positionCount = 0;
    }
}