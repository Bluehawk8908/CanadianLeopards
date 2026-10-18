using System.IO;
using UnityEngine;
using MelonLoader;

namespace CanadianLeopards
{
    public class Utilities
    {
        public static bool mute_logger;
        public static Texture2D FetchTex(int x, int y, string path)
        {
            Texture2D temp = new Texture2D(x, y);
            try
            {
                byte[] data = File.ReadAllBytes(path);
                temp.LoadImage(data);
            }
            catch (FileNotFoundException e) { MelonLogger.Error(e); }
            return temp;
        }

        public static Texture2D FetchTex(int x, int y, string path, bool linear)
        {
            Texture2D temp = new Texture2D(x, y, TextureFormat.DXT5, true, linear);
            try
            {
                byte[] data = File.ReadAllBytes(path);
                temp.LoadImage(data);
            }
            catch (FileNotFoundException e) { MelonLogger.Error(e); }
            return temp;
        }

        public static void NewQuad(GameObject go, Material mat, Texture2D tex)
        {
            MeshFilter filter = go.AddComponent<MeshFilter>();
            MeshRenderer render = go.AddComponent<MeshRenderer>();
            filter.mesh = new Mesh
            {
                vertices = new Vector3[] { new Vector3(1f, 0, 1f), new Vector3(1f, 0, -1f), new Vector3(-1f, 0, 1f), new Vector3(-1f, 0, -1f) },
                uv = new Vector2[] { new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0) },
                triangles = new int[] { 0, 1, 2, 2, 1, 3 }
            };
            filter.mesh.RecalculateNormals();
            render.material = mat;
            if (mat.shader.name == "ghpc_roundel") { render.material.SetTexture("_colour", tex); }
            else render.material.mainTexture = tex;
        }
        public static void Log(string text)
        {
            if (!mute_logger) { MelonLogger.Msg(text); }
        }
    }
}
