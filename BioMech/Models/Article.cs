using System;
using System.Collections.Generic;

namespace BioMech.Models
{
    public partial class Article
    {
        public int? IdArticle { get; set; }
        public string ContentAtricle { get; set; } = null!;
        public string ShortDescriptionArticle { get; set; } = null!;
        public DateTime PublicationDateArticle { get; set; }
        public int? UserId { get; set; }
        public int? ArticleCategoryId { get; set; }
        public string Name_Article { get; set; }
        public string? Image_Article { get; set; }
        public bool? DeletedArticle { get; set; }
    }
}
