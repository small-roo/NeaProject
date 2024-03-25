using NeaProject.Engine;
using System.Diagnostics;
using System.Xml.Linq;

namespace NeaProject.Classes
{
    public class Game
    {
        public readonly Stopwatch GameStopwatch = new();
        public Map Map { get; set; } = new(); // needs the sets to deserialise properly
        public Camera Camera { get; set; } = new();
        public List<Npc> Npcs { get; set; } = new();
        public Player Player { get; set; }
        public int ScreenTileWidth { get; set; }
        public int ScreenTileHeight { get; set; }
        
        //a constructor which takes no parameters is needed for json deserialisation
        public Game()
        {
            Player = new() { Name = "", SpriteRef = '\0' };
            GameStopwatch = Stopwatch.StartNew();
        }
        public Game(string mapString)
        {
            Map = new Map(mapString);

            //default spawn position in case the map is missing the spawn tile
            int playerSpawnX = 20;
            int playerSpawnY = 20;

            //find the actual spawn position
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

            //default camera position - will be recalculated immediately if the window size doesn't match
            Camera.DrawingStartTileX = playerSpawnX - 6;
            Camera.DrawingStartTileY = playerSpawnY - 4;
            Player = new Player()
            {
                CurrentHp = 100,
                MaxHp = 100,
                XPos = playerSpawnX,
                YPos = playerSpawnY,
                Attack = 5,
                Defence = 5,
                Name = "Mellie",
                SpriteRef = 'p',
                AllowedTiles = new List<char> { 'g', 'ĝ', 'ğ', 'ġ', 'ģ', 'm', 's', 'w' } //all kinds of grass, mars rock, sand and water
            };
            SetUpNpcs(Map);
            GameStopwatch = Stopwatch.StartNew();
        }

        //creates a list of all npcs and places them on the map
        private void SetUpNpcs(Map map)
        {
            Npcs.Clear();
            Random random = new();

            //bird
            for (int birdNumber = 1; birdNumber <= 500; birdNumber++)
            {
                BirdEnemy bird = new()
                {
                    Name = $"bird{birdNumber}", //unique ID
                    SpriteRef = 'B', //character used to store them on the map
                    XPos = random.Next(0, map.Width),
                    YPos = random.Next(0, map.Height),
                    MaxHp = 20,
                    Attack = 10,
                    Defence = 5,
                    AllowedTiles = { 'g', 'ĝ', 'ğ', 'ġ', 'ģ' }, //can move to any grassy tile
                    FrameIndex = random.Next(0, 2) //spawns facing random direction
                };

                //ensures the birds do not overwrite any previously existing overlay tiles and is attempting to spawn on one of its allowed tiles
                while (map.GetOverlayTileChar(bird.XPos, bird.YPos) != '.' || bird.AllowedTiles.Contains(map.GetTileChar(bird.XPos, bird.YPos)) == false)
                {
                    bird.XPos = random.Next(0, map.Width);
                    bird.YPos = random.Next(0, map.Height);
                }
                map.SetOverlayTileChar(bird.XPos, bird.YPos, bird.SpriteRef);
                Npcs.Add(bird);
            }

            //final boss - note: not actually an enemy at the moment
            FinalBoss finalBoss = new() 
            {
                Name = "Mellow",
                SpriteRef = 'F',
                //set spawn point
                XPos = 35, 
                YPos = 29,
                MaxHp = 20000,
                Attack = 100,
                Defence = 100
            };
            map.SetOverlayTileChar(finalBoss.XPos, finalBoss.YPos, finalBoss.SpriteRef);
            Npcs.Add(finalBoss);

            //fish
            for (int fishNumber = 1; fishNumber <= 750; fishNumber++)
            {
                FishEnemy fish = new()
                {
                    Name = $"fish{fishNumber}",
                    SpriteRef = 'I',
                    XPos = random.Next(0, map.Width),
                    YPos = random.Next(0, map.Height),
                    MaxHp = 25,
                    Attack = 5,
                    Defence = 10,
                    FrameIndex = random.Next(0, 2),
                    AllowedTiles = { 'w' } //allowed in the water
                };
                while (map.GetOverlayTileChar(fish.XPos, fish.YPos) != '.' || map.GetTileChar(fish.XPos, fish.YPos) != 'w')
                {
                    fish.XPos = random.Next(0, map.Width);
                    fish.YPos = random.Next(0, map.Height);
                }
                map.SetOverlayTileChar(fish.XPos, fish.YPos, fish.SpriteRef);
                Npcs.Add(fish);
            }

            //snake
            for (int snakeNumber = 1; snakeNumber <= 250; snakeNumber++)
            {
                SnakeEnemy snake = new()
                {
                    Name = $"snake{snakeNumber}",
                    SpriteRef = 'S',
                    XPos = random.Next(0, map.Width),
                    YPos = random.Next(0, map.Height),
                    MaxHp = 30,
                    Attack = 10,
                    Defence = 15,
                    FrameIndex = random.Next(0, 2),
                    AllowedTiles = { 'm', 's' } //allowed on mars rock or sand
                };

                //only allowed to spawn on unoccupied mars rock
                while (map.GetOverlayTileChar(snake.XPos, snake.YPos) != '.' || map.GetTileChar(snake.XPos, snake.YPos) != 'm')
                {
                    snake.XPos = random.Next(0, map.Width);
                    snake.YPos = random.Next(0, map.Height);
                }
                map.SetOverlayTileChar(snake.XPos, snake.YPos, snake.SpriteRef);
                Npcs.Add(snake);
            }
            

            //added as otherwise their current hp is 0 and swinging the sword wipes every npc on the map from existence, rendering the game unbeatable
            foreach (Npc npc in Npcs)
            {
                npc.CurrentHp = npc.MaxHp;
            }
        }

