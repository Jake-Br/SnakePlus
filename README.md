# SnakePlus

Reimagined version of the classic game Snake with new entities and game aim.
Instead of collecting as many fruit as possible with a pre-determined limit, the
aim of this version is to stay alive while either extending/shortening the snake
while avoiding bombs that will kill the snake just like running into the wall,
or itself. There is also speed boosts that occassionally spawn to create more
challenge.

Design:
All game mechanics and physics are handled by the GameEngine, while all "actors"
are Entities. The snake, tail, fruits, and bomb are all entities, contained inside
the GameEngine.

Fruit: adds 1 tail to the snake
Spiky Fruit: removes 1 tail to the snake
Speed Boost: snake gains 2x speed
Bomb: kills snake on collision

# Controls

Arrow Keys: move the snake
Q - quit the game