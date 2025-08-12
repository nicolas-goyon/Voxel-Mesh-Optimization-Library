namespace VoxelMeshOptimizer.Core;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;

/// <summary>
/// Utility class to export a <see cref="Mesh"/> to the Wavefront OBJ format.
/// </summary>
public static class ObjExporter
{
    /// <summary>
    /// Exports the provided <paramref name="mesh"/> into an OBJ file at <paramref name="filePath"/>.
    /// </summary>
    /// <param name="mesh">Mesh to export.</param>
    /// <param name="filePath">Destination file path.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="mesh"/> or <paramref name="filePath"/> is null.</exception>
    public static void Export(Mesh mesh, string filePath)
    {
        if (mesh is null) throw new ArgumentNullException(nameof(mesh));
        if (filePath is null) throw new ArgumentNullException(nameof(filePath));

        var vertices = new List<Vector3>();
        var vertexIndices = new Dictionary<Vector3, int>();

        var sb = new StringBuilder();

        // Collect unique vertices and assign indices (1-based as per OBJ spec)
        foreach (var quad in mesh.Quads)
        {
            AddVertex(quad.Vertex0);
            AddVertex(quad.Vertex1);
            AddVertex(quad.Vertex2);
            AddVertex(quad.Vertex3);
        }

        // Write vertex positions
        foreach (var v in vertices)
        {
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "v {0} {1} {2}", v.X, v.Y, v.Z));
        }

        // Write faces using quad indices
        foreach (var quad in mesh.Quads)
        {
            int i0 = vertexIndices[quad.Vertex0];
            int i1 = vertexIndices[quad.Vertex1];
            int i2 = vertexIndices[quad.Vertex2];
            int i3 = vertexIndices[quad.Vertex3];
            sb.AppendLine($"f {i0} {i1} {i2} {i3}");
        }

        File.WriteAllText(filePath, sb.ToString());
        Console.WriteLine(filePath);

        void AddVertex(Vector3 v)
        {
            if (!vertexIndices.ContainsKey(v))
            {
                vertices.Add(v);
                vertexIndices[v] = vertices.Count; // 1-based
            }
        }
    }
}