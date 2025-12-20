
using System.Collections.Generic;
using System.Text;
using Godot;
public class WorldTile
{
    float Size;
    Godot.Vector3 Position;
    private List<Godot.Vector3> Vertices = new List<Godot.Vector3>();
    private List<Godot.Vector3> Normals = new List<Godot.Vector3>();

    public WorldTile(Godot.Vector3 pos, float tile_size)
    {
        this.Position = pos;
        this.Size = tile_size;
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
    private void AddFace(Godot.Vector3 normal)
    {
        this.Vertices.AddRange(CreateFaceVertices(normal));
        this.Normals.AddRange(GetVertexNormals(normal));
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
                this.Position + new Godot.Vector3(-0.5f,  0.5f, -0.5f) * this.Size,
                this.Position + new Godot.Vector3( 0.5f,  0.5f, -0.5f) * this.Size,
                this.Position + new Godot.Vector3( 0.5f,  0.5f,  0.5f) * this.Size,
                this.Position + new Godot.Vector3(-0.5f,  0.5f,  0.5f) * this.Size
            };
        }
        else if (direction == Godot.Vector3.Down)
        {
            vertices = new List<Godot.Vector3>
            {
                this.Position + new Godot.Vector3(-0.5f, -0.5f,  0.5f) * this.Size,
                this.Position + new Godot.Vector3( 0.5f, -0.5f,  0.5f) * this.Size,
                this.Position + new Godot.Vector3( 0.5f, -0.5f, -0.5f) * this.Size,
                this.Position + new Godot.Vector3(-0.5f, -0.5f, -0.5f) * this.Size
            };
        }
        else if (direction == Godot.Vector3.Left)
        {
            vertices = new List<Godot.Vector3>
            {
                this.Position + new Godot.Vector3(-0.5f, -0.5f, -0.5f) * this.Size,
                this.Position + new Godot.Vector3(-0.5f,  0.5f, -0.5f) * this.Size,
                this.Position + new Godot.Vector3(-0.5f,  0.5f,  0.5f) * this.Size,
                this.Position + new Godot.Vector3(-0.5f, -0.5f,  0.5f) * this.Size
            };
        }
        else if (direction == Godot.Vector3.Right)
        {
            vertices = new List<Godot.Vector3>
            {
                this.Position + new Godot.Vector3( 0.5f, -0.5f,  0.5f) * this.Size,
                this.Position + new Godot.Vector3( 0.5f,  0.5f,  0.5f) * this.Size,
                this.Position + new Godot.Vector3( 0.5f,  0.5f, -0.5f) * this.Size,
                this.Position + new Godot.Vector3( 0.5f, -0.5f, -0.5f) * this.Size
            };
        }
        else if (direction == Godot.Vector3.Forward)
        {
            vertices = new List<Godot.Vector3>
            {
                this.Position + new Godot.Vector3(-0.5f, -0.5f, -0.5f) * this.Size,
                this.Position + new Godot.Vector3( 0.5f, -0.5f, -0.5f) * this.Size,
                this.Position + new Godot.Vector3( 0.5f,  0.5f, -0.5f) * this.Size,
                this.Position + new Godot.Vector3(-0.5f,  0.5f, -0.5f) * this.Size
            };
        }
        else if (direction == Godot.Vector3.Back)
        {
            vertices = new List<Godot.Vector3>
            {
                this.Position + new Godot.Vector3(-0.5f,  0.5f,  0.5f) * this.Size,
                this.Position + new Godot.Vector3( 0.5f,  0.5f,  0.5f) * this.Size,
                this.Position + new Godot.Vector3( 0.5f, -0.5f,  0.5f) * this.Size,
                this.Position + new Godot.Vector3(-0.5f, -0.5f,  0.5f) * this.Size
            };
        }

        return new List<Godot.Vector3>
        {
            vertices[0], vertices[1], vertices[3],
            vertices[1], vertices[2], vertices[3]
        };
    }
}



