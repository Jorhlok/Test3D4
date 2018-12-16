using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Test3D4
{
    struct SaturnaliaVertex : IVertexType
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

        VertexDeclaration IVertexType.VertexDeclaration { get => VertexDeclaration; }
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
        public static int colorTableLen = 32768;
        public Color[] colorTable = null;
        Texture2D colorTableTex = null;
        public int cluts = 0;
        public Vector4[] clut = null;
        Texture2D clutTex = null;
        short BatchSize;
        int t;
        int v;

        public SaturnaliaBatch(GraphicsDevice gdev, Effect saturnEffect, short bSize = 4096, int w = 320, int h = 240, Matrix? proj = null, Vector3? screendoors = null, SurfaceFormat format = SurfaceFormat.HalfVector4)
        {
            this.gdev = gdev;
            fx = saturnEffect;
            this.format = format;
            Resize(w, h);
            Projection(proj);
            CullMode();
            DepthEnable();
            SetScreendoors(screendoors);
            ChangeBatchSize(bSize);
        }

        public void Resize(int w, int h)
        {
            if (buf != null) buf.Dispose();
            width = w;
            height = h;
            buf = new RenderTarget2D(gdev, w, h, false, format, DepthFormat.Depth24);
        }

        public void ChangeBatchSize(short bSize)
        {
            BatchSize = bSize;
            verts = null;
            indecies = null;
            //quadBuf = null;
            t = v = 0;
            //if (quadBufTex != null) quadBufTex.Dispose();
            //quadBufTex = new Texture2D(gdev, 8, BatchSize, false, SurfaceFormat.Vector4);
            verts = new SaturnaliaVertex[BatchSize * 4]; //3 or 4 per quad
            indecies = new short[BatchSize * 6]; //3 or 6 per quad
            //quadBuf = new Vector4[BatchSize * 8]; //8 per quad
            for (int i = 0; i < BatchSize; i++)
            {
                for (int j = 0; j < 4; j++) verts[i * 4 + j] = new SaturnaliaVertex();
                //for (int j = 0; j < 8; j++) quadBuf[i * 8 + j] = new Vector4();
            }
        }

        public void Dispose()
        {
            //if (quadBufTex != null) quadBufTex.Dispose();
            //quadBufTex = null;
            WipeColorTable();
            WipeCLUT();
            verts = null;
            indecies = null;
            BatchSize = 0;
        }

        public void DepthEnable(bool b = true)
        {
            gdev.DepthStencilState = b ? DepthStencilState.Default : DepthStencilState.None;
        }

        public void BlendEnable(bool b = true)
        {
            gdev.BlendState = b ? BlendState.NonPremultiplied : BlendState.Opaque;
        }

        public void Projection(Matrix? m = null)
        {
            if (m == null) m = Matrix.CreateOrthographicOffCenter(0, width, height, 0, 1f / 1024, 1024);
            fx.Parameters["Projection"].SetValue(m.Value);
        }

        //The dithering pseudo half transparency called "mesh" in the VDP1 manual. (Alpha A, Alpha B, size)
        public void SetScreendoors(Vector3? screendoors = null)
        {
            if (screendoors == null) screendoors = new Vector3(1,0,1);
            fx.Parameters["Screendoors"].SetValue((Vector3)screendoors);
        }

        public void CullMode(int i = 0)
        {
            switch (i)
            {
                case 1:
                    gdev.RasterizerState = RasterizerState.CullCounterClockwise;
                    break;
                case 2:
                    gdev.RasterizerState = RasterizerState.CullClockwise;
                    break;
                default:
                    gdev.RasterizerState = RasterizerState.CullNone;
                    break;
            }
        }

        public void WriteColorTable()
        {
            if (colorTable == null)
            {
                colorTable = new Color[colorTableLen];
                for (int i = 0; i < colorTableLen; i++) colorTable[i] = new Color();
            }
            if (colorTableTex == null)
            {
                colorTableTex = new Texture2D(gdev, 1, colorTableLen, false, SurfaceFormat.Color);
            }
            colorTableTex.SetData<Color>(colorTable);
            fx.Parameters["ColorTable"].SetValue(colorTableTex);
        }

        public void WipeColorTable()
        {
            colorTable = null;
            if (colorTableTex != null) colorTableTex.Dispose();
            colorTableTex = null;
            fx.Parameters["ColorTable"].SetValue(colorTableTex);
        }
        
        public void WriteCLUT()
        {
            if (cluts <= 0) WipeCLUT();
            else
            {
                if (clut == null)
                {
                    clut = new Vector4[cluts*16];
                    for (int i = 0; i < cluts*16; i++) clut[i] = new Vector4();
                }
                if (clutTex == null)
                {
                    clutTex = new Texture2D(gdev, 16, cluts, false, SurfaceFormat.Vector4);
                }
                clutTex.SetData<Vector4>(clut);
            }
        }

        public void WipeCLUT()
        {
            cluts = 0;
            clut = null;
            if (clutTex != null) clutTex.Dispose();
            colorTableTex = null;
            fx.Parameters["CLUTLen"].SetValue(cluts);
            fx.Parameters["CLUT"].SetValue(clutTex);
        }

        public void Begin()
        {
            gdev.SetRenderTarget(buf);
            gdev.Clear(Color.Transparent);
            Clear();
        }

        public void Clear()
        {
            t = v = 0;
        }

        public void Flush()
        {
            //for (int i=0; i<v; i++)
            //{
            //    System.Console.WriteLine(verts[i]);
            //}
            //System.Console.WriteLine();
            //quadBufTex.SetData<Vector4>(quadBuf);//,0,q);
            //fx.Parameters["Quads"].SetValue(quadBufTex);
            //fx.CurrentTechnique.Passes[3].Apply();
            gdev.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, verts, 0, v, indecies, 0, t);
            Clear();
        }

        public void End()
        {
            Flush();
            gdev.SetRenderTarget(null);
        }

        public Vector3 AvgVertex3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return new Vector3((p0.X + p1.X + p2.X + p3.X) / 4, (p0.Y + p1.Y + p2.Y + p3.Y) / 4, (p0.Z + p1.Z + p2.Z + p3.Z) / 4);
        }

        public bool ptInTriangle2D(Vector3 p, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            //barycentric coordinates
            //https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
            var dX = p.X - p2.X;
            var dY = p.Y - p2.Y;
            var dX21 = p2.X - p1.X;
            var dY12 = p1.Y - p2.Y;
            var D = dY12 * (p0.X - p2.X) + dX21 * (p0.Y - p2.Y);
            var s = dY12 * dX + dX21 * dY;
            var t = (p2.Y - p0.Y) * dX + (p0.X - p2.X) * dY;
            if (D < 0) return s <= 0 && t <= 0 && s + t >= D;
            return s >= 0 && t >= 0 && s + t <= D;
        }
        
    }
}
