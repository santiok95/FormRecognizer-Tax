using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

//use your `key` and `endpoint` environment variables to create your `AzureKeyCredential` and `DocumentAnalysisClient` instances
string key = "3cefc52d84204f3dbc295d3a3cc9ed7f";
string endpoint = "https://demorecognizer15.cognitiveservices.azure.com/";
AzureKeyCredential credential = new AzureKeyCredential(key);
DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

// sample document document
Uri w2Uri = new Uri("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/rest-api/w2.png");

AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-tax.us.w2", w2Uri);

AnalyzeResult result = operation.Value;

for (int i = 0; i < result.Documents.Count; i++)
{
    Console.WriteLine($"Document {i}:");

    AnalyzedDocument document = result.Documents[i];

    if (document.Fields.TryGetValue("AdditionalInfo", out DocumentField? additionalInfoField))
    {
        if (additionalInfoField.FieldType == DocumentFieldType.List)
        {
            foreach (DocumentField infoField in additionalInfoField.Value.AsList())
            {
                Console.WriteLine("AdditionalInfo:");

                if (infoField.FieldType == DocumentFieldType.Dictionary)
                {
                    IReadOnlyDictionary<string, DocumentField> infoFields = infoField.Value.AsDictionary();

                    if (infoFields.TryGetValue("Amount", out DocumentField? amountField))
                    {
                        if (amountField.FieldType == DocumentFieldType.Double)
                        {
                            double amount = amountField.Value.AsDouble();

                            Console.WriteLine($"  Amount: '{amount}', with confidence {amountField.Confidence}");
                        }
                    }

                    if (infoFields.TryGetValue("LetterCode", out DocumentField? letterCodeField))
                    {
                        if (letterCodeField.FieldType == DocumentFieldType.String)
                        {
                            string letterCode = letterCodeField.Value.AsString();

                            Console.WriteLine($"  LetterCode: '{letterCode}', with confidence {letterCodeField.Confidence}");
                        }
                    }
                }
            }
        }
    }


    if (document.Fields.TryGetValue("AllocatedTips", out DocumentField? allocatedTipsField))
    {
        if (allocatedTipsField.FieldType == DocumentFieldType.Double)
        {
            double allocatedTips = allocatedTipsField.Value.AsDouble();
            Console.WriteLine($"Allocated Tips: '{allocatedTips}', with confidence {allocatedTipsField.Confidence}");
        }
    }

    if (document.Fields.TryGetValue("Employer", out DocumentField? employerField))
    {
        if (employerField.FieldType == DocumentFieldType.Dictionary)
        {
            IReadOnlyDictionary<string, DocumentField> employerFields = employerField.Value.AsDictionary();

            if (employerFields.TryGetValue("Name", out DocumentField? employerNameField))
            {
                if (employerNameField.FieldType == DocumentFieldType.String)
                {
                    string name = employerNameField.Value.AsString();

                    Console.WriteLine($"Employer Name: '{name}', with confidence {employerNameField.Confidence}");
                }
            }

            if (employerFields.TryGetValue("IdNumber", out DocumentField? idNumberField))
            {
                if (idNumberField.FieldType == DocumentFieldType.String)
                {
                    string id = idNumberField.Value.AsString();

                    Console.WriteLine($"Employer ID Number: '{id}', with confidence {idNumberField.Confidence}");
                }
            }

            if (employerFields.TryGetValue("Address", out DocumentField? addressField))
            {
                if (addressField.FieldType == DocumentFieldType.Address)
                {
                    Console.WriteLine($"Employer Address: '{addressField.Content}', with confidence {addressField.Confidence}");
                }
            }
        }
    }
}