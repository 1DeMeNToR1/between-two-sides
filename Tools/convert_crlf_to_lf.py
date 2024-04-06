import os

def convert_crlf_to_lf(filepath):
    # Конвертирует содержимое файла из CRLF в LF.
    with open(filepath, "rb") as f:
        data = f.read()
    data = data.replace(b"\r\n", b"\n")
    with open(filepath, "wb") as f:
        f.write(data)

def main():
    target_dir = "./Resources/Textures/"

    for root, dirs, files in os.walk(target_dir):
        for filename in files:
            filepath = os.path.join(root, filename)

            if filename.endswith(".json"):
                convert_crlf_to_lf(filepath)

if __name__ == "__main__":
    main()
