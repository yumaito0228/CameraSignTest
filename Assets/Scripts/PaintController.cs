using Assets.Scripts;
using OscJack;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaintController : MonoBehaviour
{
    [SerializeField]
    private RawImage image = null;

    private Texture2D texture = null;

    [SerializeField]
    private int width = 4;

    [SerializeField]
    private int height = 4;

    private Vector2 prePos;
    public Vector2 touchPos { get; set; }

    private float clickTime, preClickTime;

    private OscClient client;
    private Camera mainCamera;
    private CameraSignAppManager cameraSignAppManager;
    
    private void Awake()
    {
        var oscConnection = CameraSignAppManager.Instance.oscConnection;
        client = new OscClient(oscConnection.host, oscConnection.port);
        mainCamera = FindObjectOfType<Camera>();
        cameraSignAppManager = CameraSignAppManager.Instance;
    }

    public void OnDrag(BaseEventData arg) //線を描画
    {
        var eventData = arg as PointerEventData; //タッチの情報取得

        // 押されているときの処理
        touchPos = eventData.position; //現在のポインタの座標

        var viewport = mainCamera.ScreenToViewportPoint(touchPos);
        client.Send(cameraSignAppManager.oscAddress, viewport.x, viewport.y);
        
        clickTime = eventData.clickTime; //最後にクリックイベントが送信された時間を取得

        var disTime = clickTime - preClickTime; //前回のクリックイベントとの時差

        var dir = prePos - touchPos; //直前のタッチ座標との差
        if (disTime > 0.01) dir = new Vector2(0, 0); //0.1秒以上間隔があいたらタッチ座標の差を0にする

        var dist = (int)dir.magnitude; //タッチ座標ベクトルの絶対値

        dir = dir.normalized; //正規化

        //指定のペンの太さ(ピクセル)で、前回のタッチ座標から今回のタッチ座標まで塗りつぶす
        for (var d = 0; d < dist; ++d)
        {
            var pos = touchPos + dir * d; //paint position
            pos.y -= height / 2.0f;
            pos.x -= width / 2.0f;
            for (var h = 0; h < height; ++h)
            {
                var y = (int)(pos.y + h);
                if (y < 0 || y > texture.height) continue; //タッチ座標がテクスチャの外の場合、描画処理を行わない

                for (var w = 0; w < width; ++w)
                {
                    var x = (int)(pos.x + w);
                    if (x >= 0 && x <= texture.width)
                    {
                        texture.SetPixel(x, y, Color.black); //線を描画
                    }
                }
            }
        }

        texture.Apply();
        prePos = touchPos;
        preClickTime = clickTime;
    }

    public void OnTap(BaseEventData arg) //点を描画
    {
        var eventData = arg as PointerEventData; //タッチの情報取得

        // 押されているときの処理
        touchPos = eventData.position; //現在のポインタの座標

        var pos = touchPos; //paint position
        pos.y -= height / 2.0f;
        pos.x -= width / 2.0f;

        for (var h = 0; h < height; ++h)
        {
            var y = (int)(pos.y + h);
            if (y < 0 || y > texture.height) continue; //タッチ座標がテクスチャの外の場合、描画処理を行わない
            for (var w = 0; w < width; ++w)
            {
                var x = (int)(pos.x + w);
                if (x >= 0 && x <= texture.width)
                {
                    texture.SetPixel(x, y, Color.black); //点を描画
                }
            }
        }

        texture.Apply();
    }

    private void Start()
    {
        var rect = image.gameObject.GetComponent<RectTransform>().rect;
        texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);

        //下の行追加（2021/10/21）
        WhiteTexture((int)rect.width, (int)rect.height);

        image.texture = texture;
    }

    //下の関数を追加（2021/10/21）
    //テクスチャを白色にする関数
    private void WhiteTexture(int texWidth, int texHeight)
    {
        for (var w = 0; w < texWidth; w++)
        {
            for (var h = 0; h < texHeight; h++)
            {
                texture.SetPixel(w, h, Color.white);
            }
        }

        texture.Apply();
    }
}