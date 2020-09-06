using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using HugMun.Core;

namespace HugMun.Data
{
    public static class FileProvider
    {
        public static ICaseFrame Load<TCase>(string path, CaseSchema schema = null) where TCase : class
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new FileNotFoundException($"Case file {path} does not exist.");

            return new FileCaseFrame<TCase>(path, schema);
        }

        internal sealed class FileLoader<TCase> where TCase : class
        {
            private readonly string path;
            private readonly CsvConfiguration configuration;

            public FileLoader(string path, CaseDefinition definition)
            {
                if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
                if (!File.Exists(path)) throw new FileNotFoundException($"File {path} does not exist.");

                this.path = path;
                var map = new CaseClassMap<TCase>(definition);

                configuration = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" };
                configuration.RegisterClassMap(map);
            }

            public FileReader<TCase> CreateReader()
            {
                return new FileReader<TCase>(path, configuration);
            }
        }

        internal sealed class FileReader<TCase> : IDisposable where TCase : class
        {
            private readonly StreamReader streamReader;
            private readonly CsvReader csvReader;

            public TCase Current;

            public FileReader(string path, CsvConfiguration configuration)
            {
                if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
                if (configuration == null) throw new ArgumentNullException(nameof(configuration));

                streamReader = new StreamReader(path);
                csvReader = new CsvReader(streamReader, configuration);
            }

            public bool Read()
            {
                var result = csvReader.Read();
                Current = result ? csvReader.GetRecord<TCase>() : null;
                return result;
            }

            public void Dispose()
            {
                streamReader.Dispose();
                csvReader.Dispose();
            }
        }

        internal sealed class CaseClassMap<TCase> : ClassMap<TCase>
        {
            public CaseClassMap(CaseDefinition definition)
            {
                MapAttribute(definition.IdAttribute);
                MapAttribute(definition.SolutionAttribute);
                foreach (var attribute in definition)
                {
                    MapAttribute(attribute);
                }
            }

            private void MapAttribute(AttributeDefinition definition)
            {
                Map(typeof(TCase), definition.MemberInfo).Name(definition.Name);
            }
        }

        internal sealed class FileCaseFrame<TCase> : TypedCaseFrame<TCase> where TCase : class
        {
            private readonly FileLoader<TCase> fileLoader;

            public override int CaseCount => 0;

            public FileCaseFrame(string path, CaseSchema schema) : base(schema)
            {
                fileLoader = new FileLoader<TCase>(path, Definition);
            }

            public override CaseCursor GetCaseCursor()
            {
                return new DelegatingCursor<TCase>(new FileCursor<TCase>(fileLoader, this));
            }
        }

        internal sealed class FileCursor<TCase> : TypedCursor<TCase> where TCase : class
        {
            private bool disposed;

            private readonly FileReader<TCase> fileReader;

            public FileCursor(FileLoader<TCase> loader, TypedCaseFrame<TCase> frame) : base(frame)
            {
                if (loader == null) throw new ArgumentNullException(nameof(loader));
                fileReader = loader.CreateReader();
            }

            protected override bool MoveNextOuter()
            {
                var result = fileReader.Read();
                Data = fileReader.Current;
                return result;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposed) return;

                if (disposing)
                {
                    fileReader.Dispose();
                }

                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
