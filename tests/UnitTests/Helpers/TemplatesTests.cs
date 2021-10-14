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
        [InlineData(0, 4)]
        [InlineData(1, 4)]
        [InlineData(2, 5)]
        [InlineData(3, 4)]
        [InlineData(4, 4)]
        [InlineData(5, 4)]
        [InlineData(6, 5)]
        [InlineData(7, 4)]
        public void DownloadDiffUrls_Should_Return_ExpectedCouunt_Names_If_Two_Weeks_Offset(int beginDaysFromToday, int expectedCount)
        {
            // Arrange
            DateTime end = DateTime.Today.AddDays(-beginDaysFromToday);
            DateTime begin = end.AddDays(-7 * 2);

            // Action
            IEnumerable<(DateTime fileDate, string url)>? result = Templates.DownloadDiffUrls(begin, end);

            // Assert
            result.Should().HaveCount(expectedCount);
        }

        [Fact]
        public void DownloadDiffUrls_If_LastSync_Is_Tusday_Should_Return_One_Item()
        {
            // Arrange
            DateTime begin = new(2021, 10, 9);

            // Action
            IEnumerable<(DateTime fileDate, string url)>? result = Templates.DownloadDiffUrls(begin);

            // Assert
            result.Should().BeEquivalentTo(new[] {
                (new DateTime(2021,10,12), "https://fias-file.nalog.ru/downloads/2021.10.12/gar_delta_xml.zip")
            });
        }
    }
}
