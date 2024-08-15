using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;

namespace ZombEscape
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        OrthographicCamera camera;

        FastRandom random;

        Circle player;
        List<Circle> zombies;

        long zombieSpawnTimer;
        long zombieSpawnTime;

        long gameStartTime;
        long secondsSurvived;

        string screen;

        List<string> difficulties;
        int difficulty;

        Dictionary<string, long> zombieSpawnTimes;

        string saveFileName;
        Dictionary<string, string> saveData;

        Text titleText;
        Text timeSurvivedText;
        Text gameOverText;
        Text youSurvivedText;
        Text hiscoreText;
        Text newHiscoreText;
        Text clickToContinueText;

        bool newHiscore;

        Button easyButton;
        Button mediumButton;
        Button hardButton;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = graphics.PreferredBackBufferWidth;

            TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / 120));
        }

        protected override void Initialize()
        {
            base.Initialize();

            camera = new OrthographicCamera(GraphicsDevice);

            random = new FastRandom();

            screen = "title";

            difficulties = new() { "easy", "medium", "hard" };

            difficulty = 1;

            player = new Circle(0, 0, Assets.sprites.Get("player").Width/2f);
            zombies = new List<Circle>();

            zombieSpawnTimes = new Dictionary<string, long>
            {
                { difficulties[0], 10_000_000 },
                { difficulties[1],  5_000_000 },
                { difficulties[2],  2_500_000 }
            };

            saveData = new();

            saveFileName = ".save";

            if (File.Exists(saveFileName))
            {
                try
                {
                    var lines = File.ReadAllLines(saveFileName);

                    foreach (string line in lines)
                    {
                        var strings = line.Split(' ');

                        if (strings.Length >= 2)
                        {
                            saveData.Add(strings[0], strings[1]);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    Debug.WriteLine("couldn't open save file!!");
                }
            }

            foreach (var d in difficulties)
            {
                string key = "hiscore-" + d;
                if (!saveData.ContainsKey(key))
                {
                    saveData.Add(key, "0");
                }
            }

            gameStartTime = 0;

            newHiscore = false;



            titleText = new Text(
                Assets.fonts.Get("large"),
                new Vector2(Window.ClientBounds.Width / 2f, Window.ClientBounds.Height / 3f),
                "ZombEscape",
                Color.LimeGreen,
                Origin.Center
            );

            timeSurvivedText = new Text(
                Assets.fonts.Get("medium"),
                new Vector2(Window.ClientBounds.Width / 2f, 0f),
                "",
                Color.Black,
                Origin.Top
            );

            gameOverText = new Text(
                Assets.fonts.Get("large"),
                new Vector2(Window.ClientBounds.Width / 2f, Window.ClientBounds.Height / 3f),
                "Game Over",
                Color.Red,
                Origin.Center
            );

            youSurvivedText = new Text(
                Assets.fonts.Get("medium"),
                new Vector2(Window.ClientBounds.Width / 2f, Window.ClientBounds.Height / 2f),
                "",
                Color.White,
                Origin.Center
            );

            hiscoreText = new Text(
                Assets.fonts.Get("medium"),
                new Vector2(Window.ClientBounds.Width / 2f, Window.ClientBounds.Height / 2f + 50f),
                "",
                Color.White,
                Origin.Center
            );

            newHiscoreText = new Text(
                Assets.fonts.Get("medium"),
                new Vector2(Window.ClientBounds.Width / 2f, Window.ClientBounds.Height / 2f + 90f),
                "New record!",
                Color.Gold,
                Origin.Center
            );

            clickToContinueText = new Text(
                Assets.fonts.Get("small"),
                new Vector2(Window.ClientBounds.Width / 2f, Window.ClientBounds.Height - 120),
                "Click anywhere to continue",
                Color.White,
                Origin.Center
            );



            easyButton = new Button(
                new Vector2(Window.ClientBounds.Width / 2f, Window.ClientBounds.Height / 2f),
                Assets.sprites.Get("button"),
                new Text(Assets.fonts.Get("medium"), new Vector2(), "Easy", Color.Black),
                onRelease: () =>
                {
                    difficulty = 0;
                    StartGame();
                }
            );
            easyButton.SetOrigin(Origin.Center);
            easyButton.SetTextOrigin(Origin.Center);

            mediumButton = new Button(
                new Vector2(Window.ClientBounds.Width / 2f, Window.ClientBounds.Height / 2f + 80),
                Assets.sprites.Get("button"),
                new Text(Assets.fonts.Get("medium"), new Vector2(), "Medium", Color.Black),
                onRelease: () =>
                {
                    difficulty = 1;
                    StartGame();
                }
            );
            mediumButton.SetOrigin(Origin.Center);
            mediumButton.SetTextOrigin(Origin.Center);

            hardButton = new Button(
                new Vector2(Window.ClientBounds.Width / 2f, Window.ClientBounds.Height / 2f + 160),
                Assets.sprites.Get("button"),
                new Text(Assets.fonts.Get("medium"), new Vector2(), "Hard", Color.Black),
                onRelease: () =>
                {
                    difficulty = 2;
                    StartGame();
                }
            );
            hardButton.SetOrigin(Origin.Center);
            hardButton.SetTextOrigin(Origin.Center);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Assets.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            Mouse.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Timer.Update();

            switch (screen)
            {
                case "title":
                    easyButton  .Update(gameTime);
                    mediumButton.Update(gameTime);
                    hardButton  .Update(gameTime);

                    break;
                case "game":
                    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                    secondsSurvived = (DateTime.Now.Ticks - gameStartTime) / 10_000_000L;

                    SetPlayerPosition();

                    if (DateTime.Now.Ticks - zombieSpawnTimer > zombieSpawnTime)
                    {
                        //Debug.WriteLine("helo");

                        zombieSpawnTimer = DateTime.Now.Ticks;

                        SpawnZombie();
                    }

                    foreach (var zombie in zombies)
                    {
                        var dx = zombie.X - player.X;
                        var dy = zombie.Y - player.Y;
                        var dir = Math.Atan2(dy, dx);

                        zombie.X -= 240f * (float)Math.Cos(dir) * dt;
                        zombie.Y -= 240f * (float)Math.Sin(dir) * dt;

                        if (zombie.Intersects(player)) {
                            GameOver();
                            break;
                        }
                    }

                    break;
                case "gameover":
                    if (Mouse.LeftButtonReleased)
                    {
                        screen = "title";
                    }

                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (screen)
            {
                case "title":
                    GraphicsDevice.Clear(Color.Black);

                    spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

                    titleText.Draw(spriteBatch);

                    easyButton.Draw(spriteBatch);
                    mediumButton.Draw(spriteBatch);
                    hardButton.Draw(spriteBatch);

                    spriteBatch.End();

                    break;
                case "game":
                    GraphicsDevice.Clear(Color.White);

                    spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

                    foreach (var zombie in zombies)
                    {
                        spriteBatch.Draw(Assets.sprites.Get("zombie"), new Vector2(zombie.X - Assets.sprites.Get("zombie").Width / 2f, zombie.Y - Assets.sprites.Get("zombie").Height / 2f), Color.Green);
                    }

                    spriteBatch.Draw(Assets.sprites.Get("player"), new Vector2(player.X - Assets.sprites.Get("player").Width / 2f, player.Y - Assets.sprites.Get("player").Height / 2f), Color.Black);

                    timeSurvivedText.SetText(secondsSurvived + "s");
                    timeSurvivedText.Draw(spriteBatch);

                    spriteBatch.End();
                    break;
                case "gameover":
                    GraphicsDevice.Clear(Color.Black);

                    spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

                    gameOverText.Draw(spriteBatch);
                    youSurvivedText.Draw(spriteBatch);
                    hiscoreText.Draw(spriteBatch);
                    if (newHiscore) newHiscoreText.Draw(spriteBatch);
                    clickToContinueText.Draw(spriteBatch);

                    spriteBatch.End();
                    break;
            }

            base.Draw(gameTime);
        }

        void SetPlayerPosition()
        {
            player.X = Microsoft.Xna.Framework.Input.Mouse.GetState().X;
            player.Y = Microsoft.Xna.Framework.Input.Mouse.GetState().Y;
            player.X = Math.Clamp(player.X, Assets.sprites.Get("player").Width / 2f, Window.ClientBounds.Width - Assets.sprites.Get("player").Width / 2f);
            player.Y = Math.Clamp(player.Y, Assets.sprites.Get("player").Height / 2f, Window.ClientBounds.Height - Assets.sprites.Get("player").Height / 2f);
        }

        void StartGame()
        {
            screen = "game";
            zombies.Clear();
            gameStartTime = DateTime.Now.Ticks;
            zombieSpawnTime = zombieSpawnTimes[difficulties[difficulty]];
            zombieSpawnTimer = DateTime.Now.Ticks;
            SetPlayerPosition();
            IsMouseVisible = false;
        }

        void GameOver()
        {
            screen = "gameover";
            IsMouseVisible = true;
            zombies.Clear();

            youSurvivedText.SetText("You survived " + secondsSurvived + " seconds");

            newHiscore = false;

            string hiscoreKey = "hiscore-" + difficulties[difficulty];
            long hiscore;

            try
            {
                hiscore = int.Parse(saveData[hiscoreKey]);
            }
            catch (Exception)
            {
                hiscore = 0;
            }

            if (secondsSurvived > hiscore)
            {
                hiscore = secondsSurvived;
                newHiscore = true;
                saveData[hiscoreKey] = secondsSurvived.ToString();
                SaveGame();
            }

            hiscoreText.SetText("High score: " + hiscore);

            Timer.StartTimer(
                0.2f,
                onUpdate: () =>
                {
                    var intensity = 10;
                    var x = (random.NextSingle() * intensity) - (intensity / 2);
                    var y = (random.NextSingle() * intensity) - (intensity / 2);
                    var pos = new Vector2(x, y);
                    SetCameraPosition(pos);
                },
                onComplete: () =>
                {
                    SetCameraPosition(Vector2.Zero);
                }
            );
        }

        void SpawnZombie()
        {
            var side = random.Next(4);

            float x = -Assets.sprites.Get("zombie").Width  / 2f;
            float y = -Assets.sprites.Get("zombie").Height / 2f;

            switch (side)
            {
                case 0:
                    x = Window.ClientBounds.Width + Assets.sprites.Get("zombie").Width / 2f;
                    y = Assets.sprites.Get("zombie").Height / 2f + (random.NextSingle() * (Window.ClientBounds.Height - Assets.sprites.Get("zombie").Height));
                    break;
                case 1:
                    x = Assets.sprites.Get("zombie").Width / 2f + (random.NextSingle() * (Window.ClientBounds.Width - Assets.sprites.Get("zombie").Width));
                    break;
                case 2:
                    y = Assets.sprites.Get("zombie").Height / 2f + (random.NextSingle() * (Window.ClientBounds.Height - Assets.sprites.Get("zombie").Height));
                    break;
                default:
                    x = Assets.sprites.Get("zombie").Width / 2f + (random.NextSingle() * (Window.ClientBounds.Width - Assets.sprites.Get("zombie").Width));
                    y = Window.ClientBounds.Height + Assets.sprites.Get("zombie").Height / 2f;
                    break;
            }

            var zombie = new Circle(x, y, Assets.sprites.Get("zombie").Width/2f);

            zombies.Add(zombie);
        }
        
        void SaveGame()
        {
            var lines = new List<string>();

            foreach (string key in saveData.Keys)
            {
                lines.Add(key + " " + saveData[key]);
            }

            File.WriteAllLines(saveFileName, lines);
        }

        void SetCameraPosition(Vector2 position)
        {
            camera.Position = position;
        }
    }
}
