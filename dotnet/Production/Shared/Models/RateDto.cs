using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models;

public sealed class RateDto
{
    public int BaseCurrencyId { get; set; }

    public string BaseToken { get; set; }

    public string QuoteToken { get; set; }

    public string Rate { get; set; }
}