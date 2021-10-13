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

            importer = new GarImporter(tempFolder);
            importer.Progress += (s, a) => Trace.WriteLine($"Progress{(a.IsFinished ? " (finished)" : "")}: {a.Percentage}%");
        }

        [Fact]
        public void GetFull_Should_Download_FullFile()
        {
            // Action
            _ = importer.GetFull();

            // Assert
            File.Exists(Path.Combine(tempFolder, Templates.CurrentDateFullFileName)).Should().BeTrue();
        }

        [Fact]
        public async Task GetFullAsync_Should_Download_FullFile()
        {
            // Action
            _ = await importer.GetFullAsync();

            // Assert
            File.Exists(Path.Combine(tempFolder, Templates.CurrentDateFullFileName)).Should().BeTrue();
        }

        [Fact]
        public void GetDiff_Should_Download_DiffFile()
        {
            // Arrange
            DateTime lastLoad = DateTime.Today.AddDays(-2);
            IEnumerable<string> fileNames = Templates.DownloadDiffUrls(lastLoad)
                .Select(x => Templates.GetDiffFileName(x.fileDate));

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
            DateTime lastLoad = DateTime.Today.AddDays(-2);
            IEnumerable<string> fileNames = Templates.DownloadDiffUrls(lastLoad)
                .Select(x => Templates.GetDiffFileName(x.fileDate));

            // Action
            _ = await importer.GetDiffAsync(lastLoad);

            // Assert
            foreach (string? fileName in fileNames)
            {
                File.Exists(Path.Combine(tempFolder, fileName)).Should().BeTrue();
            }
        }

        [Fact]
        public void Import_FullData_Should_Load_Correctly()
        {
            // Arrange
            CopyFullDataFile(tempFolder);

            // Action
            IEnumerable<AddressObject>? data = importer.GetFull();

            // Assert

        }

        private void CopyFullDataFile(string folder)
        {

        }

        public void Dispose()
        {
            foreach (string? file in Directory.EnumerateFiles(tempFolder))
            {
                File.Delete(file);
            }
        }
    }
}
