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
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, 12, 12, 0, 1/256f, 12);

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
            SaturnEffect = Content.Load<Effect>("assets/fx/Saturn");

            Tex = Content.Load<Texture2D>("assets/img/WALL");
            TexRing = Content.Load<Texture2D>("assets/img/ring");

            qdraw = new QuadDraw(GraphicsDevice, SaturnEffect,640,480);
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

            qdraw.DrawSpriteQuick(Tex, new Rectangle(0, 0, Tex.Width, Tex.Height), new Vector3(0, 0, -2), new Vector3(95, 0, -2), new Vector3(95, 95, -2), new Vector3(0, 95, -2));

            qdraw.DrawSpriteQuick(TexRing, new Rectangle(0, 0, TexRing.Width / 8, TexRing.Height), new Vector3(0, 0, -1), new Vector3(95, 0, -1), new Vector3(95, 95, -3), new Vector3(0, 95, -3));

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
