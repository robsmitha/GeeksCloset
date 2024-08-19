using Elysian.Domain.Responses.WordPress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysian.Application.Features.ContentManagement.Models
{
    public class WordPressContent
    {
        public List<WpPageResponse> Pages { get; set; }
    }
}
