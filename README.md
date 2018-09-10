# Pan’s Labyrinth

### Background
Labyrinths have taken on a number of meanings throughout history – e.g. devotion, power, games. The first recorded labyrinth dates back to Egypt at the time of king Amenemhet 1842 - 1797 BCE. In my work I was particularly drawn to garden mazes. While the devotional varieties of such mazes have proliferated since the 13th century, secular garden mazes initially appeared as derivative of small artificial hills during the Renaissance and gaining greater popularity in 15th Century France. Often elaborate in their nature, they espoused equal portions of fun and complexity. <br />
 
Developed in Unity, using C#, Pan’s Labyrinth merges the histrorical context of a labyrinth with the simplicity of children's games. 
<br /> <br />

### Technical notes
Using object oriented programming, I create a maze object containing cells, each with three open walls, and one wall that is closed off. The cells also have an indication of the four neighbors and whether passage to them is opened or closed. <br />

Once the maze object is generated it becomes possible to assess which cells are connected. To do so, I pick a random first cell and examine its neighbors, then its neighbors' neighbors, etc. with -1 serving as an indicator of inaccessibility. I then apply a variation of [Dijkstra's algorithm](https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm) to find the shortest path from the first cell to the one farthest away (i.e. the cell with the highest cost).  <br /> <br />

### Trailer
Here is a trailer featuring the final game play: ** <br />
[![video](https://i.vimeocdn.com/video/672009081.jpg)](https://vimeo.com/246910954)
**_Note: the C# script corresponds to an earlier, simpler version of the game_
