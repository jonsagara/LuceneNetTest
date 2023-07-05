using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using LuceneDirectory = Lucene.Net.Store.Directory;

// Specify the compatibility version we want
const LuceneVersion _luceneVersion = LuceneVersion.LUCENE_48;

//// Construct a machine-independent path for the index
//var basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
//var indexPath = Path.Combine(basePath, "example_index");

//
// Open the Directory using a Lucene Directory class.
//

//Console.WriteLine($"Current directory: {Environment.CurrentDirectory}");
var indexName = "example_index";
var indexPath = Path.Combine(Environment.CurrentDirectory, indexName);
using LuceneDirectory indexDir = FSDirectory.Open(indexPath);


//
// Create an analyzer to process the text.
//

var standardAnalyzer = new StandardAnalyzer(_luceneVersion);


//
// Create an index writer.
//

var indexWriterConfig = new IndexWriterConfig(_luceneVersion, standardAnalyzer);
indexWriterConfig.OpenMode = OpenMode.CREATE;
var indexWriter = new IndexWriter(indexDir, indexWriterConfig);


//
// Add three documents to the index.
//

var doc1 = new Document();
doc1.Add(new TextField("title", "The Apache Software Foundation - The world's largest open source foundation.", Field.Store.YES));
doc1.Add(new StringField("domain", "www.apache.org/", Field.Store.YES));
indexWriter.AddDocument(doc1);

var doc2 = new Document();
doc2.Add(new TextField("title", "Powerful open source search library for .NET", Field.Store.YES));
doc2.Add(new StringField("domain", "lucenenet.apache.org", Field.Store.YES));
indexWriter.AddDocument(doc2);

var doc3 = new Document();
doc3.Add(new TextField("title", "Unique gifts made by small businesses in North Carolina.", Field.Store.YES));
doc3.Add(new StringField("domain", "www.giftoasis.com", Field.Store.YES));
indexWriter.AddDocument(doc3);


//
// Flush and commit the index data to the directory.
//

indexWriter.Commit();


//
// Search the index and display the results.
//

using var reader = indexWriter.GetReader(applyAllDeletes: true);
var searcher = new IndexSearcher(reader);

var query = new TermQuery(new Term(fld: "domain", text: "lucenenet.apache.org"));
var topDocs = searcher.Search(query, n: 2); // Indicate we want the first 2 results.

var numMatchingDocs = topDocs.TotalHits;
var resultDoc = searcher.Doc(topDocs.ScoreDocs[0].Doc); // Read back the first doc from the results (i.e., offset 0).
var title = resultDoc.Get("title");

Console.WriteLine($"Matching results: {topDocs.TotalHits}");
Console.WriteLine($"Title of first result: {title}");
