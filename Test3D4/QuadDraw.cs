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
        public Color noGouraud = new Color(0.5f,0.5f,0.5f,1f);

        public QuadDraw(GraphicsDevice gdev, Effect saturnEffect, int w = 320, int h = 240, int sdrScale = 1, SurfaceFormat format = SurfaceFormat.Color)
        {
            this.sdrScale = sdrScale;
            this.gdev = gdev;
            fx = saturnEffect;
            this.format = format;
            Resize(width,height);
        }

        void Resize(int w, int h)
        {
            if (buf != null) buf.Dispose();
            width = w;
            height = h;
            buf = new RenderTarget2D(gdev, w, h, false, format, DepthFormat.Depth24);
        }

        void DepthEnable(bool b)
        {

        }

        void BlendEnable(bool b)
        {

        }

        void GPUProjection(bool b)
        {

        }
    }
}
