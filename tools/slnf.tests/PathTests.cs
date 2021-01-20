using NUnit.Framework;
using System;
using System.IO;

namespace tools.tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var slnFilename = @"./db.sln";
            var outFilename = @"./core.slnf";
            var slnPath = Path.GetFullPath(slnFilename);
            var outPath = Path.GetFullPath(outFilename);
            var slnDirPath = Path.GetDirectoryName(slnPath);
            var outDirPath = Path.GetDirectoryName(outPath);
            var relative = Path.GetRelativePath(outDirPath, slnDirPath);
            var finalPath = Path.Combine(relative, outFilename);
            
            //var fullPath = Path.GetFullPath(relative);
            //var finalPath = fullPath.Replace(Environment.CurrentDirectory, "");
        }
    }
}