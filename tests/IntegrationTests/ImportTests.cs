using FiasGarImporter;
using FiasGarImporter.Helpers;
using FiasGarImporter.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    /// <summary>
    /// For manual run only. Files are very large
    /// </summary>
    public class ImportTests : IDisposable
    {
        private const string tempFolder = "../../../../../temp";
        private readonly IImporter importer;

        public ImportTests()
        {
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            importer = new GarImporter(tempFolder, () => new HttpClientWrapper());
            importer.Progress += (s, a) => Trace.WriteLine($"Progress{(a.IsFinished ? " (finished)" : "")}: {a.Percentage}%");
        }

        [Fact]
        public async Task GetUrlListAsync_Should_Return_List_Of_Urls()
        {
            // Action
            IEnumerable<GarUrl>? result = await importer.GetUrlListAsync();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(new GarUrl("https://fias-file.nalog.ru/downloads/2021.10.12/gar_delta_xml.zip", new DateTime(2021, 10, 12), true));
            result.Should().Contain(new GarUrl("https://fias-file.nalog.ru/downloads/2021.10.12/gar_xml.zip", new DateTime(2021, 10, 12), false));
        }

        [Fact]
        public void GetFull_Should_Download_FullFile()
        {
            // Arrange
            GarUrl lastFull = importer.GetUrlList().Where(x => !x.IsDelta).OrderByDescending(x => x.Date).First();

            // Action
            _ = importer.GetFull();

            // Assert
            File.Exists(Path.Combine(tempFolder, Templates.GetFullFileName(lastFull.Date))).Should().BeTrue();
        }

        [Fact]
        public async Task GetFullAsync_Should_Download_FullFile()
        {
            // Arrange
            GarUrl lastFull = (await importer.GetUrlListAsync()).Where(x => !x.IsDelta).OrderByDescending(x => x.Date).First();

            // Action
            _ = await importer.GetFullAsync();

            // Assert
            File.Exists(Path.Combine(tempFolder, Templates.GetFullFileName(lastFull.Date))).Should().BeTrue();
        }

        [Fact]
        public async Task Enumerate_Regions_Should_Return_List_Of_Regions()
        {
            // Arrange
            string? fileName = Path.Combine(tempFolder, "gar_xml_2021-10-26.zip");
            int regionsNumber = 0;
            bool hasMissingNumber = false;
            int previousRegionId = 0;

            // Action
            foreach (int regionId in importer.EnumerateRegionsInFile(fileName))
            {
                regionsNumber++;
                if (previousRegionId + 1 != regionId)
                {
                    hasMissingNumber = true;
                }

                previousRegionId = regionId;
            }

            // Assert
            regionsNumber.Should().BeGreaterThan(0);
            hasMissingNumber.Should().BeTrue();
        }

        [Fact]
        public void GetDiff_Should_Download_DiffFile()
        {
            // Arrange
            DateTime lastLoad = new(2021, 10, 12);
            IEnumerable<string> fileNames = importer.GetUrlList()
                .Where(x => x.IsDelta && x.Date > lastLoad)
                .Select(x => Templates.GetDiffFileName(x.Date));

            // Action
            _ = importer.GetDiff(lastLoad);

            // Assert
            foreach (string? fileName in fileNames)
            {
                File.Exists(Path.Combine(tempFolder, fileName)).Should().BeTrue();
            }
        }

        [Fact]
        public async Task GetDiffAsync_Should_Download_DiffFile()
        {
            // Arrange
            DateTime lastLoad = new(2021, 10, 12);
            IEnumerable<string> fileNames = (await importer.GetUrlListAsync())
                .Where(x => x.IsDelta && x.Date > lastLoad)
                .Select(x => Templates.GetDiffFileName(x.Date));

            // Action
            _ = await importer.GetDiffAsync(lastLoad);

            // Assert
            foreach (string? fileName in fileNames)
            {
                File.Exists(Path.Combine(tempFolder, fileName)).Should().BeTrue();
            }
        }

        [Fact]
        public void Import_DiffData_Should_Load_Correctly()
        {
            // Arrange
            DateTime lastLoad = new(2021, 10, 12);

            // Action
            IEnumerable<AddressObject>? data = importer.GetDiff(lastLoad);

            // Assert
            data.Should().NotBeNullOrEmpty();
        }

        private void CopyFullDataFile(string folder)
        {

        }

        public void Dispose()
        {
            foreach (string? file in Directory.EnumerateFiles(tempFolder))
            {
                //File.Delete(file);
            }
        }
    }
}
