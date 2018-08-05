using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Test3D4
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public BasicEffect basicEffect;
        public Effect SaturnEffect;
        public Texture2D Tex;
        public Texture2D TexRing;
        public Texture2D Tex00;
        public Texture2D Tex01;
        public Texture2D Tex02;
        public Texture2D TexTest;
        QuadDraw qdraw;
        float statetime = 0;
        float[] rng = new float[1024*256]; //1MB

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            var rand = new System.Random();
            for (int i = 0; i < rng.Length; i++) rng[i] = (float)rand.NextDouble();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, 12, 12, 0, 0/*1/1024f*/, 12);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            SaturnEffect = Content.Load<Effect>("assets/fx/SaturnExperimental");

            Tex = Content.Load<Texture2D>("assets/img/WALL");
            TexRing = Content.Load<Texture2D>("assets/img/ring");
            Tex00 = Content.Load<Texture2D>("assets/img/00");
            Tex01 = Content.Load<Texture2D>("assets/img/01");
            Tex02 = Content.Load<Texture2D>("assets/img/02");

            qdraw = new QuadDraw(GraphicsDevice, SaturnEffect);//,640,480);
            qdraw.MagicCol(Color.Lime);

            TexTest = new Texture2D(GraphicsDevice, 2, 4, false, SurfaceFormat.Vector4);
            var arr = new Vector4[8];
            arr[0] = new Vector4(1, 0, 0, 1);
            arr[1] = new Vector4(1, 1, 0, 1);
            arr[2] = new Vector4(0, 2, 0, 1);
            arr[3] = new Vector4(0, 1, 1, 1);
            arr[4] = new Vector4(0, 0, 1, 1);
            arr[5] = new Vector4(1, 0, 1, 1);
            arr[6] = new Vector4(1, 1, 1, 1);
            arr[7] = new Vector4(0, 0, 0, 1);
            TexTest.SetData<Vector4>(arr);

            SaturnEffect.Parameters["Quads"].SetValue(TexTest);
            SaturnEffect.Parameters["BatchSize"].SetValue(4f);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            graphics.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            
            qdraw.DepthEnable();
            qdraw.ScreenDoors(false);
            qdraw.Begin();

            //qdraw.DrawSprite(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), new Vector3(0 + 32, 0, -2), new Vector3(95 - 32, 0, -2), new Vector3(95, 95, -2), new Vector3(0, 95, -2), Color.Red, Color.Lime, Color.Blue, Color.White);
            //qdraw.DrawSprite(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), new Vector3(0, 0+96, -2), new Vector3(95, 0+96, -2), new Vector3(95, 95+96, -2), new Vector3(0, 95+96, -2), Color.Red, Color.Lime, Color.Blue, Color.White);

            //qdraw.DrawSprite(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), new Vector3(96 + 32, 0, -2), new Vector3(95 + 96 - 32, 0, -2), new Vector3(95 + 96, 95, -2), new Vector3(96, 95, -2), Color.Red, Color.Lime, Color.Blue, Color.White, 0, 0, 6);
            //qdraw.DrawSprite(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), new Vector3(96 + 32, 0, -2), new Vector3(95 + 96 - 32, 0, -2), new Vector3(95 + 96, 95, -2), new Vector3(96, 95, -2), Color.Red, Color.Lime, Color.Blue, Color.White);
            //qdraw.DrawSprite(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), new Vector3(96, 0+96, -2), new Vector3(95 + 96, 0+96, -2), new Vector3(95 + 96, 95+96, -2), new Vector3(96, 95+96, -2), Color.Red, Color.Lime, Color.Blue, Color.White);

            //qdraw.DrawSprite(TexRing, new Rectangle(0, 0, TexRing.Width / 8, TexRing.Height), new Vector3(0+192, 95, -1), new Vector3(95+192, 95, -1), new Vector3(95+192, 95+95, -3), new Vector3(0+192, 95+95, -3));

            //float ax = (float)System.Math.Sin(1.11 * gameTime.TotalGameTime.TotalSeconds + 0.1);
            //float ay = (float)System.Math.Sin(1.11 * gameTime.TotalGameTime.TotalSeconds + 4);
            //float bx = (float)System.Math.Sin(1.13 * gameTime.TotalGameTime.TotalSeconds + 1);
            //float by = (float)System.Math.Sin(1.13 * gameTime.TotalGameTime.TotalSeconds + 3);
            //float cx = (float)System.Math.Cos(1.17 * gameTime.TotalGameTime.TotalSeconds + 2);
            //float cy = (float)System.Math.Cos(1.17 * gameTime.TotalGameTime.TotalSeconds + 2);
            //float dx = (float)System.Math.Cos(1.15 * gameTime.TotalGameTime.TotalSeconds + 3);
            //float dy = (float)System.Math.Cos(1.15 * gameTime.TotalGameTime.TotalSeconds + 1);
            //float e = 120;
            //float f = (320 - 240)/2;

            //var pts = new Vector3[4];
            //pts[0] = new Vector3(ax * e + e + f, ay * e + e, -2);
            //pts[1] = new Vector3(bx * e + e + f, by * e + e, -2);
            //pts[2] = new Vector3(cx * e + e + f, cy * e + e, -2);
            //pts[3] = new Vector3(dx * e + e + f, dy * e + e, -2);

            //qdraw.DrawSpriteBilinear(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), pts[0], pts[1], pts[2], pts[3], Color.Red, Color.Lime, Color.Blue, Color.White);
            //qdraw.DrawSpriteBilinear(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), new Vector3(0, 0, -2), new Vector3(192, 0, -2), new Vector3(192, 192, -2), new Vector3(0, 192, -2), Color.Red, Color.Lime, Color.Blue, Color.White);

            //qdraw.DrawQuadQuick(pts[0], new Vector3(pts[0].X + 1, pts[0].Y, -2), new Vector3(pts[0].X + 1, pts[0].Y + 1, -2), new Vector3(pts[0].X, pts[0].Y + 1, -2), Color.Cyan, Color.Cyan, Color.Cyan, Color.Cyan);
            //qdraw.DrawQuadQuick(pts[1], new Vector3(pts[1].X + 1, pts[1].Y, -2), new Vector3(pts[1].X + 1, pts[1].Y + 1, -2), new Vector3(pts[1].X, pts[1].Y + 1, -2), Color.Cyan, Color.Cyan, Color.Cyan, Color.Cyan);
            //qdraw.DrawQuadQuick(pts[2], new Vector3(pts[2].X + 1, pts[2].Y, -2), new Vector3(pts[2].X + 1, pts[2].Y + 1, -2), new Vector3(pts[2].X, pts[2].Y + 1, -2), Color.Cyan, Color.Cyan, Color.Cyan, Color.Cyan);
            //qdraw.DrawQuadQuick(pts[3], new Vector3(pts[3].X + 1, pts[3].Y, -2), new Vector3(pts[3].X + 1, pts[3].Y + 1, -2), new Vector3(pts[3].X, pts[3].Y + 1, -2), Color.Cyan, Color.Cyan, Color.Cyan, Color.Cyan);

            var r32 = new Rectangle(0, 0, 32, 32);
            qdraw.DrawSpriteBilinear(Tex01, r32, new Vector3(0, 0, -1), new Vector3(32, 0, -1), new Vector3(32, 32, -1), new Vector3(0, 32, -1));
            var xoff = 32;
            qdraw.DrawSpriteBilinear(Tex00, r32, new Vector3(0 + xoff, 0, -1), new Vector3(32 + xoff, 0, -1), new Vector3(32 + xoff, 32, -1), new Vector3(0 + xoff, 32, -1));
            xoff += 32;
            qdraw.DrawSpriteBilinear(Tex01, r32, new Vector3(0 + xoff, 0, -1), new Vector3(32 + xoff, (float)(System.Math.Sin(gameTime.TotalGameTime.TotalSeconds) + 1) * 32 / 2, -1), new Vector3(32 + xoff, 32, -1), new Vector3(0 + xoff, 32, -1));
            xoff += 32;
            qdraw.DrawSprite(Tex01, r32, new Vector3(0 + xoff, 0, -1), new Vector3(32 + xoff, (float)(System.Math.Sin(gameTime.TotalGameTime.TotalSeconds) + 1) * 32 / 2, -1), new Vector3(32 + xoff, 32, -1), new Vector3(0 + xoff, 32, -1));
            xoff += 32;
            qdraw.DrawSpriteQuick(Tex01, r32, new Vector3(0 + xoff, 0, -1), new Vector3(32 + xoff, (float)(System.Math.Sin(gameTime.TotalGameTime.TotalSeconds) + 1) * 32 / 2, -1), new Vector3(32 + xoff, 32, -1), new Vector3(0 + xoff, 32, -1));
            var yoff = 32;
            qdraw.DrawSpriteBilinear(Tex02, r32, new Vector3(0, 0+yoff, -1), new Vector3(32, 0 + yoff, -1), new Vector3(32, 32 + yoff, -1), new Vector3(0, 32 + yoff, -1));
            xoff = 64;
            qdraw.DrawSpriteBilinear(Tex02, r32, new Vector3(0 + xoff, 0+yoff, -1), new Vector3(32 + xoff, (float)(System.Math.Sin(gameTime.TotalGameTime.TotalSeconds) + 1) * 32 / 2 + yoff, -1), new Vector3(32 + xoff, 32 + yoff, -1), new Vector3(0 + xoff, 32 + yoff, -1));
            xoff += 32;
            qdraw.DrawSprite(Tex02, r32, new Vector3(0 + xoff, 0 + yoff, -1), new Vector3(32 + xoff, (float)(System.Math.Sin(gameTime.TotalGameTime.TotalSeconds) + 1) * 32 / 2 + yoff, -1), new Vector3(32 + xoff, 32 + yoff, -1), new Vector3(0 + xoff, 32 + yoff, -1));
            xoff += 32;
            qdraw.DrawSpriteQuick(Tex02, r32, new Vector3(0 + xoff, 0 + yoff, -1), new Vector3(32 + xoff, (float)(System.Math.Sin(gameTime.TotalGameTime.TotalSeconds) + 1) * 32 / 2 + yoff, -1), new Vector3(32 + xoff, 32 + yoff, -1), new Vector3(0 + xoff, 32 + yoff, -1));

            qdraw.DrawSpriteQuick(TexTest, new Rectangle(0, 0, 2, 4), new Vector3(0, 100, -1), new Vector3(2, 100, -1), new Vector3(2, 104, -1), new Vector3(0, 104, -1));


            SaturnEffect.CurrentTechnique.Passes[3].Apply();

            var verts = new VertexPositionTexture[3 * 4];
            verts[0] = new VertexPositionTexture(new Vector3(100, 100, -1), new Vector2(0, 0));
            verts[1] = new VertexPositionTexture(new Vector3(103, 100, -1), new Vector2(0, 0));
            verts[2] = new VertexPositionTexture(new Vector3(100, 103, -1), new Vector2(0, 0));

            verts[3] = new VertexPositionTexture(new Vector3(110, 100, -1), new Vector2(0, 1));
            verts[4] = new VertexPositionTexture(new Vector3(113, 100, -1), new Vector2(0, 1));
            verts[5] = new VertexPositionTexture(new Vector3(110, 103, -1), new Vector2(0, 1));

            verts[6] = new VertexPositionTexture(new Vector3(120, 100, -1), new Vector2(0, 2));
            verts[7] = new VertexPositionTexture(new Vector3(123, 100, -1), new Vector2(0, 2));
            verts[8] = new VertexPositionTexture(new Vector3(120, 103, -1), new Vector2(0, 2));

            verts[9] = new VertexPositionTexture(new Vector3(130, 100, -1), new Vector2(0, 3));
            verts[10] = new VertexPositionTexture(new Vector3(133, 100, -1), new Vector2(0, 3));
            verts[11] = new VertexPositionTexture(new Vector3(130, 103, -1), new Vector2(0, 3));

            var idx = new short[3 * 4];
            for (short i = 0; i < 3 * 4; i++) idx[i] = i;
            qdraw.gdev.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, verts, 0, 3*4, idx, 0, 4);



            //var rand = new System.Random();
            //for (int i = 0; i < rng.Length; i++) rng[i] = (float)rand.NextDouble();
            //var start = System.DateTime.Now;
            //var len = rng.Length;
            //for (int i = 1; i <= 1000000; i++)
            //{
            //    qdraw.DrawSpriteBilinearToScreen(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), new Vector3(rng[(i * 3) % len] * 320, rng[(i * 5) % len] * 240, rng[(i * 7) % len] * -11 - 0.5f)
            //                                                                        , new Vector3(rng[(i * 9) % len] * 320, rng[(i * 11) % len] * 240, rng[(i * 13) % len] * -11 - 0.5f)
            //                                                                        , new Vector3(rng[(i * 15) % len] * 320, rng[(i * 17) % len] * 240, rng[(i * 19) % len] * -11 - 0.5f)
            //                                                                        , new Vector3(rng[(i * 21) % len] * 320, rng[(i * 23) % len] * 240, rng[(i * 25) % len] * -11 - 0.5f)
            //                                                                        , new Color(rng[(i * 27) % len], rng[(i * 29) % len], rng[(i * 31) % len], 1)
            //                                                                        , new Color(rng[(i * 33) % len], rng[(i * 35) % len], rng[(i * 37) % len], 1)
            //                                                                        , new Color(rng[(i * 39) % len], rng[(i * 41) % len], rng[(i * 43) % len], 1)
            //                                                                        , new Color(rng[(i * 45) % len], rng[(i * 47) % len], rng[(i * 49) % len], 1));
            //}

            qdraw.End();

            //var span = System.DateTime.Now - start;
            //System.Console.WriteLine(span.TotalMilliseconds);


            qdraw.gdev.Clear(Color.CornflowerBlue);
            basicEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp);
            spriteBatch.Draw(qdraw.buf, new Rectangle((800 - 640) / 2, 0, 640, 480), Color.White);
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
