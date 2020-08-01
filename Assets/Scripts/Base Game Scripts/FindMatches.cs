using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Net.NetworkInformation;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();

    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isAdjacentBomb)
        {// Row bomb'ları column da match olursa patlaması için.
            currentMatches.Union(GetAdjacentPieces(dot1.column, dot1.row));
        }
        if (dot2.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot2.column, dot2.row));
        }
        if (dot3.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot3.column, dot3.row));
        }
        return currentDots;
    }

    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isRowBomb)//currentDot
        {// Row bomb'ları column da match olursa patlaması için.
            currentMatches.Union(GetRowPieces(dot1.row));
        }
        if (dot2.isRowBomb)//upDot
        {
            currentMatches.Union(GetRowPieces(dot2.row));
        }
        if (dot3.isRowBomb)//downDot
        {
            currentMatches.Union(GetRowPieces(dot3.row));
        }
        return currentDots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isColumnBomb)//currentDot
        {// Row bomb'ları column da match olursa patlaması için.
            currentMatches.Union(GetColumnPieces(dot1.column));
        }
        if (dot2.isColumnBomb)//upDot
        {
            currentMatches.Union(GetColumnPieces(dot2.column));
        }
        if (dot3.isColumnBomb)//downDot
        {
            currentMatches.Union(GetColumnPieces(dot3.column));
        }
        return currentDots;
    }

    private void AddToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        { // List e eklemek için
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);

    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];

                if (currentDot != null)
                {
                    Dot currentDotDot = currentDot.GetComponent<Dot>();
                    if (i > 0 && i < board.width - 1)
                    { // Sağ ve sol objeler match oluyor mu kontrolü, oluyorsa isMatched leri true
                        GameObject leftDot = board.allDots[i - 1, j];

                        GameObject rightDot = board.allDots[i + 1, j];

                        if (leftDot != null && rightDot != null)
                        {//shouldn't be null
                            Dot rightDotDot = rightDot.GetComponent<Dot>();
                            Dot leftDotDot = leftDot.GetComponent<Dot>();
                            if (leftDot != null && rightDot != null)
                            {
                                if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                                {

                                    currentMatches.Union(IsRowBomb(leftDotDot, currentDotDot, rightDotDot));

                                    /*if (currentDot.GetComponent<Dot>().isRowBomb
                                        || leftDot.GetComponent<Dot>().isRowBomb
                                        || rightDot.GetComponent<Dot>().isRowBomb)
                                    {// Row arrow bomb tetiklemek için.
                                        currentMatches.Union(GetRowPieces(j)); // Union kodunu System.Linq sayesinde yazdık
                                    }*/

                                    currentMatches.Union(IsColumnBomb(leftDotDot, currentDotDot, rightDotDot));

                                    /*if (currentDot.GetComponent<Dot>().isColumnBomb)
                                    {// Column bomb'ları row da match olursa patlaması için.
                                        currentMatches.Union(GetColumnPieces(i));
                                    }
                                    if (leftDot.GetComponent<Dot>().isColumnBomb)
                                    {
                                        currentMatches.Union(GetColumnPieces(i - 1));
                                    }
                                    if (rightDot.GetComponent<Dot>().isColumnBomb)
                                    {
                                        currentMatches.Union(GetColumnPieces(i + 1));
                                    }*/


                                    currentMatches.Union(IsAdjacentBomb(leftDotDot, currentDotDot, rightDotDot));


                                    GetNearbyPieces(leftDot, currentDot, rightDot);

                                    /*if (!currentMatches.Contains(leftDot))
                                    { // List e eklemek için
                                        currentMatches.Add(leftDot);
                                    }
                                    leftDot.GetComponent<Dot>().isMatched = true;

                                    if (!currentMatches.Contains(rightDot))
                                    {
                                        currentMatches.Add(rightDot);
                                    }
                                    rightDot.GetComponent<Dot>().isMatched = true;

                                    if (!currentMatches.Contains(currentDot))
                                    {
                                        currentMatches.Add(currentDot);
                                    }
                                    currentDot.GetComponent<Dot>().isMatched = true;*/


                                }
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1)
                    { // Üst ve alt objeler match oluyor mu kontrolü, oluyorsa isMatched leri true
                        GameObject upDot = board.allDots[i, j + 1];

                        GameObject downDot = board.allDots[i, j - 1];

                        if (upDot != null && downDot != null)
                        {


                            Dot downDotDot = downDot.GetComponent<Dot>();
                            Dot upDotDot = upDot.GetComponent<Dot>();
                            if (upDot != null && downDot != null)
                            {//shouldn't be null
                                if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                                {

                                    currentMatches.Union(IsColumnBomb(upDotDot, currentDotDot, downDotDot));

                                    /*if (currentDot.GetComponent<Dot>().isColumnBomb
                                        || upDot.GetComponent<Dot>().isColumnBomb
                                        || downDot.GetComponent<Dot>().isColumnBomb)
                                    {// Column arrow bomb tetiklemek için.
                                        currentMatches.Union(GetColumnPieces(i)); // Union kodunu System.Linq sayesinde yazdık
                                    }*/

                                    currentMatches.Union(IsRowBomb(upDotDot, currentDotDot, downDotDot));


                                    /*if (currentDot.GetComponent<Dot>().isRowBomb)
                                    {// Row bomb'ları column da match olursa patlaması için.
                                        currentMatches.Union(GetRowPieces(j));
                                    }
                                    if (upDot.GetComponent<Dot>().isRowBomb)
                                    {
                                        currentMatches.Union(GetRowPieces(j + 1));
                                    }
                                    if (downDot.GetComponent<Dot>().isRowBomb)
                                    {
                                        currentMatches.Union(GetRowPieces(j - 1));
                                    }*/


                                    currentMatches.Union(IsAdjacentBomb(upDotDot, currentDotDot, downDotDot));


                                    GetNearbyPieces(upDot, currentDot, downDot);

                                    /*if (!currentMatches.Contains(upDot))
                                    {// List e eklemek için
                                        currentMatches.Add(upDot);
                                    }
                                    upDot.GetComponent<Dot>().isMatched = true;

                                    if (!currentMatches.Contains(downDot))
                                    {
                                        currentMatches.Add(downDot);
                                    }
                                    downDot.GetComponent<Dot>().isMatched = true;

                                    if (!currentMatches.Contains(currentDot))
                                    {
                                        currentMatches.Add(currentDot);
                                    }
                                    currentDot.GetComponent<Dot>().isMatched = true;*/
                                }
                            }
                        }
                    }

                }
            }
        }
    }

    public void MatchPiecesOfColor(string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {//Check if that piece exists
                    if (board.allDots[i, j].tag == color)
                    {//Check the tag on that dot

                        //Set that dot to be matched
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            { // Döngüye referans aldığımız column ve rowdan bir eksik başladık ve 3x3 birimlik bir destroy birimi oluşturduk.
                //Check if the piece is inside the board?
                if (i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    if (board.allDots[i, j] != null)
                    {
                        dots.Add(board.allDots[i, j]);
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
        return dots;
    }

    List<GameObject> GetColumnPieces(int column)
    {// Sutündaki tüm objeleri destroy etmek için helper code.
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.allDots[column, i] != null)
            {
                Dot dot = board.allDots[column, i].GetComponent<Dot>();
                if (dot.isRowBomb)
                {
                    dots.Union(GetRowPieces(i)).ToList();
                }
                dots.Add(board.allDots[column, i]);
                dot.isMatched = true;
            }
        }
        return dots;
    }

    List<GameObject> GetRowPieces(int row)
    { // Satırdaki tüm objeleri destroy etmek için helper code.
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                Dot dot = board.allDots[i, row].GetComponent<Dot>();
                if (dot.isColumnBomb)
                {
                    dots.Union(GetColumnPieces(i)).ToList();
                }
                dots.Add(board.allDots[i, row]);
                dot.isMatched = true;
            }
        }
        return dots;
    }

    public void CheckBombs()
    { // Did the player move something?
        if (board.currentDot != null)
        {
            //Is the piece they moved matched?
            if (board.currentDot.isMatched)
            {
                //make it unmatched
                board.currentDot.isMatched = false;                
                ///Decide what kind of bomb to make
                ///int typeOfBomb = Random.Range(0, 100);
                ///if (typeOfBomb < 50)
                ///{
                ///    //Make a row bomb
                ///    board.currentDot.MakeRowBomb();
                ///}
                ///else if (typeOfBomb >= 50)
                ///{
                ///    //Make a column bomb
                ///    board.currentDot.MakeColumnBomb(); 
                ///}
                
                if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                    || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                { // Right ve Left swipe ise RowBomb
                    //Make a row bomb
                    board.currentDot.MakeRowBomb();
                }
                else
                {// Yukarı ve aşağı swipe seçenği kalıyor öyle ise ColumnBomb
                    //Make a column bomb
                    board.currentDot.MakeColumnBomb();
                }
            }
            //Is the other piece matched?
            else if (board.currentDot.otherDot != null)
            { // Dörtlü match'lerde farklı rengi kaydırarak match yaptığımızda bomb oluşması için helper code.
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                //Is the other Dot matched?
                if (otherDot.isMatched)
                {
                    //Make it unmatched
                    otherDot.isMatched = false;                    
                   /// //Decide what kind of bomb to make
                   /// int typeOfBomb = Random.Range(0, 100);
                   /// if (typeOfBomb < 50)
                   /// {
                   ///     //Make a row bomb
                   ///     otherDot.MakeRowBomb();
                   /// }
                   /// else if (typeOfBomb >= 50)
                   /// {
                   ///     //Make a column bomb
                   ///     otherDot.MakeColumnBomb();
                   /// }
                    
                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                    || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                    { // Right ve Left swipe ise RowBomb
                      //Make a row bomb
                        otherDot.MakeRowBomb();
                    }
                    else
                    {// Yukarı ve aşağı swipe seçenği kalıyor öyle ise ColumnBomb
                     //Make a column bomb
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
    }

}
