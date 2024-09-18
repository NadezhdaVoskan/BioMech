namespace BioMech.Models
{
    public class ArticlesModel
    {
        public List<Article>? Articles { get; set; }

        public List<ArticleCategory>? ArticlesCategory { get; set; } = new List<ArticleCategory>();
    }
}

