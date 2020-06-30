using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using UnityEngine;

public enum GameState
{// Karışıklık, bug olmaması için nesnelerin hareketi durumunda wait, bittiğinde move statelerini ayarlama
    wait,
    move
}

public class Board : MonoBehaviour
{
    
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet; // Nesnelerin yukarıdan kayarak inmesi için tanımlandı.
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;


    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
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
                Vector2 tempPosition = new Vector2(i, j + offSet);//offset ne kadar yukardan ineceğini ayarlayan parametre
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
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;
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

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            //How many elements are in the matched pieces list from findmatches?
            if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
            {
                findMatches.CheckBombs();
            }

            // Destroy the object
            findMatches.currentMatches.Remove(allDots[column, row]);//List te null objeler kalmaması için

            // Destroy edilen objenin parlaması için, destroyEffect prefab kodu.
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f); // Kalıntı bırakmaması hafızada yer işgal etmemesi için silindi.
            
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {// destroy all the matches on the board
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i,j);
                }
            }

        }
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    { // nullCount counter and Decrease as many row as nullCount
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount >0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null; // Bug fixed
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {// Match edip destroy ettiğimiz bir nesne var ise yeni nesne oluştur ve oraya yerleştir. (Helper Function)
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    { // Eğer match olan bir nesne var ise true döndür yoksa false. (Helper Function)
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    { // Boş (null) haneleri doldur ve bekle.
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        { // Match var mı diye test et bekle varsa yoket yoksa while'dan çık.
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;

    }
   
}
