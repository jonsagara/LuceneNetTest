open System
open System.IO
open Lucene.Net.Analysis.Standard
open Lucene.Net.Documents
open Lucene.Net.Index
open Lucene.Net.Search
open Lucene.Net.Store
open Lucene.Net.Util


//
// From the Quick Start Tutorial: https://lucenenet.apache.org/quick-start/tutorial.html
//

// Specify the compatibility version we want
let _luceneVersion = LuceneVersion.LUCENE_48;

[<EntryPoint>]
let main args =

    //
    // Open the Directory using a Lucene Directory class.
    //

    //printfn $"Current directory: {Environment.CurrentDirectory}";
    let indexName = "example_index"
    let indexPath = Path.Combine(Environment.CurrentDirectory, indexName)
    use indexDir = FSDirectory.Open(indexPath)


    //
    // Create an analyzer to process the text.
    //

    use standardAnalyzer = new StandardAnalyzer(_luceneVersion)


    //
    // Create an index writer.
    //

    let indexWriterConfig = IndexWriterConfig(_luceneVersion, standardAnalyzer)
    indexWriterConfig.OpenMode <- OpenMode.CREATE
    use indexWriter = new IndexWriter(indexDir, indexWriterConfig)


    //
    // Add three documents to the index.
    //

    let doc1 = new Document();
    doc1.Add(new TextField("title", "The Apache Software Foundation - The world's largest open source foundation.", Field.Store.YES));
    doc1.Add(new StringField("domain", "www.apache.org/", Field.Store.YES));
    indexWriter.AddDocument(doc1);

    let doc2 = new Document();
    doc2.Add(new TextField("title", "Powerful open source search library for .NET", Field.Store.YES));
    doc2.Add(new StringField("domain", "lucenenet.apache.org", Field.Store.YES));
    indexWriter.AddDocument(doc2);

    let doc3 = new Document();
    doc3.Add(new TextField("title", "Unique gifts made by small businesses in North Carolina.", Field.Store.YES));
    doc3.Add(new StringField("domain", "www.giftoasis.com", Field.Store.YES));
    indexWriter.AddDocument(doc3);


    //
    // Flush and commit the index data to the directory.
    //

    indexWriter.Commit()


    //
    // Search the index and display the results.
    //

    use reader = indexWriter.GetReader(applyAllDeletes = true)
    let searcher = IndexSearcher(reader)

    let query = TermQuery(Term(fld = "domain", text = "lucenenet.apache.org"))
    let topDocs = searcher.Search(query, n = 2) // Indicate we want the first 2 results.

    let resultDoc = searcher.Doc(topDocs.ScoreDocs[0].Doc) // Read back the first doc from the results (i.e., offset 0).
    let title = resultDoc.Get("title")

    printfn $"Matching results: {topDocs.TotalHits}"
    printfn $"Title of first result: {title}"


    // Indicate success.
    0
