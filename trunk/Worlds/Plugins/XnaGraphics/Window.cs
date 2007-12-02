using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace AntiCulture.Worlds.Plugins.XnaGraphics
{
    internal class Window : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager mGraphics;
        private ContentManager mContent;

        private Texture2D mUnknownTexture;
        private Dictionary<string, Texture2D> mTextures = new Dictionary<string,Texture2D>();
        private SpriteFont mFont;
        private SpriteBatch mSpriteBatch;

        private World mWorld;
        private CanRenderFlag mCanRenderFlag;

        public Window(World world, CanRenderFlag canRenderFlag)
        {
            mWorld = world;
            mCanRenderFlag = canRenderFlag;

            mGraphics = new GraphicsDeviceManager(this);
            mContent = new ContentManager(base.Services);
        }

        protected override void Initialize()
        {
            mSpriteBatch = new SpriteBatch(mGraphics.GraphicsDevice);

            base.Initialize();
        }

        private Texture2D FindEntityTexture(string entityName)
        {
            Texture2D texture;
            if (mTextures.TryGetValue(entityName, out texture)) return texture;
            try
            {
                texture = mContent.Load<Texture2D>("resources/entities/" + entityName);
                mTextures[entityName] = texture;
            }
            catch (Exception)
            {
                mTextures[entityName] = mUnknownTexture;
                return mUnknownTexture;
            }
            return texture;
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                mUnknownTexture = mContent.Load<Texture2D>("resources/unknown");
                mFont = mContent.Load<SpriteFont>("resources/font");
            }
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent == true)
            {
                mContent.Unload();
                mTextures.Clear();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            //if(Keyboard.GetState().IsKeyDown(Keys.Escape)) base.Exit();

            while (!mCanRenderFlag.IsSet)
                System.Threading.Thread.Sleep(10);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            mGraphics.GraphicsDevice.Clear(Color.White);

            mSpriteBatch.Begin();

            // Draw ball
            foreach (Entity entity in mWorld.Entities)
            {
                Vector position = entity.Position * 25.0f + new Vector(400, 300);
                Rectangle rect = new Rectangle();
                rect.X = (int)(position.X - 10);
                rect.Y = (int)(position.Y - 10);
                rect.Width = 20;
                rect.Height = 20;

                if (entity.Species == Human.Species)
                {
                    try
                    {
                        Texture2D texture = FindEntityTexture(entity.Species.Name);
                        mSpriteBatch.Draw(texture, rect, Color.White);

                        Human human = (Human)entity;
                        mSpriteBatch.DrawString(mFont, entity.Name, new Vector2(position.X, position.Y), Color.Blue);
                        if (human.CurrentOperation != null)
                            mSpriteBatch.DrawString(mFont, human.CurrentOperation.ToString(), new Vector2(position.X, position.Y + 10), Color.Red);
                    }
                    catch (Exception) { }
                }
                else
                {
                    try
                    {
                        Texture2D texture = FindEntityTexture(entity.Species.Name);
                        mSpriteBatch.Draw(texture, rect, Color.White);
                    }
                    catch (Exception) { }
                }
            }

            mSpriteBatch.End();

            mCanRenderFlag.IsSet = false;

            base.Draw(gameTime);
        }
    }
}