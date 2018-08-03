using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Test3D4
{
    class QuadDraw
    {
        public int width = 320;
        public int height = 240;
        SurfaceFormat format;
        public RenderTarget2D buf;
        public GraphicsDevice gdev;
        public Effect fx;
        public static Color noGouraud = new Color(0.5f, 0.5f, 0.5f, 1f);
        public static short[] quickIdx = new short[12];
        public static short[] halfIdx = new short[4];
        public static float smallNum = -1f;
        public VertexPosition[] fullScreen = new VertexPosition[4];
        Vector3ClockCompare clocker = new Vector3ClockCompare();

        public QuadDraw(GraphicsDevice gdev, Effect saturnEffect, int w = 320, int h = 240, Matrix? proj = null, int sdrScale = 1, SurfaceFormat format = SurfaceFormat.Color)
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
            
            halfIdx[0] = 0;
            halfIdx[1] = 1;
            halfIdx[2] = 2;
            halfIdx[3] = 3;

            this.gdev = gdev;
            fx = saturnEffect;
            this.format = format;
            Resize(w, h);
            Projection(proj);
            CullMode();
            DepthEnable();
            ScreenDoorScale(sdrScale);
        }

        public void Resize(int w, int h)
        {
            if (buf != null) buf.Dispose();
            width = w;
            height = h;
            buf = new RenderTarget2D(gdev, w, h, false, format, DepthFormat.Depth24);
            fullScreen[0] = new VertexPosition(new Vector3(0, 0, smallNum));
            fullScreen[1] = new VertexPosition(new Vector3(w, 0, smallNum));
            fullScreen[2] = new VertexPosition(new Vector3(0, h, smallNum));
            fullScreen[3] = new VertexPosition(new Vector3(w, h, smallNum));
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

        public void ScreenDoors(bool b = true)
        {
            fx.Parameters["ScreenDoor"].SetValue(b);
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
        }

        public void End()
        {
            gdev.SetRenderTarget(null);
        }

        /*
         * 6867.3928
         * 6995.4001
         * 6851.3919
         * 6944.3972
         * 6838.3911
         * 6815.3898
         * 6864.3927
         * 6919.3958
         * 6901.3948
         * 6857.3922
         * avg 6885.49384
         * qps 145232.865388781
         * 60fps 2420.54775647968
         * 30fps 4841.09551295936
        */

        public void DrawQuadQuick(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Color c0, Color c1, Color c2, Color c3)
        {
            var verts = new VertexPositionColor[5];
            verts[0] = new VertexPositionColor(p0, c0);
            verts[1] = new VertexPositionColor(p1, c1);
            verts[2] = new VertexPositionColor(p2, c2);
            verts[3] = new VertexPositionColor(p3, c3);
            verts[4] = new VertexPositionColor(AvgVertex3(p0, p1, p2, p3), AvgCol(c0, c1, c2, c3));

            fx.CurrentTechnique.Passes[1].Apply();
            gdev.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, verts, 0, 5, quickIdx, 0, 4);
        }

        public void DrawSpriteQuick(Texture2D Tex, Rectangle area, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int rt90 = 0, int flips = 0)
        {
            DrawSpriteQuick(Tex, area, p0, p1, p2, p3, noGouraud, noGouraud, noGouraud, noGouraud, rt90, flips);
        }

        /*
         * 8050.4605
         * 8045.4601
         * 8059.4609
         * 7989.457
         * 8070.4616
         * 8042.46
         * 8043.46
         * 8056.4608
         * 7890.4514
         * 8038.4598
         * avg 8028.65921
         * qps 124553.798317216
         * 60fps 2075.89663862027
         * 30fps 4151.79327724054
         */
        public void DrawSpriteQuick(Texture2D Tex, Rectangle area, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Color c0, Color c1, Color c2, Color c3, int rt90 = 0, int flips = 0)
        {
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
            var verts = new VertexPositionColorTexture[5];
            verts[0] = new VertexPositionColorTexture(p0, c0, uv[0]);
            verts[1] = new VertexPositionColorTexture(p1, c1, uv[1]);
            verts[2] = new VertexPositionColorTexture(p2, c2, uv[2]);
            verts[3] = new VertexPositionColorTexture(p3, c3, uv[3]);
            verts[4] = new VertexPositionColorTexture(AvgVertex3(p0, p1, p2, p3), AvgCol(c0, c1, c2, c3), AvgVertex2(uv[0], uv[1], uv[2], uv[3]));

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
        
        public void DrawSprite(Texture2D Tex, Rectangle area, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int rt90 = 0, int flips = 0, int strips = 0)
        {
            DrawSprite(Tex, area, p0, p1, p2, p3, noGouraud, noGouraud, noGouraud, noGouraud, rt90, flips, strips);
        }

        /*
         * 96 strips
         * 25656.4675
         * 25565.4623
         * 25539.4608
         * 25433.4547
         * 25344.4496
         * 25326.4486
         * 25285.4462
         * 25299.447
         * 25298.4469
         * 25495.4583
         * avg 25424.45419
         * qps 39332.2111274004
         * 60fps 655.53685212334
         * 30fps 1311.07370424668
         * 
         * 32 strips
         * 13811.79
         * 14014.8016
         * 13926.7966
         * 13998.8007
         * 14025.8023
         * 13840.7916
         * 13935.7971
         * 13998.8007
         * 13912.7958
         * 13902.7952
         * avg 13936.89716
         * qps 71751.9824190193
         * 60fps 1195.86637365032
         * 30fps 2391.73274730064
        */
        public void DrawSprite(Texture2D Tex, Rectangle area, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Color c0, Color c1, Color c2, Color c3, int rt90 = 0, int flips = 0, int strips = 0)
        {
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
            w = area.Width;
            h = area.Height;
            for (var i = 0; i < (rt90 & 3); i++) //& 3 or % 4 should work
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

            //tesselate into texel thick strips
            if (strips > 0 && strips < h) h = strips;
            var verts = new VertexPositionColorTexture[(int)h*2+2];
            var indicies = new short[verts.Length];
            for (int i=0; i<verts.Length/2; i++)
            {
                var l = i / h;
                verts[2 * i] = new VertexPositionColorTexture(Vector3.Lerp(p0, p3, l), Color.Lerp(c0, c3, l), Vector2.Lerp(uv[0], uv[3], l));
                verts[2 * i + 1] = new VertexPositionColorTexture(Vector3.Lerp(p1, p2, l), Color.Lerp(c1, c2, l), Vector2.Lerp(uv[1], uv[2], l));
                indicies[2 * i] = (short)(2 * i);
                indicies[2 * i + 1] = (short)(2 * i + 1);
            }

            fx.Parameters["Tex"].SetValue(Tex);
            fx.CurrentTechnique.Passes[0].Apply();
            gdev.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, verts, 0, verts.Length, indicies, 0, indicies.Length-2);
        }

        /*
         * 32 strips
         * 11216.6415
         * 11114.6357
         * 10986.6284
         * 11144.6374
         * 11135.6369
         * 11160.6384
         * 11101.635
         * 11102.635
         * 11217.6416
         * 11071.6332
         * avg 11125.23631
         * qps 89885.7311553142
         * 60fps 1498.09551925524
         * 30fps 2996.19103851047
        */
        public void DrawQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Color c0, Color c1, Color c2, Color c3, int rows = 32)
        {
            var verts = new VertexPositionColor[(int)rows * 2 + 2];
            var indicies = new short[verts.Length];
            for (int i = 0; i < verts.Length / 2; i++)
            {
                var l = i / rows;
                verts[2 * i] = new VertexPositionColor(Vector3.Lerp(p0, p3, l), Color.Lerp(c0, c3, l));
                verts[2 * i + 1] = new VertexPositionColor(Vector3.Lerp(p1, p2, l), Color.Lerp(c1, c2, l));
                indicies[2 * i] = (short)(2 * i);
                indicies[2 * i + 1] = (short)(2 * i + 1);
            }
            fx.CurrentTechnique.Passes[1].Apply();
            gdev.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, verts, 0, verts.Length, indicies, 0, indicies.Length - 2);
        }


        public void DrawSpriteBilinearToScreen(Texture2D Tex, Rectangle area, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int rt90 = 0, int flips = 0)
        {
            DrawSpriteBilinearToScreen(Tex, area, p0, p1, p2, p3, noGouraud, noGouraud, noGouraud, noGouraud, rt90, flips);
        }

        /*
         * 9706.5551
         * 9698.5547
         * 9731.5566
         * 9703.555
         * 9732.5567
         * 9648.5518
         * 9703.555
         * 9668.553
         * 9691.5543
         * 9756.5581
         * avg 9704.15503
         * qps 103048.642247423
         * 60fps 1717.47737079038
         * 30fps 3434.95474158077
        */

        public void DrawSpriteBilinearToScreen(Texture2D Tex, Rectangle area, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Color c0, Color c1, Color c2, Color c3, int rt90 = 0, int flips = 0)
        {
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

            fx.Parameters["VertexA"].SetValue(p0);
            fx.Parameters["VertexB"].SetValue(p1);
            fx.Parameters["VertexC"].SetValue(p2);
            fx.Parameters["VertexD"].SetValue(p3);

            fx.Parameters["UVA"].SetValue(uv[0]);
            fx.Parameters["UVB"].SetValue(uv[1]);
            fx.Parameters["UVC"].SetValue(uv[2]);
            fx.Parameters["UVD"].SetValue(uv[3]);

            fx.Parameters["GouraudA"].SetValue(c0.ToVector4());
            fx.Parameters["GouraudB"].SetValue(c1.ToVector4());
            fx.Parameters["GouraudC"].SetValue(c2.ToVector4());
            fx.Parameters["GouraudD"].SetValue(c3.ToVector4());

            fx.Parameters["Tex"].SetValue(Tex);
            fx.CurrentTechnique.Passes[2].Apply();
            gdev.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, fullScreen, 0, 4, halfIdx, 0, 2);
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

        public void DrawSpriteBilinear(Texture2D Tex, Rectangle area, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int rt90 = 0, int flips = 0)
        {
            DrawSpriteBilinear(Tex, area, p0, p1, p2, p3, noGouraud, noGouraud, noGouraud, noGouraud, rt90, flips);
        }

        /*
         * 10868.6216
         * 10942.6258
         * 10955.6266
         * 11025.6306
         * 10959.6269
         * 11031.631
         * 10975.6278
         * 11082.6339
         * 10812.6185
         * 10827.6193
         * avg 10948.2262
         * qps 91338.9969966094
         * 60fps 1522.31661661016
         * 30fps 3044.63323322031
         */
        public void DrawSpriteBilinear(Texture2D Tex, Rectangle area, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Color c0, Color c1, Color c2, Color c3, int rt90 = 0, int flips = 0)
        {
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

            fx.Parameters["VertexA"].SetValue(p0);
            fx.Parameters["VertexB"].SetValue(p1);
            fx.Parameters["VertexC"].SetValue(p2);
            fx.Parameters["VertexD"].SetValue(p3);

            fx.Parameters["UVA"].SetValue(uv[0]);
            fx.Parameters["UVB"].SetValue(uv[1]);
            fx.Parameters["UVC"].SetValue(uv[2]);
            fx.Parameters["UVD"].SetValue(uv[3]);

            fx.Parameters["GouraudA"].SetValue(c0.ToVector4());
            fx.Parameters["GouraudB"].SetValue(c1.ToVector4());
            fx.Parameters["GouraudC"].SetValue(c2.ToVector4());
            fx.Parameters["GouraudD"].SetValue(c3.ToVector4());

            fx.Parameters["Tex"].SetValue(Tex);
            fx.CurrentTechnique.Passes[2].Apply();

            var pts = new Vector3[4];
            pts[0] = p0;
            pts[1] = p1;
            pts[2] = p2;
            pts[3] = p3;


            var center = AvgVertex3(p0,p1,p2,p3);
            clocker.center = center;
            System.Array.Sort(pts,clocker); //untwist


            for (int i = 0; i < 4; i++)
            { //check for concave and draw the encompassing triangle
                var v0 = pts[i];
                var v1 = pts[(1 + i) % 4];
                var v2 = pts[(2 + i) % 4];
                var v3 = pts[(3 + i) % 4];
                if (ptInTriangle2D(v3, v0, v1, v2))
                {
                    //v3 is inside the triangle made by the others
                    var verts = new VertexPosition[3];
                    verts[0] = new VertexPosition(v0);
                    verts[1] = new VertexPosition(v1);
                    verts[2] = new VertexPosition(v2);
                    gdev.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, verts, 0, 3, halfIdx, 0, 1);
                    return; //my work here is done
                }
            }

            var vert = new VertexPosition[4];
            vert[0] = new VertexPosition(pts[0]);
            vert[1] = new VertexPosition(pts[1]);
            vert[2] = new VertexPosition(pts[3]);
            vert[3] = new VertexPosition(pts[2]);

            gdev.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, vert, 0, 4, halfIdx, 0, 2);
        }

    }
}
