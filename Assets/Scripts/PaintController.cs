using Assets.Scripts;
using OscJack;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaintController : MonoBehaviour
{
    [SerializeField]
    private RawImage _image = null;

    private Texture2D _texture = null;

    [SerializeField]
    private int _width = 4;

    [SerializeField]
    private int _height = 4;

    private Vector2 _prePos;
    public Vector2 TouchPos { get; set; }

    private float _clickTime, _preClickTime;

    // [SerializeField] private UnityEvent<Vector2> _onDragEvent;

    private OscClient _client;
    private void Awake()
    {
        var oscConnection = CameraSignAppManager.Instance.oscConnection;
        _client = new OscClient(oscConnection.host, oscConnection.port);
    }


    public void OnDrag(BaseEventData arg) //線を描画
    {
        var _event = arg as PointerEventData; //タッチの情報取得

        // 押されているときの処理
        TouchPos = _event.position; //現在のポインタの座標
        // Debug.Log(TouchPos);
        Debug.Log(FindObjectOfType<Camera>().ScreenToViewportPoint(TouchPos));
        // _onDragEvent.Invoke(TouchPos);
        var viewport = FindObjectOfType<Camera>().ScreenToViewportPoint(TouchPos);
        _client.Send("/OscJack/position", viewport.x, viewport.y);
        
        _clickTime = _event.clickTime; //最後にクリックイベントが送信された時間を取得

        var disTime = _clickTime - _preClickTime; //前回のクリックイベントとの時差

        var width = _width; //ペンの太さ(ピクセル)
        var height = _height; //ペンの太さ(ピクセル)

        var dir = _prePos - TouchPos; //直前のタッチ座標との差
        if (disTime > 0.01) dir = new Vector2(0, 0); //0.1秒以上間隔があいたらタッチ座標の差を0にする

        var dist = (int)dir.magnitude; //タッチ座標ベクトルの絶対値

        dir = dir.normalized; //正規化

        //指定のペンの太さ(ピクセル)で、前回のタッチ座標から今回のタッチ座標まで塗りつぶす
        for (var d = 0; d < dist; ++d)
        {
            var _pos = TouchPos + dir * d; //paint position
            _pos.y -= height / 2.0f;
            _pos.x -= width / 2.0f;
            for (var h = 0; h < height; ++h)
            {
                var y = (int)(_pos.y + h);
                if (y < 0 || y > _texture.height) continue; //タッチ座標がテクスチャの外の場合、描画処理を行わない

                for (var w = 0; w < width; ++w)
                {
                    var x = (int)(_pos.x + w);
                    if (x >= 0 && x <= _texture.width)
                    {
                        _texture.SetPixel(x, y, Color.black); //線を描画
                    }
                }
            }
        }

        _texture.Apply();
        _prePos = TouchPos;
        _preClickTime = _clickTime;
    }

    public void OnTap(BaseEventData arg) //点を描画
    {
        var _event = arg as PointerEventData; //タッチの情報取得

        // 押されているときの処理
        TouchPos = _event.position; //現在のポインタの座標

        var width = _width; //ペンの太さ(ピクセル)
        var height = _height; //ペンの太さ(ピクセル)

        var _pos = TouchPos; //paint position
        _pos.y -= height / 2.0f;
        _pos.x -= width / 2.0f;

        for (var h = 0; h < height; ++h)
        {
            var y = (int)(_pos.y + h);
            if (y < 0 || y > _texture.height) continue; //タッチ座標がテクスチャの外の場合、描画処理を行わない
            for (var w = 0; w < width; ++w)
            {
                var x = (int)(_pos.x + w);
                if (x >= 0 && x <= _texture.width)
                {
                    _texture.SetPixel(x, y, Color.black); //点を描画
                }
            }
        }

        _texture.Apply();
    }

    private void Start()
    {
        var rect = _image.gameObject.GetComponent<RectTransform>().rect;
        _texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);

        //下の行追加（2021/10/21）
        WhiteTexture((int)rect.width, (int)rect.height);

        _image.texture = _texture;
    }

    //下の関数を追加（2021/10/21）
    //テクスチャを白色にする関数
    private void WhiteTexture(int width, int height)
    {
        for (var w = 0; w < width; w++)
        {
            for (var h = 0; h < height; h++)
            {
                _texture.SetPixel(w, h, Color.white);
            }
        }

        _texture.Apply();
    }
}