using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;


//
// From the Lucene.NET home page: https://lucenenet.apache.org/
//

// Ensures index backward compatibility
const LuceneVersion _luceneVersion = LuceneVersion.LUCENE_48;


//
// Open the Directory using a Lucene Directory class.
//

//Console.WriteLine($"Current directory: {Environment.CurrentDirectory}");
var indexName = "example_index";
var indexPath = Path.Combine(Environment.CurrentDirectory, indexName);
using var indexDir = FSDirectory.Open(indexPath);


//
// Create an analyzer to process the text.
//

var analyzer = new StandardAnalyzer(_luceneVersion);


//
// Create an index writer.
//

var indexConfig = new IndexWriterConfig(_luceneVersion, analyzer);
indexConfig.OpenMode = OpenMode.CREATE;
using var writer = new IndexWriter(indexDir, indexConfig);


//
// Add to the index.
//

var source = new
{
    Name = "Kermit the Frog",
    FavoritePhrase = "The quick brown fox jumps over the lazy dog",
};

var doc = new Document
{
    // StringField indexes, but doesn't tokenize.
    new StringField(name: "name", value: source.Name, Field.Store.YES),
    new TextField(name: "favoritePhrase", value: source.FavoritePhrase, Field.Store.YES),
};

writer.AddDocument(doc);
writer.Flush(triggerMerge: false, applyAllDeletes: false);


//
// Construct a query.
//

var phrase = new MultiPhraseQuery
{
    new Term(fld: "favoritePhrase", text: "brown"),
    new Term(fld: "favoritePhrase", text: "fox"),
};


//
// Fetch the results.
//

// Re-use the writer to get real-time updates.
using var reader = writer.GetReader(applyAllDeletes: true);
var searcher = new IndexSearcher(reader);
var hits = searcher.Search(phrase, n: 20).ScoreDocs;

// Display the output table.
foreach (var hit in hits)
{
    var foundDoc = searcher.Doc(hit.Doc);
    Console.WriteLine($"{ hit.Score:f8} {foundDoc.Get("name"),-15} {foundDoc.Get("favoritePhrase"),-40}");
}