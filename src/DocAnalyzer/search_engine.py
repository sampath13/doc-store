from elasticsearch import Elasticsearch
from pprint import pprint

_esClient = None

def init_elasticsearch():
    global _esClient
    if _esClient is None:
        _esClient = Elasticsearch('http://localhost:9200')
    index_name = 'pdf_docs'
    if not _esClient.indices.exists(index=index_name):
        mapping = {
                    "properties": {
                        "text": { "type" : "text" },
                        "pageNumber": { "type" : "long"},
                        "filePath": { "type" : "text" }
                    }
                }
        _esClient.indices.create(index='pdf_docs', settings={
            'index': {
                'number_of_replicas' : 3
            }
        }, mappings= mapping)

def index_pages(pages, filePath):
    documents = []
    for page_number, text in pages.items():
        # print('indexing page no: ' + str(page_number))
        doc = {
            'pageNumber': page_number,
            'text': text,
            'filePath': filePath
        }
        documents.append(doc)
        response = _esClient.index(index='pdf_docs', document=doc)
    return documents