// <copyright file="Revision.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

using AndroidSdkRepository;
using System.Reflection.Metadata;
using System;
using System.Xml.Linq;

namespace AndroidSdkRepository.Tests
{
    public class Repository3Tests
    {
        /// <summary>
        /// Tests whether a repository can be loaded.
        /// </summary>
        [Fact]
        public void LoadRepository()
        {
            var url = "https://test.uri";
            XDocument document;
            HttpClient client = new HttpClient();

            using (Stream stream =  new FileStream("repository2-1.xml", FileMode.Open))
            {
                document = XDocument.Load(stream);
            }
            var repository = Repository2.FromXElement(document.Root!, new Uri(url));
            Assert.Equal(76, repository.BuildTools.Count);
        }
    }
}
