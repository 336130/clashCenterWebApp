﻿using clashCenter.Business;
using clashCenter.Dal.Models.ClashResponse;
using clashCenter.Web.Models.RecieveObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace clashCenter.Controllers
{
    public class SearchController : ApiController
    {
        [HttpPost]
        public ClanSearchResults SearchForClan ([FromBody]ClanSearch searchParams)
        {
            var retVal = new BusinessManager().SearchForClan(searchParams.name);
            return retVal;
        }
    }
}
