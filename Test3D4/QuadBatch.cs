using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Test3D4
{
    class QuadBatch
    {
        public int width = 320;
        public int height = 240;
        SurfaceFormat format;
        public RenderTarget2D buf;
        public GraphicsDevice gdev;
        public Effect fx;
        public static Color noGouraud = new Color(0.5f, 0.5f, 0.5f, 1f);
        Vector3ClockCompare clocker = new Vector3ClockCompare();
        VertexPositionTexture[] verts;
        short[] indecies;
        Vector4[] quadBuf;
        public Texture2D quadBufTex = null;
        short BatchSize;
        int q;
        int t;
        int v;

        public QuadBatch(GraphicsDevice gdev, Effect saturnEffect, short bSize = 8000, int w = 320, int h = 240, Matrix? proj = null, int sdrScale = 1, SurfaceFormat format = SurfaceFormat.HalfVector4)
        {
            this.gdev = gdev;
            fx = saturnEffect;
            this.format = format;
            Resize(w, h);
            Projection(proj);
            CullMode();
            DepthEnable();
            ScreenDoorScale(sdrScale);
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
            quadBuf = null;
            q = t = v = 0;
            if (quadBufTex != null) quadBufTex.Dispose();
            quadBufTex = new Texture2D(gdev, 8, BatchSize, false, SurfaceFormat.Vector4);
            verts = new VertexPositionTexture[BatchSize * 4]; //3 or 4 per quad
            indecies = new short[BatchSize * 6]; //3 or 6 per quad
            quadBuf = new Vector4[BatchSize * 8]; //8 per quad
            for (int i=0; i<BatchSize; i++)
            {
                for (int j = 0; j < 4; j++) verts[i * 4 + j] = new VertexPositionTexture();
                for (int j = 0; j < 8; j++) quadBuf[i * 8 + j] = new Vector4();
            }
        }

        public void Dispose()
        {
            if (quadBufTex != null) quadBufTex.Dispose();
            quadBufTex = null;
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

        public void ScreenDoorScale(int i = 1)
        {
            fx.Parameters["ScreenDoorScale"].SetValue(i);
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

        public void MagicColEnable(bool b = true)
        {
            fx.Parameters["MagicColEnable"].SetValue(b);
        }

        public void MagicCol(Color? c = null)
        {
            if (c == null) MagicColEnable(false);
            else
            {
                MagicColEnable(true);
                fx.Parameters["MagicCol"].SetValue(c.Value.ToVector4());
            }
        }

        public void Begin()
        {
            gdev.SetRenderTarget(buf);
            gdev.Clear(Color.Transparent);
            Clear();
        }

        public void Clear()
        {
            q = t = v = 0;
        }

        public void Flush()
        {
            //for (int i=0; i<v; i++)
            //{
            //    System.Console.WriteLine(verts[i]);
            //}
            //System.Console.WriteLine();
            quadBufTex.SetData<Vector4>(quadBuf);//,0,q);
            fx.Parameters["Quads"].SetValue(quadBufTex);
            fx.CurrentTechnique.Passes[3].Apply();
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

        public void DrawSprite(Texture2D Tex, Rectangle area, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int rt90 = 0, int flips = 0)
        {
            DrawSprite(Tex, area, p0, p1, p2, p3, noGouraud, noGouraud, noGouraud, noGouraud, rt90, flips);
        }

        /* no depth 8000 buf 1M quad
         + 3495.1999
         + 3487.1994
         + 3428.1961
         + 3482.1992
         + 3469.1984
         + 3461.198
         + 3441.1968
         + 3487.1995
         + 3441.1969
         + 3490.1996
         * avg 3468.29838
         * qps 288325.827375902
         * 60fps 4805.43045626503
         * 30fps 9610.86091253006
         * 
         * depth 8000 buf 1M quad
         + 1688.0965
         + 1686.0964
         + 1691.0967
         + 1744.0998
         + 1682.0962
         + 1665.0952
         + 1699.0972
         + 1682.0963
         + 1685.0963
         + 1710.0978
         * avg 1693.29684
         * qps 590563.908452106
         * 60fps 9842.7318075351
         * 30fps 19685.4636150702
         * 
         * depth 2048 buf 1M quad
         + 2060.1178
         + 2058.1177
         + 2043.1169
         + 2053.1175
         + 2031.1161
         + 2032.1162
         + 2042.1168
         + 2043.1169
         + 2071.1185
         + 2021.1156
         * avg 2045.517
         * qps 488873.961937251
         * 60fps 8147.89936562085
         * 30fps 16295.7987312417
         * 
         * depth 4096 buf 4096 quad
         + 9.0006
         + 8.0004
         + 9.0005
         + 7.0004
         + 8.0005
         + 7.0004
         + 13.0008
         + 13.0007
         + 8.0004
         + 12.0007
         * avg 9.40054
         * qps 435719.650147757
         * 60fps 7261.99416912929
         * 30fps 14523.9883382586
         */

        public void DrawSprite(Texture2D Tex, Rectangle area, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Color c0, Color c1, Color c2, Color c3, int rt90 = 0, int flips = 0)
        {
            if (q == BatchSize) Flush();
            float w = Tex.Width;
            float h = Tex.Height;
            var uv = new Vector2[5];
            uv[0] = new Vector2(area.X / w, area.Y / h);
            uv[1] = new Vector2((area.X + area.Width) / w, area.Y / h);
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
            for (var i = 0; i < (rt90 & 3); i++) //& 3 or % 4 should work
            { //rotate clockwise
                uv[4] = uv[3];
                uv[3] = uv[2];
                uv[2] = uv[1];
                uv[1] = uv[0];
                uv[0] = uv[4];
            }

            //fx.Parameters["VertexA"].SetValue(p0);
            //fx.Parameters["VertexB"].SetValue(p1);
            //fx.Parameters["VertexC"].SetValue(p2);
            //fx.Parameters["VertexD"].SetValue(p3);
            quadBuf[q * 8].X = p0.X;
            quadBuf[q * 8].Y = p0.Y;
            quadBuf[q * 8].Z = p1.X;
            quadBuf[q * 8].W = p1.Y;
            quadBuf[q * 8 + 1].X = p2.X;
            quadBuf[q * 8 + 1].Y = p2.Y;
            quadBuf[q * 8 + 1].Z = p3.X;
            quadBuf[q * 8 + 1].W = p3.Y;

            //fx.Parameters["UVA"].SetValue(uv[0]);
            //fx.Parameters["UVB"].SetValue(uv[1]);
            //fx.Parameters["UVC"].SetValue(uv[2]);
            //fx.Parameters["UVD"].SetValue(uv[3]);
            quadBuf[q * 8 + 2].X = uv[0].X;
            quadBuf[q * 8 + 2].Y = uv[0].Y;
            quadBuf[q * 8 + 2].Z = uv[1].X;
            quadBuf[q * 8 + 2].W = uv[1].Y;
            quadBuf[q * 8 + 3].X = uv[2].X;
            quadBuf[q * 8 + 3].Y = uv[2].Y;
            quadBuf[q * 8 + 3].Z = uv[3].X;
            quadBuf[q * 8 + 3].W = uv[3].Y;

            //fx.Parameters["GouraudA"].SetValue(c0.ToVector4());
            //fx.Parameters["GouraudB"].SetValue(c1.ToVector4());
            //fx.Parameters["GouraudC"].SetValue(c2.ToVector4());
            //fx.Parameters["GouraudD"].SetValue(c3.ToVector4());
            var col = c0.ToVector4();
            quadBuf[q * 8 + 4].X = col.X;
            quadBuf[q * 8 + 4].Y = col.Y;
            quadBuf[q * 8 + 4].Z = col.Z;
            quadBuf[q * 8 + 4].W = col.W;
            col = c1.ToVector4();
            quadBuf[q * 8 + 5].X = col.X;
            quadBuf[q * 8 + 5].Y = col.Y;
            quadBuf[q * 8 + 5].Z = col.Z;
            quadBuf[q * 8 + 5].W = col.W;
            col = c2.ToVector4();
            quadBuf[q * 8 + 6].X = col.X;
            quadBuf[q * 8 + 6].Y = col.Y;
            quadBuf[q * 8 + 6].Z = col.Z;
            quadBuf[q * 8 + 6].W = col.W;
            col = c3.ToVector4();
            quadBuf[q * 8 + 7].X = col.X;
            quadBuf[q * 8 + 7].Y = col.Y;
            quadBuf[q * 8 + 7].Z = col.Z;
            quadBuf[q * 8 + 7].W = col.W;

            //fx.Parameters["Tex"].SetValue(Tex);
            //fx.CurrentTechnique.Passes[2].Apply();


            var pts = new Vector3[4];
            pts[0] = p0;
            pts[1] = p1;
            pts[2] = p2;
            pts[3] = p3;


            var center = AvgVertex3(p0, p1, p2, p3);
            clocker.center = center;
            System.Array.Sort(pts, clocker); //untwist

            var quadv = ((float)q) / BatchSize + 1f / (BatchSize * 2);
            q++;

            for (int i = 0; i < 4; i++)
            { //check for concave and draw the encompassing triangle
                var v0 = pts[i];
                var v1 = pts[(1 + i) % 4];
                var v2 = pts[(2 + i) % 4];
                var v3 = pts[(3 + i) % 4];
                if (ptInTriangle2D(v3, v0, v1, v2))
                {
                    //v3 is inside the triangle made by the others
                    verts[v].Position.X = v0.X;
                    verts[v].Position.Y = v0.Y;
                    verts[v].Position.Z = v0.Z;
                    verts[v].TextureCoordinate.X = 0;
                    verts[v].TextureCoordinate.Y = quadv;
                    verts[v + 1].Position.X = v1.X;
                    verts[v + 1].Position.Y = v1.Y;
                    verts[v + 1].Position.Z = v1.Z;
                    verts[v + 1].TextureCoordinate.X = 0;
                    verts[v + 1].TextureCoordinate.Y = quadv;
                    verts[v + 2].Position.X = v2.X;
                    verts[v + 2].Position.Y = v2.Y;
                    verts[v + 2].Position.Z = v2.Z;
                    verts[v + 2].TextureCoordinate.X = 0;
                    verts[v + 2].TextureCoordinate.Y = quadv;
                    indecies[t * 3] = (short)v;
                    indecies[t * 3 + 1] = (short)(v + 1);
                    indecies[t * 3 + 2] = (short)(v + 2);
                    v += 3;
                    t++;
                    return; //my work here is done
                }
            }
            
            verts[v].Position.X = pts[0].X;
            verts[v].Position.Y = pts[0].Y;
            verts[v].Position.Z = pts[0].Z;
            verts[v].TextureCoordinate.X = 0;
            verts[v].TextureCoordinate.Y = quadv;
            verts[v + 1].Position.X = pts[1].X;
            verts[v + 1].Position.Y = pts[1].Y;
            verts[v + 1].Position.Z = pts[1].Z;
            verts[v + 1].TextureCoordinate.X = 0;
            verts[v + 1].TextureCoordinate.Y = quadv;
            verts[v + 2].Position.X = pts[2].X;
            verts[v + 2].Position.Y = pts[2].Y;
            verts[v + 2].Position.Z = pts[2].Z;
            verts[v + 2].TextureCoordinate.X = 0;
            verts[v + 2].TextureCoordinate.Y = quadv;
            verts[v + 3].Position.X = pts[3].X;
            verts[v + 3].Position.Y = pts[3].Y;
            verts[v + 3].Position.Z = pts[3].Z;
            verts[v + 3].TextureCoordinate.X = 0;
            verts[v + 3].TextureCoordinate.Y = quadv;
            indecies[t * 3] = (short)v;
            indecies[t * 3 + 1] = (short)(v + 1);
            indecies[t * 3 + 2] = (short)(v + 2);
            indecies[t * 3 + 3] = (short)v;
            indecies[t * 3 + 4] = (short)(v + 2);
            indecies[t * 3 + 5] = (short)(v + 3);
            v += 4;
            t += 2;
        }

    }
}
