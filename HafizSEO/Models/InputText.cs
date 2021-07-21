using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HafizSEO.Models
{
    public class InputText
    {
        [Url]
        public string Text { get; set; }

        public bool checkLink { get; set; } = true;

        public bool checkWord { get; set; } = true;

        public Dictionary<string, int> occurenceLinks { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, int> occurenceMetas { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, int> occurencePages { get; set; } = new Dictionary<string, int>();
    }
}
