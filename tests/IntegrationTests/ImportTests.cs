using FiasGarImporter;
using FiasGarImporter.Helpers;
using FiasGarImporter.Models;
using FluentAssertions;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class ImportTests
    {
        private const string tempFolder = "./temp";
        private readonly IImporter importer;

        public ImportTests()
        {
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            importer = new GarImporter(tempFolder);
        }

        ~ImportTests()
        {
            foreach (string? file in Directory.EnumerateFiles(tempFolder))
            {
                File.Delete(file);
            }
        }

        [Fact]
        public void GetFull_Should_Download_FullFile()
        {
            // Arrange
            importer.Progress += (s, a) => Trace.WriteLine($"Progress{(a.IsFinished ? " (finished)" : "")}: {a.Percentage}%");

            // Action
            _ = importer.GetFull();

            // Assert
            File.Exists(Path.Combine(tempFolder, Templates.CurrentDateFullFileName)).Should().BeTrue();
        }

        [Fact]
        public async Task GetFullAsync_Should_Download_FullFile()
        {
            // Arrange
            importer.Progress += (s, a) => Trace.WriteLine($"Progress{(a.IsFinished ? " (finished)" : "")}: {a.Percentage}%");

            // Action
            _ = await importer.GetFullAsync();

            // Assert
            File.Exists(Path.Combine(tempFolder, Templates.CurrentDateFullFileName)).Should().BeTrue();
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
    }
}