        // Movement
        public void MoveUp()
        {
            //don't move if the game has ended
            if (Player.IsDead() || Player.HasWon() ) 
            {
                return;
            }
            Player.DirectionFacing = 'U'; //sets direction the player is looking
            Player.FrameIndex = 3; //switches to sprite looking away from camera
            Player.Move(0, -1, Map, Camera); //attempts to move
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

        // Fights
        public void PlayerEnemyCollision() //if the player walks into an NPC
        {
            if (Player.LookForFight == true) //if the NPC is an enemy
            {
                //calculate where the enemy is
                int enemyTileX = Player.XPos;
                int enemyTileY = Player.YPos;
                if (Player.DirectionFacing == 'U')
                {
                    enemyTileY -= 1;
                }
                else if (Player.DirectionFacing == 'R')
                {
                    enemyTileX += 1;
                }
                else if (Player.DirectionFacing == 'D')
                {
                    enemyTileY += 1;
                }
                else if (Player.DirectionFacing == 'L')
                {
                    enemyTileX -= 1;
                }

                //check if every npc is in front of the player, and if they are deal an appropriate amount of damage to both and stop looking
                foreach (Npc npc in Npcs)
                {
                    if (npc.XPos == enemyTileX && npc.YPos == enemyTileY) 
                    {
                        Player.CurrentHp -= (2 * npc.Attack / Player.Defence + 1);
                        npc.CurrentHp -= (2 * Player.Attack / npc.Defence + 1);
                        CheckForDeath(npc);
                        return;
                    }
                }
            }
        }

        //hitting the enemy with the sword
        public void SwordSweep() 
        {
            Player.SwingSword(); //sets a timer going to let the renderer know to draw the sword swing
            int enemyTileX = Player.XPos;
            int enemyTileY = Player.YPos;
            int[,] checkingTiles = new int[2, 3]; //(x, y) in 3 locations. [0, n] is the x co-ordinate and [1, n] the y co-ord for that tile
            
            //calculate where to check for enemies
            if (Player.DirectionFacing == 'U')
            {
                enemyTileY -= 1;
                for (int i = 0; i <= 2; i++)
                {
                    checkingTiles[0, i] = enemyTileX + i - 1; //left, mid, right
                    checkingTiles[1, i] = enemyTileY;
                }
            }
            else if (Player.DirectionFacing == 'R')
            {
                enemyTileX += 1;
                for (int i = 0; i <= 2; i++)
                {
                    checkingTiles[0, i] = enemyTileX;
                    checkingTiles[1, i] = enemyTileY + i - 1; //top, mid, base
                }
            }
            else if (Player.DirectionFacing == 'D')
            {
                enemyTileY += 1;
                for (int i = 0; i <= 2; i++)
                {
                    checkingTiles[0, i] = enemyTileX - i + 1; //right, mid, left
                    checkingTiles[1, i] = enemyTileY;
                }
            }
            else if (Player.DirectionFacing == 'L')
            {
                enemyTileX -= 1;
                for (int i = 0; i <= 2; i++)
                {
                    checkingTiles[0, i] = enemyTileX;
                    checkingTiles[1, i] = enemyTileY - i + 1; //base, mid, top
                }
            }

            //and then see if enemies reside there
            foreach (Npc npc in Npcs.ToList()) 
            {
                if (npc.XPos == checkingTiles[0, 0] && npc.YPos == checkingTiles[1, 0])
                { //tile location 1
                    npc.CurrentHp -= (5 * Player.Attack / npc.Defence + 3); //modifier on the end to add variation to sword sweep damage
                }
                else if (npc.XPos == checkingTiles[0, 1] && npc.YPos == checkingTiles[1, 1])
                { //tile location 2
                    npc.CurrentHp -= (5 * Player.Attack / npc.Defence + 5);
                }
                else if (npc.XPos == checkingTiles[0, 2] && npc.YPos == checkingTiles[1, 2])
                { //tile location 3
                    npc.CurrentHp -= (5 * Player.Attack / npc.Defence + 1);
                }
                //see if they died
                CheckForDeath(npc);
            }
        }

        private void CheckForDeath(Npc npc)
        {
            if (npc.IsDead())
            {
                Map.SetOverlayTileChar(npc.XPos, npc.YPos, '.'); //remove them from the map
                Npcs.Remove(npc); //remove them from the list of npcs
                //boost player's stats accordingly
                Player.MaxHp += npc.MaxHp / 10;
                Player.Attack += npc.Attack / 10;
                Player.Defence += npc.Defence / 10;
            }
        }
    }
}
