using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SnakePlus {
    class GameEngine {
        private short ROWS = 24, COLUMNS = 64;
        private Snake snake;
        private ArrayList entities;
        private System.Diagnostics.Stopwatch timer;

        public GameEngine() {
            entities = new ArrayList();
        }

        public void initialize() {
            Console.Title = "Snake+";
            Console.SetWindowSize(64, 24);
            Console.BufferHeight = 24;
            Console.BufferWidth = 64;
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Yellow;
            snake = new Snake((short)(COLUMNS / 2), (short)(ROWS / 2));
            entities.Add(GenFruit());
        }

        public void drawBorder() {
            for (int i = 0; i < COLUMNS; i++) { // top border
                Console.Write("=");
            }
            for (int i = 1; i < ROWS; i++) {
                Console.SetCursorPosition(0, i); // left-border
                Console.Write("=");
                Console.SetCursorPosition(COLUMNS-1, i); // right-border
                Console.Write("=");
            }
            for (int i = 0; i < COLUMNS; i++) { // bottom border
                Console.SetCursorPosition(i, ROWS-1);
                Console.Write("=");
            }
        }

        public void GameOver() {
            Console.Clear();

            Console.SetCursorPosition(COLUMNS / 4, ROWS / 3);
            Console.Write($"Time survived = {timer.ElapsedMilliseconds / 1000} seconds");

            for (int i = 1; i <= 10; i++)
                Console.WriteLine();
        }

        public Entity GenFruit(bool isFruit = true) {
            Random r = new Random();
            short x, y;

            while (true) { // make sure nothing is covering it
                bool entityClear = true, snakeClear = true, tailClear = true;

                x = (short)(r.Next(1, COLUMNS - 1));
                y = (short)(r.Next(1, ROWS - 1));

                foreach(Entity entity in entities) { // check all entities
                    if (entity.GetX() == x && entity.GetY() == y) {
                        entityClear = false;
                        break;
                    }
                }
                if (snake.GetX() == x && snake.GetY() == y) { // check snake head
                    snakeClear = false;
                }
                foreach (Tail tail in snake.GetTail()) { // check tail queue
                    if (tail.GetX() == x && tail.GetY() == y) {
                        tailClear = false;
                        break;
                    }
                }

                if (entityClear && snakeClear && tailClear) // if spot is free create new fruit
                    break;
            }

            // if creating a bomb create bomb
            if (!isFruit)
                return new Bomb(x, y);

            // what type of fruit to create
            int fruitNum = r.Next(0, 100);

            if (fruitNum < 5) { // 5% chance to create speedBoost
                return new SpeedBoost(x, y);
            }
            else if (fruitNum < 35) { // 20% chance to create spikyFruit
                return new SpikyFruit(x, y);
            } 
            else { // 75% chance to create normal fruit
                return new Fruit(x, y);
            }

            
        }

        public void start() {
            initialize();
            drawBorder();
            snake.DrawEntity();

            bool fatalHit = false, fruitHit = false;
            int speedInMS = 100, fruitCollisions = 0;

            while (true) {
                snake.RemoveEntity();
                fruitHit = false;

                if (Console.KeyAvailable) {
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    if (timer == null) {
                        timer = new System.Diagnostics.Stopwatch();
                        timer.Start();
                    }

                    if (keyInfo.Key == ConsoleKey.Q) // quit game
                        break;
                    if (keyInfo.Key == ConsoleKey.LeftArrow) // move left
                        snake.SetDirection(-1, 0);
                    if (keyInfo.Key == ConsoleKey.RightArrow) // move right
                        snake.SetDirection(1, 0);
                    if (keyInfo.Key == ConsoleKey.UpArrow) // move up
                        snake.SetDirection(0, -1);
                    if (keyInfo.Key == ConsoleKey.DownArrow) // move down
                        snake.SetDirection(0, 1);                                  
                }

                // get the posistion of the head after it moves
                short tempX = (short)(snake.GetX() + snake.GetDirection()[0]);
                short tempY = (short)(snake.GetY() + snake.GetDirection()[1]);

                // check for collisions with the walls
                if ((tempX == 0 || tempX == COLUMNS - 1) || (tempY == 0 || tempY == ROWS - 1))
                    break;

                // check for collisions with other entities
                for(int i =0; i < entities.Count; i++) {
                    Entity entity = (Entity)entities[i];                   

                    if (tempX == entity.GetX() && tempY == entity.GetY()) { // handle collision
                        // add new fruit
                        entities.Add(GenFruit());

                        // remove entity
                        entity.RemoveEntity();
                        entities.RemoveAt(i);
                        i--;

                        // increment fruitCollisions
                        fruitCollisions++;
                        if (fruitCollisions == 5) {
                            fruitCollisions = 0;
                            entities.Add(GenFruit(false));
                        }

                        if (entity is Fruit) { // add to tail                           
                            snake.AddTail((short)(snake.GetX() - snake.GetDirection()[0]), (short)(snake.GetY() - snake.GetDirection()[1]));                         
                            fruitHit = true;
                            break;
                        }
                        else if (entity is SpikyFruit) { // remove tail if possible
                            if (snake.GetTail().Count > 1)
                                snake.RemoveTail();
                        }
                        else if (entity is Bomb) { // kill game
                            fatalHit = true;
                            break;                           
                        }
                        else if (entity is SpeedBoost) { // speed up snake
                            speedInMS /= 2;
                        }                       
                    }
                }         
                foreach(Tail tail in snake.GetTail()) { // check for collisions with tail
                    if (snake.GetX() == tail.GetX() && snake.GetY() == tail.GetY()) {
                        fatalHit = true;
                        break;
                    }
                }

                if (fatalHit) // if hit something fatal end game loop
                    break;

                if (!fruitHit) { // when hit a fruit move the tail differently
                    snake.MoveTail();
                }

                // move the snake               
                snake.Move();

                // print entities
                foreach(Entity entity in entities) {
                    entity.DrawEntity();
                }

                // pause the thread for 2/5 of a second
                Thread.Sleep(speedInMS);
            }

            GameOver();
        }
    }
}
