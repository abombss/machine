﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Machine.Core.Services;

namespace Machine.Migrations.Services.Impl
{
  public class MigrationFinder : IMigrationFinder
  {
    #region Member Data
    private readonly Regex _regex = new Regex(@"^(\d+)_([\w_]+)\.(cs|boo)$");
    private readonly IConfiguration _configuration;
    private readonly IFileSystem _fileSystem;
    private readonly INamer _namer;
    #endregion

    #region MigrationFinder()
    public MigrationFinder(IFileSystem fileSystem, INamer namer, IConfiguration configuration)
    {
      _fileSystem = fileSystem;
      _namer = namer;
      _configuration = configuration;
    }
    #endregion

    #region IMigrationFinder Members
    public ICollection<MigrationReference> FindMigrations()
    {
      List<MigrationReference> migrations = new List<MigrationReference>();
      foreach (string file in _fileSystem.GetFiles(_configuration.MigrationsDirectory))
      {
        Match m = _regex.Match(Path.GetFileName(file));
        if (m.Success)
        {
          migrations.Add(new MigrationReference(Int16.Parse(m.Groups[1].Value), _namer.ToCamelCase(m.Groups[2].Value), file));
        }
      }
      migrations.Sort(delegate (MigrationReference mr1, MigrationReference mr2) {
        return mr1.Version.CompareTo(mr2.Version);
      });
      return migrations;
    }
    #endregion
  }
}
