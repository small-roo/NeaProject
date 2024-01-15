using NeaProject.Engine;
using System.Xml.Linq;

namespace NeaProject.Classes
{
    public class Game
    {
        public Map Map { get; set; } = new(); // needs the sets to deserialise properly
        public Camera Camera { get; set; } = new();
        public List<Npc> Npcs { get; set; } = new();
        public Player Player { get; set; }
        public int ScreenTileWidth { get; set; }
        public int ScreenTileHeight { get; set; }

        public Game()
        {
            Player = new() { Name = "", SpriteRef = '\0' };
        }
        public Game(string mapString)
        {
            Map = new Map(mapString);
            int playerSpawnX=20;
            int playerSpawnY=20;
            int charX = 0;
            int charY = 0;
            foreach (char[] charRow in Map.OverlayCharMap)
            {
                foreach (char tile in Map.OverlayCharMap[charY])
                {
                    if (tile == '0')
                    {
                        playerSpawnX = charX;
                        playerSpawnY = charY;
                    }
                    charX++;
                }
                charX = 0;
                charY++;
            }
            Camera.DrawingStartTileX = playerSpawnX - 6;
            Camera.DrawingStartTileY = playerSpawnY - 4;
            Player = new Player()
            {
                CurrentHp = 100,
                // XPos = _map.Width / 2,
                // YPos = _map.Height / 2,
                XPos = playerSpawnX,
                YPos = playerSpawnY,
                Name = "Mellie",
                SpriteRef = 'p',
                AllowedTiles = new List<char> { 'g', 'ĝ', 'ğ', 'ġ', 'ģ', 'm', 's', 'w' }
            };
            SetUpNpcs(Map);
        }

        private void SetUpNpcs(Map map)
        {
            Npcs.Clear();
            Random random = new();

            for (int birdNumber = 1; birdNumber <= 20; birdNumber++) //bird
            {
                BirdEnemy bird = new()
                {
                    Name = $"bird{birdNumber}",
                    SpriteRef = 'B',
                    XPos = random.Next(0, map.Width),
                    YPos = random.Next(0, map.Height),
                    AllowedTiles = { 'g', 'ĝ', 'ğ', 'ġ', 'ģ' },
                    FrameIndex = random.Next(0, 2),
                };
                while (map.GetOverlayTileChar(bird.XPos, bird.YPos) != '.' || bird.AllowedTiles.Contains(map.GetTileChar(bird.XPos, bird.YPos)) == false)
                {
                    bird.XPos = random.Next(0, map.Width);
                    bird.YPos = random.Next(0, map.Height);
                }
                map.SetOverlayTileChar(bird.XPos, bird.YPos, bird.SpriteRef);
                Npcs.Add(bird);
            }

            FinalBoss finalBoss = new() //final boss
            {
                Name = "Mellow",
                SpriteRef = 'F',
                XPos = 29,
                YPos = 13
            };

            for (int fishNumber = 1; fishNumber <= 30; fishNumber++) //fish
            {
                FishEnemy fish = new()
                {
                    Name = $"fish{fishNumber}",
                    SpriteRef = 'I',
                    XPos = random.Next(0, map.Width),
                    YPos = random.Next(0, map.Height),
                    FrameIndex = random.Next(0, 2),
                    AllowedTiles = { 'w' }
                };
                while (map.GetOverlayTileChar(fish.XPos, fish.YPos) != '.' || map.GetTileChar(fish.XPos, fish.YPos) != 'w')
                {
                    fish.XPos = random.Next(0, map.Width);
                    fish.YPos = random.Next(0, map.Height);
                }
                map.SetOverlayTileChar(fish.XPos, fish.YPos, fish.SpriteRef);
                Npcs.Add(fish);
            }

            for (int snakeNumber = 1; snakeNumber <= 10; snakeNumber++) //snake
            {
                SnakeEnemy snake = new()
                {
                    Name = $"snake{snakeNumber}",
                    SpriteRef = 'S',
                    XPos = random.Next(0, map.Width),
                    YPos = random.Next(0, map.Height),
                    FrameIndex = random.Next(0, 2),
                    AllowedTiles = { 'm', 's' }
                };
                while (map.GetOverlayTileChar(snake.XPos, snake.YPos) != '.' || map.GetTileChar(snake.XPos, snake.YPos) != 'm')
                {
                    snake.XPos = random.Next(0, map.Width);
                    snake.YPos = random.Next(0, map.Height);
                }
                map.SetOverlayTileChar(snake.XPos, snake.YPos, snake.SpriteRef);
                Npcs.Add(snake);
            }
            map.SetOverlayTileChar(finalBoss.XPos, finalBoss.YPos, finalBoss.SpriteRef);
            Npcs.Add(finalBoss);
        }

        // Movement
        public void MoveUp()
        {
            if (Player.IsDead() || Player.HasWon() )
            {
                return;
            }
            Player.DirectionFacing = 'U';
            Player.FrameIndex = 3;
            Player.Move(0, -1, Map, Camera);
            
        }
        public void MoveRight()
        {
            if (Player.IsDead() || Player.HasWon())
            {
                return;
            }
            Player.DirectionFacing = 'R';
            Player.FrameIndex = 1;
            Player.Move(1, 0, Map, Camera);
        }
        public void MoveDown()
        {
            if (Player.IsDead() || Player.HasWon())
            {
                return;
            }
            Player.DirectionFacing = 'D';
            Player.FrameIndex = 0;
            Player.Move(0, 1, Map, Camera);
        }
        public void MoveLeft()
        {
            if (Player.IsDead() || Player.HasWon())
            {
                return;
            }
            Player.DirectionFacing = 'L';
            Player.FrameIndex = 2;
            Player.Move(-1, 0, Map, Camera);
        }
    }
}
