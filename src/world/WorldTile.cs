
using System.Collections.Generic;
using System.Text;
using Godot;

public class WorldTile
{
    float Size = GameGlobals.TileWidth;
    public Godot.Vector3 Position;
   
    private List<Godot.Vector3> Vertices = new List<Godot.Vector3>();
    private List<Godot.Vector3> Normals = new List<Godot.Vector3>();
    private List<Godot.Vector2> Uvs = new List<Godot.Vector2>();

    public BlockType blockType = BlockType.Sand;

    public WorldTile(Godot.Vector3 pos, BlockType typeB)
    {
        this.Position = pos;
        this.blockType = typeB;
        
        this.SetVertices();
    }
    public List<Godot.Vector3> GetVertices()
    {
        return Vertices;
    }
    public List<Godot.Vector3> GetNormals()
    {
        return Normals;
    }
    public List<Godot.Vector2> GetUvs()
    {
        return Uvs;
    }
    

    private void AddFace(Godot.Vector3 normal)
    {
        this.Vertices.AddRange(CreateFaceVertices(normal));
        this.Normals.AddRange(GetVertexNormals(normal));
        this.Uvs.AddRange(GetVertexUvs(normal));
    }
    

    private void SetVertices()
    {
        this.AddFace(Godot.Vector3.Up);
        this.AddFace(Godot.Vector3.Down);
        this.AddFace(Godot.Vector3.Left);
        this.AddFace(Godot.Vector3.Right);
        this.AddFace(Godot.Vector3.Forward);
        this.AddFace(Godot.Vector3.Back);
    }
    private List<Godot.Vector2> GetVertexUvs(Godot.Vector3 normal)
    {
        Godot.Vector2 baseAppend = new Vector2(0, (int)this.blockType * GameGlobals.YAxisMove);

        Godot.Vector2 add = new Vector2();
        if (normal == Godot.Vector3.Up)
        {
            add = new Vector2(0,0);
        }
        else if (normal == Godot.Vector3.Down)
        {
            add = new Vector2((int)BlockSideUvInd.Bottom * GameGlobals.XAxisMove,0);
        }
        else if (normal == Godot.Vector3.Left || normal == Godot.Vector3.Right || normal == Godot.Vector3.Forward || normal == Godot.Vector3.Back)
        {
            add = new Vector2((int)BlockSideUvInd.Side * GameGlobals.XAxisMove,0);
        }

        List<Vector2> BaseUvs = [
            GameGlobals.baseBlockUvSector[0] + baseAppend + add,
            GameGlobals.baseBlockUvSector[1] + baseAppend + add,
            GameGlobals.baseBlockUvSector[2] + baseAppend + add,
            GameGlobals.baseBlockUvSector[3] + baseAppend + add
        ];

        List<Vector2> uvs = BaseUvs;
        
        return [
            uvs[0], uvs[1], uvs[3],
            uvs[1], uvs[2], uvs[3],
        ];
        
    }
    private List<Godot.Vector3> GetVertexNormals(Godot.Vector3 normal)
    {
        return [normal, normal, normal, normal, normal, normal]; // TODO XDDD
    }
    private List<Godot.Vector3> CreateFaceVertices(Godot.Vector3 direction)
    {
        List<Godot.Vector3> vertices = new List<Godot.Vector3>();

        if (direction == Godot.Vector3.Up)
        {
            vertices = new List<Godot.Vector3>
            {
                this.Position + (new Godot.Vector3(-0.5f,  0.5f, -0.5f) * this.Size),
                this.Position + (new Godot.Vector3( 0.5f,  0.5f, -0.5f) * this.Size),
                this.Position + (new Godot.Vector3( 0.5f,  0.5f,  0.5f) * this.Size),
                this.Position + (new Godot.Vector3(-0.5f,  0.5f,  0.5f) * this.Size)
            };
        }
        else if (direction == Godot.Vector3.Down)
        {
            vertices = new List<Godot.Vector3>
            {
                this.Position + (new Godot.Vector3(-0.5f, -0.5f,  0.5f) * this.Size),
                this.Position + (new Godot.Vector3( 0.5f, -0.5f,  0.5f) * this.Size),
                this.Position + (new Godot.Vector3( 0.5f, -0.5f, -0.5f) * this.Size),
                this.Position + (new Godot.Vector3(-0.5f, -0.5f, -0.5f) * this.Size)
            };
        }
        else if (direction == Godot.Vector3.Left)
        {
            vertices = new List<Godot.Vector3>
            {
                
                
                this.Position + (new Godot.Vector3(-0.5f,  0.5f, -0.5f) * this.Size),
                this.Position + (new Godot.Vector3(-0.5f,  0.5f,  0.5f) * this.Size),
                this.Position + (new Godot.Vector3(-0.5f, -0.5f,  0.5f) * this.Size),
                this.Position + (new Godot.Vector3(-0.5f, -0.5f, -0.5f) * this.Size),
            };
        }
        else if (direction == Godot.Vector3.Right)
        {
            vertices = new List<Godot.Vector3>
            {
                this.Position + (new Godot.Vector3( 0.5f,  0.5f,  0.5f) * this.Size),
                this.Position + (new Godot.Vector3( 0.5f,  0.5f, -0.5f) * this.Size),
                this.Position + (new Godot.Vector3( 0.5f, -0.5f, -0.5f) * this.Size),
                this.Position + (new Godot.Vector3( 0.5f, -0.5f,  0.5f) * this.Size),
                
                
            };
        }
        else if (direction == Godot.Vector3.Forward)
        {
            vertices = new List<Godot.Vector3>
            {
                
                this.Position + (new Godot.Vector3( 0.5f,  0.5f, -0.5f) * this.Size),
                this.Position + (new Godot.Vector3(-0.5f,  0.5f, -0.5f) * this.Size),
                this.Position + (new Godot.Vector3(-0.5f, -0.5f, -0.5f) * this.Size),
                this.Position + (new Godot.Vector3( 0.5f, -0.5f, -0.5f) * this.Size),
            };
        }
        else if (direction == Godot.Vector3.Back)
        {
            vertices = new List<Godot.Vector3>
            {
                this.Position + (new Godot.Vector3(-0.5f,  0.5f,  0.5f) * this.Size),
                this.Position + (new Godot.Vector3( 0.5f,  0.5f,  0.5f) * this.Size),
                this.Position + (new Godot.Vector3( 0.5f, -0.5f,  0.5f) * this.Size),
                this.Position + (new Godot.Vector3(-0.5f, -0.5f,  0.5f) * this.Size),
                
            };
        }

        return new List<Godot.Vector3>
        {
            vertices[0], vertices[1], vertices[3],
            vertices[1], vertices[2], vertices[3]
        };
    }
}



