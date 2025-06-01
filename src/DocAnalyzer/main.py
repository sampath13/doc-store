from elasticsearch import Elasticsearch
from Reader.text_reader import extract_text
from pprint import pprint
import time
from search_engine import init_elasticsearch, index_pages

filePath = './Data/ProNETMemoryManagement.pdf'

start = time.perf_counter()
extracted_text = extract_text(filePath)
end = time.perf_counter()
print(f'Elapsed time to read 1000 page document: {end - start:.4f} seconds')

init_elasticsearch()
start = time.perf_counter()
documents = index_pages(extracted_text, filePath)
end = time.perf_counter()
print(f'Elapsed time to index 1000 page document: {end - start:.4f} seconds')
# pprint(documents)

