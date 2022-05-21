// <copyright file="Level.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming


namespace OEInvaders.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using OEInvaders.Library;
    
    
    /// <summary>
    /// Tells what's the current status of the game.
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// Set when the game is starting.
        /// </summary>
        Starting,

        /// <summary>
        /// Set when the game has been started.
        /// </summary>
        Started,

        /// <summary>
        /// Set when the game has been stopped.
        /// </summary>
        Stopped,

        /// <summary>
        /// Set when the level has been completed.
        /// </summary>
        LevelCompleted,
    }

    /// <summary>
    /// This class is used to represent the level handling of the game.
    /// </summary>
    public class Level
    {
        private const Teachers SemiBoss = Teachers.SzaboZS;
        private const Teachers FirstBoss = Teachers.SergyanSz;
        private const int StartingLives = 3;

        private readonly Random random;
        private readonly double playermovement;
        private readonly double boosterveritcalmovement;
        private readonly double missilemovement;
        private readonly object locker = new object();
        private readonly List<Teacher> AllTeachers = new List<Teacher>();
        private readonly List<Teachers> EvilTeachers = new List<Teachers>();
        private readonly List<Teachers> GoodTeachers = new List<Teachers>();
        private readonly List<Shield> Shields = new List<Shield>();
        private readonly Dictionary<int, Missile> AllMissiles = new Dictionary<int, Missile>();
        private readonly object MissileLocker = new object();
        private readonly Dictionary<int, Point> coords = new Dictionary<int, Point>();
        private readonly List<Player> playerHealths = new List<Player>();
        private readonly object ShieldLocker = new object();
        private readonly object LevelLocker = new object();

        private Booster gameBooster;
        private FormattedText middleFormattedText;
        private Point middleFormattedPoint;
        private Key keypressed;
        private bool firing;
        private bool invincible;
        private bool respawninvincible;
        private bool doubletap;
        private bool canfire = true;
        private Player player;
        private double teachermovement;
        private double szabomovement;
        private Task gamehandlerthread;
        private bool running;
        private bool superfiring = false;
        private string playername;
        private PlayerCharacter playerCharacter;
        private Szabo szaboTeacher;
        private Sergyan sergyanTeacher;
        private CTimer endGameExplosion;
        private double boosterCounter;
        private CTimer boosterTimerC;
        private bool levelEnded = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Level"/> class.
        /// </summary>
        /// <param name="gamesize">Screen size.</param>
        /// <param name="pixelsize">Pixel size.</param>
        /// <param name="playername">Name.</param>
        /// <param name="character">Character.</param>
        public Level(Size gamesize, Size pixelsize, string playername, PlayerCharacter character)
        {
            this.CurrentLevel = 1;
            this.playername = playername;
            this.playerCharacter = character;
            for (int i = 0; i < StartingLives; i++)
            {
                this.playerHealths.Add(new Player("holder" + i, character, new Point(110 + (i * 35), GameLogic.Screen.Height - 16), true));
            }

            this.random = new Random();
            this.EvilTeachers.Add(Teachers.KovacsA);
            this.EvilTeachers.Add(Teachers.SiposM);
            this.EvilTeachers.Add(Teachers.SzenaS);
            this.EvilTeachers.Add(Teachers.VernyihelZ);
            this.EvilTeachers.Add(Teachers.RaczE);

            this.GoodTeachers.Add(Teachers.GyenesS);
            this.GoodTeachers.Add(Teachers.SimonG);
            this.GoodTeachers.Add(Teachers.KissD);
            this.GameSize = gamesize;
            this.PixelSize = pixelsize;
            this.playermovement = this.PixelSize.Width * 1;
            this.teachermovement = this.PixelSize.Width;
            this.szabomovement = this.PixelSize.Width / 2;
            this.missilemovement = this.PixelSize.Height * 3;
            this.boosterveritcalmovement = this.PixelSize.Height / 3;

            CTimer.SetTimer(this.MakeInvadersFire, 3500, 0);
            CTimer.SetTimer(this.RandomizeBooster, 25000, 0);
        }

        /// <summary>
        /// Gets score.
        /// </summary>
        public int Score
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current lives.
        /// </summary>
        public int Lives
        {
            get
            {
                return this.playerHealths.Count;
            }
        }

        /// <summary>
        /// Gets currentlevel.
        /// </summary>
        public int CurrentLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the current game status.
        /// </summary>
        public GameStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current pixel size.
        /// </summary>
        private Size PixelSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current game size.
        /// </summary>
        private Size GameSize
        {
            get;
            set;
        }

        /// <summary>
        /// Runs when a key is pressed.
        /// </summary>
        /// <param name="key">Key.</param>
        public void PressKey(Key key)
        {
            if (key == Key.Space)
            {
                this.firing = true;
                return;
            }

            this.keypressed = key;
        }

        /// <summary>
        /// Runs when a key is released.
        /// </summary>
        /// <param name="key">Key.</param>
        public void ReleaseKey(Key key)
        {
            if (key == Key.Space)
            {
                this.firing = false;
                return;
            }

            this.keypressed = Key.None;
        }

        /// <summary>
        /// Draws the scene.
        /// </summary>
        /// <param name="context">Context.</param>
        public void Draw(DrawingContext context)
        {
            lock (this.locker)
            {
                if (this.player != null)
                {
                    if (this.player.PlayerStatus == EntityStatus.Fine && this.playerHealths.Count > 0)
                    {
                        context.DrawImage(this.player.Image, this.player.Bounds);
                    }
                    else if (this.player.PlayerStatus == EntityStatus.Exploding)
                    {
                        if (this.player.Explosion != null)
                        {
                            context.DrawImage(this.player.Explosion, this.player.Bounds);
                        }
                    }
                    else
                    {
                        this.player.PlayerStatus = EntityStatus.Fine;
                    }
                }

                if (this.respawninvincible)
                {
                    if (this.boosterCounter <= 1000)
                    {
                        context.DrawText(GameLogic.FormattedStorage.TextRespawnInvincibleRed, GameLogic.PosBooster);
                    }
                    else
                    {
                        context.DrawText(GameLogic.FormattedStorage.TextRespawnInvincible, GameLogic.PosBooster);
                    }
                }
                else if (this.invincible)
                {
                    if (this.boosterCounter <= 1000)
                    {
                        context.DrawText(GameLogic.FormattedStorage.TextInvincibleRed, GameLogic.PosBooster);
                    }
                    else
                    {
                        context.DrawText(GameLogic.FormattedStorage.TextInvincible, GameLogic.PosBooster);
                    }
                }
                else if (this.doubletap)
                {
                    if (this.boosterCounter <= 1000)
                    {
                        context.DrawText(GameLogic.FormattedStorage.TextDoubleRocketsRed, GameLogic.PosBooster);
                    }
                    else
                    {
                        context.DrawText(GameLogic.FormattedStorage.TextDoubleRockets, GameLogic.PosBooster);
                    }
                }
                else if (this.superfiring)
                {
                    if (this.boosterCounter <= 1000)
                    {
                        context.DrawText(GameLogic.FormattedStorage.TextSuperRocketsRed, GameLogic.PosBooster);
                    }
                    else
                    {
                        context.DrawText(GameLogic.FormattedStorage.TextSuperRockets, GameLogic.PosBooster);
                    }
                }

                foreach (Teacher t in this.AllTeachers)
                {
                    if (t.IsAlive)
                    {
                        context.DrawImage(t.Image, t.Bounds);
                    }
                    else
                    {
                        if (t.InvaderStatus == EntityStatus.Fine)
                        {
                            t.InvaderStatus = EntityStatus.Exploding;

                            // Copy the collection
                            ReadOnlyCollection<BitmapFrame> frames = ExplosionEffect.GifDecoder.Frames;
                            Thread th = new Thread(() =>
                            {
                                for (int i = 0; i < ExplosionEffect.Frames; i++)
                                {
                                    try
                                    {
                                        GameLogic.GameDispatcher.Invoke(() =>
                                        {
                                            if (ExplosionEffect.Frames >= i)
                                            {
                                                BitmapFrame xd = frames[i];
                                                t.Explosion = xd;
                                            }
                                        });
                                    }
                                    catch
                                    {
                                        // the thread was probably aborted
                                    }

                                    System.Threading.Thread.Sleep(100);
                                }
                                t.InvaderStatus = EntityStatus.FinishedExploding;
                            });
                            th.Start();
                        }

                        if (t.Explosion != null && t.InvaderStatus != EntityStatus.FinishedExploding)
                        {
                            context.DrawImage(t.Explosion, t.Bounds);
                        }
                    }
                }

                if (this.szaboTeacher != null)
                {
                    if (this.szaboTeacher.IsAlive)
                    {
                        if (this.szaboTeacher.InvaderStatus == EntityStatus.Exploding)
                        {
                            if (this.szaboTeacher.Explosion != null)
                            {
                                context.DrawImage(this.szaboTeacher.Explosion, this.szaboTeacher.Bounds);
                            }
                        }
                        else if (this.szaboTeacher.InvaderStatus == EntityStatus.Fine)
                        {
                            if (this.szaboTeacher.Zsanie)
                            {
                                context.DrawImage(this.szaboTeacher.ZsanImage, this.szaboTeacher.Bounds);
                            }
                            else
                            {
                                context.DrawImage(this.szaboTeacher.Image, this.szaboTeacher.Bounds);
                            }
                        }
                    }
                }

                if (this.sergyanTeacher != null)
                {
                    context.DrawImage(this.sergyanTeacher.Image, this.sergyanTeacher.Bounds);
                }

                for (int i = 0; i < this.playerHealths.Count; i++)
                {
                    if (this.playerHealths[i] != null)
                    {
                        Player p = this.playerHealths[i];
                        context.DrawImage(p.Image, p.Bounds);
                    }
                }

                foreach (var x in this.Shields)
                {
                    if (x.IsAlive)
                    {
                        context.DrawImage(x.Image, x.Bounds);
                    }
                }

                try
                {
                    Booster temp = this.gameBooster;
                    if (temp != null)
                    {
                        context.DrawImage(temp.Image, temp.Bounds);
                    }
                }
                catch
                {
                    // ignore.
                }

                var copy = new List<int>(this.AllMissiles.Keys);
                lock (this.MissileLocker)
                {
                    foreach (int n in copy)
                    {
                        Missile x = this.AllMissiles[n];
                        if (!x.Exploded)
                        {
                            this.MoveMissile(x);
                            context.DrawImage(x.Image, x.Bounds);
                        }
                        else
                        {
                            if (this.AllMissiles.ContainsKey(n))
                            {
                                this.AllMissiles.Remove(n);
                            }
                        }
                    }
                }

                if (this.middleFormattedText != null)
                {
                    context.DrawText(this.middleFormattedText, this.middleFormattedPoint);
                }
            }
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        internal void Start()
        {
            this.running = true;
            this.gamehandlerthread = Task.Factory.StartNew(this.RenderHandle);
        }

        /// <summary>
        /// Resets the game's settings.
        /// </summary>
        internal void Reset()
        {
            this.Status = GameStatus.Starting;
            lock (this.MissileLocker)
            {
                this.AllMissiles.Clear();
            }

            this.player = Creator.CreatePlayer(this.playername, this.playerCharacter);
            this.playerHealths.Clear();
            for (int i = 0; i < StartingLives; i++)
            {
                this.playerHealths.Add(new Player("holder" + i, this.playerCharacter, new Point(110 + (i * 35), GameLogic.Screen.Height - 16), true));
            }

            this.gameBooster = null;
            this.AllTeachers.Clear();
            this.Shields.Clear();
            this.SpawnInvaderTeachers();
            this.SpawnShields();

            this.DisplayNotification(GameLogic.FormattedStorage.TextLevelStart, GameLogic.PosTextLevelStart);

            CTimer.SetTimer(
                () =>
            {
                this.Status = GameStatus.Started;
                this.DisplayNotification(null, new Point(0, 0));
            }, 2000);
        }

        /// <summary>
        /// Stops the current game.
        /// </summary>
        internal void Stop()
        {
            this.running = false;
        }

        //private int i = 0;
        private void RandomizeBooster()
        {
            if (this.Status != GameStatus.Started)
            {
                return;
            }

            if (this.gameBooster != null)
            {
                return;
            }

            Teachers t = this.GoodTeachers[this.random.Next(0, this.GoodTeachers.Count)];
            int num = (int)t;
            /*Teachers t = GoodTeachers[i];
            i++;
            int num = (int)t;
            if (i >= GoodTeachers.Count)
            {
                i = 0;
            }*/
            Point randomp = this.coords[this.random.Next(0, this.coords.Keys.Count)];

            this.gameBooster = new Booster(t, Images.ImageList[num.ToString()], randomp);
        }
        
        private List<Teachers> Shuffle(List<Teachers> deck)
        {
            List<Teachers> newdeck = deck;
            for (int n = newdeck.Count - 1; n > 0; --n)
            {
                int k = this.random.Next(n + 1);
                Teachers temp = newdeck[n];
                newdeck[n] = newdeck[k];
                newdeck[k] = temp;
            }

            return newdeck;
        }

        private void SpawnInvaderTeachers()
        {
            this.coords.Clear();
            double x = 120;
            this.AllTeachers.Clear();

            List<Teachers> shuffle = this.Shuffle(EvilTeachers);
            if (this.CurrentLevel < 5)
            {
                for (int i = 0; i < 11; i++)
                {
                    double y = 70;

                    this.AllTeachers.Add(Creator.CreateInvader(new Point(x + (i * 40), y), shuffle[0]));
                    this.coords.Add(i, new Point(x + (i * 40), y));
                    y += 50;

                    this.AllTeachers.Add(Creator.CreateInvader(new Point(x + (i * 40), y), shuffle[1]));
                    y += 50;

                    this.AllTeachers.Add(Creator.CreateInvader(new Point(x + (i * 40), y), shuffle[2]));
                    y += 50;

                    this.AllTeachers.Add(Creator.CreateInvader(new Point(x + (i * 40), y), shuffle[3]));
                    y += 50;

                    this.AllTeachers.Add(Creator.CreateInvader(new Point(x + (i * 40), y), shuffle[4]));
                }
            }
            else
            {
                int szabo = (int)Teachers.SzaboZS;
                for (int i = 0; i < 11; i++)
                {
                    double y = 90;

                    if (i == 5)
                    {
                        this.szaboTeacher = new Szabo(Images.ImageList[szabo.ToString()], new Point(x + (i * 40), 70));
                    }

                    y += 50;

                    this.AllTeachers.Add(Creator.CreateInvader(new Point(x + (i * 40), y), shuffle[1]));
                    y += 50;
                    this.coords.Add(i, new Point(x + (i * 40), y));

                    this.AllTeachers.Add(Creator.CreateInvader(new Point(x + (i * 40), y), shuffle[2]));
                    y += 50;

                    this.AllTeachers.Add(Creator.CreateInvader(new Point(x + (i * 40), y), shuffle[3]));
                    y += 50;

                    this.AllTeachers.Add(Creator.CreateInvader(new Point(x + (i * 40), y), shuffle[4]));
                }
            }
        }

        private void SpawnShields()
        {
            lock (ShieldLocker)
            {
                this.Shields.Clear();
                const string Pattern =
                    "00000111111111111111100000 00001111111111111111110000 00011111111111111111111000 00111111111111111111111100 01111111111111111111111110" +
                    " 11111111111111111111111111 11111111111111111111111111 11111111111111111111111111 11111111111111111111111111 11111111111111111111111111" +
                    " 11111111111111111111111111 11111111110000001111111111 11111111100000000111111111 11111111000000000011111111 11111110000000000001111111";
                string[] spriteLines = Pattern.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                int columnCount = 26;
                for (int r = 0; r < 15; r++)
                {
                    for (int c = 0; c < columnCount; c++)
                    {
                        if (spriteLines[r][c] == '1')
                        {
                            // 135 + (1 * 120) + (c * 4), 390 + (r * 4))
                            Shield s = Creator.CreateShield(new Point(270 + (c * 4), 390 + (r * 4)));
                            this.Shields.Add(s);
                        }
                    }
                }
            }
        }

        private void MakeInvadersFire()
        {
            if (this.Status != GameStatus.Started)
            {
                return;
            }

            if (this.szaboTeacher != null)
            {
                if (this.szaboTeacher.IsAlive)
                {
                    CTimer.SetTimer(
                        () =>
                        {
                            if (this.szaboTeacher.IsAlive)
                            {
                                lock (this.MissileLocker)
                                {
                                    int count = this.AllMissiles.Count + 1;
                                    while (this.AllMissiles.ContainsKey(count))
                                    {
                                        count++;
                                    }

                                    this.AllMissiles.Add(count, Creator.CreateMissile(MissileDirection.TowardsPlayer, MissileType.TeacherType, this.szaboTeacher.Position));
                                }
                            }
                        }, (uint)this.random.Next(300, 600), (uint)this.random.Next(6, 11));
                }
            }

            int TeachersAlive = this.AllTeachers.Count(o => o.IsAlive);
            List<Teacher> RandomTeachers = this.GetRandomTeachers(this.AllTeachers, this.random.Next(7, 11));
            int Addition = 0;
            if (TeachersAlive > 0 && TeachersAlive <= 10)
            {
                Addition = 50;
            }
            else if (TeachersAlive > 10 && TeachersAlive <= 35)
            {
                Addition = 35;
            }
            else if (TeachersAlive > 35 && TeachersAlive <= 50)
            {
                Addition = this.random.Next(20, 30);
            }

            foreach (Teacher t in RandomTeachers)
            {
                CTimer.SetTimer(
         () =>
                {
                    if (t.IsAlive)
                    {
                        if (this.random.Next(1, 101) <= 50 + Addition)
                        {
                            lock (this.MissileLocker)
                            {
                                int count = this.AllMissiles.Count + 1;
                                while (this.AllMissiles.ContainsKey(count))
                                {
                                    count++;
                                }

                                this.AllMissiles.Add(count, Creator.CreateMissile(MissileDirection.TowardsPlayer, MissileType.TeacherType, t.Position));
                            }
                        }
                    }
                }, (uint)this.random.Next(50, 1200));
            }
        }

        private List<Teacher> GetRandomTeachers(IEnumerable<Teacher> list, int elementsCount)
        {
            return list.OrderBy(arg => Guid.NewGuid()).Where(o => o.IsAlive).Take(elementsCount).ToList();
        }

        private void MoveBooster()
        {
            if (this.Status == GameStatus.Stopped)
            {
                this.gameBooster = null;
                return;
            }

            if (this.Status == GameStatus.LevelCompleted)
            {
                this.gameBooster = null;
                return;
            }

            Booster temp = this.gameBooster;
            if (temp == null)
            {
                return;
            }

            temp.SetPosition(temp.Position.X, temp.Position.Y + this.boosterveritcalmovement);
            if (temp.Bounds.IntersectsWith(this.player.Bounds))
            {
                if (temp.BoosterTeacher == Teachers.GyenesS)
                {
                    Score = Score + random.Next(10, 21);
                    this.boosterCounter = 4500;
                    this.superfiring = true;
                    CTimer.SetTimer(() => { this.superfiring = false; }, 4500);
                    
                    if (boosterTimerC != null)
                    {
                        boosterTimerC.Kill();
                        boosterTimerC = null;
                    }
                    
                    boosterTimerC = CTimer.SetTimer(() =>
                    {
                        this.boosterCounter = this.boosterCounter - 500;
                    }, 500, 9);
                }
                else if (temp.BoosterTeacher == Teachers.SimonG)
                {
                    Score = Score + random.Next(10, 21);
                    this.boosterCounter = 4500;
                    this.invincible = true;
                    CTimer.SetTimer(() => { this.invincible = false; }, 4500);
                    
                    if (boosterTimerC != null)
                    {
                        boosterTimerC.Kill();
                        boosterTimerC = null;
                    }
                    
                    boosterTimerC = CTimer.SetTimer(() =>
                    {
                        this.boosterCounter = this.boosterCounter - 500;
                    }, 500, 9);
                }
                else if (temp.BoosterTeacher == Teachers.KissD)
                {
                    Score = Score + random.Next(10, 21);
                    this.boosterCounter = 4500;
                    this.doubletap = true;
                    CTimer.SetTimer(() => { this.doubletap = false; }, 4500);
                    
                    if (boosterTimerC != null)
                    {
                        boosterTimerC.Kill();
                        boosterTimerC = null;
                    }
                    
                    boosterTimerC = CTimer.SetTimer(() =>
                    {
                        this.boosterCounter = this.boosterCounter - 500;
                    }, 500, 9);
                }

                this.gameBooster = null;
                return;
            }

            if (temp.Position.Y > 504)
            {
                this.gameBooster = null;
            }
        }

        private void MoveInvaders()
        {
            if (this.Status == GameStatus.Stopped)
            {
                return;
            }

            if (!this.AllTeachers.Any(i => i.IsAlive) && this.Status != GameStatus.LevelCompleted)
            {
                if (this.szaboTeacher == null)
                {
                    if (this.levelEnded)
                    {
                        return;
                    }
                    
                    lock (LevelLocker)
                    {
                        if (this.Status != GameStatus.LevelCompleted)
                        {
                            this.levelEnded = true;
                            this.Status = GameStatus.LevelCompleted;
                            this.DisplayNotification(GameLogic.FormattedStorage.TextLevelCompleted,
                                GameLogic.PosTextLevelCompleted);
                            CTimer.SetTimer(
                                () =>
                                {
                                    this.levelEnded = false;
                                    this.DisplayNotification(null, new Point(0, 0));
                                    this.Reset();
                                }, 2000);

                            this.CurrentLevel++;
                        }
                    }
                }
                else if (this.szaboTeacher != null)
                {
                    if (!this.szaboTeacher.IsAlive)
                    {
                        return;
                    }
                }
            }

            // 1.5
            this.teachermovement = Math.Sign(this.teachermovement) * this.PixelSize.Width * (1d + (3d * (1d - (this.AllTeachers.Count / 55d)))) / 3;

            double verticalmovement = 0;
            foreach (Teacher t in this.AllTeachers)
            {
                if (t.InvaderStatus == EntityStatus.FinishedExploding)
                {
                    continue;
                }

                Point position = t.Position;
                if ((position.X > 615 && this.teachermovement > 0) || (position.X < 15 && this.teachermovement < 0))
                {
                    this.teachermovement = -this.teachermovement;
                    if (this.CurrentLevel == 5)
                    {
                        verticalmovement = 1.5;
                    }
                    else
                    {
                        verticalmovement = 1.5 + (this.CurrentLevel / 10 * 5);
                    }
                    break;
                }
            }

            if (this.szaboTeacher != null && this.szaboTeacher.IsAlive)
            {
                Point position = this.szaboTeacher.Position;
                if ((position.X > 615 && this.szabomovement > 0) || (position.X < 25 && this.szabomovement < 0))
                {
                    this.szabomovement = -this.szabomovement;
                    if (this.CurrentLevel == 5)
                    {
                        verticalmovement = 1.5;
                    }
                    else
                    {
                        verticalmovement = 1.5 + (this.CurrentLevel / 10 * 5);
                    }
                }

                this.szaboTeacher.SetPosition(this.szaboTeacher.Position.X + this.szabomovement, this.szaboTeacher.Position.Y + verticalmovement);
            }

            foreach (Teacher t in this.AllTeachers)
            {
                if (t.InvaderStatus == EntityStatus.FinishedExploding)
                {
                    continue;
                }

                t.SetPosition(t.Position.X + this.teachermovement, t.Position.Y + verticalmovement);

                if (t.Position.Y > 504 && this.Status != GameStatus.Stopped)
                {
                    this.Status = GameStatus.Stopped;
                    this.playerHealths.Clear();
                    this.DisplayNotification(GameLogic.FormattedStorage.TextGameOver, GameLogic.PosTextGameOver);
                    CTimer.SetTimer(
                        () =>
                    {
                        EndGameHandler.RunGameOver(this.playername, this.Score);
                        this.DisplayNotification(null, new Point(0, 0));
                    }, 2000);
                }
            }
        }

        private void RenderHandle()
        {
            this.SpawnInvaderTeachers();
            this.SpawnShields();
            this.player = Creator.CreatePlayer(this.playername, this.playerCharacter);
            this.Status = GameStatus.Starting;
            this.DisplayNotification(GameLogic.FormattedStorage.TextLevelStart, GameLogic.PosTextLevelStart);
            CTimer.SetTimer(
                () =>
            {
                this.Status = GameStatus.Started;
                this.DisplayNotification(null, new Point(0, 0));
            }, 2000);
            while (this.running)
            {
                if (this.Status == GameStatus.Starting || this.Status == GameStatus.LevelCompleted)
                {
                    continue;
                }

                if (this.sergyanTeacher != null)
                {
                    if (this.sergyanTeacher.Position.X > 325)
                    {
                        this.sergyanTeacher.SetPosition(this.sergyanTeacher.Position.X - 1, this.sergyanTeacher.Position.Y);
                    }
                    else if (this.endGameExplosion == null)
                    {
                        this.Status = GameStatus.Stopped;
                        this.DisplayNotification(GameLogic.FormattedStorage.TextGameOver, GameLogic.PosTextGameOver);
                        CTimer.SetTimer(
                            () =>
                            {
                                EndGameHandler.RunGameOver(this.playername, this.Score);
                                this.DisplayNotification(null, new Point(0, 0));
                            }, 5000);

                        this.endGameExplosion = CTimer.SetTimer(
                            () =>
                        {
                            this.playerHealths.Clear();
                            this.player.PlayerStatus = EntityStatus.Exploding;

                            // Copy the collection
                            ReadOnlyCollection<BitmapFrame> frames = ExplosionEffect.GifDecoder.Frames;
                            Thread th = new Thread(() =>
                            {
                                for (int i = 0; i < ExplosionEffect.Frames; i++)
                                {
                                    try
                                    {
                                        GameLogic.GameDispatcher.Invoke(() =>
                                        {
                                            if (ExplosionEffect.Frames >= i)
                                            {
                                                BitmapFrame xd = frames[i];
                                                this.player.Explosion = xd;
                                            }
                                        });
                                    }
                                    catch
                                    {
                                        // the thread was probably aborted
                                    }

                                    System.Threading.Thread.Sleep(100);
                                }

                                this.player.PlayerStatus = EntityStatus.FinishedExploding;
                            });
                            th.Start();
                        }, 1500);
                    }
                }

                try
                {
                    this.HandleInputKey();
                    this.MoveInvaders();
                    this.MoveBooster();
                }
                catch (Exception ex)
                {
                    // Any unexpected exceptions should be caught to keep the while loop running.
                    Debugger.Log(0, "info", "Caught an exception at render: " + ex);
                }

                Thread.Sleep(10);
            }
        }

        private void HandleInputKey()
        {
            if (this.Status == GameStatus.Started)
            {
                if (this.keypressed == Key.Left)
                {
                    if (this.player != null)
                    {
                        Point position = this.player.Position;
                        if (position.X > 20 && this.player.PlayerStatus == EntityStatus.Fine)
                        {
                            this.player.SetPosition(position.X - this.playermovement, position.Y);
                        }
                    }
                }
                else if (this.keypressed == Key.Right)
                {
                    if (this.player != null)
                    {
                        Point position = this.player.Position;
                        if (position.X < 615 && this.player.PlayerStatus == EntityStatus.Fine)
                        {
                            this.player.SetPosition(position.X + this.playermovement, position.Y);
                        }
                    }
                }

                if (this.firing)
                {
                    if (this.player != null && this.player.PlayerStatus == EntityStatus.Fine)
                    {
                        if (!this.superfiring && this.canfire)
                        {
                            this.canfire = false;
                            CTimer.SetTimer(() => { this.canfire = true; }, 650);

                            Point position = this.player.Position;
                            lock (this.MissileLocker)
                            {
                                int count = this.AllMissiles.Count + 1;
                                while (this.AllMissiles.ContainsKey(count))
                                {
                                    count++;
                                }

                                this.AllMissiles.Add(count, Creator.CreateMissile(MissileDirection.TowardsInvaderTeachers, MissileType.PlayerType, position));

                                if (this.doubletap)
                                {
                                    CTimer.SetTimer(
                                        () =>
                                    {
                                        int count2 = this.AllMissiles.Count + 1;
                                        while (this.AllMissiles.ContainsKey(count2))
                                        {
                                            count2++;
                                        }

                                        this.AllMissiles.Add(count2, Creator.CreateMissile(MissileDirection.TowardsInvaderTeachers, MissileType.PlayerType, position));
                                    }, 100);
                                }
                            }
                        }
                        else if (this.superfiring && this.canfire)
                        {
                            if (!this.canfire)
                            {
                                return;
                            }

                            this.canfire = false;
                            CTimer.SetTimer(() => { this.canfire = true; }, 200);

                            Point position = this.player.Position;
                            lock (this.MissileLocker)
                            {
                                int count = this.AllMissiles.Count + 1;
                                while (this.AllMissiles.ContainsKey(count))
                                {
                                    count++;
                                }

                                this.AllMissiles.Add(count, Creator.CreateMissile(MissileDirection.TowardsInvaderTeachers, MissileType.PlayerType, position));
                            }
                        }
                    }
                }
            }
        }

        private void MoveMissile(Missile missile)
        {
            Point position = missile.Position;
            if (missile.MissileDirection == MissileDirection.TowardsInvaderTeachers)
            {
                missile.SetPosition(position.X, position.Y - this.missilemovement);
            }
            else
            {
                missile.SetPosition(position.X, position.Y + (this.missilemovement / 2));
            }

            position = missile.Position;

            if (position.Y <= 0)
            {
                missile.Exploded = true;
                return;
            }

            if (position.Y > 504)
            {
                missile.Exploded = true;
                return;
            }

            if (missile.MissileDirection == MissileDirection.TowardsInvaderTeachers)
            {
                //Rect r = missile.Bounds;
                //r.Width = r.Width / 1.5;
                //r.Height = r.Height / 1.5;

                Rect r2 = new Rect(new Point(missile.Position.X, missile.Position.Y), new Size(15, 30));
                foreach (Teacher x in this.AllTeachers)
                {
                    if (r2.IntersectsWith(x.Bounds) && x.IsAlive)
                    {
                        x.IsAlive = false;
                        missile.Exploded = true;
                        this.Score += 10;
                        break;
                    }
                }

                if (this.szaboTeacher != null && this.AllTeachers.Count(o => o.IsAlive) == 0)
                {
                    if (r2.IntersectsWith(this.szaboTeacher.Bounds) && this.szaboTeacher.IsAlive)
                    {
                        missile.Exploded = true;
                        this.szaboTeacher.Damage();
                        this.Score += random.Next(20, 40);

                        if (this.szaboTeacher.Health <= 0 && this.szaboTeacher.InvaderStatus == EntityStatus.Fine)
                        {
                            this.DisplayNotification(GameLogic.FormattedStorage.TextGameWon, GameLogic.PosGameWon);
                            CTimer.SetTimer(
                                () =>
                            {
                                this.DisplayNotification(null, new Point(0, 0));
                                int sergyan = (int)Teachers.SergyanSz;
                                this.sergyanTeacher = new Sergyan(Images.ImageList[sergyan.ToString()], new Point(900, 250));
                            }, 4000);
                            this.szaboTeacher.IsAlive = false;
                            this.szaboTeacher.InvaderStatus = EntityStatus.Exploding;

                            // Copy the collection
                            ReadOnlyCollection<BitmapFrame> frames = ExplosionEffect.GifDecoder.Frames;
                            Thread th = new Thread(() =>
                            {
                                for (int i = 0; i < ExplosionEffect.Frames; i++)
                                {
                                    try
                                    {
                                        GameLogic.GameDispatcher.Invoke(() =>
                                        {
                                            if (ExplosionEffect.Frames >= i)
                                            {
                                                BitmapFrame xd = frames[i];
                                                this.szaboTeacher.Explosion = xd;
                                            }
                                        });
                                    }
                                    catch
                                    {
                                        // the thread was probably aborted
                                    }

                                    System.Threading.Thread.Sleep(100);
                                }

                                this.szaboTeacher.InvaderStatus = EntityStatus.FinishedExploding;
                            });
                            th.Start();
                        }
                    }
                }

                if (!missile.Exploded)
                {
                    lock (ShieldLocker)
                    {
                        Shield lastshield = null;
                        foreach (Shield x in this.Shields)
                        {
                            if (r2.IntersectsWith(x.Bounds) && x.IsAlive)
                            {
                                lastshield = x;
                            }
                        }

                        if (lastshield != null)
                        {
                            lastshield.IsAlive = false;
                            missile.Exploded = true;
                        }
                    }
                }
            }
            else
            {
                //Rect r = missile.Bounds;
                //r.Width = r.Width / 1.5;
                //r.Height = r.Height / 1.5;
                Rect r2 = new Rect(new Point(missile.Position.X, missile.Position.Y), new Size(15, 30));
                
                if (!missile.Exploded)
                {
                    lock (ShieldLocker)
                    {
                        foreach (Shield x in this.Shields)
                        {
                            if (r2.IntersectsWith(x.Bounds) && x.IsAlive)
                            {
                                x.IsAlive = false;
                                missile.Exploded = true;
                                break;
                            }
                        }
                    }
                }

                if (r2.IntersectsWith(this.player.Bounds) && !this.invincible && !this.respawninvincible && !missile.Exploded && this.Lives > 0 && this.Status != GameStatus.LevelCompleted)
                {
                    if (this.szaboTeacher != null && this.szaboTeacher.Health <= 0)
                    {
                        return;
                    }
                    
                    missile.Exploded = true;
                    if (this.playerHealths.Count > 1)
                    {
                        this.boosterCounter = 4500;
                        this.playerHealths.RemoveAt(this.playerHealths.Count - 1);
                        this.respawninvincible = true;
                        CTimer.SetTimer(() => { this.respawninvincible = false; }, 6500);
                        
                        if (boosterTimerC != null)
                        {
                            boosterTimerC.Kill();
                            boosterTimerC = null;
                        }
                        
                        boosterTimerC = CTimer.SetTimer(() =>
                        {
                            this.boosterCounter = this.boosterCounter - 500;
                        }, 500, 13);
                    }
                    else if (this.playerHealths.Count == 1)
                    {
                        if (this.Status != GameStatus.Stopped)
                        {
                            this.Status = GameStatus.Stopped;
                            this.playerHealths.RemoveAt(0);
                            this.DisplayNotification(GameLogic.FormattedStorage.TextGameOver, GameLogic.PosTextGameOver);
                            CTimer.SetTimer(
                                () =>
                            {
                                EndGameHandler.RunGameOver(this.playername, this.Score);
                                this.DisplayNotification(null, new Point(0, 0));
                            }, 2000);
                        }

                        return;
                    }

                    this.player.PlayerStatus = EntityStatus.Exploding;

                    // Copy the collection
                    ReadOnlyCollection<BitmapFrame> frames = ExplosionEffect.GifDecoder.Frames;
                    Thread th = new Thread(() =>
                    {
                        for (int i = 0; i < ExplosionEffect.Frames; i++)
                        {
                            try
                            {
                                GameLogic.GameDispatcher.Invoke(() =>
                                {
                                    if (ExplosionEffect.Frames >= i)
                                    {
                                        BitmapFrame xd = frames[i];
                                        this.player.Explosion = xd;
                                    }
                                });
                            }
                            catch
                            {
                                // the thread was probably aborted
                            }

                            System.Threading.Thread.Sleep(100);
                        }

                        this.player.SetPosition(320, 485);
                        this.player.PlayerStatus = EntityStatus.FinishedExploding;
                    });
                    th.Start();
                }
            }
        }

        /// <summary>
        /// Display a notification in the middle.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="p">Point.</param>
        private void DisplayNotification(FormattedText text, Point p)
        {
            this.middleFormattedText = text;
            this.middleFormattedPoint = p;
        }
    }
}