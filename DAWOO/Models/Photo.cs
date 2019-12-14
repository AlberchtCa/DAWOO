using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWOO.Models
{
    public class Photo
    {
        public int ID { get; set; }
        public DateTime PostDate { get; set; }
        public string Description { get; set; }
        public byte[] Image { get;set; }

        public int CategoryId { get; set; }

        public string UserId { get; set; }

        public virtual Category Category { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public virtual ApplicationUser User { get; set; }

        public class PhotoDBContext : DbContext
        {
            public PhotoDBContext() : base("DBConnectionString") { }
            public DbSet<Photo> Photos { get; set; }
            public DbSet<Category> Categories { get; set; }
        }
    }
}
