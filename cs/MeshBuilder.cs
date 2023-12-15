// may be not pretty fast, but can work for most cases, where mesh size is dynamic
// inspired from rd-132211 Tesselator.java class
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SomeRaylibStuff;

public class MeshBuilder
{
    private List<Vector2> _texcoord = new List<Vector2>();
    private List<Color> _color = new List<Color>();
    private List<Vector3> _vertex = new List<Vector3>();

    private float _u, _v;
    private byte _r, _g, _b;
    private bool _hasTex;
    private bool _hasCol;

    public void Tex(float u, float v)
    {
        _hasTex = true;
        _u = u;
        _v = v;
    }

    public void Color(byte r, byte g, byte b)
    {
        _hasCol = true;
        _r = r;
        _g = g;
        _b = b;
    }

    public void Vertex(float x, float y, float z)
    {
        if (_hasTex) _texcoord.Add(new Vector2(_u, _v));
        if (_hasCol) _color.Add(new Color(_r, _g, _b, (byte)255));
        _vertex.Add(new Vector3(x, y, z));
    }

    public Mesh Flush()
    {
        Mesh m = new Mesh
        {
            VertexCount = _vertex.Count,
            TriangleCount = _vertex.Count / 3
        };
        
        m.AllocTexCoords();
        m.AllocColors();
        m.AllocVertices();
        
        if (_hasTex) CollectionsMarshal.AsSpan(_texcoord).CopyTo(m.TexCoordsAs<Vector2>());
        if (_hasCol) CollectionsMarshal.AsSpan(_color).CopyTo(m.ColorsAs<Color>());
        CollectionsMarshal.AsSpan(_vertex).CopyTo(m.VerticesAs<Vector3>());
        
        UploadMesh(ref m, false);

        return m;
    }

    public void Clear()
    {
        _texcoord.Clear();
        _color.Clear();
        _vertex.Clear();
    }

    public void Init()
    {
        (_u, _v) = (0.0f, 0.0f);
        (_r, _g, _b) = (0, 0, 0);
        (_hasTex, _hasCol) = (false, false);
        Clear();
    }
}