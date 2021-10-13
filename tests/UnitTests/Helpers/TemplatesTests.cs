using FiasGarImporter.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace UnitTests.Helpers
{
    public class TemplatesTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        public void DownloadDiffUrls_Should_Return_Five_Names_If_Two_Weeks_Offset(int beginDaysFromToday)
        {
            // Arrange
            DateTime end = DateTime.Today.AddDays(-beginDaysFromToday);
            DateTime begin = end.AddDays(-7 * 2);

            // Action
            IEnumerable<(DateTime fileDate, string url)>? result = Templates.DownloadDiffUrls(begin, end);

            // Assert
            result.Should().HaveCount(5);
        }
    }
}
