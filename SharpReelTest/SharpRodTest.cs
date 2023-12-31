
namespace SharpReelTest
{
    public class Tests
    {
        private SharpRod TestRod;
        [SetUp]
        public void BaitHookTest()
        {
            TestRod = new SharpRod("./stockfish-windows-x86-64-avx2.exe");
        }

        [Test]
        public async Task GoToPond()
        {

            bool waterIsFine = await TestRod.GoToPond();
            if (waterIsFine)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public async Task ViewFreshGame()
        {
            /* We know the board is being rendered if we have chars 1-8 and a-h.
             * This doesn't actually check that the positions are correct, but stockfish is reliable in the way we're calling it
             * We mainly just want to make sure we got a board output */
            string? newBoard = await TestRod.ViewBoardPosition();
            if (!string.IsNullOrWhiteSpace(newBoard) 
                && newBoard.Contains('a') && newBoard.Contains('b') && newBoard.Contains('c') && newBoard.Contains('d')
                && newBoard.Contains('e') && newBoard.Contains('f') && newBoard.Contains('g') && newBoard.Contains('h')
                && newBoard.Contains('1') && newBoard.Contains('2') && newBoard.Contains('3') && newBoard.Contains('4')
                && newBoard.Contains('5') && newBoard.Contains('6') && newBoard.Contains('7') && newBoard.Contains('8'))
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public async Task ViewMoveListGame()
        {
            List<string> moves = new() { "d2d4", "e7e5" };
            string? movedBoard = await TestRod.ViewBoardPosition(moves);
            if(!string.IsNullOrWhiteSpace(movedBoard))
            {
                string[] splitBoard = movedBoard.Split('\n');
                if (splitBoard[7][19] == 'p' && splitBoard[9][15] == 'P') //These are the positions of the two pawns that have moved
                    Assert.Pass();
            }
            Assert.Fail();
        }

        [Test]
        public async Task ViewFENGame()
        {
            string fen = "8/8/8/4k3/4P3/8/4K3/8 w - - 0 1";
            string? movedBoard = await TestRod.ViewBoardPosition(fen);
            if (!string.IsNullOrWhiteSpace(movedBoard))
            {
                string[] splitBoard = movedBoard.Split('\n');
                if (splitBoard[7][19] == 'k' && splitBoard[9][19] == 'P' && splitBoard[13][19] == 'K') //Three pieces in this endgame FEN
                    Assert.Pass();
            }
            Assert.Fail();
        }


        [Test]
        public async Task BestMoveNewGame()
        {
            string? bestMove = await TestRod.FindBestMove(1000);
            if (!string.IsNullOrWhiteSpace(bestMove))
                Assert.Pass();
            Assert.Fail();
        }

        [Test]
        public async Task BestMoveFEN()
        {
            string fen = "8/8/8/4k3/4P3/8/4K3/8 w - - 0 1";
            string? bestMove = await TestRod.FindBestMove(fen, 1000);
            if (bestMove != null && bestMove.Contains("bestmove") && bestMove.Contains("ponder"))
                Assert.Pass();
            Assert.Fail();
        }

        [Test]
        public async Task BestMoveList()
        {
            List<string> moves = new() { "d2d4", "e7e5" };
            string? bestMove = await TestRod.FindBestMove(moves, 1000);
            if (bestMove != null && bestMove.Contains("bestmove") && bestMove.Contains("ponder"))
                Assert.Pass();
            Assert.Fail();
        }
    }
}