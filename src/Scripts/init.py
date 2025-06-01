#!/usr/bin/env python3

import requests
import sys

ELASTIC_URL = "http://192.168.1.100:32000"
INDEX_NAME = "idx_documents"

def index_exists(url, index):
    resp = requests.head(f"{url}/{index}")
    return resp.status_code == 200

def create_index(url, index):
    resp = requests.put(f"{url}/{index}")
    if resp.status_code == 200 or resp.status_code == 201:
        print(f"Index '{index}' created successfully.")
    else:
        print(f"Failed to create index '{index}': {resp.text}")
        sys.exit(1)

if __name__ == "__main__":
    if index_exists(ELASTIC_URL, INDEX_NAME):
        print(f"Index '{INDEX_NAME}' already exists.")
    else:
        create_index(ELASTIC_URL, INDEX_NAME)