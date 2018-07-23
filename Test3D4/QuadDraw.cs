using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Test3D4
{
    class QuadDraw
    {
        public int width = 320;
        public int height = 240;
        public int sdrScale = 1; //screen door effect
        SurfaceFormat format;
        public RenderTarget2D buf;
        public GraphicsDevice gdev;
        Effect fx;
        Matrix projection;
        public static Color noGouraud = new Color(0.5f,0.5f,0.5f,1f);
        public static short[] quickIdx = new short[12];

        public QuadDraw(GraphicsDevice gdev, Effect saturnEffect, int w = 320, int h = 240, int sdrScale = 1, SurfaceFormat format = SurfaceFormat.Color)
        {
            quickIdx[0] = 0;
            quickIdx[1] = 1;
            quickIdx[2] = 4;

            quickIdx[3] = 1;
            quickIdx[4] = 2;
            quickIdx[5] = 4;

            quickIdx[6] = 2;
            quickIdx[7] = 3;
            quickIdx[8] = 4;

            quickIdx[9] = 3;
            quickIdx[10] = 0;
            quickIdx[11] = 4;

            this.sdrScale = sdrScale;
            this.gdev = gdev;
            fx = saturnEffect;
            this.format = format;
            Resize(w,h);
        }

        public void Resize(int w, int h)
        {
            if (buf != null) buf.Dispose();
            width = w;
            height = h;
            buf = new RenderTarget2D(gdev, w, h, false, format, DepthFormat.Depth24);
        }

        public void DepthEnable(bool b)
        {
            gdev.DepthStencilState = b ? DepthStencilState.DepthRead : DepthStencilState.None;
        }

        public void BlendEnable(bool b)
        {
            gdev.BlendState = b ? BlendState.NonPremultiplied : BlendState.Opaque;
        }

        public void GPUProjection(bool b)
        {

        }

        public void DrawQuadQuick(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Color c0, Color c1, Color c2, Color c3)
        {
            var verts = new VertexPositionColor[5];
            verts[0] = new VertexPositionColor(p0, c0);
            verts[1] = new VertexPositionColor(p1, c1);
            verts[2] = new VertexPositionColor(p2, c2);
            verts[3] = new VertexPositionColor(p3, c3);
            verts[4] = new VertexPositionColor(AvgVertex3(p0,p1,p2,p3), AvgCol(c0,c1,c2,c3));

            fx.CurrentTechnique.Passes[1].Apply();
            gdev.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, verts, 0, 5, quickIdx, 0, 4);
        }

        public void DrawSpriteQuick(Texture2D Tex, Rectangle area, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int rt90 = 0, int flips = 0)
        {
            DrawSpriteQuick(Tex, area, p0, p1, p2, p3, noGouraud, noGouraud, noGouraud, noGouraud, rt90, flips);
        }
        
        public void DrawSpriteQuick(Texture2D Tex, Rectangle area, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Color c0, Color c1, Color c2, Color c3, int rt90 = 0, int flips = 0)
        {
            float w = Tex.Width;
            float h = Tex.Height;
            var uv = new Vector2[5];
            uv[0] = new Vector2(area.X / w, area.Y / h);
            uv[1] = new Vector2((area.X+area.Width) / w, area.Y / h);
            uv[2] = new Vector2((area.X + area.Width) / w, (area.Y + area.Height) / h);
            uv[3] = new Vector2(area.X / w, (area.Y + area.Height) / h);
            if ((flips & 1) != 0)
            { //flip horizontally
                w = uv[0].X;
                h = uv[3].X;
                uv[0].X = uv[1].X;
                uv[1].X = w;
                uv[3].X = uv[2].X;
                uv[2].X = h;
            }
            if ((flips & 2) != 0)
            { //flip vertically
                w = uv[0].Y;
                h = uv[1].Y;
                uv[0].Y = uv[3].Y;
                uv[3].Y = w;
                uv[1].Y = uv[2].Y;
                uv[2].Y = h;
            }
            w = area.Width;
            h = area.Height;
            for (var i=0; i<(rt90&3); i++) //& 3 or % 4 should work
            { //rotate clockwise
                var tmp = w;
                w = h;
                h = tmp;
                uv[4] = uv[3];
                uv[3] = uv[2];
                uv[2] = uv[1];
                uv[1] = uv[0];
                uv[0] = uv[4];
            }
            var verts = new VertexPositionColorTexture[5];
            verts[0] = new VertexPositionColorTexture(p0, c0, uv[0]);
            verts[1] = new VertexPositionColorTexture(p1, c1, uv[1]);
            verts[2] = new VertexPositionColorTexture(p2, c2, uv[2]);
            verts[3] = new VertexPositionColorTexture(p3, c3, uv[3]);
            verts[4] = new VertexPositionColorTexture(AvgVertex3(p0,p1,p2,p3), AvgCol(c0,c1,c2,c3), AvgVertex2(uv[0], uv[1], uv[2], uv[3]));

            fx.Parameters["Tex"].SetValue(Tex);
            fx.CurrentTechnique.Passes[0].Apply();
            gdev.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, verts, 0, 5, quickIdx, 0, 4);
        }

        public Vector2 AvgVertex2(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return new Vector2((p0.X + p1.X + p2.X + p3.X) / 4, (p0.Y + p1.Y + p2.Y + p3.Y) / 4);
        }

        public Vector3 AvgVertex3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return new Vector3((p0.X + p1.X + p2.X + p3.X) / 4, (p0.Y + p1.Y + p2.Y + p3.Y) / 4, (p0.Z + p1.Z + p2.Z + p3.Z) / 4);
        }

        public Color AvgCol(Color c0, Color c1, Color c2, Color c3)
        {
            return new Color((c0.R + c1.R + c2.R + c3.R) / 4, (c0.G + c1.G + c2.G + c3.G) / 4, (c0.B + c1.B + c2.B + c3.B) / 4, (c0.A + c1.A + c2.A + c3.A) / 4);
        }

    }
}
