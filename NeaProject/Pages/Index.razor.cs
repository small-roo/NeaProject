using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using NeaProject.Classes;
using NeaProject.Engine;
using SkiaSharp.Views.Blazor;
using SkiaSharp;
using Blazored.LocalStorage;
using System.Diagnostics;

namespace NeaProject.Pages
{
    public partial class Index
    {
        [Inject] NavigationManager? NavigationManager { get; set; }
        [Inject] ILocalStorageService? LocalStorage { get; set; }

        private Game? _game;
        private Npc? _talkingToNpc;
        private Dictionary<char, Sprite?>? _sprites;
        private FpsCounter? _fpsCounter;
        private Renderer? _renderer;
        private SKBitmap? _bitmap;
        private ElementReference buttonRef;
        private SKCanvasView? _skCanvasView;
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        //store everything in the game object in local storage
        private async Task SaveDataAsync()
        {
            if (LocalStorage == null)
            { return; }
            await LocalStorage.SetItemAsync("game", _game); 
            await buttonRef.FocusAsync(); //return focus to the game's screen
        }

        //delete the game from local storage
        private async Task ClearDataAsync()
        {
            if (LocalStorage == null)
            { return; }
            await LocalStorage.RemoveItemAsync("game");
            await buttonRef.FocusAsync();
        }

        private void KeyDown(KeyboardEventArgs keyEvent)
        {
            Move(keyEvent.Key);
        }

        private void Move(string pressedKey)
        {
            //limited input to stop character from moving multiple times per key press
            if (_game == null || _renderer == null || _stopwatch.ElapsedMilliseconds < 80)
            { return; }
            _stopwatch.Restart();

            //either arrow key or WASD inputs allowed, and caps lock won't throw things off
            pressedKey = pressedKey.ToLower();
            switch (pressedKey)
            {
                case "arrowup":
                case "w":
                    {
                        _game.MoveUp();
                        break;
                    }
                case "arrowdown":
                case "s":
                    {
                        _game.MoveDown();
                        break;
                    }
                case "arrowleft":
                case "a":
                    {
                        _game.MoveLeft();
                        break;
                    }
                case "arrowright":
                case "d":
                    {
                        _game.MoveRight();
                        break;
                    }
                case ".":
                    {
                        if (_game.Player.Inventory.Contains("Sword"))
                        {
                            _game.SwordSweep();
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }

            }
            _renderer.MoveCamera(_game);
            switch (pressedKey)
            {
                //if just moved, check to see if collided w/ enemy.
                case "arrowup":
                case "arrowdown":
                case "arrowleft":
                case "arrowright":
                case "w":
                case "s":
                case "a":
                case "d":
                    {
                        _game.PlayerEnemyCollision();
                        break;
                    }
                default:
                    {
                        break; 
                    }
        }
        }

        //when the page loads
        protected override async Task OnInitializedAsync()
        {
            if (LocalStorage == null||NavigationManager == null)
            { return; }

            //grabs the tilesheet. the section after the ? forces the game to use the latest copy, not the one stored in cache memory
            var tileSheetUri = new Uri($"{NavigationManager.Uri}images/MapTiles/all_tiles.png?_={DateTime.Now}");
            
            //either loads the saved game or begins a new one
            if (await LocalStorage.ContainKeyAsync("game"))
            {
                _game = await LocalStorage.GetItemAsync<Game>("game");
            }
            else
            {
                //uses map_0.txt to create a new game
                var mapUri = new Uri($"{NavigationManager.Uri}map-data/map_0.txt?_={DateTime.Now}");
                string mapString = await DownloadAsync(mapUri);
                _game = new Game(mapString);
            }

            ImageLoader imageLoader = new(tileSheetUri);
            SKBitmap mapTileSheet = await imageLoader.GetBitmapAsync();
            _sprites = new Dictionary<char, Sprite?>()
            {
                //teleporters
                { '0', new Sprite(mapTileSheet, "Spawn", 1)},
                { '1', new Sprite(mapTileSheet, "Teleport1", 1)},
                { '2', new Sprite(mapTileSheet, "Teleport2", 1)},
                { '3', new Sprite(mapTileSheet, "Teleport3", 1)},
                { '4', new Sprite(mapTileSheet, "Teleport4", 1)},

                //various cacti
                { 'c', new Sprite(mapTileSheet, "Cactus", 1)},
                { 'ç', new Sprite(mapTileSheet, "Cactus1", 1)},
                { 'ć', new Sprite(mapTileSheet, "Cactus2", 1)},
                { 'ĉ', new Sprite(mapTileSheet, "Cactus3", 1)},
                { 'č', new Sprite(mapTileSheet, "Cactus4", 1)},

                //grass
                { 'g', new Sprite(mapTileSheet, "Grass", 1)},
                { 'ĝ', new Sprite(mapTileSheet, "Grass1", 1)},
                { 'ğ', new Sprite(mapTileSheet, "Grass2", 1)},
                { 'ġ', new Sprite(mapTileSheet, "Grass3", 1)},
                { 'ģ', new Sprite(mapTileSheet, "Grass4", 1)},

                //npcs - described by a capital
                { 'B', new Sprite(mapTileSheet, "Bird", 2)},
                { 'F', new Sprite(mapTileSheet, "FinalBoss", 4)},
                { 'I', new Sprite(mapTileSheet, "Fish", 2)},
                { 'S', new Sprite(mapTileSheet, "Snake", 2) },

                //weapons
                { '△', new Sprite(mapTileSheet, "SwingUp", 3)},
                { '▷', new Sprite(mapTileSheet, "SwingRight", 3)},
                { '▽', new Sprite(mapTileSheet, "SwingDown", 3)},
                { '◁', new Sprite(mapTileSheet, "SwingLeft", 3)},

                //misc other
                { '.', null},
                { '□', new Sprite(mapTileSheet, "Space", 1)},
                { 'd', new Sprite(mapTileSheet, "Sword", 1)},
                { 'm', new Sprite(mapTileSheet, "MarsRock", 1)},
                { 'p', new Sprite(mapTileSheet, "Player", 21)},
                { 'r', new Sprite(mapTileSheet, "Rock", 1)},
                { 's', new Sprite(mapTileSheet, "Sand", 1)},
                { 't', new Sprite(mapTileSheet, "Tree", 1)},
                { 'w', new Sprite(mapTileSheet, "Water", 1)}
            };

            _fpsCounter = new FpsCounter();
            _renderer = new Renderer(_game, _sprites);
            _bitmap = new SKBitmap(_renderer.ViewportWidth, _renderer.ViewportHeight);
        }

        //immediately focus on canvas so the player doesn't need to click it to begin
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await buttonRef.FocusAsync();
            }
        }

