using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataBrick
{
    public int currentIndex;
    public List<ItemBrick> itemBricks;
}
[System.Serializable]
public class OrderIdColorPaint
{
    public List<int> listColorCanPaint = new();
}
public class GeneratePictureManager : SingletonMonoBehaviour<GeneratePictureManager>
{
    [SerializeField] private GameObject prefab_brick;
    [SerializeField] private Transform tf_parent;
    /// <summary>
    /// key là id color
    /// </summary>
    [ShowInInspector]
    private Dictionary<int, DataBrick> dataBricks = new Dictionary<int, DataBrick>();
    private float sizePicture = 10;
    public List<OrderIdColorPaint> orderIdColorPaint = new();
    private int indexListColorPaint = 0;
    void Start()
    {

    }

    public void GeneratePicture(PixelData pixel)
    {
        ResetDataPicture();
        var sizeCell = sizePicture / pixel.width;
        for (int i = 0; i < pixel.palette.Count; i++)
        {
            dataBricks[i] = new DataBrick();
            dataBricks[i].currentIndex = 0;
            dataBricks[i].itemBricks = new List<ItemBrick>();
        }

        for (int i = 0; i < pixel.pixels.Count; i++)
        {
            for (int j = 0; j < pixel.pixels[i].row.Length; j++)
            {
                GameObject obj = Instantiate(prefab_brick, tf_parent);
                obj.transform.position = new Vector3(i * sizeCell, 0, j * sizeCell);
                obj.transform.localScale = new Vector3(sizeCell, 0, sizeCell);
                var idColor = pixel.pixels[i].row[j];
                Color color = pixel.palette[idColor].ToColor();
                var itemBrick = obj.GetComponent<ItemBrick>();
                itemBrick.GetDataColor(color);
                dataBricks[idColor].itemBricks.Add(itemBrick);
            }
        }
        tf_parent.transform.localEulerAngles = new Vector3(0, -90, 60);
        tf_parent.transform.position = new Vector3(sizePicture / 2, 0, 0);
    }
    public void CheckColorCanPaint()
    {
        int temp = 0;
        for (int i = 0; i < orderIdColorPaint[indexListColorPaint].listColorCanPaint.Count; i++)
        {
            var idColor = orderIdColorPaint[indexListColorPaint].listColorCanPaint[i];
            if (dataBricks[idColor].currentIndex == dataBricks[idColor].itemBricks.Count)
            {
                //complete color
                temp++;
            }
        }

        //chưa clear hết, break check
        if (temp != orderIdColorPaint[indexListColorPaint].listColorCanPaint.Count) return;

        //đã clear danh sách màu này, sang các màu tiếp theo
        indexListColorPaint++;

        if (indexListColorPaint >= orderIdColorPaint.Count)
        {
            //đã tô xong tranh, gọi win được rồi
            GameViewsManager.Instance.GetView<UIWin>().Show();
        }
        else
        {
            //chưa tô xong tranh, set color cho màu tiếp theo
        }
    }
    [Button]
    public void SetColorCanPaint()
    {
        for (int i = 0; i < orderIdColorPaint[indexListColorPaint].listColorCanPaint.Count; i++)
        {
            var idColor = orderIdColorPaint[indexListColorPaint].listColorCanPaint[i];
            for (int j = 0; j < dataBricks[idColor].itemBricks.Count; j++)
            {
                dataBricks[idColor].itemBricks[j].SetHint();
            }
        }
    }
    private void ResetDataPicture()
    {
        indexListColorPaint = 0;
        dataBricks.Clear();
        for (int i = 0; i < tf_parent.childCount; i++) Destroy(tf_parent.GetChild(i).gameObject);
    }
}
