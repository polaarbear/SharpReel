
namespace SharpReelTest
{
    public class Tests
    {
        private SharpRod TestRod;
        [SetUp]
        public void BaitHookTest()
        {
            TestRod = new SharpRod("./stockfish/stockfish-windows-x86-64-avx2.exe");
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
    }
}