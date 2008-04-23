﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Machine.MsMvc
{
  public interface IControllerPreparer
  {
    IController PrepareController(IController controller);
  }
}
