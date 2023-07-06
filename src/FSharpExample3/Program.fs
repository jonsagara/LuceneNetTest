open System
open System.IO
open Lucene.Net.Analysis.Standard
open Lucene.Net.Documents
open Lucene.Net.Index
open Lucene.Net.Search
open Lucene.Net.Store
open Lucene.Net.Util


//
// From the Lucene.NET home page: https://lucenenet.apache.org/
//

// Ensures index backward compatibility
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

    use analyzer = new StandardAnalyzer(_luceneVersion)


    //
    // Create an index writer.
    //

    let indexConfig = new IndexWriterConfig(_luceneVersion, analyzer)
    indexConfig.OpenMode <- OpenMode.CREATE;
    use writer = new IndexWriter(indexDir, indexConfig)


    //
    // Add to the index.
    //

    let source = 
        {|
            Name = "Kermit the Frog"
            FavoritePhrase = "The quick brown fox jumps over the lazy dog"
        |};

    let doc = Document()
    doc.Add(StringField(name = "name", value = source.Name, stored = Field.Store.YES))
    doc.Add(TextField(name = "favoritePhrase", value = source.FavoritePhrase, store = Field.Store.YES))

    writer.AddDocument(doc)
    writer.Flush(triggerMerge = false, applyAllDeletes = false)


    //
    // Construct a query.
    //

    let phrase = MultiPhraseQuery()
    phrase.Add([|
        Term(fld = "favoritePhrase", text = "brown")
        Term(fld = "favoritePhrase", text = "fox")
        |])



    //
    // Fetch the results.
    //

    // Re-use the writer to get real-time updates.
    use reader = writer.GetReader(applyAllDeletes = true)
    let searcher = IndexSearcher(reader)
    let hits = searcher.Search(phrase, n = 20).ScoreDocs

    // Display the output table.
    hits
    |> Array.iter (fun hit ->
        let foundDoc = searcher.Doc(hit.Doc)
        let name = foundDoc.Get("name")
        let favoritePhrase = foundDoc.Get("favoritePhrase")

        Console.WriteLine($"{ hit.Score:f8} {name,-15} {favoritePhrase,-40}")
        ())

    0