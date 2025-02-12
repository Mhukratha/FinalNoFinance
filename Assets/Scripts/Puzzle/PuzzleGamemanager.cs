using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class PuzzleGamemanager : MonoBehaviour
{
    [SerializeField] private Transform gameTrans;

    [SerializeField] private Transform piecePrefabs;

    private List<Transform> pieces;
    private int emptyLocation;

    private int size;
    private bool shuffling = false;

    //สร้างmethodCreateGamePieceเพื่อคำนวณความกว้างยาว
    private void CreateGamePiece(float gapThickness)
    {
        float width = 1 / (float)size; //with = 1 ชิ้นส่วนแต่ละชิ้นจะมีขนาด1ใน3หน่วย
        //สร้างตารางชองชิ้นส่วน โดยการทำซ้ำใช้Array 2 มิติ X*X
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                Transform piece = Instantiate(piecePrefabs, gameTrans);
                pieces.Add(piece);
                /*จำลองตัวอย่างเกม เราอยากให้เกมเป้นศูนย์กลาง เลยต้องการให้x มีRangeจาก -1 ถึง 1
                ต่อ ด้วย-1และเพิ่มความกว้างเป็น 2 เท่า * ด้วย col */
                piece.localPosition = new Vector3(-1 + (2 * width * col) + width, +1 - (2 * width * row) - width, 0);
                /*จากด้านบนทำให้เราอยู่จุดซ้ายสุดของPuzzleแต่จุดที่ต้องการยึดอยู่ตรงกลาง เราเลยต้องเพิ่มความกว้างเข้าไปอีกครึ่งหนึ่ง
                เมื่อเพิ่มขนาดเป็น 2 เท่าแล้ว นั่นเป็นแค่การแปรผันความกว้าง
                ทำแบบเดียวกันในพิกัด Y สุดท้ายตั้งค่า z = 0 และเมื่อได้ตำแหน่งที่ถูกต้อง ต่อไปต้องปรับขนาด*/
                
                //ตั้งค่าlocalscale ให้มี with เป็น2เท่า แล้ว - ความหนาของช่องว่างออกไปเพื่อให้มีช่องว่างเล็กๆระหว่างแต่ล่ะ 4 เหลี่ยม
                piece.localScale = ((2 * width) - gapThickness) * Vector3.one;
                
                //กำหนดให้4เหลี่ยมแต่ละอันใช้ค่าดัชนีเข้ามาช่วย
                piece.localScale = ((2 * width) - gapThickness) * Vector3.one;
                piece.name = $"{(row * size) + col}";
                //ใช้ในส่วนของการตรวจจับว่าผุ้เล่นเล่นจบไหม
                if ((row == size - 1) && (col == size -1))//save pos ในemptylo และ hide GObj
                {
                    emptyLocation = (size * size) - 1;
                    piece.gameObject.SetActive(false);
                } //ต้องสร้างภาพปริศนาทั้งหมด 9 ชิ้น โดยให้ชิ้นสุดท้ายเป็นที่ว่าง

                // ***3 obj สามารถเปลี่ยนพิกัดของ UV ของแต่ละรูปให้ตรงกับส่วนที่ถูกต้องของภาพได้***
                else
                {
                    //ต้องการเปลี่ยนพิกัดUVต้องนำค่าจาก MeshFilterบน Obj Quad
                    float gap = gapThickness / 2;
                    Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                    Vector2[] uv = new Vector2[4];//ใช้พิกัดUVสำหรับจุดยอดแต่ละจุดของภาพ จากที่ดูมาใช้ 4 พิกัด ซึ้งมีค่าตั้งแต่ 0-1
                    //ลำดับของจุดยอดใน Quad ทั้ง 4 จุดจะถูกกำหนดเป็น ซ้ายบน ขวาบน ซ้ายล่าง ขวาล่าง
                    uv[0] = new Vector2((width * col) + gap, 1 - ((width * (row + 1)) - gap)); // *width ด้วย col กับ high ด้วยแถว
                    uv[1] = new Vector2((width * (col + 1)) - gap, 1 - ((width * (row + 1)) - gap));
                    uv[2] = new Vector2((width * col) + gap, 1 - ((width * row) + gap));
                    uv[3] = new Vector2((width * (col + 1)) - gap, 1 - ((width * row) * gap));
                    
                    //แนบพิกัดUV ที่สร้างมาใหม่เข้ากับQuad
                    mesh.uv = uv;
                }
            }
        }
    }
    
    
    //เริ่มต้นกำหนดขนาดของเกมโดยจะใช้ตารางขนาด3*3
    void Start()
    {
        pieces = new List<Transform>();
        size = 3;
        CreateGamePiece(0.01f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!shuffling && CheckCompletetion())
        {
            shuffling = true;
            StartCoroutine(WaitShuffle(0.5f));
        }
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), 
                Vector2.zero);
            if (hit)
            {
                for (int i = 0; i < pieces.Count; i++)
                {
                    if (pieces[i] == hit.transform)
                    {
                        if (SwapIfValid(i, -size, size)) {break;}
                        if (SwapIfValid(i, +size, size)) {break;}
                        if (SwapIfValid(i, -1, 0)) {break;}
                        if (SwapIfValid(i, +1, size - 1)) {break;}
                    }
                }
            }
        }
    }

    private bool SwapIfValid(int i, int offset, int colCheck)
    {
        if (((i % size) != colCheck) && ((i + offset) == emptyLocation))
        {
            (pieces[i], pieces[i + offset]) = (pieces[i + offset], pieces[i]);
            (pieces[i].localPosition, pieces[i + offset].localPosition) = ((pieces[i + offset].localPosition
                , pieces[i].localPosition));

            emptyLocation = i;
            return true;
        }
        return false;
    }

    private bool CheckCompletetion()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].name != $"(i)")
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator WaitShuffle(float duration)
    {
        yield return new WaitForSeconds(duration);
        Shuffle();
        shuffling = false;
    }

    private void Shuffle()
    {
        int count = 0;
        int last = 0;
        while (count < (size * size * size))
        {
            //pick random location
            int rnd = Random.Range(0, size * size);
            //only thing we forbid is undoing the last move
            if (rnd == last)
            {
                continue;
            }

            last = emptyLocation;
            
            //try surrounding spaces looking for valid move
            if (SwapIfValid(rnd, -size, size))
            {
                count++;
            }else if (SwapIfValid(rnd, +size, size))
            {
                count++;
            }else if (SwapIfValid(rnd, -1, 0))
            {
                count++;
            }
            else if (SwapIfValid(rnd, +1, size-1))
            {
                count++;
            }
        }
    }
}

