using System;
using AI_lib;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace AI_lib_test
{
    public class NeighbourTest
    {
        [Fact]
        public void Ensure_AvailableNeigboursReturnAllNeighbours_WhenAllSet()
        {
            var neighbours = new Neighbours()
            {
                NeighbourUp = "up",
                NeighbourDown = "down",
                NeighbourLeft = "left",
                NeighbourRight = "right"
            };

            Assert.Equal(4, neighbours.AvailableNeighbours.Count);
        }

        [Fact]
        public void Ensure_AvailableNeigboursReturnNone_WhenAllIsEmptyString()
        {
            var neighbours = new Neighbours();

            Assert.Empty(neighbours.AvailableNeighbours);
        }

        [Fact]
        public void NeighbourMap_ShouldContain_AllCoordOnGameBoard()
        {
            // Arrange
            var flattenedCords = new List<string>();

            foreach (Column column in Enum.GetValues(typeof(CoordinatesHelper.Column)))
            {
                foreach (var row in Enumerable.Range(1, GameConstants.MaxRowCount))
                {
                    flattenedCords.Add(CoordinateKey.Build(column, row));
                }
            }

            foreach (var coordKey in flattenedCords)
            {
                var neighbour = CoordinateNeighbours.Instance.GetNeighbours(coordKey);
                Assert.NotEmpty(neighbour.AvailableNeighbours);
            }
        }

        [Theory]
        [InlineData("A1")]
        [InlineData("A7")]
        [InlineData("A9")]
        public void CoordKey_ShouldNotHaveNeighbour_ToLeft(string coordKey)
        {
            var neighbour = CoordinateNeighbours.Instance.GetNeighbours(coordKey);
            Assert.True(string.IsNullOrEmpty(neighbour.NeighbourLeft));
        }

        [Theory]
        [InlineData("J1")]
        [InlineData("J7")]
        [InlineData("J9")]
        public void CoordKey_ShouldNotHaveNeighbour_ToRight(string coordKey)
        {
            var neighbour = CoordinateNeighbours.Instance.GetNeighbours(coordKey);
            Assert.True(string.IsNullOrEmpty(neighbour.NeighbourRight));
        }

        [Theory]
        [InlineData("A1")]
        [InlineData("C1")]
        [InlineData("F1")]
        public void CoordKey_ShouldNotHaveNeighbour_Up(string coordKey)
        {
            var neighbour = CoordinateNeighbours.Instance.GetNeighbours(coordKey);
            Assert.True(string.IsNullOrEmpty(neighbour.NeighbourUp));
        }

        [Theory]
        [InlineData("A10")]
        [InlineData("C10")]
        [InlineData("F10")]
        public void CoordKey_ShouldNotHaveNeighbour_Down(string coordKey)
        {
            var neighbour = CoordinateNeighbours.Instance.GetNeighbours(coordKey);
            Assert.True(string.IsNullOrEmpty(neighbour.NeighbourDown));
        }

        [Theory]
        [InlineData("B5")]
        [InlineData("E2")]
        [InlineData("D9")]
        public void CoordKey_ShouldHaveNeighbour_InAllDirections(string coordKey)
        {
            var neighbour = CoordinateNeighbours.Instance.GetNeighbours(coordKey);
            Assert.False(string.IsNullOrEmpty(neighbour.NeighbourUp));
            Assert.False(string.IsNullOrEmpty(neighbour.NeighbourDown));
            Assert.False(string.IsNullOrEmpty(neighbour.NeighbourLeft));
            Assert.False(string.IsNullOrEmpty(neighbour.NeighbourRight));
        }

        [Theory]
        [InlineData("K5")]
        [InlineData("E11")]
        [InlineData("D0")]
        public void Neighbour_ShouldBeNull_WhenOutsideBounds(string coordKey)
        {
            var neighbour = CoordinateNeighbours.Instance.GetNeighbours(coordKey);
            Assert.Null(neighbour);
        }
    }
}
