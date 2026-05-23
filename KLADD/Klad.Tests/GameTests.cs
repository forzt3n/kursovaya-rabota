using Xunit;
using Klad.Core;
using Klad.Core.Map;
using Klad.Core.Entities;
using Klad.Core.Decorators;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Klad.Tests
{
    public class SimpleInputService : IInputService
    {
        public bool IsKeyPressed(GameKey key) => false;
        public bool IsKeyDown(GameKey key) => false;
    }

    public class GameTests
    {
        [Fact]
        public void MapGenerator_ShouldCreateGridWithCorrectDimensions()
        {
            var gen = new MapGenerator(21, 21);
            Point p1, p2;
            var grid = gen.Generate(out p1, out p2);
            
            Assert.Equal(21, grid.GetLength(0));
            Assert.Equal(21, grid.GetLength(1));
        }

        [Fact]
        public void GameMap_IsPassable_ShouldReturnTrueForEmpty()
        {
            var map = new GameMap(10, 10);
            map.Grid[5, 5] = new Floor();
            Assert.True(map.IsPassable(5, 5));
        }

        [Fact]
        public void GameMap_IsPassable_ShouldReturnFalseForSolidWall()
        {
            var map = new GameMap(10, 10);
            map.Grid[5, 5] = new Wall();
            Assert.False(map.IsPassable(5, 5));
        }

        [Fact]
        public void GameMap_TryPlaceWall_ShouldCreateTempWallOnEmpty()
        {
            var map = new GameMap(10, 10);
            map.Grid[5, 5] = new Floor();
            map.TryPlaceWall(1, 5, 5);
            Assert.True(map.Grid[5, 5] is TemporaryWallDecorator);
        }

        [Fact]
        public void GameMap_Update_ShouldRemoveTempWallAfterTime()
        {
            var map = new GameMap(10, 10);
            map.Grid[5, 5] = new Floor();
            map.TryPlaceWall(1, 5, 5);
            
            map.Update(4.1f); 
            
            Assert.True(map.Grid[5, 5] is Floor);
        }

        [Fact]
        public void Player_AddScore_ShouldIncreaseScore()
        {
            var player = new Player(1);
            player.Score += 100;
            Assert.Equal(100, player.Score);
        }

        [Fact]
        public void MapExporter_ColorMapping_ShouldBeConsistent()
        {
            var color = MapExporter.GetColorForCell(RawCellType.TreasureMarker);
            var cell = MapExporter.GetCellFromColor(color);
            Assert.Equal(RawCellType.TreasureMarker, cell);
        }

        [Fact]
        public void GameMap_IsVisible_ShouldHandleBlinking()
        {
            var map = new GameMap(10, 10);
            map.Grid[5, 5] = new Floor();
            map.TryPlaceWall(1, 5, 5); 
            
            Assert.True(map.IsVisible(5, 5));
      
            map.Update(0.9f); 
            Assert.True(map.IsVisible(5, 5));
        }

        [Fact]
        public void GameEngine_GameOver_ShouldBeTrueWhenNoPrizes()
        {
            var engine = new GameEngine(new SimpleInputService());
            engine.Initialize(21, 21);
            engine.Prizes.Clear(); 
            Assert.True(engine.IsGameOver());
        }

        [Fact]
        public void GameMap_OutOfBounds_IsPassable_ShouldReturnFalse()
        {
            var map = new GameMap(10, 10);
            Assert.False(map.IsPassable(-1, 0));
            Assert.False(map.IsPassable(10, 0));
        }

        [Fact]
        public void GameMap_PlaceWall_DoubleCall_ShouldNotOverwriteEffect()
        {
            var map = new GameMap(10, 10);
            map.Grid[5, 5] = new Floor();
            map.TryPlaceWall(1, 5, 5);
            map.TryPlaceWall(2, 5, 5); 
            Assert.True(map.Grid[5, 5] is TemporaryWallDecorator);
        }

        [Fact]
        public void GameEngine_MovePlayer_SpeedShouldAffectDistance()
        {
            var player = new Player(1);
            float baseSpeed = player.CurrentSpeed;
            player.SpeedMultiplier = 2.0f; 
            
            Assert.Equal(baseSpeed * 2.0f, player.CurrentSpeed);
        }

        [Fact]
        public void GameMap_OnlyOneTemporaryWallAllowed()
        {
            var map = new GameMap(10, 10);
            map.Grid[5, 5] = new Floor();
            map.Grid[6, 6] = new Floor();
            
            map.TryPlaceWall(1, 5, 5);
            Assert.True(map.Grid[5, 5] is TemporaryWallDecorator);
            
            map.TryPlaceWall(1, 6, 6);
            Assert.False(map.Grid[6, 6] is TemporaryWallDecorator);
            Assert.True(map.Grid[6, 6] is Floor);
        }
    }
}
