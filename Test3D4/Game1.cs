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
            SaturnEffect.Parameters["Projection"].SetValue(Matrix.CreateOrthographicOffCenter(0, 12, 12, 0, 1 / 256f, 12));
            SaturnEffect.Parameters["MagicColEnable"].SetValue(true);
            SaturnEffect.Parameters["MagicCol"].SetValue(new Color(0,255,0,255).ToVector4());

            Tex = Content.Load<Texture2D>("assets/img/WALL");
            TexRing = Content.Load<Texture2D>("assets/img/ring");
            SaturnEffect.Parameters["Tex"].SetValue(Tex);
            SaturnEffect.Parameters["ScreenDoorScale"].SetValue(1);
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
            SaturnEffect.Parameters["ScreenDoor"].SetValue(true);
            SaturnEffect.CurrentTechnique.Passes[1].Apply();
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            //GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var col = new Color(0.75f,0.75f,0f,1f);

            // TODO: Add your drawing code here
            VertexPositionColor[] vertecies = new VertexPositionColor[8];
            vertecies[0] = new VertexPositionColor(new Vector3(5, 4, -4), col);
            vertecies[1] = new VertexPositionColor(new Vector3(7, 4, -4), col);
            vertecies[2] = new VertexPositionColor(new Vector3(8, 8, -2), col);
            vertecies[3] = new VertexPositionColor(new Vector3(4, 8, -2), col);

            vertecies[4] = new VertexPositionColor(new Vector3(5, 2, -2.75f), Color.Black);
            vertecies[5] = new VertexPositionColor(new Vector3(7, 3, -3.25f), Color.White);
            vertecies[6] = new VertexPositionColor(new Vector3(7, 9, -3.25f), Color.White);
            vertecies[7] = new VertexPositionColor(new Vector3(5, 10, -2.75f), Color.Black);

            short[] indecies = new short[12];
            indecies[0] = 0;
            indecies[1] = 1;
            indecies[2] = 2;

            indecies[3] = 0;
            indecies[4] = 2;
            indecies[5] = 3;

            indecies[6] = 4;
            indecies[7] = 5;
            indecies[8] = 6;

            indecies[9] = 4;
            indecies[10] = 6;
            indecies[11] = 7;
            
            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertecies, 0, 8, indecies, 0, 2);

            SaturnEffect.Parameters["ScreenDoor"].SetValue(false);
            SaturnEffect.CurrentTechnique.Passes[1].Apply();

            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertecies, 0, 8, indecies, 6, 2);

            var noGourad = new Color(0.5f, 0.5f, 0.5f, 1f);


            VertexPositionColorTexture[] verts = new VertexPositionColorTexture[5];
            //verts[0] = new VertexPositionColorTexture(new Vector3(10f, 0f, -1f), noGourad, new Vector2(0, 0));
            //verts[1] = new VertexPositionColorTexture(new Vector3(12f, 0f, -1f), noGourad, new Vector2(1 / 8f, 0));
            //verts[2] = new VertexPositionColorTexture(new Vector3(12f, 16 / 9f * 2f, -1f), noGourad, new Vector2(1 / 8f, 1));
            //verts[3] = new VertexPositionColorTexture(new Vector3(10f, 16 / 9f * 2f, -1f), noGourad, new Vector2(0, 1));
            verts[0] = new VertexPositionColorTexture(new Vector3(0f, 0f, -1f), noGourad, new Vector2(0, 0));
            verts[1] = new VertexPositionColorTexture(new Vector3(12f, 0f, -1f), noGourad, new Vector2(1 / 8f, 0));
            verts[2] = new VertexPositionColorTexture(new Vector3(12f, 12f, -1f), noGourad, new Vector2(1 / 8f, 1));
            verts[3] = new VertexPositionColorTexture(new Vector3(0f, 12f, -1f), noGourad, new Vector2(0, 1));

            SaturnEffect.Parameters["Tex"].SetValue(TexRing);
            SaturnEffect.Parameters["ScreenDoor"].SetValue(true);
            SaturnEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, verts, 0, 4, indecies, 0, 2);
            
            verts[0] = new VertexPositionColorTexture(new Vector3(0f, 0f, -1f), new Color(0.75f, 0.25f, 0.25f, 1f), new Vector2(0, 0));
            verts[1] = new VertexPositionColorTexture(new Vector3(9 / 3f, 0f, -1f), new Color(0.25f, 0.75f, 0.25f, 1f), new Vector2(1, 0));
            verts[2] = new VertexPositionColorTexture(new Vector3(9 / 3f, 16 / 3f, -1f), new Color(0.25f, 0.25f, 0.75f, 1f), new Vector2(1, 1));
            verts[3] = new VertexPositionColorTexture(new Vector3(0f, 16 / 3f, -1f), new Color(0.75f, 0.75f, 0.75f, 1f), new Vector2(0, 1));
            verts[4] = new VertexPositionColorTexture(new Vector3(9f/3/2, 16 / 3f/2, -1f), new Color(0.5f, 0.5f, 0.5f, 1f), new Vector2(0.5f, 0.5f));
            
            indecies[0] = 0;
            indecies[1] = 1;
            indecies[2] = 4;

            indecies[3] = 1;
            indecies[4] = 2;
            indecies[5] = 4;

            indecies[6] = 2;
            indecies[7] = 3;
            indecies[8] = 4;

            indecies[9] = 3;
            indecies[10] = 0;
            indecies[11] = 4;

            SaturnEffect.Parameters["Tex"].SetValue(Tex);
            SaturnEffect.Parameters["ScreenDoor"].SetValue(false);
            SaturnEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, verts, 0, 5, indecies, 0, 4);



            base.Draw(gameTime);
        }
    }
}
