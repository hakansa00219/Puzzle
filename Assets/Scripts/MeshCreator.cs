using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreator : MonoBehaviour
{
    public Polygon polygon;
    private Vector2[] points;
    public Mesh CreateMesh(int pieceIndex, int puzzleSize, Vector2Int textureResolution, AllPieces puzzleData)
    {
        #region Knob Value Generation
        int scaleNumber = 50;
        int multiplair = puzzleSize / scaleNumber;

        Mesh mesh = new Mesh();

        Values R = new Values
        {
            offset = (int)(Random.Range(10, 30) * multiplair),
            offset2 = (int)(Random.Range(10, 30) * multiplair),
            pos = (int)(Random.Range(5, 20) * multiplair),
            pos2 = (int)(Random.Range(30, 45) * multiplair),
            startPos = (int)(Random.Range(15, 20) * multiplair),
            endPos = (int)(Random.Range(30, 35) * multiplair)
        };
        Values D = new Values
        {
            offset = (int)(Random.Range(10, 30) * multiplair),
            offset2 = (int)(Random.Range(10, 30) * multiplair),
            pos = (int)(Random.Range(5, 20) * multiplair),
            pos2 = (int)(Random.Range(30, 45) * multiplair),
            startPos = (int)(Random.Range(15, 20) * multiplair),
            endPos = (int)(Random.Range(30, 35) * multiplair)
        };
        Values U = new Values
        {
            offset = (int)(Random.Range(10, 30) * multiplair),
            offset2 = (int)(Random.Range(10, 30) * multiplair),
            pos = (int)(Random.Range(5, 20) * multiplair),
            pos2 = (int)(Random.Range(30, 45) * multiplair),
            startPos = (int)(Random.Range(15, 20) * multiplair),
            endPos = (int)(Random.Range(30, 35) * multiplair)
        };
        Values L = new Values
        {
            offset = (int)(Random.Range(10, 30) * multiplair),
            offset2 = (int)(Random.Range(10, 30) * multiplair),
            pos = (int)(Random.Range(5, 20) * multiplair),
            pos2 = (int)(Random.Range(30, 45) * multiplair),
            startPos = (int)(Random.Range(15, 20) * multiplair),
            endPos = (int)(Random.Range(30, 35) * multiplair)
        };
        #endregion

        #region Piece Edge Types
        int xResolution = textureResolution.x / puzzleSize;
        int yResolution = textureResolution.y / puzzleSize;

        List<PieceEdge> pieceEdgeData = FindPieceType(pieceIndex, xResolution, yResolution);

        #endregion

        #region Piece Data Save
        PieceData pData = new PieceData();

        pData.pieceEdgeData = pieceEdgeData;
        int[] sideRandomizer = new int[4] { Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2) };
        Side[] sides = new Side[4] { (Side)sideRandomizer[0], (Side)sideRandomizer[1], (Side)sideRandomizer[2], (Side)sideRandomizer[3] };

        pData.data.Add(Direction.LEFT, new Seed { isEdgeContainsKnob = !pieceEdgeData.Contains(PieceEdge.LEFT),side = sides[0], values = !pieceEdgeData.Contains(PieceEdge.LEFT) ? L : null });
        pData.data.Add(Direction.RIGHT, new Seed { isEdgeContainsKnob = !pieceEdgeData.Contains(PieceEdge.RIGHT), side = sides[1], values = !pieceEdgeData.Contains(PieceEdge.RIGHT) ? R : null });
        pData.data.Add(Direction.UP, new Seed { isEdgeContainsKnob = !pieceEdgeData.Contains(PieceEdge.UP), side = sides[2], values = !pieceEdgeData.Contains(PieceEdge.UP) ? U : null });
        pData.data.Add(Direction.DOWN, new Seed { isEdgeContainsKnob = !pieceEdgeData.Contains(PieceEdge.DOWN), side = sides[3], values = !pieceEdgeData.Contains(PieceEdge.DOWN) ? D : null });

        puzzleData.AddPiece(pieceIndex, pData);
        #endregion

        #region Edge's Mesh Bezier Values Definition
        Vector3[] Lpoints = new Vector3[4], Rpoints = new Vector3[4], Upoints = new Vector3[4] , Dpoints = new Vector3[4];

        PieceData leftPiece = pieceIndex + 1 <= xResolution * yResolution ? puzzleData.GetPiece(pieceIndex + 1) : null;
        PieceData upPiece = pieceIndex + xResolution <= xResolution * yResolution ? puzzleData.GetPiece(pieceIndex + xResolution) : null;

       
        if (!pieceEdgeData.Contains(PieceEdge.LEFT))
        {
            Lpoints = new Vector3[4]
            {
            new Vector3(0,L.startPos,0),
            new Vector3(pData.data[Direction.LEFT].side == Side.Outside ? -L.offset : L.offset ,L.pos,0),
            new Vector3(pData.data[Direction.LEFT].side == Side.Outside ? -L.offset2 : L.offset2,L.pos2,0),
            new Vector3(0,L.endPos,0)
            };
        }
        if(!pieceEdgeData.Contains(PieceEdge.RIGHT))
        {
            Rpoints = new Vector3[4]
            {
            new Vector3(puzzleSize, R.startPos, 0),
            new Vector3(pData.data[Direction.RIGHT].side == Side.Outside ? (puzzleSize + R.offset) : (puzzleSize - R.offset), R.pos , 0),
            new Vector3(pData.data[Direction.RIGHT].side == Side.Outside ? (puzzleSize + R.offset2) : (puzzleSize - R.offset2), R.pos2 , 0),
            new Vector3(puzzleSize, R.endPos, 0)
            };
        }
        if(!pieceEdgeData.Contains(PieceEdge.DOWN))
        {
            Dpoints = new Vector3[4]
            {
            new Vector3(D.startPos, 0 , 0),
            new Vector3(D.pos, pData.data[Direction.DOWN].side == Side.Outside ? - D.offset : D.offset, 0),
            new Vector3(D.pos2, pData.data[Direction.DOWN].side == Side.Outside ? - D.offset2 : D.offset2, 0),
            new Vector3(D.endPos, 0 , 0)
            };
        }
        if(!pieceEdgeData.Contains(PieceEdge.UP))
        {
            Upoints = new Vector3[4]
            {
            new Vector3(U.startPos,puzzleSize,0),
            new Vector3(U.pos, pData.data[Direction.UP].side == Side.Outside ? puzzleSize + U.offset : puzzleSize - U.offset,0),
            new Vector3(U.pos2, pData.data[Direction.UP].side == Side.Outside ? puzzleSize + U.offset2 : puzzleSize - U.offset2,0),
            new Vector3(U.endPos,puzzleSize,0)
            };
        }

        if ((pieceEdgeData.Contains(PieceEdge.UP) && pieceEdgeData.Contains(PieceEdge.RIGHT)) ||
           (pieceEdgeData.Contains(PieceEdge.DOWN) && pieceEdgeData.Contains(PieceEdge.RIGHT)) ||
           (pieceEdgeData.Count == 1 && pieceEdgeData.Contains(PieceEdge.UP)) ||
           (pieceEdgeData.Count == 1 && pieceEdgeData.Contains(PieceEdge.RIGHT)) ||
           (pieceEdgeData.Count == 1 && pieceEdgeData.Contains(PieceEdge.DOWN)) ||
           (pieceEdgeData.Count == 1 && pieceEdgeData.Contains(PieceEdge.MIDDLE)))
        {
            pData.data.Remove(Direction.LEFT);
            Lpoints = new Vector3[4]
            {
                new Vector3(0,leftPiece.data[Direction.RIGHT].values.startPos, 0),
                new Vector3(leftPiece.data[Direction.RIGHT].side == Side.Outside ?  leftPiece.data[Direction.RIGHT].values.offset : - leftPiece.data[Direction.RIGHT].values.offset , leftPiece.data[Direction.RIGHT].values.pos),
                new Vector3(leftPiece.data[Direction.RIGHT].side == Side.Outside ?  leftPiece.data[Direction.RIGHT].values.offset2 : - leftPiece.data[Direction.RIGHT].values.offset2 , leftPiece.data[Direction.RIGHT].values.pos2),
                new Vector3(0,leftPiece.data[Direction.RIGHT].values.endPos,0)
            };
            pData.data.Add(Direction.LEFT, new Seed
            {
                isEdgeContainsKnob = true,
                side = leftPiece.data[Direction.RIGHT].side == Side.Inside ? Side.Outside : Side.Inside,
                values = new Values
                {
                    endPos = leftPiece.data[Direction.RIGHT].values.endPos,
                    offset = leftPiece.data[Direction.RIGHT].side == Side.Outside ? leftPiece.data[Direction.RIGHT].values.offset : -leftPiece.data[Direction.RIGHT].values.offset,
                    offset2 = leftPiece.data[Direction.RIGHT].side == Side.Outside ? leftPiece.data[Direction.RIGHT].values.offset2 : -leftPiece.data[Direction.RIGHT].values.offset2,
                    pos = leftPiece.data[Direction.RIGHT].values.pos,
                    pos2 = leftPiece.data[Direction.RIGHT].values.pos2,
                    startPos = leftPiece.data[Direction.RIGHT].values.startPos
                }
            });
        }
        if ((pieceEdgeData.Contains(PieceEdge.DOWN) && pieceEdgeData.Contains(PieceEdge.LEFT)) ||
            (pieceEdgeData.Contains(PieceEdge.DOWN) && pieceEdgeData.Contains(PieceEdge.RIGHT)) ||
            (pieceEdgeData.Count == 1 && pieceEdgeData.Contains(PieceEdge.LEFT)) ||
            (pieceEdgeData.Count == 1 && pieceEdgeData.Contains(PieceEdge.RIGHT)) ||
            (pieceEdgeData.Count == 1 && pieceEdgeData.Contains(PieceEdge.DOWN)) ||
            (pieceEdgeData.Count == 1 && pieceEdgeData.Contains(PieceEdge.MIDDLE)))
        {
            pData.data.Remove(Direction.UP);
            Upoints = new Vector3[4]
            {
                new Vector3(upPiece.data[Direction.DOWN].values.startPos, puzzleSize,0),
                new Vector3(upPiece.data[Direction.DOWN].values.pos,upPiece.data[Direction.DOWN].side == Side.Outside ?  puzzleSize - upPiece.data[Direction.DOWN].values.offset : puzzleSize + upPiece.data[Direction.DOWN].values.offset,0),
                new Vector3(upPiece.data[Direction.DOWN].values.pos2,upPiece.data[Direction.DOWN].side == Side.Outside ? puzzleSize - upPiece.data[Direction.DOWN].values.offset2 : puzzleSize + upPiece.data[Direction.DOWN].values.offset2 ,0),
                new Vector3(upPiece.data[Direction.DOWN].values.endPos,puzzleSize,0)
            };
            pData.data.Add(Direction.UP, new Seed
            {
                isEdgeContainsKnob = true,
                side = upPiece.data[Direction.DOWN].side == Side.Inside ? Side.Outside : Side.Inside,
                values = new Values
                {
                    endPos = upPiece.data[Direction.DOWN].values.endPos,
                    offset = upPiece.data[Direction.DOWN].side == Side.Outside ? puzzleSize - upPiece.data[Direction.DOWN].values.offset : puzzleSize + upPiece.data[Direction.DOWN].values.offset,
                    offset2 = upPiece.data[Direction.DOWN].side == Side.Outside ? puzzleSize - upPiece.data[Direction.DOWN].values.offset2 : puzzleSize + upPiece.data[Direction.DOWN].values.offset2,
                    pos = upPiece.data[Direction.DOWN].values.pos,
                    pos2 = upPiece.data[Direction.DOWN].values.pos2,
                    startPos = upPiece.data[Direction.DOWN].values.startPos
                }
            });
        }

        #endregion

        #region Creating Vertices

        int edgeAmount = pieceEdgeData.Contains(PieceEdge.MIDDLE) ? 4 : 4 - pieceEdgeData.Count;
        int bezierDetail = 10;
        int verticesAmount = 4 + (bezierDetail + 1) * edgeAmount;
        Vector3[] vertices = new Vector3[verticesAmount];
       

        vertices[0] = new Vector3(0, 0, 0);
        int currentVerticesIndex = 1;
        if (!pieceEdgeData.Contains(PieceEdge.DOWN) || pieceEdgeData.Contains(PieceEdge.MIDDLE))
        {
            int nextVerticesIndex = currentVerticesIndex + (bezierDetail + 1);
            for (int i = currentVerticesIndex; i < nextVerticesIndex; i++)
            {
                vertices[i] = Bezier.GetPoint(Dpoints[0], Dpoints[1], Dpoints[2], Dpoints[3], (i - currentVerticesIndex) * (1 / (float)bezierDetail));
            }
            currentVerticesIndex = nextVerticesIndex;
        }
        vertices[currentVerticesIndex++] = new Vector3(puzzleSize, 0, 0);
        if (!pieceEdgeData.Contains(PieceEdge.RIGHT) || pieceEdgeData.Contains(PieceEdge.MIDDLE))
        {
            int nextVerticesIndex = currentVerticesIndex + (bezierDetail + 1);
            for (int i = currentVerticesIndex; i < nextVerticesIndex; i++)
            {
                vertices[i] = Bezier.GetPoint(Rpoints[0], Rpoints[1], Rpoints[2], Rpoints[3], (i - currentVerticesIndex) * (1 / (float)bezierDetail));
            }
            currentVerticesIndex = nextVerticesIndex;
        }
        vertices[currentVerticesIndex++] = new Vector3(puzzleSize, puzzleSize, 0);
        if (!pieceEdgeData.Contains(PieceEdge.UP) || pieceEdgeData.Contains(PieceEdge.MIDDLE))
        {
            int nextVerticesIndex = currentVerticesIndex + (bezierDetail + 1);
            for (int i = currentVerticesIndex; i < nextVerticesIndex; i++)
            {
                vertices[i] = Bezier.GetPoint(Upoints[3], Upoints[2], Upoints[1], Upoints[0], (i - currentVerticesIndex) * (1 / (float)bezierDetail));
            }
            currentVerticesIndex = nextVerticesIndex;
        }
        vertices[currentVerticesIndex++] = new Vector3(0, puzzleSize, 0);
        if (!pieceEdgeData.Contains(PieceEdge.LEFT) || pieceEdgeData.Contains(PieceEdge.MIDDLE))
        {
            int nextVerticesIndex = currentVerticesIndex + (bezierDetail + 1);
            for (int i = currentVerticesIndex; i < nextVerticesIndex; i++)
            {
                vertices[i] = Bezier.GetPoint(Lpoints[3], Lpoints[2], Lpoints[1], Lpoints[0], (i - currentVerticesIndex) * (1 / (float)bezierDetail));
            }
        }
        points = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            points[i] = new Vector2(vertices[i].x, vertices[i].y);
        }

        #endregion

        #region PolygonCollider Mesh Creator from Vertices

        mesh = polygon.PolygonCreate(points);
        //Debug.Log(pieceIndex + " --- Mesh --- " + mesh.vertexCount);

        //for (int i = 0; i < mesh.vertices.Length; i++)
        //{
        //    puzzleData.allPieces[pieceIndex].vertices.Add(mesh.vertices[i]);
        //}

        #endregion

        #region Creating Normals

        Vector3[] normals = new Vector3[mesh.vertexCount];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.forward;
        }


        mesh.normals = normals;

        #endregion

        #region Creating UVs
        Vector2[] uv = new Vector2[mesh.vertexCount];

        uv = Helper.CalculateUVsFromVertices(mesh.vertices,puzzleSize,pieceIndex, textureResolution.x, textureResolution.y);

        mesh.uv = uv;

        #endregion
        return mesh;
    }

    public enum PieceEdge { DOWN,UP,RIGHT,LEFT,MIDDLE};

    public static List<PieceEdge> FindPieceType(int pieceIndex, int xResolution, int yResolution)
    {
        List<PieceEdge> pieceEdgeData = new List<PieceEdge>();
        if (pieceIndex <= xResolution) // xRes = 20 yani 0,19 a kadar
        {
            //alt kenar
            pieceEdgeData.Add(PieceEdge.DOWN);
        }
        for (int i = 0; i < yResolution; i++)
        {
            if(pieceIndex == (i * xResolution) + xResolution) //(19 * 40) + 40
            {
                //sol kenar
                pieceEdgeData.Add(PieceEdge.LEFT);
            }
            if(pieceIndex == (i * xResolution + 1))
            {
                //sað kenar
                pieceEdgeData.Add(PieceEdge.RIGHT);
            }
        }
        for (int i = 0; i < xResolution; i++)
        {
            if(pieceIndex == (xResolution * (yResolution - 1)) + (i + 1))
            {
                //üst kenar
                pieceEdgeData.Add(PieceEdge.UP);
            }
        }

        //hiç bir özelliði yoksa ortadadýr.
        if(pieceEdgeData.Count == 0)
        {
            //ortadýr.
            pieceEdgeData.Add(PieceEdge.MIDDLE);
        }

        return pieceEdgeData;
    }
}
