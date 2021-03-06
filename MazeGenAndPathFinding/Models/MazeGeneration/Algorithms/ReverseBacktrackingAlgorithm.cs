﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MazeGenAndPathFinding.Models.MazeGeneration.Algorithms
{
    public class ReverseBacktrackingAlgorithm : MazeGenerationAlgorithmBase
    {
        #region Fields

        private readonly Stack<Cell> _currentChain = new Stack<Cell>();
        private readonly HashSet<Cell> _visitedCells = new HashSet<Cell>();
        private Cell _currentCell;

        #endregion

        #region Constructor

        public ReverseBacktrackingAlgorithm()
        {
            Name = "Reverse Backtracking";
        }

        #endregion

        #region Methods
        
        public override void Reset()
        {
            base.Reset();
            _currentChain.Clear();
            _visitedCells.Clear();
        }

        public override void Step()
        {
            if (!IsInitialized)
            {
                Initialize();
                return;
            }

            Step(GetValidNeighboringCells(_currentCell));
        }
        
        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            if (!IsInitialized)
            {
                Initialize();
            }

            var neighboringCells = GetValidNeighboringCells(_currentCell);
            var initialValue = neighboringCells.Any();
            do
            {
                Step(neighboringCells);
                neighboringCells = _currentCell.NeighboringCells
                    .Where(x => !_visitedCells.Contains(x.Value))
                    .ToList();
                await Task.Delay(25, cancellationToken);
            } while (neighboringCells.Any() == initialValue && !IsComplete);
        }

        private void Step(IReadOnlyCollection<KeyValuePair<Direction, Cell>> neighboringCells)
        {
            _visitedCells.Add(_currentCell);
            if (neighboringCells.Any())
            {
                var randomNeighbor = neighboringCells.ElementAt(Random.Next(0, neighboringCells.Count));
                _currentCell.BreakWall(randomNeighbor.Key);
                _currentChain.Push(_currentCell);
                _currentCell.Background = Colors.LightBlue;
                SetCurrentCell(randomNeighbor.Value);
            }
            else
            {
                _currentCell.Background = Colors.White;
                if (_currentChain.Any())
                {
                    SetCurrentCell(_currentChain.Pop());
                }
                else
                {
                    IsComplete = true;
                }
            }
            Maze.OnCellsChanged();
        }

        private IReadOnlyCollection<KeyValuePair<Direction, Cell>> GetValidNeighboringCells(Cell cell)
        {
            return cell.NeighboringCells
                .Where(x => !_visitedCells.Contains(x.Value))
                .ToList();
        }

        private void SetCurrentCell(Cell cell)
        {
            _currentCell = cell;
            _currentCell.Background = Colors.LightCoral;
        }

        private void Initialize()
        {
            SetCurrentCell(GetRandomCell());
            Maze.OnCellsChanged();
            IsInitialized = true;
        }
        
        #endregion
    }
}
