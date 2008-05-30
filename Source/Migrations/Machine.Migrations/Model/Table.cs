﻿using System.Collections.Generic;

namespace Machine.Migrations
{
  public class Table
  {
    string _name;
    readonly List<Column> _columns = new List<Column>();

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public List<Column> Columns
    {
      get { return _columns; }
    }

    public Table()
    {
    }

    public Table(string name)
    {
      _name = name;
    }
  }
}