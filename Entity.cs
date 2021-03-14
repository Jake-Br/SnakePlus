using System;
using System.Collections.Generic;

namespace SnakePlus {
    abstract class Entity {
        private short[] location = new short[2];
        private char symbol;
        private ConsoleColor color;

        public void DrawEntity() {
            ConsoleColor temp = Console.ForegroundColor; // store current color

            Console.SetCursorPosition(location[0], location[1]);
            Console.ForegroundColor = color;
            Console.Write(symbol);

            Console.ForegroundColor = temp; // set original color back
        }
        public void RemoveEntity() { // remove the entity from the console by drawing over it with whitespace
            Console.SetCursorPosition(location[0], location[1]);
            Console.Write(' ');
        }

        public void SetX(short x) {
            location[0] = x;
        }
        public void SetY(short y) {
            location[1] = y;
        }
        public void SetSymbol(char symbol) {
            this.symbol = symbol;
        }
        public void SetColor(ConsoleColor color) {
            this.color = color;
        }
        public short GetX() {
            return location[0];
        }
        public short GetY() {
            return location[1];
        }
        public char GetSymbol() {
            return symbol;
        }
        public ConsoleColor GetColor() {
            return color;
        }
    }

    class Snake : Entity {
        private Queue<Tail> tailQueue;
        private short[] direction;

        public Snake(short x, short y) {
            SetX(x);
            SetY(y);
            SetSymbol('O');
            SetColor(ConsoleColor.Yellow);
            tailQueue = new Queue<Tail>();
            direction = new short[] { 0, 0 };
        }

        public void SetDirection(short x, short y) {
            direction[0] = x;
            direction[1] = y;
        }

        public short[] GetDirection() {
            return direction;
        }
        public Queue<Tail> GetTail() {
            return tailQueue;
        }
        public void Move() {
            SetX((short)(GetX() + direction[0]));
            SetY((short)(GetY() + direction[1]));

            DrawEntity();
        }

        public void MoveTail() {
            if (tailQueue.Count == 0)
                return;

            short tempX = -1, tempY = -1;
            tailQueue.Peek().SetX(GetX());
            tailQueue.Peek().SetY(GetY());

            foreach(Tail tail in tailQueue) { // move each tail in the queue
                if (tempX == -1) { // if the first tail
                    tempX = tailQueue.Peek().GetX();
                    tempY = tailQueue.Peek().GetY();
                    continue;
                }

                short x = tempX, y = tempY; // remember the old location
                tempX = tail.GetX();
                tempY = tail.GetY();

                // set the new location
                tail.SetX(x);
                tail.SetY(y);

            }
        }

        public new void DrawEntity() {
            Console.SetCursorPosition(GetX(), GetY()); // print head
            Console.Write(GetSymbol());

            foreach(Tail tail in tailQueue) {
                tail.DrawEntity();
            }        
        }

        public new void RemoveEntity() {
            Console.SetCursorPosition(GetX(), GetY()); // remove head
            Console.Write(' ');

            foreach (Tail tail in tailQueue) {
                tail.RemoveEntity();
            }
        }

        public void AddTail(short x, short y) { // add tail at end of snake
            tailQueue.Enqueue(new Tail(x, y));
        }

        public void RemoveTail() { // removes the tail at the end of the snake
            if (tailQueue.Count == 1) { // if only one tail just dequeue it and early return
                tailQueue.Dequeue();
                return;
            }

            Queue<Tail> copyQueue = new Queue<Tail>();

            while (tailQueue.Count > 0) { // copy the tail queue
                copyQueue.Enqueue(tailQueue.Peek());
                tailQueue.Dequeue();
            }

            while (copyQueue.Count > 1) { // copy back all but the last tail
                tailQueue.Enqueue(copyQueue.Peek());
                copyQueue.Dequeue();
            }

            copyQueue.Peek().RemoveEntity(); // remove the last entity
        }
    }

    class Tail : Entity {
        public Tail(short x, short y) {
            SetX(x);
            SetY(y);
            SetSymbol('#');
            SetColor(ConsoleColor.Yellow);
        }
    }

    class Fruit : Entity {
        public Fruit(short x, short y) {
            SetX(x);
            SetY(y);
            SetSymbol('@');
            SetColor(ConsoleColor.Red);
        }
    }

    class SpikyFruit : Entity {
        public SpikyFruit(short x, short y) {
            SetX(x);
            SetY(y);
            SetSymbol('@');
            SetColor(ConsoleColor.Cyan);
        }
    }

    class Bomb : Entity {
        public Bomb(short x, short y) {
            SetX(x);
            SetY(y);
            SetSymbol('@');
            SetColor(ConsoleColor.Blue);
        }
    }

    class SpeedBoost : Entity {
        public SpeedBoost(short x, short y) {
            SetX(x);
            SetY(y);
            SetSymbol('@');
            SetColor(ConsoleColor.Magenta);
        }
    }
}
