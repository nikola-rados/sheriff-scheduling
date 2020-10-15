using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.Api.Models.Dto;

namespace SS.Api.models.dto
{
    public class SheriffByLocationDto : SheriffDto
    {
        public List<string> LoanedIn { get; set; }
        public List<string> LoanedOut { get; set; }
    }
}
