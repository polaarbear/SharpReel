using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpReel
{
    public class SharpRod
    {
        //Determines whether the stockfish process is alive
        private bool isCast { get; set; } = false;
        private Process? catchInProgress { get; set; }
        private StreamWriter? pondInput { get; set; }
        private StreamReader? pondOutput { get; set; }
        private StreamReader? pondError { get; set; }
        private readonly string pondLocation = "./stockfish/stockfish-windows-x86-64-avx2.exe";

        #region Generic Stockfish Write/Read

        //Writes a command to stockfish input which will then fill the output buffer
        private async Task CastLine(string bait)
        {
            await pondInput!.WriteLineAsync(bait);
        }

        //Reads from stockfish output buffer. Stops when it reaches the reelKey to prevent mystery hang?
        private async Task<string> ReelIn(string reelKey)
        {
            StringBuilder catchOfTheDay = new();
            string? bite = null;
            while ((bite = await pondOutput!.ReadLineAsync()) != null)
            {
                catchOfTheDay.Append(bite + "\n");
                if (bite.Contains(reelKey))
                    break;
            }
            return catchOfTheDay.ToString();
        }

        #endregion

        #region Stockfish Process Management

        public async Task<bool> GoToPond()
        {
            if (catchInProgress == null)
                FindPond();
            if (catchInProgress != null)
            {
                string? ok = null;
                bool stocked = false;
                bool locked = false;
                ok = await ReelIn("Stockfish");
                if (ok.Contains("Stockfish"))
                    stocked = true;
                if (stocked)
                {
                    await CastLine("uci");
                    ok = await ReelIn("uciok");
                    if (ok.Contains("uciok"))
                    {
                        locked = true;
                    }
                }
                if (locked)
                {
                    await CastLine("isready");
                    ok = await ReelIn("readyok");
                    if (ok == "readyok\n")
                        return true;
                }
            }
            return false;
        }

        private void FindPond()
        {
            catchInProgress = new()
            {
                StartInfo = TestWater()
            };
            catchInProgress.Start();
            pondInput = catchInProgress.StandardInput;
            pondOutput = catchInProgress.StandardOutput;
            pondError = catchInProgress.StandardError;
        }

        private ProcessStartInfo TestWater()
        {
            ProcessStartInfo pondInfo = new();
            pondInfo.FileName = pondLocation;
            pondInfo.RedirectStandardError = true;
            pondInfo.RedirectStandardInput = true;
            pondInfo.RedirectStandardOutput = true;
            return pondInfo;
        }

        private async void LeavePond()
        {
            if(catchInProgress != null)
            {
                await pondInput!.WriteLineAsync("quit");
                await Task.Delay(1000); //Give stockfish a change to quit
                if(!catchInProgress.HasExited)
                {
                    catchInProgress.Kill();
                    catchInProgress = null;
                }
            }
        }

        #endregion

        #region View Board Positions

        //View an empty board
        public async Task<string?> ViewBoardPosition()
        {
            if (await GoToPond())
            {
                await pondInput!.WriteLineAsync("position startpos");
                string ourCatch = await RenderBoardPosition();
                LeavePond();
                return ourCatch;
            }
            return null;
        }

        //View the board based on a given FEN string
        public async Task<string?> ViewBoardPosition(string fen)
        {
            if (await GoToPond())
            {
                await pondInput!.WriteLineAsync($"position fen {fen}");
                string ourCatch = await RenderBoardPosition();
                LeavePond();
                return ourCatch;
            }
            return null;
        }

        //View the board based on a given move list
        public async Task<string?> ViewBoardPosition(List<string> moveList)
        {
            if (await GoToPond())
            {
                await pondInput!.WriteLineAsync("position startpos move " + string.Join(' ', moveList));
                string ourCatch = await RenderBoardPosition();
                LeavePond();
                return ourCatch;
            }
            return null;
        }

        /* This method uses stockfish's built in way to render a board state
         * This will be very useful for testing before the proper UI can be built
         * */
        private async Task<string> RenderBoardPosition()
        {
            StringBuilder boardBuilder = new();
            await catchInProgress!.StandardInput.WriteLineAsync("d");
            string dirtyBoardString = await ReelIn("Checkers");
            string[] dirtyBoardSplit = dirtyBoardString.Split("\n");
            foreach (var row in dirtyBoardSplit)
            {
                if (IsBoardRow(row))
                {
                    boardBuilder.Append(row + "\n");
                }
            }
            return boardBuilder.ToString();
        }

        //This helper isolates the chess rows from the rest of the stockfish board output
        private bool IsBoardRow(string row)
        {
            if (row.Contains('|') || row.Contains('+') //The actual board rows always contain this, and none of the other lines do
                || (row.Contains('a') && row.Contains('b') && row.Contains('c') && row.Contains('d') //This row always contains all 8 characters
                && row.Contains('e') && row.Contains('f') && row.Contains('g') && row.Contains('g'))) //We check them all just to avoid FEN string and other conflicts
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
