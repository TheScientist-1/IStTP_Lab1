using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalleryWebApplication.Models.GalleryViewModels
{ 
    public class InstructorIndexData
    {
        public IEnumerable<Author> Authors { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}