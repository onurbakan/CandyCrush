using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] dots;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;


    // Start is called before the first frame update
    void Start()
    {
        // Arraylerin birimlerini belirterek oluşturduk.
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //Her bir kareye erişebilmek için i,j şeklinde herbir kareye isim verdik. 
                Vector2 tempPosition = new Vector2(i, j);
                GameObject backgroundTile =  Instantiate(tilePrefab,tempPosition ,Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + " , " + j + " )";
                // Hem backgroundTile'a hem dot'lara i,j şeklinde isim verdik.
                int dotToUse = Random.Range(0, dots.Length);

                int maxIterations = 0;
                while (MatchesAt(i , j, dots[dotToUse]) && maxIterations < 100)
                {// 100 den fazla döngüye girmemesi için maxIterations verildi. MatchesAt kodu için yazıldı.                 
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;                    
                }
                maxIterations = 0;

                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "( " + i + " , " + j + " )";
                allDots[i, j] = dot;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {// Board oluştuğunda match olmuş obje görmemek için
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
            {// 1.altındaki VE 2.altındaki Aynı ise True döndür
                return true;
            }
            if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
            {// 1.solundaki VE 2.solundaki Aynı ise True döndür
                return true;
            }
        }
        else if (column <= 1 || row <= 1 )
        { 
            if (row > 1)
            {// column <= 1  ve row > 1 ise buraya girer ve row a bakar sadece
                if (allDots[column, row -1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {// row <= 1  ve column > 1 ise buraya girer ve row a bakar sadece
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            // Bu şekilde ayırmamız lazım yoksa sol column ve en alt row a kontrol yapamayız :/
        }

        // Değilse false döndür
        return false;
    }


   
}
