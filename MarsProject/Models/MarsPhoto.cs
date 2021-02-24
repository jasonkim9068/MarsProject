using System.Collections.Generic;

namespace MarsProject.Models
{
    public class Photo
        {
            public int Id { get; set; }
            public int Sol { get; set; }
            public string Img_src { get; set; }
            public string Earth_date { get; set; }
        }

        public class JsonObject
        {
            public List<Photo> Photos { get; set; }
        }
 }
