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
        QuadDraw qdraw;
        float statetime = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            //SaturnEffect.Parameters["GouraudA"].SetValue(Color.Red.ToVector4());
            //SaturnEffect.Parameters["GouraudB"].SetValue(Color.Lime.ToVector4());
            //SaturnEffect.Parameters["GouraudC"].SetValue(Color.Blue.ToVector4());
            //SaturnEffect.Parameters["GouraudD"].SetValue(Color.White.ToVector4());
            //SaturnEffect.Parameters["UVA"].SetValue(new Vector2(0, 0));
            //SaturnEffect.Parameters["UVB"].SetValue(new Vector2(1, 0));
            //SaturnEffect.Parameters["UVC"].SetValue(new Vector2(1, 1));
            //SaturnEffect.Parameters["UVD"].SetValue(new Vector2(0, 1));

            Tex = Content.Load<Texture2D>("assets/img/WALL");
            TexRing = Content.Load<Texture2D>("assets/img/ring");

            qdraw = new QuadDraw(GraphicsDevice, SaturnEffect);//,640,480);
            qdraw.MagicCol(Color.Lime);
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

            qdraw.DrawSprite(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), new Vector3(0 + 32, 0, -2), new Vector3(95 - 32, 0, -2), new Vector3(95, 95, -2), new Vector3(0, 95, -2), Color.Red, Color.Lime, Color.Blue, Color.White);
            qdraw.DrawSprite(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), new Vector3(0, 0+96, -2), new Vector3(95, 0+96, -2), new Vector3(95, 95+96, -2), new Vector3(0, 95+96, -2), Color.Red, Color.Lime, Color.Blue, Color.White);

            //qdraw.DrawSprite(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), new Vector3(96 + 32, 0, -2), new Vector3(95 + 96 - 32, 0, -2), new Vector3(95 + 96, 95, -2), new Vector3(96, 95, -2), Color.Red, Color.Lime, Color.Blue, Color.White, 0, 0, 6);
            qdraw.DrawSprite(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), new Vector3(96 + 32, 0, -2), new Vector3(95 + 96 - 32, 0, -2), new Vector3(95 + 96, 95, -2), new Vector3(96, 95, -2), Color.Red, Color.Lime, Color.Blue, Color.White);
            qdraw.DrawSprite(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), new Vector3(96, 0+96, -2), new Vector3(95 + 96, 0+96, -2), new Vector3(95 + 96, 95+96, -2), new Vector3(96, 95+96, -2), Color.Red, Color.Lime, Color.Blue, Color.White);
            
            qdraw.DrawSprite(TexRing, new Rectangle(0, 0, TexRing.Width / 8, TexRing.Height), new Vector3(0+192, 95, -1), new Vector3(95+192, 95, -1), new Vector3(95+192, 95+95, -3), new Vector3(0+192, 95+95, -3));

            float ax = (float)System.Math.Sin(1.11 * gameTime.TotalGameTime.TotalSeconds + 0.1);
            float ay = (float)System.Math.Sin(1.11 * gameTime.TotalGameTime.TotalSeconds + 4);
            float bx = (float)System.Math.Sin(1.13 * gameTime.TotalGameTime.TotalSeconds + 1);
            float by = (float)System.Math.Sin(1.13 * gameTime.TotalGameTime.TotalSeconds + 3);
            float cx = (float)System.Math.Cos(1.17 * gameTime.TotalGameTime.TotalSeconds + 2);
            float cy = (float)System.Math.Cos(1.17 * gameTime.TotalGameTime.TotalSeconds + 2);
            float dx = (float)System.Math.Cos(1.15 * gameTime.TotalGameTime.TotalSeconds + 3);
            float dy = (float)System.Math.Cos(1.15 * gameTime.TotalGameTime.TotalSeconds + 1);
            float e = 120;
            float f = (320 - 240)/2;

            var pts = new Vector3[4];
            pts[0] = new Vector3(ax * e + e + f, ay * e + e, -2);
            pts[1] = new Vector3(bx * e + e + f, by * e + e, -2);
            pts[2] = new Vector3(cx * e + e + f, cy * e + e, -2);
            pts[3] = new Vector3(dx * e + e + f, dy * e + e, -2);

            qdraw.DrawSpriteBilinear(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), pts[0], pts[1], pts[2], pts[3], Color.Red, Color.Lime, Color.Blue, Color.White);
            //qdraw.DrawSpriteBilinear(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), new Vector3(0, 0, -2), new Vector3(192, 0, -2), new Vector3(192, 192, -2), new Vector3(0, 192, -2), Color.Red, Color.Lime, Color.Blue, Color.White);

            qdraw.DrawQuadQuick(pts[0], new Vector3(pts[0].X + 1, pts[0].Y, -2), new Vector3(pts[0].X + 1, pts[0].Y + 1, -2), new Vector3(pts[0].X, pts[0].Y + 1, -2), Color.Cyan, Color.Cyan, Color.Cyan, Color.Cyan);
            qdraw.DrawQuadQuick(pts[1], new Vector3(pts[1].X + 1, pts[1].Y, -2), new Vector3(pts[1].X + 1, pts[1].Y + 1, -2), new Vector3(pts[1].X, pts[1].Y + 1, -2), Color.Cyan, Color.Cyan, Color.Cyan, Color.Cyan);
            qdraw.DrawQuadQuick(pts[2], new Vector3(pts[2].X + 1, pts[2].Y, -2), new Vector3(pts[2].X + 1, pts[2].Y + 1, -2), new Vector3(pts[2].X, pts[2].Y + 1, -2), Color.Cyan, Color.Cyan, Color.Cyan, Color.Cyan);
            qdraw.DrawQuadQuick(pts[3], new Vector3(pts[3].X + 1, pts[3].Y, -2), new Vector3(pts[3].X + 1, pts[3].Y + 1, -2), new Vector3(pts[3].X, pts[3].Y + 1, -2), Color.Cyan, Color.Cyan, Color.Cyan, Color.Cyan);

            qdraw.End();

            qdraw.gdev.Clear(Color.CornflowerBlue);
            basicEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp);
            spriteBatch.Draw(qdraw.buf, new Rectangle((800 - 640) / 2, 0, 640, 480), Color.White);
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
