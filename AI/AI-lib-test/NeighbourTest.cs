using System;
using AI_lib;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;
using System.IO;

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

        [Theory]
        [InlineData(new string[] { "A2", "B2", "C2" }, "A1", "A3", "", "B2")]
        [InlineData(new string[] { "E2", "B2", "C2" }, "B1", "B3", "A2", "C2")]
        public void ShouldGet_MinHorizontalNeighbours_FromGivenCoords(
            string[] coords,
            string expectedUp,
            string expectedDown,
            string expectedLeft,
            string expectedRight)
        {
            var neighbours = NeighbourCalculator.GetNeighbours(
                coords.ToList(),
                Direction.Horizontal,
                AI_lib.Range.Min);

            Assert.Equal(expectedUp, neighbours.NeighbourUp);
            Assert.Equal(expectedDown, neighbours.NeighbourDown);
            Assert.Equal(expectedLeft, neighbours.NeighbourLeft);
            Assert.Equal(expectedRight, neighbours.NeighbourRight);
        }

        [Theory]
        [InlineData(new string[] { "A2", "B2", "C2" }, "C1", "C3", "B2", "D2")]
        [InlineData(new string[] { "E2", "B2", "C2" }, "E1", "E3", "D2", "F2")]
        public void ShouldGet_MaxHorizontalNeighbours_FromGivenCoords(
          string[] coords,
          string expectedUp,
          string expectedDown,
          string expectedLeft,
          string expectedRight)
        {
            var neighbours = NeighbourCalculator.GetNeighbours(
                coords.ToList(),
                Direction.Horizontal,
                AI_lib.Range.Max);

            Assert.Equal(expectedUp, neighbours.NeighbourUp);
            Assert.Equal(expectedDown, neighbours.NeighbourDown);
            Assert.Equal(expectedLeft, neighbours.NeighbourLeft);
            Assert.Equal(expectedRight, neighbours.NeighbourRight);
        }

        [Theory]
        [InlineData(new string[] { "A2", "A3", "A4" }, "A1", "A3", "", "B2")]
        [InlineData(new string[] { "E1", "E2", "E3", "E10" }, "", "E2", "D1", "F1")]
        public void ShouldGet_MinVerticalNeighbours_FromGivenCoords(
          string[] coords,
          string expectedUp,
          string expectedDown,
          string expectedLeft,
          string expectedRight)
        {
            var neighbours = NeighbourCalculator.GetNeighbours(
                coords.ToList(),
                Direction.Vertical,
                AI_lib.Range.Min);

            Assert.Equal(expectedUp, neighbours.NeighbourUp);
            Assert.Equal(expectedDown, neighbours.NeighbourDown);
            Assert.Equal(expectedLeft, neighbours.NeighbourLeft);
            Assert.Equal(expectedRight, neighbours.NeighbourRight);
        }


        [Theory]
        [InlineData(new string[] { "A2", "A3", "A4" }, "A3", "A5", "", "B4")]
        [InlineData(new string[] { "E1", "E2", "E3", "E10" }, "E9", "", "D10", "F10")]
        public void ShouldGet_MaxVerticalNeighbours_FromGivenCoords(
          string[] coords,
          string expectedUp,
          string expectedDown,
          string expectedLeft,
          string expectedRight)
        {
            var neighbours = NeighbourCalculator.GetNeighbours(
                coords.ToList(),
                Direction.Vertical,
                AI_lib.Range.Max);

            Assert.Equal(expectedUp, neighbours.NeighbourUp);
            Assert.Equal(expectedDown, neighbours.NeighbourDown);
            Assert.Equal(expectedLeft, neighbours.NeighbourLeft);
            Assert.Equal(expectedRight, neighbours.NeighbourRight);
        }
    }
}
