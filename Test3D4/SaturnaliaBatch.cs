using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Test3D4
{
    struct SaturnaliaVertex
    {
        /*
        modes:
        flat/tex/clut		+ 4*mode (0 flat, 1 tex, 2 tex clut)
        tri/quad            + 2 for quad
        screendoors         + 1

        minimum maximum attributes
        gl 2.1  16 min
        gles 2  8 min
        gles 3  16 min


        even smaller:
        vec3    vertex
        vec4    xy0, xy1
        vec4    xy2, xy3
        vec4    uv0 (flat color for untextured)
        vec4    uv2
        vec4    gouraud (each packed into 15 bits or up to 24 bits safely, rgba6666)
        vec4    modes (mode, half lumi/shading?, color bank, CLUT index)

        7 attr (compatible with gles 2 and webgl 1?)
        */
        private Vector3 position;
        private Vector4 xy01;
        private Vector4 xy23;
        private Vector4 uv01; //flat color for untextured
        private Vector4 uv23;
        private Vector4 gouraud;
        private Vector4 modes; //mode, color bank, clut, color calc

        public SaturnaliaVertex(Vector3 position, Vector4 xy01, Vector4 xy23, Vector4 uv01, Vector4 uv23, Vector4 gouraud, Vector4 modes)
        {
            this.position = position;
            this.xy01 = xy01;
            this.xy23 = xy23;
            this.uv01 = uv01;
            this.uv23 = uv23;
            this.gouraud = gouraud;
            this.modes = modes;
        }

        public SaturnaliaVertex(Vector3 position, Vector2 xy0, Vector2 xy1, Vector2 xy2, Vector2 xy3, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector4 gouraud, Vector4 modes)
        {
            this.position = position;
            this.xy01 = new Vector4(xy0.X, xy0.Y, xy1.X, xy1.Y);
            this.xy23 = new Vector4(xy2.X, xy2.Y, xy3.X, xy3.Y);
            this.uv01 = new Vector4(xy0.X, xy0.Y, xy1.X, xy1.Y);
            this.uv23 = new Vector4(xy2.X, xy2.Y, xy3.X, xy3.Y);
            this.gouraud = gouraud;
            this.modes = modes;
        }

        public SaturnaliaVertex(Vector3 position, Vector2 xy0, Vector2 xy1, Vector2 xy2, Vector2 xy3, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3, int gouraud0, int gouraud1, int gouraud2, int gouraud3, Vector4 modes)
        {
            this.position = position;
            this.xy01 = new Vector4(xy0.X, xy0.Y, xy1.X, xy1.Y);
            this.xy23 = new Vector4(xy2.X, xy2.Y, xy3.X, xy3.Y);
            this.uv01 = new Vector4(xy0.X, xy0.Y, xy1.X, xy1.Y);
            this.uv23 = new Vector4(xy2.X, xy2.Y, xy3.X, xy3.Y);
            this.gouraud = new Vector4(gouraud0, gouraud1, gouraud2, gouraud3);
            this.modes = modes;
        }

        public SaturnaliaVertex(Vector3 position, Vector2 xy0, Vector2 xy1, Vector2 xy2, Vector2 xy3, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3, int gouraud0, int gouraud1, int gouraud2, int gouraud3, int mode, int colorBank, int clut, int colcalc)
        {
            this.position = position;
            this.xy01 = new Vector4(xy0.X, xy0.Y, xy1.X, xy1.Y);
            this.xy23 = new Vector4(xy2.X, xy2.Y, xy3.X, xy3.Y);
            this.uv01 = new Vector4(xy0.X, xy0.Y, xy1.X, xy1.Y);
            this.uv23 = new Vector4(xy2.X, xy2.Y, xy3.X, xy3.Y);
            this.gouraud = new Vector4(gouraud0, gouraud1, gouraud2, gouraud3);
            this.modes = new Vector4(mode, colorBank, clut, colcalc);
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * (3 + 4), VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(sizeof(float) * (3 + 4*2), VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(sizeof(float) * (3 + 4*3), VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
            new VertexElement(sizeof(float) * (3 + 4*4), VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4),
            new VertexElement(sizeof(float) * (3 + 4*5), VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 5)
        );
    }

    class SaturnaliaBatch
    {
        public int width = 320;
        public int height = 240;
        SurfaceFormat format;
        public RenderTarget2D buf;
        public GraphicsDevice gdev;
        public Effect fx;
        public static Color noGouraudCol = new Color(0.5f, 0.5f, 0.5f, 1f);
        public static int noGouraud = 32*64*64*64+32*64*64+32*64+63;
        Vector3ClockCompare clocker = new Vector3ClockCompare();
        SaturnaliaVertex[] verts;
        short[] indecies;
        Vector4[] colorTable;
        public Texture2D colorTableTex = null;
        Vector4[] clut;
        public Texture2D clutTex = null;
        short BatchSize;
        int t;
        int v;
    }
}
