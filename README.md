# SharpReel
 This library employs the stockfish chess engine to view and analyze chess positions via C# calls
 
## Usage
Import the library into your project and add a reference to SharpReel to your imports.
A build of Stockfish and its source are present

Create an instance of the SharpRod class.

### Stockfish View Calls
SharpRod.ViewBoardPosition(); //Will display a new game board

SharpRod.ViewBoardPosition(string fen); //Will display a board based on a provided FEN string

SharpRod.ViewBoardPosition(List<string> moves); //Will display a board based on the list of moves (assumes a new board start)
