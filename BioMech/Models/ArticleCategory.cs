using System;
using System.Collections.Generic;

namespace BioMech.Models
{
    public partial class ArticleCategory
    {

        public int? IdArticleCategory { get; set; }
        public string? NameArticleCategory { get; set; } = null!;
        public bool? DeletedCategoryArticle { get; set; }

    }
}
