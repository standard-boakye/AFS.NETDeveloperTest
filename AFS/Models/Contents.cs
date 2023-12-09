using System;
using System.ComponentModel.DataAnnotations;

namespace AFS.Models
{
    public class Contents
    {
        [Key]
        public int content_id { get; set; }
        public string translated { get; set; }
        public string text { get; set; }
        public string translation { get; set; }
    }
}
