import fitz

def extract_text(pdf_path):
    doc = fitz.open(pdf_path)
    text_map = { }
    page_number = 1
    for page in doc:
        page_text = page.get_text()
        text_map[page_number] = page_text
        page_number += 1
    print('number of pages in doc:' + str(page_number - 1))
    return text_map