        // not my code, for the most part. comes from this project: 
        // https://github.com/JamesRandall/csharp-wolfenstein/tree/b1a34dd03730f71754ae9d04e42851b0c4c03c2d
        // also commented out drawing the fps for the actual final tag, but didn't delete it
        protected void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            if (_renderer == null || _bitmap == null || _fpsCounter == null || _game == null)
            {
                return;
            }
            // There is a good article on the different ways to update pixel data here:
            //  https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/pixel-bits
            // Having tried them all maintaining and then setting the pixel byte array is the most performant for us.

            if (e.Info.Width != _bitmap.Width || e.Info.Height != _bitmap.Height)
            {
                _bitmap = new SKBitmap(_renderer.ViewportWidth, _renderer.ViewportHeight);
            }
            unsafe
            {
                fixed (uint* ptr = _renderer.UpdateFrameBuffer(_game, e.Info.Width, e.Info.Height))
                {
                    _bitmap.SetPixels((IntPtr)ptr);
                        
                }
            }

            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.White);
            canvas.DrawBitmap(_bitmap, new SKRect(0, 0, _renderer.ViewportWidth * 1, _renderer.ViewportHeight * 1));
            var fps = _fpsCounter.GetCurrentFps();
            using var paint = new SKPaint
            {
                IsAntialias = true,
                StrokeWidth = 5f,
                StrokeCap = SKStrokeCap.Round,
                TextAlign = SKTextAlign.Center,
                TextSize = 24,
            };

            var surfaceSize = e.Info.Size;
            //canvas.DrawText($"{fps:0.00}fps", surfaceSize.Width / 2, surfaceSize.Height - 10f, paint);
        }

        public async static Task<string> DownloadAsync(Uri uri)
        {
            using var client = new HttpClient();
            var content = await client.GetStringAsync(uri);
            return content;
        }
    }
}
